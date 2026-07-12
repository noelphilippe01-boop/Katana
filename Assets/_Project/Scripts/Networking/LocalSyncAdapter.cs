namespace Katana.Networking
{
    public class LocalSyncAdapter : INetworkSyncAdapter
    {
        public bool IsOnline => false;

        public void SendCommand(IGameCommand command)
        {
            // Solo mode: commands are executed locally, no network sync needed.
        }
    }
}
