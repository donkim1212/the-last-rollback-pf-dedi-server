using DotRecast.Core.Numerics;

namespace PathfindingDedicatedServer.Src.Utils
{
  internal class Utils
  {
    public static WorldPosition ToWorldPosition(RcVec3f pos)
    {
      return new()
      {
        X = pos.X,
        Y = pos.Y,
        Z = pos.Z
      };
    }

    public static RcVec3f ToRcVector(WorldPosition pos)
    {
      return new()
      {
        X = pos.X,
        Y = pos.Y,
        Z = pos.Z
      };
    }
  }
}
