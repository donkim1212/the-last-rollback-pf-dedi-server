namespace PathfindingDedicatedServer.Src.Utils
{
  public enum AgentFlag
  {
    NONE = 0,
    PLAYER = 1,
    MONSTER = 2,
    STRUCTURE = 4,
    BASE = 8,
  }

  public class CustomAgentUtils
  {
    private static readonly int MOBILE_FLAGS = CalcFlags([AgentFlag.PLAYER, AgentFlag.MONSTER]);
    private static readonly int IMMOBILE_FLAGS = CalcFlags([AgentFlag.STRUCTURE, AgentFlag.BASE]);
    private static readonly int VALID_FLAGS = 0xffff;
    private static readonly int MONSTER_TARGETABLE_FLAGS = CalcFlags([AgentFlag.PLAYER, AgentFlag.STRUCTURE, AgentFlag.BASE]);

    public static int CalcFlags (AgentFlag[] flags)
    {
      int sum = 0;
      foreach (AgentFlag flag in flags)
      {
        sum |= (int)flag;
      }
      return sum;
    }

    //public static bool CheckTargetAgentValidity(DtCrowdAgent? targetAgent)
    //{
    //  if (targetAgent == null) return false;
    //  if (targetAgent.option.userData is AgentAdditionalData aad)
    //  {
    //    if (aad.IsAgentFlag(AgentFlag.BASE) || aad.IsAgentFlag(AgentFlag.NONE)) return false;
    //    else return true;
    //  }
    //  return false;
    //}
    public static bool IsPlayer (AgentFlag flag)
    {
      return flag == AgentFlag.PLAYER;
    }

    public static bool IsMonster (AgentFlag flag)
    {
      return flag == AgentFlag.MONSTER;
    }

    public static bool IsStructure (AgentFlag flag)
    {
      return flag == AgentFlag.STRUCTURE;
    }

    public static bool IsBase (AgentFlag flag)
    {
      return flag == AgentFlag.BASE;
    }

    public static bool IsMobileAgent (AgentFlag flag)
    {
      return ((MOBILE_FLAGS & (int)flag) != 0);
    }

    public static bool IsValidAgent (AgentFlag flag)
    {
      return ((VALID_FLAGS & (int)flag) != 0);
    }

    public static bool IsMonsterTargetable (AgentFlag flag)
    {
      return ((MONSTER_TARGETABLE_FLAGS & (int)flag) != 0);
    }
  }
}
