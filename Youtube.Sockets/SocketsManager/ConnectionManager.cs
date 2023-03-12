using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace Youtube.Sockets.SocketsManager
{
    public class ConnectionManager
    {
        private ConcurrentDictionary<string, WebSocket> _Connections = new ConcurrentDictionary<string, WebSocket>();

        public WebSocket GetWebSocketById(string Id)
        {
            return _Connections.FirstOrDefault(x => x.Key == Id).Value;
        }
        public ConcurrentDictionary<string , WebSocket> GetAllConnections()
        {
            return _Connections;
        }
        public string GetId(WebSocket webSocket)
        {
            return _Connections.FirstOrDefault(x => x.Value == webSocket).Key;
        }
        public async Task RemoveSocketAsync(string Id)
        {
            _Connections.TryRemove(Id, out var socket);
            await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Socket Connection CLosed", CancellationToken.None);
        }
        public void AddSocket(WebSocket socket)
        {
            _Connections.TryAdd(GetConnectionId(), socket);
        }
        private string GetConnectionId()
        {
            return Guid.NewGuid().ToString("N");
        }

    }
}
