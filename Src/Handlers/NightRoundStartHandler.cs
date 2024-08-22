using PathfindingDedicatedServer.handlers.abstracts;
using PathfindingDedicatedServer.Src.Sessions;
using System.Net.Sockets;

namespace PathfindingDedicatedServer.Src.Handlers
{
  public class NightRoundStartHandler : PacketHandler
  {
    public override void HandlePacket(NetworkStream stream, Guid id, byte[] bytes)
    {
      //Console.WriteLine("SetMonsterDestHandler called.");
      C_NightRoundStart packet = Deserialize<C_NightRoundStart>(bytes);

      //packet.Timestamp;
      Session.GetSession(id).StartSpawning(packet.Timestamp);
    }
  }
}
