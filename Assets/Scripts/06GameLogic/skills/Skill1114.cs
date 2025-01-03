using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Skill1114 : BeSkill
{
    protected VInt3 m_OwnerStartPos = new VInt3();      //角色初始移动时的坐标
    protected VInt3[] m_MovePosArray = new VInt3[8];    //技能移动的坐标数组
    protected int m_BoomEntityId = 60279;               //爆炸实体ID
    protected int m_BoomMaxNum = 10;                    //爆炸数量
    protected int m_CurrBoomNum = 0;                    //当前爆炸实体数量

    protected int m_DeadFlagBuffId = 111403;            //死亡标记Buff
    protected int m_AddBuffId = 111402;                 //监听爆炸时机的BuffId

    protected IBeEventHandle m_BoomHandle = null;        //爆炸
    protected IBeEventHandle m_KillHandle = null;
    protected List<BeEntity> m_KillList = new List<BeEntity>();     //击杀目标列表
    protected bool m_BoomFlag = false;

    int[] posX;
    int[] posY;
    Vector3 pos;
    bool flag = false;
    float speed = 3.0f;
#if !LOGIC_SERVER
    GeCameraControllerScroll camera = null;
#endif
    Vector3 startPos;
    public Skill1114(int sid, int skillLevel)
        : base(sid, skillLevel)
    {

    }

    public override void OnInit()
    {
        posX = new int[skillData.ValueA.Count];
        for (int i = 0; i < skillData.ValueA.Count; i++)
        {
            posX[i] = TableManager.GetValueFromUnionCell(skillData.ValueA[i], level);
        }
        posY = new int[skillData.ValueB.Count];
        for (int i = 0; i < skillData.ValueB.Count; i++)
        {
            posY[i] = TableManager.GetValueFromUnionCell(skillData.ValueB[i], level);
        }
    }

    public override void OnStart()
    {
#if !LOGIC_SERVER   
        camera = owner.CurrentBeScene.currentGeScene.GetCamera().GetController();
#endif
        m_CurrBoomNum = 0;
        time = 0;
        flag = false;
        m_BoomFlag = false;
        GetMovePos();
        KillOther();
        handleA = owner.RegisterEventNew(BeEventType.onSkillCurFrame, (args) =>
        //handleA = owner.RegisterEvent(BeEventType.onSkillCurFrame, (args) =>
        {
            if (!owner.isLocalActor) return;
            string flag = args.m_String;
            if (flag == "removeCamera")
            {
#if !LOGIC_SERVER               
                this.flag = true;
                SetCameraPause(true);
                Vector3 cameraPos = camera.transform.localPosition;
                startPos = cameraPos;
                int offset = owner.GetFace() ? -4 : 4;
                pos = new Vector3(owner.GetPosition().vector3.x + offset, cameraPos.y, cameraPos.z);
#endif

            }
            else if (flag == "addCamera")
            {
                this.flag = false;
                RestoreCamera();
            }
        });

        m_BoomHandle = owner.RegisterEventNew(BeEventType.onAddBuff,(args)=>
        {
            if (!m_BoomFlag)
            {
                BeBuff buff = args.m_Obj as BeBuff;
                if (buff.buffID == m_AddBuffId)
                {
                    m_BoomFlag = true;
                    if (m_KillList.Count > 0)
                    {
                        for (int i = 0; i < m_KillList.Count; i++)
                        {
                            if (m_KillList[i] != null)
                            {
                                BeEntity entity = m_KillList[i];
                                if (entity != null)
                                {
                                    CreateBoomEntity(entity);
                                }
                            }
                            
                        }
                    }
                }
            }
        });
    }

    int time = 0;
    public override void OnUpdate(int iDeltime)
    {
        base.OnUpdate(iDeltime);
#if !LOGIC_SERVER
        if (owner.isLocalActor && flag)
        {
            base.OnUpdate(iDeltime);
            time += iDeltime;
            camera.SetCameraPos(Vector3.Lerp(startPos, pos, time / 1000.0f * speed));
        }
#endif
    }

    void SetCameraPause(bool flag)
    {
#if !LOGIC_SERVER
        if (!owner.isLocalActor)
            return;
        owner.CurrentBeScene.currentGeScene.GetCamera().GetController().SetPause(flag);
#endif
    }

    //获取技能移动的位置坐标
    protected void GetMovePos()
    {
        BeScene scene = owner.CurrentBeScene;
        m_OwnerStartPos = owner.GetPosition();
        int logicWidth = scene.logicXSize.y - scene.logicXSize.x - GlobalLogic.VALUE_100000;
        int logicHeight = scene.logicZSize.y - scene.logicZSize.x;
        int xOffset = owner.GetFace() ? -1 : 1;
        int yOffset = logicHeight / 4;
        int xPos = m_OwnerStartPos.x;
        int yPos = m_OwnerStartPos.y;
        int zPos = m_OwnerStartPos.z;
        for (int i = 0; i < posX.Length; i++)
        {
            m_MovePosArray[i] = new VInt3(xPos + posX[i] * xOffset, yPos + posY[i], zPos);
        }
        //m_MovePosArray[0] = new VInt3(xPos + xOffset, yPos + yOffset, zPos);
        //m_MovePosArray[1] = new VInt3(xPos + xOffset * 2, yPos + yOffset * 3, zPos);
        //m_MovePosArray[2] = new VInt3(xPos + xOffset * 3, yPos + yOffset, zPos);
        //m_MovePosArray[3] = new VInt3(xPos + xOffset * 4, yPos + yOffset * 3, zPos);
        //m_MovePosArray[4] = new VInt3(xPos + xOffset * 5, yPos + yOffset, zPos);
        //m_MovePosArray[5] = new VInt3(xPos + xOffset * 6, yPos + yOffset * 3, zPos);
        //m_MovePosArray[6] = new VInt3(xPos + xOffset * 7, yPos + yOffset, zPos);
        //m_MovePosArray[7] = new VInt3(xPos + xOffset * 8, yPos + yOffset * 2, zPos);
    }

    public override void OnEnterPhase(int phase)
    {
        base.OnEnterPhase(phase);
        if (phase < 10 && phase > 1)
        {
            VInt3 pos = new VInt3();
            if (phase > 1 && phase < 10)
            {
                pos = m_MovePosArray[phase - 2];
            }
            if (owner.CurrentBeScene.IsInBlockPlayer(pos))
            {
                BeAIManager.FindStandPositionNew(pos, owner.CurrentBeScene, owner.GetFace(), false, 30);
            }
            else
            {
                owner.SetPosition(pos);
            }
        }
        else if (phase == 10)
        {
            if (m_KillList.Count <= 0)
            {
                Cancel();
                owner.sgSwitchStates(new BeStateData((int)ActionState.AS_IDLE));
            }
        }
    }

    protected void KillOther()
    {
        m_KillHandle = owner.RegisterEventNew(BeEventType.onKill, args =>
        //m_KillHandle = owner.RegisterEvent(BeEventType.onKill, (object[] args) =>
        {
            if (!m_BoomFlag)
            {
                BeActor actor = args.m_Obj as BeActor;
                if (actor != null)
                {
                    actor.showDamageNumber = false;
                    actor.Pause(GlobalLogic.VALUE_10000);
                    if (actor.buffController != null)
                    {
                        actor.buffController.TryAddBuff(m_DeadFlagBuffId, GlobalLogic.VALUE_10000);
                    }
                    m_KillList.Add(actor);
#if !LOGIC_SERVER
                    GeEffectEx effect = actor.m_pkGeActor.CreateEffect(1012, Vec3.zero);
#endif
                }
            }
        });
    }

    protected void CreateBoomEntity(BeEntity entity)
    {
        m_CurrBoomNum++;
        if (m_CurrBoomNum < m_BoomMaxNum)
        {
            owner.AddEntity(m_BoomEntityId, GetBuffPos(entity));
        }
        entity.Resume();
        entity.DoDead();
    }

    protected VInt3 GetBuffPos(BeEntity entity)
    {
        VInt3 pos = entity.GetPosition();
        return new VInt3(pos.x, pos.y, pos.z);
    }

    protected void RemoveEffecct()
    {
        m_KillList.Clear();
        if(m_KillHandle!=null)
        {
            m_KillHandle.Remove();
        }
        if (m_BoomHandle != null)
        {
            m_BoomHandle.Remove();
        }
    }

    private void RestoreCamera()
    {
#if !LOGIC_SERVER
        if (!owner.isLocalActor) return;
        SetCameraPause(false);
#endif
    }

    public override void OnCancel()
    {
        RemoveEffecct();
        RestoreCamera();
    }

    public override void OnFinish()
    {
        RemoveEffecct();
        RestoreCamera();
    }
}
