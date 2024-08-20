using PathfindingDedicatedServer.Src.Data.Abstracts;

namespace PathfindingDedicatedServer.Src.Data
{
  public class MonsterAgentInfo : AgentInfo
  {
    public uint MonsterModel { get; set; }
  }

  public class MonsterAgentData : AgentData<MonsterAgentInfo>
  {
  }
}
