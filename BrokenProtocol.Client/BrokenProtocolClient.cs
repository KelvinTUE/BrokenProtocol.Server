using System;
using System.Linq;
using System.Net;
using System.Text.Json;
using BrokenProtocol.Client.Model;
using BrokenProtocol.Shared.Models;

namespace BrokenProtocol.Client
{
    public class BrokenProtocolClient
    {
        private readonly string _serverUrl;
        private readonly string _token;

        public static BrokenProtocolClient Login(string serverUrl, string username, string password)
        {
            using (WebClient client = new WebClient())
            {
                client.Headers[HttpRequestHeader.ContentType] = "application/json";

                var credentials = new Credentials(username, password);

                var tokenData = JsonSerializer.Deserialize<ResponseCredentials>(
                    client.UploadString($"{serverUrl}/Authentication/{nameof(Login)}", "POST",
                        JsonSerializer.Serialize(credentials)));

                return new BrokenProtocolClient(serverUrl, tokenData?.Token);
            }
        }

        private BrokenProtocolClient(string serverUrl, string token)
        {
            _serverUrl = serverUrl;
            _token = token;
        }

        private WebClient Client
        {
            get
            {
                if (_token == null)
                {
                    throw new NullReferenceException("Token is null!");
                }

                var client = new WebClient {Headers = {[HttpRequestHeader.ContentType] = "application/json"}};
                client.Headers.Add("auth", _token);
                return client;
            }
        }

        public bool CanPickup()
        {
            using (WebClient client = Client)
            {
                return JsonSerializer.Deserialize<bool>(
                    client.DownloadString($"{_serverUrl}/Device/{nameof(CanPickup)}"));
            }
        }


        public void Heartbeat()
        {
            using (WebClient client = Client)
            {
                client.DownloadString($"{_serverUrl}/Device/{nameof(Heartbeat)}");
            }
        }

        public bool PickedUpObject()
        {
            using (WebClient client = Client)
            {
                return JsonSerializer.Deserialize<bool>(
                    client.UploadString($"{_serverUrl}/Device/{nameof(PickedUpObject)}", "POST", ""));
            }
        }

        public void DeterminedObject(ObjectData data)
        {
            using (WebClient client = Client)
            {
                client.UploadString($"{_serverUrl}/Device/{nameof(DeterminedObject)}", "POST",
                    JsonSerializer.Serialize(data));
            }
        }

        public void Log(Log log)
        {
            using (WebClient client = Client)
            {
                client.UploadString($"{_serverUrl}/Device/{nameof(Log)}", "POST",
                    JsonSerializer.Serialize(log));
            }
        }

        public void SendSensorData(SensorData sensorData)
        {
            using (WebClient client = Client)
            {
                client.UploadString($"{_serverUrl}/Device/{nameof(Log)}", "POST",
                    JsonSerializer.Serialize(sensorData));
            }
        }
    }
}