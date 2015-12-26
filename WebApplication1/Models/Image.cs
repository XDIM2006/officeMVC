using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class Image
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string PreviewPath { get; set; }
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public Image()
        {

        }
    }
    public class ResizedImage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public string PreviewPath { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int ParentId { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? FinishTime { get; set; }
    }

    public class ResizedImageWithPreview
    {
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public string PreviewPath { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string StartTime { get; set; }
        public string FinishTime { get; set; }
        public ResizedImageWithPreview(ResizedImage r)
        {
            FilePath = r.FilePath;
            FileName = r.FileName;
            PreviewPath = r.PreviewPath;
            Width = r.Width;
            Height = r.Height;
            StartTime = r.StartTime.GetValueOrDefault().ToString("D");
            FinishTime = r.FinishTime.GetValueOrDefault().ToString("D");
        }
    }
}