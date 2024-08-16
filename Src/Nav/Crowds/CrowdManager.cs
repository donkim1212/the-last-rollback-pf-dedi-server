using System.Threading;
using System.Threading.Tasks;
using DotRecast.Core.Numerics;
using DotRecast.Detour;
using DotRecast.Detour.Crowd;
using DotRecast.Recast;
using PathfindingDedicatedServer.Src.Constants;

namespace PathfindingDedicatedServer.Src.Nav.Crowds
{
  public enum CrowdManagerState
  {
    NONE, WAITING, RUNNING, ENDING
  }

  public class CrowdManager
  {
    private readonly DtCrowd _crowd;
    private CrowdManagerState _state = CrowdManagerState.NONE;

    private Dictionary<int, int> _monsters = [];
    private DateTime _startTime;

    //private Thread _thread;
    private int _tickRate = 20; // 0.02s (FixedUpdate, will need rate limitting)

    public CrowdManager(int dungeonCode) : this(new DtCrowdConfig(0.6f), NavMeshes.GetNavMesh(dungeonCode))
    {
    }

    public CrowdManager(DtCrowdConfig config, DtNavMesh? navMesh)
    {
      ArgumentNullException.ThrowIfNull(navMesh);
      _crowd = new(config, navMesh);
    }

    public void Start()
    {
      // TODO: gameloop?
      _state = CrowdManagerState.RUNNING;
      Task.Run(() => GameLoop());
    }

    public void End()
    {
      // TODO: cleanup?
      _state = CrowdManagerState.ENDING;
    }

    private async Task GameLoop()
    {
      _startTime = DateTime.Now;
      DateTime prevTime = _startTime;
      while (_state == CrowdManagerState.RUNNING)
      {
        DateTime curTime = DateTime.Now;
        
        float deltaTime = (float)(curTime - prevTime).TotalSeconds;
        Update(deltaTime);
      }
    }

    private void Update(float deltaTime)
    {
      _crowd.Update(deltaTime, null);
    }

    public void AddMonster(int monsterIdx, RcVec3f pos, DtCrowdAgentParams option)
    {
      //new DtCrowdAgentParams();
      DtCrowdAgent agent = _crowd.AddAgent(pos, option);
      _monsters.Add(monsterIdx, agent.idx);
    }

    public void AddMonster(int monsterIdx, DtCrowdAgentParams option)
    {
      AddMonster(monsterIdx, SpawnerConstants.GetRandomSpawnPosition(), option);
    }

    public void AddMonster(int monsterIdx)
    {
      // TODO: fetch monster's agent param stats from JSON
      DtCrowdAgentParams option = new() // temp stats
      {
        radius = 0.6f,
        height = 1.5f,
        maxAcceleration = 5f,
        maxSpeed = 3.5f,
        
      };
      AddMonster(monsterIdx, option);
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

    public CrowdManagerState GetState()
    {
      return _state;
    }
  }
}
