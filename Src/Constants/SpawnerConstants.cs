using System;
using DotRecast.Core.Numerics;

namespace PathfindingDedicatedServer.Src.Constants;
public class SpawnerConstants
{
  // TODO: store these in JSON
  public readonly RcVec3f SPAWNER_01_POSITION = new (43.5f, 1.72f, 119.63f);
  public readonly RcVec3f SPAWNER_02_POSITION = new (-5.93f, 1.06f, 92.43f);
  public readonly RcVec3f SPAWNER_03_POSITION = new (-46.71f, 0.48f, 132.75f);
  public readonly RcVec3f SPAWNER_04_POSITION = new (-41.66f, 0.48f, 90.63f);
  public readonly RcVec3f SPAWNER_05_POSITION = new (35.48f, 0.48f, 88.26f);

  public static RcVec3f GetRandomSpawnPosition()
  {
    // TODO: randomly select from the spawner positions and return the value.
    return new RcVec3f();
  }
}