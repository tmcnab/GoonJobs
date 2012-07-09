namespace IDoThis.Controllers
{
    using System.Web.Mvc;
    using IDoThis.Models;

    public class MainController : Controller
    {
        public ActionResult Index()
        {
            var model = JobListing.DAL.Active
                                  .OrderByPostedDescending()
                                  .Take(6);

            return View(model);
        }
    }
}
