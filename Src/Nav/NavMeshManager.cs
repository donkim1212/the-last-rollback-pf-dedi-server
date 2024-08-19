using DotRecast.Detour;

namespace PathfindingDedicatedServer.Nav;
public class NavMeshManager
{
  private static readonly Dictionary<int, DtNavMesh> _navMeshes = [];

  public static void AddNavMesh (int id, DtNavMesh navMesh)
  {
    ArgumentNullException.ThrowIfNull(id);
    ArgumentNullException.ThrowIfNull(navMesh);
    _navMeshes.Add(id, navMesh);
  }

  public static DtNavMesh? GetNavMesh(int id)
  {
    return _navMeshes[id];
  }

  //public static void RemoveNavMesh(int id)
  //{
  //  ArgumentNullException.ThrowIfNull(id);
  //  _navMeshes.Remove(id);
  //}
}
