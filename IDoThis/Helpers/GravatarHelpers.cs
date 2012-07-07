namespace IDoThis.Helpers
{
    using System.Security.Cryptography;
    using System.Security.Principal;
    using System.Text;

    public static class GravatarHelpers
    {
        public static string GravatarProfileLink(this IPrincipal User)
        {
            var baseLink = "http://www.gravatar.com/";
            MD5 md5Hasher = MD5.Create();

            var email = User.Identity.Name.ToLowerInvariant().Trim();
            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(email));

            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++) {
                sBuilder.Append(data[i].ToString("x2"));
            }

            return baseLink + sBuilder.ToString();            
        }

        public static string MD5Hash(this string s)
        {
            MD5 md5Hasher = MD5.Create();
            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(s));

            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++) {
                sBuilder.Append(data[i].ToString("x2"));
            }

            return sBuilder.ToString();
        }
    }
}