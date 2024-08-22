using DotRecast.Detour.Crowd;
using PathfindingDedicatedServer.Src.Monsters;

namespace PathfindingDedicatedServer.Src.Utils
{
  public class CustomAgentUtils
  {
    public static bool CheckTargetAgentValidity(DtCrowdAgent? targetAgent)
    {
      if (targetAgent == null) return false;
      if (targetAgent.option.userData is AgentAdditionalData aad)
      {
        if (aad.IsAgentType(AgentType.BASE) || aad.IsAgentType(AgentType.NONE)) return false;
        else return true;
      }
      return false;
    }
  }
}
