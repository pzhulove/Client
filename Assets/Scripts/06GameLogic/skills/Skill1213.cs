using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameClient;

/// <summary>
/// 压缩炮
/// </summary>
public class Skill1213 : BeSkill
{
    BeActor actor = null;
    VInt bornPos = 0;
    VInt speed;
    VInt dis = 0;
    int changePhase = 0;
    int mechanisID = 0;
    int smallResID = 0;
    VInt tmpSpeed;
    bool beDead = false;
    int curFrame = 0;
    Vector3 orginPos = Vector3.zero;
    int monsterID = 2150031;
    protected IBeEventHandle m_SummonHandle = null;                  //监听召唤
    protected bool m_ChargeFlag = false;                            //蓄力是否成功
    protected int[] m_BoomEntityId = new int[] { 60271, 60272 };    //爆炸实体ID
    protected IBeEventHandle m_ChargeHandle = null;                 //监听蓄力是否成功
    protected IBeEventHandle m_ActorDeadHandle = null;              //监听实体死亡
    protected IBeEventHandle m_HitOtherHandle = null;               //监听击中到别人
    
    protected bool isBombEntityCreated = false;
    public Skill1213(int sid, int skillLevel) : base(sid, skillLevel)
    {

    }

    public override void OnInit()
    {
        bornPos = VInt.NewVInt(TableManager.GetValueFromUnionCell(skillData.ValueA[0], level), GlobalLogic.VALUE_1000);
        speed = VInt.NewVInt(TableManager.GetValueFromUnionCell(skillData.ValueB[0], level), GlobalLogic.VALUE_1000);
        dis = VInt.NewVInt(TableManager.GetValueFromUnionCell(skillData.ValueC[0], level), GlobalLogic.VALUE_1000);
        changePhase = TableManager.GetValueFromUnionCell(skillData.ValueD[0], level);
        mechanisID = TableManager.GetValueFromUnionCell(skillData.ValueE[0], level);
        smallResID = TableManager.GetValueFromUnionCell(skillData.ValueF[0], level);
    }

    public override void OnStart()
    {
        actor = null;
        base.OnStart();
        isBombEntityCreated = false;
        m_ChargeFlag = false;
        RemoveHandler();
        m_SummonHandle = owner.RegisterEventNew(BeEventType.onSummon, (obj) =>
        {
            var  _beactor = obj.m_Obj as BeActor;
            if (_beactor != null &&
                _beactor.GetEntityData() != null &&
                _beactor.GetEntityData().monsterID != monsterID) return;
            actor = _beactor;
            if (actor == null) return;           
            beDead = false;
            scale = 0;
            changeScale = true;
            recordScale = actor.GetScale();
            var v = owner.GetPosition();
            int x = owner.GetFace() ? -bornPos.i + v.x : v.x + bornPos.i;
            actor.SetPosition(new VInt3(x, v.y, actor.GetPosition().z), true);
            //actor.SetPosition(new Vec3(x,v.y,v.z),true);           
            tmpSpeed = owner.GetFace() ? -speed.i : speed.i;
            //摄像机偏移
            if (actor.m_iResID == smallResID)
                SetCameraPause(true);

#if !LOGIC_SERVER
            orginPos = owner.CurrentBeScene.currentGeScene.GetCamera().GetController().transform.localPosition;
#endif
            if (!BattleMain.IsModePvP(owner.battleType))
                actor.buffController.TryAddBuff(25, -1);

            m_HitOtherHandle = actor.RegisterEventNew (BeEventType.onHitOther, eventParam =>
            //m_HitOtherHandle = actor.RegisterEvent(BeEventType.onHitOther, (object[] args) =>
            {
                BeEntity entity = eventParam.m_Obj as BeEntity;
                if (entity != null && entity is BeObject)
                {
                    SetMonsterDead();
                }
            });
            m_ActorDeadHandle = actor.RegisterEventNew(BeEventType.onDead, eventParam =>
            {
                if (isBombEntityCreated) return;
                CreateBoomEntity();
            });
           
        });

        m_ChargeHandle = owner.RegisterEventNew(BeEventType.onPreSetSkillAction, param =>
         {
             //int[] skillIdList = (int[])args[0];
             int skillId = param.m_Int;

             if (skillId == 12134)
             {
                 m_ChargeFlag = false;
             }
             else if (skillId == 121341)
             {
                 m_ChargeFlag = true;
             }
         });
    }

