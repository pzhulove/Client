using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleDungeonConnectData : IDungeonConnectData
{
    FBDungeonData.DSceneDataConnect mData;

    private ISceneData mSceneData;
    private string mSceneAreaPath;

    public BattleDungeonConnectData(FBDungeonData.DSceneDataConnect data)
    {
        mData = data;
        mSceneData = null;

        mSceneAreaPath = mData.Sceneareapath;
    }


    public int GetAreaIndex()
    {
        return mData.Areaindex;
    }

    public int GetId()
    {
        return mData.Id;
    }

    public bool GetIsConnect(int i)
    {
        return mData.GetIsconnect(i);
    }

    public int GetIsConnectLength()
    {
        return mData.IsconnectLength;
    }

    public int GetPositionX()
    {
        return mData.Positionx;
    }

    public int GetPositionY()
    {
        return mData.Positiony;
    }

    public string GetSceneAreaPath()
    {
        return mSceneAreaPath;
    }

    public void SetSceneAreaPath(string path)
    {
        mSceneAreaPath = path;
    }

    public ISceneData GetSceneData()
    {
        return mSceneData;
    }

    public bool IsBoss()
    {
        return mData.Isboss;
    }

    public bool IsEgg()
    {
        return mData.Isegg;
    }

    public bool IsNotHell()
    {
        return mData.Isnothell;
    }

    public bool IsStart()
    {
        return mData.Isstart;
    }

    public byte GetTreasureType()
    {
        return mData.TreasureType;
    }

    public void SetSceneData(ISceneData sceneData)
    {
        mSceneData = sceneData;
    }

    public int GetLinkAreaIndexesLength()
    {
        return mData.LinkAreaIndexLength;
    }

    public int GetLinkAreaIndex(int i)
    {
        return mData.GetLinkAreaIndex(i);
    }
}

public class BattleDungeonData : IDungeonData
{
    private FBDungeonData.DDungeonData mData;

    private string mName;
    private List<IDungeonConnectData> mAreaConnect = new List<IDungeonConnectData>();

    public BattleDungeonData(FBDungeonData.DDungeonData data)
    {
        mData = data;
        mName = mData.Name;
        _initConnectData();
    }

    private void _initConnectData()
    {
        for (int i = 0; i < mData.AreaconnectlistLength; ++i)
        {
            mAreaConnect.Add(new BattleDungeonConnectData(mData.GetAreaconnectlist(i)));
        }
    }

    public IDungeonConnectData GetAreaConnectList(int i)
    {
        if (null == mAreaConnect) return null;
        if (0 > i) return null;
        if (mAreaConnect.Count <= i) return null;

        return mAreaConnect[i];
    }

    public int GetAreaConnectListLength()
    {
        return mData.AreaconnectlistLength;
    }

    public int GetConnectStatus(IDungeonConnectData from, IDungeonConnectData to)
    {
        if (from == null || to == null)
        {
            return -1;
        }

        for (int i = 0; i < from.GetIsConnectLength(); ++i)
        {
            if (from.GetIsConnect(i))
            {
                IDungeonConnectData con = GetSideByType(from, (TransportDoorType)i);

                if (con.GetAreaIndex() == to.GetAreaIndex() &&
                    con.GetPositionX() == to.GetPositionX() &&
                    con.GetPositionY() == to.GetPositionY())
                {
                    return i;
                }
            }
        }

        return -1;
    }

    public int GetHeight()
    {
        return mData.Height;
    }

    public string GetName()
    {
        return mName;
    }

    public IDungeonConnectData GetSideByType(IDungeonConnectData condata, TransportDoorType fromtype)
    {
        int x = condata.GetPositionX();
        int y = condata.GetPositionY();

        switch (fromtype)
        {
            case TransportDoorType.Buttom: ++y; break;
            case TransportDoorType.Top: --y; break;
            case TransportDoorType.Right: ++x; break;
            case TransportDoorType.Left: --x; break;
        }

        for (int i = 0; i < GetAreaConnectListLength(); ++i)
        {
            IDungeonConnectData item = GetAreaConnectList(i);

            if (item.GetPositionX() == x &&
                item.GetPositionY() == y)
            {
                return item;
            }

        }

        return null;
    }

