using System.Net.Sockets;
using PathfindingDedicatedServer.handlers.abstracts;
using PathfindingDedicatedServer.Src.Sessions;
using PathfindingDedicatedServer.Nav.Crowds;

namespace PathfindingDedicatedServer.Src.Handlers
{
  internal class SetPlayersHandler : PacketHandler
  {
    public override void HandlePacket(NetworkStream stream, Guid id, byte[] bytes)
    {
      C_SetPlayers packet = Deserialize<C_SetPlayers>(bytes);

      foreach (var player in packet.Players)
      {
        Console.WriteLine($"accountId {player.Key} : charClass {player.Value}");
      }

      // TODO: Set players list to the NavManager in the session
      Session.GetSession(id).GetNavManager().SetPlayers(packet.Players);
    }
  }
}
