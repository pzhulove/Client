using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 技能再次点击控制怪物释放技能
public class Mechanism10022 : BeMechanism
{
    private int mSkillId = 0;
    private int monsterID = 0;
    private int monsterSkillID = 0;
    private bool isResetButtonState = false;
    List<BeActor> mMonsters = new List<BeActor>();
    
    public Mechanism10022(int mid, int lv) : base(mid, lv) { }
    
    
    public override void OnInit()
    {
        mSkillId = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        monsterID = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
        monsterSkillID = TableManager.GetValueFromUnionCell(data.ValueC[0], level);
        var temp = TableManager.GetValueFromUnionCell(data.ValueD[0], level);
        if (temp == 1)
        {
            isResetButtonState = true;
        }
    }

    public override void OnStart()
    {
        handleA = owner.RegisterEventNew(BeEventType.onCastSkill, (args) =>
        {
            int skillID = args.m_Int;

            if (mSkillId == skillID)
            {
                SetButtonState(true);
                var skill = owner.GetSkill(mSkillId);
                if (skill != null)
                {
                    skill.pressMode = SkillPressMode.PRESS_JOYSTICK;
                    skill.SetIgnoreCD(true);
                }
            }
            
        });
        handleB = owner.RegisterEventNew(BeEventType.onClickAgain, (args) =>
        {
            int skillID = ((BeSkill) args.m_Obj).skillID;
            
            if (mSkillId == skillID)
            {
                if (UseSkill())
                {
                    if (isResetButtonState)
                    {
                        SetButtonState(false);
                        var skill = owner.GetSkill(mSkillId);
                        if (skill != null)
                        {
                            skill.SetIgnoreCD(false);
                            skill.StartCoolDown();
                        }
                    }
                }
                var skill2 = owner.GetSkill(mSkillId);
                if (skill2 != null)
                    skill2.SetInnerState(BeSkill.InnerState.LAUNCH);
            }
        });
        
        /*sceneHandleA = owner.CurrentBeScene.RegisterEventNew(BeEventSceneType.onSummon, (args) =>
        {
            BeActor monster = (BeActor)args.m_Obj;
            BeActor monsterOwner = (BeActor)args.m_Obj2;
            if (monsterOwner.GetPID() == owner.GetPID())
            {
                AddMonster(monster);
            }
        });*/
        
        handleD = owner.RegisterEventNew(BeEventType.onSummon, (args) =>
        {
            BeActor monster = args.m_Obj as BeActor;
            AddMonster(monster);
        });
    }
    
    void AddMonster(BeActor actor)
    {
        if (owner != null && actor != null && actor.GetEntityData().MonsterIDEqual(monsterID))
        {
            mMonsters.Add(actor);
            var handle1 = actor.RegisterEventNew(BeEventType.onDead, (args2) =>
            {
                mMonsters.Remove(actor);
                if (mMonsters.Count == 0)
                {
                    SetButtonState(false);
                    var skill = owner.GetSkill(mSkillId);
                    if (skill != null)
                    {
                        skill.SetIgnoreCD(false);
                        skill.StartCoolDown();
                    }
                }
            });
            
            var currSkill = owner.GetSkill(mSkillId);
            if (currSkill != null)
            {
                currSkill.SetInnerState(BeSkill.InnerState.LAUNCH);
        
                /*VInt3 projectPos = actor.GetPosition();
                VInt3 effectPos = currSkill.GetEffectPos();

                if (owner.GetFace())
                    projectPos.x = effectPos.x;
                else
                    projectPos.x = effectPos.x;
                projectPos.y = effectPos.y;
                        
                if (owner.CurrentBeScene.IsInBlockPlayer(projectPos))
                {
                    var pos  = BeAIManager.FindStandPositionNew(projectPos, owner.CurrentBeScene, false, false, 50);
                    projectPos.x = pos.x;
                    projectPos.y = pos.y;
                }
                actor.SetPosition(projectPos);
                actor.SetFace(owner.GetFace(), true);*/
            }
        }
    }
    
    void ClearData()
    {
        mMonsters.Clear();
    }

    private bool UseSkill()
    {
        bool flag = false;
        if (owner != null)
        {
            //List<BeActor> monsters = GamePool.ListPool<BeActor>.Get();
            //owner.CurrentBeScene.FindMonsterByID(monsters, monsterID, false);
            for(int i=0; i < mMonsters.Count; ++i)
            {
                var sidetick = mMonsters[i];
                if (sidetick == null || sidetick.IsDead())
                    continue;
                
                if (monsterSkillID > 0 && (!sidetick.IsInPassiveState() || sidetick.IsCastingSkill()))
                {
                    var skill = sidetick.GetSkill(monsterSkillID);
                    if (skill != null)
                    {
                        skill.ResetCoolDown();
                        sidetick.UseSkill(monsterSkillID, true);
                        flag = true;
                    }
                }

                if (monsterSkillID == 0)
                {
                    sidetick.DoDead();
                }
            }
            //GamePool.ListPool<BeActor>.Release(monsters);
        }

        return flag;
    }

    public void SetButtonState(bool isVisible)
    {
        if (owner != null)
        {
            var skill = owner.GetSkill(mSkillId);
            if (skill != null)
                skill.SetLightButtonVisible(isVisible);
        }
    }
    
    public override void OnReset()
    {
        SetButtonState(false);
        ClearData();
    }
    public override void OnFinish()
    {
        SetButtonState(false);
        ClearData();
    }
}
