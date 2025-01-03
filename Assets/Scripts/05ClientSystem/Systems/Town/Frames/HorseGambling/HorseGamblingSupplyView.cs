using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GameClient
{

	public class HorseGamblingSupplyView : MonoBehaviour,IDisposable
	{
		[SerializeField] private Text mTextName;
		[SerializeField] private Image mImageIcon;
		[SerializeField] private Text mTextOdds;
		[SerializeField] private Text mTextSupplyTip;
		[SerializeField] private Text mTextSupplyCount;
		//[SerializeField] private Button mButtonAddBullet;
		[SerializeField] private Button mButtonMax;
		//[SerializeField] private Button mButtonMin;
		[SerializeField] private Button mButtonCountInput;
		//[SerializeField] private Button mButtonKeyBoard;
		//[SerializeField] private Scrollbar mScrollBar;
		[SerializeField] private Button mButtonConfirm;
		[SerializeField] private Button mButtonClose;

		[SerializeField] private int mBulletItemId;
		[SerializeField] private float mKeyBoardOffsetX = 100;

		public int BulletId
		{
			get { return mBulletItemId; }
		}

		public int SupplyCount { get; private set; }
		//判断是否打开输入框界面后第一次输入，如果是 则会清除掉当前的数字
		bool mIsFirstNum = false;
		private int mLeftSupplyCount;
		public void Init(string name, string icon, float odds, int leftSupply, UnityAction onConfirmClick, UnityAction onClose)
		{
			mTextName.SafeSetText(name);
			if (!string.IsNullOrEmpty(icon))
			{
				ETCImageLoader.LoadSprite(ref mImageIcon, icon);
			}
			mTextOdds.SafeSetText(string.Format(TR.Value("horse_gambling_supply_view_odds"), odds));
			mTextSupplyTip.SafeSetText(string.Format(TR.Value("horse_gambling_left_supply"), leftSupply));
			//mButtonAddBullet.SafeAddOnClickListener(OnAddBulletClick);
			mButtonMax.SafeAddOnClickListener(OnMaxClick);
			//mButtonMin.SafeAddOnClickListener(OnMinClick);
			mButtonCountInput.SafeAddOnClickListener(OnKeyBoardClick);
			//mButtonKeyBoard.SafeAddOnClickListener(OnKeyBoardClick);
			mButtonConfirm.SafeAddOnClickListener(onConfirmClick);
			mButtonClose.SafeAddOnClickListener(onClose);
			//mScrollBar.numberOfSteps = leftSupply;
			//mScrollBar.size = 0;
			//mScrollBar.value = 0;
			SupplyCount = 0;
			mLeftSupplyCount = leftSupply;
			//mScrollBar.onValueChanged.AddListener(OnScrollBarChanged);
			UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ChangeNum, OnChangeNum);
		}


		public void Dispose()
		{
			//mButtonAddBullet.SafeRemoveOnClickListener(OnAddBulletClick);
			mButtonMax.SafeRemoveOnClickListener(OnMaxClick);
			//mButtonMin.SafeRemoveOnClickListener(OnMinClick);
			mButtonCountInput.SafeRemoveOnClickListener(OnKeyBoardClick);
			//mButtonKeyBoard.SafeRemoveOnClickListener(OnKeyBoardClick);
			mButtonConfirm.SafeRemoveAllListener();
			mButtonClose.SafeRemoveAllListener();
			//mScrollBar.onValueChanged.RemoveListener(OnScrollBarChanged);
			UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ChangeNum, OnChangeNum);
		}

		public void UpdateStakeNum(int leftSupply)
		{
			mTextSupplyTip.SafeSetText(string.Format(TR.Value("horse_gambling_left_supply"), leftSupply));
		}

		public void SetConfirmEnable(bool value)
		{
			mButtonConfirm.interactable = value;
		}

		void OnChangeNum(UIEvent uiEvent)
		{
			ChangeNumType changeNumType = (ChangeNumType)uiEvent.Param1;
			if (changeNumType == ChangeNumType.BackSpace)
			{
				if (SupplyCount < 10)
				{
					OnMinClick();
					mIsFirstNum = true;
				}
				else
				{
					SetSupplyCount(SupplyCount / 10);
				}
			}
			else if (changeNumType == ChangeNumType.Add)
			{
				int addNum = (int)uiEvent.Param2;
				if (addNum < 0 || addNum > 9)
				{
					Logger.LogErrorFormat("传入数字不合法，请控制在0-9之间");
					return;
				}
				if (mIsFirstNum)
				{
					if (addNum != 0)
					{
						SupplyCount = addNum;
						mIsFirstNum = false;
					}
					else
					{
						SupplyCount = 0;
					}
				}
				else
				{
					SupplyCount = (int)SupplyCount * 10 + addNum;
				}

				var bulletCount = ItemDataManager.GetInstance().GetOwnedItemCount(mBulletItemId, false);

				if (SupplyCount > mLeftSupplyCount)
				{
					SystemNotifyManager.SystemNotify(9930);
				}
				else if (bulletCount == 0)
				{
					SystemNotifyManager.SystemNotify(9928);
				}

				var max = Mathf.Min(bulletCount, mLeftSupplyCount);
				if (SupplyCount >= max)
				{
					SupplyCount = max;
				}

				SetSupplyCount(SupplyCount);

			}
		}
		void OnKeyBoardClick()
		{
			mIsFirstNum = true;
			ClientSystemManager.GetInstance().OpenFrame<VirtualKeyboardFrame>(FrameLayer.Middle, mKeyBoardOffsetX);
		}

		void OnMaxClick()
		{
			SetSupplyCount(Mathf.Min(ItemDataManager.GetInstance().GetOwnedItemCount(mBulletItemId, false), mLeftSupplyCount));
		}

		void OnMinClick()
		{
			SetSupplyCount(0);
		}

		void SetSupplyCount(int count)
		{
			mTextSupplyCount.SafeSetText(count.ToString());
			SupplyCount = count;
		}
	}
}