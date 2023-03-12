using Microsoft.AspNetCore.Http;
using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace Youtube.Sockets.SocketsManager
{
    public class SocketMiddleware
    {
        private readonly RequestDelegate _Next;
        private SocketHandler _handler { get; set; }
        public SocketMiddleware(RequestDelegate Next , SocketHandler handler)
        {
            _Next = Next;
            _handler = handler;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.WebSockets.IsWebSocketRequest)
                return;

            var Socket = await context.WebSockets.AcceptWebSocketAsync();
            await _handler.OnConnected(Socket);
            await Recieve(Socket, async (result, buffer) => {
                if (result.MessageType == WebSocketMessageType.Text)
                {
                    await _handler.Receive(Socket, result, buffer);
                }
                else if (result.MessageType == WebSocketMessageType.Close)
                {
                    await _handler.OnDisConnected(Socket);
                }
            });
        }

        private async Task Recieve(WebSocket webSocket, Action<WebSocketReceiveResult, byte[]> messageToHandler)
        {
            var buffer = new byte[1024 * 4];
            while (webSocket.State == WebSocketState.Open)
            {
                var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                messageToHandler(result, buffer);
            }
        }
    }
}
