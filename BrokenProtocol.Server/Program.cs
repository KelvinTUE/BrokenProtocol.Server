using BrokenProtocol.Server.Data;
using LogicReinc.Asp;
using LogicReinc.ConsoleUtility;
using LogicReinc.Data.FileIO;
using System;

namespace BrokenProtocol.Server
{
    class Program : ExtendedConsole<Program>
    {
        static AspServer _server = null;
        static BPLifecycle _lifecycle = new BPLifecycle();

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

            _server.EnabledSync = true;

            //Start server
            _server.Start();
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
            _server.Stop();
        }

        /// <summary>
        /// Adds a new user to the database
        /// </summary>
        [Handler("adduser")]
        public static void AddUser(string username, string password, string role)
        {
            User user = new User()
            {
                Username = username,
                Password = password,
                Roles = new string[] { role }
            };

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
