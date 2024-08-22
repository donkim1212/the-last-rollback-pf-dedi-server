using DotRecast.Core.Numerics;

namespace PathfindingDedicatedServer.Src.Monsters
{
  public enum AgentType
  {
    NONE = 0,
    PLAYER = 1,
    MONSTER = 2,
    STRUCTURE = 3,
    BASE = 4,
  }

  public class AgentAdditionalData
  {
    public readonly AgentType agentType;
    private int _prevTargeAgentIdx = -2;
    private int _targetAgentIdx = -1; // Defaults to base, value is temporary
    private float _targetActualDistance = 100f;

    public AgentAdditionalData()
    {
      agentType = AgentType.NONE;
    }

    public AgentAdditionalData(AgentType agentType)
    {
      this.agentType = agentType;
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
      Utils.Utils.CalcActualDistance(myPos, myRad, targetPos, targetRad);
    }

    public float GetTargetActualDistance ()
    {
      return _targetActualDistance;
    }

    public bool IsAgentType(AgentType agentType)
    {
      return this.agentType == agentType;
    }
  }
}
