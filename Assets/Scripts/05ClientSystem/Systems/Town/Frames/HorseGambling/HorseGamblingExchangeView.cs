using System;
using System.Collections;
using System.Collections.Generic;
using ProtoTable;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GameClient
{
	public class HorseGamblingExchangeView : MonoBehaviour,IDisposable
	{
		[SerializeField] private Text mTextExchangeCount;
		[SerializeField] private Button mButtonCountInput;
		[SerializeField] private Button mButtonMax;
		[SerializeField] private Text mTextCostCount;
		[SerializeField] private Button mButtonConfirm;
		[SerializeField] private Button mButtonCancel;
		[SerializeField] private int mBulletItemId;
		[SerializeField] private int mGoldItemId;
		[SerializeField] private int mExchangeRate = 100;
		[SerializeField] private float mKeyBoardOffsetX = 100;
		[SerializeField] private int mMaxBuyBulletCount = 50000;//一次购买子弹上限

		//判断是否打开输入框界面后第一次输入，如果是 则会清除掉当前的数字
		bool mIsFirstNum = false;
		public int ExchangeCount { get; private set; }
		public int GoldItemId
		{
			get { return mGoldItemId; }
		}

		public int ExchangeRate
		{
			get { return mExchangeRate; }
		}

		public void Init(UnityAction onConfirm, UnityAction onCancel)
		{
			mButtonConfirm.SafeAddOnClickListener(onConfirm);
			mButtonCancel.SafeAddOnClickListener(onCancel);
			ExchangeCount = 0;
			UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ChangeNum, OnChangeNum);
			mButtonCountInput.SafeAddOnClickListener(OnKeyBoardClick);
			mButtonMax.SafeAddOnClickListener(OnMaxClick);
			mButtonConfirm.interactable = true;
		}

		public void Dispose()
		{
			UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ChangeNum, OnChangeNum);
			mButtonCountInput.SafeRemoveOnClickListener(OnKeyBoardClick);
			mButtonMax.SafeRemoveOnClickListener(OnMaxClick);
			mButtonConfirm.interactable = false;
		}

		public void SetConfirmEnable(bool value)
		{
			mButtonConfirm.interactable = value;
		}

		void OnKeyBoardClick()
		{
			mIsFirstNum = true;
			ClientSystemManager.GetInstance().OpenFrame<VirtualKeyboardFrame>(FrameLayer.Middle, mKeyBoardOffsetX);
		}

		void OnMaxClick()
		{
			var max = ItemDataManager.GetInstance().GetOwnedItemCount(mGoldItemId, false) / mExchangeRate;
			if (max > mMaxBuyBulletCount)
			{
				max = mMaxBuyBulletCount;
				SystemNotifyManager.SystemNotify(9931);
			}
			SetBuyCount(max);
		}

		void OnChangeNum(UIEvent uiEvent)
		{
			ChangeNumType changeNumType = (ChangeNumType)uiEvent.Param1;
			if (changeNumType == ChangeNumType.BackSpace)
			{
				if (ExchangeCount < 10)
				{
					SetBuyCount(0);
					mIsFirstNum = true;
				}
				else
				{
					SetBuyCount(ExchangeCount / 10);
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
						ExchangeCount = addNum;
						mIsFirstNum = false;
					}
					else
					{
						ExchangeCount = 0;
					}
				}
				else
				{
					ExchangeCount = (int)ExchangeCount * 10 + addNum;
				}
				var max = ItemDataManager.GetInstance().GetOwnedItemCount(mGoldItemId, false) / mExchangeRate;
				if (ExchangeCount >= max)
				{
					ExchangeCount = max;
				}

				if (ExchangeCount > mMaxBuyBulletCount)
				{
					ExchangeCount = mMaxBuyBulletCount;
					SystemNotifyManager.SystemNotify(9931);
				}

				SetBuyCount(ExchangeCount);

				//提示金币不足
				if (max == 0)
				{
					SystemNotifyManager.SystemNotify(9929);
				}
			}
		}

		void SetBuyCount(int count)
		{
			mTextCostCount.SafeSetText(Utility.GetShowPrice((ulong)(count * mExchangeRate)));
			mTextExchangeCount.SafeSetText(count.ToString());
			ExchangeCount = count;
		}
	}
}