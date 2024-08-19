namespace PathfindingDedicatedServer.Src.Data.Abstracts
{
  public class AgentInfo()
  {
    public float Radius { get; set; }
    public float Height { get; set; }
    public float MaxAcc { get; set; }
    public float MaxSpd { get; set; }
    public float CollisionQueryRange { get; set; }
    public float PathOptRange { get; set; }
    public float SeparationWeight { get; set; }
    public int UpdateFlags { get; set; }
    public int ObsAvoidanceType { get; set; }
    public int QueryFilterType { get; set; }
  }

  public class AgentData<T> where T : AgentInfo
  {
    public string Name { get; set; }
    public string Version { get; set; }
    public virtual List<T> Data { get; set; }
  }
}
