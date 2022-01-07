using System;
using System.Collections.Concurrent;
using System.Net.WebSockets;

namespace ws_server
{
    public class WebSocketServerConnectionManager
    {
        private ConcurrentDictionary<string, WebSocket> _sockets = new ConcurrentDictionary<string, WebSocket>();

        public string AddSocket(WebSocket socket)
        {

            string connId = Guid.NewGuid().ToString();

            _sockets.TryAdd(connId, socket);
            System.Console.WriteLine("");
            Console.WriteLine($"WebSocketServerConnectionManager-> AddSocket: WebSocket added with ID: {connId} ");

            return connId;
        }

        public ConcurrentDictionary<string, WebSocket> GetAllSockets()
        {
            return _sockets;
        }
    }
}