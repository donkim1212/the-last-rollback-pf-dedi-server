using PathfindingDedicatedServer.handlers.abstracts;
using PathfindingDedicatedServer.Nav.Crowds;
using PathfindingDedicatedServer.Src.Sessions;
using System.Net.Sockets;

namespace PathfindingDedicatedServer.Src.Handlers
{
  internal class KillMonsterHandler : PacketHandler
  {
    public override void HandlePacket(NetworkStream stream, Guid id, byte[] bytes)
    {
      C_KillMonster packet = Deserialize<C_KillMonster>(bytes);
      Console.WriteLine($"[{packet.MonsterIdx}] 몬스터 처치");

      NavManager nav = Session.GetSession(id).GetNavManager();
      nav.RemoveMonster(packet.MonsterIdx);
    }
  }
}
