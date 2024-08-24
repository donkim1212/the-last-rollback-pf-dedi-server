using DotRecast.Core.Numerics;
using DotRecast.Detour.Crowd;
using PathfindingDedicatedServer.Src.Nav.Crowds.Agents.Models.Base;

namespace PathfindingDedicatedServer.Src.Nav.Crowds.Agents.Models
{
  public class StructureAgent : CustomAgent
  {
    public readonly int structureIdx;
    public StructureAgent(int structureIdx, int agentIdx) : base(agentIdx)
    {
      this.structureIdx = structureIdx;
    }

    public StructureAgent(int structureIdx, int idx, DtCrowdAgentParams option, DtCrowd crowd, RcVec3f pos) : base(idx)
    {
      this.structureIdx = structureIdx;
      Init(crowd, option, pos);
    }
  }
}
