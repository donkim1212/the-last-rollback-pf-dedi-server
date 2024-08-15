namespace PathfindingDedicatedServer.handlers.abstracts
{
    public abstract class PacketHandler
    {
        public abstract void HandlePacket(byte[] bytes);
    }
}
