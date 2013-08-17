using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Spreadsheet_Uploader {
    public class SessionCore {
        public static void Authorize() {
            if (umbraco.BusinessLogic.User.GetCurrent() == null) {
                HttpContext.Current.Response.StatusCode = 403;
                HttpContext.Current.Response.End();
            }
        }
    }
}