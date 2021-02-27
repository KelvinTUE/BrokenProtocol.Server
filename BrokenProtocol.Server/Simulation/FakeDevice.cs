using BrokenProtocol.Server.Data;
using BrokenProtocol.Shared.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BrokenProtocol.Server.Simulation
{
    public enum DeviceState
    {
        CheckPickup = 0,
        Pickup = 1,
        DetermineObject = 2,
    }
    public class FakeDevice : Device
    {
        private static Random _random = new Random();
        private static ObjectColor[] _colors = new ObjectColor[] { ObjectColor.Black, ObjectColor.White }; 

        private VirtualGroup Group { get; set; }

        private TimeSpan ObjectInterval { get; set; } = TimeSpan.FromSeconds(_random.Next(2, 4));
        private DateTime LastObject { get; set; } = DateTime.Now;

        private TimeSpan PickupInterval { get; set; } = TimeSpan.FromSeconds(_random.Next(1, 3));
        private DateTime PickupTime { get; set; } = DateTime.Now;

        private TimeSpan DetermineInterval { get; set; } = TimeSpan.FromSeconds(_random.Next(2, 4));
        private DateTime DetermineTime { get; set; } = DateTime.Now;


        private DeviceState State { get; set; } = DeviceState.CheckPickup;

        public FakeDevice(VirtualGroup group)
        {
            Group = group;
        }

        public void DoEvents()
        {
            bool objectAvailable = UpdateObjectBelt();
            switch (State)
            {
                case DeviceState.CheckPickup:
                    CheckPickup(objectAvailable);
                    break;
                case DeviceState.Pickup:
                    Pickup();
                    break;
                case DeviceState.DetermineObject:
                    DetermineObject();
                    break;
            }
        }

        private bool UpdateObjectBelt()
        {
            if (ObjectInterval > DateTime.Now.Subtract(LastObject))
            {
                LastObject = DateTime.Now;
                return true;
            }
            return false;
        }

        private void CheckPickup(bool canPickup)
        {
            if (canPickup)
            {
                bool allowedPickup = Group.CanPickup(this);
                if (allowedPickup)
                {
                    PickupTime = DateTime.Now;
                    State = DeviceState.Pickup;
                }
            }
        }
        private void Pickup()
        {
            if(DateTime.Now.Subtract(PickupTime) > PickupInterval)
            {
                DetermineTime = DateTime.Now;
                State = DeviceState.DetermineObject;
                TotalCount++;
            }
        }
        private void DetermineObject()
        {
            if(DateTime.Now.Subtract(DetermineTime) > DetermineInterval)
            {
                ObjectColor color = _colors[_random.Next(_colors.Length)];
                IncrementColor(color);
                State = DeviceState.CheckPickup;
            }
        }
    }
}
