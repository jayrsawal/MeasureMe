using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Optimization;
using System.Text.RegularExpressions;
using System.Text;
using System.Data.SqlClient;
using AdoLib.Extensions;

namespace MeasureMe {
    public class MvcApplication : System.Web.HttpApplication {
        protected void Application_Start() {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_BeginRequest(object sender, EventArgs e) {
            //You don't want to redirect on posts, or images/css/js
            bool isGet = HttpContext.Current.Request.RequestType.ToLowerInvariant().Contains("get");
            if (isGet && HttpContext.Current.Request.Url.AbsolutePath.Contains(".") == false) {
                string lowercaseURL = (Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority + HttpContext.Current.Request.Url.AbsolutePath);
                if (Regex.IsMatch(lowercaseURL, @"[A-Z]")) {
                    //You don't want to change casing on query strings
                    lowercaseURL = lowercaseURL.ToLower() + HttpContext.Current.Request.Url.Query;

                    Response.Clear();
                    Response.Status = "301 Moved Permanently";
                    Response.AddHeader("Location", lowercaseURL);
                    Response.End();
                }
            }
        }

        protected void Application_Error(object sender, EventArgs e) {
            Exception exception = Server.GetLastError();
            System.Diagnostics.Debug.WriteLine(exception);
            Response.Redirect("~/main/error");
        }
    }
}
