using System.Web.Mvc;

namespace SD250_Deliverable_tmp_DGrouette.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            // Just incase ¯\_(ツ)_/¯
            return RedirectToAction("Index", "Household");
        }

        [HttpGet]
        public ActionResult AnotherLink()
        {
            return RedirectToAction("Index", "Household");
        }
    }
}
