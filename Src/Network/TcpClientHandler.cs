using System.Buffers;
using System.Net;
using System.Net.Sockets;
using System.Reflection.PortableExecutable;
using static Packet;

namespace PathfindingDedicatedServer.Src.Network;
public class TcpClientHandler
{
  private static readonly Dictionary<Guid, TcpClientHandler> _connections = [];
  private readonly Guid _id;
  private readonly TcpClient _tcpClient;
  private readonly NetworkStream _stream;
  private readonly List<byte> incompleteData = [];

  public event Action<string>? OnDataReceived;

  public TcpClientHandler(TcpClient tcpClient)
  {
    _id = Guid.NewGuid();
    _tcpClient = tcpClient;
    _stream = _tcpClient.GetStream();
    AddTcpClientHandler(_id, this);
  }

  public static bool AddTcpClientHandler(Guid id, TcpClientHandler handler)
  {
    return _connections.TryAdd(id, handler);
  }

  public static TcpClientHandler? GetTcpClientHandler(Guid id)
  {
    if (_connections.TryGetValue(id, out var client)) {
      return client;
    }
    return null;
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
      Console.WriteLine($"{e.StackTrace}");
    }
    finally
    {
      IPEndPoint? endPoint = _tcpClient.GetStream().Socket.RemoteEndPoint as IPEndPoint;
      Console.WriteLine($"[{endPoint?.Address}:{endPoint?.Port}] TcpClient closed.");
      _tcpClient.Close();
      _connections.Remove(_id);
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

  public async Task SendPacket<T>(PacketType packetType, T data)
  {
    var arrayBufferWriter = new ArrayBufferWriter<byte>();
    Packet.Serialize(arrayBufferWriter, data);
    byte[] buffer = arrayBufferWriter.WrittenSpan.ToArray();

    byte[] header = CreatePacketHeader(buffer.Length, packetType);

    byte[] packet = new byte[header.Length + buffer.Length];
    Array.Copy(header, 0, packet, 0, header.Length);
    Array.Copy(buffer, 0, packet, header.Length, buffer.Length);

    //Console.WriteLine("??? " + header.Length + "  " + packet.Length);
    //Console.WriteLine("??? " + header[4]);

    _tcpClient.GetStream().Write(packet, 0, packet.Length);
  }

  private static byte[] CreatePacketHeader(int dataLength, PacketType packetType)
  {
    int packetLength = 4 + 1 + dataLength;
    byte[] header = new byte[5];

    byte[] lengthBytes = BitConverter.GetBytes(packetLength);
    lengthBytes = ToBigEndian(lengthBytes);
    Array.Copy(lengthBytes, 0, header, 0, 4);

    header[4] = (byte)packetType;

    return header;
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