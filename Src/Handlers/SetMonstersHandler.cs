using System.Net.Sockets;
using PathfindingDedicatedServer.handlers.abstracts;
using PathfindingDedicatedServer.Src.Sessions;

namespace PathfindingDedicatedServer.Src.Handlers
{
  internal class SetMonstersHandler : PacketHandler
  {
    public override void HandlePacket(NetworkStream stream, Guid id, byte[] bytes)
    {
      C_SetMonsters packet = Deserialize<C_SetMonsters>(bytes);
      // TODO: call NavManager instance's SetMonsters()

      foreach (uint monsterIdx in packet.Monsters.Keys)
      {
        Console.WriteLine($"monsterIdx {} : {packet.Monsters[monsterIdx]}");
      }

      Session.GetSession(id).GetNavManager().SetMonsters(packet.Monsters);
    }
  }
}
