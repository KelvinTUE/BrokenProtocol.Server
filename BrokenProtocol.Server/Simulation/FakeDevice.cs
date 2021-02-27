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
        public override bool IsOnline => true;

        private static Random _random = new Random();
        private static ObjectColor[] _colors = new ObjectColor[] { ObjectColor.Black, ObjectColor.White }; 

        private VirtualGroup Group { get; set; }

        private TimeSpan ObjectInterval { get; set; }
        private DateTime LastObject { get; set; } = DateTime.Now;

        private TimeSpan PickupInterval { get; set; }
        private DateTime PickupTime { get; set; } = DateTime.Now;

        private TimeSpan DetermineInterval { get; set; }
        private DateTime DetermineTime { get; set; } = DateTime.Now;

        private DeviceState State { get; set; } = DeviceState.CheckPickup;

        private VirtualGroupOptions _options = null;

        public FakeDevice(string name, VirtualGroup group)
        {
            Name = name;
            Group = group;
            _options = group.Options;

            int obj = Math.Max(1, _options.SecondsObjects);
            int pickup = Math.Max(1, _options.SecondsPickup);
            int determine = Math.Max(1, _options.SecondsDetermine);

            ObjectInterval = TimeSpan.FromSeconds(_random.Next(obj, obj + _options.SecondsVariance + 1));
            PickupInterval = TimeSpan.FromSeconds(_random.Next(pickup, pickup + _options.SecondsVariance + 1));
            DetermineInterval = TimeSpan.FromSeconds(_random.Next(determine, determine + _options.SecondsVariance + 1));
        }

        public void DoEvents()
        {
            bool objectAvailable = UpdateObjectBelt();
            switch (State)
            {
                case DeviceState.CheckPickup:
                    if(_options.AllowOfflinePickup || Group.Device.IsOnline)
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
            if (ObjectInterval < DateTime.Now.Subtract(LastObject))
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
