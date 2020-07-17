using AngleSharp;
using EhentaiDownloader.Delegates;
using EhentaiDownloader.Exceptions;
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
        public static int ParallelTaskNum { get; set; } = 10;
        public static List<ImageModel> DownloadFailImages { get; set; } = new List<ImageModel>();
        public static List<ImageModel> DownloadFinishImages { get; set; } = new List<ImageModel>();
        public static List<ImagePageModel> UnAvailablePages { get; set; } = new List<ImagePageModel>();
        private static IWebpageParser webpageParser;

        public static async Task StartDownload(ObservableCollection<TaskItem> taskItems)
        {
            DownloadFailImages.Clear();
            DownloadFinishImages.Clear();
            UnAvailablePages.Clear();
            for (int i = 0; i < taskItems.Count; i++)
            {
                await downloadATask(taskItems[i]);
            }
            SoundPlayer.PlayCompleteness();
            new Window_FinishResult().Show();
        }
        /// <summary>
        /// 下载一个TaskItem，根据传入的Url自动匹配解析器
        /// </summary>
        /// <param name="taskItem"></param>
        /// <returns></returns>
        private static async Task downloadATask(TaskItem taskItem)
        {
            taskItem.Status = "Downloading";
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

            List<ImagePageModel> imagePages = new List<ImagePageModel>();
            try
            {
                imagePages = await webpageParser.FindImagePageUrl(taskItem);
            }
            catch (TargetNotFindException)
            {
                taskItem.Status = "Can't find target element";
                return;
            }
            catch (TaskCanceledException e)
            {
                taskItem.Status = "Url Inaccessible";
                Debug.WriteLine("用户输入的链接无法访问" + taskItem.Url + "\n原因：" + e.Message);
                return;
            }
            if (imagePages.Count() == 0)
            {
                taskItem.Status = "None ImagePage";
                return;
            }
            Debug.WriteLine("成功：" + "共找到ImagePage页面:" + imagePages.Count());
            List<ImageModel> imageModels = await getAllImageUrls(imagePages);
            foreach (ImageModel i in imageModels)
            {
                if (i == null)
                {
                    Debug.WriteLine("!!!");
                    throw new NullReferenceException();
                }
                else
                {
                    Debug.WriteLine("准备下载" + i.ImageUrl);
                }
            }
            await downloadImagesParallel(imageModels);
            taskItem.Status = "Download Finish";
        }

        /// <summary>
        /// 获得所有图片页面的所有图片Url
        /// </summary>
        /// <param name="imagePageModels"></param>
        /// <returns></returns>
        private static async Task<List<ImageModel>> getAllImageUrls(List<ImagePageModel> imagePageModels)
        {
            List<Task> tasks = new List<Task>();
            var limitation = new SemaphoreSlim(ParallelTaskNum);
            List<ImageModel> imageModels = new List<ImageModel>();
            foreach (ImagePageModel imagePageModel in imagePageModels)
            {
                ImagePageModel imagePage = imagePageModel;
                await limitation.WaitAsync();
                tasks.Add(Task.Run(async () =>
                {
                    try
                    {
                        List<ImageModel> imagesInAPage = await webpageParser.FindImageUrls(imagePage);
                        //Debug.WriteLine($"在{imagePage.ImagePageUrl}中找到{imagesInAPage.Count()}个图片");
                        imagePage.TaskItem.NumberOfFiles += imagesInAPage.Count();
                        foreach(ImageModel i in imagesInAPage)
                        {
                            if (i == null) throw new NullReferenceException();
                        }
                        imageModels.AddRange(imagesInAPage);
                    }
                    catch (Exception e)
                    {
                        imagePage.FailMessage = e.Message;
                        UnAvailablePages.Add(imagePage);
                        DelegateCommands.SetUnavailableImagePageCountCommand?.Invoke(UnAvailablePages.Count());
                        Debug.WriteLine("getAllImageUrls(" + imagePage.ImagePageUrl + ")发生错误，" + e.Message);
                    }
                    finally
                    {
                        limitation.Release();
                    }
                }));
            }
            //等待所有Task完成
            await Task.WhenAll(tasks);
            Debug.WriteLine("共找到图片=" + imageModels.Count());
            return imageModels;
        }


        /// <summary>
        /// 从网页中查找图片链接并下载到本地（异步）
        /// </summary>
        /// <param name="imagePageUrls">一组网页的Url</param>
        /// <param name="taskItem">该组网页所属的TaskItem</param>
        /// <returns>Task</returns>
        private static async Task downloadImagesParallel(List<ImageModel> imageModelList)
        {

            List<Task> taskList = new List<Task>();
            var limitation = new SemaphoreSlim(ParallelTaskNum);
            foreach (ImageModel imageModel in imageModelList)
            {
                ImageModel imageModelTemp = imageModel;
                await limitation.WaitAsync();
                //Debug.WriteLine("准备下载" + imageModelTemp.ImageUrl);
                //把Task添加到线程池中
                taskList.Add(Task.Run(async () =>
                {
                    try
                    {
                        await downloadAImageAndSave(imageModelTemp);
                        imageModelTemp.ImagePage.TaskItem.NumberOfFinish++;
                        DownloadFinishImages.Add(imageModelTemp);
                        DelegateCommands.SetImageDownloadCountCommand?.Invoke(DownloadFinishImages.Count());
                    }
                    catch (Exception e)
                    {
                        DownloadFailImages.Add(imageModelTemp);
                        DelegateCommands.SetImageDownloadFailCountCommand?.Invoke(DownloadFailImages.Count());
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
        public static async Task ReTryAsync()
        {
            Debug.WriteLine("成功：" + "开始Retry");


            List<ImageModel> failsList = new List<ImageModel>();
            DownloadFailImages.ForEach(img => failsList.Add(img));
            DownloadFailImages.Clear();
            DelegateCommands.SetImageDownloadFailCountCommand?.Invoke(DownloadFailImages.Count());
            await downloadImagesParallel(failsList);

            if (UnAvailablePages.Count != 0)
            {
                List<ImagePageModel> failImagePages = new List<ImagePageModel>();
                UnAvailablePages.ForEach(page => failImagePages.Add(page));
                UnAvailablePages.Clear();
                DelegateCommands.SetUnavailableImagePageCountCommand?.Invoke(UnAvailablePages.Count());
                List<ImageModel> imageModels = await getAllImageUrls(failImagePages);
                await downloadImagesParallel(imageModels);
            }

            SoundPlayer.PlayCompleteness();
            new Window_FinishResult().Show();
        }
    }
}
