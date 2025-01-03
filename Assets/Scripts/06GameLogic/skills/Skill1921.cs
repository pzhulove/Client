using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameClient;

public class Skill1921 : BeSkill
{
    IBeEventHandle handle1 = null;
    BeEvent.BeEventHandleNew handle2 = null;
    IBeEventHandle handle3 = null;
    IBeEventHandle handle4 = null;
    IBeEventHandle handle5 = null;
    GameClient.BeEvent.BeEventHandleNew handle6 = null;
    List<BeActor> hitList = new List<BeActor>();
    List<BeActor> killList = new List<BeActor>();
    int buffID = 192101;          //剪影效果
    int buffInfoID = 192105;     //怪物控制
    int effectId = 19211;   //触发效果Id
    string hitEffectPath = "Effects/Common/Sfx/Hit/Prefab/Eff_hit_kong";   //击中特效
    GeEffectEx baipingEffect = null;
    DelayCallUnitHandle delayCall ;
    bool hideFlag = false;
    int blackSceneID = -1;
    public Skill1921(int sid, int skillLevel) : base(sid, skillLevel)
    {

    }

    public override void OnInit()
    {
        base.OnInit();
    }

    public override void OnStart()
    {
        base.OnStart();
        //场景特效标签和最后爆炸伤害标签
        handle1 = owner.RegisterEventNew(BeEventType.onSkillCurFrame, (args) =>
        //handle1 = owner.RegisterEvent(BeEventType.onSkillCurFrame, (object[] args) =>
        {
            string flag = args.m_String;
#if !LOGIC_SERVER
            if (owner.isLocalActor)
            {
                if (flag == "true")
                {
                    _switchSceneEffect(true);
                }
                else if (flag == "false")
                {
                    hideFlag = false;
                    _switchSceneEffect(false);
                }
            }

            if (flag == "192101")
            {
                if (owner.m_pkGeActor != null)
                {
                    owner.m_pkGeActor.SetActorVisible(false);
                }
            }
            else if (flag == "192102")
            {
                if (owner.m_pkGeActor != null)
                {
                    owner.m_pkGeActor.SetActorVisible(true);
                }
            }
#endif
            if (flag == "19211")
            {
                int thirdType = owner.GetWeaponType();
                int hurtId = effectId;
                switch (thirdType)
                {
                    case 1:
                        hurtId = 19217;
                        break;
                    case 2:
                        hurtId = 19211;
                        break;
                    case 3:
                        hurtId = 19219;
                        break;
                    case 4:
                        hurtId = 19215;
                        break;
                }
                _OnHurtEntity(hurtId);
            }
        });

        //将所有被伤害的怪物收集起来，最后爆炸有用
        handle2 = owner.RegisterEventNew(BeEventType.onHitOther, (args) =>
        //handle2 = owner.RegisterEvent(BeEventType.onHitOther, (args) =>
        {
            BeActor actor = args.m_Obj as BeActor;
            if (actor != null && !hitList.Contains(actor))
            {
                hitList.Add(actor);
            }
        });

        handle3 = owner.RegisterEventNew(BeEventType.onKill, args =>
        //handle3 = owner.RegisterEvent(BeEventType.onKill, (args) =>
        {
            BeActor actor = args.m_Obj as BeActor;
            if (actor != null)
            {
                actor.Pause(GlobalLogic.VALUE_10000);
                killList.Add(actor);
            }
        });

        //添加剪影buff的时候，隐藏翅膀和其他怪物，显示白屏特效
        handle4 = owner.RegisterEventNew(BeEventType.onAddBuff, (args) =>
        {
            BeBuff buff = args.m_Obj as BeBuff;
            if (buff != null && buff.buffID == buffID)
            {
#if !LOGIC_SERVER
                if (owner.isLocalActor)
                {
                    //翅膀隐藏
                    owner.m_pkGeActor.SetAttachmentVisible("wing", false);
                    hideFlag = true;
                    AddJianYingBuff();
                    //显示白屏特效
                    Vec3 pos = owner.CurrentBeScene.GetSceneCenterPosition().vec3;
                    var vec = new Vector3(Screen.width / 2.0f, Screen.height / 2.0f, 0);
                    Ray ray = Camera.main.ScreenPointToRay(vec);
                    var t = -ray.origin.y / ray.direction.y;
                    var worldPos = ray.GetPoint(t);
                    var offsetPos = new Vec3(worldPos.x, owner.CurrentBeScene.logicZSize.fy, 0);

                    baipingEffect = owner.CurrentBeScene.currentGeScene.CreateEffect(1026, offsetPos);
                }
#endif
                TryAddControlBuff();
            }
        });

        //移除剪影BUFF的时候，把之前隐藏的显示出来
        handle5 = owner.RegisterEventNew(BeEventType.onRemoveBuff, (args) =>
        {
            int buffID = (int)args.m_Int;
            if (buffID == this.buffID)
            {
#if !LOGIC_SERVER

                if (owner.isLocalActor)
                {
                    owner.m_pkGeActor.SetAttachmentVisible("wing", true);

                    if (baipingEffect != null)
                    {
                        owner.CurrentBeScene.currentGeScene.DestroyEffect(baipingEffect);
                        baipingEffect = null;
                    }
                }
#endif
            }
        });

#if !LOGIC_SERVER
        handle6 = owner.RegisterEventNew(BeEventType.onChangeHitEffect, (GameClient.BeEvent.BeEventParam beEventParam) =>
        {
            int hurtId = beEventParam.m_Int;
            if (hurtId == effectId)
            {
                beEventParam.m_String = hitEffectPath;
            }
        });
#endif
    }

