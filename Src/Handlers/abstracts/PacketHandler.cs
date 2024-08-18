using System.Net.Sockets;
namespace PathfindingDedicatedServer.handlers.abstracts
{
    public abstract class PacketHandler
    {
        public abstract void HandlePacket(NetworkStream stream, byte[] bytes);
        public T Deserialize<T>(byte[] bytes)
        {
          T packet = Packet.Deserialize<T>(bytes);
          return packet;
        }
    }
}
