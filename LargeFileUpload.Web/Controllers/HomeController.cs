using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.WebSockets;
using System.Security.AccessControl;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using Newtonsoft.Json;
using PrDCOldApp.Web.Models;

namespace PrDCOldApp.Web.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/
        public ActionResult Index()
        {
            List<ImageEntry> model = new List<ImageEntry>();
            foreach (FileInfo f in new DirectoryInfo(Configurations.UploadsFolder).GetFiles())
            {
                model.Add(new ImageEntry() { Path = f.Name, Published = f.CreationTimeUtc });
            }
            return View(model);
        }

        public ActionResult Upload()
        {
            return View();
        }
        public ActionResult UploadWebAPI()
        {
            return View();
        }
        public ActionResult Delete(string id)
        {

            IOWrapper.DeleteFileFromUploads(id);
            return RedirectToAction("Index");
        }
        const string uploadsPath = "/Content/Uploads/";
        public static string DataUrlPrefix = "data:image/png;base64,";

        [HttpPost]
        internal ActionResult Upload(UploadEntry entry)
        {
            if (entry.UploadFile != null && entry.UploadFile.ContentLength > 0)
            {
                var path = uploadsPath + Path.GetRandomFileName();
                path = Path.ChangeExtension(path, Path.GetExtension(entry.UploadFile.FileName));
                entry.UploadFile.SaveAs(Server.MapPath(path));

                entry.UploadUrl = path;
            }
            else if (!string.IsNullOrEmpty(entry.CameraImage))
            {
                var path = uploadsPath + Path.GetRandomFileName();
                System.IO.File.WriteAllBytes(Server.MapPath(path), Convert.FromBase64String(entry.CameraImage.Substring(DataUrlPrefix.Length)));

                entry.UploadUrl = path;
            }

            new Database().SaveUploadEntryInDatabase(entry);

            return RedirectToAction("Index");
        }



        public ActionResult Image(int id)
        {
            //ImageEntry image;
            //using (var context = new ImageEntryContext())
            //{
            //    image = context.Images.Include(i => i.Comments).First(i => i.Id == id);
            //}
            //return View(image);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Image(int id, string username, string comment)
        {
            var c = new Comment() { Content = comment, User = username };
            ImageEntry image;
            using (var context = new ImageEntryContext())
            {
                image = context.Images.Include(i => i.Comments).First(i => i.Id == id);
                image.Comments.Add(c);
                context.SaveChanges();
            }
            return View(image);
        }

        public HttpStatusCodeResult SendData()
        {
            try
            {
                HttpContext.AcceptWebSocketRequest(HandleWebSocket);
                return new HttpStatusCodeResult(HttpStatusCode.SwitchingProtocols);
            }
            catch (PlatformNotSupportedException ex)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotImplemented);
            }
        }

        private async Task HandleWebSocket(WebSocketContext context)
        {
            var closed = false;
            var maxMessageSize = 2 * 1024 * 1024;
            var data = new byte[maxMessageSize];
            var socket = context.WebSocket;
            UploadEntry uploadModel = null;
            while (!closed)
            {
                try
                {
                    var receive = await socket.ReceiveAsync(new ArraySegment<byte>(data), CancellationToken.None);
                    if (receive.MessageType == WebSocketMessageType.Close)
                    {
                        new Database().SaveUploadEntryInDatabase(uploadModel);

                        await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                        closed = true;
                    }
                    else if (receive.MessageType == WebSocketMessageType.Binary && uploadModel != null)
                    {
                        using (var writer = new BinaryWriter(new FileStream(Server.MapPath("~/" + uploadModel.UploadUrl), FileMode.Append)))
                        {
                            writer.Write(data, 0, receive.Count);
                            await socket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes("{ \"received\": " + writer.BaseStream.Position + " }")),
                                WebSocketMessageType.Text, true, CancellationToken.None);
                        }
                    }
                    else
                    {
                        var receivedString = Encoding.UTF8.GetString(data, 0, receive.Count);
                        uploadModel = JsonConvert.DeserializeObject<UploadEntry>(receivedString);
                        var path = "/Content/Uploads/" + uploadModel.FileName;
                        //path = Path.ChangeExtension(path, "png");
                        uploadModel.UploadUrl = path;
                        
                        IOWrapper.DeleteFileFromUploads(uploadModel.FileName);
                        var output = new ArraySegment<byte>(Encoding.UTF8.GetBytes("{ \"accepted\": true }"));
                        await socket.SendAsync(output, WebSocketMessageType.Text, true, CancellationToken.None);
                    }
                }
                catch (Exception ex)
                {
                    if (socket != null)
                        await socket.CloseAsync(WebSocketCloseStatus.InternalServerError, ex.Message, CancellationToken.None);

                    closed = true;
                }
            }
        }
    }
}