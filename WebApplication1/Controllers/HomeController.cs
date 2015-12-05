using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using WebApplication1.Models;
using System.Net.Http;
using System.IO;
using System.Net.Http.Headers;
using System.Net;
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

            var result = new List<WorkModel>();
            List<string> ImagesForWorker = new List<string>();
            foreach (string file in Request.Files)
            {
                var upload = Request.Files[file];
                var workModel = new WorkModel();
                if (upload != null)
                {
                    workModel.OriginalFile = new ImageModel();
                    // получаем имя файла
                    string fileName = Path.GetFileName(upload.FileName);
                    // сохраняем файл в папку Files в проекте
                    workModel.OriginalFile.filePath = Url.Content("~/Images/" + fileName);
                    workModel.OriginalFile.fileName = fileName;
                    upload.SaveAs(Server.MapPath(workModel.OriginalFile.filePath));
                    ImagesForWorker.Add(Server.MapPath(workModel.OriginalFile.filePath));
                }
                result.Add(workModel);
            }
            worker.AddListOfImages(ImagesForWorker);

            await worker.StartResizingTask(600);

            result.ForEach(workermodel =>
            {
                worker.ListOfFileAndCustomResizeSettings.Keys
                .Where(f => f.FileSource == Server.MapPath(workermodel.OriginalFile.filePath)).ToList().ForEach(key =>
                 {
                     StatusOfImage localStatus;
                     worker.ListOfFileAndCustomResizeSettings.TryGetValue(key, out localStatus);
                     
                     if (key.FileName.Contains(workerClass.PreviewName))
                     {
                         workermodel.PreviewFile = new ImageModel(localStatus)
                         {
                             fileName = key.FileName,
                             filePath = Url.Content("~/Images/" + key.FileName),
                             
                         };
                     }
                     else
                     {
                         workermodel.Files.Add(new ImageModel(localStatus)
                         {
                             fileName = key.FileName,
                             filePath = Url.Content("~/Images/" + key.FileName)
                         });
                     }
                     localStatus = null;
                 });
            });


            return Json(result);
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