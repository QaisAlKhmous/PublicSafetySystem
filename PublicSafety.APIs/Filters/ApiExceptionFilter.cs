using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace PublicSafety.APIs.Filters
{
    public class ApiExceptionFilter : FilterAttribute, IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            context.HttpContext.Response.StatusCode = 500;
            context.ExceptionHandled = true;

            context.Result = new JsonResult
            {
                Data = new ApiResponse<object>
                {
                    Success = false,
                    Message = "مشكلة في السيرفر",
                    Data = null
                },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
    }
}
