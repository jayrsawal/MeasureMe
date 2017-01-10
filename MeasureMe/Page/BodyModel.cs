using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MeasureMe.Models;

namespace MeasureMe.Page {
    public class BodyModel {
        public MeasureModel Measurement { get; set; }
        public RatioModel Ratio { get; set; }
        public FatModel Fat { get; set; }
        public UserModel User { get; set; }
        public HistoryModel History { get; set; }
        public double LastWeight { get; set; }
    }
}