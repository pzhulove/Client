using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Protocol;
using ProtoTable;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace GameClient
{
	public class HorseGamblingMapShooter : MonoBehaviour, IDisposable
	{
		[SerializeField] private Image mImageIcon;
		[SerializeField] private Text mTextWinOdds;
		[SerializeField] private Text mTextName;
		[SerializeField] private Image mImageResult;
		[SerializeField] private Button mButton;
		[SerializeField] private UIGray mGray;
		[SerializeField] private GameObject mImageDeath;
		[SerializeField] private GameObject mRatePanel;
		[SerializeField] private GameObject mEffectSelected;
		[SerializeField] private GameObject mEffectRefresh;
		[SerializeField] private DOTweenAnimation mSelectTween;

		[SerializeField] private string mShooterUnknownIconPath = "UI/Image/Packed/p_UI_Duma.png:UI_Duma_Touxiang_00";
		[SerializeField] private string mWinSprite = "UI/Image/Packed/p_UI_Duma.png:UI_Duma_Text_Shengli";
		[SerializeField] private string mLoseSprite = "UI/Image/Packed/p_UI_Duma.png:UI_Duma_Text_ShiBai";
		[SerializeField] private string mChampionSprite = "UI/Image/Packed/p_UI_Duma.png:UI_Duma_Text_Guanjun";

		private UnityAction<HorseGamblingMapShooter> mOnClick;

		public enum EPosition
		{
			None,
			Top,
			Bottom
		}


		public EPosition Position { get; set; }

		private int mId;
		public int Id
		{
			get { return mId; }
		}

		public int RandomId { get; private set; }

		private bool mIsShowRefreshEffect = true;//第一次进入界面 播放刷新动画

		public void Init(bool isRight)
		{
			if (isRight)
			{
				Vector3 scale = mRatePanel.transform.localScale;
				scale.x = -Mathf.Abs(scale.x);
				mRatePanel.transform.localScale = scale;
				mTextName.alignment = TextAnchor.MiddleRight;
				mTextName.transform.localScale = scale;
				mTextWinOdds.alignment = TextAnchor.MiddleRight;
				mTextWinOdds.transform.localScale = scale;
			}
			else
			{
				Vector3 scale = mRatePanel.transform.localScale;
				scale.x = Mathf.Abs(scale.x);
				mRatePanel.transform.localScale = scale;
				mTextName.transform.localScale = scale;
				mTextName.alignment = TextAnchor.MiddleLeft;
				mTextWinOdds.alignment = TextAnchor.MiddleLeft;
				mTextWinOdds.transform.localScale = scale;
			}
			mRatePanel.CustomActive(false);
		}

		public void SetData(IHorseGamblingMapShooterModel model, UnityAction<HorseGamblingMapShooter> onShooterClick, bool isRefresh = false, bool isRight = false)
		{
			if (model == null)
			{
				Dispose();
				return;
			}

			var tableData = TableManager.GetInstance().GetTableItem<BetHorseShooter>((int)model.Id);

			var shooterData = HorseGamblingDataManager.GetInstance().GetShooterModel(model.Id);
			if (shooterData == null)
				return;

			if (shooterData.IsUnknown && HorseGamblingDataManager.GetInstance().State == BetHorsePhaseType.PHASE_TYPE_STAKE)
			{
				tableData = TableManager.GetInstance().GetTableItem<BetHorseShooter>(0);
			}

			if (tableData == null)
				return;

			string iconPath = tableData.IconPath;
			if (!string.IsNullOrEmpty(iconPath))
				ETCImageLoader.LoadSprite(ref mImageIcon, iconPath);
			mImageIcon.CustomActive(true);

			UpdateOdds(model);

			ShowBattleResult(model);
			mTextName.SafeSetText(tableData.Name);
			mOnClick = onShooterClick;
			mId = model.Id;
			mButton.SafeRemoveAllListener();
			mButton.SafeAddOnClickListener(OnButtonClick);
			if (mIsShowRefreshEffect)
			{
				if (isRefresh)
				{
					mEffectRefresh.CustomActive(false);
					mEffectRefresh.CustomActive(true);
				}
				mIsShowRefreshEffect = false;
			}
		}

		public void ShowUnBattleData(IHorseGamblingMapShooterModel model, UnityAction<HorseGamblingMapShooter> onShooterClick)
		{
			if (model == null)
			{
				Dispose();
				return;
			}
			string iconPath = HorseGamblingDataManager.GetInstance().GetShooterIconPath(model.Id);
			if (!string.IsNullOrEmpty(iconPath))
				ETCImageLoader.LoadSprite(ref mImageIcon, iconPath);
			mImageIcon.CustomActive(true);
			UpdateOdds(model);
			mImageResult.CustomActive(false);
			mImageDeath.CustomActive(false);
			mGray.enabled = false;

			mOnClick = onShooterClick;
			mId = model.Id;
			mButton.SafeRemoveAllListener();
			mButton.SafeAddOnClickListener(OnButtonClick);
		}

		public void ShowChampion(UnityAction<HorseGamblingMapShooter> onShooterClick, int id)
		{
			mId = id;
			mImageIcon.CustomActive(true);
			string iconPath = HorseGamblingDataManager.GetInstance().GetShooterIconPath(id);
			if (!string.IsNullOrEmpty(iconPath))
				ETCImageLoader.LoadSprite(ref mImageIcon, iconPath);

			mOnClick = onShooterClick;
			mButton.SafeRemoveAllListener();
			mButton.SafeAddOnClickListener(OnButtonClick);
			mImageResult.CustomActive(false);
			mRatePanel.CustomActive(false);
			ETCImageLoader.LoadSprite(ref mImageResult, mChampionSprite);
		}

		public void ShowBattleResult(IHorseGamblingMapShooterModel model)
		{
			if (model == null)
			{
				return;
			}
			switch (model.BattleState)
			{
				case EHorseGamblingBattleResult.NotBattle:
					mImageResult.CustomActive(false);
					mImageDeath.CustomActive(false);
					mGray.enabled = false;
					break;
				case EHorseGamblingBattleResult.Win:
					mImageResult.CustomActive(true);
					ETCImageLoader.LoadSprite(ref mImageResult, mWinSprite);
					mImageDeath.CustomActive(false);
					mGray.enabled = false;
					break;
				case EHorseGamblingBattleResult.Lose:
					mImageResult.CustomActive(false);
					//ETCImageLoader.LoadSprite(ref mImageResult, mLoseSprite);
					mImageDeath.CustomActive(true);
					mGray.enabled = true;
					break;
			}
		}

		public void UpdateOdds(IHorseGamblingMapShooterModel model)
		{
			mRatePanel.CustomActive(model.IsShowOdds);
			if (model.IsShowOdds)
			{
				float odds = HorseGamblingDataManager.GetInstance().GetShooterOdds(model.Id);
				mTextWinOdds.SafeSetText(string.Format(TR.Value("horse_gambling_map_shooter_odd"), odds));
			}
		}

		public void Reset()
		{
			mRatePanel.CustomActive(false);
			mImageResult.CustomActive(false);
			mImageDeath.CustomActive(false);
			ETCImageLoader.LoadSprite(ref mImageIcon, mShooterUnknownIconPath);
			mGray.enabled = false;
			mImageResult.CustomActive(false);
		}

		public void RandomShooter()
		{
			var shooters = TableManager.GetInstance().GetTable<BetHorseShooter>();
			if (shooters != null && shooters.Count > 0)
			{
				List<int> ids = new List<int>(shooters.Keys);
				RandomId = ids[Random.Range(0, ids.Count - 1)];
				ETCImageLoader.LoadSprite(ref mImageIcon, ((BetHorseShooter)shooters[ids[RandomId]]).IconPath);
			}
		}

		public void StopRamdom()
		{
			mEffectRefresh.CustomActive(false);
			mEffectRefresh.CustomActive(true);
		}

		public void SetSelected(bool value)
		{
			mEffectSelected.CustomActive(value);
			if(value)
				mSelectTween.DORestart();
		}

		void OnButtonClick()
		{
			if (mOnClick != null)
			{
				mOnClick(this);
			}
		}

		public void Dispose()
		{
			mButton.SafeRemoveOnClickListener(OnButtonClick);
		}
	}
}