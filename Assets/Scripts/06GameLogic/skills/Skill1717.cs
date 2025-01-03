using System.Collections;
using System.Collections.Generic;
using UnityEngine;
///////删除linq
using GameClient;

public class Skill1717 : BeSkill
{

    VInt3[] pointsList = new VInt3[8];

    VInt3[] initPointsList = new VInt3[]
    {
        new VInt3(0.0f,  2.0f, 0.0f),
        new VInt3(-1.7f, 1.1f, 0.0f),
        new VInt3(-1.8f, -0.9f, 0.0f),
        new VInt3(-0.2f, -2.0f, 0.0f),
        new VInt3(1.6f, -1.2f, 0.0f),
        new VInt3(1.9f, 0.7f, 0.0f),
        new VInt3(0.4f, 2.0f, 0.0f),
        new VInt3(-1.5f, 1.3f, 0.0f)
    };


    List<BeProjectile> genBullete = new List<BeProjectile>();
    public Mechanism22 runeManager;
    IBeEventHandle handle = null;
    List<IBeEventHandle> handleList = new List<IBeEventHandle>();

    List<BeActor> targets = new List<BeActor>();//抓取到的怪物列表

    int curFrame = 0;//当前技能阶段
    int maxCount = 5;//召唤实体的最大数量
    int count = 0;//当前召唤的实体的数量    
    int time = 0;//普攻按钮计时器
    bool control = false;//实体旋转标志
    bool canMove = false;//抓取怪物的移动标志

    VInt dis = VInt.zero;//实体旋转的当前半径
    VInt pi = VInt.zero;//实体旋转当前的角度
    VFactor v0 = VFactor.zero;//实体旋转的初始角度
    VInt rSpeed = new VInt(0.42f);//实体当前旋转的速度


    //以下都是确定的值
    VInt y = new VInt(2f);//实体旋转的高度
    double speed = 0.1d;//怪物移动到抓取点的速度
    VInt maxRadius = new VInt(2f);//实体旋转最大半径

    VInt maxRspeed = new VInt(0.42f);//实体旋转最大速度
    VInt stepRspeed = new VInt(0.025f);//每次的加速度

    VInt initSpeed = new VInt(0.21f);//初始速度

    int grapPosCount = 8;//怪物抓取到环上的等分

    int shockTime = 0;//震动时间
    int shockSpeed = 0;//震动速度
    int shockRangeX = 0;//震动X的范围
    int shockRangeY = 0;//震动Y的范围

    protected bool m_CanBoom = false;           //标识暗球能否引爆

    int targetuffID = 171701;
    int targetEntityID = 60273;
    int boomEntityID = 60274;
    CrypticInt32 percent = 0;
    public Skill1717(int sid, int skillLevel) : base(sid, skillLevel)
    {

    }

    public override void OnInit()
    {
        shockTime = TableManager.GetValueFromUnionCell(skillData.ValueA[0], level);
        shockSpeed = TableManager.GetValueFromUnionCell(skillData.ValueB[0], level);
        shockRangeX = TableManager.GetValueFromUnionCell(skillData.ValueC[0], level);
        shockRangeY = TableManager.GetValueFromUnionCell(skillData.ValueD[0], level);
        percent = TableManager.GetValueFromUnionCell(skillData.ValueE[0], level);
    }


    public VInt3 GetClostPoint(BeActor actor)
    {
        int temp = int.MaxValue;
        int index = 0;
        var orginPos = actor.GetPosition();
        for (int i = 0; i < pointsList.Length; i++)
        {
            var destination = pointsList[i];

            int dis = (destination - orginPos).magnitude;
            if (dis < temp)
            {
                temp = dis;
                index = i;
            }
        }
        return pointsList[index];
    }

