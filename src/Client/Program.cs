using System.Net;
using System.Net.Sockets;
using System.Text;

var client = new UdpClient();
client.Connect("127.0.0.1", 9050);
client.Client.ReceiveTimeout = 2000;

byte[] message = Encoding.UTF8.GetBytes("Hello, server!");
client.Send(message, message.Length);

try
{
    var from = new IPEndPoint(IPAddress.Any, 0);
    byte[] reply = client.Receive(ref from);
    Console.WriteLine($"Ответ: {Encoding.UTF8.GetString(reply)}");
}
catch (SocketException)
{
    Console.WriteLine("Ответ не пришёл (таймаут)");
}