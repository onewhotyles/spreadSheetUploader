using System.Web;
using System.Web.Mvc;

namespace Spreadsheet_Uploader
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}