    public override void OnStart()
    {
        InitData();
        ModifyRadius();
        owner.grabPos = true;
        InitGrapPos();
        SetRuneManager();
        m_CanBoom = false;
        RemoveHandle();
        handle = owner.RegisterEventNew(BeEventType.onAfterGenBullet, (args) =>
        {
            // BeProjectile pkProjectile = args[0] as BeProjectile;
            BeProjectile pkProjectile = args.m_Obj as BeProjectile;
            if (pkProjectile == null)
                return;

            if (pkProjectile.m_iResID == targetEntityID || pkProjectile.m_iResID == boomEntityID)
            {
                var handler = pkProjectile.RegisterEventNew(BeEventType.onAfterCalFirstDamage, (obj) =>
                {
                    // bool isBeHit = (bool)obj[1];
                    bool isBeHit = obj.m_Bool;
                    if (!isBeHit)
                    {
                        int addPercent = percent;
                        // int[] vals = (int[])obj[0];
                        // int damage = vals[0];
                        int damage = obj.m_Int;

                        int cnt = 0;
                        if (runeManager != null)
                        {
                            cnt = runeManager.GetRuneCount();
                        }
                        var param = owner.TriggerEventNew(BeEventType.onCalcRuneAddDamage, new GameClient.EventParam() { m_Int = skillID, m_Int2 = cnt });
                        cnt = param.m_Int2;
                        addPercent *= (cnt - 1);
                        // vals[0] = (damage * (VFactor.one + VFactor.NewVFactor(addPercent, (long)GlobalLogic.VALUE_1000)));
                        obj.m_Int = (damage * (VFactor.one + VFactor.NewVFactor(addPercent, (long)GlobalLogic.VALUE_1000)));
                    }
                });
                handleList.Add(handler);


                if (pkProjectile.m_iResID == targetEntityID)
                {
                    owner.delayCaller.DelayCall(100, () =>
                    {
                        SetEntityIdleTime();
                    });

#if !LOGIC_SERVER
                    ShockInfo targetShockInfo = new ShockInfo();
                    targetShockInfo.shockTime = shockTime / 1000f;
                    targetShockInfo.shockSpeed = shockSpeed / 1000f;
                    targetShockInfo.shockRangeX = shockRangeX / 100f;
                    targetShockInfo.shockRangeY = shockRangeY / 100f;
                    pkProjectile.SetTargetShockInfo(targetShockInfo);
#endif

                    pkProjectile.RegisterEventNew(BeEventType.onDead, (obj) =>
                    {
                        control = false;
                    });
                    genBullete.Add(pkProjectile);
                    count++;
                    if (count == maxCount)
                    {
                        control = true;
                        InitPos();
                        if (!BattleMain.IsModePvP(battleType))
                        {
                            AddButtonEffect();
                        }
                    }
                }
            }
        });
    }

    private void InitData()
    {
        genBullete.Clear();
        maxRadius = new VInt(2f);
        maxCount = 5;
        count = 0;
        canMove = false;
        targets.Clear();
        time = 0;
        control = false;

        dis = VInt.zero;
        pi = VInt.zero;
        v0 = VFactor.zero;
        rSpeed = initSpeed;//new VInt(0.2f);//初始速度
        curFrame = 0;
    }

    public void ModifyRadius()
    {
        int rate = GlobalLogic.VALUE_1000;
        VInt r = 0;
        for (int i = 0; i < owner.MechanismList.Count; i++)
        {
            var m = owner.MechanismList[i] as Mechanism129;
            if (m != null)
            {
                rate += m.disRate;
                r += m.dis;
            }
        }

        maxRadius = maxRadius.i * VFactor.NewVFactor(rate, GlobalLogic.VALUE_1000) + r;

    }

    public void SetEntityIdleTime()
    {
        if (genBullete.Count <= 0) return;
        int curTime = (int)genBullete[0].GetStateGraph().GetCurrentStateData().uiTimeOut;
        int totalRate = 0;
        int totalTime = 0;
        for (int i = 0; i < owner.MechanismList.Count; i++)
        {
            var m = owner.MechanismList[i] as Mechanism128;
            if (m != null)
            {
                if (m.time != 0)
                {
                    totalTime += m.time;
                }

                if (m.timeRate != 0)
                {
                    totalRate += m.timeRate;
                }
            }
        }
        int addTime = (int)(curTime * VFactor.NewVFactor(totalRate, GlobalLogic.VALUE_1000).single) + totalTime;
        for (int j = 0; j < genBullete.Count; j++)
        {
            if (genBullete[j].m_cpkCurEntityActionInfo != null)
                genBullete[j].GetStateGraph().SetCurrentStatesTimeout(curTime + addTime, true);
        }
    }

    private void InitGrapPos()
    {
        var pos = owner.GetPosition();

        for (int i = 0; i < grapPosCount; ++i)
        {
            pointsList[i] = initPointsList[i] + pos;
        }
    }

    public override bool CanUseSkill()
    {
        bool hasRune = false;

        if (runeManager == null)
            SetRuneManager();

        if (runeManager != null)
            hasRune = runeManager.GetRuneCount() > 0;

        return base.CanUseSkill() && hasRune;
    }
    public override BeSkillManager.SkillCannotUseType GetCannotUseType()
    {
        bool hasRune = false;
        if (runeManager != null)
            hasRune = runeManager.GetRuneCount() > 0;

        if (!hasRune)
            return BeSkillManager.SkillCannotUseType.NO_KEYING;

        return base.GetCannotUseType();
    }

    void SetRuneManager()
    {
        if (owner != null)
        {
            var skill = owner.GetSkill(Global.BODONGKEYIN_SKILL_ID) as Skill1710;
            if (skill != null)
            {
                runeManager = skill.runeManager;
            }
        }
    }


    public override void OnEnterPhase(int phase)
    {
        curFrame = phase;
        if (phase == 2)
        {          
            targets = owner.GrapedActorList;
            SetRunEffect();
            canMove = true;
        }

        if (phase == 3)
        {
            for (int i = 0; i < maxCount; i++)
            {
                owner.AddEntity(targetEntityID, owner.GetPosition());
            }
        }

        if (phase != 3 || BattleMain.IsModePvP(battleType))
        {
            RemoveButtonEffect();
        }
    }

    private void SetRunEffect()
    {
#if !LOGIC_SERVER
        if (runeManager == null)
        {
            return;
        }
        List<Rune> list = runeManager.getRuneList();
        for (int i = 0; i < list.Count; i++)
        {
            if(list[i]!=null&&list[i].effectRune!=null)
            list[i].effectRune.SetVisible(false);
        }
#endif
    }


