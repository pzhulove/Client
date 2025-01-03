﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System;
 using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.AnimatedValues;
#endif

[System.Serializable]
public enum DEntityType
{
    [Description("NPC")]
    NPC = 0,
    [Description("怪物")]
    MONSTER,
    [Description("修饰物")]
    DECORATOR,
    [Description("可破坏物")]
    DESTRUCTIBLE,
    [Description("区域")]
    REGION,
    [Description("传送门")]
    TRANSPORTDOOR,
    [Description("BOSS怪物")]
    BOSS,
    [Description("精英怪物")]
    ELITE,
    [Description("出生点")]
    BIRTHPOSITION,
    [Description("城镇传送门")]
    TOWNDOOR,
    [Description("功能预设")]
    FUNCTION_PREFAB,
    /// <summary>
    /// 当做可破坏物的怪物
    /// 技能实现的怪物
    /// </summary>
    [Description("技能实现的怪物")]
    MONSTERDESTRUCT,
    [Description("深渊柱子出生点")]
    HELLBIRTHPOSITION,

	[Description("活动普通怪物刷怪点")]
	ACTIVITY_MONSTER_POS,

	[Description("活动精英怪物刷怪点")]
	ACTIVITY_ELITE_POS,

	[Description("活动BOSS怪物刷怪点")]
	ACTIVITY_BOSS_POS,

    [Description("吃鸡道具投放点")]
    RESOURCE_POS,

    [Description("吃鸡人物降落点")]
    FIGHTER_BORN_POS,

    [Description("生态资源点")]
    ECOSYSTEM_RESOURCE_POS,

    MAX
}

[System.Serializable]
public class DEntityInfo : ISceneEntityInfoData
{
    [MultiPropertyGUI("单场景唯一ID", 1, false)]
    public int          globalid;
    [MultiPropertyGUI("资源ID", 2, false)]
    public int          resid;

    [MultiPropertyGUI("名字:", 3)]
    public string Name
    {
        get { return name; }
        set
        { 
            name = value;
            if (OnNameChanged != null)
            {
                OnNameChanged(this, name);
            }
        }
    }
    
    [MultiPropertyGUI("Path:", 4, false)]
    public string       path;
    
    [MultiPropertyGUI("描述:",5)]
    public string Description
    {
        get { return description; }
        set
        {
            description = value; 
            if (OnDescriptionChanged != null)
            {
                OnDescriptionChanged(this, description);
            }
        }
    }
    [MultiPropertyGUI("类型:", 6, false, false)]
    public DEntityType  type;
    
    [Space(3)]
    [FontStyle(24, 0.5f, 0.5f, 0.5f)]
    [MultiPropertyGUI("", 0, false)]
    public string       typename;

    [MultiPropertyGUI("位置:", 8)]
    public Vector3 Position
    {
        get { return position; }
        set
        {
            position = value; 
            
/*
            var monster = this as DMonsterInfo;
            if (monster != null)
            {
                monster.destructInfo.position = value;
            }
            */

            if (OnPositionChange != null)
            {
                OnPositionChange(this, position);
            }
        }
    }
    
    [ConditionDraw("type", DEntityType.TRANSPORTDOOR, ConditionDrawAttribute.Option.UnEqual)]
    [MultiPropertyGUI("缩放:", 9)]
    public float Scale
    {
        get { return scale; }
        set
        {
            scale = value;

            /*var monster = this as DMonsterInfo;
            if (monster != null)
            {
                monster.destructInfo.scale = value;
            }*/
            if (OnScaleChange != null)
            {
                OnScaleChange(this, scale);
            }
        }
    }
    [MultiPropertyGUI("颜色:", 10)]
    public Color Color
    {
        get { return color; }
        set
        {
            color = value; 
            if (OnColorChange != null)
            {
                OnColorChange(this, color);
            }
        }
    }

    [SerializeField]
    private string      name;
    [SerializeField]
    private string      description;
    [SerializeField]
    public Vector3      position;
    [SerializeField]
    public float        scale = 1.0f;
    [SerializeField]
    public Color        color = Color.white;
    
    public UnityAction<DEntityInfo, string> OnNameChanged;
    public UnityAction<DEntityInfo, string> OnDescriptionChanged;
    public UnityAction<DEntityInfo, Vector3> OnPositionChange;
    public UnityAction<DEntityInfo, float> OnScaleChange;
    public UnityAction<DEntityInfo, Color> OnColorChange;

#if UNITY_EDITOR
    [System.NonSerialized]
    public GameObject obj;
#endif

    public virtual void Duplicate(DEntityInfo info)
    {
        resid       = info.resid;
        name  		= info.name;
        path  		= info.path;
        description = info.description;
        type        = info.type;
        typename    = info.typename;
        position 	= info.position;
        scale    	= info.scale;
        color    	= info.color;
    } 

    public virtual string GetModelPathByResID()
    {
        var res = TableManager.GetInstance().GetTableItem<ProtoTable.ResTable>(resid);
        if (res == null)
        {
            return "";
        }

        return res.ModelPath;
    }

#if UNITY_EDITOR
     public virtual void DuplicateObjInfo(DEntityInfo info)
     {
         if(obj == null)
         {
             return;
         }
         
         obj.transform.localPosition = info.obj.transform.localPosition;
         obj.transform.localRotation = info.obj.transform.localRotation;
         obj.transform.localScale    = info.obj.transform.localScale;
     }
#endif

    public int GetGlobalid()
    {
        return globalid;
    }

    public int GetResid()
    {
        return resid;
    }

    public string GetName()
    {
        return name;
    }

