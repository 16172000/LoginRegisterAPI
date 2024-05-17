using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace TicketProjectWEB
{
    public class SessionAuthorize : ActionFilterAttribute
    {

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var httpContext = context.HttpContext;
            var session = httpContext.Session.GetString("UserName");

            if (session == null)
            {
                context.Result = new RedirectToActionResult("Login", "Account", null);
                httpContext.Session.Clear();
                //httpContext.SignOutAsync();
                httpContext.Session.Remove("session");
                //httpContext.c.Delete("CDMM.Session");
                return;
            }
            base.OnActionExecuting(context);
        }
    }
}
