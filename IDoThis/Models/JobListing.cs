namespace IDoThis.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Configuration;
    using System.Linq;
    using System.Security.Principal;
    using System.Text;
    using System.Web.Mvc;
    using Simple.Data;
    using Simple.Data.MongoDB;

    public class JobListing
    {
        #region Fields

        public string Slug { get; set; }                                        // Job Listing Slug

        public DateTime Posted { get; set; }                                    // When the job was posted

        public DateTime Expires { get; set; }                                   // When the job listing expires

        public string Title { get; set; }                                       // Job Title

        public string TitleSearchable { get; set; }                             // Searchable Title

        public string Blurb { get; set; }                                       // Really short basic description

        public string BlurbSearchable { get; set; }                             // Searchable blurb

        public string Location { get; set; }                                    // Where located
                 
        public string LocationSearchable { get; set; }

        public string Body { get; set; }                               

        public string User { get; set; }                                        // The user who posted the job listing

        public string Who { get; set; }

        #endregion

        #region DAL

        public static class DAL
        {
            static dynamic db {
                get {
                    return Database.Opener.OpenMongo(ConfigurationManager.AppSettings["MONGOLAB_URI"]);
                }
            }

            public static dynamic Listings {
                get {
                    return db.Listings;
                }
            }

            public static dynamic Active {
                get {
                    return db.Listings.All().Where(db.Listings.Expires > DateTime.UtcNow.Date);
                }
            }

            public static dynamic Search(string query)
            {
                var tokens = query.Split(' ', ',');
                var model = Active;

                foreach (var keyword in query.ToLowerInvariant().Split(' ')) {
                    model = model.Where(db.Listings.TitleSearchable.Contains(keyword) || 
                                        db.Listings.LocationSearchable.Contains(keyword) ||
                                        db.Listings.BlurbSearchable.Contains(keyword));
                }

                return model;
            }
        }

        #endregion
    }

    public class JobListingViewModel
    {
        [Required(ErrorMessage = "A job title is required")]
        [StringLength(50)]
        public string Title { get; set; }

        [Required(ErrorMessage = "A blurb is required")]
        [StringLength(140)]
        public string Blurb { get; set; }

        [Required(ErrorMessage = "The name of who is hiring is required.")]
        [StringLength(50)]
        public string Who { get; set; }

        [StringLength(50)]
        public string Where { get; set; }

        [AllowHtml]
        [Required(ErrorMessage = "The job details are required.")]
        [StringLength(10000)]
        public string Body { get; set; }

        public JobListing ToJobListing(IPrincipal User)
        {
            StringBuilder b = new StringBuilder();
            foreach(var s in this.Blurb.ToLowerInvariant().Replace(',',' ').Replace('.', ' ').Split(' ').Distinct())
            {
                b.Append(s);
                b.Append(' ');
            }

            return new JobListing()
            {
                Blurb = this.Blurb,
                BlurbSearchable = b.ToString().Trim(),
                Body = this.Body,
                Expires = DateTime.UtcNow.AddDays(60),
                Title = this.Title,
                TitleSearchable = this.Title.ToLowerInvariant().Replace('/', ' '),
                Posted = DateTime.UtcNow,
                Slug = Guid.NewGuid().ToString("N").Substring(0, 10),
                Location = this.Where,
                LocationSearchable = this.Where.ToLowerInvariant().Replace(',', ' '),
                User = User.Identity.Name,
                Who = this.Who
            };
        }
    }

    public class JobApplicationViewModel
    {
        [Required]
        [StringLength(100)]
        public string full_name { get; set; }

        [Required]
        [AllowHtml]
        [StringLength(10000)]
        public string  application_letter { get; set; }
    }
}