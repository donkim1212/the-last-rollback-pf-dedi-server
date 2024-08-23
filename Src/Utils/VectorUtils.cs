using DotRecast.Core;
using DotRecast.Core.Numerics;

namespace PathfindingDedicatedServer.Src.Utils
{
  public class VectorUtils
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

    public static RcVec3f ToRcVector(WorldPosition? pos)
    {
      if (pos == null)
      {
        return RcVec3f.Zero;
      }
      return new()
      {
        X = pos.X,
        Y = pos.Y,
        Z = pos.Z
      };
    }

    public static bool VectorEquals(WorldPosition pos, RcVec3f other)
    {
      return (pos.X == other.X && pos.Y == other.Y && pos.Z == other.Z);
    }

    public static float CalcDistance (RcVec3f a, RcVec3f b)
    {
      RcVec3f diff = a - b;
      return (float) Math.Sqrt(RcMath.Sqr(diff.X) + RcMath.Sqr(diff.Y) + RcMath.Sqr(diff.Z));
    }

    public static float CalcActualDistance (RcVec3f a, float aRad, RcVec3f b, float bRad)
    {
      return Math.Abs(CalcDistance(a, b) - (aRad + bRad));
    }

    public static RcVec3f CalcDirectionNormalized (RcVec3f from, RcVec3f to)
    {
      RcVec3f diff = from - to;
      float magnitude = (float)Math.Sqrt(RcMath.Sqr(diff.X) + RcMath.Sqr(diff.Y) + RcMath.Sqr(diff.Z));
      if (magnitude == 0)
      {
        // The two vectors are on the same position
        return RcVec3f.Zero;
      }

      return diff * (1 / magnitude);
    }
  }
}
