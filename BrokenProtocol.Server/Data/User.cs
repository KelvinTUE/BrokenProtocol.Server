using LogicReinc.Data.Unified;
using LogicReinc.Data.Unified.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;

namespace BrokenProtocol.Server.Data
{
    [UnifiedCollection("Users")]
    public class User : UnifiedIMObject<User>
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


        //Authentication
        public int PassItt { get; set; }
        public byte[] Salt { get; set; }
        public string Password { get; set; }


        public string[] Roles { get; set; }




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
            return Index.GetIndex(nameof(Username), username.ToLower()).Cast<User>().FirstOrDefault();
        }
        public static User Login(string username, string password)
        {

            User user = GetUserByName(username);
            //Database.FirstOrDefault(x => x.Username.ToLower() == username.ToLower());


            if (user == null)
                throw new Exception("User does not exist");

            if (user.Hash(password) != user.Password)
                throw new Exception("Wrong credentials");
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
            return LogicReinc.Security.Cryptographics.SecureHash(password, Salt, PassItt, PASSWORD_LENGTH);
        }

    }

    public class UserDevice
    {
        public string Name { get; set; }
    }
}
