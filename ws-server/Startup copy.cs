using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ws_server
{
    public class Startupex
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseWebSockets();

            app.Use(async (context, next) =>
            {

                WriteRequestParam(context, env);

                if (context.WebSockets.IsWebSocketRequest)
                {
                    var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                    System.Console.WriteLine("WebSocket Connected");

                    await ReceiveMessage(webSocket, async(result,buffer) => {

                        if (result.MessageType == WebSocketMessageType.Text)
                        {
                            System.Console.WriteLine($"Receive --> Text");
                            return;
                        }
                        else if (result.MessageType == WebSocketMessageType.Close)
                        {
                            System.Console.WriteLine($"Receive --> Close");
                        }                       


                    });

                }
                else
                {
                    System.Console.WriteLine("Hello from 2nd Request - No web socket");
                    await next();
                }

            });

            app.Run(async context =>
            {
                System.Console.WriteLine("Hello from 3d (terminal) reques");
                await context.Response.WriteAsync("Hello from 3rd (terminal) Request Delegate");
            });

        }
        public void WriteRequestParam(HttpContext context, IWebHostEnvironment env)
        {

            if (env.IsDevelopment())
            {
                System.Console.WriteLine("Request Method: " + context.Request.Method);
                System.Console.WriteLine("Request Protocol: " + context.Request.Protocol);

                if (context.Request.Headers != null)
                {

                    System.Console.WriteLine("Request Headers:");

                    foreach (var h in context.Request.Headers)
                    {
                        System.Console.WriteLine($"--> {h.Key}: {h.Value}");
                    }
                }
            }

        }

        private async Task ReceiveMessage(WebSocket socket, Action<WebSocketReceiveResult, byte[]> handleMessage)
        {

            var buffer = new byte[1024 * 4];

            while (socket.State == WebSocketState.Open)
            {
                var result = await socket.ReceiveAsync(buffer: new ArraySegment<byte>(buffer),
                                                    cancellationToken: System.Threading.CancellationToken.None);

                handleMessage(result, buffer);

            }

        }
    }
}
