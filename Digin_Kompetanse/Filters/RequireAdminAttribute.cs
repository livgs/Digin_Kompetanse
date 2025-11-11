using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Digin_Kompetanse.Filters
{
    public class RequireAdminAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var role = context.HttpContext.Session.GetString("Role");
            if (role != "Admin")
            {
                context.Result = new RedirectToActionResult("Index", "Home", null);
            }
        }
    }
}