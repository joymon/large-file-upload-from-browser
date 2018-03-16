using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PrDCOldApp.Web.Common;

namespace LargeFileUploader.Tests
{
    [TestClass]
    public class FileMerger_Merge
    {
        [TestMethod]
        public void TestMethod1()
        {
            FileMerger m = new FileMerger();
            m.Merge(@"C:\Source\Repos\large-file-upload-from-browser\LargeFileUpload.Web\Content\Uploads\a3cedb61-0fcc-47e0-b05c-059139e07965",
                "PRD_Standardization.docx");
            Assert.IsTrue(true);
        }
    }
}
