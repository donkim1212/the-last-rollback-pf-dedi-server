using DotRecast.Core.Numerics;
using DotRecast.Detour.Crowd;
using PathfindingDedicatedServer.Src.Nav.Crowds.Agents.Models.Base;

namespace PathfindingDedicatedServer.Src.Nav.Crowds.Agents.Models
{
  public class PlayerAgent : CustomAgent
  {
    public readonly string accountId;

    public PlayerAgent(string accountId, int agentIdx) : base(agentIdx)
    {
      this.accountId = accountId;
    }

    public PlayerAgent(string accountId, int idx, DtCrowdAgentParams option, DtCrowd crowd, RcVec3f pos) : base(idx)
    {
      this.accountId = accountId;
      Init(crowd, option, pos);
    }
  }
}