    public string GetPath()
    {
        return path;
    }

    public string GetDescription()
    {
        return description;
    }

    DEntityType ISceneEntityInfoData.GetType()
    {
        return type;
    }

    public string GetTypename()
    {
        return typename;
    }

    public Vector3 GetPosition()
    {
        return position;
    }

    public float GetScale()
    {
        return scale;
    }

    public Color GetColor()
    {
        return color;
    }
}

[System.Serializable]
public class FunctionPrefab : DEntityInfo, ISceneFunctionPrefabData
{
    public enum FunctionType
    {
        [Description("随从")]
        FT_FollowPet = 0,
        [Description("魔法宝贝")]
        FT_FollowPet2,
        FT_COUNT,
    }
    
    [MultiPropertyGUI(false)]
    public FunctionType eFunctionType;

    public override void Duplicate(DEntityInfo info)
    {
        if(info != this)
        {
            base.Duplicate(info);
            FunctionPrefab target = info as FunctionPrefab;
            this.eFunctionType = target.eFunctionType;
        }
    }

    public FunctionType GetFunctionType()
    {
        return eFunctionType;
    }

    public ISceneEntityInfoData GetEntityInfo()
    {
        return this;
    }
}

[System.Serializable]
public class DNPCInfo : DEntityInfo, ISceneNPCInfoData
{
    [MultiPropertyGUI(false)]
    public Quaternion rotation;
    [MultiPropertyGUI("最小寻路范围:", 20)]
    public Vector2 minFindRange = new Vector2(1.0f, 3.0f);
    [MultiPropertyGUI("最大寻路范围:", 21)]
    public Vector2 maxFindRange = new Vector2(2.5f, 5.0f);

    public override void Duplicate(DEntityInfo info)
    {
        base.Duplicate(info);
        var value = info as DNPCInfo;
        if(value != null)
        {
            rotation = value.rotation;
            minFindRange = value.minFindRange;
            maxFindRange = value.maxFindRange;
        }
    }

    public Quaternion GetRotation()
    {
        return rotation;
    }

    public Vector2 GetMinFindRange()
    {
        return minFindRange;
    }

    public Vector2 GetMaxFindRange()
    {
        return maxFindRange;
    }

    public ISceneEntityInfoData GetEntityInfo()
    {
        return this;
    }
}


[System.Serializable]
public class DMonsterInfo : DEntityInfo, ISceneMonsterInfoData
{
    public enum MonsterSwapType
    {
        [Description("点刷怪")]
        POINT_SWAP,
        [Description("圆形刷怪")]
        CIRCLE_SWAP,
        [Description("矩形刷怪")]
        RECT_SWAP
    }

    public enum FaceType
    {
        [Description("右朝向")]
        Right,
        [Description("左朝向")]
        Left,
        [Description("随机朝向")]
        Random
    }

    public enum FlowRegionType
    {
        [Description("无")]
        None,
        [Description("区域跟随")]
        Region,
        [Description("可破坏物->怪物")]
        Destruct,
    }

    [MultiPropertyGUI("刷怪类型:", 11)]
    public MonsterSwapType  swapType;
    //[MultiPropertyGUI("朝向:", 12)]
    //public FaceType         faceType;

    [MultiPropertyGUI("是否朝左:", 12)]
    public bool             isFaceLeft;
    [MultiPropertyGUI("数量:", 13)]
    public int              swapNum;
    [MultiPropertyGUI("延迟刷新:", 13)]
    public int              swapDelay;
    [MultiPropertyGUI("刷怪组ID:", 14)]
    public int              flushGroupID;

    [MultiPropertyGUI("跟随区域类型:", 15)]
    public FlowRegionType       flowRegionType;
    public DRegionInfo          regionInfo = new DRegionInfo();
    public DDestructibleInfo    destructInfo = new DDestructibleInfo();

    [MultiPropertyGUI("视野:", 16)]
    public int              sight;

    //（同组怪物一个被攻击后，其他组内怪物也会进入攻击状态）
    [MultiPropertyGUI("组ID:", 17)]
    public int              groupIndex;

    [MultiPropertyGUI("动作AI:", 18)]
    public string              aiActionPath;
    
    [MultiPropertyGUI("剧情AI:", 19)]
    public string              aiScenarioPath;

    [MultiPropertyGUI("创建分组ID:", 20)]
    public int subGroupID;

    #region Const
    private const int kDiffculte = 10;
    private const int kTypeID = 10;
    private const int kLevel = 100;
    private const int kID = 1000;
    #endregion

    #region Get && Set
    private int monsterID;
    private int monsterLevel;
    private int monsterTypeID;
    private int monsterDiffcute;

    public override void Duplicate(DEntityInfo info)
    {
        base.Duplicate(info);
        var value = info as DMonsterInfo;
        if(value != null)
        {
            swapType = value.swapType;
            isFaceLeft = value.isFaceLeft;
            swapNum  = value.swapNum;
            swapDelay = value.swapDelay;
            
            monsterID = value.monsterID;
            monsterLevel = value.monsterLevel;
            monsterTypeID = value.monsterTypeID;
            monsterDiffcute = value.monsterDiffcute;
            sight = value.sight;
            groupIndex = value.groupIndex;
            aiActionPath = value.aiActionPath;
            aiScenarioPath = value.aiScenarioPath;
        }
    }
    