    List<BeEntity> hideList = new List<BeEntity>();
    public override void OnUpdate(int iDeltime)
    {
        base.OnUpdate(iDeltime);
#if !LOGIC_SERVER
        if (!owner.isLocalActor) return;
        if (hideFlag)
        {
            List<BeEntity> list = GamePool.ListPool<BeEntity>.Get();
            owner.CurrentBeScene.GetEntitys2(list);
            for (int i = 0; i < list.Count; i++)
            {
                if (!hideList.Contains(list[i]) &&
                    list[i] != null &&
                    list[i].m_pkGeActor != null &&
                    !list[i].m_pkGeActor.IsActorHide()
                    )
                    _hideOtherActor(list[i]);
            }
            GamePool.ListPool<BeEntity>.Release(list);
        }
        else
        {
            _showActor();
        }
#endif
    }

    private void _OnHurtEntity(int hurtID)
    {
        for (int i = 0; i < hitList.Count; i++)
        {
            var target = hitList[i];
            if (target == null || target.m_pkGeActor == null) continue;
            var hitPos = target.GetPosition();
            hitPos.z += VInt.one.i;
            owner._onHurtEntity(target, hitPos, hurtID);
        }
        hitList.Clear();

        owner.delayCaller.DelayCall(400, () =>
        {
            for (int i = 0; i < killList.Count; i++)
            {
                var target = killList[i];
                if (target == null || target.m_pkGeActor == null) continue;
                target.Resume();
                target.DoDead();
            }
            killList.Clear();
        });
    }

    private void TryAddControlBuff()
    {
        for (int i = 0; i < hitList.Count; i++)
        {
            if (hitList[i] != null && !hitList[i].IsDead())
            {
                hitList[i].buffController.TryAddBuff(buffInfoID);
            }
        }

    }

    //场景变黑特效
    private void _switchSceneEffect(bool flag)
    {
#if !LOGIC_SERVER
        if (owner.CurrentBeScene == null) return;
        if (flag)
        {
            blackSceneID = owner.CurrentBeScene.currentGeScene.BlendSceneSceneColor(Color.white*0.1f ,0.3f);
            delayCall = owner.delayCaller.DelayCall(300, () =>
            {
                if (canceled) return;
                owner.CurrentBeScene.currentGeScene.GetSceneRoot().CustomActive(false);
                owner.CurrentBeScene.currentGeScene.GetSceneActorRoot().CustomActive(false);
            });
        }
        else
        {
            owner.CurrentBeScene.currentGeScene.GetSceneRoot().CustomActive(true);
            owner.CurrentBeScene.currentGeScene.GetSceneActorRoot().CustomActive(true);
            owner.CurrentBeScene.currentGeScene.RecoverSceneColor(0.3f, blackSceneID);
        }
#endif
    }


