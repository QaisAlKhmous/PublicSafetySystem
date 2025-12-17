using PublicSafety.APIs.Filters;
using System.Web;
using System.Web.Mvc;

namespace PublicSafety.Presentation
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new ApiExceptionFilter());
        }
    }
}
