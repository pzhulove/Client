using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
	public class HorseGamblingBulletView : MonoBehaviour
	{
		[SerializeField] private int mBulletItemId;
		[SerializeField] private Text mTextCount;
		[SerializeField] private Button mButtonAdd;

		void Awake()
		{
			mButtonAdd.SafeAddOnClickListener(OnAddClick);
			UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ItemTakeSuccess, OnChange);
			UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ItemCountChanged, OnChange);
			mTextCount.SafeSetText(Utility.ToThousandsSeparator((ulong)ItemDataManager.GetInstance().GetOwnedItemCount(mBulletItemId, false)));
		}

        void OnDestroy()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ItemTakeSuccess, OnChange);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ItemCountChanged, OnChange);
        }

		void OnChange(UIEvent uiEvent)
		{
			mTextCount.SafeSetText(Utility.ToThousandsSeparator((ulong)ItemDataManager.GetInstance().GetOwnedItemCount(mBulletItemId, false)));
		}

		void OnAddClick()
		{
			ClientSystemManager.GetInstance().CloseFrame<HorseGamblingSupplyFrame>();
			ClientSystemManager.GetInstance().OpenFrame<HorseGamblingExchangeFrame>();
		}
	}
}