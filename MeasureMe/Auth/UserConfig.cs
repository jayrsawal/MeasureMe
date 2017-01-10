using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AdoLib;
using System.Data.SqlClient;
using MeasureMe.Models;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Xml.Linq;

namespace MeasureMe.Auth {
    public class UserConfig {
        Database db;

        public UserConfig(Database _db) {
            db = _db;
        }

        public bool VerifyUser(LoginModel model, out ClaimsIdentity identity) {
            SqlCommand cmd = new SqlCommand(@"select * from useraccount where username=@username");
            cmd.Parameters.AddWithValue("@username", model.Username);
            XElement ndUser = db.ExecQueryElem(cmd);

            if (ndUser != null) {
                if (ndUser.Element("lockout") != null) {
                    throw new Exception("This user account has been locked.");
                }
                if (ComparePasswordHash(model.Password, ndUser.Element("passhash").Value)) {
                    identity = new ClaimsIdentity(new[] {
                        new Claim(ClaimTypes.Name,  ndUser.Element("username").Value)
                        , new Claim(ClaimTypes.Email,  ndUser.Element("email").Value)
                        , new Claim("UserId",  ndUser.Element("id").Value)
                        , new Claim(ClaimTypes.NameIdentifier,  ndUser.Element("id").Value)
                        , new Claim("Gender",  ndUser.Element("gender").Value)
                        , new Claim("Height",  ndUser.Element("height").Value)
                        , new Claim("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider",  ndUser.Element("id").Value)
                        }
                        , "ApplicationCookie");
                    return true;
                } else {
                    this.ProcessFailedLogin(ndUser);
                }
            }

            identity = null;
            return false;
        }

        public void RegisterUser(RegisterModel model) {
            SqlCommand cmd = new SqlCommand(@"select * from useraccount where username=@username or email=@email");
            cmd.Parameters.AddWithValue("@username", model.Username);
            cmd.Parameters.AddWithValue("@email", model.Email);
            XElement ndExists = db.ExecQueryElem(cmd);

            if (ndExists == null) {
                try {
                    cmd = new SqlCommand(@"insert into useraccount(username, email, passhash, gender, height, dob) values(@username, @email, @passhash, @gender, @height, @dob)");
                    cmd.Parameters.AddWithValue("@username", model.Username);
                    cmd.Parameters.AddWithValue("@email", model.Email);
                    cmd.Parameters.AddWithValue("@passhash", HashPassword(model.Password));
                    cmd.Parameters.AddWithValue("@gender", model.Gender);
                    cmd.Parameters.AddWithValue("@height", model.Height);
                    cmd.Parameters.AddWithValue("@dob", model.DOB);
                    db.ExecNonQuery(cmd);
                } catch (Exception ex) {
                    throw ex;
                }
            } else {
                throw new Exception("A user already exists with this username and/or email");
            }
        }

        private void ProcessFailedLogin(XElement ndUser) {
            string strId = ndUser.Element("id").Value;

            SqlCommand cmd;
            int iAttempts;
            if (ndUser.Element("failed") != null) {
                Int32.TryParse(ndUser.Element("failed").Value, out iAttempts);
            } else {
                iAttempts = 0;
            }
            iAttempts++;

            DateTime dt;
            if (ndUser.Element("attemptdt") != null) {
                DateTime.TryParse(ndUser.Element("attemptdt").Value, out dt);
            } else {
                dt = DateTime.Now;
            }
            if ((DateTime.Now - dt).TotalHours > 1) {
                iAttempts = 0;
            }

            if (iAttempts > 3) {
                cmd = new SqlCommand(@"update useraccount set lockout=1, lockoutdt=getdate(), failedattempt=@attempt where id=@id");
                cmd.Parameters.AddWithValue("@id", strId);
                cmd.Parameters.AddWithValue("@attempt", iAttempts);
                db.ExecNonQuery(cmd);

                throw new Exception("3 consecutively failed login attempts. User account has been locked.");
            } else {
                cmd = new SqlCommand(@"update useraccount set attemptdt=getdate(), failedattempt=@attempt where id=@id");
                cmd.Parameters.AddWithValue("@id", strId);
                cmd.Parameters.AddWithValue("@attempt", iAttempts);
                db.ExecNonQuery(cmd);
            }
        }

        private bool ComparePasswordHash(string strAttempt, string strPassHash) {
            byte[] aPassHash = Convert.FromBase64String(strPassHash);
            byte[] aSalt = new byte[16];
            Array.Copy(aPassHash, 0, aSalt, 0, 16);

            string strAttemptHash = HashPassword(strAttempt, aSalt);
            byte[] aAttemptHash = Convert.FromBase64String(strAttemptHash);

            for (int i = 0; i < 36; i++) {
                if (aPassHash[i] != aAttemptHash[i]) {
                    return false;
                }
            }

            return true;
        }

        private string HashPassword(string strPassword, byte[] aSalt = null) {
            if (aSalt == null) {
                new RNGCryptoServiceProvider().GetBytes(aSalt = new byte[16]);
            }
            var pbkdf2 = new Rfc2898DeriveBytes(strPassword, aSalt, 10000);
            byte[] hash = pbkdf2.GetBytes(20);
            byte[] aPassHash = new byte[36];
            Array.Copy(aSalt, 0, aPassHash, 0, 16);
            Array.Copy(hash, 0, aPassHash, 16, 20);

            string strPassHash = Convert.ToBase64String(aPassHash);
            return strPassHash;
        }
    }
}