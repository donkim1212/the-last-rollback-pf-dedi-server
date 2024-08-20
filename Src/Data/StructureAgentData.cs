using PathfindingDedicatedServer.Src.Data.Abstracts;

namespace PathfindingDedicatedServer.Src.Data
{
  public class StructureInfo : AgentInfo
  {
    public uint StructureModel { get; set; }
  }

  public class StructureAgentData : AgentData<StructureInfo>
  {
  }
}