    public override void OnUpdate(int iDeltime)
    {

        if (control)
        {
            StartRotate();
        }
        if (canMove)
        {
            for (int i = 0; i < targets.Count; ++i)
            {
                var target = targets[i];

                if (!target.stateController.CanMove())
                    continue;

                var absorbPos = GetClostPoint(target);
                absorbPos.z = y.i - new VInt(0.5f).i;
                var pos = target.GetPosition();

                var del = absorbPos - pos;

                del.NormalizeTo((int)IntMath.kIntDen);


                var newVect = pos + del * speed;

                if ((target.GetPosition() - absorbPos).magnitude < VInt.Float2VIntValue(0.1f))
                    continue;

                target.SetPosition(newVect);
            }
        }

        UpdateAttackBtnState(iDeltime);

        if (curFrame == 2)
        {
            AddBuff();
        }
        else if (curFrame == 4)
        {
            RemoveBuff();
        }
    }

    private void AddBuff()
    {
        if (owner != null)
        {
            for (int i = 0; i < owner.GrapedActorList.Count; i++)
            {
                BeActor actor = owner.GrapedActorList[i];
                if (actor.buffController.HasBuffByID(targetuffID) == null)
                    actor.buffController.TryAddBuff(targetuffID, -1);
            }
        }
    }

    private void RemoveBuff()
    {
        for (int i = 0; i < owner.GrapedActorList.Count; i++)
        {
            BeActor actor = owner.GrapedActorList[i];
            actor.buffController.RemoveBuff(targetuffID);
        }
    }

    private void UpdateAttackBtnState(int iDeltaTime)
    {
        if (time > 200 && rSpeed < maxRspeed)
        {
            rSpeed += stepRspeed;
            if (rSpeed > maxRspeed)
            {
                rSpeed = maxRspeed;
            }
            time = 0;
        }
        else
        {
            time += iDeltaTime;
        }
    }

    public override void OnFinish()
    {
        canMove = false;
        owner.grabPos = false;
        RestoreData();
        SetRuneDead();
    }

    protected void RestoreData()
    {
        RemoveButtonEffect();
        RestoreButton();
    }

    void RemoveHandle()
    {
        if (handle != null)
        {
            handle.Remove();
            handle = null;
        }

        for (int i = 0; i < handleList.Count; i++)
        {
            if (handleList[i] != null)
            {
                handleList[i].Remove();
                handleList[i] = null;
            }
        }
        handleList.Clear();
    }

    protected void AddButtonEffect()
    {
        owner.delayCaller.DelayCall(30, () =>
        {
            m_CanBoom = true;
            skillButtonState = BeSkill.SkillState.WAIT_FOR_NEXT_PRESS;
            pressMode = SkillPressMode.TWO_PRESS;
#if !LOGIC_SERVER
            if (button != null)
            {
                button.AddEffect(ETCButton.eEffectType.onContinue);
            }
#endif
        });
    }

    protected void RemoveButtonEffect()
    {
#if !LOGIC_SERVER
        if (button != null)
        {
            button.RemoveEffect(ETCButton.eEffectType.onContinue);
        }
#endif
    }

    protected void RestoreButton()
    {
        m_CanBoom = false;
    }

    public override void OnCancel()
    {
        canMove = false;
        //技能被中断时将实体引爆
        for (int i = 0; i < genBullete.Count; i++)
        {
            genBullete[i].DoDie();
        }
        RemoveBuff();
        RestoreData();
        SetRuneDead();
    }

    private void SetRuneDead()
    {
        if (runeManager == null)
        {
            return;
        }
       runeManager.RemoveRune();
    }

    public override void OnClickAgain()
    {
        if (!m_CanBoom)
            return;
        if (curFrame == 3 && !BattleMain.IsModePvP(battleType))
        {
            for (int i = 0; i < genBullete.Count; i++)
            {
                genBullete[i].DoDie();
            }
            (owner.GetStateGraph() as BeActorStateGraph).ExecuteNextPhaseSkill();
            RemoveButtonEffect();
            RestoreButton();
        }

    }

    void InitPos()
    {
        v0 = (VFactor.pi * 2) / genBullete.Count;
        for (int i = 1; i < genBullete.Count; ++i)
        {
            genBullete[i].SetPosition(owner.GetPosition());
        }
    }

    void StartRotate()
    {
        for (int i = 0; i < genBullete.Count; ++i)
        {
            var radian = pi.factor + v0 * i;
            VInt z = dis.i * IntMath.cos(radian.nom, radian.den);
            VInt x = dis.i * IntMath.sin(radian.nom, radian.den);
            genBullete[i].SetPosition(new VInt3(x.i + owner.GetPosition().x, z.i + owner.GetPosition().y, y.i));
        }
        dis += VInt.Float2VIntValue(0.1f);
        pi += owner.GetFace() ? rSpeed : -rSpeed;
        dis = VInt.Clamp(dis, 0, maxRadius);
    }
}
