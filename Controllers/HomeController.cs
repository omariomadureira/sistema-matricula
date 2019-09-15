using System.Web.Mvc;

namespace SistemaMatricula.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        [AllowAnonymous]
        public ActionResult Index()
        {
            if (!Request.IsAuthenticated)
                return RedirectToAction("Login", "User");

            if (User.IsInRole(Models.User.ROLE_STUDENT))
                return RedirectToAction("Index", "Registry");

            if (User.IsInRole(Models.User.ROLE_TEACHER))
                return RedirectToAction("List", "Registry");

            return RedirectToAction("Index", "Grid");
        }

        public ActionResult About()
        {
            return View();
        }
    }
}