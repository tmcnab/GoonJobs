namespace IDoThis.Models
{
    using System;
    using System.Net;
    using System.Security.Principal;
    using System.Text;
    using Codeplex.Data;
    using IDoThis.Helpers;

    public static class Gravatar
    {
        public static dynamic Profile(IPrincipal User)
        {
            return Profile(User.Identity.Name.MD5Hash());
        }

        public static dynamic Profile(string emailHash)
        {
            try
            {
                var url = string.Format("https://en.gravatar.com/{0}.json", emailHash);
                var data = UTF8Encoding.UTF8.GetString((new WebClient()).DownloadData(url))
                                       .Replace("{\"entry\":[", string.Empty)
                                       .TrimEnd('}')
                                       .TrimEnd(']');
                return DynamicJson.Parse(data);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}