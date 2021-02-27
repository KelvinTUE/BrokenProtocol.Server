using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using BrokenProtocol.Client;
using BrokenProtocol.Server.Data;
using BrokenProtocol.Shared.Models;
using LogicReinc.Collections;
using LogicReinc.Data.FileIO;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Program = BrokenProtocol.Server.Program;

namespace BrokenProtocol.Tests
{
    [TestClass]
    public class DeviceControllerTests
    {
        private const int Port = 8888;
        private static readonly string _serverURL = $"http://localhost:{Port}";

        //Setup
        [ClassInitialize]
        public static void SetupServer(TestContext c)
        {
            FileIOProvider provider = new FileIOProvider("TestData");
            User.SetProvider(provider);
            UserDeviceGroup.SetProvider(provider);

            ClearDatabase();

            Program.StartServer(Port);
        }

        [ClassCleanup]
        public static void StopServer()
        {
            ClearDatabase();

            Program.StopServer();
        }

        //Tests

        [TestMethod]
        public void TestCorrectLogin()
        {
            CreateUser("testCorrect");
            Assert.IsNotNull(BrokenProtocolClient.Login(_serverURL, "testCorrect", "password"));
        }

        [TestMethod]
        [ExpectedException(typeof(WebException))]
        public void TestIncorrectLogin()
        {
            CreateUser("testIncorrect");
            BrokenProtocolClient.Login(_serverURL, "test", "err");
        }


        [TestMethod]
        public void TestHeartbeat()
        {
            CreateUser("testHeartbeat");
            var client = BrokenProtocolClient.Login(_serverURL, "testHeartbeat", "password");

            client.Heartbeat();
        }

        [TestMethod]
        public void TestCannotPickUpIfNoGroup()
        {
            CreateUser("testSolo");
            var client = BrokenProtocolClient.Login(_serverURL, "testSolo", "password");

            Assert.IsFalse(client.CanPickup());
        }

        [TestMethod]
        public void TestSimplePickupSolo()
        {
            CreateGroup(1, "solo_pickup");
            var clientOne = BrokenProtocolClient.Login(_serverURL, "solo_pickup_0", "password");
            Assert.IsTrue(clientOne.CanPickup());

            Assert.IsTrue(clientOne.PickedUpObject());

            Assert.IsTrue(clientOne.CanPickup());
        }

        [TestMethod]
        public void TestDetailedPickupSolo()
        {
            CreateGroup(1, "solo_pickup_detailed");
            var clientOne = BrokenProtocolClient.Login(_serverURL, "solo_pickup_detailed_0", "password");
            Assert.IsTrue(clientOne.CanPickup());

            Assert.IsTrue(clientOne.PickedUpObject());

            clientOne.DeterminedObject(new ObjectData {Color = ObjectColor.Black});
        }

        [TestMethod]
        public void TestLog()
        {
            CreateUser("test_log");
            var clientOne = BrokenProtocolClient.Login(_serverURL, "test_log", "password");
            clientOne.Log(new Log() {Message = "TESTY", Tags = new[] {"Meh"}});
        }


        [TestMethod]
        public void TestSensorData()
        {
            CreateUser("test_sensor");
            var clientOne = BrokenProtocolClient.Login(_serverURL, "test_sensor", "password");
            clientOne.SendSensorData(new SensorData()
            {
                Data = new Dictionary<string, object>()
                {
                    {"test", "2"}
                }
            });
        }

        [TestMethod]
        public void TestSimplePickupGroup()
        {
            AppDomain.CurrentDomain.FirstChanceException += (a, b) =>
            {
                object msg = b;
            };

            string groupName = "group_pickup";
            CreateGroup(2, groupName);
            var clientOne = BrokenProtocolClient.Login(_serverURL, groupName + "_0", "password");
            var clientTwo = BrokenProtocolClient.Login(_serverURL, groupName + "_1", "password");
            Assert.IsTrue(clientOne.CanPickup());
            Assert.IsTrue(clientTwo.CanPickup());

            Assert.IsTrue(clientOne.PickedUpObject());

            Assert.IsTrue(clientTwo.CanPickup());
            Assert.IsFalse(clientOne.CanPickup());
        }



        //Utility
        private static UserDeviceGroup CreateGroup(int users, string prefix)
        {
            var group = new UserDeviceGroup
            {
                Name = "Test Group"
            };
            group.Insert();

            for (int i = 0; i < users; i++)
            {
                AttachUserToGroup($"{prefix}_{i}", group);
            }

            return group;
        }

        private static void AttachUserToGroup(string username, UserDeviceGroup group)
        {
            var user = CreateUser(username);

            user.GroupID = group.ObjectID;
            user.Update();
        }

        private static User CreateUser(string username)
        {
            var user = new User { Username = username, Password = "password" };
            user.Insert();
            return user;
        }



        private static void ClearDatabase()
        {
            foreach (User user in User.Database.ToList())
                user.Delete();
            foreach(UserDeviceGroup group in UserDeviceGroup.Database.ToList())
                group.Delete();
        }
    }
}