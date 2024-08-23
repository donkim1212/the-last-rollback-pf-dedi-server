using DotRecast.Core;
using DotRecast.Core.Numerics;
using DotRecast.Detour;
using DotRecast.Detour.Crowd;
using PathfindingDedicatedServer.Src.Constants;
using PathfindingDedicatedServer.Src.Data;
using static PathfindingDedicatedServer.Src.Utils.VectorUtils;
using static PathfindingDedicatedServer.Src.Utils.CustomAgentUtils;
using static PathfindingDedicatedServer.Src.Constants.NavConstants;
using PathfindingDedicatedServer.Src.Nav.Crowds.Agents;
using PathfindingDedicatedServer.Src.Nav.Crowds.Agents.Models;
using PathfindingDedicatedServer.Src.Nav.Crowds;
using System.Collections.Concurrent;
using System.Linq;
using System.Numerics;

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
    private readonly CustomCrowd _crowd;
    private readonly uint _dungeonCode;
    private NavManagerState _state = NavManagerState.NONE;
    private readonly int _tickRate = ServerConstants.TICK_RATE; // ms
    private readonly RcAtomicInteger rcAtomicInteger = new (1);

    private readonly S_PlayersLocationUpdate _plu = new() { Positions = [] };
    private readonly S_MonstersLocationUpdate _mlu = new() { Positions = [] };

    private readonly ConcurrentDictionary<string, PlayerAgent> _players = [];
    private readonly ConcurrentDictionary<uint, MonsterAgent> _monsters = [];
    private readonly ConcurrentDictionary<int, StructureAgent> _structures = [];

    private DateTime _startTime;
    private DateTime _prevTime;
    private DateTime _curTime;
    private DateTime _lastSpawnTime;
    private bool _spawnActive = false;
    private uint[] _spawnArr;
    private int _spawnIdx = 0;
    private int _spawnInterval = SPAWN_INTERVAL; // ms

    private RcVec3f _basePos;
    private long _baseRef;
    private int _baseIdx;

    public NavManager(uint dungeonCode) : this(dungeonCode, new DtCrowdConfig(0.6f), NavMeshManager.GetNavMesh(dungeonCode))
    {
    }

    public NavManager(uint dungeonCode, DtCrowdConfig config, DtNavMesh? navMesh)
    {
      ArgumentNullException.ThrowIfNull(navMesh);
      _dungeonCode = dungeonCode;
      _crowd = new(config, navMesh);
      _state = NavManagerState.WAITING;
      DtNavMeshQueryResult result = GetNavMeshQueryResult(
        Storage.GetPos(_dungeonCode, PosType.BASE_SPAWNER, 0),
        3f, 
        2f
      );
      _baseRef = result.NearestRef;
      _basePos = result.NearestPt;
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
    }

    public void End()
    {
      _state = NavManagerState.ENDING;

      _players.Clear();
      _monsters.Clear();
      _structures.Clear();
    }

    public void Reset()
    {
      _state = NavManagerState.WAITING;
    }

    public float Update()
    {
      _curTime = DateTime.UtcNow;
      float deltaTime = (float)(_curTime - _prevTime).TotalSeconds;
      _crowd.Update(deltaTime, null);
      _prevTime = _curTime;
      return deltaTime;
    }

    public double GetCurrentElapsedTime()
    {
      return (DateTime.UtcNow - _curTime).TotalMilliseconds;
    }

    public double GetMilliSecondsDelay()
    {
      double delay = (double)_tickRate - GetCurrentElapsedTime();
      return delay <= 0 ? 0 : delay;
    }

    public S_MonstersLocationUpdate GetMonsterLocations()
    {
      _mlu.Positions.Clear();
      foreach (var monster in _monsters)
      {
        if (monster.Value.vel.Equals(RcVec3f.Zero)) continue;
        //Console.WriteLine($"[ {monster.Key} ] monster moved");
        _mlu.Positions.TryAdd(monster.Key, ToWorldPosition(monster.Value.npos));
      }
      return _mlu;
    }

    public S_PlayersLocationUpdate GetPlayerLocations()
    {
      _plu.Positions.Clear();
      foreach (var player in _players)
      {
        if (player.Value.vel.Equals(RcVec3f.Zero)) continue;
        //Console.WriteLine($"[ {player.Key} ] player moved");
        _plu.Positions.TryAdd(player.Key, ToWorldPosition(player.Value.npos));
      }
      return _plu;
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
    /// 
    /// </summary>
    /// <param name="monsters">key: monsterIdx, value: monsterModel</param>
    public void SetMonsters(Dictionary<uint, uint> monsters)
    {
      _monsters.Clear();
      foreach (var monster in monsters)
      { // try catch?
        var option = Storage.GetMonsterAgentInfo(monster.Value);
        var pos = Storage.GetRandomPos(_dungeonCode, PosType.MONSTER_SPAWNER);

        MonsterAgent agent = new MonsterAgent(monster.Key, rcAtomicInteger.GetAndIncrement(), option, _crowd, pos);
        _monsters.TryAdd(monster.Key, agent);
      }
      _spawnArr = [.. _monsters.Keys];
    }

    //public Dictionary<uint, DtCrowdAgentParams>.KeyCollection GetMonsterIndices ()
    //{
    //  return _monsterOptions.Keys;
    //}

    public void SetPlayers(Dictionary<string, uint> players)
    {
      foreach (var player in players)
      {
        var option = Storage.GetPlayerAgentInfo(player.Value);
        var pos = Storage.GetRandomPos(_dungeonCode, PosType.PLAYER_SPAWNER);

        PlayerAgent agent = new PlayerAgent(player.Key, rcAtomicInteger.GetAndIncrement(), option, _crowd, pos);
        _players.TryAdd(player.Key, agent);
        AddPlayer(agent);
        //players.TryGetValue(accountId, out uint charClass);
        //_playerOptions.Add(accountId, Storage.GetPlayerAgentInfo(charClass));

        //// Add the player to the DtCrowd manager
        //RcVec3f center = Storage.GetRandomPos(_dungeonCode, PosType.PLAYER_SPAWNER);
        //DtNavMeshQueryResult result = GetNavMeshQueryResult(center, 3f, 1.5f);
        //// TODO: handle DtStatus
        //_crowd.GetNavMeshQuery().FindRandomPointWithinCircle(
        //  result.NearestRef,
        //  center,
        //  5f,
        //  _crowd.GetFilter(0),
        //  new RcRand(),
        //  out long randomRef,
        //  out RcVec3f randomPt
        //);
        //AddPlayer(accountId, randomPt, Storage.GetPlayerAgentInfo(charClass));
      }
    }

    public void AddMonster(MonsterAgent agent)
    {
      if (agent.state == DtCrowdAgentState.DT_CROWDAGENT_STATE_INVALID)
      {
        Console.WriteLine($"[ {agent.monsterIdx} ] is currently {agent.state}");
        return;
      }
      _crowd.AddAgent(agent);
      Console.WriteLine($"[ {agent.monsterIdx} ] is currently {agent.state}");
      MoveToBase(agent);
    }

    public void AddMonster(uint monsterIdx)
    {
      if (_monsters.TryGetValue(monsterIdx, out var agent))
      {
        AddMonster(agent);
      }
    }

    public void StartSpawn(ulong timestamp)
    {
      if (_spawnActive) return;
      _spawnActive = true;
      _spawnIdx = 0;
    }

    public void TrySpawn()
    {
      if (!_spawnActive) return;
      if (_spawnArr.Length <= _spawnIdx)
      { // reached the end
        _spawnActive = false;
        Console.WriteLine($"Done spawning {_spawnArr.Length} monsters");
        return;
      }
      DateTime now = DateTime.UtcNow;
      double diff = (DateTime.UtcNow - _lastSpawnTime).TotalMilliseconds;
      if (diff >= _spawnInterval)
      { // can spawn
        AddMonster(_spawnArr[_spawnIdx++]);
        _lastSpawnTime = now;
      }
    }

    /// <summary>
    /// Adds player agent to the crowd, if its state is valid.
    /// </summary>
    /// <param name="agent">PlayerAgent instance that is alread initialized with its Init method.</param>
    public void AddPlayer(PlayerAgent agent)
    {
      if (agent.state == DtCrowdAgentState.DT_CROWDAGENT_STATE_INVALID)
      {
        Console.WriteLine($"[ {agent.accountId} ] is currently {agent.state}");
        return;
      }
      _crowd.AddAgent(agent);
    }

    public void AddPlayer(string accountId)
    {
      if (_players.TryGetValue(accountId, out var agent))
      {
        AddPlayer(agent);
      }
    }

    public void AddStructure(int structureIdx, RcVec3f pos, DtCrowdAgentParams option)
    {
      //_structureOptions.Add(structureIdx, option);
      if (option.userData is AgentAdditionalData agentData)
      {
        if (IsBase(agentData.agentFlag))
        {
          StructureAgent agent = new(structureIdx, 0, option, _crowd, _basePos);
          _structures.TryAdd(structureIdx, agent);
          _crowd.AddAgent(agent);
          Console.WriteLine($"[ {structureIdx}:{agent.idx} ] BASE AGENT STATE: {agent.state}");
          Console.WriteLine($"-- npos {agent.npos}, expected: {_basePos}");
        }
        else
        {
          StructureAgent agent = new(structureIdx, rcAtomicInteger.GetAndIncrement(), option, _crowd, pos);
          _structures.TryAdd(structureIdx, agent);
          _crowd.AddAgent(agent);
        }
        return;
      }
      Console.WriteLine($"Invalid structure with idx [ {structureIdx} ]");
    }

    public void RemoveMonster(uint monsterIdx)
    {
      if (_monsters.TryRemove(monsterIdx, out var value))
      {
        _crowd.RemoveAgent(value);
      }
    }

    public void ClearMonsters()
    {
      foreach (uint monsterIdx in _monsters.Keys)
      {
        RemoveMonster(monsterIdx);
      }
      _monsters.Clear();
    }

    public void RemoveStructure(int structureIdx)
    {
      _structures.TryRemove(structureIdx, out var value);
      if (value != null)
      {
        _crowd.RemoveAgent(value);
      }
    }

    private bool RemoveAgentByAgentIdx(int agentIdx)
    {
      var agent = _crowd.GetAgent(agentIdx);
      if (agent == null) return false;
      _crowd.RemoveAgent(agent);
      return true;
    }

    public DtCrowdAgent? GetMonsterAgent(uint monsterIdx)
    {
      if (_monsters.TryGetValue(monsterIdx, out var monster))
      {
        return monster;
      }
      return null;
    }

    public DtCrowdAgent? GetPlayerAgent(string accountId)
    {
      if (_players.TryGetValue(accountId, out var player))
      {
        return player;
      }
      return null;
    }

    public DtCrowdAgent? GetStructureAgent(int structureIdx)
    {
      if (_structures.TryGetValue(structureIdx, out var structure))
      {
        return structure;
      }
      return null;
    }

    public NavManagerState GetState()
    {
      return _state;
    }

    /// <summary>
    /// Updates monster's custom data, renew dest position if necessary.
    /// </summary>
    /// <param name="monsterIdx"></param>
    public void ReCalc(uint monsterIdx)
    {
      if (!_monsters.ContainsKey(monsterIdx))
      {
        return;
      }
      // Get agent
      DtCrowdAgent? agent = GetMonsterAgent(monsterIdx);
      if (agent?.option.userData is AgentAdditionalData agentData)
      {
        // Get target agent
        var targetAgentIdx = agentData.GetTargetAgentIdx();
        if (targetAgentIdx < 0)
        {
          MoveToBase(agent);
          return;
        }
        var targetAgent = _crowd.GetAgent(targetAgentIdx);

        if (targetAgent?.option.userData is AgentAdditionalData targetAgentData)
        {
          if (IsValidAgent(targetAgentData.agentFlag))
          {
            // is a valid target
            
            if (IsMonster(targetAgentData.agentFlag))
            { // is a monster
              //agentData.SetTargetActualDistance(0);
            }
            else if (IsPlayer(targetAgentData.agentFlag))
            { // is a player
              MoveToTarget(agent, targetAgent);
              //agentData.SetTargetActualDistance(agent, targetAgent);
            }
            else
            { // is a structure or base
              //if (agent.state != DtCrowdAgentState.DT_CROWDAGENT_STATE_WALKING)
              //{
              //  MoveToBase(agent);
              //}
              //agentData.SetTargetActualDistance(agent, targetAgent);
            }
            return;
          }
        }

        // Not a valid target, reset to base?
        Console.WriteLine("INVALID TARGET DETECTED");
        MoveToBase(agent);
      }
    }

    public void ReCalcExt(uint monsterIdx)
    {
      
    }

    public void ReCalcAll()
    {
      foreach (var monster in _monsters)
      {
        ReCalc(monster.Key);
      }
    }

    public void SetMonsterDest(uint monsterIdx, DtCrowdAgent? targetAgent)
    {
      DtCrowdAgent? agent = GetMonsterAgent(monsterIdx);
      if (agent?.option.userData is AgentAdditionalData data)
      {
        data.SetTargetAgentIdx(targetAgent?.idx ?? -1);
      }
      if (agent == null) return;
      if (targetAgent == null)
      {
        MoveToBase(agent);
        return;
      }
      MoveToTarget(agent, targetAgent);
    }

    public void SetMonsterDest(uint monsterIdx, TargetStructure target)
    {
      SetMonsterDest(monsterIdx, GetStructureAgent(target.StructureIdx));
    }

    public void SetMonsterDest(uint monsterIdx, TargetPlayer target)
    {
      SetMonsterDest(monsterIdx, GetPlayerAgent(target.AccountId));
    }

    public void SetPlayerDest(string accountId, RcVec3f? pos)
    {
      var agent = GetPlayerAgent(accountId);
      if (agent == null) return;
      MoveTo(agent, pos);
    }

    public void MoveTo (DtCrowdAgent agent, RcVec3f? pos)
    {
      if (pos == null || pos == agent.npos)
      {
        Halt(agent);
        return;
      }
      var result = GetNavMeshQueryResult(pos.Value, HORIZONTAL_SEARCH_RAD, VERTICAL_SEARCH_RAD);
      _crowd.RequestMoveTarget(agent, result.NearestRef, result.NearestPt);
    }

    public void MoveToTarget (DtCrowdAgent agent, DtCrowdAgent targetAgent)
    {
      if (agent.option.userData is AgentAdditionalData agentData)
      {
        if (targetAgent.option.userData is AgentAdditionalData targetData)
        { // valid target
          Console.WriteLine($"{agentData.agentFlag} moving to {targetData.agentFlag}");
          agentData.SetTargetAgentIdx(targetAgent.idx);
          MoveTo(agent, targetAgent.npos);
        }
        else
        { // invalid target
          Console.WriteLine($"Invalid target: {targetAgent.idx}");
          agentData.SetTargetAgentIdx(agent.idx);
          MoveTo(agent, agent.npos);
        }
      }
      else
      {
        return;
      }
    }

    public void MoveToBase (DtCrowdAgent agent)
    {
      DtCrowdAgent baseAgent = _crowd.GetAgent(0) ?? throw new InvalidDataException("MoveToBase Error: base agent is missing");
      MoveToTarget(agent, baseAgent);
      // TODO: Get nearest circular border position
    }

    public void Halt(DtCrowdAgent agent)
    {
      if (agent.option.userData is AgentAdditionalData data)
      {
        data.SetTargetAgentIdx(agent.idx);
      }
      _crowd.ResetMoveTarget(agent);
    }

    public void Halt(string accountId)
    {
      var agent = GetPlayerAgent(accountId);
      if (agent == null) return;
      Halt(agent);
    }

    public void Halt(uint monsterIdx)
    {
      var agent = GetMonsterAgent(monsterIdx);
      if (agent == null) return;
      Halt(agent);
    }
  }
}
