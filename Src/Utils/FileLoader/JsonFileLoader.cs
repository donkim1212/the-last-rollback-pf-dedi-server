using System.Data;
using System.Text.Json;

namespace PathfindingDedicatedServer.Src.Utils.FileLoader
{
  public class JsonFileLoader : FileLoader
  {
    private readonly JsonSerializerOptions option = new ()
    {
      AllowTrailingCommas = true,
      MaxDepth = 64,
      PropertyNameCaseInsensitive = true,
      PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
      //IncludeFields = true,
    };

    protected override T ReadFile<T>(string filePath)
    {
      //FileStream fs = File.OpenRead(filePath);
      //StreamReader reader = new(fs);
      string text = File.ReadAllText(filePath);
      T? ret = JsonSerializer.Deserialize<T>(text, option);
      if (ret == null)
      {
        throw new NoNullAllowedException($"Failed to deserialize JSON file: {filePath}");
      }
      return ret;
    }
  }
}
