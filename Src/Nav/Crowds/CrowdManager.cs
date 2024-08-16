using DotRecast.Detour;
using DotRecast.Detour.Crowd;

namespace PathfindingDedicatedServer.Src.Nav.Crowds
{
  public class CrowdManager
  {
    private DtCrowd _crowd;
    private DtCrowdConfig _config;
    private DtNavMesh _mesh;

    public CrowdManager(int id) : this(new DtCrowdConfig(0.6f), NavMeshes.GetNavMesh(id))
    {
    }

    public CrowdManager(DtCrowdConfig config, DtNavMesh? navMesh)
    {
      if (navMesh == null)
      {
          throw new NullReferenceException($"CrowdManager: NavMesh is null");
      }
      _config = config;
      _mesh = navMesh;
      _crowd = new (config, navMesh);
    }


  }
}
