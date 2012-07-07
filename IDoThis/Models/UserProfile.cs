namespace IDoThis.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Configuration;
    using System.Security.Principal;
    using IDoThis.Helpers;
    using Simple.Data;
    using Simple.Data.MongoDB;

    public class UserProfile
    {
        #region Fields

        public List<string> AppliedFor { get; set; }

        public string Description { get; set; }

        public bool HasPaid { get; set; }

        public bool IsBanned { get; set; }

        public bool IsHirable { get; set; }

        public DateTime LastLogin { get; set; }

        public List<string> Saved { get; set; }
        
        public string ShowcaseUrl { get; set; }

        public string Username { get; set; }

        public string UsernameHashed { get; set; }

        public string DisplayName { get; set; }

        #endregion

        #region Constructors

        public UserProfile() { }

        public UserProfile(string username)
        {
            this.AppliedFor = new List<string>();
            this.Description = string.Empty;
            this.HasPaid = false;
            this.IsBanned = false;
            this.IsHirable = false;
            this.LastLogin = DateTime.UtcNow;
            this.Saved = new List<string>();
            this.ShowcaseUrl = string.Empty;
            this.Username = username;
            this.UsernameHashed = username.Trim().ToLowerInvariant().MD5Hash();
            this.DisplayName = string.Empty;
        }

        #endregion

        public void UpdateLastLogin()
        {
            this.LastLogin = DateTime.UtcNow;
        }

        public static class DAL
        {
            static dynamic db {
                get {
                    return Database.Opener.OpenMongo(ConfigurationManager.AppSettings["MONGOLAB_URI"]);
                }
            }

            public static dynamic UserProfiles {
                get {
                    return db.UserProfiles;
                }
            }

            public static dynamic PublicProfiles {
                get {
                    return UserProfiles.All().Where(db.UserProfiles.IsHirable == true && 
                                                    db.UserProfiles.IsBanned == false &&
                                                    db.UserProfiles.DisplayName != string.Empty);
                }
            }
        }
    }

    public static class UserProfileExtensions
    {
        public static dynamic Profile(this IPrincipal User)
        {
            return UserProfile.DAL.UserProfiles.FindByUsername(User.Identity.Name);
        }
    }

    public class UserProfileUpdateModel
    {
        [StringLength(100)]
        public string DisplayName { get; set; }

        [StringLength(10000)]
        public string Description { get; set; }

        [StringLength(200)]
        [DataType(DataType.Url)]
        public string ShowcaseUrl { get; set; }

        [Required]
        public bool IsHirable { get; set; }
    }
}