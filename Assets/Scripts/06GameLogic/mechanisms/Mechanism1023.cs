using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mechanism1023 : BeMechanism
{
    int skillID = 0;
    List<IBeEventHandle> handleList = new List<IBeEventHandle>();
    public Mechanism1023(int mid, int lv) : base(mid, lv) { }

    public override void OnInit()
    {
        base.OnInit();
        skillID = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
    }

    public override void OnReset()
    {
        RemoveHandleList();
    }

    public override void OnStart()
    {
        base.OnStart();

        List<BeActor> list = new List<BeActor>();
        owner.CurrentBeScene.FindMainActor(list);
        for (int i = 0; i < list.Count; i++)
        {
            BeActor actor = list[i];
            IBeEventHandle handle = actor.RegisterEventNew(BeEventType.OnUseCrystal, (args) =>
             {
                 bool faceLeft = actor.GetFace();
                 float offset = faceLeft ? 1.0f : -1.0f;
                 owner.SetFace(faceLeft);
                 owner.SetPosition(actor.GetPosition() + new VInt3(offset, 0, 0), true);

                 if (owner.CanUseSkill(skillID))
                 {
                     owner.UseSkill(skillID);
                 }
             });
            handleList.Add(handle);
        }
    }

    private void RemoveHandleList()
    {
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

    public override void OnDead()
    {
        base.OnDead();
        RemoveHandleList();
    }

    public override void OnFinish()
    {
        base.OnFinish();
        RemoveHandleList();
    }
}
