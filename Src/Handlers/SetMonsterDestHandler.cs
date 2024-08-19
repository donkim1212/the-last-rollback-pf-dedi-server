using PathfindingDedicatedServer.handlers.abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace PathfindingDedicatedServer.Src.Handlers
{
  internal class SetMonsterDestHandler : PacketHandler
  {
    public override void HandlePacket(NetworkStream stream, Guid id, byte[] bytes)
    {
      
    }
  }
}
