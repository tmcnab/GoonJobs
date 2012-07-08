namespace IDoThis.Helpers
{
    using System.Web;

    public static class MailtoHelpers
    {
        public static string ProfileMailto(dynamic GProfile)
        {
            var subject = string.Format("{0} {1} (GoonJobs Profile)", GProfile.name.givenName, GProfile.name.familyName);
            var body = HttpUtility.UrlEncode(string.Format("View on GoonJobs: {0}", "http://goonjobs.apphb.com/people/details/" + GProfile.hash));

            return string.Format("mailto:?subject={0}&body={1}", subject, body);
        }
    }
}