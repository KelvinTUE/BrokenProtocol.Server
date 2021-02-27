using BrokenProtocol.Server.Data;
using BrokenProtocol.Server.Simulation;
using LogicReinc.Asp;
using System;
using System.Collections.Generic;
using System.Text;

namespace BrokenProtocol.Server
{
    public class BPLifecycle : LifeCycle
    {

        [LifeCycleAction(7000)]
        public void PushManagementOnlineStatus()
        {
            foreach(User user in User.Database.ToList())
            {
                try
                {
                    if (user.HasClients())
                    {
                        user.PushOnlineStatus();
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine("LifeCycle ManagementOnlineStatus failure: " + ex.Message);
                }
            }
        }

        [LifeCycleAction(3000)]
        public void PushGroupStatus()
        {
            foreach(User user in User.Database.ToList())
            {
                try
                {
                    if (user.HasClients())
                    {
                        IGroup group = user.GetActiveGroup();
                        user.PushGroupStatus();
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine("Group Status failure: " + ex.Message);
                }
            }
        }

        [LifeCycleAction(1000)]
        public void VirtualSimulation()
        {
            foreach (User user in User.Database.ToList())
            {
                try
                {
                    VirtualGroup group = user.VirtualGroup;
                    if (group != null)
                    {
                        if (group.Expiration > DateTime.Now)
                            user.VirtualGroup?.DoEvents();
                        else
                            user.VirtualGroup = null;
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Simulation failure: " + ex.Message);
                }
            }
        }
    }
}
