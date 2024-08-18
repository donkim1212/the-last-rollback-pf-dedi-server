using DotRecast.Core.Numerics;
using DotRecast.Detour;
using DotRecast.Detour.Crowd;

namespace PathfindingDedicatedServer.Src.Nav.Crowds
{
  public enum NavManagerState
  {
    NONE, WAITING, RUNNING, ENDING
  }

  public class NavManager
  {
    private readonly DtCrowd _crowd;
    private readonly int _dungeonCode;
    private NavManagerState _state = NavManagerState.NONE;
    private readonly int _tickRate = 500; // ms

    private readonly Dictionary<int, int> _monsters = [];
    private readonly Dictionary<string, int> _players = [];
    private readonly Dictionary<int, int> _structures = []; // won't move
    private DateTime _startTime;

    public NavManager(int dungeonCode) : this(dungeonCode, new DtCrowdConfig(0.6f), NavMeshManager.GetNavMesh(dungeonCode))
    {
    }

    public NavManager(int dungeonCode, DtCrowdConfig config, DtNavMesh? navMesh)
    {
      ArgumentNullException.ThrowIfNull(navMesh);
      _dungeonCode = dungeonCode;
      _crowd = new(config, navMesh);
      _state = NavManagerState.WAITING;
    }

    public void Start()
    {
      if (_state != NavManagerState.WAITING)
      {
        Console.WriteLine("CrowdManager is already running.");
        return;
      }

      _state = NavManagerState.RUNNING;
      _ = Task.Run(() => GameLoop());
    }

    public void End()
    {
      // TODO: cleanup?
      _state = NavManagerState.ENDING;
    }

    public void Reset()
    {
      _state = NavManagerState.WAITING;
    }

    private async Task GameLoop()
    {
      _startTime = DateTime.Now;
      DateTime prevTime = _startTime;
      int count = 0;
      while (_state == NavManagerState.RUNNING)
      {
        DateTime curTime = DateTime.Now;
        
        float deltaTime = (float) (curTime - prevTime).TotalSeconds;
        Update(deltaTime);
        Console.WriteLine($"[{count++}], deltaTime: {deltaTime}s");

        double elapsed = (DateTime.Now - curTime).TotalMilliseconds;
        if (elapsed < _tickRate)
        {
          await Task.Delay(_tickRate - (int) elapsed);
        }
        prevTime = curTime;

        // TODOs

        // 1.
        // increase aggro weight per tick?
        // choose target with most aggro point
        // base gets fixed weight value
      }

      Console.WriteLine("Game loop ended.");
    }

    private void Update(float deltaTime)
    {
      _crowd.Update(deltaTime, null);
    }

    /// <summary>
    /// Get DtCrowd's current status
    /// </summary>
    public void GetCurrentStatus()
    {
      // temp
      Object data = new();

      // wrap up agents data
      foreach (int monsterIdx in _monsters.Keys)
      {
        // TODO: monster position, aggro, attack target?
        DtCrowdAgent agent = GetMonsterAgent(monsterIdx);
        //agent.npos
      }
    }

    public RcVec3f? GetMonsterPos (int monsterIdx)
    {
      try
      {
        DtCrowdAgent agent = GetMonsterAgent(monsterIdx);
        return agent.npos;
      }
      catch (Exception e)
      {
        Console.WriteLine($"GetMonsterPos Error: {e.Message}");
        return null;
      }
    }

    public void SetMonsters()
    {

    }

    public void SetPlayers()
    {

    }

    /// <summary>
    /// Add a new monster to the given position, with the given option.
    /// </summary>
    /// <param name="monsterIdx">monster's index to be added to _monsters list</param>
    /// <param name="pos">vector position on navMesh the mosnter is to be spwawned at</param>
    /// <param name="option">crowd agent's option parameters</param>
    public void AddMonster(int monsterIdx, RcVec3f pos, DtCrowdAgentParams option)
    {
      // Add new agent
      DtCrowdAgent agent = _crowd.AddAgent(pos, option);
      _monsters.Add(monsterIdx, agent.idx);
      Console.WriteLine($"monster[ {monsterIdx} ] spawned at Vector3({pos.X},{pos.Y},{pos.Z})");

      // Query NavMesh
      DtNavMeshQuery navQuery = _crowd.GetNavMeshQuery();
      RcVec3f center = new(-5.04f, 0.55f, 135.68f);
      RcVec3f halfExtents = new(3f, 1.5f, 3f);
      
      navQuery.FindNearestPoly(
        center,
        halfExtents,
        new DtQueryDefaultFilter(),
        out long nearestRef,
        out RcVec3f nearestPt,
        out bool isOverPoly
      );

      // Set the agent's initial target
      _crowd.RequestMoveTarget(agent, nearestRef, nearestPt);
      Console.WriteLine($"monsterIdx[ {monsterIdx} ] state: " + agent.state);
    }

    public void AddMonster(int monsterIdx, DtCrowdAgentParams option)
    {
      // TODO: create a external function to get a random spawn position on a map
      RcVec3f spawnPos = SpawnerManager.GetRandomSpawnerPosition(_dungeonCode, 1234);
      AddMonster(monsterIdx, spawnPos, option);
    }

    public void AddMonster(int monsterIdx)
    {
      // TODO: fetch monster's agent param stats from JSON

      DtCrowdAgentParams option = new() // temp stats
      {
        radius = 0.6f,
        height = 1.5f,
        maxAcceleration = 1000f,
        maxSpeed = 2f,
        collisionQueryRange = 0.6f,
        pathOptimizationRange = 10f, // temp
        separationWeight = 0,
        updateFlags = DtCrowdAgentUpdateFlags.DT_CROWD_ANTICIPATE_TURNS,
        obstacleAvoidanceType = 0,
        queryFilterType = 0,
        userData = new(),
      };
      AddMonster(monsterIdx, option);
    }

    public void AddPlayer(string accountId, RcVec3f pos, DtCrowdAgentParams option)
    {
      // TODO:
      //_players.Add();
    }

    public void AddStructure(int structureIdx, RcVec3f pos, DtCrowdAgentParams option)
    {
      // TODO:
    }

    public DtCrowdAgent GetMonsterAgent (int monsterIdx)
    {
      return _crowd.GetAgent(_monsters[monsterIdx]);
    }

    public void RemoveMonster(int monsterIdx)
    {
      _crowd.RemoveAgent(GetMonsterAgent(monsterIdx));
      _monsters.Remove(monsterIdx);
    }

    public void ClearMonsters()
    {
      foreach(int monsterIdx in _monsters.Keys)
      {
        RemoveMonster(monsterIdx);
      }
    }

    public NavManagerState GetState()
    {
      return _state;
    }

    public void SetMonsterDest(string accountId)
    {

    }

    public void SetMonsterDest(int structureIdx)
    {

    }

    public void SetPlayerDest(RcVec3f pos)
    {
      
    }

    public void Halt(DtCrowdAgent agent)
    {
      //_crowd
      _crowd.ResetMoveTarget(agent);
    }
  }
}
