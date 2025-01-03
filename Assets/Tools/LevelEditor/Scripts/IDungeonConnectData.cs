using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDungeonConnectData
{
    void SetSceneData(ISceneData sceneData);

    ISceneData GetSceneData();

    int GetLinkAreaIndexesLength();

    int GetLinkAreaIndex(int i);

    int GetIsConnectLength();

    bool GetIsConnect(int i);

    int GetAreaIndex();

    int GetId();

    string GetSceneAreaPath();

    void SetSceneAreaPath(string path);

    int GetPositionX();

    int GetPositionY();

    bool IsBoss();

    bool IsStart();

    bool IsEgg();

    bool IsNotHell();
    byte GetTreasureType();
}
