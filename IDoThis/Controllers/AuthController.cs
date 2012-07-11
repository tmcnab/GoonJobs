namespace IDoThis.Controllers
{
    using System;
    using System.Web.Mvc;
    using System.Web.Security;
    using IDoThis.Models;
    using NBrowserID;
    using AttributeRouting.Web.Mvc;

    public class AuthController : Controller
    {
        [Route("auth/", HttpVerbs.Get)]
        public ActionResult Index()
        {
            return View();
        }

        [Route("auth/signin/", HttpVerbs.Post)]
        public ActionResult SignIn(string assertion)
        {
            var authentication = new BrowserIDAuthentication();
            var verificationResult = authentication.Verify(assertion);
            if (verificationResult.IsVerified)
            {
                string email = verificationResult.Email;
                var profile = UserProfile.DAL.UserProfiles.FindByUsername(email);
                if (profile == null) 
                {
                    profile = new UserProfile(verificationResult.Email);
                    UserProfile.DAL.UserProfiles.Insert(profile);
                }
                else 
                {
                    if (profile.IsBanned)
                    {
                        return Json(new
                        {
                            email = "",
                            is_banned = true
                        });
                    }
                    else
                    {
                        profile.LastLogin = DateTime.UtcNow;
                        UserProfile.DAL.UserProfiles.Update(profile);
                    }
                }

                FormsAuthentication.SetAuthCookie(email, false);
                return Json(new
                {
                    email = email,
                    is_banned = false
                });
            }

            return Json(null);
        }

        [Route("auth/signout/", HttpVerbs.Get)] 
        public ActionResult SignOut()
        {
            FormsAuthentication.SignOut();
            return Redirect("/");
        }
    }
}
