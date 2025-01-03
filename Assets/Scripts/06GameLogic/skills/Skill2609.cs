using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill2609 : BeSkill
{
    private BeActor actor = null;
    private VInt bornPos = 0;
    private VInt speed = 0;
    private VInt dis = 0;
    private int changePhase = 0;
    private int mechanisID = 0;
    private int smallResID = 0;
    private VInt tmpSpeed;
    private bool beDead = false;
    private int curFrame = 0;
    private Vector3 orginPos = Vector3.zero;
    private IBeEventHandle m_SummonHandle = null;                   //监听召唤
    private bool m_ChargeFlag = false;                              //蓄力是否成功
    private int[] m_BoomEntityId = new int[] { 60271, 60272 };      //爆炸实体ID
    private IBeEventHandle m_ChargeHandle = null;                   //监听蓄力是否成功
    private IBeEventHandle m_ActorDeadHandle = null;                //监听实体死亡
    private GameClient.BeEvent.BeEventHandleNew m_HitOtherHandle = null;                //监听击中到别人
    private int currentDir = 0;
    private bool m_dir_flag = false;
    private VInt speedy = 0;
    private VInt speedX = 0;
    private VInt speedY = 0;
    private VInt disX = 0;
    private VInt maxScale = 0;
    private int monsterID = 94080031;
    private bool isBombEntityCreated = false;
    private int tagPosX;
    private int restoreCameraSec;
    private VFactor mCameraPVEOff;
    private VFactor mCameraPVPOff;
    private VFactor mMoveCameraTimeLen;
    private VFactor mResetCameraTimeLen;

    private VFactor mCameraOff;

    public Skill2609(int sid, int skillLevel) : base(sid, skillLevel)
    {
    }

    public override void OnInit()
    {
        bornPos = VInt.NewVInt(TableManager.GetValueFromUnionCell(skillData.ValueA[0], level), GlobalLogic.VALUE_1000);
        speed = VInt.NewVInt(TableManager.GetValueFromUnionCell(skillData.ValueB[0], level), GlobalLogic.VALUE_1000);
        speedX = VInt.NewVInt(TableManager.GetValueFromUnionCell(skillData.ValueB[1], level), GlobalLogic.VALUE_1000);
        speedY = VInt.NewVInt(TableManager.GetValueFromUnionCell(skillData.ValueB[2], level), GlobalLogic.VALUE_1000);
        dis = VInt.NewVInt(TableManager.GetValueFromUnionCell(skillData.ValueC[0], level), GlobalLogic.VALUE_1000);
        disX = VInt.NewVInt(TableManager.GetValueFromUnionCell(skillData.ValueC[1], level), GlobalLogic.VALUE_1000);
        changePhase = TableManager.GetValueFromUnionCell(skillData.ValueD[0], level);
        restoreCameraSec = TableManager.GetValueFromUnionCell(skillData.ValueD[1], level);
        mechanisID = TableManager.GetValueFromUnionCell(skillData.ValueE[0], level);
        smallResID = TableManager.GetValueFromUnionCell(skillData.ValueF[0], level);
        maxScale = VInt.NewVInt(Mathf.Clamp(TableManager.GetValueFromUnionCell(skillData.ValueG[0], level), 10000, 50000), GlobalLogic.VALUE_10000);

        mCameraPVEOff = VFactor.NewVFactor(TableManager.GetValueFromUnionCell(skillData.ValueG[1], level), GlobalLogic.VALUE_1000);
        mCameraPVPOff = VFactor.NewVFactor(TableManager.GetValueFromUnionCell(skillData.ValueG[2], level), GlobalLogic.VALUE_1000);
        mMoveCameraTimeLen = VFactor.NewVFactor(TableManager.GetValueFromUnionCell(skillData.ValueG[3], level), GlobalLogic.VALUE_1000);
        mResetCameraTimeLen = VFactor.NewVFactor(TableManager.GetValueFromUnionCell(skillData.ValueG[4], level), GlobalLogic.VALUE_1000);
    }

    public override void OnStart()
    {
        m_BoomEntityId = new int[] { 63683, 63684 };
        actor = null;
        isBombEntityCreated = false;
        m_ChargeFlag = false;
        m_dir_flag = false;
        tagPosX = 0;
        mCameraOff = VFactor.zero;
        RemoveHandler();
        monsterID = BattleMain.IsModePvP(owner.battleType) ? 94090031 : 94080031;
        m_SummonHandle = owner.RegisterEventNew(BeEventType.onSummon, (obj) =>
        {
            var _beactor = obj.m_Obj as BeActor;
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
            tagPosX = owner.GetPosition().x;
            int x = owner.GetFace() ? -bornPos.i + v.x : v.x + bornPos.i;
            
            actor.SetPosition(new VInt3(x, v.y, actor.GetPosition().z), true);
            //actor.SetPosition(new Vec3(x,v.y,v.z),true);           
            speed = owner.GetFace() ? -Mathf.Abs(speed.i) : Mathf.Abs(speed.i);
            //摄像机偏移

#if !LOGIC_SERVER
            orginPos = owner.CurrentBeScene.currentGeScene.GetCamera().GetController().transform.localPosition;
#endif
            if (!BattleMain.IsModePvP(owner.battleType))
                actor.buffController.TryAddBuff(25, -1);

            m_HitOtherHandle = actor.RegisterEventNew(BeEventType.onHitOther, args =>
            //m_HitOtherHandle = actor.RegisterEvent(BeEventType.onHitOther, (object[] args) =>
            {
                BeEntity entity = args.m_Obj as BeEntity;
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

        m_ChargeHandle = owner.RegisterEventNew(BeEventType.onPreSetSkillAction, (GameClient.BeEvent.BeEventParam param) =>
        {
            //int[] skillIdList = (int[])args[0];
            int skillId = param.m_Int;

            if (skillId == 26094)
            {
                m_ChargeFlag = false;
            }
            else if (skillId == 26095)
            {
                m_ChargeFlag = true;
            }
        });
    }

    protected VInt recordScale = 0;
    protected VInt scale = 0;
    protected bool changeScale = true;

    public override void OnUpdate(int iDeltime)
    {
        if (actor != null && owner != null && !beDead)
        {
            if (changeScale)
            {
                scale += 100;
                VInt targetScale = VInt.Clamp(recordScale + scale, VInt.zero, maxScale);
                actor.SetScale(targetScale);
            }
            if(curFrame == changePhase)
            {
                if (!m_dir_flag)
                {
                    m_dir_flag = true;
                    currentDir = (owner as BeEntity).GetJoystickDegree();
                    if (currentDir > 45 && currentDir < 135) //TOP
                    {
                        speed = owner.GetFace() ? -Mathf.Abs(speedX.i) : Mathf.Abs(speedX.i);
                        speedy = Mathf.Abs(speedY.i);
                        dis = disX;
                    }
                    else if (currentDir >= 225 && currentDir < 315) //DOWN
                    {
                        speed = owner.GetFace() ? -Mathf.Abs(speedX.i) : Mathf.Abs(speedX.i);
                        speedy = -Mathf.Abs(speedY.i);
                        dis = disX;
                    }
                    else
                    {
                        speed = owner.GetFace() ? -Mathf.Abs(speed.i) : Mathf.Abs(speed.i);
                        speedy = 0;
                        dis = VInt.NewVInt(TableManager.GetValueFromUnionCell(skillData.ValueC[0], level), GlobalLogic.VALUE_1000);
                    }
                }
                if (BattleMain.IsModePvP(owner.battleType))
                    actor.buffController.TryAddBuff(25, -1);
                changeScale = false;
                actor.ResetMoveCmd();

                actor.SetMoveSpeedX(speed);
                actor.SetMoveSpeedY(speedy);
#if !LOGIC_SERVER
                orginPos.x += speed.scalar * iDeltime / 1000.0f;
#endif
            }
            if (Mathf.Abs(actor.GetPosition().x - tagPosX) > dis 
                || (owner.CurrentBeScene.IsInBlockPlayer(actor.GetPosition()) && curFrame == changePhase))//后面的判断条件 在释放阶段并且在墙内
            {
                SetMonsterDead();
            }
        }
    }

    public override void OnEnterPhase(int phase)
    {
        curFrame = phase;
        base.OnEnterPhase(phase);
        if (actor != null && owner != null && curFrame == changePhase)
        {
            actor.RemoveMechanism(mechanisID);
        }
        if (curFrame == 1 && owner.GetCurSkillID() == 2609)
        {
            var tempOff = BattleMain.IsModePvP(owner.battleType) ? mCameraPVPOff : mCameraPVEOff;
            mCameraOff = tempOff * (owner.GetFace() ? -1 : 1);
            MoveCameraPos(mCameraOff, mMoveCameraTimeLen);
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
    private void CreateBoomEntity()
    {
        if (actor != null)
        {
            isBombEntityCreated = true;
            int entityId = m_ChargeFlag ? m_BoomEntityId[1] : m_BoomEntityId[0];
            owner.AddEntity(entityId, actor.GetPosition(), level);
        }

    }

    private void SetMonsterDead()
    {
        if (actor != null && owner != null && !beDead)
        {
            actor.SetScale(recordScale);
            beDead = true;
            actor.DoDead();
            owner.delayCaller.DelayCall(500, () =>
            {
                if (!isFinish() && !owner.IsDead())
                {
                    owner.Locomote(new BeStateData((int)ActionState.AS_IDLE), true);
                }
            });
            owner.delayCaller.DelayCall(restoreCameraSec, () =>
            {
                RestoreCamera(mResetCameraTimeLen);
            });
            actor.buffController.RemoveBuff((int)GlobalBuff.GRAB);
        }
    }

    private void RestoreCamera(VFactor timeLen)
    {
#if !LOGIC_SERVER
        if (!owner.isLocalActor)
            return;
        BeUtility.ResetCamera();
#endif
    }

    private void MoveCameraPos(VFactor off,VFactor timeLen)
    {
#if !LOGIC_SERVER
        if (!owner.isLocalActor)
            return;
        owner.CurrentBeScene.currentGeScene.GetCamera().GetController().MoveCamera(off.single, timeLen.single);
#endif
    }

    protected void RemoveHandler()
    {
        if (m_SummonHandle != null)
        {
            m_SummonHandle.Remove();
            m_SummonHandle = null;
        }
        if (m_ChargeHandle != null)
        {
            m_ChargeHandle.Remove();
            m_ChargeHandle = null;
        }
        if (m_ActorDeadHandle != null)
        {
            m_ActorDeadHandle.Remove();
            m_ActorDeadHandle = null;
        }
        if (m_HitOtherHandle != null)
        {
            m_HitOtherHandle.Remove();
            m_HitOtherHandle = null;
        }
    }
}
