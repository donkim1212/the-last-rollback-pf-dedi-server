using DotRecast.Core;
using DotRecast.Core.Numerics;
using DotRecast.Detour.Crowd;
using PathfindingDedicatedServer.Src.Constants;
using PathfindingDedicatedServer.Src.Data.Abstracts;
using PathfindingDedicatedServer.Src.Monsters;
using PathfindingDedicatedServer.Src.Utils.FileLoader;
using System.Collections.ObjectModel;

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
    public static StructureAgentData? StructureAgentData { get; set; }
    public static DungeonData? DungeonData { get; set; }
    public static AgentUpdateFlagsData? AgentUpdateFlagsData { get; set; }

    private static readonly Dictionary<uint, List<RcVec3f>> _monsterSpawnPosList = [];
    private static readonly Dictionary<uint, List<RcVec3f>> _playerSpawnPosList = [];
    private static readonly Dictionary<uint, List<RcVec3f>> _baseSpawnPosList = [];

    public static Random Rand { get; set; }

    public static void InitStorage ()
    {
      JsonFileLoader loader = new();
      MonsterAgentData monsterData = loader.LoadFileFromAssets<MonsterAgentData>("MonsterAgentInfo.json");
      MonsterAgentData = monsterData;
      Console.WriteLine($"Loaded {monsterData.Name}");
      Console.WriteLine($"-- version: {monsterData.Version}");
      Console.WriteLine($"-- test: {monsterData.Data.First().MonsterModel}");
      PlayerAgentData playerData = loader.LoadFileFromAssets<PlayerAgentData>("PlayerAgentInfo.json");
      PlayerAgentData = playerData;
      Console.WriteLine($"Loaded {playerData.Name}");
      Console.WriteLine($"-- version: {playerData.Version}");
      Console.WriteLine($"-- test: {playerData.Data.First().CharClass}");
      StructureAgentData structureAgentData = loader.LoadFileFromAssets<StructureAgentData>("StructureAgentInfo.json");
      StructureAgentData = structureAgentData;
      Console.WriteLine($"Loaded {structureAgentData.Name}");
      Console.WriteLine($"-- version: {structureAgentData.Version}");
      Console.WriteLine($"-- test: {structureAgentData.Data.First().StructureModel}");
      DungeonData dungeonData = loader.LoadFileFromAssets<DungeonData>("DungeonInfo.json");
      DungeonData = dungeonData;
      Console.WriteLine($"Loaded {dungeonData.Name}");
      Console.WriteLine($"-- version: {dungeonData.Version}");
      Console.WriteLine($"-- test: {dungeonData.Data.First().X}");
      
      InitSpawnPosLists();
      Rand = new Random(ServerConstants.RANDOM_SEED);
    }

    #pragma warning disable CS8602
    private static void InitSpawnPosLists ()
    {
      foreach(DungeonInfo dungeonInfo in DungeonData.Data)
      {
        var dict = GetDictionary(dungeonInfo.PosType);
        if (!dict.ContainsKey(dungeonInfo.DungeonCode))
        {
          dict.TryAdd(dungeonInfo.DungeonCode, []);
        }
        dict[dungeonInfo.DungeonCode].Add(
          new RcVec3f(dungeonInfo.X, dungeonInfo.Y, dungeonInfo.Z)
        );
      }
    }

    private static void InitAgentInfoDictonaries ()
    {

    }

    private static Dictionary<uint, List<RcVec3f>>? GetDictionary(PosType posType)
    {
      switch(posType)
      {
        case (PosType.PLAYER_SPAWNER):
          return _playerSpawnPosList;
        case (PosType.MONSTER_SPAWNER):
          return _monsterSpawnPosList;
        case (PosType.BASE_SPAWNER):
          return _baseSpawnPosList;
        default:
          return null;
      }
    }

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

    public static DtCrowdAgentParams GetStructureAgentInfo(uint structureModel)
    {
      if (StructureAgentData == null)
      {
        throw new InvalidOperationException("GetPlayerAgentInfo Error: StructureAgentData is NULL");
      }
      StructureAgentInfo? info = StructureAgentData.Data.Find((data) => data.StructureModel == structureModel);
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
        userData = new AgentAdditionalData(),
      };
    }

    public static List<RcVec3f> GetSpawnPosList (uint dungeonCode, PosType posType)
    {
      return GetDictionary(posType)[dungeonCode];
    }

    public static RcVec3f GetRandomPos (uint dungeonCode, PosType posType)
    {
      var list = GetDictionary(posType)[dungeonCode];
      return list[Rand.Next(0, list.Count)];
    }

    public static RcVec3f GetPos(uint dungeonCode, PosType posType, int index)
    {
      var list = GetDictionary(posType)[dungeonCode];
      return list[index];
    }
  }
}
