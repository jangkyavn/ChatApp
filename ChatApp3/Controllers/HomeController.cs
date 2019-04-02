using ChatApp3.Models;
using System.Linq;
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
    }
}