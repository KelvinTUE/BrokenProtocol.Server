using BrokenProtocol.Server.Data;
using BrokenProtocol.Server.Websockets;
using LogicReinc.Asp;
using LogicReinc.ConsoleUtility;
using LogicReinc.Data.FileIO;
using System;

namespace BrokenProtocol.Server
{
    public class Program : ExtendedConsole<Program>
    {
        static AspServer _server = null;
        static BPLifecycle _lifecycle = new BPLifecycle();

        public static AspServer Server => _server;

        public static bool Active { get; private set; } = true;

        static void Main(string[] args)
        {
            FileIOProvider provider = new FileIOProvider("Data");
            User.SetProvider(provider);


            //Setup server
            _server = new AspServer(Settings.Instance.Port);
            _server.AddAssemblies(typeof(Program).Assembly);
            _server.SetAuthentication(new AuthService());
            _server.AddStaticDirectory("", "Files");
            _server.SetJsonOptions((options) => options.JsonSerializerOptions.PropertyNamingPolicy = null);
            _server.AddEndpoint("index.html", httpContext => httpContext.Response.Redirect("/Index.html"));
            _server.AddWebSocketAuthenticated<ManagementSocket>("/ws/management", "Management");
            
            _server.EnabledSync = true;

            //Start server
            _ =_server.Start();
            _lifecycle.Start();


            //TODO: Add read timeout to properly handle Ctrl+C
            string line = null;
            while (Active)
            {
                line = Console.ReadLine();
                try
                {
                    HandleCommand(line);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception: [{ex.GetType().Name}]: {ex.Message}");
                }
            }

            //Stop server
            _lifecycle.Stop();
            _ = _server.Stop();
        }

        /// <summary>
        /// Adds a new user to the database
        /// </summary>
        [Handler("adduser")]
        public static void AddUser(string username, string password, string deviceName, string role)
        {
            User user = new User()
            {
                Username = username,
                Password = password,
                Roles = new string[] { role }
            };
            user.Device.Name = deviceName;

            user.Insert();
            Console.WriteLine("Created user");
        }

        /// <summary>
        /// Stop the application
        /// </summary>
        [Handler("exit")]
        public static void Exit()
        {
            Active = false;
        }
    }
}
