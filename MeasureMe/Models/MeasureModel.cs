using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MeasureMe.Models {
    public class MeasureModel {
        public DateTime Created { get; set; }

        public string strCreated { get; set; }

        public double Neck { get; set; }

        public double Shoulder { get; set; }

        public double Arm { get; set; }

        public double Forearm { get; set; }

        public double Wrist { get; set; }

        public double Chest { get; set; }

        public double Waist { get; set; }

        public double Abdomen { get; set; }

        public double Hips { get; set; }

        public double Thigh { get; set; }

        public double Knee { get; set; }

        public double Calf { get; set; }

        public double BodyWeight { get; set; }

        public double BodyFat { get; set; }
    }
}