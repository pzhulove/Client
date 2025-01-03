using UnityEngine;

/// <summary>
/// Combo判定时间机制
/// </summary>
public class Mechanism2076 : BeMechanism
{
    private int[] mComboTimeArray = {0,  0};
    private int mComboTime = 0;

    public Mechanism2076(int mid, int lv) : base(mid, lv)
    {
    }

    public override void OnInit()
    {
        base.OnInit();
        if (data.ValueA.Count >= 2)
        {
            mComboTimeArray[0] = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
            mComboTimeArray[1] = TableManager.GetValueFromUnionCell(data.ValueA[1], level);
        }
    }

    public override void OnReset()
    {
        mComboTimeArray = new int[]{ 0,  0};
        mComboTime = 0;
    }

    public override void OnStart()
    {
        base.OnStart();
        InitData();

        if (mComboTime <= 0)
            return;

        if (owner != null && owner.actorData != null)
        {
            owner.actorData.SetComboIntervel(mComboTime);
        }
    }

    private void InitData()
    {
        mComboTime = !BattleMain.IsModePvP(battleType) ? mComboTimeArray[0] : mComboTimeArray[1];
    }

    public override void OnFinish()
    {
        base.OnFinish();
        if (owner != null && owner.actorData != null)
        {
            owner.actorData.ResetComboIntervel();
        }
    }
}