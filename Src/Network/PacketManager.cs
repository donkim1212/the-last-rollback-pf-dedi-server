using PathfindingDedicatedServer.handlers.abstracts;
using PathfindingDedicatedServer.Src.Handlers;
using System.Net.Sockets;

public class PacketManager
{
  static readonly PacketManager _Instance = new();
  public static PacketManager Instance { get { return _Instance; } }

  private Dictionary<int, PacketHandler> _handlers = [];

  public PacketManager()
  {
    Register();
  }

  private void Register()
  {
    _handlers.Add((int)Packet.PacketType.C_CreateSession, new CreateSessionHandler());
    _handlers.Add((int)Packet.PacketType.C_SetPlayers, new SetPlayersHandler());
    _handlers.Add((int)Packet.PacketType.C_SetMonsters, new SetMonstersHandler());
    _handlers.Add((int)Packet.PacketType.C_SetPlayerDest, new SetPlayerDestHandler());
    _handlers.Add((int)Packet.PacketType.C_SetMonsterDest, new SetMonsterDestHandler());
    _handlers.Add((int)Packet.PacketType.C_NightRoundStart, new NightRoundStartHandler());
    _handlers.Add((int)Packet.PacketType.C_KillMonster, new KillMonsterHandler());
    _handlers.Add((int)Packet.PacketType.C_AddStructure, new AddStructureHandler());
    _handlers.Add((int)Packet.PacketType.C_RemoveStructure, new RemoveStructureHandler());
  }

  public Action<NetworkStream, Guid, byte[]> GetPacketHandler(int id)
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