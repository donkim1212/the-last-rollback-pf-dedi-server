using PathfindingDedicatedServer.handlers;
using PathfindingDedicatedServer.handlers.abstracts;
using System.Net.Sockets;

public class PacketManager
{
  static readonly PacketManager _Instance = new PacketManager();
  public static PacketManager Instance {  get { return _Instance; } }
    
  private Dictionary<int, PacketHandler> _handlers = [];

  public PacketManager()
  {
    Register();
  }

  private void Register()
  {
    _handlers.Add((int)Packet.PacketType.TestRequest, new TestReqHandler()); // ¿¹½Ã
  }

  public Action<NetworkStream, byte[]> GetPacketHandler(int id)
  {
    // PacketHandler handler;
    if (_handlers.TryGetValue(id, out var handler))
    {
      return handler.HandlePacket;
    }
    Console.WriteLine($"Can't find handler with id {id}");
    return null;
  }
}