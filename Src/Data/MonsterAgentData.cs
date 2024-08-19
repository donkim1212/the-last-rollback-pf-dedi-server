using PathfindingDedicatedServer.Src.Data.Abstracts;

namespace PathfindingDedicatedServer.Src.Data
{
  public class MonsterAgentInfo : AgentInfo
  {
    public int MonsterModel { get; set; }
  }

  public class MonsterAgentData : AgentData<MonsterAgentInfo>
  {
  }
}
