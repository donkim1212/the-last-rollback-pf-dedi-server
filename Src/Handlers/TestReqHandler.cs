using System.Net.Sockets;
using PathfindingDedicatedServer.handlers.abstracts;

namespace PathfindingDedicatedServer.handlers
{
    public class TestReqHandler : PacketHandler
    {
        public override void HandlePacket(NetworkStream stream, byte[] bytes)
        {
            TestRequest testRequest = Deserialize<TestRequest>(bytes);
            Console.WriteLine($"TestIntValue: {testRequest.TestIntValue}");
            Console.WriteLine($"Message: {testRequest.Message}");
        }
    }
}
