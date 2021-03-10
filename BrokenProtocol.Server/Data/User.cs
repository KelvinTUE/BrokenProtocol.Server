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
using BrokenProtocol.Server.Simulation;

namespace BrokenProtocol.Server.Data
{
    [UnifiedCollection("Users")]
    public class User : UnifiedIMObject<User>, IAuthUser
    {
        private static Random _random = new Random();
        private const int PASSWORD_LENGTH = 32;

        [UnifiedIMIndex]
        public string Username { get; set; }

        public string GroupID { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        [JsonIgnore]
        [UnifiedIMReference(nameof(GroupID), typeof(UserDeviceGroup), nameof(UserDeviceGroup.ObjectID))]
        public UserDeviceGroup Group { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        [JsonIgnore]
        public VirtualGroup VirtualGroup { get; set; } = null;

        public UserDevice Device { get; set; } = new UserDevice();

        public bool IsAdmin => Roles?.Contains("admin") ?? false;

        private TSList<ManagementSocket> ActiveClients { get; set; } = new TSList<ManagementSocket>();

        //Authentication
        public int PassItt { get; set; }
        public byte[] Salt { get; set; }
        public string Password { get; set; }

        public int[] ActivityChecks = new int[365];

        public List<string> Roles { get; set; } = new List<string>();

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
                Online = Device?.IsOnline ?? false,
                LastActivity = Device?.LastActivity ?? DateTime.MinValue
            }));
        }

        public void PushGroupStatus()
        {
            Send(new ManagementMessage()
            {
                Type = "GroupStatus",
                Data = GetActiveGroup()
            });
        }

        public static void PushAdminMessage(ManagementMessage message)
        {
            List<User> users = Database.Where(x => x.IsAdmin && x.HasClients());
            foreach (User user in users)
            {
                try
                {
                    user.Send(message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed to send management message to admin");
                }
            }
        }
        public static void PushGroupsStatusToAdmins(List<UserDeviceGroup> groups)
        {
            PushAdminMessage(new ManagementMessage()
            {
                Type = "AdminGroupsStatus",
                Data = groups
            });
        }
        public static void PushAdminUserLog(string user, string type, string log)
        {
            try
            {
                PushAdminMessage(new ManagementMessage()
                {
                    Type = "UserLog",
                    Data = new UserLogModel()
                    {
                        UserID = user,
                        Log = new Log()
                        {
                            Tags = new string[] { type },
                            Message = $"{log}"
                        }
                    }
                });
            }
            catch(Exception ex)
            {
                Console.WriteLine("Failed to send admin log");
            }
        }
        public void PushAdminGroupsStatus(List<UserDeviceGroup> groups)
        {
            Send(new ManagementMessage()
            {
                Type = "AdminGroupsStatus",
                Data = groups
            });
        }

        //Others
        public IGroup GetActiveGroup()
        {
            IGroup group = null;
            if (GroupID != null)
                group = Group;

            return group ?? VirtualGroup;
        }

        public void CreateVirtualGroup(VirtualGroupOptions options, TimeSpan expiration)
        {
            VirtualGroup = new VirtualGroup(this, options, expiration);
        }
        public void DeleteVirtualGroup()
        {
            VirtualGroup = null;
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
            if(Device != null)
                Device.LastActivity = DateTime.Now;

            if (ActivityChecks == null)
                ActivityChecks = new int[365];
            lock (ActivityChecks)
            {
                ActivityChecks[DateTime.Now.DayOfYear]++;
            }

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
        public UserDeviceModel GetDevice() => new UserDeviceModel()
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
            return Roles?.ToArray() ?? new string[0];
        }

        public string GetID()
        {
            return ObjectID;
        }

        public UserModel ToModel()
        {
            lock (ActivityChecks)
            {
                return new UserModel()
                {
                    GroupID = GroupID,
                    GroupName = (GroupID != null ? Group : null)?.Name,
                    DeviceName = Device?.Name,
                    Name = Username,
                    ObjectID = ObjectID,
                    Activity = ActivityChecks.ToArray()
                };
            }
        }
    }

    public class UserDeviceModel
    {
        public string Name { get; set; }
    }
}
