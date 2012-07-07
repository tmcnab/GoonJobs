namespace IDoThis.Controllers
{
    using System.Dynamic;
    using System.Net;
    using System.Web.Mvc;
    using AttributeRouting.Web.Mvc;
    using Codeplex.Data;
    using System;
    using IDoThis.Models;
    using System.Text;

    public class PeopleController : Controller
    {
        [Route("people/", HttpVerbs.Get)]
        [Route("people/list/{?pg}", HttpVerbs.Get)]
        public ActionResult Index(int pg = 0, string q = "")
        {
            dynamic model = UserProfile.DAL.PublicProfiles
                                       .OrderByLastLoginDescending();
                                   
            return View(model.Skip(pg * 12).Take(12));
        }

        [Route("people/details/{emailhash}", HttpVerbs.Get)]
        public ActionResult Details(string emailhash)
        {
            dynamic model = new ExpandoObject();
            model.SProfile = UserProfile.DAL.UserProfiles.FindByUsernameHashed(emailhash);
            if (model.SProfile == null) {
                return View("404");
            }

            try
            {
                var url = string.Format("https://en.gravatar.com/{0}.json", emailhash);
                var data = UTF8Encoding.UTF8.GetString((new WebClient()).DownloadData(url))
                                       .Replace("{\"entry\":[", string.Empty)
                                       .TrimEnd('}')
                                       .TrimEnd(']');
                model.GProfile = DynamicJson.Parse(data);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                model.GProfile = null;
            }
            

            return View(model);
        }
    }
}
