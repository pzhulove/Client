using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 武斗家觉醒被动
/// </summary>
public class Mechanism2031 : BeMechanism
{
    private int buffInfoID = 0;
    private int timeGap = 800;
    private int _shadowLiftTime;
    private bool flag = false;
    private int shadowInterval = 200;
    private int[] skillIDs;

    private int buffId = 0; //添加的增加智力的BuffId
    private int _addSpeedBuffId;  //添加移速的BuffId

#if !LOGIC_SERVER
    private int posIndex = 0;
    private Vec3[] effectPosArr = new Vec3[4];
#endif

    public Mechanism2031(int mid, int lv) : base(mid, lv) { }

    public override void OnInit()
    {
        base.OnInit();
        timeGap = TableManager.GetValueFromUnionCell(data.ValueA[0], level);

        buffInfoID = TableManager.GetValueFromUnionCell(data.ValueB[0], level);

        _shadowLiftTime = TableManager.GetValueFromUnionCell(data.ValueC[0], level);
        shadowInterval = TableManager.GetValueFromUnionCell(data.ValueD[0], level);
        skillIDs = new int[data.ValueE.Count];
        for (int i = 0; i < skillIDs.Length; i++)
        {
            skillIDs[i] = TableManager.GetValueFromUnionCell(data.ValueE[i], level);
        }
        _addSpeedBuffId = TableManager.GetValueFromUnionCell(data.ValueF[0], level);
    }

    public override void OnReset()
    {
        flag = false;
        buffId = 0;

#if !LOGIC_SERVER
        posIndex = 0;
        effectPosArr = new Vec3[4];
#endif

        timer = 0;
        shadowTime = 0;
}

private int tmpBuffInfoID;
    public override void OnStart()
    {
        base.OnStart();

        InitBuffId();

        handleA = owner.RegisterEventNew(BeEventType.onCastSkill, (args) =>
        {
            AddBuff(args.m_Int);
        });

        handleB = owner.RegisterEventNew(BeEventType.onRemoveBuff, (args) =>
        {
            int id = (int)args.m_Int;
            if(buffId == id)
            {
                RemoveBuff();
            }
        });
    }

    private void InitBuffId()
    {
        var buffInfoTable = TableManager.instance.GetTableItem<ProtoTable.BuffInfoTable>(buffInfoID);
        if (buffInfoTable == null)
            return;
        buffId = buffInfoTable.BuffID;
    }

    /// <summary>
    /// 添加Buff
    /// </summary>
    private void AddBuff(int skillId)
    {
        if (!flag)
            return;
        if (Array.IndexOf(skillIDs, skillId) == -1)
            return;
        owner.buffController.TryAddBuffInfo(buffInfoID,owner,level);
    }

    /// <summary>
    /// 移除添加的Buff
    /// </summary>
    private void RemoveBuff()
    {
        // ClearSnapEffect();
        // AddEffect();
        timer = 0;
        flag = false;
    }

    #region 创建残影相关
    private int timer = 0;
    private int shadowTime = 0;

    public override void OnUpdate(int deltaTime)
    {
        base.OnUpdate(deltaTime);
        if (owner.sgGetCurrentState() == (int)ActionState.AS_RUN || owner.sgGetCurrentState() == (int)ActionState.AS_WALK)
        {
            timer += deltaTime;
            if (timer >= timeGap)
            {
                flag = true;
                timer = 0;
            }
        }

        if (flag)
        {
            shadowTime += deltaTime;

            if (shadowTime >= shadowInterval)
            {
                _AddSpeedBuff();
                // CreateSnapEffect();
                shadowTime = 0;
            }
        }
    }

    private void _AddSpeedBuff()
    {
        if (_addSpeedBuffId > 0)
            owner.buffController.TryAddBuff(_addSpeedBuffId, _shadowLiftTime, level);
    }

    private void CreateSnapEffect()
    {
#if !LOGIC_SERVER
        VInt3 pos = owner.GetPosition();
        pos.z = owner.GetPosition().z + 10000;
        AddEffectPos(pos.vec3);
        owner.m_pkGeActor.CreateSnapshot(Color.white, _shadowLiftTime / 1000.0f, "Shader/Materials/Snapshot_Sanda_Beidongjuexing.mat");
#endif
    }

    private void ClearSnapEffect()
    {
#if !LOGIC_SERVER
        owner.m_pkGeActor.DestroySnapEffect();
#endif
    }

    private void AddEffectPos(Vec3 pos)
    {
#if !LOGIC_SERVER
        if (posIndex >= effectPosArr.Length)
            posIndex = 0;
        effectPosArr[posIndex] = pos;
        posIndex++;
#endif
    }

    /// <summary>
    /// 创建影子消失特效
    /// </summary>
    private void AddEffect()
    {
#if !LOGIC_SERVER
        for(int i = 0; i< effectPosArr.Length; i++)
        {
            if(effectPosArr[i] != Vec3.zero)
            {
                //影子消散特效
                owner.CurrentBeScene.currentGeScene.CreateEffect(1021, effectPosArr[i]);
                effectPosArr[i] = Vec3.zero;
            }
        }
        posIndex = 0;
#endif
    }
#endregion
}
