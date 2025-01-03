using System;

/// <summary>
/// 身上有某buff，指定触发效果不生效
/// </summary>
public class Mechanism10001 : BeMechanism
{
    public Mechanism10001(int mid, int lv) : base(mid, lv) { }

    int mBuffId;
    int[] mHurtIdArray;

    public override void OnInit()
    {
        mBuffId = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        mHurtIdArray = new int[data.ValueBLength];
        for (int i = 0; i < data.ValueBLength; i++)
        {
            mHurtIdArray[i] = TableManager.GetValueFromUnionCell(data.ValueB[i], level);
        }
    }

    public override void OnStart()
    {
        if (owner.CurrentBeScene != null)
        {
            handleA = owner.CurrentBeScene.RegisterEventNew(BeEventSceneType.onHurtEntity, args =>
            {
                var attacker = args.m_Obj as BeActor;
                var target = args.m_Obj2 as BeActor;
                if (target.GetPID() == owner.GetPID())
                {
                    var hurtId = args.m_Int;
                    if (Array.IndexOf(mHurtIdArray, hurtId) >= 0 &&
                        owner.buffController.HasBuffByID(mBuffId) != null)
                    {
                        args.m_Int = 0;
                    }
                }
            });
        }
    }
}