    private void _recoverScene()
    {
        owner.CurrentBeScene.currentGeScene.GetSceneRoot().CustomActive(true);
        owner.CurrentBeScene.currentGeScene.GetSceneActorRoot().CustomActive(true);
        owner.CurrentBeScene.currentGeScene.RecoverSceneColor(0.01f, blackSceneID);
    }

    /// <summary>
    /// 1.击中的怪物上剪影，未击中的隐藏
    /// 2.除了自己的实体，其他的实体全部隐藏
    /// 3.技能实现的怪物隐藏
    /// </summary>
    /// <param name="flag"></param>
    private void _hideOtherActor(BeEntity entity)
    {
        if (entity == null  || entity.GetPID() == owner.GetPID()) return;

        BeActor actor = entity as BeActor;
        if (actor != null)
        {
            if(actor.GetEntityData()!= null)
            {
                if (BeUtility.IsMonsterIDEqual(actor.GetEntityData().monsterID, 30430011))
                    return;
            }
            if (actor.IsSkillMonster() || actor.GetCamp() == owner.GetCamp())
            {
                if (actor.m_pkGeActor != null)
                {
                    actor.m_pkGeActor.HideActor(true);
                    hideList.Add(entity);
                }
            }
            else
            {
                if (actor.GetCamp() != owner.GetCamp() && !hitList.Contains(actor))
                {
                    if (actor.m_pkGeActor != null)
                    {
                        actor.m_pkGeActor.HideActor(true);
                        hideList.Add(entity);
                    }
                }
            }
        }
        else
        {
            if (entity.GetOwner() != null && entity.GetOwner().GetPID() != owner.GetPID())
            {
                if (entity.m_pkGeActor != null)
                {
                    entity.m_pkGeActor.HideActor(true);
                    hideList.Add(entity);
                }
            }
        }
    }

    private void _showActor()
    {
        for (int i = 0; i < hideList.Count; i++)
        {
            BeEntity entity = hideList[i];
            if (entity != null &&
                entity.m_pkGeActor != null &&
                entity.m_pkGeActor.IsActorHide())
            {
                entity.m_pkGeActor.HideActor(false);
            }
        }
        hideList.Clear();
    }

    private void AddJianYingBuff()
    {
        for (int i = 0; i < hitList.Count; i++)
        {
            BeActor actor = hitList[i];
            if (actor == null || actor.m_pkGeActor == null) continue;
            actor.m_pkGeActor.ChangeSurface("寸拳", 1.5f);
        }
        if (owner != null && owner.m_pkGeActor != null)
            owner.m_pkGeActor.ChangeSurface("寸拳", 1.2f);
    }


    public override void OnCancel()
    {
        base.OnCancel();
        RemoveHandle();
    }

    public override void OnFinish()
    {
        base.OnFinish();
        RemoveHandle();
    }

    private void RemoveHandle()
    {
        if (handle1 != null)
        {
            handle1.Remove();
            handle1 = null;
        }

        if (handle2 != null)
        {
            handle2.Remove();
            handle2 = null;
        }
        if (handle3 != null)
        {
            handle3.Remove();
            handle3 = null;
        }
        if (handle4 != null)
        {
            handle4.Remove();
            handle4 = null;
        }

        if (handle5 != null)
        {
            handle5.Remove();
            handle5 = null;
        }
        if (handle6 != null)
        {
            handle6.Remove();
            handle6 = null;
        }
#if !LOGIC_SERVER
        if (owner.isLocalActor)
        {
            _showActor();
            hideFlag = false;
            _recoverScene();                               
            if (delayCall.IsValid())
            {
                delayCall.SetRemove(true);
            }
        }       
        
        if (owner.m_pkGeActor != null)
        {
            owner.m_pkGeActor.SetActorVisible(true);
        }
#endif
    }
}
