using Protocol;
using ProtoTable;

namespace GameClient
{
    class HorseGamblingExchangeFrame : ClientFrame
    {
	    private HorseGamblingExchangeView mView;
	    private int mShooterId;
	    public override string GetPrefabPath()
	    {
		    return "UIFlatten/Prefabs/HorseGambling/HorseGamblingExchangeFrame";
	    }

	    protected override void _OnOpenFrame()
	    {
		    mView = frame.GetComponent<HorseGamblingExchangeView>();
		    if (mView != null)
		    {
			    mView.Init(OnBuy, OnClose);
			}
		    UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.HorseGamblingBuyBulletResponse, OnBuyResp);
		}

		protected override void _OnCloseFrame()
        {
	        UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.HorseGamblingBuyBulletResponse, OnBuyResp);
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

	    void OnBuy()
	    {
		    if (mView != null)
		    {
			    var goldCount = ItemDataManager.GetInstance().GetOwnedItemCount(mView.GoldItemId, false);

			    if (goldCount < mView.ExchangeRate * mView.ExchangeCount)
			    {
				    SystemNotifyManager.SystemNotify(9929);
				    return;
			    }

			    if (mView.ExchangeCount > 0)
			    {
					mView.SetConfirmEnable(false);
				    HorseGamblingDataManager.GetInstance().ExchangeBullet(mView.ExchangeCount);
			    }
			    else
				{
					SystemNotifyManager.SystemNotify(9926);
				}
			}
		}

	    void OnBuyResp(UIEvent uiEvent)
	    {
		    if (uiEvent != null && uiEvent.Param1 != null)
		    {
			    int code = (int) uiEvent.Param1;
			    if (code == 0)
			    {
				    Close();
			    }
			    else
			    {
				    if (mView != null)
				    {
					    mView.SetConfirmEnable(true);
				    }
				}
			}
	    }
    }
}