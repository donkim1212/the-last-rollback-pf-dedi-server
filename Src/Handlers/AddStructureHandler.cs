using PathfindingDedicatedServer.handlers.abstracts;
using PathfindingDedicatedServer.Src.Data;
using PathfindingDedicatedServer.Src.Sessions;
using System.Net.Sockets;

namespace PathfindingDedicatedServer.Src.Handlers
{
  public class AddStructureHandler : PacketHandler
  {
    public override void HandlePacket(NetworkStream stream, Guid id, byte[] bytes)
    {
      C_AddStructure packet = Deserialize<C_AddStructure>(bytes);

      var option = Storage.GetStructureAgentInfo(packet.Structure.StructureModel);
      var worldPosition = Utils.Utils.ToRcVector(packet.WorldPosition);

      Session.GetSession(id).GetNavManager().AddStructure(
        packet.Structure.StructureIdx,
        worldPosition,
        option
      );
    }
  }
}
