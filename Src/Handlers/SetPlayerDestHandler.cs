using PathfindingDedicatedServer.handlers.abstracts;
using System.Net.Sockets;

namespace PathfindingDedicatedServer.Src.Handlers
{
  internal class SetPlayerDestHandler : PacketHandler
  {
    public override void HandlePacket(NetworkStream stream, Guid id, byte[] bytes)
    {
      
    }
  }
}
