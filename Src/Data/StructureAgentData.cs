using PathfindingDedicatedServer.Src.Data.Abstracts;

namespace PathfindingDedicatedServer.Src.Data
{
  public class StructureAgentInfo : AgentInfo
  {
    public uint StructureModel { get; set; }
  }

  public class StructureAgentData : AgentData<StructureAgentInfo>
  {
  }
}