    public override string GetModelPathByResID()
    {
        var unit = TableManager.GetInstance().GetTableItem<ProtoTable.UnitTable>(resid);
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
    
    [MultiPropertyGUI("", 0, false, false)]
    public int monID
    {
        get
        {
            return monsterID;
        }

        set
        {
            if (monsterID != value && value > 0)
            {
                monsterID = value;
                _encodeID();
            }
        }
    }

    [MultiPropertyGUI("怪物等级:", 20)]
    public int monLevel
    {
        get
        {
            _decodeID();
            
            return monsterLevel;
        }

        set
        {
            if (monsterLevel != value && value >= 0)
            {
                monsterLevel = value;
                _encodeID();
            }
        }
    }

    [MultiPropertyGUI("", 20, false, false)]
    public int monTypeID
    {
        get
        {
            _decodeID();
            return monsterTypeID;
        }

        set
        {
            if (monsterTypeID != value && value > 0)
            {
                monsterTypeID = value;
                _encodeID();
            }
        }
    }

    
    [MultiPropertyGUI("怪物剧情难度:", 21)]
    public int monDiffcute
    {
        get
        {
            _decodeID();
            return monsterDiffcute;
        }

        set
        {
            if (monsterDiffcute != value && value > 0)
            {
                monsterDiffcute = value;
                _encodeID();
            }
        }
    }


    public void SetID(int id)
    {
        resid = id;
        _decodeID();
    }
    #endregion

    [MultiPropertyGUI("怪物信息表ID:", 100, typeof(ProtoTable.MonsterInstanceInfoTable))]
    public int MonsterInfoTableID;

    #region Decode && Encode
    public void _decodeID()
    {
        var tid = resid;
        monsterDiffcute = tid % kDiffculte;
        tid /= kDiffculte;

        monsterTypeID = tid % kTypeID;
        tid /= kTypeID;

        monsterLevel = tid % kLevel;
        tid /= kLevel;

        monsterID = tid;
    }

    public void _encodeID()
    {
        resid = monsterID;

        resid *= kLevel;
        resid += monsterLevel % kLevel;

        resid *= kTypeID;
        resid += monsterTypeID % kTypeID;

        resid *= kDiffculte;
        resid += monsterDiffcute % kDiffculte;
    }
    #endregion

    public MonsterSwapType GetSwapType()
    {
        return swapType;
    }

    public int GetMonsterInfoTableID()
    {
        return MonsterInfoTableID;
    }

    /*public FaceType GetFaceType()
    {
        return faceType;
    }*/

    public bool GetIsFaceLeft()
    {
        return isFaceLeft;
    }

    public int GetSight()
    {
        return sight;
    }

    public int GetSwapNum()
    {
        return swapNum;
    }

    public int GetSwapDelay()
    {
        return swapDelay;
    }

    public int GetFlushGroupID()
    {
        return flushGroupID;
    }

    public int GetSubGroupID()
    {
        return subGroupID;
    }

    public FlowRegionType GetFlowRegionType()
    {
        return flowRegionType;
    }

    public ISceneRegionInfoData GetRegionInfo()
    {
        return regionInfo;
    }

    public ISceneDestructibleInfoData GetDestructInfo()
    {
        return destructInfo;
    }

    public void SetMonsterID(int id)
    {
        SetID(id);
    }

    public int GetMonsterID()
    {
        return monID;
    }


    public int GetMonsterLevel()
    {
        return monLevel;
    }

    public int GetMonsterTypeID()
    {
        return monTypeID;
    }

    public int GetMonsterDiffcute()
    {
        return monDiffcute;
    }

    public ISceneEntityInfoData GetEntityInfo()
    {
        return this;
    }

    public int GetGroupIndex()
    {
        return groupIndex;
    }

    public string GetAIActionPath()
    {
        return aiActionPath;
    }
    
    public string GetAIScenarioPath()
    {
        return aiScenarioPath;
    }
}

[System.Serializable]
public class DDecoratorInfo : DEntityInfo, ISceneDecoratorInfoData
{
    [MultiPropertyGUI("修饰物缩放", 20)]
    public Vector3 LocalScale
    {
        get { return localScale; }
        set
        {
            localScale = value;
            if (OnDecoratorScaleChange != null)
            {
                OnDecoratorScaleChange(this, localScale);
            }
        }
    }
    [MultiPropertyGUI("修饰物旋转", 21)]
    public Quaternion Rotation
    {
        get { return rotation; }
        set
        {
            rotation = value;
            if (OnDecoratorRotationChange != null)
            {
                OnDecoratorRotationChange(this, rotation);
            }
        }
    }
    
    [SerializeField]
    private Quaternion rotation;
    [SerializeField]
    private Vector3 localScale = Vector3.one;
    
    public UnityAction<DEntityInfo, Vector3> OnDecoratorScaleChange;
    public UnityAction<DEntityInfo, Quaternion> OnDecoratorRotationChange;
    
    public override void Duplicate(DEntityInfo info)
    {
        base.Duplicate(info);
        var value = info as DDecoratorInfo;
        if(value != null)
        {
             localScale = value.localScale;
             rotation   = value.rotation;
        }
    }

    public Vector3 GetLocalScale()
    {
        return localScale;
    }

    public Quaternion GetRotation()
    {
        return rotation;
    }

    public ISceneEntityInfoData GetEntityInfo()
    {
        return this;
    }
}

[System.Serializable]
public class DDestructibleInfo : DEntityInfo, ISceneDestructibleInfoData
{
    [MultiPropertyGUI(false)]
    public Quaternion   rotation;
    
    [MultiPropertyGUI(false)]
    public int          level;
    
    [MultiPropertyGUI("刷怪组ID:", 20)]
    public int          flushGroupID;

