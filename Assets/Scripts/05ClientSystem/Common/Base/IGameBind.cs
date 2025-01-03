using UnityEngine;

namespace  GameClient
{
    public interface IGameBind
    {
        T GetComponent<T>(string name) where T : Component;
        T GetComponentInChildren<T>(string name) where T : Component;
    }
}