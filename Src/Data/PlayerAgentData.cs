using PathfindingDedicatedServer.Src.Data.Abstracts;

namespace PathfindingDedicatedServer.Src.Data
{
  public class PlayerAgentInfo : AgentInfo
  {
    public int CharClass { get; set; }
  }
  internal class PlayerAgentData : AgentData<PlayerAgentInfo>
  {
  }
}
