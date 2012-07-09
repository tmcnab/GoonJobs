namespace IDoThis.Controllers
{
    using System.Web.Mvc;
    using AttributeRouting.Web.Mvc;

    public class StatusCodesController : Controller
    {
        #region 402

        [Route("402", HttpVerbs.Get)]
        public ActionResult HttpStatus402()
        {
            return View("402");
        }

        #endregion

        #region 404

        [Route("404", HttpVerbs.Get)]
        public ActionResult HttpStatus404()
        {
            return View("404");
        }

        #endregion

        #region 500

        [Route("500", HttpVerbs.Get)]
        public ActionResult HttpStatus500()
        {
            return View("500");
        }

        #endregion
    }
}
