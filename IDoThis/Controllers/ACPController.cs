namespace IDoThis.Controllers
{
    using System;
    using System.Linq;
    using System.Web.Mvc;
    using AttributeRouting.Web.Mvc;
    using IDoThis.Models;
    using System.Dynamic;

    [Authorize(Users = "tristan@seditious-tech.com")]
    public class ACPController : Controller
    {
        public ActionResult Index()
        {
            dynamic model = new ExpandoObject();

            model.ActivationsUsed = ActivationCode.DAL.Activated().ToList().Count;
            model.ActivationsFree = ActivationCode.DAL.NonActivated().Count;

            return View(model);
        }

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

    }
}
