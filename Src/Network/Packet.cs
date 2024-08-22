using ProtoBuf;
using System.Buffers;
using System.Security.Cryptography.X509Certificates;

#pragma warning disable IDE1006, CA1050
public class Packet
{
  public enum PacketType
  {
    None = 0,

    C_NightRoundStart = 5,

    C_CreateSession = 10,
    
    C_SetPlayers = 11,
    C_SetMonsters = 12,
    C_SetPlayerDest = 13,
    C_SetMonsterDest = 14,
    C_AddStructure = 15,
    C_RemoveStructure = 16,

    S_PlayerLocationUpdate = 31,
    S_MonstersLocationUpdate = 32,

    C_KillMonster = 40,
  }

  public static void Serialize<T>(IBufferWriter<byte> writer, T data)
  {
    Serializer.Serialize(writer, data);
  }

  public static T Deserialize<T>(byte[] data)
  {
    try
    {
      using var stream = new MemoryStream(data);
      return Serializer.Deserialize<T>(stream);
    }
    catch (Exception e)
    {
      Console.WriteLine("Deserialize failed:" + e.Message);
      throw;
    }
  }
}

// ----- IN -----

[ProtoContract]
public class C_CreateSession
{
  [ProtoMember(1)]
  public uint DungeonCode { get; set; }
}

[ProtoContract]
public class C_StartSession
{
  [ProtoMember(1)]
  public long Timestamp { get; set; }
}

[ProtoContract]
public class C_AddStructure
{
  [ProtoMember(1)]
  public WorldPosition WorldPosition { get; set; }
  [ProtoMember(2)]
  public Structure Structure { get; set; }
}

[ProtoContract]
public class C_RemoveStructure
{
  [ProtoMember(1)]
  public int StructureIdx { get; set; }
}

[ProtoContract]
public class C_SetPlayers
{
  [ProtoMember(1)]
  public List<Player> Players { get; set; } = [];
}

[ProtoContract]
public class C_SetMonsters
{
  [ProtoMember(1)]
  public List<Monster> Monsters { get; set; } = [];
}

[ProtoContract]
public class C_SetPlayerDest
{
  [ProtoMember(1)]
  public string AccountId { get; set; }
  [ProtoMember(2)]
  public WorldPosition? Pos { get; set; }
}

[ProtoContract]
public class C_SetMonsterDest
{
  [ProtoMember(1)]
  public uint MonsterIdx { get; set; }
  [ProtoMember(2)]
  public Target? Target { get; set; }
}

[ProtoContract]
public class C_NightRoundStart
{
  [ProtoMember(1)]
  public ulong Timestamp { get; set; }
}

[ProtoContract]
public class C_KillMonster
{
    [ProtoMember(1)]
    public uint MonsterIdx { get; set; }
}

// ----- OUT -----

//[ProtoContract]
//public class S_CreateSession
//{
//  [ProtoMember(1)]
//  public long Timestamp { get; set; }
//}

[ProtoContract]
public class S_PlayersLocationUpdate
{
  [ProtoMember(1)] // accountId : Vector3 (X,Y,Z)
  public Dictionary<string, WorldPosition> Positions { get; set; }
}

[ProtoContract]
public class S_MonstersLocationUpdate
{
  [ProtoMember(1)] // monsterIdx : Vector3 (X,Y,Z)
  public Dictionary<uint, WorldPosition> Positions { get; set; }
}

[ProtoContract]
public class S_MonsterAttack
{
  [ProtoMember(1)]
  public uint MonsterIdx { get; set; }
  [ProtoMember(2)]
  public Target? Target { get; set; }
  [ProtoMember(3)]
  public float Rotation { get; set; }
}

// // ----- ETC -----

[ProtoContract]
public class WorldPosition
{
  [ProtoMember(1)]
  public float X { get; set; }
  [ProtoMember(2)]
  public float Y { get; set; }
  [ProtoMember(3)]
  public float Z { get; set; }
}

[ProtoContract]
public class Structure
{
  [ProtoMember(1)]
  public int StructureIdx { get; set; }
  [ProtoMember(2)]
  public uint StructureModel { get; set; }
}

[ProtoContract]
public class Player
{
  [ProtoMember(1)]
  public string AccountId { get; set; }
  [ProtoMember(2)]
  public uint CharClass { get; set; }
}

[ProtoContract]
public class Monster
{
  [ProtoMember(1)]
  public uint MonsterIdx { get; set; }
  [ProtoMember(2)]
  public uint MonsterModel { get; set; }
}

[ProtoContract]
[ProtoInclude(1, typeof(TargetPlayer))]
[ProtoInclude(2, typeof(TargetStructure))]
public class Target
{
}

[ProtoContract]
public class TargetPlayer : Target
{
  [ProtoMember(1)]
  public string AccountId { get; set; }
}

[ProtoContract]
public class TargetStructure : Target
{
  [ProtoMember(1)]
  public int StructureIdx { get; set; }
}

//[ProtoContract]
//public class PlayerInfo
//{
//  [ProtoMember(1)]
//  public string AccountId { get; set; }
//  [ProtoMember(2)]
//  public uint CharClass { get; set; }
//}

//[ProtoContract]
//public class MonsterInfo
//{
//  [ProtoMember(1)]
//  public uint MonsterIdx { get; set; }
//  [ProtoMember(2)]
//  public uint MonsterModel { get; set; }
//}

