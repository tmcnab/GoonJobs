namespace IDoThis.Controllers
{
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Web.Mvc;
    using AttributeRouting.Web.Mvc;
    using IDoThis.Models;

    public class ProfileController : Controller
    {
        #region /profile/

        [Authorize]
        [Route("profile/", HttpVerbs.Get)]
        public ActionResult Index()
        {
            var profile = User.Profile();

            dynamic model = new ExpandoObject();
            model.Listings = JobListing.DAL.Listings.FindAllByUser(User.Identity.Name);
            var savedList = new List<dynamic>();
            foreach (dynamic item in SavedListing.DAL.SavedListingsForUser(User).ToList())
            {
                savedList.Add(JobListing.DAL.Listings.FindBySlug(item.Slug));  
            }
            model.Saved = savedList;
            model.Profile = profile;
            model.Gravatar = Gravatar.Profile(User);

            return View(model);
        }

        [Authorize]
        [Route("profile/", HttpVerbs.Post)]
        public ActionResult Index(UserProfileUpdateModel model)
        {
            if (ModelState.IsValid)
            {
                dynamic profile = User.Profile();
                profile.IsHirable = model.IsHirable;
                profile.Description = model.Pitch;
                profile.DescriptionSearchable = model.Pitch.ToLowerInvariant().Replace(':', ' ')
                                                                              .Replace('.', ' ')
                                                                              .Replace(',', ' ');
                UserProfile.DAL.UserProfiles.Update(profile);
            }
            return RedirectToAction("Index");
        }

        #endregion

        #region /profile/banned

        public ActionResult Banned()
        {
            return View();
        }

        #endregion

        #region /profile/upgrade

        [Authorize]
        [Route("profile/upgrade", HttpVerbs.Get)]
        public ActionResult Upgrade()
        {
            return View("402");
        }

        #endregion

        #region /profile/paid

        [Authorize]
        [Route("profile/paid")]
        public ActionResult Paid()
        {
            var thisUser = User.Profile();
            thisUser.HasPaid = true;
            UserProfile.DAL.UserProfiles.Update(thisUser);
            return RedirectToAction("Index");
        }

        #endregion

        #region /profile/activate/

        [Authorize]
        [Route("profile/activate/", HttpVerbs.Post)]
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

        #endregion
    }
}
