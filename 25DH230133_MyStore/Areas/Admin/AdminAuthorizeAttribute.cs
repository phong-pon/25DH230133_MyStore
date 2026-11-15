using System.Web.Mvc;
using System.Web.Routing;

namespace _25DH230133_MyStore.Areas.Admin
{
    public class AdminAuthorizeAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(System.Web.HttpContextBase httpContext)
        {
            return httpContext.Session != null &&
                   httpContext.Session["Username"] != null &&
                   string.Equals(httpContext.Session["UserRole"] as string, "A", System.StringComparison.OrdinalIgnoreCase);
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary {
                { "controller", "Account" }, { "action", "Login" },
                { "returnUrl", filterContext.HttpContext.Request.RawUrl }, { "area", "" }
            });
        }
    }
}
