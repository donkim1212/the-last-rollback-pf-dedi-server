using PathfindingDedicatedServer.Src.Data.Abstracts;

namespace PathfindingDedicatedServer.Src.Data
{
  public enum AgentUpdateFlagsMapping
  {

  }

  public class AgentUpdateFlagsIndex
  {
    public string Name { get; set; }
    public int Flags { get; set; } 
  }
  public class AgentUpdateFlagsData : JsonData<AgentUpdateFlagsIndex>
  {
  }
}
