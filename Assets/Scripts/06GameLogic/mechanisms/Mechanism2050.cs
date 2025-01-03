using System.Collections;
using System.Collections.Generic;
using GameClient;
using UnityEngine;
/// <summary>
/// 团本机制泡泡机制
/// </summary>
public class Mechanism2050 : BeMechanism
{

    private BeActor target;
    private int monsterID = 0;
    private int moveXSpeed = 50000;
    private int tempMoveSpeed = 0;
    private int entityID = 60478;
    private int boomEntityID = 60464;
    private int maxZpos = 30000;
    private int recordHitNum = 0;
    private int maxHitNum = 5;
    private int moveZSpeed = 100000;
    private bool isInit = false;
    private bool attackFlag = false;
    private int buffID = 570028;
    private List<IBeEventHandle> handlList = new List<IBeEventHandle>();
    public Mechanism2050(int mid, int lv) : base(mid, lv)
    {
    }

    public override void OnInit()
    {
        base.OnInit();
        moveXSpeed = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        moveZSpeed = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
        maxHitNum = TableManager.GetValueFromUnionCell(data.ValueC[0], level);
        maxZpos = TableManager.GetValueFromUnionCell(data.ValueD[0], level);
        entityID = TableManager.GetValueFromUnionCell(data.ValueE[0], level);
        var tempBuffId = TableManager.GetValueFromUnionCell(data.ValueF[0], level);
        if(tempBuffId != 0)
        {
            buffID = tempBuffId;
        }
    }

    public override void OnReset()
    {
        target = null;
        monsterID = 0;
        moveXSpeed = 50000;
        tempMoveSpeed = 0;
        entityID = 60478;
        boomEntityID = 60464;
        maxZpos = 30000;
        recordHitNum = 0;
        maxHitNum = 5;
        moveZSpeed = 100000;
        isInit = false;
        attackFlag = false;
        buffID = 570028;
        handlList.Clear();
    }

    public override void OnStart()
    {
        base.OnStart();

        //handleA = owner.RegisterEvent(BeEventType.onCollide, (args) =>
        //{

        //});

        handleA = owner.RegisterEventNew(BeEventType.onCollide, _OnCollide);

        var handle5 = owner.RegisterEventNew(BeEventType.onTouchGround, eventParam =>
        {         
            if (isInit && !owner.IsDead())
                owner.DoDead();
        });
        handlList.Add(handle5);
    }

    private void _OnCollide(BeEvent.BeEventParam param)
    {
        if (owner.IsDead() || owner.CurrentBeScene.IsInBlockPlayer(owner.GetPosition())) return;
        BeActor target = param.m_Obj as BeActor;
        if (!target.isMainActor || target.IsDead()) return;
        if (this.target == null)
        {
            if (target != null && !target.isSpecialMonster)
            {
                target.SetAttackButtonState(GameClient.ButtonState.RELEASE);
                target.sgSwitchStates(new BeStateData((int)ActionState.AS_IDLE));
                target.buffController.TryAddBuff(buffID, -1);
                target.buffController.TryAddBuff(99, -1);
                target.m_pkGeActor.ChangeAction("Anim_Xiadun", 1.0f, true);
                target.buffController.TryAddBuff((int)GlobalBuff.INVINCIBLE_NO_ALPHA, -1);
                target.stateController.SetAbilityEnable(BeAbilityType.CHANGE_FACE, false);

                this.target = target;
                SetSkillBtn(false);
                var handle1 = target.RegisterEventNew(BeEventType.OnChangeMoveDir, eventParam =>
                {
                    short degree = (short)eventParam.m_Int;
                    bool bSet = (bool)eventParam.m_Bool;
                    if (bSet)
                    {
                        if (degree > 6 && degree < 18)
                        {
                            tempMoveSpeed = -moveXSpeed;
                        }
                        else
                        {
                            tempMoveSpeed = moveXSpeed;
                        }
                    }
                    else
                    {
                        tempMoveSpeed = 0;
                    }
                });
                var handle3 = target.RegisterEventNew(BeEventType.onCastNormalAttack, eventParam =>
                {
                    attackFlag = true;
                    isInit = true;
                    target.AddEntity(entityID, target.GetPosition());
                    owner.SetMoveSpeedZ(-moveZSpeed);
                });

                handleB = target.RegisterEventNew(BeEventType.onHitOther, _OnCastNormalAttack);

                //var handle4 = target.RegisterEvent(BeEventType.onHitOther, (obj) =>
                //{
                    
                //});
                var handle6 = target.RegisterEventNew(BeEventType.onDead, eventParam =>
                {
                    var pos = target.GetPosition();
                    target.SetPosition(new VInt3(pos.x, pos.y, 0));
                    if (!owner.IsDead())
                        owner.DoDead();
                });

                handlList.Add(handle1);
                handlList.Add(handle3);
                //handlList.Add(handle4);
                handlList.Add(handle6);
            }
        }
    }

    private void _OnCastNormalAttack(BeEvent.BeEventParam param)
    {
        if (!attackFlag) return;
        attackFlag = false;
        recordHitNum++;
        if (recordHitNum >= maxHitNum && !owner.IsDead())
        {
            owner.DoDead();
        }
    }

    private void SetSkillBtn(bool flag)
    {
#if !SERVER_LOGIC
        if (target != null && target.isLocalActor)
        {
            if (flag)
            {
                InputManager.instance.ResetButtonState();
            }
            else
            {
                InputManager.instance.SetButtonStateActive(0);
            }
        }
#endif
    }

    public override void OnUpdate(int deltaTime)
    {
        base.OnUpdate(deltaTime);
        if (owner.IsDead()) return;
        
        if (owner.GetPosition().z > maxZpos && !owner.IsDead())
        {
            if (target != null)
            {
                target.DoHPChange(-int.MaxValue,false);
               
            }
            owner.DoDead();
        }
        if (!owner.IsDead())
        {
            owner.SetMoveSpeedX(tempMoveSpeed);
            if (target != null)
                target.SetPosition(owner.GetPosition(), true);
        }
    }

    public override void OnFinish()
    {
        base.OnFinish();
        ClearHandle();
        RestoreTarget();
    }

    private void RestoreTarget()
    {
        if (target != null)
        {
            SetSkillBtn(true);
#if !LOGIC_SERVER
            target.TriggerEventNew(BeEventType.onMechanism2050RestoreBtn);
#endif
            target.buffController.RemoveBuff(buffID);
            target.buffController.RemoveBuff(99);
            target.stateController.SetAbilityEnable(BeAbilityType.CHANGE_FACE, true);
            target.buffController.RemoveBuff((int)GlobalBuff.INVINCIBLE_NO_ALPHA);
            target.sgSwitchStates(new BeStateData((int)ActionState.AS_IDLE));
            target = null;
        }
    }

    private void ClearHandle()
    {
        for (int i = 0; i < handlList.Count; i++)
        {
            if (handlList[i] != null)
            {
                handlList[i].Remove();
                handlList[i] = null;
            }
        }
        handlList.Clear();
    }
}
