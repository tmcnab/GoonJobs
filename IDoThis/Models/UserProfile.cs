﻿namespace IDoThis.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Configuration;
    using System.Security.Principal;
    using System.Web.Mvc;
    using IDoThis.Helpers;
    using Simple.Data;
    using Simple.Data.MongoDB;

    public class UserProfile
    {
        #region Fields

        public string Description { get; set; }

        public string DescriptionSearchable { get; set; }

        public bool HasPaid { get; set; }

        public bool IsBanned { get; set; }

        public bool IsHirable { get; set; }

        public DateTime LastLogin { get; set; }

        public string Username { get; set; }

        public string UsernameHashed { get; set; }

        #endregion

        #region Constructors

        public UserProfile() { }

        public UserProfile(string username)
        {
            this.Description = string.Empty;
            this.DescriptionSearchable = string.Empty;
            this.HasPaid = false;
            this.IsBanned = false;
            this.IsHirable = false;
            this.LastLogin = DateTime.UtcNow;
            this.Username = username;
            this.UsernameHashed = username.Trim().ToLowerInvariant().MD5Hash();
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
                                                    db.UserProfiles.IsBanned == false);
                }
            }

            public static dynamic Search(string query)
            {
                var tokens = query.Split(' ', ',');
                var model = PublicProfiles;

                foreach (var keyword in query.ToLowerInvariant().Split(' '))
                {
                    model = model.Where(db.UserProfiles.DescriptionSearchable.Contains(keyword));
                }

                return model;
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
        [AllowHtml]
        [StringLength(10000)]
        public string Pitch { get; set; }

        [Required]
        public bool IsHirable { get; set; }
    }
}