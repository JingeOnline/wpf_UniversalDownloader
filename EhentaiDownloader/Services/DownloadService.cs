using AngleSharp;
using EhentaiDownloader.Delegates;
using EhentaiDownloader.Models;
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
        //设置UI上的ImageDownloadCount
        public static Action<int> SetImageDownloadCount_Delegate;
        //设置UI上的ImageDownloadFailCount
        public static Action<int> SetImageDownloadFailCount_Delegate;
        private static Func<string, Task<List<string>>> func_findImagePageUrl;
        private static Func<string, Task<ImageModel>> func_findImageUrl;

        public static async Task StartDownload(ObservableCollection<TaskItem> taskItems)
        {
            for (int i = 0; i < taskItems.Count; i++)
            {
                taskItems[i].Status = "Downloading";
                taskItems[i].NumberOfFinish = 0;
                await downloadATask(taskItems[i]);
                taskItems[i].Status = "Download Finish";
            }
            new Window_FinishResult().Show();

            //await test();
        }

        //private static async Task test()
        //{
        //    string link = "https://asmhentai.com/gallery/305541/181/";
        //    await findImageUrlInPageAndSaveToFile_AsmHentai(link);
        //    Debug.WriteLine("Test Finish");
        //}


        private static async Task downloadATask(TaskItem taskItem)
        {
            if (taskItem.Url.Contains("e-hentai.org"))
            {
                func_findImagePageUrl = (url) => EHentaiParser.FindImagePageLink(url);
                func_findImageUrl = (url) => EHentaiParser.FindImageUrl(url);
            }
            else if (taskItem.Url.Contains("asmhentai.com"))
            {
                func_findImagePageUrl = (url) => AsmHentaiParser.FindImagePageLink(url);
                func_findImageUrl = (url) => AsmHentaiParser.FindImageUrl(url);
            }
            else
            {
                taskItem.Status = "Invalid Url";
                return;
            }

            List<string> imagePageUrls = await func_findImagePageUrl(taskItem.Url);
            taskItem.NumberOfFiles = imagePageUrls.Count();
            Debug.WriteLine("成功：" + "共找到Image页面:" + imagePageUrls.Count());
            await downloadFromImagePageParallel(imagePageUrls, taskItem);
        }

        private static async Task downloadFromImagePageParallel(List<string> imagePageUrls, TaskItem taskItem = null)
        {
            List<Task> taskList = new List<Task>();
            var limitation = new SemaphoreSlim(initialCount: 10, maxCount: 10);
            foreach (string url in imagePageUrls)
            {
                await limitation.WaitAsync();
                //把Task添加到线程池中
                taskList.Add(Task.Run(async () =>
                {
                    ImageModel imageModel = new ImageModel { ImageName = "未找到图片链接" };
                    try
                    {
                        imageModel = await func_findImageUrl.Invoke(url);
                        await downloadAImageAndSave(imageModel);
                        if (taskItem != null)
                        {
                            taskItem.NumberOfFinish++;
                            SetImageDownloadCount_Delegate?.Invoke(DownloadFinish.Count());
                        }
                    }
                    catch (Exception e)
                    {
                        DownloadFails.Add(imageModel);
                        SetImageDownloadFailCount_Delegate?.Invoke(DownloadFails.Count());
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


        private static async Task downloadAImageAndSave(ImageModel image)
        {
            try
            {
                if (image.ImageUrl != null)
                {
                    Byte[] imageBytes = await HttpDownloader.DownloadBytes(image.ImageUrl);
                    FileWriter.WriteToFile(imageBytes, image.ImageSavePath);
                }
            }
            catch (TaskCanceledException e)
            {
                Debug.WriteLine("失败：" + "下载二进制文件超时。" + image.ImageUrl);
                throw e;
            }
            catch (Exception e)
            {
                Debug.WriteLine("失败：" + image.ImageUrl + " 下载失败，原因：" + e.Message);
                throw e;
            }
        }

        public static async Task ReTry()
        {
            Debug.WriteLine("成功：" + "开始Retry");
            List<string> failsPages = new List<string>();
            //DownloadFails.ForEach(i => fails.Add(i));
            foreach (ImageModel image in DownloadFails)
            {
                failsPages.Add(image.ImagePageUrl);
            }
            DownloadFails.Clear();
            await downloadFromImagePageParallel(failsPages);
            new Window_FinishResult().Show();
        }
    }
}
