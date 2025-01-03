using System;
using System.Collections.Generic;
using Scripts.UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GameClient
{
	public enum EHorseGamblingRecord
	{
		None,
		Stake,
		HistoryAndRank,
		ShooterHistory
	}
	public class HorseGamblingRecordView : MonoBehaviour,IDisposable
	{
		[SerializeField] private ComTabGroup mTabGroup;
		[SerializeField] private Transform mRecordTitleRoot;
		[SerializeField] private Text mTextTitle;
		[SerializeField] private Button mButtonClose;
		[SerializeField] private string mTitleItemPrefabPath = "UIFlatten/Prefabs/HorseGambling/HorseGamblingRecordTitleItem";
		[SerializeField] private string mTabPrefabPath = "UIFlatten/Prefabs/HorseGambling/HorseGamblingRecordTitleItem";
		[SerializeField] private string mStakeToggleName = "支援记录";
		[SerializeField] private string mGameRecordToggleName = "比赛历史";
		[SerializeField] private string mShooterRankToggleName = "选手排名";
		[SerializeField] private string mShooterRecordToggleName = "选手战绩";
		[SerializeField] private string mStakeItemPrefab = "UIFlatten/Prefabs/HorseGambling/HorseGamblingStakeRecordItem";
		[SerializeField] private string mGameRecordItemPrefab = "UIFlatten/Prefabs/HorseGambling/HorseGamblingGameRecordItem";
		[SerializeField] private string mShooterRankItemPrefab = "UIFlatten/Prefabs/HorseGambling/HorseGamblingGameRecordItem";
		[SerializeField] private string mShooterHistoryItemPrefab = "UIFlatten/Prefabs/HorseGambling/HorseGamblingShooterHistoryItem";
		[SerializeField] private string mItemBg1 = "UI/Image/Packed/p_UI_Duma.png:UI_Duma_LieBiao_Bg_01";
		[SerializeField] private string mItemBg2 = "UI/Image/Packed/p_UI_Duma.png:UI_Duma_LieBiao_Bg_02";
		[SerializeField] private List<string> mStakeTitleNames = new List<string>{"场次", "选手方案", "赔率", "支援数量", "方案盈利"};
		[SerializeField] private List<string> mGameRecordTitleNames = new List<string>{"场次", "冠军", "赔率", "最大奖金"};
		[SerializeField] private List<string> mShooterRankTitleNames = new List<string>{"排名", "选手名称", "比赛次数", "胜率"};
		[SerializeField] private List<string> mShooterRecordTitleNames = new List<string>{"场次", "选手名称", "赔率", "比赛结果" };

		public List<string> StakeTitleNames
		{
			get { return mStakeTitleNames; }
		}
		public List<string> GameRecordTitleNames
		{
			get { return mGameRecordTitleNames; }
		}
		public List<string> ShooterRankTitleNames
		{
			get { return mShooterRankTitleNames; }
		}
		public List<string> ShooterRecordTitleNames
		{
			get { return mShooterRecordTitleNames; }
		}
		public string ItemBg1
		{
			get { return mItemBg1; }
		}
		public string ItemBg2
		{
			get { return mItemBg2; }
		}

		public string StakeItemPrefab
		{
			get { return mStakeItemPrefab; }
		}
		public string GameRecordItemPrefab
		{
			get { return mGameRecordItemPrefab; }
		}
		public string ShooterRankItemPrefab
		{
			get { return mShooterRankItemPrefab; }
		}
		public string ShooterRecordItemPrefab
		{
			get { return mShooterHistoryItemPrefab; }
		}

		public string StakeToggleName
		{
			get { return mStakeToggleName; }
		}
		public string GameRecordToggleName
		{
			get { return mGameRecordToggleName; }
		}
		public string ShooterRankToggleName
		{
			get { return mShooterRankToggleName; }
		}
		public string ShooterRecordToggleName
		{
			get { return mShooterRecordToggleName; }
		}

		[SerializeField] private ComUIListScript mListScript;
		private readonly List<HorseGamblingRecordTitleItem> mTitleItems = new List<HorseGamblingRecordTitleItem>();
		public void Init(string[] toggleNames, ComTabGroup.OnToggleValueChanged onValueChanged, ComUIListScript.OnItemVisiableDelegate onItemVisible, ComUIListScript.OnItemUpdateDelegate onItemUpdate, UnityAction onClose, int defaultSelectId = 0)
		{
			mListScript.onItemVisiable = onItemVisible;
			mListScript.OnItemUpdate = onItemUpdate;
			mButtonClose.SafeRemoveAllListener();
			mButtonClose.SafeAddOnClickListener(onClose);
			mTabGroup.Init(toggleNames, mTabPrefabPath, onValueChanged, null, defaultSelectId);
		}

		public void SetData(string titleName, List<string> titles, int dataCount, string itemPrefabPath)
		{
			mTextTitle.SafeSetText(titleName);
			for (int i = mTitleItems.Count - 1; i >= titles.Count - 1; --i)
			{
				mTitleItems[i].Dispose();
			}

			for (int i = 0; i < titles.Count; ++i)
			{
				if (i > mTitleItems.Count - 1)
				{
					var go = AssetLoader.GetInstance().LoadResAsGameObject(mTitleItemPrefabPath);
					if (go != null)
					{
						var script = go.GetComponent<HorseGamblingRecordTitleItem>();
						if (script != null)
						{
							script.Init(titles[i]);
							mTitleItems.Add(script);
						}
						go.transform.SetParent(mRecordTitleRoot, false);
					}
				}
				else
				{
					mTitleItems[i].Init(titles[i]);
				}
			}
			mListScript.InitialLizeWithExternalElement(itemPrefabPath);
			mListScript.SetElementAmount(0);
			mListScript.CustomActive(false);
			mListScript.SetElementAmount(dataCount);
			mListScript.EnsureElementVisable(0);
			mListScript.CustomActive(true);
		}

		public void UpdateData(int dataCount)
		{
			mListScript.UpdateElementAmount(dataCount);
		}

		public void Dispose()
		{
			if (mTabGroup != null) mTabGroup.Dispose();
			mListScript.onItemVisiable = null;
			mListScript.OnItemUpdate = null;
			mButtonClose.SafeRemoveAllListener();
		}
	}
}