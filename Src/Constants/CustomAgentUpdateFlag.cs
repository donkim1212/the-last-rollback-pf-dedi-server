namespace PathfindingDedicatedServer.Src.Constants
{
  public enum CustomAgentUpdateFlag
  {
    NONE = 0,
    ANTICIPATED_TURNS = 1,
    COLLISION_AVOIDANCE = 2,
    CAUTIOUS_NAVIGATOR = 3,

    REACTIVE_RUNNER = 6,
    CHARGER = 7,
    DUMB_CHARGER = 8,
    VISIONARY_MOVER = 9,

    STRATEGIC_AVOIDER = 18,

    DEFAULT = 24,

    EFFICIENT_MOVER = 26,
    PLAN_AHEAD = 27,

    ALL_IN_ONE = 31
  }
}
