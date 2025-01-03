using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HorseGamblingSupplyShooter : MonoBehaviour {

	[SerializeField] private Image mImageIcon;
	[SerializeField] private Text mTextSupply;
	[SerializeField] private GameObject mWinGO;
	[SerializeField] private GameObject mLoseGO;
	[SerializeField] private UIGray mGray;
	[SerializeField] private GameObject mImageDeath;

	public void Init(string iconPath, int supply, bool isShowResult = false, bool isWin = false)
	{
		if (!string.IsNullOrEmpty(iconPath))
			ETCImageLoader.LoadSprite(ref mImageIcon, iconPath);

		if (isShowResult)
		{
			mWinGO.CustomActive(isWin);
			mLoseGO.CustomActive(!isWin);
			mImageDeath.CustomActive(!isWin);
			mGray.enabled = !isWin;
		}
		else
		{
			mWinGO.CustomActive(false);
			mLoseGO.CustomActive(false);
			mImageDeath.CustomActive(false);
			mGray.enabled = false;
		}

		mTextSupply.SafeSetText(supply.ToString());
	}
}
