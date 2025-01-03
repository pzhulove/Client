using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//念气怪物继承念气师的分身技能
public class Skill9703 : Skill3008
{
    public Skill9703(int sid, int skillLevel): base(sid, skillLevel)
	{

    }
}

//气功师 - 分身
public class Skill3008 : BeSkill 
{

    protected int m_CloneNum = 2;           //克隆分身数量
    protected int[] m_CloneIdArr = new int[2];            //分身Id数组(PVE|PVP)
    protected int[] m_CloneRayIdArr = new int[2];         //雷分身Id(PVE|PVP)
    protected int m_BombEntityId = 0;       //爆炸实体Id
    protected int m_BuffId = 0;             //BuffId 用于判断召唤时机
    protected int m_RayCloneTime = 5000;    //分身存在时间

    protected int m_RaySkillId = 3109;      //幻影爆碎技能Id
    protected VInt3[] m_PosOffset1 = new VInt3[1] { new VInt3(1.1f, 0, 0) };
    protected VInt3[] m_PosOffset2 = new VInt3[2] { new VInt3(1.25f, 0, 0), new VInt3(-1.25f, 0, 0) };
    protected VInt3[] m_PosOffset3 = new VInt3[3] { new VInt3(1.25f, 0, 0), new VInt3(-0.75f, 1.1f, 0), new VInt3(-0.75f, -1.1f, 0) };
    protected VInt3[] m_PosOffset4 = new VInt3[4] { new VInt3(1.35f, 0, 0), new VInt3(-1.35f, 0, 0), new VInt3(0, 1.1f, 0), new VInt3(0, -1.1f, 0) };
    protected VInt3[] m_PosOffset5 = new VInt3[5] { new VInt3(1.4f, 0, 0), new VInt3(-1, -0.65f, 0), new VInt3(-1, 0.65f, 0), new VInt3(0.45f, -1.3f, 0), new VInt3(0.45f, 1.3f, 0) };
    protected VInt3[] m_PosOffset6 = new VInt3[6] { new VInt3(1.5f, 0, 0), new VInt3(-1.5f, 0, 0), new VInt3(0.65f, 1.1f, 0), new VInt3(0.65f, -1.1f, 0), new VInt3(-0.65f, 1.1f, 0), new VInt3(-0.65f, -1.1f, 0) };
    protected VInt3[] m_PosOffset7 = new VInt3[7] { new VInt3(1.7f, 0, 0), new VInt3(-0.3f, 1.4f, 0), new VInt3(-0.3f, -1.4f, 0), new VInt3(0.9f, -0.8f, 0), new VInt3(0.9f, 0.8f, 0), new VInt3(-1.3f, 0.6f, 0), new VInt3(-1.3f, -0.6f, 0)};
    protected VInt3[] m_PosOffset8 = new VInt3[8] { new VInt3(1.7f, 0, 0), new VInt3(0, 1.4f, 0), new VInt3(0, -1.4f, 0), new VInt3(1.1f, -0.8f, 0), new VInt3(1.1f, 0.8f, 0), new VInt3(-1.1f, 0.8f, 0), new VInt3(-1.1f, -0.8f, 0), new VInt3(-1.7f, 0, 0) };
    protected VInt3[] m_PosOffset9 = new VInt3[9] { new VInt3(1.9f, 0, 0), new VInt3(0.35f, 1.4f, 0), new VInt3(0.35f, -1.4f, 0), new VInt3(1.4f, 0.8f, 0), new VInt3(1.4f, -0.8f, 0), new VInt3(-1.55f, 0.5f, 0), new VInt3(-1.55f, -0.5f, 0), new VInt3(-0.7f, 1.05f, 0), new VInt3(-0.7f, -1.05f, 0) };
    protected VInt3[] m_PosOffset10 = new VInt3[10] { new VInt3(1.95f, 0, 0), new VInt3(-1.95f, 0, 0), new VInt3(0.5f, 1.4f, 0), new VInt3(0.5f, -1.4f, 0), new VInt3(1.4f, 0.8f, 0), new VInt3(1.4f, -0.8f, 0), new VInt3(-1.4f, 0.8f, 0), new VInt3(-1.4f, -0.8f, 0), new VInt3(-0.5f, 1.4f, 0), new VInt3(-0.5f, -1.4f, 0) };

    protected List<VInt3[]> m_RebornPosList = new List<VInt3[]>();
    protected bool m_CreateCloneFlag = false;
    protected int m_CloneMonsterId = 0;
    protected int m_CurrentRayCloneNum = 0;

    public Skill3008(int sid, int skillLevel): base(sid, skillLevel)
	{
        
	}

    public override void OnInit()
    {
        m_CloneIdArr[0] = TableManager.GetValueFromUnionCell(skillData.ValueB[0], level);
        m_CloneIdArr[1] = TableManager.GetValueFromUnionCell(skillData.ValueB[1], level);
        m_CloneRayIdArr[0] = TableManager.GetValueFromUnionCell(skillData.ValueC[0], level);
        m_CloneRayIdArr[1] = TableManager.GetValueFromUnionCell(skillData.ValueC[1], level);
        m_BombEntityId = TableManager.GetValueFromUnionCell(skillData.ValueD[0], level);
        m_BuffId = TableManager.GetValueFromUnionCell(skillData.ValueE[0], level);
        m_RayCloneTime = TableManager.GetValueFromUnionCell(skillData.ValueF[0],level);

        m_RebornPosList.Clear();
        m_RebornPosList.Add(m_PosOffset1);
        m_RebornPosList.Add(m_PosOffset2);
        m_RebornPosList.Add(m_PosOffset3);
        m_RebornPosList.Add(m_PosOffset4);
        m_RebornPosList.Add(m_PosOffset5);
        m_RebornPosList.Add(m_PosOffset6);
        m_RebornPosList.Add(m_PosOffset7);
        m_RebornPosList.Add(m_PosOffset8);
        m_RebornPosList.Add(m_PosOffset9);
        m_RebornPosList.Add(m_PosOffset10);

        if (owner != null && owner.CurrentBeBattle != null && owner.CurrentBeBattle.PkRaceType == (int)Protocol.RaceType.ChiJi)
        {
            m_CloneNum = TableManager.GetValueFromUnionCell(skillData.ValueG[0], level);
        }
        else
        {
            m_CloneNum = TableManager.GetValueFromUnionCell(skillData.ValueA[0], level);
        }
    }

