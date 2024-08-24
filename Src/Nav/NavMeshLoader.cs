using DotRecast.Core;
using DotRecast.Detour;
using DotRecast.Detour.Io;
using DotRecast.Recast;
using PathfindingDedicatedServer.Nav.Config;
using PathfindingDedicatedServer.Src.Constants;

namespace PathfindingDedicatedServer.Nav;
public class NavMeshLoader
{
  /// <summary>
  /// Reads all files in Assets directory and loads all .NAVMESH files into NavMeshes list.
  /// </summary>
  public static void LoadAllNavMeshAssets()
  {
    try
    {
      // TODO: map the indices somehow
      string[] files = Directory.GetFiles(PathConstants.ASSETS_REL_PATH, PathConstants.NAVMESH_EXT, SearchOption.AllDirectories);
      foreach (string file in files)
      {
        DtNavMesh? navMesh = LoadNavMesh(file);
        if (navMesh != null)
        {
          // TODO: get index from filename
          // ./Assets/001_town.navmesh
          string[] split = file.Split('/');
          uint idx = uint.Parse(split[split.Length - 1].Split('_')[0]);

          NavMeshManager.AddNavMesh(idx, navMesh);
          Console.WriteLine($"[ {idx} ] Loaded {file}");
          float[] verts = navMesh.GetTile(1).data.verts;
          // Testing
          Console.WriteLine($"-- id: {idx} | Tile[1] verts: {verts[0]} {verts[1]} {verts[2]}");
        }
        else
        {
          Console.WriteLine($"Failed loading file: {file}");
        }
      }
    }
    catch (Exception e)
    {
      Console.WriteLine("LoadAllNavMeshAssets Error: " + e.Message);
    }
  }

  /// <summary>
  /// Loads NavMesh from Assets directory.
  /// </summary>
  /// <param name="path">name of the navmesh file with extension included</param>
  /// <returns>DtNavMesh, or null if loading failed</returns>
  public static DtNavMesh? LoadNavMesh(string path)
  {
    try
    {
      ArgumentNullException.ThrowIfNull(path);
      FileStream fs = File.OpenRead(path);
      BinaryReader br = new(fs);

      DtMeshSetReader reader = new();

      DtNavMesh navMesh = reader.Read(br, MyRcConfigConstants.VERTS_PER_POLY);
      if (navMesh == null)
      {
        throw new InvalidOperationException("LoadNavMesh failed to read NavMesh.");
      }

      return navMesh;
    }
    catch (Exception e)
    {
      Console.WriteLine("LoadNavMesh Error: " + e.Message);
      return null;
    }
  }

  public static InputGeomProvider? LoadInputMesh(string path)
  {
    try
    {
      RcObjImporterContext ctx = LoadObjFromFile(path);
      // List<float> vertexPositions
      // List<int> meshFaces
      InputGeomProvider geom = new(ctx);
      return geom;
    }
    catch (Exception e)
    {
      Console.WriteLine("LoadInputMesh Error: " + e.Message);
      return null;
    }
  }

  private static RcObjImporterContext LoadObjFromFile(string path)
  {
    ArgumentNullException.ThrowIfNull(path);
    byte[] navData = File.ReadAllBytes(path);
    return RcObjImporter.LoadContext(navData);
  }

  /// <summary>
  /// Incomplete
  /// </summary>
  /// <param name="ctx"></param>
  /// <returns></returns>
  private static RcPolyMesh ConvertObjToPolyMesh(RcObjImporterContext ctx)
  {
    RcPolyMesh rcPolyMesh = new()
    {
      nverts = ctx.vertexPositions.Count / 3,
      verts = new int[ctx.vertexPositions.Count],
      nvp = 3,
      polys = new int[ctx.meshFaces.Count],
      maxpolys = ctx.meshFaces.Count,
      regs = new int[ctx.meshFaces.Count],
      areas = new int[ctx.meshFaces.Count],
      flags = new int[ctx.meshFaces.Count],
      bmin = new DotRecast.Core.Numerics.RcVec3f(),
      bmax = new DotRecast.Core.Numerics.RcVec3f(),
      cs = 0.3f,
      ch = 0.2f
    };

    for (int i = 0; i < rcPolyMesh.nverts; i++)
    {
      //rcPolyMesh.verts[i * 3] = (int)()
    }

    // temp
    return new RcPolyMesh();
  }

  //DtNavMesh
  //DtNavMeshBuilder
}