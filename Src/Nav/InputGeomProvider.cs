using DotRecast.Core;
using DotRecast.Core.Collections;
using DotRecast.Core.Numerics;
using DotRecast.Recast;
using DotRecast.Recast.Geom;

namespace PathfindingDedicatedServer.Nav
{
  public class InputGeomProvider : IInputGeomProvider
  {
    private readonly float[] _vertices;
    private readonly int[] _faces;
    private readonly float[] _normals;
    private readonly RcVec3f _bmin;
    private readonly RcVec3f _bmax;

    private readonly List<RcConvexVolume> _convexVolumes = new();
    private readonly List<RcOffMeshConnection> _offMeshConnections = new();
    private readonly RcTriMesh _mesh;

    public InputGeomProvider(RcObjImporterContext ctx) : this(ctx.vertexPositions, ctx.meshFaces)
    {
    }

    public InputGeomProvider(List<float> vertexPositions, List<int> faces) : this(MapVertices(vertexPositions), MapFaces(faces))
    {
    }

    public InputGeomProvider(float[] vertices, int[] faces)
    {
      _vertices = vertices;
      _faces = faces;
      _normals = new float[faces.Length];
      CalculateNormals();
      _bmin = new RcVec3f(vertices);
      _bmax = new RcVec3f(vertices);
      for (int i = 1; i < vertices.Length / 3; i++)
      {
        _bmin = RcVec3f.Min(_bmin, RcVec.Create(vertices, i * 3));
        _bmax = RcVec3f.Max(_bmax, RcVec.Create(vertices, i * 3));
      }

      _mesh = new(vertices, faces);
    }

    public void AddConvexVolume(RcConvexVolume convexVolume)
    {
      _convexVolumes.Add(convexVolume);
    }

    public void AddOffMeshConnection(RcVec3f start, RcVec3f end, float radius, bool bidir, int area, int flags)
    {
      _offMeshConnections.Add(new RcOffMeshConnection(start, end, radius, bidir, area, flags));
    }

    public IList<RcConvexVolume> ConvexVolumes()
    {
      return _convexVolumes;
    }

    public RcTriMesh GetMesh()
    {
      return _mesh;
    }

    public RcVec3f GetMeshBoundsMax()
    {
      return _bmax;
    }

    public RcVec3f GetMeshBoundsMin()
    {
      return _bmin;
    }

    public List<RcOffMeshConnection> GetOffMeshConnections()
    {
      return _offMeshConnections;
    }

    public IEnumerable<RcTriMesh> Meshes()
    {
      return RcImmutableArray.Create(_mesh);
    }

    public void RemoveOffMeshConnections(Predicate<RcOffMeshConnection> filter)
    {
      _offMeshConnections.RemoveAll(filter); // mark
    }

    public void CalculateNormals()
    {
      for (int i = 0; i < _faces.Length; i += 3)
      {
        RcVec3f v0 = RcVec.Create(_vertices, _faces[i] * 3);
        RcVec3f v1 = RcVec.Create(_vertices, _faces[i + 1] * 3);
        RcVec3f v2 = RcVec.Create(_vertices, _faces[i + 2] * 3);
        RcVec3f e0 = v1 - v0;
        RcVec3f e1 = v2 - v0;

        _normals[i] = e0.Y * e1.Z - e0.Z * e1.Y;
        _normals[i + 1] = e0.Z * e1.X - e0.X * e1.Z;
        _normals[i + 2] = e0.X * e1.Y - e0.Y * e1.X;
        float d = MathF.Sqrt(_normals[i] * _normals[i] + _normals[i + 1] * _normals[i + 1] + _normals[i + 2] * _normals[i + 2]);
        if (d > 0)
        {
          d = 1.0f / d;
          _normals[i] *= d;
          _normals[i + 1] *= d;
          _normals[i + 2] *= d;
        }
      }
    }

    private static float[] MapVertices(List<float> vertexPositions)
    {
      float[] vertices = new float[vertexPositions.Count];
      for (int i = 0; i < vertices.Length; i++)
      {
        vertices[i] = vertexPositions[i];
      }

      return vertices;
    }

    private static int[] MapFaces(List<int> meshFaces)
    {
      int[] faces = new int[meshFaces.Count];
      for (int i = 0; i < faces.Length; i++)
      {
        faces[i] = meshFaces[i];
      }

      return faces;
    }
  }
}
