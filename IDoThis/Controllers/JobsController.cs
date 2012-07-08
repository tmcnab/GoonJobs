﻿namespace IDoThis.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.ServiceModel.Syndication;
    using System.Web.Mvc;
    using AttributeRouting.Web.Mvc;
    using IDoThis.Controllers.Results;
    using IDoThis.Models;

    public class JobsController : Controller
    {
        [Route("jobs/", HttpVerbs.Get)]
        [Route("jobs/list/{?pg}/", HttpVerbs.Get)]
        public ActionResult Index(int pg = 0, string q = "")
        {
            dynamic model;

            if (!string.IsNullOrWhiteSpace(q)) {
                model = JobListing.DAL.Search(q);
            }
            else {
                model = JobListing.DAL.Active.OrderByPostedDescending();
            }

            ViewBag.Page = pg;
            ViewBag.Total = model.Count;
            ViewBag.NPerPage = 12;

            return View(model.Skip(pg * ViewBag.NPerPage).Take(ViewBag.NPerPage));
        }

        #region Adding a Job Listing

        [Authorize]
        [Route("jobs/add", HttpVerbs.Get)]
        public ActionResult Add()
        {
            if (!User.Profile().HasPaid)
            {
                return View("402");
            }
            return View();
        }

        [Authorize]
        [Route("jobs/add", HttpVerbs.Post)]
        public ActionResult Add(JobListingViewModel model)
        {
            if (ModelState.IsValid)
            {
                var item = model.ToJobListing(User);
                JobListing.DAL.Listings.Insert(item);
                return RedirectToAction("Details", new { slug = item.Slug });
            }
            else
            {
                return View(model);
            }
        }

        #endregion

        #region Job Listing Details

        [Route("jobs/details/{slug}", HttpVerbs.Get)]
        public ActionResult Details(string slug)
        {
            var model = JobListing.DAL.Listings.FindBySlug(slug);
            if (model == null) {
                return View("404");
            }
            else {
                return View(model);
            }
        }

        #endregion

        #region Applying for Jobs

        [Route("jobs/apply/{slug}", HttpVerbs.Get)]
        public ActionResult Apply(string slug)
        {
            var model = JobListing.DAL.Listings.FindBySlug(slug);
            if (model == null) {
                return View("404");
            }
            else {
                ViewBag.Errors = false;
                return View(model); 
            }
        }

        [Route("jobs/apply/{slug}", HttpVerbs.Post)]
        public ActionResult Apply(string slug, JobApplicationViewModel model)
        {
            var job = JobListing.DAL.Listings.FindBySlug(slug);
            if (job == null) {
                return View("404");
            }

            if (ModelState.IsValid) {
                return Redirect("/profile");
            }
            else
            {
                ViewBag.Errors = true;
                return View(job);
            }
        }

        #endregion

        #region Saving Jobs for Later

        [Authorize]
        [Route("jobs/save/{slug}", HttpVerbs.Get)]
        public ActionResult SaveJob(string slug)
        {
            User.SaveListing(slug);
            return RedirectToAction("Details", new { slug = slug });
        }

        [Authorize]
        [Route("jobs/unsave/{slug}", HttpVerbs.Get)]
        public ActionResult UnSaveJob(string slug)
        {
            User.UnSaveListing(slug);
            return RedirectToAction("Index", "Profile");
        }

        #endregion

        #region Delisting Jobs

        [Authorize]
        [Route("jobs/delist/{slug}", HttpVerbs.Get)]
        public ActionResult Delist(string slug)
        {
            var model = JobListing.DAL.Listings.FindBySlugAndUser(slug, User.Identity.Name);
            if (model == null)
            {
                return View("404");
            }
            else
            {
                model.Expires = DateTime.MinValue;
                JobListing.DAL.Listings.Update(model);
                return RedirectToAction("Index", "Profile");
            }
        }

        #endregion

        #region RSS

        [OutputCache(Duration = 60)]
        [Route("jobs/rss", HttpVerbs.Get)]
        public ActionResult RSSFirehose()
        {
            var model = new List<SyndicationItem>();
            foreach (var item in JobListing.DAL.Listings.All().OrderByPostedDescending())
            {
                model.Add(new SyndicationItem(item.Title, item.Blurb, new Uri("http://goonjobs.apphb.com/jobs/details/" + item.Slug)));
            }

            var feed = new SyndicationFeed("GoonJobs Firehose", "Newest job listings from GoonJobs", new Uri("http://goonjobs.apphb.com/jobs/rss"), model);

            return new FeedResult(new Rss20FeedFormatter(feed));
        }


        #endregion
    }
}