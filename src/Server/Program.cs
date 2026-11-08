using System.Net;
using System.Net.Sockets;
using System.Text;

var server = new UdpClient(9050);

var sender = new IPEndPoint(IPAddress.Any, 0);

Console.WriteLine("Echo-сервер запущен на порту 9050");

while (true)
{
    byte[] data = server.Receive(ref sender);
    Console.WriteLine($"[{sender}] {Encoding.UTF8.GetString(data)}");
    
    server.Send(data, data.Length, sender);
}