    public IDungeonConnectData GetSideByType(int idx, TransportDoorType fromtype)
    {
        if (idx < 0 || idx >= GetAreaConnectListLength())
        {
            Logger.LogError("areaidx out of range");
            return null;
        }
        return GetSideByType(GetAreaConnectList(idx), fromtype);
    }

    public void GetSideByType(IDungeonConnectData condata, TransportDoorType fromtype, out int index)
    {
        index = -1;

        int x = condata.GetPositionX();
        int y = condata.GetPositionY();

        switch (fromtype)
        {
            case TransportDoorType.Buttom: ++y; break;
            case TransportDoorType.Top: --y; break;
            case TransportDoorType.Right: ++x; break;
            case TransportDoorType.Left: --x; break;
        }

        for (int i = 0; i < GetAreaConnectListLength(); ++i)
        {
            var item = GetAreaConnectList(i);
            if (item.GetPositionX() == x && item.GetPositionY() == y)
            {
                index = i;
                return;
            }
        }
    }

    public void GetSideByType(int x, int y, TransportDoorType fromtype, out int index)
    {
        index = -1;

        switch (fromtype)
        {
            case TransportDoorType.Buttom: ++y; break;
            case TransportDoorType.Top: --y; break;
            case TransportDoorType.Right: ++x; break;
            case TransportDoorType.Left: --x; break;
        }

        for (int i = 0; i < GetAreaConnectListLength(); ++i)
        {
            IDungeonConnectData item = GetAreaConnectList(i);

            if (item.GetPositionX() == x && item.GetPositionY() == y)
            {
                index = i;
                return;
            }
        }
    }

    public int GetStartIndex()
    {
        return mData.Startindex;
    }

    public int GetWeidth()
    {
        return mData.Weidth;
    }

    public void SetName(string name)
    {
        mName = name;
    }
}



public class SceneEntityInfo : ISceneEntityInfoData
{
    private FBSceneData.DEntityInfo mData;

    public SceneEntityInfo(FBSceneData.DEntityInfo data)
    {
        mData = data;
    }

    public Color GetColor()
    {
        return new Color(mData.Color.R, mData.Color.G, mData.Color.B, mData.Color.A);
    }

    public string GetDescription()
    {
        return mData.Description;
    }

    public int GetGlobalid()
    {
        return mData.Globalid;
    }

    public virtual string GetModelPathByResID()
    {
        var res = TableManager.GetInstance().GetTableItem<ProtoTable.ResTable>(GetResid());
        if (res == null)
        {
            return "";
        }

        return res.ModelPath;
    }

    public string GetName()
    {
        return mData.Name;
    }

    public string GetPath()
    {
        return mData.Path;
    }

    public Vector3 GetPosition()
    {
        return new Vector3(mData.Position.X, mData.Position.Y, mData.Position.Z);
    }

    public int GetResid()
    {
        return mData.Resid;
    }

    public float GetScale()
    {
        return mData.Scale;
    }

    public string GetTypename()
    {
        return mData.TypeName;
    }

    DEntityType ISceneEntityInfoData.GetType()
    {
        return (DEntityType)mData.Type;
    }
}

public class SceneDecoratorInfo : SceneEntityInfo, ISceneDecoratorInfoData
{
    private FBSceneData.DDecoratorInfo mData;

    public SceneDecoratorInfo(FBSceneData.DDecoratorInfo data) : base(data.Super)
    {
        mData = data;
    }

    public ISceneEntityInfoData GetEntityInfo()
    {
        return this;
    }

    public Vector3 GetLocalScale()
    {
        return new Vector3(
            mData.LocalScale.X,
            mData.LocalScale.Y,
            mData.LocalScale.Z
        );
    }

