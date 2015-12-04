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
            MultiThreadResizerClass.Worker.AddListOfImages(ImagesForWorker);

            await MultiThreadResizerClass.Worker.StartResizingTask(600);

            result.ForEach(workermodel => {
                MultiThreadResizerClass.Worker.ListOfFileAndCustomResizeSettings.Keys
                .Where(f => f.FileSource == Server.MapPath(workermodel.OriginalFile.filePath)).Select(f=> f.FileName).ToList().ForEach(FileName =>
                {
                    if (FileName.Contains("_thumb"))
                    {
                        workermodel.PreviewFile = new ImageModel()
                        {
                            fileName = FileName,
                            filePath = Url.Content("~/Images/" + FileName)
                        };
                    }
                    else
                    {
                        workermodel.Files.Add(new ImageModel()
                        {
                            fileName = FileName,
                            filePath = Url.Content("~/Images/" + FileName)
                        });
                    }

                });
            });


            return Json(result);
        }
        [HttpPost]
        public JsonResult AddSetting(ResizeSettingsModel model)
        {
            MultiThreadResizerClass.Worker.ListOfResizeSettings.Add(new CustomResizeSettings(model.Name, model.Width, model.Height));
            return Json(model);
        }
        [HttpPost]
        public JsonResult SetThreadCount(ResizeSettingsModel model)
        {
            MultiThreadResizerClass.Worker.ListOfResizeSettings.Add(new CustomResizeSettings(model.Name, model.Width, model.Height));
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