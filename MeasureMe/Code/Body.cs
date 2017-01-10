using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Xml.Linq;
using MeasureMe.Models;
using MeasureMe.Common;
using MeasureMe.Page;
using AdoLib;

namespace MeasureMe.Code {
    public class Body {

        Database db;
        Common.Common cm;
        bool bIsMale;
        UserModel user;

        public Body(Database _db, UserModel _user) {
            this.db = _db;
            this.user = _user;
            this.cm = new Common.Common(db);
            this.bIsMale = this.user.Gender == "M";
        }

        public void SetUser(UserModel _user) {
            this.user = _user;
        }

        public void AddMeasurements(MeasureModel me) {
            var strUserId = user.UserId;
            var strId = Guid.NewGuid().ToString();
            SqlCommand cmd = new SqlCommand(@"insert into measurement(id, neck, shoulder, arm, forearm, wrist, chest, waist, abdomen, hips, thigh, knee, calf, bodyweight, userid)
values(@id, @neck, @shoulder, @arm, @forearm, @wrist, @chest, @waist, @abdomen, @hips, @thigh, @knee, @calf, @weight, @userid)");
            cmd.Parameters.AddWithValue("@id", strId);
            cmd.Parameters.AddWithValue("@neck", cm.AddParameter(me.Neck));
            cmd.Parameters.AddWithValue("@shoulder", cm.AddParameter(me.Shoulder));
            cmd.Parameters.AddWithValue("@arm", cm.AddParameter(me.Arm));
            cmd.Parameters.AddWithValue("@forearm", cm.AddParameter(me.Forearm));
            cmd.Parameters.AddWithValue("@wrist", cm.AddParameter(me.Wrist));
            cmd.Parameters.AddWithValue("@chest", cm.AddParameter(me.Chest));
            cmd.Parameters.AddWithValue("@waist", cm.AddParameter(me.Waist));
            cmd.Parameters.AddWithValue("@abdomen", cm.AddParameter(me.Abdomen));
            cmd.Parameters.AddWithValue("@hips", cm.AddParameter(me.Hips));
            cmd.Parameters.AddWithValue("@thigh", cm.AddParameter(me.Thigh));
            cmd.Parameters.AddWithValue("@knee", cm.AddParameter(me.Knee));
            cmd.Parameters.AddWithValue("@calf", cm.AddParameter(me.Calf));
            cmd.Parameters.AddWithValue("@weight", cm.AddParameter(me.BodyWeight));
            cmd.Parameters.AddWithValue("@userid", strUserId);
            db.ExecNonQuery(cmd);

            cmd = new SqlCommand(@"insert into bodyweight(userid, bodyweight, measurementid) values(@userid, @weight, @measurementid)");
            cmd.Parameters.AddWithValue("@userid", strUserId);
            cmd.Parameters.AddWithValue("@weight", cm.AddParameter(me.BodyWeight));
            cmd.Parameters.AddWithValue("@measurementid", strId);
            db.ExecNonQuery(cmd);
        }

        public void AddBodyWeight(double BodyWeight) {
            SqlCommand cmd = new SqlCommand(@"insert into bodyweight(userid, bodyweight) values(@userid, @weight)");
            cmd.Parameters.AddWithValue("@userid", this.user.UserId);
            cmd.Parameters.AddWithValue("@weight", BodyWeight);
            db.ExecNonQuery(cmd);
        }


        public BodyModel GetBodyModel() {
            BodyModel body = new BodyModel();
            body.Measurement = this.GetMeasurements(this.user.UserId);
            body.LastWeight = this.GetLastWeighIn(this.user.UserId);
            if (this.user.Measurements != null) {
                body.Ratio = this.GetGoldenRatio(this.user.Measurements);
                body.Fat = this.GetBodyFat(this.user.Measurements, this.user.Height, body.LastWeight);
                body.User = this.user;
                body.History = this.GetBodyHistory();
            }
            return body;
        }

        private double CalculateBodyFat(MeasureModel me, double height) {
            double bf = 0.0;

            if (bIsMale) {
                //(((86.01 * LOG10((D11 - D4))) - (70.041 * LOG10($A$51))) + 30.3) / 100
                bf = (((86.01 * Math.Log10(me.Abdomen - me.Neck)) - (70.041 * Math.Log10(height)) + 30.3) / 100);
            } else {
                //(495/(1.29579-0.35004*(LOG10(C10+C12-C4))+0.221*(LOG10($A$43)))-450)/100
                double dWaist = this.CalculateGoldenWaist();
                bf = (495 / (1.29579 - (0.35004 * (Math.Log10(me.Waist + me.Hips - me.Neck))) + (0.221 * Math.Log10(this.user.Height))) - 450) / 100;
            }
            return bf;
        }

        private FatModel GetBodyFat(MeasureModel me, double height, double weight) {
            FatModel fat = new FatModel();
            fat.BodyFat = this.CalculateBodyFat(me, height);
            fat.FatMass = weight * fat.BodyFat;
            fat.LeanMass = weight - fat.FatMass;
            return fat;
        }

        public MeasureModel GetMeasurements(string strUserId) {
            MeasureModel me = new MeasureModel();
            SqlCommand cmd = new SqlCommand("select top 1 * from measurement where userid=@id order by created desc");
            cmd.Parameters.AddWithValue("@id", strUserId);
            XElement ndMeasure = db.ExecQueryElem(cmd);

            if (ndMeasure != null) {
                me = this.XmlToMeasure(ndMeasure);
            }

            return me;
        }

        public double GetLastWeighIn(string strUserId) {
            // lets check if we have a more recent body weight reading
            SqlCommand cmd = new SqlCommand(@"select top 1 * from bodyweight where userid=@id order by created desc");
            cmd.Parameters.AddWithValue("@id", strUserId);
            XElement ndWeight = db.ExecQueryElem(cmd);
            if (ndWeight != null) {
                DateTime dtBW;
                if (DateTime.TryParse(ndWeight.Element("created").Value, out dtBW)) { 
                    double d;
                    if (Double.TryParse(ndWeight.Element("bodyweight").Value, out d)) {
                        return d;
                    }
                }
            }

            return 0;
        }

        private double CalculateGoldenWaist() {
            return this.user.Height * 0.382;
        }

        private RatioModel GetGoldenRatio(MeasureModel me) {
            RatioModel ratio = new RatioModel();
            // check if using local gender
            if (bIsMale) {
                ratio.Neck = me.Neck - (me.Wrist * 2.5);
                ratio.Shoulder = me.Shoulder - (me.Waist * 1.618);
                ratio.Chest = me.Chest - (me.Wrist * 6.5);
                ratio.Arm = me.Arm - (me.Wrist * 2.5);
                ratio.Forearm = me.Forearm - (me.Wrist * 1.88);
                ratio.Calf = me.Calf - (me.Wrist * 2.5);
                ratio.Thigh = me.Knee - (me.Knee * 1.75);
            } else {
                double dWaist = this.CalculateGoldenWaist();
                ratio.Shoulder = me.Shoulder - (dWaist * 1.618);
                ratio.Waist = me.Waist - dWaist;
                ratio.Hips = me.Hips - (dWaist * 1.42);
            }
            return ratio;
        }

        private HistoryModel GetBodyHistory() {
            HistoryModel history = new HistoryModel();
            SqlCommand cmd = new SqlCommand("select * from measurement where userid=@id order by created");
            cmd.Parameters.AddWithValue("@id", this.user.UserId);
            XDocument xmlMeasure = db.ExecQuery(cmd);
            List<MeasureModel> Measurements = new List<MeasureModel>();
            if (xmlMeasure != null) {
                foreach (XElement ndMeasure in xmlMeasure.Element("Root").Elements()) {
                    MeasureModel me = this.XmlToMeasure(ndMeasure);
                    Measurements.Add(me);
                }
            }
            history.Measurements = Measurements;
            return history;
        }

        private MeasureModel XmlToMeasure(XElement ndMeasure) {
            MeasureModel me = new MeasureModel();
            foreach (XElement nd in ndMeasure.Elements()) {
                if (nd.Name == "id" || nd.Name == "userid") {
                    continue;
                }
                if(nd.Name == "created") {
                    DateTime dt;
                    if(DateTime.TryParse(nd.Value, out dt)) {
                        me.Created = dt;
                        me.strCreated = dt.ToString("yyyy-MM-dd");
                    }
                    continue;
                }
                double d;
                if (double.TryParse(nd.Value, out d)) {
                    switch (nd.Name.ToString()) {
                        case "neck":
                            me.Neck = d;
                            break;
                        case "shoulder":
                            me.Shoulder = d;
                            break;
                        case "arm":
                            me.Arm = d;
                            break;
                        case "forearm":
                            me.Forearm = d;
                            break;
                        case "wrist":
                            me.Wrist = d;
                            break;
                        case "chest":
                            me.Chest = d;
                            break;
                        case "waist":
                            me.Waist = d;
                            break;
                        case "abdomen":
                            me.Abdomen = d;
                            break;
                        case "hips":
                            me.Hips = d;
                            break;
                        case "thigh":
                            me.Thigh = d;
                            break;
                        case "knee":
                            me.Knee = d;
                            break;
                        case "calf":
                            me.Calf = d;
                            break;
                        case "bodyweight":
                            me.BodyWeight = d;
                            break;
                    }
                }
            }

            double dBodyFat = this.CalculateBodyFat(me, this.user.Height);
            me.BodyFat = dBodyFat;
            return me;
        }
    }
}