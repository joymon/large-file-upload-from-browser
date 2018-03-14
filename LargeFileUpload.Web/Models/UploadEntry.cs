using System;
using System.Web;

namespace PrDCOldApp.Web.Controllers
{
    public class UploadEntry
    {
        public HttpPostedFileBase UploadFile { get; set; }
        public string UploadUrl { get; set; }
        public string EmailAddress { get; set; }
        public DateTime? DatePublished { get; set; }
        public string Location { get; set; }
        public string CameraImage { get; set; }
        public string FileName { get; set; }
    }
}