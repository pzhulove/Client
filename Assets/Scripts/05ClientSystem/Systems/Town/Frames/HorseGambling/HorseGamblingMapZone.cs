using System;
using System.Collections.Generic;
using Protocol;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GameClient
{
	public interface IHorseGamblingMapZoneModel
	{
		int Id { get; }
		string TerrainPath { get; }
		Dictionary<int, HorseGamblingMapShooterModel> Shooters { get; }
		int Phase { get; }
	}

	public enum EHorseGamblingBattleResult
	{
		NotBattle,//未开战
		Win,	//胜利
		Lose	//失败
	}


	public interface IHorseGamblingMapShooterModel
	{
		int Id { get; }
		//string IconPath { get; }
		string Name { get; }
		bool IsShowOdds { get; }
		EHorseGamblingBattleResult BattleState { get; }
	}

	public interface IHorseGamblingShooterModel
	{
		int Id { get; }
		string IconPath { get; }
		string PortraitPath { get; }
		string Name { get; }
		string Occupation { get; }
		string OccupationIcon { get; }
		string Terrain { get; }
		string Weather { get; }
		ShooterStatusType Status { get; }

		float WinRate { get; }
		bool IsUnknown { get; }
		int ChampionCount { get; }
		ShooterRecord[] Records { get; }
	}

	public class HorseGamblingMapZone : MonoBehaviour, IDisposable
	{
		[SerializeField] private List<GameObject> mShooterGOList;
		[SerializeField] private int mZoneId;
		[SerializeField] private Image mImageTerrain;
		[SerializeField] private bool mIsRight;
		[SerializeField] private GameObject mTopWinLine;
		[SerializeField] private GameObject mBottomWinLine;
		[SerializeField] private HorseGamblingBattleAnim mBattleAnim;

		public int Id
		{
			get { return mZoneId; }
		}

		public int Phase;

		private readonly List<HorseGamblingMapShooter> mShooterList = new List<HorseGamblingMapShooter>();
		private UnityAction<HorseGamblingMapShooter> mOnShooterClick;
		private IHorseGamblingMapZoneModel mModel;
		[SerializeField] private int mFinalZoneId = 7;

		public void Init(UnityAction<HorseGamblingMapShooter> onShooterClick, Transform shooterRoot, Transform terrainRoot, Transform lineRoot, bool isShowShooter, bool isShowBattleEffect)
		{
			Dispose();

			mOnShooterClick = onShooterClick;
			for (int i = 0; i < mShooterGOList.Count; ++i)
			{
				var wrapper = mShooterGOList[i].GetComponent<UIPrefabWrapper>();
				if (wrapper == null)
				{
					//Logger.LogError(string.Format("检查赌马 挂载HorseGamblingMapZone脚本的预制体上 面板中设置的mShooterGOList中下标为{0}UIPrefabWrapper", i));
					continue;
				}

				HorseGamblingMapShooter.EPosition positon = (HorseGamblingMapShooter.EPosition)wrapper.IntParam;
				mShooterGOList[i] = wrapper.LoadUIPrefab(mShooterGOList[i].transform);
				var script = mShooterGOList[i].GetComponent<HorseGamblingMapShooter>();
				if (script == null)
				{
					continue;
				}

				script.Position = positon;
				mShooterList.Add(script);
				//script.transform.SetParent(shooterRoot);
				script.CustomActive(isShowShooter);
				script.Init(mIsRight);
			}

			mTopWinLine.transform.SetParent(lineRoot);
			mBottomWinLine.transform.SetParent(lineRoot);
			mImageTerrain.transform.SetParent(terrainRoot);
			mBattleAnim.transform.SetParent(shooterRoot);
			mBattleAnim.CustomActive(isShowBattleEffect);
			if (isShowBattleEffect)
				mBattleAnim.Stop();
		}

		public void HideBattleAnimation()
		{
			mBattleAnim.CustomActive(false);
		}

		public void SetData(IHorseGamblingMapZoneModel model, bool isShowShooter, bool isAutoSelect = false, bool isShowWinLine = false)
		{
			if (model == null)
			{
				return;
			}

			mModel = model;
			if (string.IsNullOrEmpty(model.TerrainPath))
			{
				//Logger.LogError(string.Format("赌马预制体 地形贴图路径为空, 地形id{0}", mZoneId));
			}
			else
			{
				ETCImageLoader.LoadSprite(ref mImageTerrain, model.TerrainPath);
			}

			Phase = model.Phase;
			if (model.Shooters != null)
			{
				var shooterData = model.Shooters.Values.GetEnumerator();
				shooterData.MoveNext();
				for (int i = 0; i < mShooterList.Count; ++i)
				{
					var script = mShooterList[i];
					if (i < model.Shooters.Count)
					{
						if (shooterData.Current != null)
						{
							script.SetData(shooterData.Current, mOnShooterClick, Phase == 1, mIsRight);
							if (isAutoSelect)
							{
								mOnShooterClick(script);
								isAutoSelect = false;
							}
							script.CustomActive(isShowShooter);

							if (isShowWinLine && shooterData.Current.BattleState == EHorseGamblingBattleResult.Win)
							{
								ShowWinLine(script);
							}
							shooterData.MoveNext();
						}
					}
					else
					{
						script.Reset();
					}
				}
			}
		}

		public void ShowUnBattle(IHorseGamblingMapZoneModel model)
		{
			var shooterData = model.Shooters.Values.GetEnumerator();
			Phase = model.Phase;
			shooterData.MoveNext();
			for (int i = 0; i < mShooterList.Count; ++i)
			{
				var script = mShooterList[i];
				if (model.Shooters != null && i < model.Shooters.Count)
				{
					if (shooterData.Current != null)
					{
						script.CustomActive(true);
						script.ShowUnBattleData(shooterData.Current, mOnShooterClick);
						shooterData.MoveNext();
					}
				}
				else
				{
					script.Reset();
				}
			}
			mBattleAnim.CustomActive(true);
			mBattleAnim.Play();
		}

		void ShowWinLine(HorseGamblingMapShooter shooter)
		{
			switch (shooter.Position)
			{
				case HorseGamblingMapShooter.EPosition.None:
					mTopWinLine.CustomActive(false);
					mBottomWinLine.CustomActive(false);
					break;
				case HorseGamblingMapShooter.EPosition.Top:
					mTopWinLine.CustomActive(true);
					mBottomWinLine.CustomActive(false);
					break;
				case HorseGamblingMapShooter.EPosition.Bottom:
					mTopWinLine.CustomActive(false);
					mBottomWinLine.CustomActive(true);
					break;
			}

		}

		//public void SetChampionShooterData(IHorseGamblingMapZoneModel model)
		//{
		//	var shooterData = model.Shooters.Values.GetEnumerator();
		//	shooterData.MoveNext();
		//	for (int i = 0; i < mShooterList.Count; ++i)
		//	{
		//		var script = mShooterList[i];
		//		if (model.Shooters != null && i < model.Shooters.Count)
		//		{
		//			if (shooterData.Current != null)
		//			{
		//				script.SetId(shooterData.Current.Id);
		//				shooterData.MoveNext();
		//			}
		//		}
		//		else
		//		{
		//			script.Reset();
		//		}
		//	}
		//}
		public void ShowBattleResult(IHorseGamblingMapZoneModel model)
		{
			mBattleAnim.CustomActive(false);
			for (int i = 0; i < mShooterList.Count; ++i)
			{
				var script = mShooterList[i];
				if (model.Shooters != null && model.Shooters.ContainsKey(mShooterList[i].Id))
				{
					if (model.Shooters[mShooterList[i].Id].BattleState == EHorseGamblingBattleResult.Win)
					{
						ShowWinLine(mShooterList[i]);
					}
					script.CustomActive(true);
					script.ShowBattleResult(model.Shooters[mShooterList[i].Id]);
				}
			}
		}

		public void RandomShooter()
		{
			for (int i = 0; i < mShooterList.Count; ++i)
			{
				mShooterList[i].RandomShooter();
			}
		}

		public void StopRandomShooter()
		{
			for (int i = 0; i < mShooterList.Count; ++i)
			{
				mShooterList[i].StopRamdom();
			}
		}

		public void UpdateData(IHorseGamblingMapZoneModel model)
		{
			if (model == null || model.Shooters == null || model.Shooters.Count <= 0)
			{
				return;
			}

			Phase = model.Phase;
			mZoneId = model.Id;
			for (int i = 0; i < mShooterList.Count; ++i)
			{
				if (model.Shooters.ContainsKey(mShooterList[i].Id))
				{
					mShooterList[i].CustomActive(true);
					mShooterList[i].SetData(model.Shooters[mShooterList[i].Id], mOnShooterClick, mIsRight);
				}
				else
				{
					mShooterList[i].CustomActive(false);
				}
			}
		}

		public void UpdateOdds(IHorseGamblingMapZoneModel model)
		{
			for (int i = 0; i < mShooterList.Count; ++i)
			{
				if (model.Shooters.ContainsKey(mShooterList[i].Id))
				{
					mShooterList[i].UpdateOdds(model.Shooters[mShooterList[i].Id]);
				}
			}
		}

		public void Reset(bool isShowBattleAnim)
		{
			for (int i = 0; i < mShooterList.Count; ++i)
			{
				mShooterList[i].Reset();
				if (Phase != 1)
				{
					mShooterList[i].CustomActive(false);
				}
			}

			mBattleAnim.CustomActive(isShowBattleAnim);
			if (isShowBattleAnim)
			{
				mBattleAnim.Stop();
			}
			mTopWinLine.CustomActive(false);
			mBottomWinLine.CustomActive(false);
		}

		public void Dispose()
		{
			if (mShooterList != null)
			{
				for (int i = 0; i < mShooterList.Count; ++i)
				{
					mShooterList[i].Dispose();
				}

				mShooterList.Clear();
			}
		}

		public void PlayBattleAnimation()
		{

		}
	}
}