    public Quaternion GetRotation()
    {
        return new Quaternion(
            mData.Rotation.X,
            mData.Rotation.Y,
            mData.Rotation.Z,
            mData.Rotation.W
        );
    }
}

public class SceneTransportDoor : SceneRegionInfo, ISceneTransportDoorData
{
    private FBSceneData.DTransportDoor mData;
    private VInt3 mBirthPosition;

    public SceneTransportDoor(FBSceneData.DTransportDoor data) : base(data.Super)
    {
        mData = data;
        mBirthPosition = new VInt3( mData.Birthposition.X, mData.Birthposition.Y, mData.Birthposition.Z );
    }

    public VInt3 GetBirthposition()
    {
        return mBirthPosition;
    }

    public TransportDoorType GetDoortype()
    {
        return (TransportDoorType)mData.Doortype;
    }

    public TransportDoorType GetNextdoortype()
    {
        return (TransportDoorType)mData.Nextdoortype;
    }

    public int GetNextsceneid()
    {
        return mData.Nextsceneid;
    }

    public ISceneRegionInfoData GetRegionInfo()
    {
        return this;
    }

    public string GetTownscenepath()
    {
        return mData.Townscenepath;
    }

    public void SetBirthposition(VInt3 pos)
    {
        mBirthPosition = pos;
    }
    public bool IsEggDoor()
    {
        return mData.Iseggdoor;
    }
    public string GetMaterialPath()
    {
        return "";
    }

}

public class SceneDestructibleInfo : SceneEntityInfo, ISceneDestructibleInfoData
{
    private FBSceneData.DDestructibleInfo mData;
    public SceneDestructibleInfo(FBSceneData.DDestructibleInfo data) : base(data.Super)
    {
        mData = data;
    }

    public override string GetModelPathByResID()
    {
        var destruc = TableManager.GetInstance().GetTableItem<ProtoTable.DestrucTable>(GetResid());
        if (null == destruc)
        {
            return string.Empty;
        }

        var res = TableManager.GetInstance().GetTableItem<ProtoTable.ResTable>(destruc.Mode);
        if (res == null)
        {
            return string.Empty;
        }

        return res.ModelPath;
    }

    public ISceneEntityInfoData GetEntityInfo()
    {
        return this;
    }

    public int GetFlushGroupID()
    {
        return mData.FlushGroupID;
    }

    public int GetLevel()
    {
        return mData.Level;
    }

    public Quaternion GetRotation()
    {
        return new Quaternion(
            mData.Rotation.X,
            mData.Rotation.Y,
            mData.Rotation.Z,
            mData.Rotation.W
            );
    }
}

public class SceneFunctionPrefab : SceneEntityInfo, ISceneFunctionPrefabData
{
    private FBSceneData.FunctionPrefab mData;

    public SceneFunctionPrefab(FBSceneData.FunctionPrefab data) : base(data.Super)
    {
        mData = data;
    }

    public ISceneEntityInfoData GetEntityInfo()
    {
        return this;
    }

    public FunctionPrefab.FunctionType GetFunctionType()
    {
        return (FunctionPrefab.FunctionType)mData.EFunctionType;
    }
}

public class SceneTransferInfo : SceneEntityInfo
{
    private FBSceneData.DTransferInfo mData;

    public SceneTransferInfo(FBSceneData.DTransferInfo data) : base(data.Super)
    {
        mData = data;
    }
}

public class SceneMonsterInfo : SceneEntityInfo, ISceneMonsterInfoData
{
    private FBSceneData.DMonsterInfo mData;
    private ISceneDestructibleInfoData mDestructInfo;
    private ISceneRegionInfoData mRegionInfo;

    private MonsterID mMonsterID;

