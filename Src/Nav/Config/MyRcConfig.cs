using DotRecast.Recast;

namespace PathfindingDedicatedServer.Nav.Config
{
    public class MyRcConfig
    {
        public const RcPartition RC_PARTITION_TYPE = RcPartition.WATERSHED;
        public const float CELL_SIZE = 0.3f;
        public const float CELL_HEIGHT = 0.2f;
        public const float AGENT_MAX_SLOPE = 20f;
        public const float AGENT_HEIGHT = 1.0f;
        public const float AGENT_RADIUS = 0.0f;
        public const float AGENT_MAX_CLIMB = 2.0f;
        public const int REGION_MIN_SIZE = 60;
        public const int REGION_MERGE_SIZE = 20;
        public const float EDGE_MAX_LENGTH = 12.5f;
        public const float EDGE_MAX_ERROR = 1.6f;
        public const int VERTS_PER_POLY = 6;
        public const float DETAIL_SAMPLE_DIST = 6.0f;
        public const float DETAIL_SAMPLE_MAX_ERROR = 2.0f;
        public const bool FILTER_LOW_HANGING_OBSTACLES = true;
        public const bool FILTER_LEDGE_SPANS = true;
        public const bool FILTER_WALKABLE_LOW_HEIGHT_SPANS = true;
        public const int AREA_ID = RcRecast.RC_WALKABLE_AREA;
        public static readonly RcAreaModification RC_AREA_MODIFICATION = new(AREA_ID);
        public const bool BUILD_MESH_DETAIL = false;

        private RcConfig _config;

        public MyRcConfig()
        {
            _config = new(
                RC_PARTITION_TYPE,
                CELL_SIZE, CELL_HEIGHT,
                AGENT_MAX_SLOPE, AGENT_HEIGHT, AGENT_RADIUS, AGENT_MAX_CLIMB,
                REGION_MIN_SIZE, REGION_MERGE_SIZE,
                EDGE_MAX_LENGTH, EDGE_MAX_ERROR,
                VERTS_PER_POLY,
                DETAIL_SAMPLE_DIST, DETAIL_SAMPLE_MAX_ERROR,
                FILTER_LOW_HANGING_OBSTACLES, FILTER_LEDGE_SPANS, FILTER_WALKABLE_LOW_HEIGHT_SPANS,
                RC_AREA_MODIFICATION, BUILD_MESH_DETAIL
            );

            //RcMeshs.BuildPolyMesh(new RcContext(), );
        }
    }
}
