namespace PathfindingDedicatedServer.Src.Utils.FileLoader
{
  public abstract class FileLoader
  {
    public string[] LoadAllFilePaths (string dirPath, string fileExt)
    {
      string[] filePaths;
      filePaths = Directory.GetFiles(dirPath, fileExt);
      return filePaths;
    }

    public abstract void ReadFile (string filePath);
  }
}
