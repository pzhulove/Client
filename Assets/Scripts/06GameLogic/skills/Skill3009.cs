using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

////念气怪物继承念气师的技能
public class Skill9701 : Skill3009
{
   public Skill9701 (int sid,int skillLevel) : base(sid, skillLevel)
   {

   }
}

//1.5版本注释掉背摔功能 但是念气怪物的背摔保留
//格斗家 - 背摔
public class Skill3009 : BeSkill
{
    protected BeActor m_grabActor = null;       //抓取的目标
    protected bool m_attachToFlag = false;

#if !LOGIC_SERVER
    protected GameObject m_GeGrapObj = null;
    protected GameObject m_GeGrapParentObj = null;
#endif

    protected int m_BeiShuaiId = 30092;
    protected int m_RaySkillId = 30093;

    IBeEventHandle m_ReplaceSkillHandle = null;
    protected GameObject attachRoot = null;


    public Skill3009(int sid, int skillLevel) : base(sid, skillLevel)
    {

    }

    public override void OnInit()
    {

    }

    public override void OnStart()
    {

#if !LOGIC_SERVER
        m_GeGrapObj = null;
        m_GeGrapParentObj = null;
#endif

        m_attachToFlag = false;
        owner.grabController.IsSuplexGrab = true;
        m_ReplaceSkillHandle = owner.RegisterEventNew(BeEventType.onPreSetSkillAction, (GameClient.BeEvent.BeEventParam param) =>
        {
            if (owner.HasSkill(3108))
            {
                //int[] skillIdList = (int[])args[0];
                int curSkillId = param.m_Int;
                if (curSkillId == m_BeiShuaiId)
                {
                    param.m_Int = m_RaySkillId;
                }
            }
        });
    }

    public override void OnUpdate(int iDeltime)
    {
        List<BeActor> GrabbedActorList = owner.grabController.grabbedActorList;
        if (GrabbedActorList.Count > 0)
        {
            m_grabActor = GrabbedActorList[0];
            if (m_grabActor != null)
            {
                if (!m_attachToFlag)
                {
                    m_attachToFlag = true;
                    if (m_grabActor.aiManager != null)
                    {
                        m_grabActor.aiManager.Stop();
                    }


#if !LOGIC_SERVER
                    if (m_grabActor.m_pkGeActor != null && !m_grabActor.m_pkGeActor.charactorRootIsNull)
                    {
                        try
                        {
                            if (m_grabActor != null && m_grabActor.m_pkGeActor != null)
                                m_grabActor.m_pkGeActor.charactorRootIsNull = true;
                            float height = m_grabActor.m_pkGeActor.GetOverHeadPosition().y;
                            m_GeGrapObj = m_grabActor.m_pkGeActor.GetEntityNode(GeEntity.GeEntityNodeType.Charactor);
                            m_GeGrapParentObj = m_GeGrapObj.transform.parent.gameObject;
                            attachRoot = owner.m_pkGeActor.GetAttachNode("[actor]RWeapon");
                            if (attachRoot != null)
                            {
                                Battle.GeUtility.AttachTo(m_GeGrapObj, attachRoot, false);
                                attachRoot.transform.localRotation = Quaternion.Euler(new Vector3(-90, -90, -90));
                            }

                            owner.delayCaller.DelayCall(5, () =>
                            {
                                try
                                {
                                    if (m_GeGrapObj != null)
                                    {
                                        var scale = m_GeGrapObj.transform.localScale;
                                        if (owner.GetFace())
                                            scale.x *= -1;

                                        m_GeGrapObj.transform.localScale = scale;
                                    }
                                }
                                catch (Exception e)
                                {
                                    Logger.LogError(e.Message);
                                }

                            });

                            m_GeGrapObj.transform.localPosition = new Vector3(0, -height / 2, 0);
                        }
                        catch (Exception e)
                        {
                            Logger.LogError(e.Message);
                        }
                    }

#endif
                        SetStartGrapLogicPos();

                        owner.delayCaller.DelayCall(700, () =>
                        {
                            ChangeGrapedLogicPos();
                        });
                    }
            }

        }
    }

