

using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace signalr_server.Hubs
{
    public class ChatHub : Hub
    {
        public override Task OnConnectedAsync()
        {
            System.Console.WriteLine($"--> Connection Opened: {Context.ConnectionId} ");
            Clients.Client(Context.ConnectionId).SendAsync("ReceivedConnId", Context.ConnectionId);
            return base.OnConnectedAsync();
        }

        public async Task SendMessageAsync(string message)
        {
            
            var routeOb = JsonSerializer.Deserialize<MessageDto>(message);
            Console.WriteLine("To: " + routeOb.To.ToString());
            Console.WriteLine("Message Recieved on: " + Context.ConnectionId);
            if (routeOb.To.ToString() == string.Empty)
            {
                Console.WriteLine("Broadcast");
                await Clients.All.SendAsync("ReceiveMessage", message);
            }
            else
            {
                string toClient = routeOb.To;
                Console.WriteLine("Targeted on: " + toClient);

                await Clients.Client(toClient).SendAsync("ReceiveMessage", message);
            }

        }

    }

    internal class MessageDto
    {
        public string From { get; set; }
        public string To { get; set; }
        public string Message { get; set; }
    }

}