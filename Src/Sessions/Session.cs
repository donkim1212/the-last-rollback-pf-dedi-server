using PathfindingDedicatedServer.Nav.Crowds;
using PathfindingDedicatedServer.Src.Network;
using static Packet;

namespace PathfindingDedicatedServer.Src.Sessions
{
  internal class Session
  {
    private static readonly Dictionary<Guid, Session> _sessions = [];
    
    private readonly NavManager _navManager;
    private readonly Func<Task> _gameLoop;
    private readonly Guid _id;

    public static bool AddSession(uint dungeonCode, Guid guid)
    {
      return _sessions.TryAdd(guid, new Session(dungeonCode, guid));
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
    
    public Session (uint dungeonCode, Guid id)
    {
      _navManager = new NavManager(dungeonCode);
      _gameLoop = StartGameLoop;
      _id = id;
      Start();
    }

    public void Start()
    {
      _ = Task.Run(_gameLoop);
    }

    public async Task StartGameLoop ()
    {
      try
      {
        TcpClientHandler? clientHandler = TcpClientHandler.GetTcpClientHandler(_id);
        if (clientHandler == null)
        {
          throw new InvalidOperationException("No TcpClientHandler found for the session.");
        }
        _navManager.Start();
        int prevMonsterCount = 0;
        int prevPlayerCount = 0;
        while (_navManager.GetState() == NavManagerState.RUNNING)
        {
          // Update DtCrowd
          var deltaTime = _navManager.Update();
          //Console.WriteLine($"deltaTime: {deltaTime}s");

          // Send position packets
          var monsterLocations = _navManager.GetMonsterLocations();
          _ = clientHandler.SendPacket<S_MonstersLocationUpdate>(
            PacketType.S_MonstersLocationUpdate,
            monsterLocations
          );
          if (prevMonsterCount != monsterLocations.Positions.Count)
          {
            prevMonsterCount = monsterLocations.Positions.Count;
            Console.WriteLine($"Monsters count: {prevMonsterCount}");
          }
          
          var playerLocations = _navManager.GetPlayerLocations();
          _ = clientHandler.SendPacket<S_PlayersLocationUpdate>(
            PacketType.S_PlayerLocationUpdate,
            playerLocations
          );

          if (prevPlayerCount != playerLocations.Positions.Count)
          {
            prevPlayerCount = playerLocations.Positions.Count;
            Console.WriteLine($"Players count: {prevMonsterCount}");
          }

          // Re-path
          _navManager.ReCalcAll();

          // Spawn if possible
          _navManager.TrySpawn();

          // wait for remaining frame time
          await Task.Delay((int)_navManager.GetMilliSecondsDelay());

          // TODOs

          // 1.
          // increase aggro weight per tick?
          // choose target with most aggro point
          // base gets fixed weight value

          // 2.
          // calc distance btw each monster and its target
          //  Vector2 distance - (monster r + target r) ??
          // if the calc dist <= atk range, then
          //  monster stops moving, emits atk packet
          //  - option 1: waits until atk end packet arrives and resume tracking
          //  - option 2: waits async for atk cooldown and retrack the prev target

          // 3.
        }
        Console.WriteLine("Game loop ended.");
      } catch (Exception e)
      {
        // TODO: print error msg & stack trace
        Console.WriteLine(e.ToString());
      } finally
      {
        // TODO: release resources
        _navManager.End();
        RemoveSession(_id); // both gets GC'd (hopefully)
      }
      
    }

    public void StartSpawning(ulong timestamp)
    {
      _navManager.StartSpawn(timestamp);
    }

    public NavManager GetNavManager()
    {
      return _navManager;
    }
  }
}
