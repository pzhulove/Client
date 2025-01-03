using System.Collections;
using System.Collections.Generic;
using GameClient;
using UnityEngine;

/// <summary>
/// 全场景随机找点 同时保证每个点半径范围内不重叠
/// </summary>
public class Mechanism1063 : BeMechanism
{
    private struct EntityData
    {
        public int entityId;    //光剑实体ID
        public int num;     //光剑实体数量
        public int maxHitCount; //最大攻击次数
    }

    public Mechanism1063(int mid, int lv) : base(mid, lv) { }
    
    private int time = 2000;   //召唤时间间隔
    private VInt checkDis = 10000;   //检查位置的直径距离

    private readonly int whileCount = 100;  //最大循环次数
    private VInt forceZHeight = 0;  //强制Z轴高度

    private List<EntityData> entityList = new List<EntityData>();

    private Dictionary<int, VInt3> entityDataDic = new Dictionary<int, VInt3>();

    public override void OnInit()
    {
        base.OnInit();
        for(int i = 0; i < data.ValueA.Count; i++)
        {
            var entityData = new EntityData();
            entityData.entityId = TableManager.GetValueFromUnionCell(data.ValueA[i], level);
            entityData.num = TableManager.GetValueFromUnionCell(data.ValueC[i], level);
            entityData.maxHitCount = TableManager.GetValueFromUnionCell(data.ValueF[i], level);

            entityList.Add(entityData);
        }
        time = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
        checkDis = VInt.NewVInt(TableManager.GetValueFromUnionCell(data.ValueD[0], level), GlobalLogic.VALUE_1000);
        forceZHeight = VInt.NewVInt(TableManager.GetValueFromUnionCell(data.ValueE[0], level), GlobalLogic.VALUE_1000);
    }

    public override void OnReset()
    {
        entityList.Clear();
        entityDataDic.Clear();
    }

    public override void OnStart()
    {
        base.OnStart();

        if (owner.CurrentBeScene != null)
        {
            sceneHandleA = owner.CurrentBeScene.RegisterEventNew(BeEventSceneType.onEntityRemove, RegisterEntityRemove);
        }

        InitTimeAcc(time);
    }

    public override void OnUpdateTimeAcc()
    {
        base.OnUpdateTimeAcc();
        AddEntity();
    }

    /// <summary>
    /// 监听实体消失
    /// </summary>
    private void RegisterEntityRemove(BeEvent.BeEventParam args)
    {
        var entity = args.m_Obj as BeEntity;
        if (entity == null)
            return;
        if (!CheckEntityResId(entity.m_iResID))
            return;
        if (!entityDataDic.ContainsKey(entity.GetPID()))
            return;
        entityDataDic.Remove(entity.GetPID());
    }

    /// <summary>
    /// 创建实体
    /// </summary>
    private void AddEntity()
    {
        for(int i = 0; i < entityList.Count; i++)
        {
            var entityData = entityList[i];
            for(int j = 0; j < entityData.num; j++)
            {
                AddEntitySingle(entityData);
            }
        }
    }

    /// <summary>
    /// d创建单个实体
    /// </summary>
    private void AddEntitySingle(EntityData data)
    {
        int curNum = 0;
        do
        {
            curNum++;
            VInt3 targetPos = owner.CurrentBeScene.GetRandomPos(20);
            if (!CheckDis(targetPos))
                continue;
            if (forceZHeight != 0)
            {
                targetPos.z = forceZHeight.i;
            }
            var projectile = owner.AddEntity(data.entityId, targetPos, level) as BeProjectile;
            if (projectile != null && !entityDataDic.ContainsKey(projectile.GetPID()))
            {
                projectile.totoalHitCount = data.maxHitCount;
                entityDataDic.Add(projectile.GetPID(), targetPos);
                break;
            }
        }
        while (curNum < whileCount);
    }

    /// <summary>
    /// 判断当前坐标能否创建实体
    /// </summary>
    /// <returns></returns>
    private bool CheckDis(VInt3 targetPos)
    {
        Dictionary<int, VInt3>.Enumerator enumerator = entityDataDic.GetEnumerator();
        while (enumerator.MoveNext())
        {
            VInt3 pos = enumerator.Current.Value;
            if ((pos - targetPos).magnitude < checkDis)
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// 检测实体是否是自己召唤的
    /// </summary>
    /// <returns></returns>
    private bool CheckEntityResId(int resId)
    {
        for(int i=0;i< entityList.Count; i++)
        {
            if (entityList[i].entityId == resId)
            {
                return true;
            }   
        }
        return false;
    }
}
