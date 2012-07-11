namespace IDoThis.Controllers
{
    using System.Web.Mvc;
    using AttributeRouting.Web.Mvc;

    public class LegalController : Controller
    {
        [Route("legal/", HttpVerbs.Get)]
        public ActionResult Index()
        {
            return View();
        }
    }
}
