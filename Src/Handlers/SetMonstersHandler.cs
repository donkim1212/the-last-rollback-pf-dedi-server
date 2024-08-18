using System.Net.Sockets;
using PathfindingDedicatedServer.handlers.abstracts;

namespace PathfindingDedicatedServer.Src.Handlers
{
  internal class SetMonstersHandler : PacketHandler
  {
    public override void HandlePacket(NetworkStream stream, Guid id, byte[] bytes)
    {
      C_SetMonsters packet = Deserialize<C_SetMonsters>(bytes);
      // TODO: call NavManager instance's SetMonsters()

    }
  }
}
