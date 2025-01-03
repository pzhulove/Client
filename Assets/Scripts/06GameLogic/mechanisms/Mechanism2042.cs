using System;
using System.Collections.Generic;
using GameClient;

//夜光效果
public class Mechanism2042 : BeMechanism
{
    List<int> monsters = new List<int>();
    int buffID = 0;
    BeMonsterIDFilter filter = null;
    public Mechanism2042(int sid, int skillLevel) : base(sid, skillLevel)
    {

    }
    public override void OnInit()
    {
        base.OnInit();
        for (int i = 0; i < data.ValueA.Count; i++)
        {
            monsters.Add(TableManager.GetValueFromUnionCell(data.ValueA[i], level));
        }
        buffID = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
        filter = new BeMonsterIDFilter(monsters);
    }

    public override void OnReset()
    {
        monsters.Clear();
    }

    public override void OnStart()
    {
        base.OnStart();
        if (owner == null || owner.CurrentBeScene == null) return;
        handleA = owner.CurrentBeScene.RegisterEventNew(BeEventSceneType.onMouthClose, OnMouthClose);
    }
    private void OnMouthClose(BeEvent.BeEventParam args)
    {
        if (owner == null || owner.CurrentBeScene == null) return;
        bool isClosed = args.m_Bool;
        List<BeActor> list = GamePool.ListPool<BeActor>.Get();
        owner.CurrentBeScene.FindTargets(list, owner, true, filter);
        if (isClosed)
        {
           for(int i = 0; i < list.Count;i++)
           {
                var curActor = list[i];
                if(curActor != null && curActor.buffController != null)
                {
                    curActor.buffController.TryAddBuff(buffID);
                }
           }
        }
        else
        {
            for (int i = 0; i < list.Count; i++)
            {
                var curActor = list[i];
                if (curActor != null && curActor.buffController != null)
                {
                    curActor.buffController.RemoveBuff(buffID);
                }
            }
        }
        GamePool.ListPool<BeActor>.Release(list);
    }
}