    void SetCameraPause(bool flag)
    {
#if !LOGIC_SERVER
        if (!owner.isLocalActor)
            return;
        owner.CurrentBeScene.currentGeScene.GetCamera().GetController().SetPause(flag);
#endif
    }

    void SetCameraPos(Vector3 offset)
    {
#if !LOGIC_SERVER
        if (!owner.isLocalActor)
            return;
        owner.CurrentBeScene.currentGeScene.GetCamera().GetController().SetCameraPos(offset);
#endif
    }

    public override void OnEnterPhase(int phase)
    {
        curFrame = phase;
        base.OnEnterPhase(phase);
        if (actor != null && owner != null && curFrame == changePhase)
        {
            actor.RemoveMechanism(mechanisID);
        }
    }

    VInt recordScale = 0;
    VInt scale = 0;
    bool changeScale = true;
    public override void OnUpdate(int iDeltime)
    {
        base.OnUpdate(iDeltime);
        if (actor != null && owner != null && !beDead)
        {
            if (changeScale)
            {
                scale += 100;
                actor.SetScale(recordScale + scale);
            }
            if (curFrame == changePhase)
            {
              //  if (BattleMain.IsModePvP(owner.battleType))
               //     actor.buffController.TryAddBuff(25,-1);
                changeScale = false;
                actor.ResetMoveCmd();
                actor.SetMoveSpeedX(tmpSpeed);
#if !LOGIC_SERVER
                orginPos.x += tmpSpeed.scalar * iDeltime / 1000.0f;
#endif
                if (BattleMain.IsModePvP(owner.battleType))
                    actor.buffController.TryAddBuff(25, -1);
            }
            SetCameraPos(new Vector3(orginPos.x, orginPos.y, orginPos.z));
            if (Mathf.Abs(actor.GetPosition().x - owner.GetPosition().x) > dis 
                || (owner.CurrentBeScene.IsInBlockPlayer(actor.GetPosition()) && curFrame == changePhase))
            {
                SetMonsterDead();
            }
        }
    }

    private void SetMonsterDead()
    {
        if (actor != null && owner != null&&!beDead)
        {
            actor.SetScale(recordScale);
            beDead = true;
            actor.DoDead();
            owner.delayCaller.DelayCall(500, () => 
            {
                if(!isFinish() && !owner.IsDead())
                owner.Locomote(new BeStateData((int)ActionState.AS_IDLE), true);
            });
            owner.delayCaller.DelayCall(1300, () =>
            {
                SetCameraPause(false);
            });
        }
    }

    public override void OnCancel()
    {
        SetMonsterDead();
        changeScale = false;
    }

    public override void OnFinish()
    {
        changeScale = false;
    }

    //创建爆炸实体
    protected void CreateBoomEntity()
    {
        if (actor != null)
        {
            isBombEntityCreated = true;
            int entityId = m_ChargeFlag ? m_BoomEntityId[1] : m_BoomEntityId[0];
            owner.AddEntity(entityId, actor.GetPosition(), level);
        }
        
    }

    void RemoveHandler()
    {
        if (m_SummonHandle != null)
        {
            m_SummonHandle.Remove();
            m_SummonHandle = null;
        }
        if (m_ChargeHandle != null)
        {
            m_ChargeHandle.Remove();
        }
        if (m_ActorDeadHandle != null)
        {
            m_ActorDeadHandle.Remove();
        }
        if (m_HitOtherHandle != null)
        {
            m_HitOtherHandle.Remove();
        }
    }
}
