namespace PathfindingDedicatedServer.Src.Data.Abstracts
{
  public class JsonData<T>
  {
    public string Name { get; set; }
    public string Version { get; set; }
    public virtual List<T> Data { get; set; }
  }
}
