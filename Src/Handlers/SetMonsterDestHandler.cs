using PathfindingDedicatedServer.handlers.abstracts;
using PathfindingDedicatedServer.Src.Sessions;
using System.Net.Sockets;

namespace PathfindingDedicatedServer.Src.Handlers
{
  internal class SetMonsterDestHandler : PacketHandler
  {
    public override void HandlePacket(NetworkStream stream, Guid id, byte[] bytes)
    {
      C_SetMonsterDest packet = Deserialize<C_SetMonsterDest>(bytes);
      if (packet.Target is TargetPlayer tp)
      {
        Session.GetSession(id).GetNavManager().SetMonsterDest(packet.MonsterIdx, tp);
      }
      else if (packet.Target is TargetStructure ts)
      {
        Session.GetSession(id).GetNavManager().SetMonsterDest(packet.MonsterIdx, ts);
      }
      else
      {
        Session.GetSession(id).GetNavManager().Halt(packet.MonsterIdx);
      }
    }
  }
}