    public override string GetModelPathByResID()
    {
        var unit = TableManager.GetInstance().GetTableItem<ProtoTable.UnitTable>(this.GetResid());
        if (unit == null)
        {
            return "";
        }

        var res = TableManager.GetInstance().GetTableItem<ProtoTable.ResTable>(unit.Mode);
        if (res == null)
        {
            return "";
        }

        return res.ModelPath;
    }

    public SceneMonsterInfo(FBSceneData.DMonsterInfo data) : base(data.Super)
    {
        mData = data;
        mDestructInfo = new SceneDestructibleInfo(data.DestructInfo);
        mMonsterID = new MonsterID();
        mMonsterID.resID = mData.Super.Resid;
    }

    public ISceneDestructibleInfoData GetDestructInfo()
    {
        return mDestructInfo;
    }

    public ISceneEntityInfoData GetEntityInfo()
    {
        return this;
    }

    /*public DMonsterInfo.FaceType GetFaceType()
    {
        return (DMonsterInfo.FaceType)mData.FaceType;
    }*/

    public bool GetIsFaceLeft()
    {
        return mData.IsFaceLeft;
    }

    public int GetSight()
    {
        return mData.Sight;
    }

    public DMonsterInfo.FlowRegionType GetFlowRegionType()
    {
        return (DMonsterInfo.FlowRegionType)mData.FlowRegionType;
    }

    public int GetFlushGroupID()
    {
        return mData.FlushGroupID;
    }

    public int GetSubGroupID()
    {
        return mData.SubGroupID;
    }

    public int GetMonsterDiffcute()
    {
        return mMonsterID.monsterDiffcute;
    }

    public int GetMonsterID()
    {
        return mMonsterID.monsterID;
    }

    public int GetMonsterLevel()
    {
        return mMonsterID.monsterLevel;
    }

    public int GetMonsterTypeID()
    {
        return mMonsterID.monsterID;
    }

    public ISceneRegionInfoData GetRegionInfo()
    {
        return mRegionInfo;
    }

    public int GetSwapDelay()
    {
        return mData.SwapDelay;
    }

    public int GetSwapNum()
    {
        return mData.SwapNum;
    }

    public DMonsterInfo.MonsterSwapType GetSwapType()
    {
        return (DMonsterInfo.MonsterSwapType)mData.SwapType;
    }

    public void SetMonsterID(int id)
    {
        mMonsterID.resID = id;
    }

    public int GetGroupIndex()
    {
        return mData.GroupIndex;
    }

    public string GetAIActionPath()
    {
        return mData.AiActionPath;
    }

    public string GetAIScenarioPath()
    {
        return mData.AiScenarioPath;
    }
    
    public int GetMonsterInfoTableID()
    {
        return mData.MonsterInfoTableID;
    }
}

public class SceneNPCInfo : SceneEntityInfo, ISceneNPCInfoData
{
    private FBSceneData.DNPCInfo mData;

    public SceneNPCInfo(FBSceneData.DNPCInfo data) : base(data.Super)
    {
        mData = data;
    }

    public ISceneEntityInfoData GetEntityInfo()
    {
        return this;
    }

    public Vector2 GetMaxFindRange()
    {
        return new Vector2(
            mData.MaxFindRange.X,
            mData.MaxFindRange.Y
            );
    }

    public Vector2 GetMinFindRange()
    {
        return new Vector2(
            mData.MinFindRange.X,
            mData.MinFindRange.Y
            );
    }

    public Quaternion GetRotation()
    {
        return new Quaternion(
            mData.Rotation.X,
            mData.Rotation.Y,
            mData.Rotation.Z,
            mData.Rotation.W
            );
    }
}

public class SceneRegionInfo : SceneEntityInfo, ISceneRegionInfoData
{
    private FBSceneData.DRegionInfo mData;
    private float mRadius = 0.0f;
    private DRegionInfo.RegionType mRegionType;

    public override string GetModelPathByResID()
    {
        var region = TableManager.GetInstance().GetTableItem<ProtoTable.SceneRegionTable>(GetResid());
        if (region == null)
        {
            return "";
        }

        var res = TableManager.GetInstance().GetTableItem<ProtoTable.ResTable>(region.ResID);
        if (res == null)
        {
            return "";
        }

        return res.ModelPath;
    }

