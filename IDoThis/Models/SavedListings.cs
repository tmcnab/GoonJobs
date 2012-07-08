namespace IDoThis.Models
{
    using System.Configuration;
    using System.Security.Principal;
    using Simple.Data;
    using Simple.Data.MongoDB;

    public class SavedListing
    {
        #region Fields

        public string Slug { get; set; }

        public string Username { get; set; }

        #endregion

        #region Constructors

        public SavedListing() { }

        public SavedListing(IPrincipal User, string slug)
        {
            this.Slug = slug;
            this.Username = User.Identity.Name;
        }

        #endregion

        public static class DAL
        {
            static dynamic db {
                get {
                    return Database.Opener.OpenMongo(ConfigurationManager.AppSettings["MONGOLAB_URI"]);
                }
            }

            public static dynamic SavedListings {
                get {
                    return db.SavedListings;
                }
            }

            public static dynamic SavedListingsForUser(IPrincipal User)
            {
                return db.SavedListings.FindAllByUsername(User.Identity.Name);
            }
        }
    }

    public static class SavedListingExtensions
    {
        public static int SavedListingCount(this IPrincipal User)
        {
            return SavedListing.DAL.SavedListingsForUser(User)
                               .ToList().Count;
        }

        public static void SaveListing(this IPrincipal User, string slug)
        {
            var data = SavedListing.DAL.SavedListings.FindByUsernameAndSlug(User.Identity.Name, slug);
            if (data == null) {
                data = new SavedListing(User, slug);
                SavedListing.DAL.SavedListings.Insert(data);
            }
        }

        public static void UnSaveListing(this IPrincipal User, string slug)
        {
            SavedListing.DAL.SavedListings.DeleteByUsernameAndSlug(User.Identity.Name, slug);
        }
    }
}