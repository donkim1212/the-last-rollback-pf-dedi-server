using System;
using DotRecast.Core;
using DotRecast.Detour;
using DotRecast.Detour.Io;
using DotRecast.Recast;

namespace PathfindingDedicatedServer.Src.Nav;
public class NavMeshLoader
{
    private const RcPartition RC_PARTITION_TYPE = RcPartition.WATERSHED;
    private const float CELL_SIZE = 0.3f;
    private const float CELL_HEIGHT = 0.2f;
    private const float AGENT_MAX_SLOPE = 20f;
    private const float AGENT_HEIGHT = 1.0f;
    private const float AGENT_RADIUS = 0.0f;
    private const float AGENT_MAX_CLIMB = 2.0f;
    private const int REGION_MIN_SIZE = 60;
    private const int REGION_MERGE_SIZE = 20;
    private const float EDGE_MAX_LENGTH = 12.5f;
    private const float EDGE_MAX_ERROR = 1.6f;
    private const int VERTS_PER_POLY = 6;
    private const float DETAIL_SAMPLE_DIST = 6.0f;
    private const float DETAIL_SAMPLE_MAX_ERROR = 2.0f;
    private const bool FILTER_LOW_HANGING_OBSTACLES = true;
    private const bool FILTER_LEDGE_SPANS = true;
    private const bool FILTER_WALKABLE_LOW_HEIGHT_SPANS = true;
    private const int AREA_ID = RcRecast.RC_WALKABLE_AREA;
    private static readonly RcAreaModification RC_AREA_MODIFICATION = new(AREA_ID);
    private const bool BUILD_MESH_DETAIL = false;

    private const string ASSETS_REL_PATH_PREFIX = "./Assets/";

    /// <summary>
    /// Loads NavMesh from Assets directory.
    /// </summary>
    /// <param name="filename">name of the navmesh file with extension included</param>
    /// <returns>DtNavMesh, or null if loading failed</returns>
    public static DtNavMesh? LoadNavMesh(string filename)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(filename);
            FileStream fs = File.OpenRead(ASSETS_REL_PATH_PREFIX + filename);
            BinaryReader br = new(fs);

            DtMeshSetReader reader = new();

            DtNavMesh navMesh = reader.Read(br, VERTS_PER_POLY);
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

    public static InputGeomProvider? LoadInputMesh(string filename)
    {
        try
        {
            RcObjImporterContext ctx = LoadObjFromFile(ASSETS_REL_PATH_PREFIX + filename);
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

        //RcConfig config = new(
        //    RC_PARTITION_TYPE,
        //    CELL_SIZE, CELL_HEIGHT,
        //    AGENT_MAX_SLOPE, AGENT_HEIGHT, AGENT_RADIUS, AGENT_MAX_CLIMB,
        //    REGION_MIN_SIZE, REGION_MERGE_SIZE,
        //    EDGE_MAX_LENGTH, EDGE_MAX_ERROR,
        //    VERTS_PER_POLY,
        //    DETAIL_SAMPLE_DIST, DETAIL_SAMPLE_MAX_ERROR,
        //    FILTER_LOW_HANGING_OBSTACLES, FILTER_LEDGE_SPANS, FILTER_WALKABLE_LOW_HEIGHT_SPANS,
        //    RC_AREA_MODIFICATION, BUILD_MESH_DETAIL
        //);

        //RcMeshs.BuildPolyMesh(new RcContext(), );
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