using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class Image
    {
        public int Id { get; set; }
        public int ParentId { get; set; }
        public string filePath { get; set; }
        public string fileName { get; set; }
        public DateTime StartTime { get; }
        public DateTime FinishTime { get; }
        public int Width { get; set; }
        public int Height { get; set; }
    }
}