    public override string GetModelPathByResID()
    {
        var destruc = TableManager.GetInstance().GetTableItem<ProtoTable.DestrucTable>(resid);
        if (null == destruc)
        {
            return "";
        }

        var res = TableManager.GetInstance().GetTableItem<ProtoTable.ResTable>(destruc.Mode);
        if (res == null)
        {
            return "";
        }

        return res.ModelPath;
    }

    public override void Duplicate(DEntityInfo info)
    {
        base.Duplicate(info);
        var value = info as DDestructibleInfo;
        if(value != null)
        {
             rotation   = value.rotation;
        }
    }

    public Quaternion GetRotation()
    {
        return rotation;    
    }

    public int GetLevel()
    {
        return level;
    }

    public int GetFlushGroupID()
    {
        return flushGroupID;
    }

    public ISceneEntityInfoData GetEntityInfo()
    {
        return this;
    }
}

[System.Serializable]
public class DRegionInfo : DEntityInfo, ISceneRegionInfoData
{
    public enum RegionType
    {
        [Description("圆形")]
        Circle = 0,
        [Description("矩形")]
        Rectangle
    }
    [MultiPropertyGUI("区域类型:", 16)]
    public RegionType regiontype;
    
    [ConditionDraw("regiontype", RegionType.Rectangle, ConditionDrawAttribute.Option.Equal)]
    [MultiPropertyGUI("矩形:", 17)]
    public Vector2    rect   = Vector2.one;
    
    [ConditionDraw("regiontype", RegionType.Circle, ConditionDrawAttribute.Option.Equal)]
    [MultiPropertyGUI("半径:", 18)]
    public float      radius = 1.0f;
    [MultiPropertyGUI(false)]
    public Quaternion rotation;
    
    public override string GetModelPathByResID()
    {
        var region = TableManager.GetInstance().GetTableItem<ProtoTable.SceneRegionTable>(resid);
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
    
    public override void Duplicate(DEntityInfo info)
    {
        base.Duplicate(info);
        var value = info as DRegionInfo;
        if(value != null)
        {
             regiontype   = value.regiontype;
             rect         = value.rect;
             radius       = value.radius;
             rotation     = value.rotation;
        }
    }

    public RegionType GetRegiontype()
    {
        return regiontype;
    }

    public Vector2 GetRect()
    {
        return rect;
    }

    public float GetRadius()
    {
        return radius;
    }

    public Quaternion GetRotation()
    {
        return rotation;
    }

    public ISceneEntityInfoData GetEntityInfo()
    {
        return this;
    }

    public void SetRegiontype(RegionType type)
    {
        this.regiontype = type;
    }

    public void SetRadius(float r)
    {
        this.radius = r;
    }
}

public static class DTransportDoorTool
{
    public static TransportDoorType OppositeType(this TransportDoorType doortype)
    {
        switch (doortype)
        {
            case TransportDoorType.Buttom:
                return TransportDoorType.Top;
            case TransportDoorType.Top:
                return TransportDoorType.Buttom;
            case TransportDoorType.Right:
                return TransportDoorType.Left;
            case TransportDoorType.Left:
                return TransportDoorType.Right;
        }
        return TransportDoorType.Right;
    }
}

public enum TransportDoorType
{
    [Description("←")]
    Left = 0,
    [Description("↑")]
    Top = 1,
    [Description("→")]
    Right = 2,
    [Description("↓")]
    Buttom = 3,
	[Description("X")]
	None = 4,
}


[System.Serializable]
public class DTransportDoor : DRegionInfo, ISceneTransportDoorData
{
    [MultiPropertyGUI("是否是彩蛋房间的门", 30)]
    public bool isEggDoor;
    
    [SerializeField]
    private TransportDoorType doortype;
    [MultiPropertyGUI("始发地类型", 31)]
    public TransportDoorType DoorType
    {
        get { return doortype; }
        set
        {
            doortype = value;
            nextdoortype = doortype.OppositeType();
        }
    }
    
    // default is top<->buttom left<->right
    // but anything is possible
    [MultiPropertyGUI("目的地类型", 32, false)]
    public TransportDoorType nextdoortype;
    
    [MultiPropertyGUI("城镇目的地路径（测试使用，请在版本过后删除）：", 33)]
    public string townscenepath;

    // according the id to get the scene path
    [MultiPropertyGUI("城镇目的地路径ID（测试使用，请在版本过后删除）：", 34)]
    public int nextsceneid;
    


    [MultiPropertyGUI("出生点位置：", 35)]
    public Vector3 birthposition;

    [MultiPropertyGUI("材质：", 36)]
    public DAssetObject materialAsset;

    [System.NonSerialized]
    private VInt3 mBirthPos;

    [System.NonSerialized]
    private bool bInitBirthPos = false;

#if UNITY_EDITOR && !SERVER_LOGIC
    [System.NonSerialized]
    [MultiPropertyGUI(false)]
    public ControlDoorState ctrlDoorState;

