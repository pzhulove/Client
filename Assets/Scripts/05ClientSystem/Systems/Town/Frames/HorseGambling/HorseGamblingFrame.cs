using System;
using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine.UI;
using Protocol;

namespace GameClient
{
    class HorseGamblingFrame : ClientFrame
    {
	    private HorseGamblingView mView;
	    private bool mIsNeedUpdate = false;
	    private float mUpdatedelta = 0f;
	    public override string GetPrefabPath()
	    {
		    return "UIFlatten/Prefabs/HorseGambling/HorseGamblingFrame";
	    }

	    protected override void _OnOpenFrame()
	    {
		    BindEvents();
		    mView = frame.GetComponent<HorseGamblingView>();
		    if (mView != null)
		    {
			    mView.Init(HorseGamblingDataManager.GetInstance(), ShowSupply, ShowStakeRecords, ShowGameRecords, ShowShooterRecord, OnClose);
			    mIsNeedUpdate = true;
			    mUpdatedelta = 0;
				HorseGamblingDataManager.GetInstance().RequestData();
			    HorseGamblingDataManager.GetInstance().RequestStakeHistory();
			    HorseGamblingDataManager.GetInstance().RequestGameHistory();

			}
		}

	    public override bool IsNeedUpdate()
	    {
		    return mIsNeedUpdate;
	    }

	    protected override void _OnUpdate(float timeElapsed)
	    {
		    if (mView != null && HorseGamblingDataManager.GetInstance().State == BetHorsePhaseType.PHASE_TYPE_STAKE)
		    {
			    mUpdatedelta += timeElapsed;
			    if (mUpdatedelta >= mView.RefreshOddsInterval)
			    {
				    HorseGamblingDataManager.GetInstance().RequestShooterOdds();
				    mUpdatedelta -= mView.RefreshOddsInterval;
			    }
		    }
	    }

		protected override void _OnCloseFrame()
        {
	        UnBindEvents();
	        mIsNeedUpdate = false;
	        mUpdatedelta = 0;
	        if (mView != null)
	        {
				mView.Dispose();
		        mView = null;
	        }
        }

		void BindEvents()
	    {
		    UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.HorseGamblingGameStateUpdate, OnStateUpdate);
		    UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.HorseGamblingUpdate, OnDataUpdate);
		    UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.HorseGamblingOddsUpdate, OnOddsUpdate);
		    UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.HorseGamblingShooterHistoryUpdate, OnShooterHistoryUpdate);
		}

	    void UnBindEvents()
	    {
		    UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.HorseGamblingGameStateUpdate, OnStateUpdate);
		    UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.HorseGamblingUpdate, OnDataUpdate);
		    UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.HorseGamblingOddsUpdate, OnOddsUpdate);
		    UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.HorseGamblingShooterHistoryUpdate, OnShooterHistoryUpdate);
	    }

	    void OnClose()
	    {
		    Close();
	    }

		void OnStateUpdate(UIEvent data)
	    {
		    if (mView != null && data != null && data.Param1 != null)
		    {
				mView.UpdateState((BetHorsePhaseType) data.Param1);
		    }

			HorseGamblingDataManager.GetInstance().RequestData();
	    }
		void OnDataUpdate(UIEvent data)
		{
			if (mView != null)
			{
				mView.UpdateData(HorseGamblingDataManager.GetInstance());
			}
		}

	    void ShowSupply()
	    {
		    if (mView != null)
		    {
			    ClientSystemManager.GetInstance().OpenFrame<HorseGamblingSupplyFrame>(FrameLayer.Middle, mView.SelectShooterId);
		    }
		}

		void ShowStakeRecords()
	    {
		    HorseGamblingRecordFrameParam param = new HorseGamblingRecordFrameParam();
		    param.RecordType = EHorseGamblingRecord.Stake;
			ClientSystemManager.GetInstance().OpenFrame<HorseGamblingRecordFrame>(FrameLayer.Middle, param);
	    }

	    void ShowGameRecords()
	    {
		    HorseGamblingRecordFrameParam param = new HorseGamblingRecordFrameParam();
		    param.RecordType = EHorseGamblingRecord.HistoryAndRank;
		    param.Param = 0;
		    ClientSystemManager.GetInstance().OpenFrame<HorseGamblingRecordFrame>(FrameLayer.Middle, param);
	    }

	    void ShowShooterRecord()
	    {
		    if (mView != null)
		    {
			    HorseGamblingRecordFrameParam param = new HorseGamblingRecordFrameParam();
			    param.RecordType = EHorseGamblingRecord.ShooterHistory;
			    param.Param = mView.SelectShooterId;
			    ClientSystemManager.GetInstance().OpenFrame<HorseGamblingRecordFrame>(FrameLayer.Middle, param);
		    }
		}

		void OnOddsUpdate(UIEvent data)
	    {
		    if (mView != null)
		    {
				mView.UpdateOdds(HorseGamblingDataManager.GetInstance());
		    }
	    }
	    void OnShooterHistoryUpdate(UIEvent data)
	    {
		    if (mView != null && data != null && data.Param1 != null)
		    {
			    mView.UpdateShooterInfo((IHorseGamblingShooterModel)data.Param1);
		    }
	    }
	}
}