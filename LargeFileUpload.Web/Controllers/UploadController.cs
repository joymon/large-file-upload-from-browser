using PrDCOldApp.Web.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace PrDCOldApp.Web.Controllers
{
    public class UploadController : ApiController
    {
        // GET api/<controller>
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
        //public string Get(int id)
        //{
        //    return "value";
        //}

        // POST api/<controller>
        public void Post([FromBody]string value)
        {
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }

        [HttpPost]
        [Route("upload/{targetFolder:int}")]
        public async Task<IHttpActionResult> UploadDocument(int targetFolder)
        {
            return Ok(HttpStatusCode.Continue);
        }
        [HttpGet]
        [Route("api/upload/new")]
        public Guid UploadNew()
        {
            Guid newGuid = Guid.NewGuid();
            FileManager.CreateFolderInUploads(newGuid);
            return newGuid;
        }

        [HttpPost]
        [Route("api/upload/{id:Guid}")]
        public HttpResponseMessage UploadFile(Guid id)
        {
            foreach (string file in HttpContext.Current.Request.Files)
            {
                var FileDataContent = HttpContext.Current.Request.Files[file];
                if (FileDataContent != null && FileDataContent.ContentLength > 0)
                {
                    // take the input stream, and save it to a temp folder using
                    // the original file.part name posted
                    var stream = FileDataContent.InputStream;
                    var fileName = Path.GetFileName(FileDataContent.FileName);
                    var UploadsPhysicalPath = Configurations.UploadsFolder;
                    string path = Path.Combine(UploadsPhysicalPath, id.ToString(), fileName);
                    try
                    {
                        if (System.IO.File.Exists(path))
                            System.IO.File.Delete(path);
                        using (var fileStream = System.IO.File.Create(path))
                        {
                            stream.CopyTo(fileStream);
                        }
                        // Once the file part is saved, see if we have enough to merge it
                        //Shared.Utils UT = new Shared.Utils();
                        //UT.MergeFile(path);
                    }
                    catch (IOException ex)
                    {
                        // handle
                    }
                }
            }
            return new HttpResponseMessage()
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Content = new StringContent("File uploaded.")
            };
        }
        /// <summary>
        /// original name + ".part_N.X" (N = file part number, X = total files)
        /// Objective = enumerate files in folder, look for all matching parts of
        /// split file. If found, merge and return true.
        /// </summary>
        /// <param name="FileName"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/upload/merge/{id:Guid}/{name}")]
        public bool MergeFile(Guid  id, string name)
        {
            FileMerger merger = new FileMerger();
            merger.Merge(Path.Combine(Configurations.UploadsFolder, id.ToString()), name);
            return true;
        }
        //    bool rslt = false;
        //    parse out the different tokens from the filename according to the convention
        //    string partToken = ".part_";
        //    string baseFileName = FileName.Substring(0, FileName.IndexOf(partToken));
        //    string trailingTokens = FileName.Substring(FileName.IndexOf(partToken) + partToken.Length);
        //    int FileIndex = 0;
        //    int FileCount = 0;
        //    int.TryParse(trailingTokens.Substring(0, trailingTokens.IndexOf(".")), out FileIndex);
        //    int.TryParse(trailingTokens.Substring(trailingTokens.IndexOf(".") + 1), out FileCount);
        //    get a list of all file parts in the temp folder
        //    string Searchpattern = Path.GetFileName(baseFileName) + partToken + "*";
        //    string[] FilesList = Directory.GetFiles(Path.GetDirectoryName(FileName), Searchpattern);
        //    merge..improvement would be to confirm individual parts are there / correctly in
        //     sequence, a security check would also be important
        //     only proceed if we have received all the file chunks
        //    if (FilesList.Count() == FileCount)
        //    {
        //        use a singleton to stop overlapping processes
        //        if (!MergeFileManager.Instance.InUse(baseFileName))
        //        {
        //            MergeFileManager.Instance.AddFile(baseFileName);
        //            if (File.Exists(baseFileName))
        //                File.Delete(baseFileName);
        //            add each file located to a list so we can get them into
        //            the correct order for rebuilding the file

        //           List < SortedFile > MergeList = new List<SortedFile>();
        //            foreach (string File in FilesList)
        //            {
        //                SortedFile sFile = new SortedFile();
        //                sFile.FileName = File;
        //                baseFileName = File.Substring(0, File.IndexOf(partToken));
        //                trailingTokens = File.Substring(File.IndexOf(partToken) + partToken.Length);
        //                int.TryParse(trailingTokens.
        //                   Substring(0, trailingTokens.IndexOf(".")), out FileIndex);
        //                sFile.FileOrder = FileIndex;
        //                MergeList.Add(sFile);
        //            }
        //            sort by the file-part number to ensure we merge back in the correct order
        //           var MergeOrder = MergeList.OrderBy(s => s.FileOrder).ToList();
        //            using (FileStream FS = new FileStream(baseFileName, FileMode.Create))
        //            {
        //                merge each file chunk back into one contiguous file stream
        //                foreach (var chunk in MergeOrder)
        //                {
        //                    try
        //                    {
        //                        using (FileStream fileChunk =
        //                           new FileStream(chunk.FileName, FileMode.Open))
        //                        {
        //                            fileChunk.CopyTo(FS);
        //                        }
        //                    }
        //                    catch (IOException ex)
        //                    {
        //                        handle
        //                    }
        //                }
        //            }
        //            rslt = true;
        //            unlock the file from singleton
        //            MergeFileManager.Instance.RemoveFile(baseFileName);
        //        }
        //    }
        //    return rslt;
        //}
        #region Helpers
       
        #endregion
    }

}