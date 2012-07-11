namespace IDoThis.Controllers
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Dynamic;
    using System.Web.Mvc;
    using AttributeRouting.Web.Mvc;
    using IDoThis.Models;

    public class PeopleController : Controller
    {
        #region /people

        [Route("people/", HttpVerbs.Get)]
        [Route("people/list/{?pg}", HttpVerbs.Get)]
        public ActionResult Index(int pg = 0, string q = "")
        {
            dynamic profiles;

            if (!string.IsNullOrWhiteSpace(q)) {
                profiles = UserProfile.DAL.Search(q);
            }
            else {
                profiles = UserProfile.DAL.PublicProfiles.OrderByLastLoginDescending();
            }

            ViewBag.TotalCount = profiles.ToList().Count;
            ViewBag.PageSize = 18;
            ViewBag.CurrentPage = pg;
            profiles = profiles.Skip(pg * 18).Take(18);


            var model = new List<dynamic>();
            foreach (dynamic p in profiles)
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

        #endregion

        #region /people/details/#

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

        #endregion

        #region /people/flag/#

        [Authorize]
        [Route("people/flag/{hash}", HttpVerbs.Get)]
        public ActionResult Flag(string hash)
        {
            var issue = new FlaggedIssue(hash, true, User);
            FlaggedIssue.DAL.Issues.Insert(issue);
            return RedirectToAction("Index");
        }

        #endregion
    }

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
