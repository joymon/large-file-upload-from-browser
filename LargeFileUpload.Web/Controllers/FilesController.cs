using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace PrDCOldApp.Web.Controllers
{
    /// <summary>
    /// Used for receiving chunks via JSON payload.
    /// </summary>
    public class FilesController : ApiController
    {
        // GET api/<controller>
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        public void Post([FromBody]string value)
        {
        }

        [HttpPost]
        [Route("api/files/{id:Guid}/{fileName}")]
        public string MultiUpload(Guid id,string fileName)
        {
            var chunks = HttpContext.Current.Request.InputStream;

            string physicalFilePath = Path.Combine(Configurations.UploadsFolder, id.ToString(), fileName);

            using (System.IO.FileStream fs = System.IO.File.Create(physicalFilePath))
            {
                chunks.CopyTo(fs);
                //byte[] bytes = new byte[77570];

                //int bytesRead;
                //while ((bytesRead = Request.InputStream.Read(bytes, 0, bytes.Length)) > 0)
                //{
                //    fs.Write(bytes, 0, bytesRead);
                //}
            }
            return "ok";
        }
        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}