using Protocol;
using ProtoTable;

namespace GameClient
{
    class HorseGamblingSupplyFrame : ClientFrame
    {
	    private HorseGamblingSupplyView mView;
	    private int mShooterId;
	    public override string GetPrefabPath()
	    {
		    return "UIFlatten/Prefabs/HorseGambling/HorseGamblingSupplyFrame";
	    }

	    protected override void _OnOpenFrame()
	    {
		    mView = frame.GetComponent<HorseGamblingSupplyView>();
		    if (mView != null)
		    {
			    int shooterId = (int) userData; 
			    mShooterId = shooterId;
				var tableData = TableManager.GetInstance().GetTableItem<BetHorseShooter>(shooterId);


			    var shooter = HorseGamblingDataManager.GetInstance().GetShooterModel(mShooterId);
			    if (shooter != null && shooter.IsUnknown && HorseGamblingDataManager.GetInstance().State == BetHorsePhaseType.PHASE_TYPE_STAKE)
			    {
				    tableData = TableManager.GetInstance().GetTableItem<BetHorseShooter>(0);
			    }

			    if (tableData == null)
			    {
					return;
			    }


				mView.Init(tableData.Name, tableData.IconPath, HorseGamblingDataManager.GetInstance().GetShooterOdds(shooterId), HorseGamblingDataManager.GetInstance().LeftSupply, OnSupply, OnClose);
			}
			HorseGamblingDataManager.GetInstance().RequestStakeHistory();
		    UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.HorseGamblingShooterStakeResp, OnSupplyResp);
		    UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.HorseGamblingShooterStakeUpdate, OnStakeDataResponse);
		}


		protected override void _OnCloseFrame()
        {
	        UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.HorseGamblingShooterStakeResp, OnSupplyResp);
	        UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.HorseGamblingShooterStakeUpdate, OnStakeDataResponse);
	        if (mView != null)
	        {
				mView.Dispose();
		        mView = null;
	        }
        }

	    void OnClose()
	    {
			Close();
	    }

	    void OnStakeDataResponse(UIEvent uiEvent)
	    {
		    if (mView != null)
		    {
			    mView.UpdateStakeNum(HorseGamblingDataManager.GetInstance().LeftSupply);

		    }
	    }

	    void OnSupply()
	    {
		    if (mView != null)
		    {
			    if (HorseGamblingDataManager.GetInstance().State != BetHorsePhaseType.PHASE_TYPE_STAKE)
			    {
				    SystemNotifyManager.SystemNotify(9915);
				    return;
			    }

			    if (mView.SupplyCount == 0)
			    {
				    SystemNotifyManager.SystemNotify(9925);
				    return;
			    }

			    var bulletCount = ItemDataManager.GetInstance().GetOwnedItemCount(mView.BulletId, false);

				if (bulletCount == 0 || bulletCount < mView.SupplyCount)
			    {
				    SystemNotifyManager.SystemNotify(9928);
				    return;
			    }

			    if (mView.SupplyCount > HorseGamblingDataManager.GetInstance().LeftSupply)
			    {
				    SystemNotifyManager.SystemNotify(9930);
				    return;
			    }
				mView.SetConfirmEnable(false);
				HorseGamblingDataManager.GetInstance().ShooterStake(mShooterId, mView.SupplyCount);
		    }
		}
	    void OnSupplyResp(UIEvent uiEvent)
	    {
		    if (uiEvent != null && uiEvent.Param1 != null)
		    {
			    int code = (int)uiEvent.Param1;
			    if (mView != null)
				    mView.SetConfirmEnable(true);
			    switch (code)
			    {
					case (int)ProtoErrorCode.SUCCESS:
						if (mView != null)
							mView.SetConfirmEnable(false);
						SystemNotifyManager.SystemNotify(9919);
					    Close();
						break;
					case (int)ProtoErrorCode.BET_HORSE_NOT_STAKE_PHASE:
						SystemNotifyManager.SystemNotify(9915);
						break;
				    case (int)ProtoErrorCode.BET_HORSE_STAKE_MAX:
					    SystemNotifyManager.SystemNotify(9916);
					    break;
				    case (int)ProtoErrorCode.BET_HORSE_STAKE_FAILED:
					    SystemNotifyManager.SystemNotify(9917);
					    break;
				    case (int)ProtoErrorCode.BET_HORSE_SHOOTER_NOT_JOIN:
					    SystemNotifyManager.SystemNotify(9918);
					    break;
				}
		    }
	    }
	}
}