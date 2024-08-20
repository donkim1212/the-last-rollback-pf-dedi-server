using DotRecast.Core;
using DotRecast.Core.Numerics;
using DotRecast.Detour;
using DotRecast.Detour.Crowd;
using PathfindingDedicatedServer.Src.Constants;
using PathfindingDedicatedServer.Src.Data;

namespace PathfindingDedicatedServer.Nav.Crowds
{
  public enum NavManagerState
  {
    NONE, WAITING, RUNNING, ENDING
  }

  internal class DtNavMeshQueryResult
  {
    public long NearestRef { get; set; }
    public RcVec3f NearestPt { get; set; }
    public bool IsOverPoly { get; set; }
  }

  public class NavManager
  {
    private readonly DtCrowd _crowd;
    private readonly uint _dungeonCode;
    private NavManagerState _state = NavManagerState.NONE;
    private readonly int _tickRate = 500; // ms

    private readonly Dictionary<uint, int> _monsterAgents = []; // monsterIdx, agentIdx
    private readonly Dictionary<uint, DtCrowdAgentParams> _monsterOptions = [];
    private readonly Dictionary<string, int> _playerAgents = []; // accountId, agentIdx
    private readonly Dictionary<string, DtCrowdAgentParams> _playerOptions = [];
    private readonly Dictionary<uint, int> _structureAgents = []; // structureIdx, agentIdx / won't move
    private readonly Dictionary<uint, DtCrowdAgentParams> _structureOptions = [];
    private DateTime _startTime;

    public NavManager(uint dungeonCode) : this(dungeonCode, new DtCrowdConfig(0.6f), NavMeshManager.GetNavMesh(dungeonCode))
    {
    }

    public NavManager(uint dungeonCode, DtCrowdConfig config, DtNavMesh? navMesh)
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

        float deltaTime = (float)(curTime - prevTime).TotalSeconds;
        Update(deltaTime);
        Console.WriteLine($"[{count++}], deltaTime: {deltaTime}s");

        double elapsed = (DateTime.Now - curTime).TotalMilliseconds;
        if (elapsed < _tickRate)
        {
          await Task.Delay(_tickRate - (int)elapsed);
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
      foreach (uint monsterIdx in _monsterAgents.Keys)
      {
        // TODO: monster position, aggro, attack target?
        DtCrowdAgent agent = GetMonsterAgent(monsterIdx);
        //agent.npos
      }
    }

    public RcVec3f GetPosition(DtCrowdAgent agent)
    {
      return agent.npos;
    }

    public RcVec3f? GetMonsterPos(uint monsterIdx)
    {
      try
      {
        DtCrowdAgent agent = GetMonsterAgent(monsterIdx);
        return GetPosition(agent);
      }
      catch (Exception e)
      {
        Console.WriteLine($"GetMonsterPos Error: {e.Message}");
        return null;
      }
    }

    public RcVec3f? GetPlayerPos(string accountId)
    {
      try
      {
        DtCrowdAgent agent = GetPlayerAgent(accountId);
        return GetPosition(agent);
      }
      catch (Exception e)
      {
        Console.WriteLine($"GetPlayerPos Error: {e.Message}");
        return null;
      }
    }

    private DtNavMeshQueryResult GetNavMeshQueryResult(RcVec3f pos, float hRad, float vRad)
    {
      return GetNavMeshQueryResult(pos, hRad, vRad, _crowd.GetFilter(0));
    }

    private DtNavMeshQueryResult GetNavMeshQueryResult(RcVec3f pos, float hRad, float vRad, IDtQueryFilter filter)
    {
      DtNavMeshQuery navQuery = _crowd.GetNavMeshQuery();

      RcVec3f halfExtents = new (hRad, vRad, hRad);
      navQuery.FindNearestPoly(
        pos,
        halfExtents,
        filter,
        out long nearestRef,
        out RcVec3f nearestPt,
        out bool isOverPoly
      );

      return new()
      {
        NearestRef = nearestRef,
        NearestPt = nearestPt,
        IsOverPoly = isOverPoly
      };
    }

    /// <summary>
    /// Add a new monster to the given position, with the given option.
    /// </summary>
    /// <param name="monsterIdx">monster's index to be added to _monsters list</param>
    /// <param name="pos">vector position on navMesh the mosnter is to be spwawned at</param>
    /// <param name="option">crowd agent's option parameters</param>
    public void AddMonster(uint monsterIdx, RcVec3f pos, DtCrowdAgentParams option)
    {
      // Add new agent
      DtCrowdAgent agent = _crowd.AddAgent(pos, option);
      _monsterAgents.Add(monsterIdx, agent.idx);
      Console.WriteLine($"monster[ {monsterIdx} ] spawned at Vector3({pos.X},{pos.Y},{pos.Z})");

      // Query NavMesh
      RcVec3f center = new(-5.04f, 0.55f, 135.68f);
      RcVec3f halfExtents = new(3f, 1.5f, 3f);
      DtNavMeshQueryResult result = GetNavMeshQueryResult(center, 3f, 1.5f);

      // Set the agent's initial target
      _crowd.RequestMoveTarget(agent, result.NearestRef, result.NearestPt);
      Console.WriteLine($"monsterIdx[ {monsterIdx} ] state: " + agent.state);
    }

    public void AddMonster(uint monsterIdx, DtCrowdAgentParams option)
    {
      // TODO: create a external function to get a random spawn position on a map
      RcVec3f spawnPos = Storage.GetRandomPos(_dungeonCode, PosType.MONSTER_SPAWNER);
      AddMonster(monsterIdx, spawnPos, option);
    }

