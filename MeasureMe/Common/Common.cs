using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using AdoLib;

namespace MeasureMe.Common {
    public class Common {
        Database db;
        public Common(Database _db) {
            this.db = _db;
        }

        public object AddParameter(object strParam) {
            if (strParam == null) {
                return DBNull.Value;
            } else {
                return strParam;
            }
        }
    }
}