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

      Dictionary<string, uint> players = [];
      foreach (var player in packet.Players)
      {
        if (players.TryAdd(player.AccountId, player.CharClass))
          Console.WriteLine($"accountId {player.AccountId} : charClass {player.CharClass}");
      }

      // TODO: Set players list to the NavManager in the session
      Session.GetSession(id).GetNavManager().SetPlayers(players);
    }
  }
}
