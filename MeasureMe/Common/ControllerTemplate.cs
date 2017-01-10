using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Collections;
using AdoLib;
using System.Web.Mvc;
using System.Data.SqlClient;
using System.Security.Claims;
using MeasureMe.Models;
using System.Xml.Linq;
using MeasureMe.Code;

namespace MeasureMe.Common {
    public class ControllerTemplate : Controller {
        public Database db;
        public string strEnv;
        public Hashtable aFields;
        public UserModel user;
        public Common cm;

        public ControllerTemplate(string _strEnv = "test") {
            this.strEnv = _strEnv;
            this.aFields = new Hashtable();
            this.SetDatabase(this.strEnv);

            aFields = new Hashtable();
            cm = new Common(db);

            var context = System.Web.HttpContext.Current;
            if (context.Request.IsAuthenticated) {
                ClaimsIdentity identity = (ClaimsIdentity) context.User.Identity;
                this.user = this.GetUserFromIdentity(identity);
            }

        }

        public void SetDatabase(string _strEnv = "test") {
            this.strEnv = _strEnv;
            string strConn;

            switch (this.strEnv) {
                case "production":
                    strConn = ConfigurationManager.ConnectionStrings["MMEProd"].ConnectionString;
                    break;

                default:
                    strConn = ConfigurationManager.ConnectionStrings["MMETest"].ConnectionString;
                    break;
            }

            db = new Database(strConn);
        }

        private UserModel GetUserFromIdentity(ClaimsIdentity identity) {
            UserModel _user = new UserModel();
            _user.UserId = identity.FindFirst("UserId").Value;
            _user.UserName = identity.FindFirst(ClaimTypes.Name).Value;
            _user.Email = identity.FindFirst(ClaimTypes.Email).Value;

            string strHeight= "0.0";
            if(identity.FindFirst("Height") != null) {
                strHeight = identity.FindFirst("Height").Value;
                double d;
                if (double.TryParse(strHeight, out d)) {
                    _user.Height = d;
                }
            }
            if (identity.FindFirst("Gender") != null) {
                _user.Gender = identity.FindFirst("Gender").Value;
            }
            Body b = new Body(db, _user);
            MeasureModel me = b.GetMeasurements(_user.UserId);
            _user.Measurements = me;
            return _user;
        }

        public void LogException(Exception ex) {
            string strMessage = ex.Message;
            Exception ex2 = ex;

            while (ex2.InnerException != null) {
                ex2 = ex.InnerException;
                strMessage += " \\ " + ex2.Message;
            }

            SqlCommand cmd = new SqlCommand("insert into systemerror(errortext, uri, userid) values(@error, @uri, @userid)");
            cmd.Parameters.AddWithValue("@error", strMessage);
            cmd.Parameters.AddWithValue("@uri", Request.Url.AbsoluteUri);
            if (Request.IsAuthenticated) {
                string strUserId = ((ClaimsIdentity)User.Identity).FindFirst("UserId").Value;
                cmd.Parameters.AddWithValue("@userid", strUserId);
            }
            db.ExecNonQuery(cmd);
        }
    }
}