using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebSockets.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            if (Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).Count() <= 2)
            {
                Process.Start("WebSockets.Console.exe");
            }
            StartWebSockets().GetAwaiter().GetResult();
        }
        public static async Task StartWebSockets()
        {
            var Client = new ClientWebSocket(); 
            await Client.ConnectAsync(new Uri("ws://localhost:5000/ws"), CancellationToken.None);
            System.Console.WriteLine($"Web sockets Connection established @ {DateTime.UtcNow:F}");
            var Send = Task.Run(async () =>
            {
                string Message;
                while ((Message = System.Console.ReadLine()) != null && Message != string.Empty)
                {
                    var bytes = Encoding.UTF8.GetBytes(Message);
                    await Client.SendAsync(new ArraySegment<byte>(bytes),
                        WebSocketMessageType.Text, true, CancellationToken.None);
                }
                await Client.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
            });
            var Recieve = RecieveAsync(Client);
            await Task.WhenAll(Send, Recieve);
        }
        public static async Task RecieveAsync(ClientWebSocket client)
        {
            var buffer = new byte[1024 * 4];
            while (true)
            {
                var result = await client.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                System.Console.WriteLine(Encoding.UTF8.GetString(buffer , 0 , result.Count));
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await client.CloseOutputAsync(WebSocketCloseStatus.NormalClosure,"" , CancellationToken.None);
                    break;
                }
            }
        }
    }
}
