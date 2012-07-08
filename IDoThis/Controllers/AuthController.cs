namespace IDoThis.Controllers
{
    using System;
    using System.Web.Mvc;
    using System.Web.Security;
    using IDoThis.Models;
    using NBrowserID;

    public class AuthController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
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

        public ActionResult SignOut()
        {
            FormsAuthentication.SignOut();
            return Redirect("/");
        }
    }
}
