namespace IronIO.Core
{
    public class AuthToken
    {
        public AuthToken()
        {
            Location = AuthTokenLocation.Header;
            Scheme = "OAuth";
        }

        public AuthTokenLocation Location { get; set; }
        public string Scheme { get; set; }
        public string Token { get; set; }
    }
}