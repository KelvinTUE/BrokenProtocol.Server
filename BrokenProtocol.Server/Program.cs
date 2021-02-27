using BrokenProtocol.Server.Data;
using BrokenProtocol.Server.Websockets;
using LogicReinc.Asp;
using LogicReinc.ConsoleUtility;
using LogicReinc.Data.FileIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace BrokenProtocol.Server
{
    public class Program : ExtendedConsole<Program>
    {
        static AspServer _server = null;
        static readonly BPLifecycle _lifecycle = new BPLifecycle();

        public static AspServer Server => _server;

        public static bool Active { get; private set; } = true;

        public static void Main(string[] args)
        {
            FileIOProvider provider = new FileIOProvider("data");
            User.SetProvider(provider);
            UserDeviceGroup.SetProvider(provider);

            StartServer(Settings.Instance.Port);

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
            StopServer();
        }

        public static void StopServer()
        {
            _lifecycle.Stop();
            _ = _server.Stop();
        }

        public static void StartServer(int port)
        {
            //Setup server
            _server = new AspServer(port);
            _server.AddAssemblies(typeof(Program).Assembly);
            _server.SetAuthentication(new AuthService());
            _server.AddStaticDirectory("", "Files");
            _server.SetJsonOptions((options) => options.JsonSerializerOptions.PropertyNamingPolicy = null);
            _server.AddEndpoint("index.html", httpContext => httpContext.Response.Redirect("/Index.html"));
            _server.AddWebSocketAuthenticated<ManagementSocket>("/ws/ManagementSocket", "ManagementSocket");


            _server.EnabledSync = true;

            //Start server
            _ =_server.Start();
            _lifecycle.Start();
        }

        /// <summary>
        /// Adds a new user to the database
        /// </summary>
        [Handler("adduser")]
        public static void AddUser(string username, string password, string deviceName, string role)
        {
            User user = new User
            {
                Username = username, 
                Password = password, 
                Roles = new List<string>()
                {
                    role
                }, 
                Device = new UserDevice() 
                {
                    Name = deviceName
                }
            };

            user.Insert();
            Console.WriteLine("Created user");
        }

        [Handler("toggleadmin")]
        public static void MakeAdmin(string username)
        {
            User user = User.Database.FirstOrDefault(x => x.Username.ToLower() == username.ToLower());
            if(user == null)
            {
                Console.WriteLine("User does not exist");
            }
            else
            {
                if(user.Roles.Contains("admin"))
                {
                    user.Roles.Remove("admin");
                    Console.WriteLine($"{user.Username} is no longer an admin");
                }
                else
                {
                    user.Roles.Add("admin");
                    Console.WriteLine($"{user.Username} is now an admin");
                }
                user.Update();
            }
        }

        [Handler("fixuserdevices")]
        public static void FixDeviceUserIDs()
        {
            foreach(User user in User.Database)
            {
                user.Device.UserID = user.ObjectID;
                if (string.IsNullOrEmpty(user.Device.Name))
                    user.Device.Name = user.Username;
                user.Update();
            }
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
