using System;
using System.Buffers;
using System.IO;
using ProtoBuf;

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

//[ProtoContract]
//public class 