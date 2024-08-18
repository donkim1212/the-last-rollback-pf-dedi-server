using ProtoBuf;
using System.Buffers;

public class Packet
{
  public enum PacketType
  {
    None = 0,
    TestRequest = 1,
    TestResponse = 2,
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

[ProtoContract]
public class TestRequest
{
  [ProtoMember(1)]
  public uint TestIntValue { get; set; }
  public string? Message { get; set; }
}

[ProtoContract]
public class TestResponse
{
  [ProtoMember(1)]
  public string? TestStringValue { get; set; }
}

// ----- IN -----

[ProtoContract]
public class C_SetPlayers
{
  [ProtoMember(1)] // accountId : charClass
  public Dictionary<string, uint> players { get; set; }
}

[ProtoContract]
public class C_SetMonsters
{
  [ProtoMember(1)] // monsterIdx : monsterModel
  public Dictionary<uint, uint> monsters { get; set; }
}

[ProtoContract]
public class C_SetPlayerDest
{
  [ProtoMember(1)]
  public string accountId { get; set; }
  [ProtoMember(2)]
  public WorldPosition? Pos { get; set; }
}

[ProtoContract]
public class C_SetMonsterDest
{
  [ProtoMember(1)]
  public uint monsterIdx { get; set; }
  [ProtoMember(2)]
  public WorldPosition? Pos { get; set; }
}

// ----- OUT -----

[ProtoContract]
public class S_MonstersLocationUpdate
{
  [ProtoMember(1)] // monsterIdx : Vector3 (X,Y,Z)
  public Dictionary<uint, WorldPosition> MonsterPositions { get; set; }
}

[ProtoContract]
public class S_PlayerLocationUpdate
{
  [ProtoMember(1)] // accountId : Vector3 (X,Y,Z)
  public Dictionary<string, WorldPosition> PlayerPositions { get; set; }
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

