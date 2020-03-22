using Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MonitorAgent
{
    class Program
    {
        static private string guid = "{EDCDC221-C412-4792-B6F8-EF4453ADE415}";
        static HttpListener httpListener = new HttpListener();
        private static Mutex signal = new Mutex();
        static async Task Main(string[] args)
        {
            httpListener.Prefixes.Add("http://127.0.0.1:8080/");
            httpListener.Start();
            Console.WriteLine("Server has started on 127.0.0.1:8080");
            while (signal.WaitOne())
            {
                Console.WriteLine("Waiting for a connection...");
                await ReceiveConnection(() => JsonConvert.SerializeObject(GetProcessess()));
            }

            Console.ReadKey();
        }

        private static Dictionary<string, List<ProcessDto>> GetProcessess()
        {
            var processes = Process.GetProcesses()
                .Where(x => x.ProcessName != "Idle")
                .Where(x => x.ProcessName == "chrome");

            var result = new Dictionary<string, List<ProcessDto>>();
            foreach (var process in processes)
            {
                if (!result.ContainsKey(process.ProcessName))
                    result.Add(process.ProcessName, processes.Where(x => x.ProcessName == process.ProcessName).Select(ToDto).ToList());
            }

            return result;
        }

        public static async Task ReceiveConnection(Func<string> onSend)
        {
            HttpListenerContext context = await
            httpListener.GetContextAsync();
            if (context.Request.IsWebSocketRequest)
            {
                if (IsAuthorized(context.Request.Headers["X-Auth-ApiKey"])) {
                    signal.ReleaseMutex();
                    return;
                }

                HttpListenerWebSocketContext webSocketContext = await context.AcceptWebSocketAsync(null);
                WebSocket webSocket = webSocketContext.WebSocket;
                while (webSocket.State == WebSocketState.Open)
                {
                    await webSocket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(onSend())),
                        WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }
            signal.ReleaseMutex();
        }

        private static bool IsAuthorized(string apiKey)
        {
            return string.IsNullOrEmpty(apiKey) || apiKey != guid;
        }

        private static void Print(IDictionary<string, List<ProcessDto>> dtoProcess)
        {
            foreach (var processName in dtoProcess.Keys)
            {
                var currentProcesses = dtoProcess[processName];
                Console.WriteLine($"processName ({currentProcesses.Count()})     {currentProcesses.Sum(x=> x.CPU)}%     {currentProcesses.Sum(x=> x.Memory)}MB");
                foreach (var process in currentProcesses)
                {
                    Print(process);
                }
            }
        }

        private static void Print(ProcessDto dto)
        {
            Console.WriteLine("\t" + dto.Name);
            Console.WriteLine("\t" + dto.Responding);
            Console.WriteLine("\t" + dto.CPU + "%");
            Console.WriteLine("\t" + dto.Memory + "MB");
            Console.WriteLine();
        }

        private static ProcessDto ToDto(Process process) =>
            new ProcessDto {
                Name = process.ProcessName,
                Responding = process.Responding,
                CPU = Math.Round(new PerformanceCounter("Process", "% Processor Time", process.ProcessName).NextValue() / Environment.ProcessorCount, 2),
                Memory = Math.Round(process.WorkingSet64.ConvertBytesToMegabytes(), 2)
            };
    }

    public static class Extensions
    {
        public static double ConvertBytesToMegabytes(this long bytes)
        {
            return (bytes / 1024f) / 1024f;
        }
    }
}