    public void UpdateData()
    {
        if(ctrlDoorState == null && obj != null)
        {
            ctrlDoorState = obj.GetComponentInChildren<ControlDoorState> (true);
        }
    }
    public Vector3 GetDoorTransCenter()
	{
        if(obj == null)
        {
            Logger.LogErrorFormat(" GetDoorTransCenter GameObject is null!!");
            return Vector3.zero;
        }

        UpdateData();
		if (ctrlDoorState != null) {

			switch(doortype)
			{
			case TransportDoorType.Buttom:
				{
					if (ctrlDoorState.OpenDoorObj_BOTTOM) {
						return ctrlDoorState.OpenDoorObj_BOTTOM.transform.position;
					}
				}
				break;
			case TransportDoorType.Top:
				{
					if (ctrlDoorState.OpenDoorObj_TOP) {
						return ctrlDoorState.OpenDoorObj_TOP.transform.position;
					}
				}
				break;
			case TransportDoorType.Left:
				{
					if (ctrlDoorState.OpenDoorObj_LEFT) {
						return ctrlDoorState.OpenDoorObj_LEFT.transform.position;
					}
				}
				break;
			case TransportDoorType.Right:
				{
					if (ctrlDoorState.OpenDoorObj_RIGHT) {
						return ctrlDoorState.OpenDoorObj_RIGHT.transform.position;
					}
				}
				break;
			}
		}

        //Logger.LogErrorFormat("DoorTrans {0} Do Not Set In {1},Use default!",doortype,obj.name);

		return obj.transform.position;
	}
#else
     public void UpdateData()
    {  
    }
    public Vector3 GetDoorTransCenter()
	{
#if UNITY_ANDROID
        return Vector3.zero;
#else
		return Vector3.zero;
#endif
	}
    
#endif

    public override void Duplicate(DEntityInfo info)
    {
        base.Duplicate(info);
        var value = info as DTransportDoor;
        if(value != null)
        {
            doortype   = value.doortype;
            nextsceneid         = value.nextsceneid;
            townscenepath       = value.townscenepath;
            nextdoortype        = value.nextdoortype;
            birthposition       = value.birthposition;
            mBirthPos           = value.mBirthPos;
            bInitBirthPos       = value.bInitBirthPos;
            materialAsset       = value.materialAsset;
        }
    }

    public TransportDoorType GetDoortype()
    {
        return doortype;
    }
    public bool IsEggDoor()
    {
        return isEggDoor;
    }


    public int GetNextsceneid()
    {
        return nextsceneid;
    }

    public string GetTownscenepath()
    {
        return townscenepath;
    }

    public TransportDoorType GetNextdoortype()
    {
        return nextdoortype;
    }

    public VInt3 GetBirthposition()
    {
        if(bInitBirthPos == false)
        {
            mBirthPos = new VInt3(birthposition);
            bInitBirthPos = true;
        }
        return mBirthPos;
    }

    public ISceneRegionInfoData GetRegionInfo()
    {
        return this;
    }

    public void SetBirthposition(VInt3 pos)
    {
        if(bInitBirthPos == false)
        {
            bInitBirthPos = true;
        }

        mBirthPos = pos;
    }

    public string GetMaterialPath()
    {
        return materialAsset.m_AssetPath;
    }
}

[System.Serializable]
public class DTransferInfo : DEntityInfo
{
    [MultiPropertyGUI("区域ID:", 20)]
    public int regionId;
    public override void Duplicate(DEntityInfo info)
    {
        base.Duplicate(info);
        var value = info as DTransferInfo;
        if (value != null)
        {
            regionId = value.regionId;
        }
    }
}
[System.Serializable]
public class DResourceInfo : DEntityInfo
{
    [MultiPropertyGUI("资源表ID", 20)]
    public int resouceId;
    public override void Duplicate(DEntityInfo info)
    {
        base.Duplicate(info);
        var value = info as DResourceInfo;
        if (value != null)
        {
            resouceId = value.resouceId;
        }
    }
}
[System.Serializable]
public class DEcosystemResourceInfo : DEntityInfo
{
    [MultiPropertyGUI("生态资源ID:", 30)]
    public int ecosystemResourceId;
    [MultiPropertyGUI("生态资源类型:", 31)]
    public int ecosystemResourceType = 0;
    public override void Duplicate(DEntityInfo info)
    {
        base.Duplicate(info);
        var value = info as DEcosystemResourceInfo;
        if (value != null)
        {
            ecosystemResourceId = value.ecosystemResourceId;
            ecosystemResourceType = value.ecosystemResourceType;
        }
    }
}
public enum DoorTargetType
{
    Normal = 0,//正常的场景切换的门
    PVEPracticeCourt = 1,//直接进入pve练习场的（战斗）的门
} 
[System.Serializable]
public class DTownDoor : DRegionInfo, ISceneTownDoorData
{
    [MultiPropertyGUI("场景ID:", 30)]
    public int SceneID;
    
    [MultiPropertyGUI("门ID:", 31)]
    public int DoorID;
    
    [MultiPropertyGUI("传送点:", 32)]
    public Vector3 BirthPos;

    [MultiPropertyGUI("目标场景ID:", 33)]
    public int TargetSceneID;
    
    [MultiPropertyGUI("目标门ID:", 34)]
    public int TargetDoorID;
    
    [MultiPropertyGUI("门类型:", 35)]
    public DoorTargetType DoorType;

    public override void Duplicate(DEntityInfo info)
    {
        base.Duplicate(info);
        var value = info as DTownDoor;
        if(value != null)
        {
             SceneID            = value.SceneID;
             DoorID             = value.DoorID;
             BirthPos           = value.BirthPos;
             TargetSceneID      = value.TargetSceneID;
             TargetDoorID       = value.TargetDoorID;
             DoorType           = value.DoorType;
        }
    }

    public int GetSceneID()
    {
        return SceneID;
    }

    public int GetDoorID()
    {
        return DoorID;
    }

    public Vector3 GetBirthPos()
    {
        return GetLocalBirthPos() + GetPosition();
    }

    public int GetTargetSceneID()
    {
        return TargetSceneID;
    }

    public int GetTargetDoorID()
    {
        return TargetDoorID;
    }

    public ISceneRegionInfoData GetRegionInfo()
    {
        return this;
    }

    public Vector3 GetLocalBirthPos()
    {
        return BirthPos;
    }

