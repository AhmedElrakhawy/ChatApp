using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Youtube.Sockets.SocketsManager
{
    public abstract class SocketHandler
    {
        public ConnectionManager _Connections { get; set; }
        public SocketHandler(ConnectionManager connections)
        {
            _Connections = connections;
        }
        public virtual async Task OnConnected(WebSocket socket)
        {
            await Task.Run(() => _Connections.AddSocket(socket));
        }
        public virtual async Task OnDisConnected(WebSocket socket)
        {
            await Task.Run(() => _Connections.RemoveSocketAsync(_Connections.GetId(socket)));
        }
        public virtual async Task SendMessage(WebSocket socket , string Message)
        {
            if (socket.State != WebSocketState.Open)
                return;
            await socket.SendAsync(new ArraySegment<byte>(Encoding.ASCII.GetBytes(Message), 0, Message.Length),
                WebSocketMessageType.Text, true, CancellationToken.None);
        }
        public async Task SendMessage(string Id , string Message)
        {
            await SendMessage(_Connections.GetWebSocketById(Id), Message);
        }
        public async Task SendMessageToAll(string Message)
        {
            foreach (var con in _Connections.GetAllConnections())
                await SendMessage(con.Value, Message);
        }
        public abstract Task Receive(WebSocket socket, WebSocketReceiveResult result, byte[] buffer);
    }
}
