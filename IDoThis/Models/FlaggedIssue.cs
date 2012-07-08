using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IDoThis.Models
{
    using System.Security.Principal;
    using Simple.Data;
    using Simple.Data.MongoDB;
    using System.Configuration;

    public class FlaggedIssue
    {
        #region Data Fields

        public string SlugOrUserHash { get; set; }

        public bool IsProfile { get; set; }

        public DateTime Reported { get; set; }

        public string Reporter { get; set; }

        public bool IsOpen { get; set; }

        #endregion

        #region Constructors

        public FlaggedIssue() { }

        public FlaggedIssue(string slugOrHash, bool isProfile, IPrincipal User)
        {
            this.IsProfile = isProfile;
            this.Reported = DateTime.UtcNow;
            this.Reporter = User.Identity.Name;
            this.SlugOrUserHash = slugOrHash;
            this.IsOpen = true;
        }

        #endregion

        #region DAL

        public static class DAL
        {
            static dynamic db {
                get {
                    return Database.Opener.OpenMongo(ConfigurationManager.AppSettings["MONGOLAB_URI"]);
                }
            }

            public static dynamic Issues {
                get {
                    return db.Issues;
                }
            }
        }

        #endregion
    }
}