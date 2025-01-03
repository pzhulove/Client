using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 秘术师-天雷
/// </summary>
public class Skill2208 : BeSkill
{
    protected int m_AutoTimeAcc = 2000;                 //天雷自动释放时间间隔
    protected int m_CreateNumMax = 5;                   //天雷释放次数
    protected int m_ClickTimeAcc = 500;                //天雷手动点击释放时间间隔

    protected int m_RayEntityId = 60300;                //天雷实体ID
    protected int m_RayChargeEntityId = 60301;          //天雷蓄力实体ID
    protected int m_CurrAutoTimeAcc = 0;                //当前自动释放时间间隔
    protected int m_CurrCreateNum = 0;                  //当前实体创建数量
    protected bool m_CreateRayFlag = false;

    protected int m_CurrClickTimeAcc = 0;               //当前手动释放时间间隔
    protected bool m_CanManual = false;                 //能否手动释放

    protected bool m_ShowSpeelBar = false;              //显示读条

    public Skill2208(int sid, int skillLevel) : base(sid, skillLevel)
    {

    }

    public override void OnInit()
    {
        m_AutoTimeAcc = TableManager.GetValueFromUnionCell(skillData.ValueA[0], level);
        m_CreateNumMax = TableManager.GetValueFromUnionCell(skillData.ValueB[0], level);
        if (skillData.ValueC.Count > 1)
        {
            m_ClickTimeAcc =  !BattleMain.IsModePvP(battleType) ? TableManager.GetValueFromUnionCell(skillData.ValueC[0],level) :TableManager.GetValueFromUnionCell(skillData.ValueC[1],level);
        }
        else
        {
            m_ClickTimeAcc = TableManager.GetValueFromUnionCell(skillData.ValueC[0], level);
        }
        
        int manual = TableManager.GetValueFromUnionCell(skillData.ValueD[0], level);
        m_CanManual = manual == 0 ? false : true;                                         //0表示不能手动释放 1表示能手动释放

        if (skillData.ValueE.Count > 1)
        {
            m_RayEntityId = !BattleMain.IsModePvP(battleType) ? TableManager.GetValueFromUnionCell(skillData.ValueE[0], level) : TableManager.GetValueFromUnionCell(skillData.ValueE[1], level);
        }

        if(skillData.ValueF.Count > 1)
        {
            m_RayChargeEntityId = !BattleMain.IsModePvP(battleType) ? TableManager.GetValueFromUnionCell(skillData.ValueF[0], level) : TableManager.GetValueFromUnionCell(skillData.ValueF[1], level);
        }
    }

    public override void OnStart()
    {
        base.OnStart();
        InitData();
    }

    public override void OnUpdate(int iDeltime)
    {
        if (m_CreateRayFlag)
        {
            if (m_CurrCreateNum < m_CreateNumMax)
            {
                if (m_CurrAutoTimeAcc > 0)
                {
                    if (!m_ShowSpeelBar)
                    {
                        m_ShowSpeelBar = true;
                        if (owner.isLocalActor)
                        {
                            owner.StartSpellBar(eDungeonCharactorBar.RayDrop, m_AutoTimeAcc, true);
                        }
                    }
                    m_CurrAutoTimeAcc -= iDeltime;
                }
                else
                {
                    CreateRayEntity();
                }

                if (m_CanManual)
                {
                    //手动释放CD
                    if (m_CurrClickTimeAcc > 0)
                    {
                        m_CurrClickTimeAcc -= iDeltime;
                    }
                    else
                    {
                        m_CurrClickTimeAcc = 0;
                    }
                }
            }
        }
    }

    public override void OnReleaseJoystick()
    {
        if (m_CreateRayFlag && m_CanManual)
        {
            if (m_CurrCreateNum < m_CreateNumMax && m_CurrClickTimeAcc == 0)
            {
                CreateRayEntity();
            }
        }
    }

    public override void OnCancel()
    {
        End();
    }

    public override void OnFinish()
    {
        End();
    }

    protected void End()
    {
        m_CreateRayFlag = false;
        canRemoveJoystick = true;
        StartCoolDown();
        SetInnerState(InnerState.LAUNCH);
        skillButtonState = SkillState.NORMAL;
    }

    protected void InitData()
    {
        m_ShowSpeelBar = false;
        m_CreateRayFlag = true;
        canRemoveJoystick = false;


        m_CurrCreateNum = 0;
        m_CurrAutoTimeAcc = m_AutoTimeAcc;
        m_CurrClickTimeAcc = m_ClickTimeAcc;
    }

    //创建天雷
    protected void CreateRayEntity()
    {
        m_CurrAutoTimeAcc = m_AutoTimeAcc;
        m_CurrClickTimeAcc = m_ClickTimeAcc;
        m_CurrCreateNum++;
        if (owner.HasAction("Tianlei_04"))
        {
            owner.PlayAction("Tianlei_04");
        }
        int rayEntityId = charged ? m_RayChargeEntityId : m_RayEntityId;
        VInt3 createPos = effectPos;
        owner.AddEntity(rayEntityId, createPos, level);
        if (m_CurrCreateNum == m_CreateNumMax)
        {
            if (owner.isLocalActor)
            {
                owner.StopSpellBar(eDungeonCharactorBar.RayDrop);
            }
            owner.Locomote(new BeStateData((int)ActionState.AS_IDLE), true);
            Cancel();
            //移除膝撞霸体
            if (owner.buffController.HasBuffByID(1)!=null)
            {
                owner.buffController.RemoveBuff(1);
            }
        }
        else
        {
            m_ShowSpeelBar = false;
        }
    }
}
