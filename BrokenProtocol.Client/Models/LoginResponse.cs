using System.Text.Json.Serialization;

namespace BrokenProtocol.Client.Model
{
    public class LoginResponse
    {
        public string Token { get; set; }
    }
}