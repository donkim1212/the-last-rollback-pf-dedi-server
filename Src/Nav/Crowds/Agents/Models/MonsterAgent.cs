using DotRecast.Core.Numerics;
using DotRecast.Detour.Crowd;
using PathfindingDedicatedServer.Src.Nav.Crowds.Agents.Models.Base;

namespace PathfindingDedicatedServer.Src.Nav.Crowds.Agents.Models
{
  public class MonsterAgent : CustomAgent
  {
    public readonly uint monsterIdx;

    public MonsterAgent(uint monsterIdx, int agentIdx) : base(agentIdx)
    {
      this.monsterIdx = monsterIdx;
    }

    public MonsterAgent(uint monsterIdx, int idx, DtCrowdAgentParams option, DtCrowd crowd, RcVec3f pos) : base(idx)
    {
      this.monsterIdx = monsterIdx;
      Init(crowd, option, pos);
    }
  }
}
