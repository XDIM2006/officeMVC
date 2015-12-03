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

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Upload(SettingsModel model)
        {
            var result = new List<ImageModel>();
            foreach (string file in Request.Files)
            {
                var upload = Request.Files[file];
                if (upload != null)
                {
                    var img = new ImageModel();
                    // получаем имя файла
                    string fileName = System.IO.Path.GetFileName(upload.FileName);
                    // сохраняем файл в папку Files в проекте
                    img.filePath = "~/Images/" + fileName;
                    upload.SaveAs(Server.MapPath(img.filePath));
                    result.Add(img);
                }
            }
            return PartialView(result);
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