using Protocol;
using UnityEngine;

namespace GameClient
{
    public sealed class BossQuestActivity : LimitTimeCommonActivity
    {

	    public override void Init(uint activityId)
	    {
		    var data = ActivityDataManager.GetInstance().GetBossActivityData(activityId);
		    if (data != null)
		    {
				mDataModel = new BossQuestModel(data);
			}
	    }

	    public override void UpdateData()
		{
			if (mDataModel == null)
				return;
			var data = ActivityDataManager.GetInstance().GetBossActivityData(mDataModel.Id);
		    if (data != null)
		    {
			    mDataModel = new BossQuestModel(data);
			    if (mView != null)
			    {
				    mView.UpdateData(mDataModel);
			    }
		    }
	    }

	    protected override void _OnItemClick(int taskId, int param,ulong param2)
	    {
		    ActivityDataManager.GetInstance().SendSubmitBossExchangeTask(taskId);
	    }
	}
}