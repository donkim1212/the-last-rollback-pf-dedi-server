namespace PathfindingDedicatedServer.Src.Constants
{
  internal class ServerConstants
  {
    /* using */
    public const string HOST = "127.0.0.1";
    public const int PORT = 5507;
    public static readonly int RANDOM_SEED = DateTime.UtcNow.Millisecond;
    public const int TICK_RATE = 50; // ms

    /* unused */
    public const string VERSION = "1.0.0";
    public const int MAX_CONNECTIONS = 10; // needs testing
  }
}