    protected void SetStartGrapLogicPos()
    {
        if (m_grabActor == null)
            return;

        VInt3 ownerPos = owner.GetPosition();
        int xPos = owner.GetFace() ? ownerPos.x - VInt.Float2VIntValue(0.3f) : ownerPos.x + VInt.Float2VIntValue(0.3f);
        SetNewPos(m_grabActor, new VInt3(xPos, ownerPos.y, ownerPos.z));
    }

    //更改被抓取者的逻辑位置
    protected void ChangeGrapedLogicPos()
    {
        if (m_grabActor == null)
            return;
        //         if (m_GrapActor.aiManager != null)
        //         {
        //             m_GrapActor.aiManager.Start();
        //         }
        bool ownerFace = owner.GetFace();
        VInt3 ownerPos = owner.GetPosition();
        int xPos = ownerFace ? ownerPos.x + VInt.Float2VIntValue(0.6f) : ownerPos.x - VInt.Float2VIntValue(0.6f);
        SetNewPos(m_grabActor, new VInt3(xPos, ownerPos.y, ownerPos.z));
    }

    public override void OnCancel()
    {
        // Logger.LogErrorFormat("OnCancel!!!!!!!!!!!!!");
        RestoreGrapedPos(true);
        Remove();
    }

    public override void OnFinish()
    {
        //   Logger.LogErrorFormat("OnFinish!!!!!!!!!!!!!");
        RestoreGrapedPos();
        Remove();
    }

    //重置被抓取怪物的位置
    protected void RestoreGrapedPos(bool interput = false)
    {
        if (m_attachToFlag)
        {
            if (m_grabActor != null)
            {
                if (interput)
                {
                    //m_grapActor.Locomote(new BeStateData((int)ActionState.AS_FALL, 0, 0, VInt.one.i, 0, 0, GlobalLogic.VALUE_300), true);
                    m_grabActor.Locomote(new BeStateData((int)ActionState.AS_FALL) { _StateData3 = VInt.one.i, _timeout = GlobalLogic.VALUE_300 }, true);
                }

                if (m_grabActor.aiManager != null)
                    m_grabActor.aiManager.Start();
            }

        }

        // Logger.LogErrorFormat("RestoreGrapedPos!!!!!!!!!!!");


#if !LOGIC_SERVER
        if (m_GeGrapObj != null && attachRoot != null)
        {
            if (m_grabActor != null && m_grabActor.m_pkGeActor != null)
                m_grabActor.m_pkGeActor.charactorRootIsNull = false;
            attachRoot.transform.localRotation = Quaternion.Euler(new Vector3(-90, 0, -90));
            m_GeGrapObj.transform.localPosition = Vector3.zero;
            m_GeGrapObj.transform.localRotation = Quaternion.Euler(Vector3.zero);
            if (m_GeGrapParentObj == null)
            {
                // 变身后的怪物可能立即死亡移除了，导致没有父节点
                if (m_grabActor != null && m_grabActor.IsRemoved())
                {
                    GameObject.Destroy(m_GeGrapObj);
                }
            }
            else
            {
                Battle.GeUtility.AttachTo(m_GeGrapObj, m_GeGrapParentObj);
            }
            m_GeGrapObj = null;
            m_GeGrapParentObj = null;
        }
#endif

    }

    protected void SetNewPos(BeActor actor, VInt3 newPos)
    {
        if (actor == null || actor.IsDead())
            return;
        if (actor.CurrentBeScene.IsInBlockPlayer(newPos))
        {
            var pos = BeAIManager.FindStandPositionNew(newPos,actor.CurrentBeScene);
            actor.SetPosition(pos);
        }
        else
        {
            actor.SetPosition(newPos);
        }
    }

    protected void Remove()
    {
        m_attachToFlag = false;
        owner.grabController.IsSuplexGrab = false;
        if (m_ReplaceSkillHandle != null)
        {
            m_ReplaceSkillHandle.Remove();
            m_ReplaceSkillHandle = null;
        }
    }
}
