using System.Net.Security;
using System.Net.Sockets;
using System.Text;

namespace Cachara.Playground;

public class NetworkProgrammingPractice
{
    public async Task PracticeTcpClientHTTPSRequest()
    {
        using TcpClient client = new();
        await client.ConnectAsync("www.potato.com.br", 443);
        Console.WriteLine("Connected to server.");

        await using var networkStream = client.GetStream();

        // 🔹 Secure the connection with TLS
        using SslStream sslStream = new(networkStream, true);
        await sslStream.AuthenticateAsClientAsync("www.potato.com.br");

        Console.WriteLine("TLS Handshake completed.");

        //🔹 Send an HTTP request
        var request = "GET / HTTP/1.1\r\n" +
                      "Host: www.potato.com.br\r\n" +
                      "Connection: close\r\n\r\n";
        var requestBytes = Encoding.ASCII.GetBytes(request);
        await sslStream.WriteAsync(requestBytes);
        await sslStream.FlushAsync();

        // 🔹 Read the response
        using StreamReader reader = new(sslStream, Encoding.UTF8);
        var response = await reader.ReadToEndAsync();
        Console.WriteLine("Response received:");
        Console.WriteLine(response);
    }
}
