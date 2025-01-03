using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDungeonData
{
    string GetName();

    void SetName(string name);

    int GetHeight();

    int GetWeidth();

    int GetStartIndex();

    int GetAreaConnectListLength();

    IDungeonConnectData GetAreaConnectList(int i);

    IDungeonConnectData GetSideByType(int idx, TransportDoorType fromtype);

    void GetSideByType(IDungeonConnectData condata, TransportDoorType fromtype, out int index);

    void GetSideByType(int x, int y, TransportDoorType fromtype, out int index);

    int GetConnectStatus(IDungeonConnectData from, IDungeonConnectData to);

    IDungeonConnectData GetSideByType(IDungeonConnectData condata, TransportDoorType fromtype);
}
