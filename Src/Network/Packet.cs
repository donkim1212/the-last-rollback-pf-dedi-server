using ProtoBuf;
using System.Buffers;

public class Packet
{
  public enum PacketType
  {
    None = 0,

    C_CreateSession = 10,
    
    C_SetPlayers = 11,
    C_SetMonsters = 12,
    C_SetPlayerDest = 13,
    C_SetMonsterDest = 14,

    S_PlayerLocationUpdate = 31,
    S_MonstersLocationUpdate = 32,

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
public class C_SetPlayers
{
  [ProtoMember(1)] // accountId : charClass
  public Dictionary<string, uint> Players { get; set; }
}

[ProtoContract]
public class C_SetMonsters
{
  [ProtoMember(1)] // monsterIdx : monsterModel
  public Dictionary<uint, uint> Monsters { get; set; }
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

// ----- OUT -----

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
  public uint StructureIdx { get; set; }
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

