using PathfindingDedicatedServer.Src.Data;

namespace PathfindingDedicatedServer.Src.Models
{
  internal class Monster
  {
    private readonly int _agentIdx;
    private readonly MonsterAgentInfo _monsterInfo;

    public Monster(MonsterAgentInfo monsterInfo, int agentIdx)
    {
      _monsterInfo = monsterInfo;
      _agentIdx = agentIdx;
    }

    public MonsterAgentInfo GetMonsterInfo()
    {
      return _monsterInfo;
    }

    public int GetAgentIdx()
    {
      return _agentIdx;
    }
  }
}
