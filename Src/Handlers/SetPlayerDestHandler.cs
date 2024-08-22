using DotRecast.Core.Numerics;
using PathfindingDedicatedServer.handlers.abstracts;
using PathfindingDedicatedServer.Src.Sessions;
using System.Net.Sockets;

namespace PathfindingDedicatedServer.Src.Handlers
{
  internal class SetPlayerDestHandler : PacketHandler
  {
    public override void HandlePacket(NetworkStream stream, Guid id, byte[] bytes)
    {
      //Console.WriteLine("SetPlayerDestHandler called.");
      C_SetPlayerDest packet = Deserialize<C_SetPlayerDest>(bytes);
      if (packet.Pos == null)
      {
        Session.GetSession(id).GetNavManager().Halt(packet.AccountId);
      }
      else
      {
        RcVec3f pos = new(packet.Pos.X, packet.Pos.Y, packet.Pos.Z);
        Session.GetSession(id).GetNavManager().SetPlayerDest(packet.AccountId, pos);
      }
      
    }
  }
}
