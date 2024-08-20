using PathfindingDedicatedServer.Src.Data.Abstracts;

namespace PathfindingDedicatedServer.Src.Data
{
  public enum PosType
  {
    PLAYER_SPAWNER = 0,
    MONSTER_SPAWNER = 1,
    BASE_SPAWNER = 5
  }

  public class DungeonInfo
  {
    public uint Id { get; set; }
    public uint DungeonCode { get; set; }
    public PosType PosType { get; set; }
    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }
  }

  public class DungeonData : JsonData<DungeonInfo>
  {
  }
}
