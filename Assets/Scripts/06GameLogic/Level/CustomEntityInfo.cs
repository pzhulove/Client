using System;
using UnityEngine;
public class CustomSceneRegionInfo : ISceneEntityInfoData, ISceneRegionInfoData
{
    private int mResId = 0;
    private VInt3 mPos;
    private int mGlobalId = 0;
    public string GetModelPathByResID()
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
    public VInt3 GetLogicPosition()
    {
        return mPos;
    }

    public CustomSceneRegionInfo(int resId,VInt3 pos,int globalId)
    {
        mResId = resId;
        mPos = pos;
        mGlobalId = globalId;
    }

    public ISceneEntityInfoData GetEntityInfo()
    {
        return this;
    }

    public float GetRadius()
    {
        return 1.0f;
    }

    public Vector2 GetRect()
    {
        return new Vector2(
            1,
            1
            );
    }

    public DRegionInfo.RegionType GetRegiontype()
    {
        return DRegionInfo.RegionType.Circle;
    }

    public Quaternion GetRotation()
    {
        return new Quaternion(
            1,
            1,
            1,
            1
            );
    }

    public void SetRadius(float r)
    {
        return;
    }

    public void SetRegiontype(DRegionInfo.RegionType type)
    {
        return;
    }
    public int GetGlobalid()
    {
        return mGlobalId;
    }

    public int GetResid()
    {
        return mResId;
    }

    public string GetName()
    {
        return string.Empty;
    }

    public string GetPath()
    {
        return string.Empty;
    }

    public string GetDescription()
    {
        return string.Empty;
    }


    public DEntityType GetType()
    {
        return DEntityType.REGION;
    }

    public string GetTypename()
    {
        return string.Empty;
    }

    public Vector3 GetPosition()
    {
        return mPos.vector3;
    }

    public float GetScale()
    {
        return 1.0f;
    }

    public Color GetColor()
    {
        return Color.white;
    }
}

