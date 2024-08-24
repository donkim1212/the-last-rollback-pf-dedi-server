using DotRecast.Detour.Crowd;
using PathfindingDedicatedServer.Src.Data;
using PathfindingDedicatedServer.Src.Utils;

namespace PathfindingDedicatedServer.Src.Nav.Crowds.Agents
{
  public class MonsterAgentParams : DtCrowdAgentParams
  {
    public readonly uint monsterModel;

    public MonsterAgentParams(uint monsterModel)
    {
      this.monsterModel = monsterModel;
      DtCrowdAgentParams agentParams = Storage.GetMonsterAgentInfo(monsterModel);
      radius = agentParams.radius;
      height = agentParams.height;
      maxAcceleration = agentParams.maxAcceleration;
      maxSpeed = agentParams.maxSpeed;
      collisionQueryRange = agentParams.collisionQueryRange;
      pathOptimizationRange = agentParams.pathOptimizationRange; // temp
      separationWeight = agentParams.separationWeight;
      updateFlags = agentParams.updateFlags; // needs checking
      obstacleAvoidanceType = agentParams.obstacleAvoidanceType;
      queryFilterType = agentParams.queryFilterType;
      userData = new AgentAdditionalData(AgentFlag.MONSTER);
    }
  }
}
