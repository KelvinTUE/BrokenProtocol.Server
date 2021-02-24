namespace BrokenProtocol.Client.Model
{
    public class Credentials
    {
        public Credentials(string username, string password)
        {
            User = username;
            Password = password;
        }

        public Credentials()
        {
            
        }

        public string User { get; set; }
        public string Password { get; set; }
    }
}