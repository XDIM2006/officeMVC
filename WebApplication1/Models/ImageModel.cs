using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using System;
using System.Net.Http;
using System.Net;

namespace WebApplication1.Models
{
    public class ImageModel
    {
        public int MyProperty { get; set; }
        public string filePath { get; set; }
    }
}