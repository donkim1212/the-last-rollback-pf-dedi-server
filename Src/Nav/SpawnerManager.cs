using DotRecast.Core.Numerics;
using PathfindingDedicatedServer.Src.Constants;

namespace PathfindingDedicatedServer.Src.Nav;
public class SpawnerManager
{
  private static readonly Dictionary<int, List<RcVec3f>> _dungeonSpawners = new Dictionary<int, List<RcVec3f>>();

  public static void Init ()
  {
    // TODO: read from JSON
    _dungeonSpawners.Clear ();
    AddSpawner(1, SpawnerConstants.SPAWNER_01_POSITION);
    AddSpawner(1, SpawnerConstants.SPAWNER_02_POSITION);
    AddSpawner(1, SpawnerConstants.SPAWNER_03_POSITION);
    AddSpawner(1, SpawnerConstants.SPAWNER_04_POSITION);
    AddSpawner(1, SpawnerConstants.SPAWNER_05_POSITION);
    
  }

  public static void InitDungeonSpawnersList (int dungeonCode)
  {
    _dungeonSpawners.Add(dungeonCode, []);
    Console.WriteLine(_dungeonSpawners[dungeonCode]);
  }

  public static void AddSpawner (int dungeonCode, RcVec3f spawnerPos)
  {
    if (!_dungeonSpawners.ContainsKey(dungeonCode))
    {
      InitDungeonSpawnersList(dungeonCode);
    }
    _dungeonSpawners[dungeonCode].Add(spawnerPos);
  }

  public static RcVec3f GetRandomSpawnerPosition(int dungeonCode, int seed)
  {
    Random rand = new Random(seed);
    if (_dungeonSpawners[dungeonCode] == null)
    {
      throw new InvalidOperationException($"No dungeon spawners set for dungeonCode {dungeonCode}");
    }
    int idx = rand.Next(0, _dungeonSpawners[dungeonCode].Count);
    RcVec3f pos = _dungeonSpawners[dungeonCode][idx];
    return pos;
  }
}
