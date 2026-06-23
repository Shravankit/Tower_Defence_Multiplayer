using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

public class ServerDiscovery : MonoBehaviour
{
    int discoveryPort = 7777;

    public async Task<string> FindServer()
    {
        UdpClient client = new UdpClient();

        client.EnableBroadcast = true;

        byte[] bytes = Encoding.UTF8.GetBytes("DISCOVER_SERVER");

        await client.SendAsync(
    bytes,
       bytes.Length,
    new IPEndPoint(IPAddress.Broadcast, discoveryPort)
        );

        Debug.Log("Searching for LAN Server...");

        var result = await client.ReceiveAsync();

        string serverIpAddress = result.RemoteEndPoint.Address.ToString();

        Debug.Log(serverIpAddress);

        client.Close();

        return serverIpAddress;
    }
}