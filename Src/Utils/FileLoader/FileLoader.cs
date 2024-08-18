using PathfindingDedicatedServer.Src.Constants;

namespace PathfindingDedicatedServer.Src.Utils.FileLoader
{
  public abstract class FileLoader
  {
    public string[] SearchAllFilePaths (string dirPath, string fileExt)
    {
      string[] filePaths;
      filePaths = Directory.GetFiles(dirPath, fileExt);
      return filePaths;
    }

    protected abstract T ReadFile<T> (string filePath);
    public T[] ReadAllFiles<T>(string[] filePaths)
    {
      T[] data = new T[filePaths.Length]; 
      
      for (int i = 0; i < data.Length; i++)
      {
        data[i] = ReadFile<T>(filePaths[i]);
      }

      return data;
    }

    public T[] LoadAllFiles<T>(string dirPath, string fileExt)
    {
      // DO NOT TRY CATCH. Server should not run with missing files.
      return ReadAllFiles<T>(SearchAllFilePaths(dirPath, fileExt));
    }

    public T LoadFileFromAssets<T> (string fileNameWithExt)
    {
      return ReadFile<T>(PathConstants.ASSETS_REL_PATH + fileNameWithExt);
    }
  }
}
