using DotRecast.Detour;

namespace PathfindingDedicatedServer.Nav;
public class NavMeshManager
{
  private static readonly Dictionary<uint, DtNavMesh> _navMeshes = [];

  public static void AddNavMesh(uint dungeonCode, DtNavMesh navMesh)
  {
    ArgumentNullException.ThrowIfNull(dungeonCode);
    ArgumentNullException.ThrowIfNull(navMesh);
    _navMeshes.Add(dungeonCode, navMesh);
  }

  public static DtNavMesh? GetNavMesh(uint dungeonCode)
  {
    return _navMeshes[dungeonCode];
  }

  //public static void RemoveNavMesh(int id)
  //{
  //  ArgumentNullException.ThrowIfNull(id);
  //  _navMeshes.Remove(id);
  //}
}
