using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameClient;

//特殊子弹类型
public enum SpecialBulletType
{
    SILVER,
    STRESILVER,
    ICE,
    FIRE
}

//强化银弹
public class Skill1302 : Skill1015
{
    public Skill1302(int sid, int skillLevel) : base(sid, skillLevel)
    {

    }

    public override void OnInit()
    {
        base.OnInit();
        addBuffFlag = "130201";
    }
}

//冰冻弹
public class Skill1303 : Skill1015
{
    public Skill1303(int sid, int skillLevel) : base(sid, skillLevel)
    {

    }
    public override void OnInit()
    {
        base.OnInit();
        addBuffFlag = "1303101";
    }
}

//爆炎弹
public class Skill1308 : Skill1015
{
    public Skill1308(int sid, int skillLevel) : base(sid, skillLevel)
    {

    }

    public override void OnInit()
    {
        base.OnInit();
        addBuffFlag = "130801";
    }
}

public class Skill1015 : BeSkill
{
    public struct NewBullet
    {
        public int weaponType;
        public int bulletID;
    }
    CrypticInt32 bulletsCount = 0;
    int[] effectTableID = new int[2];

    int bullets = 0;
    int mBulletsPve = 0;
    int mBulletsPvp = 0;

    protected string addBuffFlag = "101501";
    int mRemoveBuffId = 0;

    IBeEventHandle handler = null;
    List<NewBullet> newBullets = new List<NewBullet>();
    IBeEventHandle removeBuffHandle = null;
    IBeEventHandle curFrameHandle = null;

    public Skill1015(int sid, int skillLevel) : base(sid, skillLevel)
    {

    }

    public override void OnInit()
    {
        newBullets.Clear();
        if (skillData != null)
        {
            mBulletsPve = TableManager.GetValueFromUnionCell(skillData.ValueA[0], level);
            mBulletsPvp = skillData.ValueA.Count > 1 ? TableManager.GetValueFromUnionCell(skillData.ValueA[1], level) : TableManager.GetValueFromUnionCell(skillData.ValueA[0], level);

            for (int i = 0; i < skillData.ValueC.Count; i += 2)
            {
                NewBullet bullet;
                bullet.weaponType = TableManager.GetValueFromUnionCell(skillData.ValueC[i], level);
                bullet.bulletID = TableManager.GetValueFromUnionCell(skillData.ValueC[i + 1], level);

                newBullets.Add(bullet);
            }

            for(int i = 0; i < skillData.ValueD.Count; i++)
            {
                effectTableID[i] = TableManager.GetValueFromUnionCell(skillData.ValueD[i], level);
            }
            mRemoveBuffId = TableManager.GetValueFromUnionCell(skillData.ValueE[0], level);
        }
    }

    public override void OnPostInit()
    {
        base.OnPostInit();
        UpdateBulletInfo();
    }

    void SetBulletNum()
    {
        bulletsCount = 0;
        bullets = BattleMain.IsModePvP(battleType) ? mBulletsPvp : mBulletsPve;

        SpecialBulletType type = SpecialBulletType.SILVER;
        switch (skillID)
        {
            case 1302:
                type = SpecialBulletType.STRESILVER;
                break;
            case 1303:
                type = SpecialBulletType.ICE;
                break;
            case 1308:
                type = SpecialBulletType.FIRE;
                break;
        }
        for (int i = 0; i < owner.MechanismList.Count; i++)
        {
            var m = owner.MechanismList[i] as Mechanism126;
            if (m != null && type == (SpecialBulletType)m.bulletType)
                bullets += m.bulletNum;
        }
    }

    public override void OnStart()
    {
        SetBulletNum();
        RemoveHandle();
        curFrameHandle = owner.RegisterEventNew(BeEventType.onSkillCurFrame, (args) =>
        //curFrameHandle = owner.RegisterEvent(BeEventType.onSkillCurFrame, (object[] args) =>
         {
             string flag = args.m_String;
             if (flag == addBuffFlag)
             {
                 UpdateBulletInfo();
             }
         });

        removeBuffHandle = owner.RegisterEventNew(BeEventType.onRemoveBuff, (args) =>
        {
            int buffId = (int)args.m_Int;
            if (buffId == mRemoveBuffId)
            {
                bulletsCount = bullets;
                DoClear();
            }
        });

        handler = owner.RegisterEventNew(BeEventType.onBeforeGenBullet, (args) =>
        {
            if (owner.IsCastingSkill())
            {
                if (IsSkillTakeEffect(owner.GetCurSkillID()))
                {
                    args.m_Int2 = BattleMain.IsModePvP(battleType) ? effectTableID[1] : effectTableID[0];

                    int ownerWeaponType = owner.GetWeaponType();
                    for (int i = 0; i < newBullets.Count; ++i)
                    {
                        if (ownerWeaponType == newBullets[i].weaponType)
                        {
                            args.m_Int = newBullets[i].bulletID;
                        }
                    }

                    ConsumBulletCount();
                }
            }
        });
    }

    public override void OnCancel()
    {
        base.OnCancel();
        if (bulletsCount < bullets)
        {
            bulletsCount = bullets;
            DoClear();
        }
    }

    void DoClear()
    {
        RemoveHandle();

        UpdateBulletInfo();
    }

    bool IsSkillTakeEffect(int skillID)
    {
        if (skillData != null)
        {
            for (int i = 0; i < skillData.ValueB.Count; ++i)
            {
                var sid = TableManager.GetValueFromUnionCell(skillData.ValueB[i], level);
                if (sid == skillID)
                    return true;
            }
        }
        return false;
    }

    void UpdateBulletInfo()
    {
        if (bullets - bulletsCount <= 0)
        {
            RemoveBuff();
        }
#if !SERVER_LOGIC
        if (owner != null && owner.isLocalActor)
        {
            var battleUI = BattleUIHelper.CreateBattleUIComponent<BattleUIProfession>();;
            if (battleUI != null)
            {
                SpecialBulletType type = SpecialBulletType.SILVER;
                switch (skillID)
                {
                    case 1302:
                        type = SpecialBulletType.STRESILVER;
                        break;
                    case 1303:
                        type = SpecialBulletType.ICE;
                        break;
                    case 1308:
                        type = SpecialBulletType.FIRE;
                        break;
                }
                battleUI.SetSilverBulletNum(skillID,bullets - bulletsCount, type);
            }
        }
#endif
    }

    protected void RemoveHandle()
    {
        if (handler != null)
        {
            handler.Remove();
            handler = null;
        }

        if (removeBuffHandle != null)
        {
            removeBuffHandle.Remove();
            removeBuffHandle = null;
        }

        if (curFrameHandle != null)
        {
            curFrameHandle.Remove();
            curFrameHandle = null;
        }
    }

    //当子弹打完以后移除当前特殊子弹的Buff
    protected void RemoveBuff()
    {
        if (mRemoveBuffId != 0)
        {
            owner.buffController.RemoveBuff(mRemoveBuffId);
        }
    }

    /// <summary>
    /// 获取剩余银弹次数
    /// </summary>
    /// <returns></returns>
    public int GetLeftBulletNum()
    {
        int leftNum = bullets - bulletsCount;
        if (leftNum > 0)
            return leftNum;
        else
            return 0;
    }

    /// <summary>
    /// 消耗子弹数量
    /// </summary>
    public void ConsumBulletCount()
    {
        bulletsCount++;
        UpdateBulletInfo();

        if (bulletsCount >= bullets)
        {
            DoClear();
        }
    }
}
