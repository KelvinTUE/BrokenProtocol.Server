using BrokenProtocol.Server.Websockets;
using BrokenProtocol.Server.Websockets.Packets;
using LogicReinc.Asp.Authentication;
using LogicReinc.Collections;
using LogicReinc.Data.Unified;
using LogicReinc.Data.Unified.Attributes;
using LogicReinc.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using BrokenProtocol.Shared.Models;

namespace BrokenProtocol.Server.Data
{
    [UnifiedCollection("Users")]
    public class User : UnifiedIMObject<User>, IAuthUser
    {
        private static readonly TimeSpan ACTIVITY_TIMEOUT = TimeSpan.FromSeconds(5);

        private static Random _random = new Random();
        private const int PASSWORD_LENGTH = 32;

        [UnifiedIMIndex]
        public string Username { get; set; }

        public DateTime LastActivity { get; set; }
        public bool IsOnline => DateTime.Now.Subtract(LastActivity) < ACTIVITY_TIMEOUT;

        public string GroupID { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        [JsonIgnore]
        [UnifiedIMReference(nameof(GroupID), typeof(DeviceGroup), nameof(DeviceGroup.ObjectID))]
        public DeviceGroup Group { get; set; }

        public Device Device { get; set; } = new Device();

        private TSList<ManagementSocket> ActiveClients { get; set; } = new TSList<ManagementSocket>();

        //Authentication
        public int PassItt { get; set; }
        public byte[] Salt { get; set; }
        public string Password { get; set; }


        public string[] Roles { get; set; }

        //Socket
        public void AddClient(ManagementSocket socket)
        {
            ActiveClients.Add(socket);
        }
        public void RemoveClient(ManagementSocket socket)
        {
            ActiveClients.Remove(socket);
        }
        public bool HasClients()
        {
            return ActiveClients.Length > 0;
        }

        public void Send(ManagementMessage message)
        {
            ActiveClients.Where(x => x.Active).ForEach(x =>
            {
                try
                {
                    _ = x.SendAsync(message);
                }
                catch(Exception ex)
                {
                    //Ignore
                    Console.WriteLine($"Failed to send message of type [{message.Type} to {x.Address}");
                }
            });
        }

        public void Log(Log log)
        {
            Send(ManagementMessage.FromObject(log));
        }

        public void SensorData(SensorData sensorData)
        {
            Send(ManagementMessage.FromObject(sensorData));
        }

        public void PushOnlineStatus()
        {
            Send(ManagementMessage.FromObject(new UserStatus()
            {
                Online = IsOnline,
                LastActivity = LastActivity
            }));
        }

        //Overrides
        public override bool Insert()
        {
            PassItt = _random.Next(10, 15);
            Salt = new byte[32];
            _random.NextBytes(Salt);

            Password = Hash(Password);

            Username = Username.ToLower();

            return base.Insert();
        }
        public override bool Update()
        {
            Username = Username.ToLower();
            return base.Update();
        }


        public void UpdateActivity()
        {
            LastActivity = DateTime.Now;
            Update();
        }


        //Fetching
        public static User GetUserByName(string username)
        {
            if (username == null)
                throw new ArgumentNullException(nameof(username),"Missing username, ensure 'User' property");
            return Index.GetIndex(nameof(Username), username.ToLower()).Cast<User>().FirstOrDefault();
        }
        public static User Login(string username, string password)
        {

            User user = GetUserByName(username);
            //Database.FirstOrDefault(x => x.Username.ToLower() == username.ToLower());


            if (user == null)
                return null;
            //throw new ArgumentException("User does not exist");

            if (user.Hash(password) != user.Password)
                return null;
                //throw new ArgumentException("Wrong credentials");
            return user;
        }



        //Model conversion
        public UserDevice GetDevice() => new UserDevice()
        {
            Name = Username
        };


        //Private
        private string Hash(string password)
        {
            return Cryptographics.SecureHash(password, Salt, PassItt, PASSWORD_LENGTH);
        }

        public string[] GetRoles()
        {
            return Roles;
        }

        public string GetID()
        {
            return ObjectID;
        }
    }

    public class UserDevice
    {
        public string Name { get; set; }
    }
}
