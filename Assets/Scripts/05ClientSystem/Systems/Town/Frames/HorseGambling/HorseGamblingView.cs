using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Protocol;
using ProtoTable;
using Scripts.UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GameClient
{
	public interface IHorseGablingDataManager
	{
		List<IHorseGamblingMapZoneModel> MapZoneModels { get; }
		HorseGamblingShooterModel GetShooterModel(int shooterId);
		ShooterRecord[] GetShooterHistory(int shooterId);
		List<IHorseGamblingStakeModel> GetStakeHistory();
		BetHorsePhaseType State { get; }
		WeatherType Weather { get; }

		uint TimeStamp { get; }
	}
	public interface IHorseGamblingStakeModel
	{
		int ShooterId { get; }
		string ShooterName { get; }
		int Stake { get; }//下注信息

		int GameId { get; }//游戏场次

		int Profit { get; }//盈利
		float Odds { get; }//赔率
	}

	public class HorseGamblingView : MonoBehaviour, IDisposable
	{
		[SerializeField][Header("赔率刷新间隔")] private float mRefreshOddsInterval = 5;
		public float RefreshOddsInterval
		{
			get { return mRefreshOddsInterval; }
		}
		[SerializeField] private Button mButtonClose;

		#region selected shooter info
		[SerializeField] private GameObject mShooterInfoGO;
		[SerializeField] private GameObject mShooterRecordGO;
		[SerializeField] private Toggle mToggleShooterInfo;
		[SerializeField] private Toggle mToggleShooterRecord;
		[SerializeField] private UIGray mToggleShooterRecordGray;
		[SerializeField] private Text mTextShooterName;
		[SerializeField] private Image mImageShooterPortrait;
		[Header("射手信息")]
		[SerializeField] private Text mTextShooterStatus;
		[SerializeField] private Text mTextShooterOccu;
		[SerializeField] private Text mTextShooterTerrain;
		[SerializeField] private Text mTextShooterWeather;
		[SerializeField] private Text mTextShooterWinRate;
		[SerializeField] private Text mTextShooterChampionCount;
		[SerializeField] private Image mImageShooterStatus;
		[SerializeField] private Image mImageShooterOccu;

		[Header("射手战绩")]
		[SerializeField] private ComUIListScript mShooterRecordList;
		[SerializeField] private Button mButtonShooterRecords;
		[SerializeField] private string mRecordItemPath = "UIFlatten/Prefabs/HorseGambling/HorseGamblingShooterRecordItem";
		#endregion

		private const int WIN_RATE_TO_PERCENT = 100;
		#region map
		[Header("地图")]
		[SerializeField] private List<GameObject> mMapZoneGOList;
		[SerializeField] private Transform mShooterRoot;
		[SerializeField] private Transform mLineRoot;
		[SerializeField] private Transform mTerrainRoot;
		private readonly Dictionary<int, HorseGamblingMapZone> mMapZones = new Dictionary<int, HorseGamblingMapZone>();
		#endregion

		#region bottom
		[SerializeField] private Image mImageWeather;
		[SerializeField] private Text mTextWeather;
		[SerializeField] private Text mTextHelp;
		[SerializeField] private Button mButtonSupply;
		[SerializeField] private UIGray mButtonSupplyGray;
		[SerializeField] private Button mButtonStakeRecords;
		[SerializeField] private Button mButtonGameRecords;
		#endregion

		private bool mIsInitWeather = false;

		[Serializable]
		struct ShooterStatusIcon
		{
			[Header("状态")]public ShooterStatusType Status;
			[Header("状态描述")]public string Description;
			[Header("状态图标")] public string Icon;
		}

		[Header("射手状态数据")][SerializeField] private List<ShooterStatusIcon> mShooterStatusDatas;
		[SerializeField] private string mShooterRecordBg1 = "UI/Image/Packed/p_UI_Duma.png:UI_Duma_LieBiao_Bg_01";
		[SerializeField] private string mShooterRecordBg2 = "UI/Image/Packed/p_UI_Duma.png:UI_Duma_LieBiao_Bg_02";


		[Serializable]
		struct WeatherData
		{
			public WeatherType WeatherType;
			public string Description;
			public string Icon;
			public string EffectPrefabPath;
		}
		[Header("天气数据")] [SerializeField] private List<WeatherData> mWeatherDatas;
		[SerializeField] private Transform mWeatherEffectRoot;

		private IHorseGablingDataManager mDataManager;
		private bool mIsShowShooterRecord = false;
		private HorseGamblingMapShooter mSelectShooter;
		public int SelectShooterId
		{
			get { return mSelectShooter == null ? 0 : mSelectShooter.Id; }
		}

		[Header("动画特效")]
		private bool mIsRandomShooter = false;
		private bool mIsShowBattleAnimation = false;
		private float mAnimationDelta = 0f;
		private float mAnimationDuration = 0f;
		[SerializeField] private float mAnimationInterval = 0.1f;
		[SerializeField] private float mAnimationTime = 1f;
		[SerializeField] private float mZoneBattleAnimationTime = 2f;
		[SerializeField] private float mBattleStartAnimationTime = 2f;
		[SerializeField] private GameObject mBattleStartAnim;
		[SerializeField] private DOTweenAnimation mPortraitSelectAnim;
		private bool mIsNeedSetMapZoneData = false;
		public void Init(IHorseGablingDataManager dataManager, UnityAction onButtonSupplyClick, UnityAction onStakeRecordsClick, UnityAction onGameRecordsClick, UnityAction onShooterRecordClick, UnityAction onClose)
		{
			//Dispose();
			mDataManager = dataManager;

			if (dataManager == null)
			{
				Dispose();
				return;
			}

			mButtonClose.SafeAddOnClickListener(onClose);
			mButtonSupply.SafeAddOnClickListener(onButtonSupplyClick);
			mButtonGameRecords.SafeAddOnClickListener(onGameRecordsClick);
			mButtonStakeRecords.SafeAddOnClickListener(onStakeRecordsClick);
			mButtonShooterRecords.SafeAddOnClickListener(onShooterRecordClick);
			mToggleShooterInfo.SafeAddOnValueChangedListener(OnToggleShooterInfo);
			mToggleShooterRecord.SafeAddOnValueChangedListener(OnToggleShooterRecord);
			mShooterRecordList.InitialLizeWithExternalElement(mRecordItemPath);
			mShooterRecordList.onItemVisiable = OnShooterRecordItemVisible;
			mShooterRecordList.OnItemUpdate = OnShooterRecordItemVisible;
			//SetWeather();
			StartCoroutine(InitMapZones());
			UpdateTipText();
		}

		public void Dispose()
		{
			StopAllCoroutines();
			mMapZones.Clear();
			mDataManager = null;
			mSelectShooter = null;
			mButtonClose.SafeRemoveAllListener();
			mToggleShooterInfo.SafeRemoveOnValueChangedListener(OnToggleShooterInfo);
			mToggleShooterRecord.SafeRemoveOnValueChangedListener(OnToggleShooterRecord);
			mButtonGameRecords.SafeRemoveAllListener();
			mButtonStakeRecords.SafeRemoveAllListener();
			mButtonShooterRecords.SafeRemoveAllListener();
			mButtonSupply.SafeRemoveAllListener();
			mShooterRecordList.onItemVisiable = null;
			mShooterRecordList.OnItemUpdate = null;
			StopAnimation();
		}

		public void UpdateData(IHorseGablingDataManager dataManager)
		{
			if (dataManager == null)
			{
				return;
			}

			mDataManager = dataManager;
			UpdateMapZones();
			if (!mIsInitWeather)
				SetWeather();

			if (dataManager.State != BetHorsePhaseType.PHASE_TYPE_STAKE)
				SetButtonSupplyEnable(false);
			else
				SetButtonSupplyEnable(true);
		}

		public void UpdateState(BetHorsePhaseType status)
		{
			switch (status)
			{
				case BetHorsePhaseType.PHASE_TYPE_STAKE:
					SetButtonSupplyEnable(true);
					mIsRandomShooter = true;
					mIsInitWeather = false;
					ResetMapZones();
					mToggleShooterInfo.isOn = true;
					OnToggleShooterInfo(true);
					ResetShooterInfo();
					if (mSelectShooter != null)
					{
						mSelectShooter.SetSelected(false);
					}
					mSelectShooter = null;
					mTextHelp.SafeSetText(TR.Value("horse_gambling_match_tip"));
					break;
				case BetHorsePhaseType.PHASE_TYPE_ADJUST:
					SetButtonSupplyEnable(false);
					mTextHelp.SafeSetText(string.Format(TR.Value("horse_gambling_adjust_tip"), Function.GetLeftMinutes((int)HorseGamblingDataManager.GetInstance().TimeStamp, (int)TimeManager.GetInstance().GetServerTime())));
					break;
				case BetHorsePhaseType.PHASE_TYPE_RESULT:
					mIsShowBattleAnimation = true;
					mTextHelp.SafeSetText(string.Format(TR.Value("horse_gambling_show_tip"), Function.GetLeftMinutes((int)HorseGamblingDataManager.GetInstance().TimeStamp, (int)TimeManager.GetInstance().GetServerTime())));
					StartCoroutine(PlayBattleAnimation());
					break;
				case BetHorsePhaseType.PHASE_TYPE_DAY_END:
				case BetHorsePhaseType.PHASE_TYPE_READY:
					mTextHelp.SafeSetText(string.Format(TR.Value("horse_gambling_day_end_tip"), Function.GetTime((int)HorseGamblingDataManager.GetInstance().TimeStamp)));
					break;
			}
		}

		public void UpdateOdds(IHorseGablingDataManager dataManager)
		{
			if (mIsRandomShooter)
				return;

			for (int i = 0; i < dataManager.MapZoneModels.Count; ++i)
			{
				if (mMapZones.ContainsKey(dataManager.MapZoneModels[i].Id))
				{
					mMapZones[dataManager.MapZoneModels[i].Id].UpdateOdds(dataManager.MapZoneModels[i]);
				}
			}
		}

		public void UpdateShooterInfo(IHorseGamblingShooterModel model)
		{
			if (model == null || mSelectShooter == null)
				return;

			if (mSelectShooter.Id == model.Id)
			{
				if (mIsShowShooterRecord)
				{
					ShowShooterRecord(model.Records);
				}
				else
				{
					ShowShooterInfo(model);
				}
			}

		}

		void UpdateMapZones()
		{
			if (mDataManager == null || mDataManager.MapZoneModels == null || mMapZones == null)
			{
				return;
			}

			UpdateTipText();
			//此时是本赛季第一场比赛准备阶段
			if (mDataManager.MapZoneModels.Count == 0)
			{
				mTextHelp.SafeSetText(string.Format(TR.Value("horse_gambling_first_game_tip"), Function.GetTime((int)mDataManager.TimeStamp)));
			}

			if (!mIsShowBattleAnimation && !mIsRandomShooter)
			{
				SetMapZoneDatas();
			}
		}

		void SetWeather()
		{
			if (mDataManager == null)
				return;
			for (int i = 0; i < mWeatherDatas.Count; ++i)
			{
				if (mDataManager.Weather == mWeatherDatas[i].WeatherType)
				{
					mTextWeather.SafeSetText(mWeatherDatas[i].Description);
					ETCImageLoader.LoadSprite(ref mImageWeather, mWeatherDatas[i].Icon);
					if (mWeatherEffectRoot.transform.childCount > 0)
					{
						var lastEffect = mWeatherEffectRoot.GetChild(0);
						if (lastEffect != null)
							Destroy(lastEffect.gameObject);
					}

					var effect = AssetLoader.GetInstance().LoadResAsGameObject(mWeatherDatas[i].EffectPrefabPath);
					if (effect != null)
					{
						effect.transform.SetParent(mWeatherEffectRoot, false);
					}
					mIsInitWeather = true;
					break;
				}
			}
		}

		IEnumerator PlayBattleAnimation()
		{
			mBattleStartAnim.CustomActive(true);
			yield return new WaitForSeconds(mBattleStartAnimationTime);
			mBattleStartAnim.CustomActive(false);
			foreach (var zone in mMapZones.Values)
			{
				if (zone.Phase == 1)
				{
					MapZoneShowUnBattle(zone);
				}
			}
			yield return new WaitForSeconds(mZoneBattleAnimationTime);
			foreach (var zone in mMapZones.Values)
			{
				if (zone.Phase == 1)
				{
					MapZoneShowBattleResult(zone);
				}
				else if (zone.Phase == 2)
				{
					MapZoneShowUnBattle(zone);
				}
			}
			yield return new WaitForSeconds(mZoneBattleAnimationTime);
			foreach (var zone in mMapZones.Values)
			{
				if (zone.Phase == 2)
				{
					MapZoneShowBattleResult(zone);
				}
				else if (zone.Phase == 3)
				{
					MapZoneShowUnBattle(zone);
				}
			}
			yield return new WaitForSeconds(mZoneBattleAnimationTime);
			foreach (var zone in mMapZones.Values)
			{
				if (zone.Phase == 3)
				{
					zone.HideBattleAnimation();
					for (int j = 0; j < mDataManager.MapZoneModels.Count; ++j)
					{
						if (zone.Id == mDataManager.MapZoneModels[j].Id)
						{
							zone.ShowBattleResult(mDataManager.MapZoneModels[j]);
							ShowChampion(zone, mDataManager.MapZoneModels[j].Shooters);
							break;
						}
					}
				}
			}
			mIsShowBattleAnimation = false;
		}

		void ShowChampion(HorseGamblingMapZone zone, Dictionary<int, HorseGamblingMapShooterModel> shooters)
		{
			var champion = zone.GetComponent<HorseGamblingMapZoneChampionComponent>();
			if (champion != null)
			{
				foreach (var shooter in shooters.Values)
				{
					if (shooter.BattleState == EHorseGamblingBattleResult.Win)
					{
						champion.ShowChampion(shooter.Id, OnSelectShooter);
						break;
					}
				}
			}
		}

		void MapZoneShowBattleResult(HorseGamblingMapZone zone)
		{
			zone.HideBattleAnimation();
			for (int j = 0; j < mDataManager.MapZoneModels.Count; ++j)
			{
				if (zone.Id == mDataManager.MapZoneModels[j].Id)
				{
					zone.ShowBattleResult(mDataManager.MapZoneModels[j]);
					break;
				}
			}
		}

		void MapZoneShowUnBattle(HorseGamblingMapZone zone)
		{
			//zone.HideBattleAnimation();
			for (int j = 0; j < mDataManager.MapZoneModels.Count; ++j)
			{
				if (zone.Id == mDataManager.MapZoneModels[j].Id)
				{
					zone.ShowUnBattle(mDataManager.MapZoneModels[j]);
					break;
				}
			}
		}

		void UpdateTipText()
		{
			if (mDataManager != null)
			{
				switch (mDataManager.State)
				{
					case BetHorsePhaseType.PHASE_TYPE_STAKE:
						SetButtonSupplyEnable(true);
						mTextHelp.SafeSetText(string.Format(TR.Value("horse_gambling_stake_tip"), Function.GetLeftMinutes((int)HorseGamblingDataManager.GetInstance().TimeStamp, (int)TimeManager.GetInstance().GetServerTime())));
						break;
					case BetHorsePhaseType.PHASE_TYPE_ADJUST:
						SetButtonSupplyEnable(false);
						mTextHelp.SafeSetText(string.Format(TR.Value("horse_gambling_adjust_tip"), Function.GetLeftMinutes((int)HorseGamblingDataManager.GetInstance().TimeStamp, (int)TimeManager.GetInstance().GetServerTime())));
						break;
					case BetHorsePhaseType.PHASE_TYPE_RESULT:
						SetButtonSupplyEnable(false);
						mTextHelp.SafeSetText(string.Format(TR.Value("horse_gambling_show_tip"), Function.GetLeftMinutes((int)HorseGamblingDataManager.GetInstance().TimeStamp, (int)TimeManager.GetInstance().GetServerTime())));
						break;
					case BetHorsePhaseType.PHASE_TYPE_DAY_END:
					case BetHorsePhaseType.PHASE_TYPE_READY:
						mTextHelp.SafeSetText(string.Format(TR.Value("horse_gambling_day_end_tip"), Function.GetTime((int)HorseGamblingDataManager.GetInstance().TimeStamp)));
						break;
				}
			}
		}

		void SetButtonSupplyEnable(bool value)
		{
			mButtonSupplyGray.enabled = !value;
			mButtonSupply.enabled = value;
		}

		void ResetMapZones()
		{
			foreach (var zone in mMapZones.Values)
			{
				zone.Reset(zone.Phase != 3);
				if (zone.Phase == 3)
				{
					var champion = zone.GetComponent<HorseGamblingMapZoneChampionComponent>();
					if (champion != null)
					{
						champion.Reset();
					}
				}
			}
		}

		IEnumerator InitMapZones()
		{
			for (int i = 0; i < mMapZoneGOList.Count; ++i)
			{
				yield return null;
				bool isNeedAutoSelectShooter = mSelectShooter == null;
				var wrapper = mMapZoneGOList[i].GetComponent<UIPrefabWrapper>();
				if (wrapper != null)
				{
					mMapZoneGOList[i] = wrapper.LoadUIPrefab(mMapZoneGOList[i].transform);
					var script = mMapZoneGOList[i].GetComponent<HorseGamblingMapZone>();
					if (script != null && mDataManager != null)
					{
						bool isShowShooter = script.Phase == 1 || mDataManager.State == BetHorsePhaseType.PHASE_TYPE_RESULT || mDataManager.State == BetHorsePhaseType.PHASE_TYPE_DAY_END;
						bool isShowWinLine = mDataManager.State == BetHorsePhaseType.PHASE_TYPE_RESULT || mDataManager.State == BetHorsePhaseType.PHASE_TYPE_DAY_END;
						bool isShowBattleAnimation = script.Phase != 3 ||(mDataManager.State == BetHorsePhaseType.PHASE_TYPE_RESULT || mDataManager.State == BetHorsePhaseType.PHASE_TYPE_DAY_END);

						script.Init(OnSelectShooter, mShooterRoot, mTerrainRoot, mLineRoot, isShowShooter, isShowBattleAnimation);
						mMapZones.Add(script.Id, script);
						if (mIsNeedSetMapZoneData && mDataManager.MapZoneModels != null)
						{
							for (int j = 0; j < mDataManager.MapZoneModels.Count; ++j)
							{
								if (script.Id == mDataManager.MapZoneModels[j].Id)
								{
									if (script.Id == 1)
									{
										script.SetData(mDataManager.MapZoneModels[j], isShowShooter, isNeedAutoSelectShooter, isShowWinLine);
									}
									else
									{
										script.SetData(mDataManager.MapZoneModels[j], isShowShooter, false, isShowWinLine);

										if (script.Phase == 3)
										{
											var champion = script.GetComponent<HorseGamblingMapZoneChampionComponent>();
											if (champion != null)
											{
												champion.Init(mShooterRoot);
											}
											if ((mDataManager.State == BetHorsePhaseType.PHASE_TYPE_RESULT || mDataManager.State == BetHorsePhaseType.PHASE_TYPE_DAY_END))
											{
												ShowChampion(script, mDataManager.MapZoneModels[j].Shooters);
											}
										}
									}
									break;
								}
							}
						}
					}
				}
			}
		}

		void Update()
		{
			if (mDataManager != null)
			{
				switch (mDataManager.State)
				{
					case BetHorsePhaseType.PHASE_TYPE_STAKE:
						mTextHelp.SafeSetText(string.Format(TR.Value("horse_gambling_stake_tip"), Function.GetLeftMinutes((int)HorseGamblingDataManager.GetInstance().TimeStamp, (int)TimeManager.GetInstance().GetServerTime())));
						break;
					case BetHorsePhaseType.PHASE_TYPE_ADJUST:
						mTextHelp.SafeSetText(string.Format(TR.Value("horse_gambling_adjust_tip"), Function.GetLeftMinutes((int)HorseGamblingDataManager.GetInstance().TimeStamp, (int)TimeManager.GetInstance().GetServerTime())));
						break;
					case BetHorsePhaseType.PHASE_TYPE_RESULT:
						mTextHelp.SafeSetText(string.Format(TR.Value("horse_gambling_show_tip"), Function.GetLeftMinutes((int)HorseGamblingDataManager.GetInstance().TimeStamp, (int)TimeManager.GetInstance().GetServerTime())));
						break;
				}
			}

			if (mIsRandomShooter)
			{
				mAnimationDelta += Time.deltaTime;
				mAnimationDuration += Time.deltaTime;
				if (mAnimationDelta >= mAnimationInterval)
				{
					mAnimationDelta = 0f;
					foreach (var zone in mMapZones.Values)
					{
						if (zone.Phase == 1)
							zone.RandomShooter();
					}


					if (mSelectShooter != null)
					{
						var shooterData = TableManager.GetInstance().GetTableItem<BetHorseShooter>(mSelectShooter.RandomId);
						if (shooterData != null)
						{
							ETCImageLoader.LoadSprite(ref mImageShooterPortrait, shooterData.PortraitPath);
						}
					}
				}

				if (mAnimationDuration >= mAnimationTime)
				{
					StopAnimation();
					foreach (var zone in mMapZones.Values)
					{
						if (zone.Phase == 1)
							zone.StopRandomShooter();
					}
					SetMapZoneDatas();
					OnToggleShooterInfo(true);
				}
			}
		}

		void ResetShooterInfo()
		{
			var data = TableManager.GetInstance().GetTableItem<BetHorseShooter>(0);
			mTextShooterName.SafeSetText(data.Name);
			mTextShooterStatus.SafeSetText(TR.Value("horse_gambling_shooter_unknown_staus"));
			mTextShooterTerrain.SafeSetText(data.Terrain);
			//ETCImageLoader.LoadSprite(ref mImageShooterStatus, mShooterStatusDatas[0].Icon);
			mImageShooterStatus.CustomActive(false);
			mImageShooterOccu.CustomActive(false);
			mTextShooterOccu.SafeSetText(data.Occupation);
			mTextShooterWeather.SafeSetText(data.Weather);
			mTextShooterWinRate.SafeSetText(TR.Value("horse_gambling_shooter_unknown_staus"));
			mTextShooterChampionCount.SafeSetText(TR.Value("horse_gambling_shooter_unknown_staus"));
			//ETCImageLoader.LoadSprite(ref mImageShooterOccu, data.OccupationIcon);
			ETCImageLoader.LoadSprite(ref mImageShooterPortrait, data.PortraitPath);
		}

		void SetMapZoneDatas()
		{
			if (mDataManager == null || mDataManager.MapZoneModels == null || mMapZones == null)
				return;

			bool isAutoSelectShooter = mSelectShooter == null;
			bool isShowWinLine = mDataManager.State == BetHorsePhaseType.PHASE_TYPE_RESULT || mDataManager.State == BetHorsePhaseType.PHASE_TYPE_DAY_END;

			for (int i = 0; i < mDataManager.MapZoneModels.Count; ++i)
			{
				if (mMapZones.ContainsKey(mDataManager.MapZoneModels[i].Id))
				{
					var zone = mMapZones[mDataManager.MapZoneModels[i].Id];
					bool isShowShooter = zone.Phase == 1 || mDataManager.State == BetHorsePhaseType.PHASE_TYPE_RESULT || mDataManager.State == BetHorsePhaseType.PHASE_TYPE_DAY_END;
					zone.SetData(mDataManager.MapZoneModels[i], isShowShooter, isAutoSelectShooter, isShowWinLine);
					if ((mDataManager.State == BetHorsePhaseType.PHASE_TYPE_RESULT || mDataManager.State == BetHorsePhaseType.PHASE_TYPE_DAY_END) && zone.Phase == 3)
					{
						ShowChampion(zone, mDataManager.MapZoneModels[i].Shooters);
					}
					isAutoSelectShooter = false;
				}
			}

			mIsNeedSetMapZoneData = true;
		}

		void StopAnimation()
		{
			mAnimationDuration = 0f;
			mAnimationDelta = 0f;
			mIsRandomShooter = false;
		}

		void OnToggleShooterInfo(bool value)
		{
			if (value)
			{
				if (mDataManager == null || mSelectShooter == null)
					return;

				var shooterModel = mDataManager.GetShooterModel(mSelectShooter.Id);
				if (shooterModel == null)
					return;
				ShowShooterInfo(shooterModel);
				mIsShowShooterRecord = false;
			}
		}

		void OnToggleShooterRecord(bool value)
		{
			if (value)
			{
				if (mDataManager == null || mSelectShooter == null)
					return;

				var shooterModel = mDataManager.GetShooterModel(mSelectShooter.Id);
				if (shooterModel == null)
					return;

				if (shooterModel.IsUnknown && mDataManager.State == BetHorsePhaseType.PHASE_TYPE_STAKE)
				{
					mToggleShooterInfo.isOn = true;
				}
				else
				{
					var shooterRecords = mDataManager.GetShooterHistory(mSelectShooter.Id);
					ShowShooterRecord(shooterRecords);
					mIsShowShooterRecord = true;
				}
			}
		}

		void OnSelectShooter(HorseGamblingMapShooter shooter)
		{
			if (shooter == null)
				return;

			if (mDataManager == null)
				return;

			if (mIsRandomShooter)
				return;

			var shooterModel = mDataManager.GetShooterModel(shooter.Id);
			if (shooterModel == null)
				return;

			if (mSelectShooter != null)
			{
				mSelectShooter.SetSelected(false);
			}

			mSelectShooter = shooter;
			mSelectShooter.SetSelected(true);
			mTextShooterName.SafeSetText(shooterModel.Name);
			if (mPortraitSelectAnim != null)
				mPortraitSelectAnim.DORestart();

			ETCImageLoader.LoadSprite(ref mImageShooterPortrait, shooterModel.PortraitPath);
			if (mIsShowShooterRecord)
			{
				if (shooterModel.IsUnknown && mDataManager.State == BetHorsePhaseType.PHASE_TYPE_STAKE)
				{
					mToggleShooterInfo.isOn = true;
				}
				else
				{
					var shooterRecords = mDataManager.GetShooterHistory(mSelectShooter.Id);
					ShowShooterRecord(shooterRecords);
				}
			}
			else
			{
				ShowShooterInfo(shooterModel);
			}
			if (shooterModel.IsUnknown && mDataManager.State == BetHorsePhaseType.PHASE_TYPE_STAKE)
			{
				mToggleShooterRecord.enabled = false;
				mToggleShooterRecordGray.enabled = true;
			}
			else
			{
				mToggleShooterRecord.enabled = true;
				mToggleShooterRecordGray.enabled = false;
			}
		}

		void ShowShooterInfo(IHorseGamblingShooterModel shooterModel)
		{
			if (mIsRandomShooter || mDataManager == null)
				return;

			mShooterInfoGO.CustomActive(true);
			mShooterRecordGO.CustomActive(false);
			mImageShooterStatus.CustomActive(true);
			mImageShooterOccu.CustomActive(true);
			if (shooterModel.IsUnknown && mDataManager.State == BetHorsePhaseType.PHASE_TYPE_STAKE)
			{
				ResetShooterInfo();
			}
			else
			{
				for (int i = 0; i < mShooterStatusDatas.Count; ++i)
				{

					if (mShooterStatusDatas[i].Status == shooterModel.Status)
					{
						mTextShooterStatus.SafeSetText(mShooterStatusDatas[i].Description);
						if (!string.IsNullOrEmpty(mShooterStatusDatas[i].Icon))
							ETCImageLoader.LoadSprite(ref mImageShooterStatus, mShooterStatusDatas[i].Icon);
						break;
					}
				}

				mTextShooterOccu.SafeSetText(shooterModel.Occupation);
				mTextShooterTerrain.SafeSetText(shooterModel.Terrain);
				mTextShooterWeather.SafeSetText(shooterModel.Weather);
				mTextShooterWinRate.SafeSetText(string.Format(TR.Value("horse_gambling_shooter_win_rate"), shooterModel.WinRate));
				mTextShooterChampionCount.SafeSetText(string.Format(TR.Value("horse_gambling_shooter_champion_count"), shooterModel.ChampionCount));
				ETCImageLoader.LoadSprite(ref mImageShooterOccu, shooterModel.OccupationIcon);
				ETCImageLoader.LoadSprite(ref mImageShooterPortrait, shooterModel.PortraitPath);
				mTextShooterName.SafeSetText(shooterModel.Name);
			}
		}

		void ShowShooterRecord(ShooterRecord[] records)
		{
			mShooterInfoGO.CustomActive(false);
			mShooterRecordGO.CustomActive(true);
			if (records != null)
				mShooterRecordList.UpdateElementAmount(records.Length);
		}

		void OnShooterRecordItemVisible(ComUIListElementScript item)
		{
			if (item == null)
				return;

			var script = item.GetComponent<HorseGamblingShooterRecordItem>();
			if (script == null)
			{
				Logger.LogError(string.Format("路径在{0}下的预置上没有挂载脚本HorseGamblingShooterRecordItem", mRecordItemPath));
				return;
			}

			if (mDataManager == null || mSelectShooter == null)
			{
				return;
			}
			var shooterRecords = mDataManager.GetShooterHistory(mSelectShooter.Id);
			if (shooterRecords == null)
				return;

			int index = shooterRecords.Length - 1 - item.m_index;
			if (index >= shooterRecords.Length || index < 0)
				return;

			var record = shooterRecords[shooterRecords.Length - 1 - item.m_index];
			script.Init(record.coutrId.ToString(), ((float)record.odds / 10000).ToString(), record.result == 1, item.m_index % 2 == 0 ? mShooterRecordBg2 : mShooterRecordBg1);
		}
	}
}