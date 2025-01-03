using GameClient;

/// <summary>
/// 进入场景事件区域添加Buff，离开场景事件区域移除Buff
/// </summary>
public class Mechanism10008 : BeMechanism
{
    public Mechanism10008(int mid, int lv) : base(mid, lv) { }

    int mCamp;
    int[] mBuffIdArray;

    public override void OnStart()
    {
        //0：玩家  1：怪物  >1：所有
        mCamp = TableManager.GetValueFromUnionCell(data.ValueA[0], level);

        mBuffIdArray = new int[data.ValueBLength];
        for (int i = 0; i < data.ValueBLength; i++)
        {
            mBuffIdArray[i] = TableManager.GetValueFromUnionCell(data.ValueB[i], level);
        }

        var list = owner.CurrentBeScene.GetFullEntities();
        for (int i = 0; i < list.Count; i++)
        {
            BeActor actor = list[i] as BeActor;
            if (actor != null && (mCamp > 1 || actor.GetCamp() == mCamp))
            {
                AddHandle(actor.RegisterEventNew(BeEventType.onSummon, OnSummon));

                AddHandle(actor.RegisterEventNew(BeEventType.onEnterEventArea, OnEnterEventArea));
                AddHandle(actor.RegisterEventNew(BeEventType.onExitEventArea, OnExitEventArea));
            }
        }
    }

    void OnSummon(BeEvent.BeEventParam param)
    {
        BeActor actor = param.m_Obj as BeActor;
        if (actor != null)
        {
            AddHandle(actor.RegisterEventNew(BeEventType.onEnterEventArea, OnEnterEventArea));
            AddHandle(actor.RegisterEventNew(BeEventType.onExitEventArea, OnExitEventArea));
        }
    }

    void OnEnterEventArea(BeEvent.BeEventParam param)
    {
        BeActor actor = param.m_Obj as BeActor;
        if (actor != null)
        {
            for (int i = 0; i < mBuffIdArray.Length; i++)
            {
                actor.buffController.TryAddBuff(mBuffIdArray[i], -1, level);
            }
        }
    }

    void OnExitEventArea(BeEvent.BeEventParam param)
    {
        BeActor actor = param.m_Obj as BeActor;
        if (actor != null)
        {
            for (int i = 0; i < mBuffIdArray.Length; i++)
            {
                actor.buffController.RemoveBuff(mBuffIdArray[i]);
            }
        }
    }
}
