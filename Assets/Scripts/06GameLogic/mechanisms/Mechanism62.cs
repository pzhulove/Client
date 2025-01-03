using UnityEngine;
using System.Collections.Generic;
using GameClient;

/*
 * 增加/减少某一实体的最大攻击次数
*/
public class Mechanism62 : BeMechanism
{
    protected List<int> m_EntityIds = new List<int>();                  //实体ID
    protected int m_AddHitCount = 0;                                    //增加的攻击次数
    protected int[] m_ModelScale = new int[2];                          //模型缩放千分比（Pve|Pvp）
    protected int[] m_ModelZDimScale = new int[2];                      //模型Z轴缩放千分比（Pve|Pvp）
    protected int m_AddTime = 0;                                        //增加实体持续时间
    protected int m_SpeedAddValue = 0;                                  //实体速度增加固定值
    protected int m_SpeedAddRate = 0;                                   //实体速度增加千分比           
    protected int[] m_DistAddRate = new int[2];                         //实体射程增加千分比

    public Mechanism62(int mid, int lv) : base(mid, lv) { }

    public override void OnReset()
    {
        m_EntityIds.Clear();
        m_AddHitCount = 0; 
        BeUtility.ResetIntArray(m_ModelScale);
        BeUtility.ResetIntArray(m_ModelZDimScale);
        m_AddTime = 0;
        m_SpeedAddValue = 0;  
        m_SpeedAddRate = 0;   
        BeUtility.ResetIntArray(m_DistAddRate);
    }
    public override void OnInit()
    {
        if (data.ValueA.Count > 0)
        {
            for(int i = 0; i < data.ValueA.Length;i++)
            {
                m_EntityIds.Add(TableManager.GetValueFromUnionCell(data.ValueA[i], level));
            }
        }
        if (data.ValueB.Count > 0)
        {
            m_AddHitCount = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
        }
        if (data.ValueC.Count > 1)
        {
            m_ModelScale[0] = TableManager.GetValueFromUnionCell(data.ValueC[0], level);
            m_ModelScale[1] = TableManager.GetValueFromUnionCell(data.ValueC[1], level);
        }
        if (data.ValueD.Count > 1)
        {
            m_ModelZDimScale[0] = TableManager.GetValueFromUnionCell(data.ValueD[0], level);
            m_ModelZDimScale[1] = TableManager.GetValueFromUnionCell(data.ValueD[1], level);
        }

        if (data.ValueE.Count > 0)
        {
            m_AddTime = TableManager.GetValueFromUnionCell(data.ValueE[0], level);
        }

        if (data.ValueF.Count > 0)
        {
            m_SpeedAddValue = TableManager.GetValueFromUnionCell(data.ValueF[0], level);
        }

        if(data.ValueG.Count > 0)
        {
            m_SpeedAddRate = TableManager.GetValueFromUnionCell(data.ValueG[0],level);
        }
        if(data.ValueH.Count > 1)
        {
            m_DistAddRate[0] = TableManager.GetValueFromUnionCell(data.ValueH[0], level);
            m_DistAddRate[1] = TableManager.GetValueFromUnionCell(data.ValueH[1], level);
        }
    }

    public override void OnStart()
    {
        base.OnStart();
        handleA = owner.RegisterEventNew(BeEventType.onAfterGenBullet, (args) =>
        {
            BeProjectile projectile = (BeProjectile)args.m_Obj;
            if (projectile != null&& m_EntityIds.Contains(projectile.m_iResID))
            {
                if (m_AddHitCount != 0)
                {
                    int curCount = projectile.totoalHitCount + m_AddHitCount;
                    if (curCount <= 0)
                        curCount = 1;
                    projectile.totoalHitCount = curCount;
                }

                int modelsScale = BattleMain.IsModePvP(battleType) ? m_ModelScale[1] : m_ModelScale[0];
                if (modelsScale != 0)
                {
                    ChangeModelScale(projectile, modelsScale);
                }
                
                int modelsZDimScale = BattleMain.IsModePvP(battleType) ? m_ModelZDimScale[1] : m_ModelZDimScale[0];
                if (modelsZDimScale != 0)
                {
                    ChangeModelZDimScale(projectile, modelsZDimScale);
                }
                var distFactor = new VFactor(BattleMain.IsModePvP(battleType) ? m_DistAddRate[1] : m_DistAddRate[0], GlobalLogic.VALUE_1000);
                projectile.distance = projectile.distance.i * (VFactor.one + distFactor);
                if (m_AddTime != 0)
                {
                    int lifeTime = projectile.m_fLifes + m_AddTime;
                    if (lifeTime <= 0)
                    {
                        projectile.m_fLifes = 1;            //如果时间小于0则强设时间为1ms
                    }
                    else
                    {
                        projectile.m_fLifes = lifeTime;
                    }
                }
            }
        });

        handleB = owner.RegisterEventNew(BeEventType.onChangeProjectileSpeed, args => 
        {
            BeProjectile projectile = (BeProjectile)args.m_Obj;
            if (projectile != null && m_EntityIds.Contains(projectile.m_iResID))
            {
                if(m_SpeedAddValue!=0 || m_SpeedAddRate != 0)
                {
                    args.m_Vint = ChangeProjectileSpeed(args.m_Vint, projectile);
                }
            }
        });
    }

    protected void ChangeModelScale(BeProjectile projectile,int scale)
    {
        if(projectile!=null && !projectile.IsDead())
        {
            VInt originalScale = projectile.GetScale();
            projectile.SetScale(originalScale.i + scale * 10);
        }
    }

    protected void ChangeModelZDimScale(BeProjectile projectile, int scale)
    {
        if (projectile != null && !projectile.IsDead())
        {
            VInt originalScale = projectile.GetZDimScaleFactor().vint;
            projectile.SetZDimScaleFactor(new VFactor(originalScale.i + scale * GlobalLogic.VALUE_10,GlobalLogic.VALUE_10000));                                    
        }
    }

    //改变实体速度
    protected VInt ChangeProjectileSpeed(VInt speed,BeProjectile projectile)
    {
        VInt ret = speed;
        if (m_SpeedAddValue != 0)
        {
            ret += m_SpeedAddValue;
        }

        if (m_SpeedAddRate != 0)
        {
            VFactor scale = new VFactor(m_SpeedAddRate, GlobalLogic.VALUE_1000);
            ret = ret.i * (VFactor.one + scale);
        }

#if UNITY_EDITOR
        if (ret < 0)
        {
            Logger.LogErrorFormat("=实体{0}速度缩减太多了，为负数了==", projectile.GetName());
        }
#endif
        
        return ret;
    }
}
