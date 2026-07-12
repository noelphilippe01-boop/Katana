using UnityEngine;

namespace Katana.Networking
{
    public interface INetworkSyncAdapter
    {
        void SendCommand(IGameCommand command);
        bool IsOnline { get; }
    }

    public interface IGameCommand { }

    public struct MoveCommand : IGameCommand
    {
        public Vector3 Destination;
    }

    public struct AttackCommand : IGameCommand
    {
        public GameObject Target;
    }
}
