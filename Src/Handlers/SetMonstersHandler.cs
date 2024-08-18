using System.Net.Sockets;
using PathfindingDedicatedServer.handlers.abstracts;

namespace PathfindingDedicatedServer.Src.Handlers
{
  internal class SetMonstersHandler : PacketHandler
  {
    public override void HandlePacket(NetworkStream stream, byte[] bytes)
    {
      C_SetMonsters packet = Deserialize<C_SetMonsters>(bytes);
      // TODO: call NavManager instance's SetMonsters()

    }
  }
}