    public DoorTargetType GetDoorTargetType()
    {
        return DoorType;
    }
}

public class DSceneData : ScriptableObject, ISceneData
{
    public string       _name;
    public int          _id;
    public GameObject   _prefab;
    public string       _prefabpath;
    public float        _CameraLookHeight = 1.0f;
    public float        _CameraDistance = 10.0f;
    public float        _CameraAngle = 20.0f;
    public float        _CameraNearClip = -1f;
    public float        _CameraFarClip = 100.0f;
    public float        _CameraSize = 3.05f;
    public Vector2      _CameraZRange;
    public Vector2      _CameraXRange;
    public bool         _CameraPersp = false;
    public Vector3      _CenterPostionNew;
    public Vector3      _ScenePostion;  // 图形坐标（相对于逻辑原点）
    public float        _SceneUScale = 1.0f;  // 图形坐标（相对于逻辑原点）
    public Vector2      _GridSize = new Vector2(0.25f, 0.25f);
    public Vector2      _LogicXSize;
    public Vector2      _LogicZSize;
    public Color        _ObjectDyeColor = Color.white;

    public Vector3      _LogicPos = Vector3.zero;      // 逻辑原点

    public EWeatherMode _WeatherMode = EWeatherMode.None;

	public int 			_TipsID;

    //public Vector2      _BirthPositionOffset;

    [System.NonSerialized]
    public DynSceneSetting _lightmapsetting;

    public string       _LightmapsettingsPath;
    public int          _LogicXmin, _LogicXmax, _LogicZmin, _LogicZmax;

    public string _LutTexPath;
    public string _LightmapTexPath;
    public Vector4 _LightmapPosition;

    public void LoadDynSceneSetting()
    {
        if(!string.IsNullOrEmpty(_LightmapsettingsPath))
        {
            _lightmapsetting = AssetLoader.instance.LoadRes(_LightmapsettingsPath, typeof(DynSceneSetting)).obj as DynSceneSetting;
        }
    }

    public string GetName()
    {
        return _name;
    }

    public int GetId()
    {
        return _id;
    }

    public string GetPrefabPath()
    {
        return _prefabpath;
    }

    public float GetCameraLookHeight()
    {
        return _CameraLookHeight;
    }

    public float GetCameraDistance()
    {
        return _CameraDistance;
    }

    public float GetCameraAngle()
    {
        return _CameraAngle;
    }

    public float GetCameraNearClip()
    {
        return _CameraNearClip;
    }

    public float GetCameraFarClip()
    {
        return _CameraFarClip;
    }

    public Vector2 GetCameraZRange()
    {
        return _CameraZRange;
    }

    public Vector2 GetCameraXRange()
    {
        return _CameraXRange;
    }

    public bool IsCameraPersp()
    {
        return _CameraPersp;
    }

    public Vector3 GetCenterPostionNew()
    {
        return _CenterPostionNew;
    }

    public Vector3 GetScenePostion()
    {
        return _ScenePostion;
    }

    public float GetSceneUScale()
    {
        return _SceneUScale;
    }

    public Vector2 GetGridSize()
    {
        return _GridSize;
    }

    public Vector2 GetLogicXSize()
    {
        return _LogicXSize;
    }

    public Vector2 GetLogicZSize()
    {
        return _LogicZSize;
    }

    public Color GetObjectDyeColor()
    {
        return _ObjectDyeColor;
    }

    public Vector3 GetLogicPos()
    {
        return _LogicPos;
    }

    public EWeatherMode GetWeatherMode()
    {
        return _WeatherMode;
    }

    public int GetTipsID()
    {
        return _TipsID;
    }

    public string GetLightmapsettingsPath()
    {
        return _LightmapsettingsPath;
    }

    public int GetLogicXmin()
    {
        return _LogicXmin;
    }

    public int GetLogicXmax()
    {
        return _LogicXmax;
    }

    public int GetLogicZmin()
    {
        return _LogicZmin;
    }

    public int GetLogicZmax()
    {
        return _LogicZmax;
    }

    public int GetLogicX()
    {
        return LogicX;
    }

    public int GetLogicZ()
    {
        return LogicZ;
    }

    public int GetEntityInfoLength()
    {
        if (null == _entityinfo) return 0;
        return _entityinfo.Length;
    }

    public ISceneEntityInfoData GetEntityInfo(int i)
    {
        if (null == _entityinfo) return null;
        if (0 > i) return null;
        if (_entityinfo.Length <= i) return null;

        return _entityinfo[i];
    }

    public int GetBlockLayerLength()
    {
        if (null == _blocklayer) return 0;
        return _blocklayer.Length;
    }
    
    public byte[] GetBlockLayer()
    {
        return _blocklayer;
    }

    public int GetNpcInfoLength()
    {
        if (null == _npcinfo) return 0;
        return _npcinfo.Length;
    }

    public ISceneNPCInfoData GetNpcInfo(int i)
    {
        if (null == _npcinfo) return null;
        if (0 > i) return null;
        if (_npcinfo.Length <= i) return null;

        return _npcinfo[i];
    }

    public int GetMonsterInfoLength()
    {
        if (null == _monsterinfo) return 0;
        return _monsterinfo.Length;
    }

    private DMonsterInfo _getMonsterInfo(int i)
    {
        if (null == _monsterinfo) return null;
        if (0 > i) return null;
        if (_monsterinfo.Length <= i) return null;

        return _monsterinfo[i];
    }

    public ISceneMonsterInfoData GetMonsterInfo(int i)
    {
        return _getMonsterInfo(i);
    }

