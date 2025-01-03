using GameClient;
using System.Collections.Generic;

/// <summary>
/// 怪物血量共享机制
/// 其中一个怪物受到伤害，另外一个也会受到伤害
/// </summary>
public class Mechanism10000 : BeMechanism
{
    public Mechanism10000(int mid, int lv) : base(mid, lv) { }

    int[] mMonsterIdArray;
    List<BeActor> mListMonster = new List<BeActor>();
    List<BeEvent.BeEventHandleNew> mListEventHandle = new List<BeEvent.BeEventHandleNew>();
    int mMonsterPID;
    int mHurtValue;

    public override void OnInit()
    {
        mMonsterIdArray = new int[data.ValueALength];
        for (int i = 0; i < data.ValueALength; i++)
        {
            mMonsterIdArray[i] = TableManager.GetValueFromUnionCell(data.ValueA[i], level);
        }
    }

    public override void OnReset()
    {
        mMonsterIdArray = null;
        mListMonster.Clear();
        mListEventHandle.Clear();
        mMonsterPID = 0;
        mHurtValue = 0;
    }

    public override void OnStart()
    {
        mMonsterPID = 0;
        var list = GamePool.ListPool<BeActor>.Get();
        mListMonster.Clear();
        for (int i = 0; i < mMonsterIdArray.Length; i++)
        {
            var mid = mMonsterIdArray[i];
            owner.CurrentBeScene.FindMonsterByID(list, mid);
            for (int j = 0; j < list.Count; j++)
            {
                mListMonster.Add(list[j]);
            }
        }

        for (int i = 0; i < mListMonster.Count; i++)
        {
            var monster = mListMonster[i];
            var handle = monster.RegisterEventNew(BeEventType.onHurt, _onMonsterGetHurt);
            mListEventHandle.Add(handle);
        }
    }

    private void _onMonsterGetHurt(BeEvent.BeEventParam args)
    {
        if (mMonsterPID == -1)
        {
            return;
        }
        if (mMonsterPID != 0)
        {
            mMonsterPID = -1;
            return;
        }

        mHurtValue = args.m_Int;
        var target = args.m_Obj as BeActor;
        if (target != null)
        {
            mMonsterPID = target.GetPID();
        }
    }

    public override void OnUpdate(int deltaTime)
    {
        if (mMonsterPID > 0)
        {
            int pid = mMonsterPID;
            mMonsterPID = -1;
            for (int i = 0; i < mListMonster.Count; i++)
            {
                var monster = mListMonster[i];
                if (monster.GetPID() != pid)
                {
                    monster.DoHurt(mHurtValue);
                }
            }
        }
        mMonsterPID = 0;
    }

    public override void OnFinish()
    {
        mMonsterIdArray = null;
        mListMonster.Clear();

        for (int i = 0; i < mListEventHandle.Count; i++)
        {
            mListEventHandle[i].Remove();
        }
        mListEventHandle.Clear();
    }
}
