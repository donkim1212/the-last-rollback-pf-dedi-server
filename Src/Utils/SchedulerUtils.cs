namespace PathfindingDedicatedServer.Src.Utils
{
  public class SchedulerUtils
  {
    /// <summary>
    /// Sets a non-blocking async timed action.
    /// </summary>
    /// <param name="ms">waiting time in millis</param>
    /// <param name="callback">callback function to run</param>
    public static void SetTimedAction(int ms, Action callback)
    {
      _ = Task.Run(async () =>
      {
        await Task.Delay(ms);
        callback();
      });
    }

    public static void SetIntervalAction(int ms, Action callback)
    {
      _ = Task.Run(async () =>
      {
        await Task.Delay(ms);
        callback();
        SetIntervalAction(ms, callback);
      });
    }

    public static void SetIntervalAction(int ms, int count, Action callback)
    {
      if (count == 0) return;
      _ = Task.Run(async () =>
      {
        await Task.Delay(ms);
        callback();
        SetIntervalAction(ms, count - 1, callback);
      });
    }
  }
}