    public ISceneEntityInfoData GetMonsterEntityInfo(int i)
    {
        return _getMonsterInfo(i);
    }

    public int GetDecoratorInfoLenth()
    {
        if (null == _decoratorinfo) return 0;

        return _decoratorinfo.Length;
    }

    public ISceneDecoratorInfoData GetDecoratorInfo(int i)
    {
        if (null == _decoratorinfo) return null;
        if (0 > i) return null;
        if (_decoratorinfo.Length <= i) return null;

        return _decoratorinfo[i];
    }

    public int GetDestructibleInfoLength()
    {
        if (null == _desructibleinfo) return 0;

        return _desructibleinfo.Length;
    }

    public ISceneDestructibleInfoData GetDestructibleInfo(int i)
    {
        if (null == _desructibleinfo) return null;
        if (0 > i) return null;
        if (_desructibleinfo.Length <= i) return null;

        return _desructibleinfo[i];
    }

    public int GetRegionInfoLength()
    {
        if (null == _regioninfo) return 0;

        return _regioninfo.Length;
    }

    public ISceneRegionInfoData GetRegionInfo(int i)
    {
        if (null == _regioninfo) return null;
        if (0 > i) return null;
        if (_regioninfo.Length <= i) return null;

        return _regioninfo[i];
    }

    public int GetTransportDoorLength()
    {
        if (null == _transportdoor) return 0;

        return _transportdoor.Length;
    }

    private DTransportDoor _getTransportDoor(int i)
    {
        if (null == _transportdoor) return null;
        if (0 > i) return null;
        if (_transportdoor.Length <= i) return null;

        return _transportdoor[i];
    }

    public ISceneTransportDoorData GetTransportDoor(int i)
    {
        return _getTransportDoor(i);
    }

    public ISceneRegionInfoData GetTransportDoorRegionInfo(int i)
    {
        return _getTransportDoor(i);
    }

    public ISceneEntityInfoData GetTransportDoorEntityInfo(int i)
    {
        return _getTransportDoor(i);
    }


    public ISceneEntityInfoData GetBirthPosition()
    {
        return _birthposition;
    }

    public ISceneEntityInfoData GetAirBattleBornPos(int i)
    {
        if (null == _fighterBornPosition) return null;
        if (0 > i) return null;
        if (_fighterBornPosition.Length <= i) return null;

        return _fighterBornPosition[i];
    }

    public ISceneEntityInfoData GetHellbirthposition()
    {
        return _hellbirthposition;
    }

    public int GetTownDoorLength()
    {
        if (null == _townDoor) return 0;
        return _townDoor.Length;
    }

    public ISceneTownDoorData GetTownDoor(int i)
    {
        if (null == _townDoor) return null;
        if (0 > i) return null;
        if (_townDoor.Length <= i) return null;

        return _townDoor[i];
    }

    public int GetFunctionPrefabLength()
    {
        if (null == _FunctionPrefab) return 0;
        return _FunctionPrefab.Length;
    }

    public ISceneFunctionPrefabData GetFunctionPrefab(int i)
    {
        if (null == _FunctionPrefab) return null;
        if (0 > i) return null;
        if (_FunctionPrefab.Length <= i) return null;

        return _FunctionPrefab[i];
    }

    public float GetCameraSize()
    {
        //return _CameraSize;
        //改为固定的视域大小3.3
        return 3.3f;
    }

    public DynSceneSetting GetLightmapsettings()
    {
        return _lightmapsetting;
    }

    public void SetLightmapsettings(DynSceneSetting setting)
    {
        _lightmapsetting = setting;
    }

    public byte[] GetRawBlockLayer()
    {
        return _blocklayer;
    }
    public ushort[] GetRawGrassLayer()
    {
        return _grasslayer;
    }
    public ushort[] GetRawEcosystemLayer()
    {
        return _ecosystemLayer;
    }
    public byte[] GetRawEventAreaLayer()
    {
        return _eventAreaLayer;
    }
    public byte GetBlockLayer(int i)
    {
        if (null == _blocklayer) return byte.MinValue;
        if (0 > i) return byte.MinValue;
        if (_blocklayer.Length <= i) return byte.MinValue;

        return _blocklayer[i];
    }
    public ushort GetGrassId(int gridX, int gridY)
    {
        if (_grasslayer == null || _grasslayer.Length <= 0)
        {
            return 0;
        }
        int index = gridY * LogicX + gridX;
        if (index < 0 || index >= _grasslayer.Length) return 0;
        return _grasslayer[gridY * LogicX + gridX];
    }
    public ushort GetEcosystemId(int gridX,int gridY)
    {
        if (_ecosystemLayer == null || _ecosystemLayer.Length <= 0)
        {
            return 0;
        }
        int index = gridY * LogicX + gridX;
        if (index < 0 || index >= _ecosystemLayer.Length) return 0;
        return _ecosystemLayer[gridY * LogicX + gridX];
    }
    public byte GetEventAreaLayer(int i)
    {
        if (null == _eventAreaLayer) return byte.MinValue;
        if (0 > i) return byte.MinValue;
        if (_eventAreaLayer.Length <= i) return byte.MinValue;

        return _eventAreaLayer[i];
    }

    public int          LogicX
    {
        get { return _LogicXmax - _LogicXmin; }
    }

    public int          LogicZ
    {
        get { return _LogicZmax - _LogicZmin; }
    }

    public string GetLutTexPath()
    {
        return _LutTexPath;
    }
    
    public string GetLightmapTexPath()
    {
        return _LightmapTexPath;
    }

    public Vector4 GetLightmapPosition()
    {
        return _LightmapPosition;
    }

