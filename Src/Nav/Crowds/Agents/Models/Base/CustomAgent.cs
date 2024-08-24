using DotRecast.Core.Numerics;
using DotRecast.Detour.Crowd;

namespace PathfindingDedicatedServer.Src.Nav.Crowds.Agents.Models.Base
{
  public class CustomAgent : DtCrowdAgent
  {
    public RcVec3f ppos { get; set; }

    public CustomAgent(int idx) : base(idx)
    {
      state = DtCrowdAgentState.DT_CROWDAGENT_STATE_INVALID;
    }

    public void Init(DtCrowd crowd, DtCrowdAgentParams option, RcVec3f pos)
    {
      corridor.Init(DtCrowdConst.MAX_PATH_RESULT); // needs checking
      this.option = option;

      var status = crowd.GetNavMeshQuery().FindNearestPoly(
        pos,
        crowd.GetQueryExtents(),
        crowd.GetFilter(0),
        out long nearestRef,
        out RcVec3f nearestPt,
        out bool isOverPoly
      );

      if (status.Failed())
      {
        nearestPt = pos;
        nearestRef = 0;
      }

      corridor.Reset(nearestRef, nearestPt);
      boundary.Reset();
      partial = false;

      topologyOptTime = 0;
      targetReplanTime = 0;
      nneis = 0;

      dvel = RcVec3f.Zero;
      nvel = RcVec3f.Zero;
      vel = RcVec3f.Zero;
      npos = nearestPt;
      ppos = npos;

      desiredSpeed = 0;

      if (nearestRef != 0)
      {
        state = DtCrowdAgentState.DT_CROWDAGENT_STATE_WALKING;
      }
      else
      {
        state = DtCrowdAgentState.DT_CROWDAGENT_STATE_INVALID;
      }

      targetState = DtMoveRequestState.DT_CROWDAGENT_TARGET_NONE;
    }

    public override void Integrate(float dt)
    {
      // Fake dynamic constraint.
      float maxDelta = option.maxAcceleration * dt;
      RcVec3f dv = RcVec3f.Subtract(nvel, vel);
      float ds = dv.Length();
      if (ds > maxDelta)
        dv = dv * (maxDelta / ds);
      vel = RcVec3f.Add(vel, dv);

      // Integrate
      if (vel.Length() > 0.2f) // 0.0001f
      {
        //Console.WriteLine($"ppos {ppos} : npos {npos}");
        ppos = npos;
        npos = RcVec.Mad(npos, vel, dt);
      }
      else
        vel = RcVec3f.Zero;
    }

    public AgentAdditionalData GetUserData()
    {

      if (option.userData is AgentAdditionalData agentData)
      {
        return agentData;
      }
      return new AgentAdditionalData();
    }
  }
}
