namespace BrokenProtocol.Client.Model
{
    public class LoginRequest
    {
        public LoginRequest(string username, string password)
        {
            User = username;
            Password = password;
        }

        public LoginRequest()
        {
            
        }

        public string User { get; set; }
        public string Password { get; set; }
    }
}