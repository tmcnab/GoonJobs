namespace IDoThis.Controllers
{
    using System;
    using System.Dynamic;
    using System.Linq;
    using System.Web.Mvc;
    using AttributeRouting.Web.Mvc;
    using IDoThis.Models;

    [Authorize(Users = "tristan@seditious-tech.com")]
    public class ACPController : Controller
    {
        #region /acp

        public ActionResult Index()
        {
            dynamic model = new ExpandoObject();
            
            model.Activations = ActivationCode.DAL.ActivationCodes;
            model.Users = UserProfile.DAL.UserProfiles;
            model.Issues = FlaggedIssue.DAL.Issues;
            model.Listings = JobListing.DAL.Listings;

            return View(model);
        }

        #endregion

        #region /acp/users

        public ActionResult Users()
        {
            dynamic model = new ExpandoObject();

            return View();
        }

        #endregion

        #region /acp/activations

        [Route("acp/activations", HttpVerbs.Get)]
        public ActionResult Activations()
        {
            return View(ActivationCode.DAL.ActivationCodes.All());
        }

        [Route("acp/activations", HttpVerbs.Post)]
        public ActionResult ActivationsCreate(FormCollection collection)
        {
            var n = int.Parse(collection["n"]);
            for (int i = 0; i < n; i++)
            {
                var code = new ActivationCode() {
                    Code = new string(Guid.NewGuid().ToString("N").Take(10).ToArray()),
                    Activated = DateTime.MinValue,
                    Username = null
                };

                ActivationCode.DAL.ActivationCodes.Insert(code);
            }

            return RedirectToAction("Activations");
        }

        #endregion

        #region /acp/issues
        
        [Route("acp/issues")]
        public ActionResult Issues()
        {
            var model = FlaggedIssue.DAL.Issues;
            return View(model);
        }

        #endregion

        #region /acp/issues/close

        [Route("acp/issues/close/{slugOrUserHash}", HttpVerbs.Get)]
        public ActionResult CloseIssue(string slugOrUserHash)
        {
            dynamic item = FlaggedIssue.DAL.Issues.FindBySlugOrUserHash(slugOrUserHash);
            item.IsOpen = false;
            item.Reported = DateTime.UtcNow;
            FlaggedIssue.DAL.Issues.Update(item);

            return RedirectToAction("Issues");
        }

        #endregion
    }
}
