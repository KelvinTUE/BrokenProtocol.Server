using BrokenProtocol.Server.Data;
using BrokenProtocol.Server.Websockets.Packets;
using LogicReinc.Asp;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace BrokenProtocol.Server.Websockets
{
   public  class ManagementSocket : WebSocketClient
    {
        private CancellationTokenSource _cToken = new CancellationTokenSource();

        public User User { get; private set; }


        public ManagementSocket(string group, AspServer server, HttpContext req, WebSocket socket) : base(group, server, req, socket)
        {
            User = req.GetAuthenticatedUser();
            if (User == null)
                throw new InvalidOperationException("Missing Authentication");
        }

        public override void OnConnected()
        {
            User.AddClient(this);
            base.OnConnected();
        }

        public override void OnDisconnected()
        {
            User.RemoveClient(this);
            base.OnDisconnected();
            _cToken.Cancel();
        }


        public async Task SendAsync(ManagementMessage msg)
        {
            byte[] data = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(msg));
            await base.Socket.SendAsync(data, WebSocketMessageType.Text, true, _cToken.Token);
        }
    }
}
