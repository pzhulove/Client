using ProtoTable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 替换模型机制
/// </summary>
public class Mechanism1021 : BeMechanism
{
    int resID = 0;
    int curWeaponItemID = 0;
    public Mechanism1021(int sid, int skillLevel) : base(sid, skillLevel)
    {
    }

    public override void OnInit()
    {
        base.OnInit();
        resID = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
    }

    public override void OnStart()
    {
        curWeaponItemID = owner.GetWeaponID();
#if !LOGIC_SERVER
        base.OnStart();
        ResTable resData = TableManager.instance.GetTableItem<ResTable>(resID);
        if (resData == null)
            return;
        JobTable jobData = TableManager.instance.GetTableItem<JobTable>(owner.professionID);
        if (jobData == null)
            return;
        if (owner.attachmentproxy != null)
        {
            owner.attachmentproxy.SetShowFashionWeapon(resData.ModelPath, jobData.DefaultWeaponPath);
            owner.attachmentproxy.Update(0);
        }

#endif
    }

    public override void OnFinish()
    {
        base.OnFinish();
        ResetWeaponModel();
    }

    public override void OnDead()
    {
        base.OnDead();
        ResetWeaponModel();
    }

    private void ResetWeaponModel()
    {
#if !LOGIC_SERVER
        JobTable jobData = TableManager.instance.GetTableItem<JobTable>(owner.professionID);
        if (jobData == null)
            return;
        ItemTable itemData = TableManager.instance.GetTableItem<ItemTable>(curWeaponItemID);
        if (owner.attachmentproxy != null)
        {

            if (owner.m_cpkCurEntityActionInfo != null)
            {
                var acActionName = owner.m_cpkEntityInfo.GetCurrentActionName();
                if (owner.m_cpkEntityInfo.HasAction(acActionName))
                {
                    owner.m_cpkCurEntityActionInfo = owner.m_cpkEntityInfo._vkActionsMap[acActionName];

                    owner.attachmentproxy.Init(owner.m_cpkCurEntityActionInfo);
                }
            }
            if (itemData == null)
            {
                owner.attachmentproxy.SetShowFashionWeapon(jobData.DefaultWeaponPath, jobData.DefaultWeaponPath);
                owner.attachmentproxy.Update(0);
                return;
            }
            ResTable resData = TableManager.instance.GetTableItem<ResTable>(itemData.ResID);
            if (resData == null)
                return;
            owner.attachmentproxy.SetShowFashionWeapon(resData.ModelPath, jobData.DefaultWeaponPath);
            owner.attachmentproxy.Update(0);
        }

#endif
    }
}
