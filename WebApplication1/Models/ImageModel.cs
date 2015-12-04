using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using MultiThreadResizer;
using System;

namespace WebApplication1.Models
{
    [Serializable]
    public class WorkModel
    {
        public ImageModel OriginalFile { get; set; }
        public ImageModel PreviewFile { get; set; }
        public List<ImageModel> Files { get; set; }
        public WorkModel()
        {
            Files = new List<ImageModel>();
        }
    }
    [Serializable]
    public class ImageModel
    {
        public string filePath { get; set; }
        public string fileName { get; set; }
    }

    public class CustomResizeSettingsModel
    {
        public static List<CustomResizeSettings> CustomResizeSettings { get; set; }
    }

    public class MultiThreadResizerClass
    {
        public static MultiThreadResizerWorker Worker = new MultiThreadResizerWorker(2,10);
    }

    public class ResizeSettingsModel
    {
        public string Name { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }

}