    public override void OnPostInit()
    {
        if (owner != null && owner.CurrentBeBattle != null && owner.CurrentBeBattle.PkRaceType == (int)Protocol.RaceType.ChiJi)
        {
            m_CloneNum = TableManager.GetValueFromUnionCell(skillData.ValueG[0], level);
        }
        else
        {
            m_CloneNum = TableManager.GetValueFromUnionCell(skillData.ValueA[0], level);
        }
        if (owner.HasSkill(m_RaySkillId) && !BattleMain.IsModePvP(battleType) && !owner.IsMonster())            //念气怪物不处理按钮
        {
            pressMode = SkillPressMode.TWO_PRESS_OUT;
        }
    }

    public override void OnStart ()
	{
        m_CreateCloneFlag = true;
        owner.delayCaller.DelayCall(m_RayCloneTime, () => 
        {
            List<BeActor> monsterList = new List<BeActor>();
            owner.CurrentBeScene.FindActorById(monsterList, m_CloneMonsterId);
            for (int i = 0; i < monsterList.Count; i++)
            {
                monsterList[i].DoDead();
            }
        });
	}

    public override void OnUpdate(int iDeltime)
    {
        if (owner.buffController.HasBuffByID(m_BuffId) != null && m_CreateCloneFlag)
        {
            m_CreateCloneFlag = false;
            owner.buffController.RemoveBuff(m_BuffId);
            CreateCloneMonster();
        }
    }

    //创建克隆分身并且设置位置
    protected void CreateCloneMonster()
    {
        int monsterId = 0;
        if (!owner.HasSkill(m_RaySkillId) && !owner.IsMonster())                //念气怪物只会创建雷分身
        {
            monsterId = BattleMain.IsModePvP(battleType)? m_CloneIdArr[1]: m_CloneIdArr[0];
        }
        else
        {
            monsterId = BattleMain.IsModePvP(battleType) ? m_CloneRayIdArr[1] : m_CloneRayIdArr[0];
        }

        List<BeActor> cloneMonsterList = new List<BeActor>();
        if (!BattleMain.IsModePvP(battleType))
        {
            m_CurrentRayCloneNum = m_CloneNum;
        }
        for (int i = 0; i < m_CloneNum; i++)
        {
            BeActor cloneMonster = owner.CurrentBeScene.CreateMonster(monsterId+level*100, true, null, level, owner.GetCamp(), owner);
            Buff12 lifeTimeBuff = cloneMonster.buffController.HasBuffByID((int)GlobalBuff.LIFE_TIME) as Buff12;
            if (lifeTimeBuff != null)
            {
                lifeTimeBuff.showDisappearEffect = false;
            }
            cloneMonster.SetOwner(owner);
            cloneMonster.SetRestrainPosition(false);
            cloneMonster.stateController.SetAbilityEnable(BeAbilityType.BLOCK, false);
            VInt3 ownerPos = owner.GetPosition();
            VInt3[] pos = m_RebornPosList[m_CloneNum - 1];
            int xOffset = owner.GetFace() ? -pos[i].x : pos[i].x;
            cloneMonster.SetPosition(new VInt3(ownerPos.x + xOffset, ownerPos.y + pos[i].y, ownerPos.z + pos[i].z));
            cloneMonster.SetFace(owner.GetFace(),true,true);
            cloneMonsterList.Add(cloneMonster);
            if (!BattleMain.IsModePvP(battleType))
            {
                var handle = cloneMonster.RegisterEventNew(BeEventType.onDead, eventParam =>
                {
                    RayCloneDead();
                });
            }
        }
    }

    //监听雷分身死亡
    protected void RayCloneDead()
    {
        m_CurrentRayCloneNum--;
        if (m_CurrentRayCloneNum <= 0)
        {
            ResetButtonEffect(); 
        }
    }

    public override void OnClickAgain()
    {
        if (owner.IsMonster())              //念气怪物不走这个流程
            return;
        if (BattleMain.IsModePvP(battleType))
            return;
        List<BeActor> cloneMonsterList = GamePool.ListPool<BeActor>.Get();
        owner.CurrentBeScene.FindActorById2(cloneMonsterList, m_CloneRayIdArr[0]);
        if (cloneMonsterList != null)
        {
            for (int i = 0; i < cloneMonsterList.Count; i++)
            {
                BeActor actor = cloneMonsterList[i];
                if (actor.GetOwner() != null && actor.GetOwner() == owner)
                {
                    actor.DoDead();
                    actor.m_pkGeActor.SetActorVisible(false);
                    owner.AddEntity(m_BombEntityId, actor.GetPosition());
                }
            }
            //修复释放技能时间内 所有幻影被打破以后 念气师卡死在分身技能的bug
            ResetButtonEffect();
        }
        GamePool.ListPool<BeActor>.Release(cloneMonsterList);
    }
}
