using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EhentaiDownloader.Parsers;
using EhentaiDownloader.Models;
using EhentaiDownloader.Tools;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;

namespace Download.Tests
{
    [TestClass]
    public class ShzxParserTest
    {
        [TestMethod]
        public async Task TestMethod1()
        {
            //Arrange
            ShzxParser parser = new ShzxParser();
            //
            string html = File.ReadAllText("./TestHtmls/HTMLPage1.html");
            //Console.WriteLine(html);
            //Assert.IsTrue(html.Length > 10);
            List<ImageModel> imageModels =await parser.test(html);
            Assert.IsTrue(imageModels.Count ==2);
        }

        
    }
}