    public SceneRegionInfo(FBSceneData.DRegionInfo data) : base(data.Super)
    {
        mData       = data;
        mRadius     = mData.Radius;
        mRegionType = (DRegionInfo.RegionType)mData.Regiontype;
    }

    public ISceneEntityInfoData GetEntityInfo()
    {
        return this;
    }

    public float GetRadius()
    {
        return mRadius;
    }

    public Vector2 GetRect()
    {
        return new Vector2(
            mData.Rect.X,
            mData.Rect.Y
            );
    }

    public DRegionInfo.RegionType GetRegiontype()
    {
        return mRegionType;
    }

    public Quaternion GetRotation()
    {
        return new Quaternion(
            mData.Rotation.X,
            mData.Rotation.Y,
            mData.Rotation.Z,
            mData.Rotation.W
            );
    }

    public void SetRadius(float r)
    {
        mRadius = r;
    }

    public void SetRegiontype(DRegionInfo.RegionType type)
    {
        mRegionType = type;
    }
}

public class SceneTownDoors : SceneRegionInfo, ISceneTownDoorData
{
    private FBSceneData.DTownDoor mData;
    private ISceneRegionInfoData mSuper;

    public SceneTownDoors(FBSceneData.DTownDoor data) : base (data.Super)
    {
        mData = data;
        mSuper = this;
    }
    

    public Vector3 GetBirthPos()
    {
        return GetLocalBirthPos() + GetRegionInfo().GetEntityInfo().GetPosition();
    }

    public int GetDoorID()
    {
        return mData.DoorID;
    }

    public Vector3 GetLocalBirthPos()
    {
        return new Vector3(
            mData.BirthPos.X,
            mData.BirthPos.Y,
            mData.BirthPos.Z
            );
    }

    public ISceneRegionInfoData GetRegionInfo()
    {
        return mSuper;
    }

    public int GetSceneID()
    {
        return mData.SceneID;
    }

    public int GetTargetDoorID()
    {
        return mData.TargetDoorID;
    }

    public int GetTargetSceneID()
    {
        return mData.TargetSceneID;
    }

    public DoorTargetType GetDoorTargetType()
    {
        return (DoorTargetType)mData.DoorType;
    }
}

public class BattleSceneData : ISceneData
{
    private FBSceneData.DSceneData mData = null;

    private ISceneEntityInfoData mBirthPosition = null;
    private ISceneEntityInfoData mHellbirthposition = null;

    private DynSceneSetting mDynSceneSetting = null;

    private List<ISceneDecoratorInfoData> mDecoratorInfos = new List<ISceneDecoratorInfoData>();

    private List<ISceneTransportDoorData> mTransportDoors = new List<ISceneTransportDoorData>();

    private List<ISceneDestructibleInfoData> mDestructibleInfos = new List<ISceneDestructibleInfoData>();

    private List<ISceneEntityInfoData> mEntityInfos = new List<ISceneEntityInfoData>();

    private List<ISceneFunctionPrefabData> mFunctionPrefabDatas = new List<ISceneFunctionPrefabData>();

    private List<ISceneMonsterInfoData> mMonsterInfos = new List<ISceneMonsterInfoData>();

    private List<ISceneNPCInfoData> mNPCInfos = new List<ISceneNPCInfoData>();

    private List<ISceneRegionInfoData> mRegionInfos = new List<ISceneRegionInfoData>();

    private List<ISceneTownDoorData> mTownDoors = new List<ISceneTownDoorData>();
    private List<ISceneEntityInfoData> airBattleBornPos = new List<ISceneEntityInfoData>();
    private byte[] mBlockLayer = new byte[0];

