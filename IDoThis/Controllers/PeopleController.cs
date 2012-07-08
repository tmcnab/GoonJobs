namespace IDoThis.Controllers
{
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Web.Mvc;
    using AttributeRouting.Web.Mvc;
    using IDoThis.Models;

    public class PeopleController : Controller
    {
        [Route("people/", HttpVerbs.Get)]
        [Route("people/list/{?pg}", HttpVerbs.Get)]
        public ActionResult Index(int pg = 0, string q = "")
        {
            dynamic profiles = UserProfile.DAL.PublicProfiles
                                          .OrderByLastLoginDescending()
                                          .Skip(pg * 18)
                                          .Take(18);

            var model = new List<dynamic>();
            foreach (var p in profiles)
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
    }
}
