using PrDCOldApp.Web.Controllers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace PrDCOldApp.Web
{
    public class IOWrapper
    {
        internal static string MapPath(string uploadsFolder)
        {
            return HttpContext.Current.Server.MapPath(uploadsFolder);
        }

        internal static void DeleteFileFromUploads(string file)
        {
            File.Delete(MapPath(Configurations.UploadsFolder + file));
        }
    }
}