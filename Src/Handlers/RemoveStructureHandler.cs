using PathfindingDedicatedServer.handlers.abstracts;
using PathfindingDedicatedServer.Src.Sessions;
using System.Net.Sockets;

namespace PathfindingDedicatedServer.Src.Handlers
{
  internal class RemoveStructureHandler : PacketHandler
  {
    public override void HandlePacket(NetworkStream stream, Guid id, byte[] bytes)
    {
      C_RemoveStructure packet = Deserialize<C_RemoveStructure>(bytes);

      Session.GetSession(id).GetNavManager().RemoveStructure(packet.StructureIdx);
    }
  }
}