    public BattleSceneData(FBSceneData.DSceneData data)
    {
        mData = data;
        _initAirBattleBornPos();
        _initBirthPosition();
        _initDecorators();
        _initTransportDoors();
        _initDestructibleInfos();
        _initEntityInfos();
        _initFunctionPrefabs();
        _initMonsterInfos();
        _initNPCInfos();
        _initHellbirthposition();
        _initRegionInfos();
        _initTownDoors();

        _initBlockLayer();
    }

    private void _initBlockLayer()
    {
        mBlockLayer = new byte[mData.BlocklayerLength];
        for (int i = 0; i < mBlockLayer.Length; ++i)
        {
            mBlockLayer[i] = mData.GetBlocklayer(i);
        }
    }

    private void _initBirthPosition()
    {
        mBirthPosition = new SceneEntityInfo(mData.Birthposition);
    }

    private void _initDecorators()
    {
        for (int i = 0; i < mData.DecoratorinfoLength; ++i)
        {
            mDecoratorInfos.Add(new SceneDecoratorInfo(mData.GetDecoratorinfo(i)));
        }
    }
    private void _initAirBattleBornPos()
    {
        for (int i = 0; i < mData.FighterBornPositionLength; ++i)
        {
            airBattleBornPos.Add(new SceneTransferInfo(mData.GetFighterBornPosition(i)));
        }
    }

    private void _initTransportDoors()
    {
        for (int i = 0; i < mData.TransportdoorLength; ++i)
        {
            mTransportDoors.Add(new SceneTransportDoor(mData.GetTransportdoor(i)));
        }
    }

    private void _initDestructibleInfos()
    {
        for (int i = 0; i < mData.DesructibleinfoLength; i++)
        {
            mDestructibleInfos.Add(new SceneDestructibleInfo(mData.GetDesructibleinfo(i)));
        }
    }

    private void _initEntityInfos()
    {
        for (int i = 0; i < mData.EntityinfoLength; ++i)
        {
            mEntityInfos.Add(new SceneEntityInfo(mData.GetEntityinfo(i)));
        }
    }

    private void _initFunctionPrefabs()
    {
        for (int i = 0; i < mData.FunctionPrefabLength; i++)
        {
            mFunctionPrefabDatas.Add(new SceneFunctionPrefab(mData.GetFunctionPrefab(i)));
        }
    }


    private void _initMonsterInfos()
    {
        for (int i = 0; i < mData.MonsterinfoLength; i++)
        {
            mMonsterInfos.Add(new SceneMonsterInfo(mData.GetMonsterinfo(i)));
        }
    }

    private void _initNPCInfos()
    {
        for (int i = 0; i < mData.NpcinfoLength; i++)
        {
            mNPCInfos.Add(new SceneNPCInfo(mData.GetNpcinfo(i)));
        }
    }

    private void _initHellbirthposition()
    {
        mHellbirthposition = new SceneEntityInfo(mData.Hellbirthposition);
    }

    private void _initRegionInfos()
    {
        for (int i = 0; i < mData.RegioninfoLength; i++)
        {
            mRegionInfos.Add(new SceneRegionInfo(mData.GetRegioninfo(i)));
        }
    }


    private void _initTownDoors()
    {
        for (int i = 0; i < mData.TownDoorLength; i++)
        {
            mTownDoors.Add(new SceneTownDoors(mData.GetTownDoor(i)));
        }
    }

    public ISceneEntityInfoData GetBirthPosition()
    {
        return mBirthPosition;
    }

    public byte GetBlockLayer(int i)
    {
        return mData.GetBlocklayer(i);
    }

    public int GetBlockLayerLength()
    {
        return mData.BlocklayerLength;
    }

    public float GetCameraAngle()
    {
        return mData.CameraAngle;
    }

    public float GetCameraDistance()
    {
        return mData.CameraDistance;
    }

    public float GetCameraFarClip()
    {
        return mData.CameraFarClip;
    }

    public float GetCameraLookHeight()
    {
        return mData.CameraLookHeight;
    }

