using DotRecast.Detour;
using DotRecast.Detour.Crowd;

namespace PathfindingDedicatedServer.Src.Nav.Crowds
{
  internal class CustomCrowd : DtCrowd
  {
    public CustomCrowd(DtCrowdConfig config, DtNavMesh nav) : base(config, nav)
    {
    }
  }
}
