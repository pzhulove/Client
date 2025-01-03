using System;
using System.Collections.Generic;
using Protocol;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GameClient
{
	public class HorseGamblingMapZoneChampionComponent : MonoBehaviour, IDisposable
	{

		[SerializeField] private GameObject mShooterGO;
		[SerializeField] private GameObject mEffect;
		[SerializeField] private GameObject mBattleEffect;

		private HorseGamblingMapShooter mShooter;

		public void Init(Transform shooterRoot)
		{
			var wrapper = mShooterGO.GetComponent<UIPrefabWrapper>();
			if (wrapper == null)
			{
				//Logger.LogError(string.Format("检查赌马 挂载HorseGamblingMapZone脚本的预制体上 面板中设置的mShooterGOList中下标为{0}UIPrefabWrapper", i));
				return;
			}

			HorseGamblingMapShooter.EPosition positon = (HorseGamblingMapShooter.EPosition)wrapper.IntParam;
			mShooterGO = wrapper.LoadUIPrefab(mShooterGO.transform);
			mShooter = mShooterGO.GetComponent<HorseGamblingMapShooter>();
			if (mShooter != null)
			{
				mShooter.Position = positon;
				mShooter.transform.SetParent(shooterRoot);
				Reset();
			}
		}

		public void ShowChampion(int shooterId, UnityAction<HorseGamblingMapShooter> onShooterClick)
		{
			mEffect.CustomActive(true);

			if (mShooter != null)
			{
				mShooter.ShowChampion(onShooterClick, shooterId);
			}
			mBattleEffect.CustomActive(false);
		}

		public void Reset()
		{
			if (mShooter != null)
			{
				mShooter.Reset();
				//mShooter.CustomActive(false);
			}
			mEffect.CustomActive(false);
			mBattleEffect.CustomActive(false);
		}

		public void Dispose()
		{
			if (mShooter != null)
			{
				mShooter.Dispose();
			}
		}

	}
}