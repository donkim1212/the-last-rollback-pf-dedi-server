using System.Net.Sockets;
using PathfindingDedicatedServer.handlers.abstracts;

namespace PathfindingDedicatedServer.Src.Handlers
{
  internal class SetPlayersHandler : PacketHandler
  {
    public override void HandlePacket(NetworkStream stream, Guid id, byte[] bytes)
    {
      C_SetPlayers packet = Deserialize< C_SetPlayers>(bytes);
    }
  }
}
