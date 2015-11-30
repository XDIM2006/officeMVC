﻿using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using System;
using System.Net.Http;
using System.Net;

namespace WebApplication1.Models
{
    public class old
    {
        [Display(Name = "URL")]
        public string Site { get; set; }
        public Guid ID { get; set; }
        public string Content { get; set; }
    }
    public class TaskModel
    {
        [Display(Name = "URL")]
        public string Site { get; set; }
        public Guid ID { get; set; }
        public WebClient client { get; set; }
        public TaskModel(old model)
        {
            Site = model.Site;
            ID = model.ID;
        }
    }
    public class Downloader
    {
        private static ConcurrentDictionary<Guid, TaskModel> DownloaderManager 
            = new ConcurrentDictionary<Guid, TaskModel>();
       
        public static void CreateNew(old model)
        {
            model.ID = Guid.NewGuid();
            var taskmodel = new TaskModel(model);
            taskmodel.client = new WebClient();
            DownloaderManager.TryAdd(model.ID, taskmodel);
        }

        public static async Task<string> StartNew(Guid Id)
        {
            TaskModel taskmodel;
            DownloaderManager.TryGetValue(Id, out taskmodel);
            return await taskmodel.client.DownloadStringTaskAsync(new Uri(taskmodel.Site));
        } 

        public static old Cancel(old model)
        {
            TaskModel taskmodel;
            DownloaderManager.TryGetValue(model.ID, out taskmodel);
            try
            {
                taskmodel.client.CancelAsync();
                model.Content = "Canceled";
            }
            catch (Exception ex)
            {
                model.Content = ex.Message;
            }
            return model;
        }

    }
}