using System.Net;
using System.Net.Sockets;

namespace PathfindingDedicatedServer.Src.Network;
public class TcpClientHandler
{
  private readonly Guid _id;
  private readonly TcpClient _tcpClient;
  private readonly NetworkStream _stream;
  private List<byte> incompleteData = new List<byte>();

  public event Action<string>? OnDataReceived;

  public TcpClientHandler(TcpClient tcpClient)
  {
    _id = Guid.NewGuid();
    _tcpClient = tcpClient;
    _stream = _tcpClient.GetStream();
  }

  public async Task StartHandlingClientAsync()
  {
    byte[] buffer = new byte[2048];
    try
    {
      while (_tcpClient.Connected)
      {
        int bytesRead = await _stream.ReadAsync(buffer, 0, buffer.Length);
        if (bytesRead == 0)
        {
          Console.WriteLine("Client disconnected");
          break;
        }
        ProcessData(buffer, bytesRead);
      }
    }
    catch (Exception e)
    {
      Console.WriteLine($"HandleClient Error: {e.Message}");
    }
    finally
    {
      IPEndPoint? endPoint = _tcpClient.GetStream().Socket.RemoteEndPoint as IPEndPoint;
      Console.WriteLine($"[{endPoint?.Address}:{endPoint?.Port}] TcpClient closed.");
      _tcpClient.Close();
    }
  }

  private void ProcessData(byte[] data, int length)
  {
    incompleteData.AddRange(data.AsSpan(0, length).ToArray());
    while (incompleteData.Count >= 5)
    {
      byte[] lengthBytes = incompleteData.GetRange(0, 4).ToArray();
      int packetLength = BitConverter.ToInt32(ToBigEndian(lengthBytes), 0);
      Packet.PacketType packetType = (Packet.PacketType)incompleteData[4];

      if (incompleteData.Count < packetLength)
      {
        return;
      }

      byte[] packetData = incompleteData.GetRange(5, packetLength - 5).ToArray();
      incompleteData.RemoveRange(0, packetLength);

      Action<NetworkStream, Guid, byte[]> handler = PacketManager.Instance.GetPacketHandler((int)packetType);
      handler?.Invoke(_tcpClient.GetStream(), _id, packetData);
    }
    /*string request = Encoding.UTF8.GetString(data, 0, length);
    OnDataReceived?.Invoke(request);

    byte[] response = Encoding.UTF8.GetBytes("Hello from TCP server!");
    _stream.WriteAsync(response, 0, response.Length);*/
  }

  public static byte[] ToBigEndian(byte[] bytes)
  {
    if (BitConverter.IsLittleEndian)
    {
      Array.Reverse(bytes);
    }
    return bytes;
  }
}