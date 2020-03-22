using Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SampleClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            do
            {
                using (var socket = new ClientWebSocket())
                    try
                    {
                        socket.Options.SetRequestHeader("X-Auth-ApiKey", "{EDCDC221-C412-4792-B6F8-EF4453ADE415}");
                        await socket.ConnectAsync(new Uri("ws://127.0.0.1:8080"), CancellationToken.None);

                        await Receive(socket);

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"ERROR - {ex.Message}");
                    }
            } while (true);
        }

        static async Task Send(ClientWebSocket socket, string data) =>
            await socket.SendAsync(Encoding.UTF8.GetBytes(data), WebSocketMessageType.Text, true, CancellationToken.None);

        static async Task Receive(ClientWebSocket socket)
        {
            var buffer = new ArraySegment<byte>(new byte[2048]);
            do
            {
                WebSocketReceiveResult result;
                using (var ms = new MemoryStream())
                {
                    do
                    {
                        result = await socket.ReceiveAsync(buffer, CancellationToken.None);
                        ms.Write(buffer.Array, buffer.Offset, result.Count);
                    } while (!result.EndOfMessage);

                    if (result.MessageType == WebSocketMessageType.Close)
                        break;

                    ms.Seek(0, SeekOrigin.Begin);
                    using (var reader = new StreamReader(ms, Encoding.UTF8))
                    {
                        var message = await reader.ReadToEndAsync();
                        var processess = JsonConvert.DeserializeObject<Dictionary<string, IList<ProcessDto>>>(message);
                        Console.WriteLine(processess);
                    }
                }
            } while (true);
        }
    }
}
