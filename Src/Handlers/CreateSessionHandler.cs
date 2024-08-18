using PathfindingDedicatedServer.handlers.abstracts;
using PathfindingDedicatedServer.Src.Sessions;
using System.Net.Sockets;

namespace PathfindingDedicatedServer.Src.Handlers
{
  internal class CreateSessionHandler : PacketHandler
  {
    public override void HandlePacket(NetworkStream stream, Guid id, byte[] bytes)
    {
      C_CreateSession packet = Deserialize<C_CreateSession>(bytes);
      // TODO: create a session
      
      Session.AddSession(packet.DungeonCode);
    }
  }
}
