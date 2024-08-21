using DotRecast.Core;
using DotRecast.Core.Numerics;
using DotRecast.Detour;
using DotRecast.Detour.Crowd;
using PathfindingDedicatedServer.Src.Constants;
using PathfindingDedicatedServer.Src.Data;
using PathfindingDedicatedServer.Src.Utils;
using System.Reflection;

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
    private readonly int _tickRate = 20; // ms

    private readonly S_PlayersLocationUpdate _plu = new() { Positions = [] };
    private readonly S_MonstersLocationUpdate _mlu = new() { Positions = [] };

    private readonly Dictionary<uint, int> _monsterAgents = []; // monsterIdx, agentIdx
    private readonly Dictionary<uint, DtCrowdAgentParams> _monsterOptions = [];
    private readonly Dictionary<string, int> _playerAgents = []; // accountId, agentIdx
    private readonly Dictionary<string, DtCrowdAgentParams> _playerOptions = [];
    private readonly Dictionary<uint, int> _structureAgents = []; // structureIdx, agentIdx / won't move
    private readonly Dictionary<uint, DtCrowdAgentParams> _structureOptions = [];
    private DateTime _startTime;
    private DateTime _prevTime;
    private DateTime _curTime;

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
        Console.WriteLine($"NavManager is {_state}.");
        return;
      }

      _state = NavManagerState.RUNNING;
      _startTime = DateTime.UtcNow;
      _prevTime = _startTime;

      //_ = Task.Run(() => GameLoop());
    }

    public void End()
    {
      _state = NavManagerState.ENDING;

      // TODO: cleanup?
      _monsterAgents.Clear();
      _monsterOptions.Clear();
      _playerAgents.Clear();
      _structureAgents.Clear();
      _playerOptions.Clear();
      _structureOptions.Clear();
    }

    public void Reset()
    {
      _state = NavManagerState.WAITING;
    }

    //private async Task GameLoop()
    //{
    //  _startTime = DateTime.UtcNow;
    //  _prevTime = _startTime;
    //  int count = 0;
    //  while (_state == NavManagerState.RUNNING)
    //  {
    //    var deltaTime = UpdateImmediately();
    //    Console.WriteLine($"[{count++}], deltaTime: {deltaTime}s");

    //    double elapsed = (DateTime.UtcNow - _curTime).TotalMilliseconds;
    //    if (elapsed < _tickRate)
    //    {
    //      await Task.Delay(_tickRate - (int)elapsed);
    //    }
        
    //  }

    //  Console.WriteLine("Game loop ended.");
    //}

    private void Update(float deltaTime)
    {
      _crowd.Update(deltaTime, null);
    }

    public float UpdateImmediately()
    {
      _curTime = DateTime.UtcNow;
      float deltaTime = (float)(_curTime - _prevTime).TotalSeconds;
      Update(deltaTime);
      _prevTime = _curTime;
      return deltaTime;
    }

    public double GetCurrentElapsedTime()
    {
      return (DateTime.UtcNow - _curTime).TotalMilliseconds;
    }

    public int GetTickRate()
    {
      return _tickRate;
    }

    public double GetMilliSecondsDelay()
    {
      double delay = (double)_tickRate - GetCurrentElapsedTime();
      return delay <= 0 ? 0 : delay;
    }

    public S_MonstersLocationUpdate GetMonsterLocations()
    {
      _mlu.Positions.Clear();
      foreach (uint monsterIdx in _monsterAgents.Keys)
      {
        RcVec3f? pos = GetMonsterPos(monsterIdx);
        if (pos == null) continue; // monster might be dead & removed
        _mlu.Positions[monsterIdx] = Utils.ToWorldPosition(pos.Value);
      }
      return _mlu;
    }

    public S_PlayersLocationUpdate GetPlayerLocations()
    {
      _plu.Positions.Clear();
      foreach (string accountId in _playerAgents.Keys)
      {
        RcVec3f? pos = GetPlayerPos(accountId);
        if (pos == null) continue;
        _plu.Positions[accountId] = Utils.ToWorldPosition(pos.Value);
      }
      return _plu;
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

    public RcVec3f? GetStructurePos(uint structureIdx)
    {
      try
      {
        DtCrowdAgent agent = GetStructureAgent(structureIdx);
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
      
      //UpdateImmediately();
      
      // Query NavMesh
      //RcVec3f center = new(-5.04f, 0.55f, 135.68f);
      //RcVec3f halfExtents = new(3f, 1.5f, 3f);
      //DtNavMeshQueryResult result = GetNavMeshQueryResult(center, 3f, 1.5f);

      // Set the agent's initial target
      //_crowd.RequestMoveTarget(agent, result.NearestRef, result.NearestPt);
      MoveToBase(agent);
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
      _monsterAgents.Remove(monsterIdx); // hmm
    }

    public void ClearMonsters()
    {
      foreach(uint monsterIdx in _monsterAgents.Keys)
      {
        _crowd.RemoveAgent(GetMonsterAgent(monsterIdx));
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
      MoveTo(GetMonsterAgent(monsterIdx), GetStructurePos(target.StructureIdx));
    }

    public void SetMonsterDest(uint monsterIdx, TargetPlayer target)
    {
      MoveTo(GetMonsterAgent(monsterIdx), GetPlayerPos(target.AccountId));
    }

    public void SetPlayerDest(string accountId, RcVec3f? pos)
    {
      MoveTo(GetPlayerAgent(accountId), pos);
    }

    public void MoveTo (DtCrowdAgent agent, RcVec3f? pos)
    {
      if (pos == null)
      {
        Halt(agent);
        return;
      }
      var result = GetNavMeshQueryResult(pos.Value, 3f, 1.5f);
      _crowd.RequestMoveTarget(agent, result.NearestRef, result.NearestPt);
    }

    public void MoveToBase (DtCrowdAgent agent)
    {
      RcVec3f basePos = Storage.GetPos(_dungeonCode, PosType.BASE_SPAWNER, 0);
      MoveTo(agent, basePos);
      // TODO: Get nearest circular border position

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
