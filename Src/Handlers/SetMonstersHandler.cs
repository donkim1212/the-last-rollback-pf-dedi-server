using System.Net.Sockets;
using PathfindingDedicatedServer.handlers.abstracts;
using PathfindingDedicatedServer.Src.Sessions;

namespace PathfindingDedicatedServer.Src.Handlers
{
  internal class SetMonstersHandler : PacketHandler
  {
    public override void HandlePacket(NetworkStream stream, Guid id, byte[] bytes)
    {
      Console.WriteLine("SetMonstersHandler called.");
      C_SetMonsters packet = Deserialize<C_SetMonsters>(bytes);
      // TODO: call NavManager instance's SetMonsters()

      Dictionary<uint, uint> monsters = [];
      foreach (var monster in packet.Monsters)
      {
        if (monsters.TryAdd(monster.MonsterIdx, monster.MonsterModel))
          Console.WriteLine($"accountId {monster.MonsterIdx} : charClass {monster.MonsterModel}");
      }

      Session.GetSession(id).GetNavManager().SetMonsters(monsters);
    }
  }
}
