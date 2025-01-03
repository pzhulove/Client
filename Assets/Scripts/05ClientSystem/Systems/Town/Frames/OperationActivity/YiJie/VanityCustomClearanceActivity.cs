using System;
using GameClient;
using Protocol;
using UnityEngine;

public class VanityCustomClearanceActivity : LimitTimeCommonActivity
{
    private OpActivityData mData;
    public sealed override void Init(uint activityId)
    {
        base.Init(activityId);
        if (mDataModel != null)
        {
            mDataModel.SortTaskByState();
        }
        mData = ActivityDataManager.GetInstance().GetLimitTimeActivityData(activityId);
    }

     public override bool IsHaveRedPoint()
    {
        return base.IsHaveRedPoint();
    }

    protected sealed override string _GetPrefabPath()
    {
        if (mData != null)
        {
            string path = mData.prefabPath;
            if (!string.IsNullOrEmpty(path))
            {
                return path;
            }
        }
        return "UIFlatten/Prefabs/OperateActivity/YiJie/Activities/VanityCustomClearanceActivity";
    }

   
    protected sealed override string _GetItemPrefabPath()
    {
        return "UIFlatten/Prefabs/OperateActivity/YiJie/Items/VanityDailyCustomClearanceRewardItem";
    }

   
}