    public float GetCameraNearClip()
    {
        return mData.CameraNearClip;
    }

    public float GetCameraSize()
    {
        return mData.CameraSize;
    }

    public Vector2 GetCameraXRange()
    {
        return new Vector2(mData.CameraXRange.X, mData.CameraXRange.Y);
    }

    public Vector2 GetCameraZRange()
    {
        return new Vector2(
            mData.CameraZRange.X,
            mData.CameraZRange.Y
        );
    }

    public Vector3 GetCenterPostionNew()
    {
        return new Vector3(
            mData.CenterPostionNew.X,
            mData.CenterPostionNew.Y,
            mData.CenterPostionNew.Z
        );
    }

    public ISceneDecoratorInfoData GetDecoratorInfo(int i)
    {
        if (null == mDecoratorInfos) return null;
        if (0 > i) return null;
        if (mDecoratorInfos.Count <= i) return null;

        return mDecoratorInfos[i];
    }

    public int GetDecoratorInfoLenth()
    {
        if (null == mDecoratorInfos) return 0;
        return mDecoratorInfos.Count;
    }

    public ISceneDestructibleInfoData GetDestructibleInfo(int i)
    {
        if (null == mDestructibleInfos) return null;
        if (0 > i) return null;
        if ( mDestructibleInfos.Count <= i) return null;

        return mDestructibleInfos[i];
    }

    public int GetDestructibleInfoLength()
    {
        if (null == mDestructibleInfos) return 0;
        return mDestructibleInfos.Count;
    }

    public ISceneEntityInfoData GetEntityInfo(int i)
    {
        if (null == mEntityInfos) return null;
        if (0 > i) return null;
        if ( mEntityInfos.Count <= i) return null;

        return mEntityInfos[i];
    }

    public int GetEntityInfoLength()
    {
        if (null == mEntityInfos) return 0;
        return mEntityInfos.Count;
    }

    public ISceneFunctionPrefabData GetFunctionPrefab(int i)
    {
        if (null == mFunctionPrefabDatas) return null;
        if (0 > i) return null;
        if ( mFunctionPrefabDatas.Count <= i) return null;

        return mFunctionPrefabDatas[i];
    }

    public int GetFunctionPrefabLength()
    {
        if (null == mFunctionPrefabDatas) return 0;
        return mFunctionPrefabDatas.Count;
    }

    public Vector2 GetGridSize()
    {
        return new Vector2(
            mData.GridSize.X,
            mData.GridSize.Y
            );
    }

    public ISceneEntityInfoData GetAirBattleBornPos(int i)
    {
        if (null == airBattleBornPos) return null;
        if (0 > i) return null;
        if (airBattleBornPos.Count <= i) return null;

        return airBattleBornPos[i];
    }

    public ISceneEntityInfoData GetHellbirthposition()
    {
        return mHellbirthposition;
    }

    public int GetId()
    {
        return mData.Id;
    }

    public DynSceneSetting GetLightmapsettings()
    {
        return mDynSceneSetting;
    }

    public string GetLightmapsettingsPath()
    {
        return mData.LightmapsettingsPath;
    }

    public Vector3 GetLogicPos()
    {
        return new Vector3(
            mData.LogicPos.X,
            mData.LogicPos.Y,
            mData.LogicPos.Z
            );
    }

    public int GetLogicX()
    {
        return mData.LogicXmax - mData.LogicXmin;
    }

    public int GetLogicXmax()
    {
        return mData.LogicXmax;
    }

    public int GetLogicXmin()
    {
        return mData.LogicXmin;
    }

    public Vector2 GetLogicXSize()
    {
        return new Vector2(
            mData.LogicXSize.X,
            mData.LogicXSize.Y
        );
    }

    public int GetLogicZ()
    {
        return mData.LogicZmax - mData.LogicZmin;
    }

    public int GetLogicZmax()
    {
        return mData.LogicZmax;
    }

    public int GetLogicZmin()
    {
        return mData.LogicZmin;
    }

