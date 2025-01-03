using System.Collections;
using System.Collections.Generic;
using GameClient;
using UnityEngine;

/// <summary>
/// 团本-霏雨剑机制
/// </summary>
public class Mechanism2063 : BeMechanism
{
    public Mechanism2063(int mid, int lv) : base(mid, lv) { }

    private List<int> swordEntityIdList = new List<int>();  //光剑实体ID
    private int lightEntityId = 0;  //光线实体ID
    private int useSkillId = 0; //释放全屏伤害技能ID
    private int completeCount = 6;  //完成数量

#if !LOGIC_SERVER
    private float rotateRadius = 1; //光剑特效旋转半径
    private float rotateSpeed = 20; //光剑特效旋转速度
    private List<GeEffectEx> swordEffectList = new List<GeEffectEx>(6);       //光剑特效数组
#endif
    private int curCompleteCount = 0;
    private bool isOver = false;    //这一轮计数是否已经结束
    private int lightEntitySpeed = 5000;   //光线实体移动的速度
    private Transform attachTrans = null;

    private int mUpdateInterval = 70;
    private int mUpdateTimeAcc = 0;

    public override void OnInit()
    {
        base.OnInit();
        for (int i = 0; i < data.ValueA.Count; i++)
        {
            swordEntityIdList.Add(TableManager.GetValueFromUnionCell(data.ValueA[i], level));
        }
        lightEntityId = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
        useSkillId = TableManager.GetValueFromUnionCell(data.ValueC[0], level);
        completeCount = TableManager.GetValueFromUnionCell(data.ValueD[0], level);
    }

    public override void OnReset()
    {
        swordEntityIdList.Clear();

#if !LOGIC_SERVER
        swordEffectList.Clear();
#endif

        curCompleteCount = 0;
        isOver = false;
        attachTrans = null;
        mUpdateTimeAcc = 0;
    }

public override void OnStart()
    {
        base.OnStart();
        if (owner.CurrentBeScene == null)
            return;
        curCompleteCount = 0;
        mUpdateTimeAcc = 0;
        
        sceneHandleA = owner.CurrentBeScene.RegisterEventNew(BeEventSceneType.onAddEntity, RegisterEntityCreate);
        
#if !LOGIC_SERVER
        // 刷新挂节点，修复变身后挂节点不刷新bug
        GetAttachObj();
        handleB = owner.RegisterEventNew(BeEventType.onChangeModelFinish, GetAttachObj);
#endif
    }

    private void GetAttachObj(BeEvent.BeEventParam args = null)
    {
        attachTrans = null;
        if (owner == null || owner.m_pkGeActor == null)
            return;
        
        attachTrans = owner.m_pkGeActor.GetAttachNode("[actor]OrignBuff").transform;
    }
    
    public override void OnUpdateGraphic(int deltaTime)
    {
        base.OnUpdateGraphic(deltaTime);
        
        UpdateSwordEffect(deltaTime);
    }

    /// <summary>
    /// 监听光剑怪物创建
    /// </summary>
    private void RegisterEntityCreate(BeEvent.BeEventParam args)
    {
        var entity = args.m_Obj as BeEntity;
        if (entity == null)
            return;
        if (!swordEntityIdList.Contains(entity.m_iResID))
            return;
        //handleA = entity.RegisterEvent(BeEventType.onHitOther, (object[] args1) =>
        // {

        // });
        handleA = entity.RegisterEventNew(BeEventType.onHitOther, _OnHitOther);
    }

    private void _OnHitOther(GameClient.BeEvent.BeEventParam param)
    {
        if (isOver)
            return;
        var target = param.m_Obj as BeActor;
        if (target == null)
            return;
        if (target.professionID == 0)
            return;

        //创建光线实体
        CreateLightEntity(target);
        curCompleteCount++;
        AddSwordEffect();
        if (curCompleteCount >= completeCount)
        {
            CounterOver();
        }
    }

    /// <summary>
    /// 创建光线实体
    /// </summary>
    private void CreateLightEntity(BeActor player)
    {
        VInt3 playerPos = player.GetPosition();
        playerPos.z = GlobalLogic.VALUE_10000;
        var projectile = owner.AddEntity(lightEntityId, playerPos) as BeProjectile;
        if (projectile == null)
            return;
        //设置实体运行轨迹
        //var trail = new FollowTarget();
        //trail.StartPoint = projectile.GetPosition();
        //projectile.ClearMoveSpeed();
        //var targetPos = owner.GetPosition();
        //targetPos.z = trail.StartPoint.z;
        //trail.EndPoint = targetPos;
        //trail.MoveSpeed = lightEntitySpeed;
        //trail.nearReove = true;
        //trail.Init();
        //projectile.trail = trail;
        //trail.owner = projectile;
        //trail.target = owner;

        var trail = owner.CurrentBeBattle.LogicTrailManager.AddFollowTargetTrail(owner, projectile, lightEntitySpeed, true);
        projectile.logicTrail = trail;
    }

    /// <summary>
    /// 创建光剑特效
    /// </summary>
    private void AddSwordEffect()
    {
#if !LOGIC_SERVER
        Vec3 pos = Vec3.zero;
        var effect = owner.m_pkGeActor.CreateEffect(1010, pos);
        if (effect != null)
        {
            Battle.GeUtility.AttachTo(effect.GetRootNode(), owner.m_pkGeActor.GetEntityNode(GeEntity.GeEntityNodeType.Transform));
            swordEffectList.Add(effect);
            AdjustEffectPos();
        }
#endif
    }

    /// <summary>
    /// 调整特效的坐标
    /// </summary>
    private void AdjustEffectPos()
    {
#if !LOGIC_SERVER
        float averageAngle = 360.0f / swordEffectList.Count;
        for (int i = 0; i < swordEffectList.Count; i++)
        {
            float curAngle = averageAngle * i;
            float x = rotateRadius * Mathf.Cos(curAngle * 3.14f / 180);
            float z = rotateRadius * Mathf.Sin(curAngle * 3.14f / 180);
            swordEffectList[i].SetLocalPosition(new Vector3(x, 1.5f, z));
        }
#endif
    }

    /// <summary>
    /// 旋转特效
    /// </summary>
    private void UpdateSwordEffect(int deltaTime)
    {
#if !LOGIC_SERVER
        if (attachTrans == null)
            return;
        
        mUpdateTimeAcc += deltaTime;
        if (mUpdateTimeAcc <= mUpdateInterval)
        {
            return;
        }
        mUpdateTimeAcc -= mUpdateInterval;

        for (int i = 0; i < swordEffectList.Count; i++)
        {
            swordEffectList[i].GetRootNode().transform.RotateAround(attachTrans.position, attachTrans.forward, rotateSpeed * mUpdateInterval * 0.001f);
        }
#endif
    }

    /// <summary>
    /// 计数达成
    /// </summary>
    private void CounterOver()
    {
        isOver = true;
        owner.delayCaller.DelayCall(2000, () =>
        {
            if (!owner.IsDead())
            {
                isOver = false;
                curCompleteCount = 0;
                ClearAllEffect();
                owner.UseSkill(useSkillId, true);
            }
        });
    }

    /// <summary>
    /// 清除光剑特效
    /// </summary>
    private void ClearAllEffect()
    {
#if !LOGIC_SERVER
        for (int i = 0; i < swordEffectList.Count; i++)
        {
            owner.m_pkGeActor.DestroyEffect(swordEffectList[i]);
        }
        swordEffectList.Clear();
#endif
    }
}
