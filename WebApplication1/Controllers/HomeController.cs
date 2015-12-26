using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using WebApplication1.Models;
using System.Net.Http;
using System.IO;
using System.Net.Http.Headers;
using MultiThreadResizer;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<JsonResult> Upload()
        {
            MultiThreadResizerWorker worker;
            var uuid = Request["uuid"];
            if (!string.IsNullOrEmpty(uuid))
            {
                worker = workerClass.GetWorker(uuid);
            }
            else
            {
                throw new Exception("uuid is not passed");
            }

            var userImages = new List<Image>();
            List<string> ImagesForWorker = new List<string>();
            foreach (string file in Request.Files)
            {
                var upload = Request.Files[file];
                var userImage = new Image();
                if (upload != null)
                {
                    // получаем имя файла
                    string fileName = Path.GetFileName(upload.FileName);
                    // сохраняем файл в папку Files в проекте
                    userImage.FilePath = Url.Content("~/Images/" + fileName);
                    userImage.FileName = fileName;
                    upload.SaveAs(Server.MapPath(userImage.FilePath));
                    using (var img = System.Drawing.Image.FromFile(Server.MapPath(userImage.FilePath)))
                    {
                        userImage.Height = img.Height;
                        userImage.Width = img.Width;
                    }
                    ImagesForWorker.Add(Server.MapPath(userImage.FilePath));
                }
                userImages.Add(userImage);
            }
            worker.AddListOfImages(ImagesForWorker);

            var ResizingTask = worker.StartResizingTask(600);

            using (var db = new ImageContext())
            {
                for (int i = 0; i < userImages.Count; i++)
                {
                    Image img = userImages[i];
                    try
                    {
                        db.Images.Add(img);
                        db.SaveChanges();
                    }
                    catch (Exception exe)
                    {
                        userImages.Remove(img);
                    }
                }                
            }

            await ResizingTask;

            var keys = worker.ListOfFileAndCustomResizeSettings.Keys.ToList();

            userImages.ForEach(userImage =>
            {
                userImage.PreviewPath = Url.Content("~/Images/" + keys.FirstOrDefault(f => 
                    f.FileSource == Server.MapPath(userImage.FilePath) 
                    && f.FileName.Contains(workerClass.PreviewName)
                )?.FileName??"");

                keys.Where(f => 
                f.FileSource == Server.MapPath(userImage.FilePath)
                && !f.FileName.Contains(workerClass.PreviewName)).ToList().ForEach(key =>
                 {
                     using (var db = new ImageContext())
                     {
                         StatusOfImage localStatus;

                         worker.ListOfFileAndCustomResizeSettings.TryGetValue(key, out localStatus);

                         var ResizedImage = new ResizedImage()
                         {
                             ParentId = userImage.Id,
                             FileName = key.FileName,
                             FilePath = Url.Content("~/Images/" + key.FileName),
                             PreviewPath = userImage.PreviewPath,
                             Height = key.CustomResizeSetting.Height.GetValueOrDefault(),
                             Width = key.CustomResizeSetting.Width.GetValueOrDefault(),
                             StartTime = localStatus.StartTime,
                             FinishTime = localStatus.FinishTime
                         };

                        try
                        {
                            db.ResizedImages.Add(ResizedImage);
                            db.SaveChanges();
                        }
                        catch (Exception exe)
                        {

                        }

                        localStatus = null;
                     }
                 });
            });

            return Json(userImages);
        }
        [HttpPost]
        public JsonResult AddSetting(ResizeSettingsModel model)
        {
            var uuid = Request["uuid"];
            if (!string.IsNullOrEmpty(uuid))
            {
                workerClass.GetWorker(uuid).ListOfResizeSettings.Add(new CustomResizeSettings(model.Name, model.Width, model.Height));
            }
            else
            {
                throw new Exception("uuid is not passed");
            }
            return Json(model);
        }
        [HttpPost]
        public JsonResult SetThreadCount(ResizeSettingsModel model)
        {
            var uuid = Request["uuid"];
            if (!string.IsNullOrEmpty(uuid))
            {
                workerClass.GetWorker(uuid).ListOfResizeSettings.Add(
                    new CustomResizeSettings(model.Name, model.Width, model.Height));
            }
            else
            {
                throw new Exception("uuid is not passed");
            }
            return Json(model);
        }

        public HttpResponseMessage GetPreview(string filePath)
        {
            // resize

            var fileStream = new FileStream(Server.MapPath(filePath), FileMode.Open);
            var resp = new HttpResponseMessage()
            {
                Content = new StreamContent(fileStream)
            };
            fileStream.Dispose();
            // Find the MIME type
            string mimeType = Path.GetExtension(filePath).Replace(".", "image/");
            resp.Content.Headers.ContentType = new MediaTypeHeaderValue(mimeType);

            return resp;
        }


    }
}