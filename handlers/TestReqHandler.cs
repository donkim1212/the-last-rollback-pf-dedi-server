using PathfindingDedicatedServer.handlers.abstracts;

namespace PathfindingDedicatedServer.handlers
{
    public class TestReqHandler : PacketHandler
    {
        public override void HandlePacket(byte[] bytes)
        {
            TestRequest testRequest = Packet.Deserialize<TestRequest>(bytes);
            Console.WriteLine($"TestIntValue: {testRequest.TestIntValue}");
            Console.WriteLine($"Message: {testRequest.Message}");
        }
    }
}
