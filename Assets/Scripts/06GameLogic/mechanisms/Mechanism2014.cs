using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 周长深渊冰雪女王监听是否玩家是否被冰冻
/// </summary>
public class Mechanism2014 : BeMechanism
{
    readonly int buffID = 521803;
    readonly int skillID = 20211;
    readonly int castSkillID = 20213;
    List<IBeEventHandle> handleList = new List<IBeEventHandle>();
    public Mechanism2014(int mid, int lv) : base(mid, lv) { }

    public override void OnReset()
    {
        RemoveHandleList();
    }

    public override void OnStart()
    {
        base.OnStart();
        List<BeActor> list = GetMainActorList();
        for (int i = 0; i < list.Count; i++)
        {
            IBeEventHandle handle = list[i].RegisterEventNew(BeEventType.onAddBuff, (args) => 
            {
                //如果玩家都上了冰冻buff那么就放终结技能
                BeBuff buff = args.m_Obj as BeBuff;
                if (buff.buffID == buffID )
                {
                    if (owner.sgGetCurrentState() != (int)ActionState.AS_CASTSKILL || (owner.sgGetCurrentState() == (int)ActionState.AS_CASTSKILL && owner.GetCurSkillID() != skillID))
                    {
                        int cnt = GetMainActorList().Count;
                        if (cnt == 1)
                        {
                            owner.UseSkill(castSkillID);
                        }
                        else
                        {
                            if (AllHaveFreezeBuff())
                            {
                                owner.UseSkill(castSkillID);
                            }
                        }
                    }
                }
            });
            handleList.Add(handle);
        }
    }

    /// <summary>
    /// 是否所有玩家有冰冻buff
    /// </summary>
    /// <returns></returns>
    private bool AllHaveFreezeBuff()
    {
        List<BeActor> list = GetMainActorList();
        bool flag = true;
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].buffController.HasBuffByID(buffID) == null)
                flag = false;
        }
        return flag;
    }

    private List<BeActor> GetMainActorList()
    {
        List<BeActor> list = new List<BeActor>();
        owner.CurrentBeScene.FindMainActor(list);
        return list;
    }

    public override void OnDead()
    {
        RemoveHandleList();
        base.OnDead();
    }

    public override void OnFinish()
    {
        RemoveHandleList();
        base.OnFinish();
    }

    private void RemoveHandleList()
    {
        for (int i = 0; i < handleList.Count; i++)
        {
            if (handleList[i]!=null)
            {
                handleList[i].Remove();
                handleList[i] = null;
            }
        }
        handleList.Clear();
    }
}
