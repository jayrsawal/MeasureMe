using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MeasureMe.Models;
using System.Security.Claims;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using MeasureMe.Common;
using MeasureMe.Auth;

namespace MeasureMe.Controllers {
    [AllowAnonymous]
    public class AuthController : ControllerTemplate {
        // GET: Auth
        public ActionResult Index() {
            return View();
        }

        [HttpGet]
        public ActionResult Login(string strReturnUrl) {
            var ctx = Request.GetOwinContext();
            var authManager = ctx.Authentication;

            authManager.SignOut("ApplicationCookie");

            LoginModel model = new LoginModel {
                ReturnUrl = strReturnUrl
            };
            ViewBag.Title = "Login";
            return View(model);
        }

        [HttpPost]
        public ActionResult Login(LoginModel model) {
            if (!ModelState.IsValid) {
                return View();
            }

            ClaimsIdentity identity = null;
            UserConfig user = new UserConfig(db);
            try {
                if (user.VerifyUser(model, out identity)) {
                    IOwinContext ctx = Request.GetOwinContext();
                    IAuthenticationManager authManager = ctx.Authentication;

                    authManager.SignIn(identity);

                    if (model.ReturnUrl != null) {
                        return View(GetRedirectUrl(model.ReturnUrl));
                    } else {
                        return RedirectToAction("Index", "Main");
                    }
                }
                ModelState.AddModelError("error", "Invalid username or password");
            } catch (Exception ex) {
                ModelState.AddModelError("error", ex.Message);
            }
            ViewBag.Title = "Login";
            // user authN failed
            return View();
        }

        [HttpGet]
        public ActionResult Register() {
            ViewBag.Title = "Register";
            return View();
        }

        [HttpPost]
        public ActionResult Register(RegisterModel model) {
            UserConfig user = new UserConfig(db);
            ViewBag.Title = "Register";
            try {
                user.RegisterUser(model);
                TempData["Status"] = "Your user account has successfully been created";
                return Redirect("../Auth/Login");
            } catch (Exception ex) {
                ModelState.AddModelError("error", ex.Message);
                return View();
            }
        }

        public ActionResult Logout() {
            var ctx = Request.GetOwinContext();
            var authManager = ctx.Authentication;

            authManager.SignOut("ApplicationCookie");
            return RedirectToAction("login", "auth");
        }

        private string GetRedirectUrl(string strReturnUrl) {
            if (string.IsNullOrEmpty(strReturnUrl)) {
                return Url.Action("index", "tank");
            } else {
                return strReturnUrl;
            }
        }
    }
}