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
        public ActionResult Index()
        {
            dynamic model = new ExpandoObject();
            model.Activations = ActivationCode.DAL.ActivationCodes;
            model.Profiles = UserProfile.DAL.UserProfiles;
            return View(model);
        }

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
    }
}
