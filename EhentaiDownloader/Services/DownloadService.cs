using AngleSharp;
using EhentaiDownloader.Delegates;
using EhentaiDownloader.Models;
using EhentaiDownloader.Parsers;
using EhentaiDownloader.Tools;
using EhentaiDownloader.Views;
using Microsoft.WindowsAPICodePack.Shell.Interop;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace EhentaiDownloader.Services
{
    class DownloadService
    {
        public static List<ImageModel> DownloadFails { get; set; } = new List<ImageModel>();
        public static List<ImageModel> DownloadFinish { get; set; } = new List<ImageModel>();
        private static IWebpageParser webpageParser;

        public static async Task StartDownload(ObservableCollection<TaskItem> taskItems)
        {
            DownloadFails.Clear();
            DownloadFinish.Clear();
            for (int i = 0; i < taskItems.Count; i++)
            {
                taskItems[i].Status = "Downloading";
                taskItems[i].NumberOfFinish = 0;
                await downloadATask(taskItems[i]);
                taskItems[i].Status = "Download Finish";
            }
            new Window_FinishResult().Show();
        }
        /// <summary>
        /// 下载一个TaskItem，根据传入的Url自动匹配解析器
        /// </summary>
        /// <param name="taskItem"></param>
        /// <returns></returns>
        private static async Task downloadATask(TaskItem taskItem)
        {
            if (taskItem.Url.Contains("e-hentai.org"))
            {
                webpageParser = new EHentaiParser();
            }
            else if (taskItem.Url.Contains("asmhentai.com"))
            {
                webpageParser = new AsmHentaiParser();
            }
            else if (taskItem.Url.Contains("shzx.org"))
            {
                webpageParser = new ShzxParser();
            }
            else
            {
                taskItem.Status = "Invalid Url";
                return;
            }

            List<string> imagePageUrls = await webpageParser.FindImagePageUrl(taskItem.Url);
            taskItem.NumberOfFiles = imagePageUrls.Count();
            if (imagePageUrls.Count() == 0)
            {
                taskItem.Status = "No ImagePage Found";
                return;
            }
            Debug.WriteLine("成功：" + "共找到Image页面:" + imagePageUrls.Count());

            List<ImageModel> imageModelList=new List<ImageModel>();
            foreach(string imagePageUrl in imagePageUrls)
            {
                imageModelList.Add(new ImageModel {ImagePageUrl=imagePageUrl,TaskItem=taskItem });
            }
            await downloadFromImagePageParallel(imageModelList);
        }

        /// <summary>
        /// 从网页中查找图片链接并下载到本地（异步）
        /// </summary>
        /// <param name="imagePageUrls">一组网页的Url</param>
        /// <param name="taskItem">该组网页所属的TaskItem</param>
        /// <returns>Task</returns>
        private static async Task downloadFromImagePageParallel(List<ImageModel> imageList)
        {
            List<Task> taskList = new List<Task>();
            var limitation = new SemaphoreSlim(initialCount: 10, maxCount: 10);
            foreach (ImageModel imageModel in imageList)
            {
                ImageModel imageModelTemp = imageModel;
                await limitation.WaitAsync();
                //把Task添加到线程池中
                taskList.Add(Task.Run(async () =>
                {
                    try
                    {
                        imageModelTemp = await webpageParser.FindImageUrl(imageModelTemp);
                        await downloadAImageAndSave(imageModelTemp);

                        imageModelTemp.TaskItem.NumberOfFinish++;
                        DownloadFinish.Add(imageModelTemp);
                        DelegateCommands.SetImageDownloadCountCommand?.Invoke(DownloadFinish.Count());

                    }
                    catch (Exception e)
                    {
                        DownloadFails.Add(imageModelTemp);
                        DelegateCommands.SetImageDownloadFailCountCommand?.Invoke(DownloadFails.Count());
                    }
                    finally
                    {
                        limitation.Release();
                    }
                }));
            }
            //等待所有Task完成
            await Task.WhenAll(taskList);
        }

        /// <summary>
        /// 下载一个ImageModel并写入到本地(异步)
        /// </summary>
        /// <param name="imageModel"></param>
        /// <returns>Task</returns>
        private static async Task downloadAImageAndSave(ImageModel imageModel)
        {
            try
            {
                if (imageModel.ImageUrl != null)
                {
                    Byte[] imageBytes = await HttpDownloader.DownloadBytes(imageModel.ImageUrl);
                    FileWriter.WriteToFile(imageBytes, imageModel.ImageSavePath);
                }
            }
            catch (TaskCanceledException e)
            {
                Debug.WriteLine("失败：" + "下载二进制文件超时。" + imageModel.ImageUrl);
                throw e;
            }
            catch (Exception e)
            {
                Debug.WriteLine("失败：" + imageModel.ImageUrl + " 下载失败，原因：" + e.Message);
                throw e;
            }
        }

        /// <summary>
        /// 下载结束后重新尝试下载失败的图片（异步）
        /// </summary>
        /// <returns>Task</returns>
        public static async Task ReTry()
        {
            Debug.WriteLine("成功：" + "开始Retry");
            List<ImageModel> failsList = new List<ImageModel>();
            DownloadFails.ForEach(img => failsList.Add(img));

            DownloadFails.Clear();
            DelegateCommands.SetImageDownloadFailCountCommand?.Invoke(DownloadFails.Count());
            await downloadFromImagePageParallel(failsList);
            new Window_FinishResult().Show();
        }
    }
}
