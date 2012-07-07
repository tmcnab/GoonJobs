namespace IDoThis.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Security.Principal;
    using System.Configuration;
    using Simple.Data;
    using Simple.Data.MongoDB;

    public class ActivationCode
    {
        #region Record Fields

        public DateTime Activated { get; set; }

        public string Username { get; set; }

        public string Code { get; set; }

        #endregion

        public static class DAL
        {
            static dynamic db { 
                get {
                    return Database.Opener.OpenMongo(ConfigurationManager.AppSettings["MONGOLAB_URI"]);
                }
            }

            public static dynamic ActivationCodes {
                get {
                    return db.ActivationCodes;
                }
            }

            public static bool IsValidSubmission(string code)
            {
                var item = ActivationCodes.FindByCode(code);
                if (item == null || !string.IsNullOrEmpty(item.Username)) {
                    return false;
                }
                else {
                    return true;
                }
            }

            public static void Activate(IPrincipal User, string code)
            {
                var item = ActivationCodes.FindByCode(code);
                item.Username = User.Identity.Name;
                item.Activated = DateTime.UtcNow;

                var user = User.Profile();
                user.HasPaid = true;
                UserProfile.DAL.UserProfiles.Update(user);

                ActivationCodes.Update(item);
            }

            public static dynamic Activated()
            {
                return ActivationCodes.All().Where(db.ActivationCodes.Username == null);
            }

            public static dynamic NonActivated()
            {
                return ActivationCodes.All().Where(db.ActivationCodes.Username != null);
            }
        }
    }

    public class ActivationCodeViewModel
    {
        [Required]
        [StringLength(10, MinimumLength = 10)]
        public string activation_code { get; set; }
    }
}