using PrDCOldApp.Web.Controllers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PrDCOldApp.Web.Common
{
    public class FileMerger
    {
        private static void CombineMultipleFilesIntoSingleFile(string inputDirectoryPath,
            string outputFilePath)
        {
            IEnumerable<string> inputFilePaths = Directory.GetFiles(inputDirectoryPath, "*.*")
                .OrderBy(path=> int.Parse( Path.GetFileName(path )))
                .Select(a => a.ToString());

            //Console.WriteLine("Number of files: {0}.", inputFilePaths.Length);
            using (var outputStream = File.Create(Path.Combine(inputDirectoryPath, outputFilePath)))
            {
                foreach (var inputFilePath in inputFilePaths)
                {
                    using (var inputStream = File.OpenRead(inputFilePath))
                    {
                        // Buffer size can be passed as the second argument.
                        inputStream.CopyTo(outputStream);
                    }
                    File.Delete(inputFilePath);
                    Console.WriteLine("The file {0} has been processed.", inputFilePath);
                }
            }
        }

        public void Merge(string physicalFolderPath, string fileName)
        {
            CombineMultipleFilesIntoSingleFile(physicalFolderPath, fileName);
        }
    }
}