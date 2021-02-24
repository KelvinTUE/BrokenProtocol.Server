using BrokenProtocol.Server.Data;
using LogicReinc.Asp;
using System;
using System.Collections.Generic;
using System.Text;

namespace BrokenProtocol.Server
{
    public class BPLifecycle : LifeCycle
    {

        [LifeCycleAction(7000)]
        public void ManagementOnlineStatus()
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
    }
}
