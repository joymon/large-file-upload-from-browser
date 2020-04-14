using PrDCOldApp.Web.Controllers;
using System;
using System.IO;

namespace PrDCOldApp.Web.Common
{

    public class FileManager
    {
        internal static void CreateFolderInUploads(Guid newGuid)
        {
            var path = Path.Combine(Configurations.UploadsFolder, newGuid.ToString());
            IOWrapper.CreateFolderIfNotExists(path);
        }
    }
}