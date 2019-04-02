using ChatApp3.Hubs;
using ChatApp3.Models;
using Microsoft.AspNet.SignalR;
using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace ChatApp3.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(string userName)
        {
            FormsAuthentication.SetAuthCookie(userName, true);
            return Json(true);
        }

        [HttpGet]
        public ActionResult GetMessages(string receiver, int pageIndex)
        {
            using (var db = new DataContext())
            {
                var list = db.Messages
                    .Where(x => x.SenderName == User.Identity.Name && x.ReceiverName == receiver || 
                                x.SenderName == receiver && x.ReceiverName == User.Identity.Name)
                    .OrderByDescending(x => x.DateCreated)
                    .Skip((pageIndex - 1) * 20)
                    .Take(20).ToList();
                list.Reverse();
                return Json(list, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public ActionResult Upload(string receiverName)
        {
            if (Request.Files.Count > 0)
            {
                for (int i = 0; i < Request.Files.Count; i++)
                {
                    HttpPostedFileBase file = Request.Files[i];
                    var isImage = true;
                    if (file != null && file.ContentLength > 0)
                    {
                        if (!string.Equals(file.ContentType, "image/jpg", StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(file.ContentType, "image/jpeg", StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(file.ContentType, "image/pjpeg", StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(file.ContentType, "image/gif", StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(file.ContentType, "image/x-png", StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(file.ContentType, "image/png", StringComparison.OrdinalIgnoreCase))
                        {
                            isImage = false;
                        }

                        string fileName = Path.GetFileName(file.FileName);

                        using (var db = new DataContext())
                        {
                            bool isStartDate = false;

                            var messages = db.Messages
                                .Where(x => x.SenderName == User.Identity.Name && x.ReceiverName == receiverName ||
                                        x.ReceiverName == User.Identity.Name && x.SenderName == receiverName)
                                .OrderByDescending(x => x.DateCreated);
                            if (messages.Count() == 0)
                            {
                                isStartDate = true;
                            }

                            var getLastDate = messages.Select(x => x.DateCreated).FirstOrDefault();
                            if ((DateTime.Now - getLastDate).Days > 0)
                            {
                                isStartDate = true;
                            }

                            var message = new Message();
                            message.MessageID = Guid.NewGuid();
                            message.Content = fileName;
                            message.DateCreated = DateTime.Now;
                            message.SenderName = User.Identity.Name;
                            message.ReceiverName = receiverName;
                            message.IsStartDate = isStartDate;
                            message.ContentType = isImage ? "image" : "file";

                            db.Messages.Add(message);
                            db.SaveChanges();

                            file.SaveAs(Server.MapPath("~/Uploaded/" + message.Content));

                            var hubContext = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
                            hubContext.Clients.All.ReceiveMessage(message);
                        }
                    }
                }
            }

            return Json(Request.Files.Count);
        }
    }
}