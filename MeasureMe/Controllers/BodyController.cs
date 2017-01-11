using MeasureMe.Common;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using MeasureMe.Models;
using MeasureMe.Code;

namespace MeasureMe.Controllers {
    public class BodyController : ControllerTemplate {
        // GET: Body
        public ActionResult Index() {
            return View();
        }

        [HttpGet]
        public ActionResult Measure() {
            return View();
        }

        [HttpPost]
        public ActionResult Measure(MeasureModel me) {
            if (this.user.Gender == "M") {
                if (me.Neck == 0 || me.Abdomen == 0 || me.BodyWeight == 0) {
                    TempData["Error"] = "Unable to add measurements - Please ensure all required fields have a value";
                }
            } else {
                if (me.Neck == 0 || me.Waist == 0 || me.Hips == 0 || me.BodyWeight == 0) {
                    TempData["Error"] = "Unable to add measurements - Please ensure all required fields have a value";
                }
            }

            if (TempData["Error"] != null) {
                return RedirectToAction("../Main/Home");
            }

            try {
                Body b = new Body(this.db, this.user);
                b.AddMeasurements(me);
                this.user.Measurements = me;
                ViewModel model = new ViewModel();
                TempData["Status"] = "Measurements successfully recorded";
            } catch {
                TempData["Error"] = "Unable to add measurements - Please try again later";
            }
            return RedirectToAction("../Main/Home");
        }

        [HttpPost]
        public ActionResult Weight(WeightModel model) {
            if(model.BodyWeight == 0) {
                TempData["Error"] = "Please provide your body weight";
                return RedirectToAction("../Main/Home");
            }
            try {
                Body b = new Body(this.db, this.user);
                b.AddBodyWeight(model.BodyWeight);
                this.user.Measurements.BodyWeight = model.BodyWeight;
                TempData["Status"] = "Body weight successfully recorded";
            } catch {
                TempData["Error"] = "Unable to record body weight - Please try again later";
            }
            return RedirectToAction("../Main/Home");
        }
    }
}