    public Vector2 GetLogicZSize()
    {
        return new Vector2(
            mData.LogicZSize.X,
            mData.LogicZSize.Y
            );
    }

    public ISceneMonsterInfoData GetMonsterInfo(int i)
    {
        if (null == mMonsterInfos) return null;
        if (0 > i) return null;
        if ( mMonsterInfos.Count <= i) return null;

        return mMonsterInfos[i];
    }

    public int GetMonsterInfoLength()
    {
        if (null == mMonsterInfos) return 0;
        return mMonsterInfos.Count;
    }

    public string GetName()
    {
        return mData.Name;
    }

    public ISceneNPCInfoData GetNpcInfo(int i)
    {
        if (null == mNPCInfos) return null;
        if (0 > i) return null;
        if ( mNPCInfos.Count <= i) return null;

        return mNPCInfos[i];
    }

    public int GetNpcInfoLength()
    {
        if (null == mNPCInfos) return 0;
        return mNPCInfos.Count;
    }

    public Color GetObjectDyeColor()
    {
        return new Color(
            mData.ObjectDyeColor.R,
            mData.ObjectDyeColor.G,
            mData.ObjectDyeColor.B,
            mData.ObjectDyeColor.A
            );
    }

    public string GetPrefabPath()
    {
        return mData.Prefabpath;
    }

    public byte[] GetRawBlockLayer()
    {
        return mBlockLayer;
    }

    public ushort[] GetRawGrassLayer()
    {
        return null;
    }

    public ushort GetGrassId(int gridX, int gridY)
    {
        return 0;
    }
    public ushort GetEcosystemId(int gridX, int gridY)
    {
        return 0;
    }
    public ushort[] GetRawEcosystemLayer()
    {
        return null;
    }

    public byte[] GetRawEventAreaLayer()
    {
        return null;
    }

    public byte GetEventAreaLayer(int i)
    {
        return 0;
    }

    public ISceneRegionInfoData GetRegionInfo(int i)
    {
        if (null == mRegionInfos) return null;
        if (0 > i) return null;
        if ( mRegionInfos.Count <= i) return null;

        return mRegionInfos[i];
    }

    public int GetRegionInfoLength()
    {
        if (null == mRegionInfos) return 0;
        return mRegionInfos.Count;
    }

    public Vector3 GetScenePostion()
    {
        return new Vector3(
            mData.ScenePostion.X,
            mData.ScenePostion.Y,
            mData.ScenePostion.Z
            );
    }

    public float GetSceneUScale()
    {
        return mData.SceneUScale;
    }

    public int GetTipsID()
    {
        return mData.TipsID;
    }

    public ISceneTownDoorData GetTownDoor(int i)
    {
        if (null == mTownDoors) return null;
        if (0 > i) return null;
        if ( mTownDoors.Count <= i) return null;

        return mTownDoors[i];
    }

    public int GetTownDoorLength()
    {
        if (null == mTownDoors) return 0;
        return mTownDoors.Count;
    }
    public ISceneTransportDoorData GetTransportDoor(int i)
    {
        if (null == mTransportDoors) return null;
        if (0 > i) return null;
        if ( mTransportDoors.Count <= i) return null;

        return mTransportDoors[i];
    }

    public int GetTransportDoorLength()
    {
        if (null == mTransportDoors) return 0;
        return mTransportDoors.Count;
    }

    public EWeatherMode GetWeatherMode()
    {
        return (EWeatherMode)mData.WeatherMode;
    }

    public bool IsCameraPersp()
    {
        return mData.CameraPersp;
    }

    public void SetLightmapsettings(DynSceneSetting setting)
    {
        mDynSceneSetting = setting;
    }

    public string GetLutTexPath()
    {
        return string.Empty;
    }

    public string GetLightmapTexPath()
    {
        return string.Empty;
    }

    public Vector4 GetLightmapPosition()
    {
        return Vector4.zero;
    }
}
