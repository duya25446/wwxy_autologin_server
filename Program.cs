
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;

namespace server
{
    public class Invoke
    {
        public class TcpServer
        {
            private TcpListener _listener;
            Invoke _invoke = new Invoke();

            public TcpServer(IPAddress ip, int port)
            {
                _listener = new TcpListener(ip, port);
            }
            public TcpServer() : this(IPAddress.Parse("0.0.0.0"), 25446)
            {
            }

            private async Task HandleConnection(TcpClient client)
            {
                var IDall = "";
                try
                {
                    client.ReceiveTimeout = 10 * 1000;
                    client.SendTimeout = 10 * 1000;
                    client.NoDelay = true;
                    await using var networkStream = client.GetStream();
                    var sr = new StreamReader(networkStream);
                    var sw = new StreamWriter(networkStream, Encoding.ASCII);
                    await sw.WriteLineAsync($"Welcome Duya25446 Server");
                    await sw.FlushAsync();
                    var buffer = new[] { ' ' };
                    IDall = await sr.ReadLineAsync();
                    // TODO:落盘 每日一个日志文件 甚至可以直接把日志文件用lzma算法压缩
                    // 又打印又落盘 造一个Logger类 提供方法 log(string);
                    var time = DateTime.Now;
                    Console.WriteLine($"Welcome User {IDall} Connect Server \n Time :{time} ");
                    var log = $"User {IDall} Connect Server \n Time :{time} \n";
                    await File.AppendAllTextAsync("log.txt",log,Encoding.UTF8);
                    // Console.WriteLine($"Welcome User {ID} Connect Server \n Time :{DateTime.Now.ToString(CultureInfo.InvariantCulture)} ");
                    while (true)
                    {
                        var successReadByteCount = await sr.ReadAsync(buffer.AsMemory());
                        if (successReadByteCount == 1)
                        {
                           
                            //Console.WriteLine(buffer[0]);
                            await sw.WriteAsync(buffer);
                            await sw.FlushAsync();
                        }
                        else
                        {
                            // TODO:加一句日志 某个连接中断
                            break;
                        }
                        await Task.Delay(1000);
                    }
                }
                catch (Exception e)
                {
                    var time = DateTime.Now;
                    Console.WriteLine($"connect close detail:{e.Message} \n time : {time}");
                    var log = $"{IDall} connect close detail:{e.Message} \n time : {time}\n";
                    await File.AppendAllTextAsync("log.txt", log, Encoding.UTF8);
                }
                client.Close();
                client.Dispose();
                Console.WriteLine("connect released");
            }

            public async Task Run()
            {
                _listener.Start();
                Console.WriteLine("Ready to create client");
                while (true)
                {
                    var client = await _listener.AcceptTcpClientAsync();
                    // TODO:merge into HandleConnection
                    var remoteEndPoint = client.Client.RemoteEndPoint;
                    var s = (remoteEndPoint?.ToString()) ?? "error"; //TODO
                    _ = HandleConnection(client);
                }
            }
        }

        class MainApp
        {

            private static async Task Main(string[] args)
            {
                var server = new TcpServer();
                var startup = new Startup();
                startup.SetMeAutoStart(true);
                await server.Run();

            }
        }

    }
}

