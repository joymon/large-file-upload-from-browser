using PrDCOldApp.Web.Common;
using PrDCOldApp.Web.Controllers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using System.Web;

namespace PrDCOldApp.Web
{
    [ServiceContract(Namespace = "")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class FilesService
    {
        // To create an operation that returns XML,
        //     add [WebGet(ResponseFormat=WebMessageFormat.Xml)],
        //     and include the following line in the operation body:
        //         WebOperationContext.Current.OutgoingResponse.ContentType = "text/xml";
        [OperationContract]
        [WebInvoke(UriTemplate = "/files", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        public Guid CreateFile()
        {
            Guid newGuid = Guid.NewGuid();
            FileManager.CreateFolderInUploads(newGuid);
            return newGuid;
        }
        [OperationContract]
        [WebInvoke(UriTemplate = "/files/chunk", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        public void UploadChunk(Stream data)
        {
            var mode = HttpContext.Current.Request.ReadEntityBodyMode;
            MultipartParser parser = new MultipartParser(data);
            
            if (parser.Success)
            {
                // Save the file
                string filename = parser.Filename;
                string contentType = parser.ContentType;
                byte[] filecontent = parser.FileContents;
                string physicalFilePath = Path.Combine(Configurations.UploadsFolder, filename);
                File.WriteAllBytes(physicalFilePath, filecontent);
            }
        }
        /// <summary>
        /// Merges the file.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="name">The name.</param>
        /// <remarks>The ending / is important otherwise the routing may not work properyly</remarks>
        [OperationContract]
        [WebInvoke(UriTemplate = "/files/{id}/{name}/", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        public void MergeFile(string id, string name)
        {
            FileMerger merger = new FileMerger();
            merger.Merge(Path.Combine(Configurations.UploadsFolder, id.ToString()), name);
        }
    }
}
