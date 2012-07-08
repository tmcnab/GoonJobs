namespace IDoThis.Controllers
{
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Web.Mvc;
    using AttributeRouting.Web.Mvc;
    using System.ComponentModel.DataAnnotations;
    using IDoThis.Models;

    public class PeopleController : Controller
    {
        [Route("people/", HttpVerbs.Get)]
        [Route("people/list/{?pg}", HttpVerbs.Get)]
        public ActionResult Index(int pg = 0, string q = "")
        {
            dynamic profiles = UserProfile.DAL.PublicProfiles
                                          .OrderByLastLoginDescending();
            
            ViewBag.TotalCount = profiles.ToList().Count;
            ViewBag.PageSize = 18;
            ViewBag.CurrentPage = pg;

            var model = new List<dynamic>();
            foreach (var p in profiles.Skip(pg * 18).Take(18))
            {
                var result = Gravatar.Profile(p.UsernameHashed);
                if(result != null && result.IsDefined("name") 
                                  && result.name.IsDefined("givenName") 
                                  && result.name.IsDefined("familyName")) {
                    model.Add(result);
                }
            }

            return View(model);
        }

        [Route("people/details/{emailhash}", HttpVerbs.Get)]
        public ActionResult Details(string emailhash)
        {
            dynamic model = new ExpandoObject();
            model.SProfile = UserProfile.DAL.UserProfiles.FindByUsernameHashed(emailhash);
            if (model.SProfile == null) {
                return View("404");
            }

            model.GProfile = Gravatar.Profile(emailhash);

            return View(model);
        }

        #region Contacting People

        [Authorize]
        [Route("people/contact/{emailHash}", HttpVerbs.Get)]
        public ActionResult Contact(string emailHash)
        {
            var recipientsProfile = UserProfile.DAL.UserProfiles.FindByUsernameHashed(emailHash);
            if (recipientsProfile == null   || 
                recipientsProfile.IsBanned  || 
               !recipientsProfile.IsHirable) {
                return View("404");
            }

            var sendersProfile = User.Profile();
            if (!sendersProfile.HasPaid) {
                return View("402");
            }

            ViewBag.Recipient = recipientsProfile;
            return View();
        }

        [Authorize]
        [Route("people/contact/{emailHash}", HttpVerbs.Post)]
        public ActionResult Contact(string emailHash, PeopleContactViewModel model)
        {
            var recipientsProfile = UserProfile.DAL.UserProfiles.FindByUsernameHashed(emailHash);
            if (recipientsProfile == null  ||
                recipientsProfile.IsBanned ||
               !recipientsProfile.IsHirable) {
                return View("404");
            }

            var sendersProfile = User.Profile();
            if (!sendersProfile.HasPaid) {
                return View("402");
            }

            if (ModelState.IsValid)
            {
                Email.Send(recipientsProfile.Username,
                    sendersProfile.Username,
                    "Contact / Inquiry - GoonJobs",
                    model.Body);

                TempData["Message"] = "Message sent!";
                return RedirectToAction("Index", "Profile");
            }
            else
            {
                ViewBag.Recipient = recipientsProfile;
                return View(model);
            }
        }

        #endregion

        #region View Models 
        public class PeopleContactViewModel
        {
            [AllowHtml]
            [Required]
            [StringLength(10000)]
            public string Body { get; set; }
        }
        #endregion
    }
}