    public void AddMonster(uint monsterIdx, uint monsterModel)
    {
      DtCrowdAgentParams option = Storage.GetMonsterAgentInfo(monsterModel);
      AddMonster(monsterIdx, option);
    }

    public void AddMonster(uint monsterIdx)
    {
      DtCrowdAgentParams option = Storage.GetDefaultAgentInfo();
      AddMonster(monsterIdx, option);
    }

    public DtCrowdAgent GetMonsterAgent (uint monsterIdx)
    {
      return _crowd.GetAgent(_monsterAgents[monsterIdx]);
    }

    public DtCrowdAgent GetPlayerAgent (string accountId)
    {
      return _crowd.GetAgent(_playerAgents[accountId]);
    }

    public DtCrowdAgent GetStructureAgent(uint structureIdx)
    {
      return _crowd.GetAgent(_structureAgents[structureIdx]);
    }

    public void RemoveMonster(uint monsterIdx)
    {
      _crowd.RemoveAgent(GetMonsterAgent(monsterIdx));
    }

    public void ClearMonsters()
    {
      foreach(uint monsterIdx in _monsterAgents.Keys)
      {
        RemoveMonster(monsterIdx);
      }
      _monsterAgents.Clear();
      //_monsterOptions.Clear();
    }

    public NavManagerState GetState()
    {
      return _state;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="monsters">key: monsterIdx, value: monsterModel</param>
    public void SetMonsters(Dictionary<uint, uint> monsters)
    {
      _monsterOptions.Clear();
      foreach (uint monsterIdx in monsters.Keys)
      {
        monsters.TryGetValue(monsterIdx, out uint monsterModel);
        _monsterOptions.Add(monsterIdx, Storage.GetMonsterAgentInfo(monsterModel));
      }
    }

    public void SetPlayers(Dictionary<string, uint> players)
    {
      foreach (string accountId in players.Keys)
      {
        players.TryGetValue(accountId, out uint charClass);
        _playerOptions.Add(accountId, Storage.GetPlayerAgentInfo(charClass));
        
        // Add the player to the DtCrowd manager
        RcVec3f center = Storage.GetRandomPos(_dungeonCode, PosType.PLAYER_SPAWNER);
        DtNavMeshQueryResult result = GetNavMeshQueryResult(center, 3f, 1.5f);
        // TODO: handle DtStatus
        _crowd.GetNavMeshQuery().FindRandomPointWithinCircle(
          result.NearestRef,
          center,
          5f,
          _crowd.GetFilter(0),
          new RcRand(),
          out long randomRef,
          out RcVec3f randomPt
        );
        AddPlayer(accountId, randomPt, Storage.GetPlayerAgentInfo(charClass));
      }
    }

    public void AddPlayer(string accountId, RcVec3f pos, DtCrowdAgentParams option)
    {
      _playerAgents.Add(accountId, _crowd.AddAgent(pos, option).idx);
    }

    public void AddStructure(uint structureIdx, RcVec3f pos, DtCrowdAgentParams option)
    {
      _structureAgents.Add(structureIdx, _crowd.AddAgent(pos, option).idx);
    }

    public void SetMonsterDest(uint monsterIdx, TargetStructure target)
    {
      //RcVec3f? targetPos = GetStructurePos(target.StructureIdx);
    }

    public void SetMonsterDest(uint monsterIdx, TargetPlayer target)
    {
      RcVec3f? targetPos = GetPlayerPos(target.AccountId);
      if (targetPos == null)
      {
        // TODO: set target to Base
        
        //Halt(monsterIdx);
        return;
      }

      DtNavMeshQueryResult result = GetNavMeshQueryResult(targetPos.Value, 3f, 1.5f);
      _crowd.RequestMoveTarget(GetMonsterAgent(monsterIdx), result.NearestRef, targetPos.Value);
    }

    public void SetPlayerDest(string accountId, RcVec3f pos)
    {
      var result = GetNavMeshQueryResult(pos, 3f, 1.5f, new DtQueryDefaultFilter());
      _crowd.RequestMoveTarget(GetPlayerAgent(accountId), result.NearestRef, pos);
    }

    public void MoveToBase (DtCrowdAgent agent)
    {

      //DtNavMeshQuery query = _crowd.GetNavMeshQuery();
      // TODO: Get Base's poly ref
      //query.FindNearestPoly(
      //  Storage.GetPos(_dungeonCode, PosType.BASE_SPAWNER, 0),
      //  new RcVec3f(3f, 1.5f, 3f),
      //  _crowd.GetFilter(0),
      //  out long nearestRef,
      //  out RcVec3f nearestPt,
      //  out bool isOverPoly
      //);

      //List<long> resultRef = [];
      //List<long> resultParent = [];
      //List<float> resultCost = [];
      // TODO: Get nearest circular border point of the Base
      //query.FindPolysAroundCircle(
      //  agent.corridor.GetFirstPoly(),
      //  nearestPt,
      //  3f,
      //  _crowd.GetFilter(0),
      //  ref resultRef,
      //  ref resultParent,
      //  ref resultCost
      //);

      //_crowd.RequestMoveTarget(agent, resultRef.First(), )
    }

    public void Halt(DtCrowdAgent agent)
    {
      _crowd.ResetMoveTarget(agent);
    }

    public void Halt(string accountId)
    {
      Halt(GetPlayerAgent(accountId));
    }

    public void Halt(uint monsterIdx)
    {
      Halt(GetMonsterAgent(monsterIdx));
    }
  }
}
