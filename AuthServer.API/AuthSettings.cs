namespace Expertec.Sigeco.AuthServer.API
{
    public class AuthSettings
    {
        public string ReturnUrl { get; set; }

        public ConnectionStringsSettings ConnectionStrings { get; set; }

        public ServicesSettings Services {get; set; }
        
    }

    public class ConnectionStringsSettings
    {
        public string Api { get; set; }
    }

    public class ServicesSettings
    {
        public string AdminUrl { get; set; }
    }
}