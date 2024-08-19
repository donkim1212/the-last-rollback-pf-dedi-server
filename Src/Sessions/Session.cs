using PathfindingDedicatedServer.Nav.Crowds;
using System.Net.Sockets;

namespace PathfindingDedicatedServer.Src.Sessions
{
  internal class Session
  {
    private static readonly Dictionary<Guid, Session> _sessions = [];
    
    private readonly NavManager _navManager;

    public static bool AddSession(int dungeonCode, Guid guid)
    {
      return _sessions.TryAdd(guid, new Session(dungeonCode));
    }

    public static Session GetSession(Guid guid)
    {
      return _sessions[guid];
    }

    public static bool RemoveSession(Guid guid)
    {
      bool exists = _sessions.TryGetValue(guid, out Session? session);
      if (session != null)
      {
        session.GetNavManager().End();
        _sessions.Remove(guid);
      }
      return exists;
    }
    
    public Session (int dungeonCode)
    {
      _navManager = new NavManager(dungeonCode);
    }

    public NavManager GetNavManager()
    {
      return _navManager;
    }

    //public void SendPacket(Packet.PacketType packetType)
    //{

    //}
  }
}
