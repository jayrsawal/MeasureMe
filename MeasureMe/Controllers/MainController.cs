using MeasureMe.Common;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using MeasureMe.Code;

namespace MeasureMe.Controllers {
    public class MainController : ControllerTemplate {
        [AllowAnonymous]
        public ActionResult Error() {
            try {
                this.LogException(Server.GetLastError());
            } catch { }
            return View();
        }

        // GET: Main
        [AllowAnonymous]
        public ActionResult Index() {
            if (Request.IsAuthenticated) {
                return Home();
            }
            ViewBag.Title = "Login";
            return View("../Auth/Register");
        }

        public ActionResult Home() {
            if (TempData["Error"] != null) {
                ModelState.AddModelError("error", TempData["Error"].ToString());
            }
            if (TempData["Status"] != null) {
                ViewBag.Status = TempData["Status"].ToString();
            }
            Body body = new Body(db, this.user);
            ViewBag.Gender = this.user.Gender;
            ViewBag.Title = "Home";
            return View("Home", body.GetBodyModel());
        }
    }
}