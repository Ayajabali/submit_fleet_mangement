using Fleck;
using Track_project.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;
using System.Text;

namespace Track_project.Services
{
    public class WebSocketManagerService
    {
        private readonly List<IWebSocketConnection> _sockets = new List<IWebSocketConnection>();

        public void Start(string listenerUrl)
        {
            var server = new WebSocketServer(listenerUrl);
            server.Start(socket =>
            {
                socket.OnOpen = () =>
                {
                    Console.WriteLine($"WebSocket opened: {socket.ConnectionInfo.Id}");
                    _sockets.Add(socket);
                };
                socket.OnClose = () =>
                {
                    Console.WriteLine($"WebSocket closed: {socket.ConnectionInfo.Id}");
                    _sockets.Remove(socket);
                };
                socket.OnMessage = message =>
                {
                    Console.WriteLine($"Received message from {socket.ConnectionInfo.Id}: {message}");
                    // Handle message as needed
                };
            });
        }

        public void Broadcast(string message)
        {
            foreach (var socket in _sockets)
            {
                socket.Send(message);
            }
        }
    }
}