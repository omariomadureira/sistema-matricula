using System.Web.Mvc;

namespace SistemaMatricula.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        [AllowAnonymous]
        public ActionResult Index()
        {
            if(!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "User");
            }

            return View();
        }

        public ActionResult About()
        {
            return View();
        }
    }
}