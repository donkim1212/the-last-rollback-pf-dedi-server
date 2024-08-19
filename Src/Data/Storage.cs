using DotRecast.Detour.Crowd;
using PathfindingDedicatedServer.Src.Data.Abstracts;

namespace PathfindingDedicatedServer.Src.Data
{
  internal class Storage
  {
    private static readonly DtCrowdAgentParams _defaultCrowdAgentParams = new()
    {
      radius = 0.3f,
      height = 1.5f,
      maxAcceleration = 100f,
      maxSpeed = 2f,
      collisionQueryRange = 0.6f,
      pathOptimizationRange = 10f, // temp
      separationWeight = 0,
      updateFlags = DtCrowdAgentUpdateFlags.DT_CROWD_ANTICIPATE_TURNS,
      obstacleAvoidanceType = 0,
      queryFilterType = 0,
      userData = new(),
    };
    public static MonsterAgentData? MonsterAgentData { get; set; }
    public static PlayerAgentData? PlayerAgentData { get; set; }

    public static DtCrowdAgentParams GetDefaultAgentInfo()
    {
      return _defaultCrowdAgentParams;
    }

    public static DtCrowdAgentParams GetMonsterAgentInfo(uint monsterModel)
    {
      if (MonsterAgentData == null)
      {
        throw new InvalidOperationException("GetMonsterAgentInfo Error: MonsterAgentData is NULL");
      }
      MonsterAgentInfo? info = MonsterAgentData.Data.Find((data) => data.MonsterModel == monsterModel);
      if (info == null)
      {
        return _defaultCrowdAgentParams;
      }
      return ConvertToDtAgentParams(info);
    }

    public static DtCrowdAgentParams GetPlayerAgentInfo(uint charClass)
    {
      if (PlayerAgentData == null)
      {
        throw new InvalidOperationException("GetPlayerAgentInfo Error: PlayerAgentData is NULL");
      }
      PlayerAgentInfo? info = PlayerAgentData.Data.Find((data) => data.CharClass == charClass);
      if (info == null)
      {
        return _defaultCrowdAgentParams;
      }
      return ConvertToDtAgentParams(info);
    }

    private static DtCrowdAgentParams ConvertToDtAgentParams(AgentInfo agentInfo)
    {
      return new()
      {
        radius = agentInfo.Radius,
        height = agentInfo.Height,
        maxAcceleration = agentInfo.MaxAcc,
        maxSpeed = agentInfo.MaxSpd,
        collisionQueryRange = agentInfo.CollisionQueryRange,
        pathOptimizationRange = agentInfo.PathOptRange, // temp
        separationWeight = agentInfo.SeparationWeight,
        updateFlags = agentInfo.UpdateFlags, // needs checking
        obstacleAvoidanceType = agentInfo.ObsAvoidanceType,
        queryFilterType = agentInfo.QueryFilterType,
        userData = new(),
      };
    }
  }
}