    public DEntityInfo[] _entityinfo            = new DEntityInfo[0];
    public byte[] _blocklayer                   = new byte[0];
    public DNPCInfo[] _npcinfo                  = new DNPCInfo[0];
    public DMonsterInfo[] _monsterinfo          = new DMonsterInfo[0];
    public DDecoratorInfo[] _decoratorinfo      = new DDecoratorInfo[0];
    public DDestructibleInfo[] _desructibleinfo = new DDestructibleInfo[0];
    public DRegionInfo[] _regioninfo            = new DRegionInfo[0];
    public DTransportDoor[] _transportdoor      = new DTransportDoor[0];
    public DEntityInfo _birthposition           = null;
    public DEntityInfo _hellbirthposition       = null;
    public DTownDoor[] _townDoor                = new DTownDoor[0];
    public FunctionPrefab[] _FunctionPrefab     = new FunctionPrefab[0];
    public DResourceInfo[] _resourcePosition = new DResourceInfo[0];
    public DTransferInfo[] _fighterBornPosition = new DTransferInfo[0];
    public DEcosystemResourceInfo[] _ecosystemResoucePosition = new DEcosystemResourceInfo[0];

#if UNITY_EDITOR
    [System.NonSerialized]
     public AnimVector3 animCameraPostion = new AnimVector3();
#endif
    public ushort[] _grasslayer = new ushort[0];
    public ushort[] _ecosystemLayer = new ushort[0];    //生态系统
    public byte[] _eventAreaLayer = new byte[0];        //事件触发区域
}

#if UNITY_EDITOR 
public class DSceneEntitySelection : ScriptableObject
{
    public delegate void OnRemoveEntityGameObject(object obj);

    public static OnRemoveEntityGameObject onRemoveEntity;

    public delegate void OnRenameEntityInfo(DEntityInfo info);

    public delegate void OnMarkDirty();

    static public OnRenameEntityInfo onRenameEntityInfo;
    static public OnMarkDirty        onMarkDirty;

    public static void MarkDirty()
    {
        if( onMarkDirty != null )
        {
            onMarkDirty();
        }
    }

    public static void OnRemoveEntity(object obj)
    {
        if(onRemoveEntity != null)
        {
            onRemoveEntity(obj);
        }
    }

    //private DEntityInfo _SelectedObject;

    private DEntityInfo[] _SelectedObjects;
    public DEntityInfo[] SelectedObjects
    {
        get
        {
            return _SelectedObjects;
        }
    }

    private static DSceneEntitySelection _Instance;

    public static DSceneEntitySelection Instance
    {
        get {
            if (_Instance == null)
            {
                _Instance = UnityEngine.ScriptableObject.CreateInstance<DSceneEntitySelection>();
                _Instance.name = "SceneEntityInfo";
            }

            return _Instance;
        }
    }
/*    public static void Select(DEntityInfo p)
    {
        _Instance = Instance;
        _Instance._SelectedObject = p;
        if (p != null)
        {
            _Instance.name = p.name;
        }

        if (p != null)
            UnityEditor.Selection.activeObject = _Instance;
        else
            UnityEditor.Selection.activeObject = null;
    }*/

    public static void Select(DEntityInfo[] infos)
    {
        _Instance = Instance;
        _Instance._SelectedObjects = infos;

        /*if (infos != null)
        {
            _Instance.name = p.name;
        }*/

        if (infos != null && infos.Length > 0)
            UnityEditor.Selection.activeObject = _Instance;
        else
            UnityEditor.Selection.activeObject = null;
    }
    
    public static bool IsSelected(DEntityInfo p)
    {
        if (_Instance != null && _Instance._SelectedObjects != null)
        {
            foreach (var selected in _Instance._SelectedObjects)
            {
                if (selected == p)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public static DEntityInfo[] GetSelected()
    {
        if (_Instance != null)
            return _Instance._SelectedObjects;
        else return null;
    }
}

#endif

public class EnumMapIDAttribute : Attribute
{
    public int id;
    public EnumMapIDAttribute(int id)
    {
        this.id = id;
    }
}

public static class DHelper
{
    public static string GetDescription(this System.Enum value, bool nameInstead = true)  
    {
        System.Type type = value.GetType();
        string name = System.Enum.GetName(type, value);
        if (name == null)
        {
            return null;
        }

        FieldInfo field = type.GetField(name);
        DescriptionAttribute attribute = System.Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;

        if (attribute == null && nameInstead == true)
        {
            return name;
        }
        return attribute == null ? null : attribute.Description;
    }

//     public static int GetMapID(this System.Enum value)
//     {
//         System.Type type = value.GetType();
//         string name = System.Enum.GetName(type, value);
//         if (name == null)
//         {
//             return -1;
//         }
//         FieldInfo field = type.GetField(name);
//         EnumMapIDAttribute attribute = System.Attribute.GetCustomAttribute(field, typeof(EnumMapIDAttribute)) as EnumMapIDAttribute;
//         return attribute == null ? (int)-1 : attribute.id;
//     }

    public static string[] GetDescriptions(this System.Type type)
    {
        string[] names = System.Enum.GetNames(type);
        if (names == null)
        {
            return null;
        }

        for(int i = 0; i < names.Length; ++i)
        {
            FieldInfo field = type.GetField(names[i]);
            DescriptionAttribute attribute = System.Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;

            if (attribute != null)
            {
                names[i] = attribute.Description;
            }
        }

        return names;
    }

    public static System.Array GetValues(this System.Type type)
    {
        return System.Enum.GetValues(type);
    }
}

