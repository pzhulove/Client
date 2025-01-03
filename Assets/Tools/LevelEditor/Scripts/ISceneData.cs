using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISceneData
{
    string GetName();

    int GetId();

    string GetPrefabPath();

    float GetCameraLookHeight();

    float GetCameraDistance();

    float GetCameraAngle();

    float GetCameraNearClip();

    float GetCameraFarClip();

    float GetCameraSize();

    Vector2 GetCameraZRange();

    Vector2 GetCameraXRange();

    bool IsCameraPersp();

    Vector3 GetCenterPostionNew();
    Vector3 GetScenePostion();  // 图形坐标（相对于逻辑原点）

    float GetSceneUScale();  // 图形坐标（相对于逻辑原点）

    Vector2 GetGridSize();

    Vector2 GetLogicXSize();

    Vector2 GetLogicZSize();

    Color GetObjectDyeColor();

    Vector3 GetLogicPos();// = Vector3.zero;      // 逻辑原点

    EWeatherMode GetWeatherMode();// = EWeatherMode.None;

    int GetTipsID();

    string GetLightmapsettingsPath();

    DynSceneSetting GetLightmapsettings();

    void SetLightmapsettings(DynSceneSetting setting);

    int GetLogicXmin();

    int GetLogicXmax();

    int GetLogicZmin();

    int GetLogicZmax();

    int GetLogicX();

    int GetLogicZ();

    int GetEntityInfoLength();

    ISceneEntityInfoData GetEntityInfo(int i);

    int GetBlockLayerLength();

    byte[] GetRawBlockLayer();
    byte[] GetRawEventAreaLayer();

    ushort[] GetRawGrassLayer();
    ushort[] GetRawEcosystemLayer();

    ushort GetGrassId(int gridX, int gridY);
    ushort GetEcosystemId(int gridX, int gridY);

    byte GetBlockLayer(int i);
    byte GetEventAreaLayer(int i);

    int GetNpcInfoLength();

    ISceneNPCInfoData GetNpcInfo(int i);


    int GetMonsterInfoLength();

    ISceneMonsterInfoData GetMonsterInfo(int i);

    int GetDecoratorInfoLenth();

    ISceneDecoratorInfoData GetDecoratorInfo(int i);


    int GetDestructibleInfoLength();

    ISceneDestructibleInfoData GetDestructibleInfo(int i);


    int GetRegionInfoLength();

    ISceneRegionInfoData GetRegionInfo(int i);


    int GetTransportDoorLength();

    ISceneTransportDoorData GetTransportDoor(int i);

    ISceneEntityInfoData GetBirthPosition();

    ISceneEntityInfoData GetAirBattleBornPos(int i);

    ISceneEntityInfoData GetHellbirthposition();

    int GetTownDoorLength();

    ISceneTownDoorData GetTownDoor(int i);

    int GetFunctionPrefabLength();

    ISceneFunctionPrefabData GetFunctionPrefab(int i);

    string GetLutTexPath();
    string GetLightmapTexPath();
    Vector4 GetLightmapPosition();
}

public interface ISceneFunctionPrefabData
{
    ISceneEntityInfoData GetEntityInfo();

    FunctionPrefab.FunctionType GetFunctionType();
}


public interface ISceneTownDoorData
{
    ISceneRegionInfoData GetRegionInfo();

    int GetSceneID();

    int GetDoorID();

    Vector3 GetBirthPos();

    Vector3 GetLocalBirthPos();

    int GetTargetSceneID();

    int GetTargetDoorID();

    DoorTargetType GetDoorTargetType();
}

public interface ISceneTransportDoorData
{
    ISceneRegionInfoData GetRegionInfo();

    TransportDoorType GetDoortype();
    bool IsEggDoor();

    // according the id to get the scene path
    int GetNextsceneid();

    string GetTownscenepath();

    // default is top<->buttom left<->right
    // but anything is possible
    TransportDoorType GetNextdoortype();

    VInt3 GetBirthposition();

    void SetBirthposition(VInt3 pos);

    string GetMaterialPath();
}

public interface ISceneRegionInfoData
{
    ISceneEntityInfoData GetEntityInfo();

    DRegionInfo.RegionType GetRegiontype();

    void SetRegiontype(DRegionInfo.RegionType type);

    Vector2 GetRect(); // = Vector2.one;

    float GetRadius();// = 1.0f;

    void SetRadius(float r);

    Quaternion GetRotation();
}

public interface ISceneDestructibleInfoData
{
    ISceneEntityInfoData GetEntityInfo();

    Quaternion GetRotation();

    int GetLevel();

    int GetFlushGroupID();
}

public interface ISceneDecoratorInfoData
{
    ISceneEntityInfoData GetEntityInfo();

    Vector3 GetLocalScale();// = Vector3.one;

    Quaternion GetRotation();
}

public interface ISceneMonsterInfoData
{
    ISceneEntityInfoData GetEntityInfo();

    DMonsterInfo.MonsterSwapType GetSwapType();

    //DMonsterInfo.FaceType GetFaceType();

    bool GetIsFaceLeft();

    int GetSight();

    int GetSwapNum();

    int GetSwapDelay();

    int GetFlushGroupID();

    /// <summary>
    /// 怪物分组ID
    /// </summary>
    /// <returns></returns>
    int GetSubGroupID();

    DMonsterInfo.FlowRegionType GetFlowRegionType();

    ISceneRegionInfoData GetRegionInfo();// = new DRegionInfo();        kkkk

    ISceneDestructibleInfoData GetDestructInfo();// = new DDestructibleInfo();

    void SetMonsterID(int id);

    int GetMonsterID();

    int GetMonsterLevel();

    int GetMonsterTypeID();

    int GetMonsterDiffcute();

    int GetGroupIndex();

    string GetAIActionPath();
    
    string GetAIScenarioPath();

    int GetMonsterInfoTableID();
}

public interface ISceneNPCInfoData
{
    ISceneEntityInfoData GetEntityInfo();

    Quaternion GetRotation();
    Vector2    GetMinFindRange();// = new Vector2(1.0f, 3.0f);
    Vector2    GetMaxFindRange();// = new Vector2(2.5f, 5.0f);

}

public interface ISceneEntityInfoData
{
    int          GetGlobalid();

    int          GetResid();

    string       GetName();

    string       GetPath();

    string       GetDescription();

    DEntityType  GetType();

    string       GetTypename();

    Vector3      GetPosition();

    float        GetScale();

    Color        GetColor();

    string       GetModelPathByResID();
}


