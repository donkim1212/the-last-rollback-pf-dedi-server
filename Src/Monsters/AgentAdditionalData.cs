using DotRecast.Core.Numerics;

namespace PathfindingDedicatedServer.Src.Monsters
{
  internal class AgentAdditionalData
  {
    private int _prevTargeAgentIdx = -2;
    private int _targetAgentIdx = -1; // Defaults to base, value is temporary
    private float _targetActualDistance = 100f;

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

  }
}
