using DotRecast.Core.Numerics;
using DotRecast.Detour.Crowd;
using PathfindingDedicatedServer.Src.Utils;

namespace PathfindingDedicatedServer.Src.Monsters
{
  public class AgentAdditionalData
  {
    public readonly AgentFlag agentFlag;
    private int _prevTargeAgentIdx = -2;
    private int _targetAgentIdx = -1; // Defaults to base, value is temporary
    private float _targetActualDistance = 100f;

    public AgentAdditionalData()
    {
      agentFlag = AgentFlag.NONE;
    }

    public AgentAdditionalData(AgentFlag agentType)
    {
      this.agentFlag = agentType;
    }

    public void SetTargetAgentIdx(int targetAgentIdx)
    {
      _prevTargeAgentIdx = _targetAgentIdx;
      _targetAgentIdx = targetAgentIdx;
      SetTargetActualDistance(100f); // temp
    }

    public int GetTargetAgentIdx()
    {
      return _targetAgentIdx;
    }

    public int GetPrevTargetAgentIdx()
    {
      return _prevTargeAgentIdx;
    }

    public void SetTargetActualDistance (float dist)
    {
      _targetActualDistance = dist;
    }

    public void SetTargetActualDistance (RcVec3f myPos, float myRad, RcVec3f targetPos, float targetRad)
    {
      _targetActualDistance = Utils.Utils.CalcActualDistance(myPos, myRad, targetPos, targetRad);
    }

    public void SetTargetActualDistance (DtCrowdAgent agent, DtCrowdAgent target)
    {
      SetTargetActualDistance(agent.npos, agent.option.radius, target.npos, target.option.radius);
    }

    public float GetTargetActualDistance ()
    {
      return _targetActualDistance;
    }

    public bool IsAgentFlag(AgentFlag agentType)
    {
      return this.agentFlag == agentType;
    }

    public bool TargetChanged()
    {
      return _prevTargeAgentIdx != _targetActualDistance;
    }
  }
}
