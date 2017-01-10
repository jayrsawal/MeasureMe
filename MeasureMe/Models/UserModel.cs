using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MeasureMe.Models {
    public class UserModel {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public double Height { get; set; }
        public string Gender { get; set; }
        public MeasureModel Measurements { get; set; }
    }
}