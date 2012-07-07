namespace IDoThis.Controllers
{
    using System.Dynamic;
    using System.Web.Mvc;
    using IDoThis.Models;
    using System.Collections.Generic;
    using AttributeRouting.Web.Mvc;

    public class ProfileController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            var profile = User.Profile();

            dynamic model = new ExpandoObject();
            model.Listings = JobListing.DAL.Listings.FindAllByUser(User.Identity.Name);
            model.Applications = new List<dynamic>();
            model.Saved = new List<dynamic>();
            model.HasPaid = profile.HasPaid;
            model.Profile = profile;

            foreach (var item in profile.AppliedFor) {
                model.Applications.Add(JobListing.DAL.Listings.FindBySlug(item).JobTitle);
            }

            foreach (var item in profile.Saved) {
                model.Applications.Add(JobListing.DAL.Listings.FindBySlug(item).JobTitle);
            }

            return View(model);
        }

        [Authorize]
        [HttpPost]
        public ActionResult Index(UserProfileUpdateModel model)
        {
            if (ModelState.IsValid)
            {
                dynamic profile = User.Profile();
                profile.IsHirable = model.IsHirable;
                profile.Description = model.Description;
                profile.ShowcaseUrl = model.ShowcaseUrl;
                UserProfile.DAL.UserProfiles.Update(profile);
            }
            return RedirectToAction("Index");
        }

        public ActionResult Banned()
        {
            return View();
        }

        [Authorize]
        [Route("profile/upgrade", HttpVerbs.Get)]
        public ActionResult Upgrade()
        {
            return View("402");
        }

        [Authorize]
        [HttpPost]
        public ActionResult Activate(ActivationCodeViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (!ActivationCode.DAL.IsValidSubmission(model.activation_code))
                {
                    ViewBag.Errors = true;
                    ViewBag.ErrorMsg = "Activation code provided has already been used.";
                    return View("402");
                }
                else
                {
                    ActivationCode.DAL.Activate(User, model.activation_code);
                    return RedirectToAction("Index");
                }
            }
            else
            {
                ViewBag.Errors = true;
                ViewBag.ErrorMsg = "The activation did not succeed successfully. The code must be 10 characters";
                return View("402");
            }
        }
    }
}
