using DotRecast.Core.Numerics;
using DotRecast.Detour;
using DotRecast.Detour.Crowd;
using PathfindingDedicatedServer.Src.Nav.Crowds.Agents;
using PathfindingDedicatedServer.Src.Nav.Crowds.Agents.Models;

namespace PathfindingDedicatedServer.Src.Nav.Crowds
{
  internal class CustomCrowd : DtCrowd
  {
    public CustomCrowd(DtCrowdConfig config, DtNavMesh nav) : base(config, nav)
    {
    }
  }
}
