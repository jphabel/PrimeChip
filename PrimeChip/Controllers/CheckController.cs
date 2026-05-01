using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
namespace PrimeChip.Controllers
{
    public class CheckController : Controller
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var user = HttpContext.Session.GetString("user");

            if (string.IsNullOrEmpty(user))
            {
                context.Result = RedirectToAction("Index", "Login");
            }
            base.OnActionExecuting(context);
        }
    }
}
