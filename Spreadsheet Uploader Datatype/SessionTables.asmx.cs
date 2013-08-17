using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace Spreadsheet_Uploader {
    /// <summary>
    /// Summary description for SessionTables
    /// </summary>
    [WebService(Namespace = "http://redlionproducts.com/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line.
    [System.Web.Script.Services.ScriptService]
    public class SessionTables : System.Web.Services.WebService {

        [WebMethod(EnableSession = true)]
        public void StoreTable(string strTable) {
            SessionCore.Authorize();
            HttpContext.Current.Session["sessionTable"] = HttpUtility.UrlDecode(strTable);
        }
    }
}
