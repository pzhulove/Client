using UnityEngine;
using System.Collections;
using Network;
using Protocol;

namespace GameClient
{
    public interface IClientNet
    {
        void OnDisconnect(ServerType type);

        void OnReconnect();

        void OnReconnectError();
    }
}
