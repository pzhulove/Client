using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
	public class HorseGamblingShooterHistoryItem : MonoBehaviour
	{
		[SerializeField] private Image mImageBg;
		[SerializeField] private Text mTextGameId;
		[SerializeField] private Image mImageIcon;
		[SerializeField] private Text mTextName;
		[SerializeField] private Text mTextOdds;
		[SerializeField] private Image mImageResult;
		[SerializeField] private string mWinPath = "UI/Image/Packed/p_UI_Duma.png:UI_Duma_Text_Shengli";
		[SerializeField] private string mLosePath= "UI/Image/Packed/p_UI_Duma.png:UI_Duma_Text_ShiBai";

		public void Init(string gameId, string name, string odds, string imagePath, bool isWin, string bgPath)
		{
			mTextGameId.SafeSetText(gameId);
			mTextName.SafeSetText(name);
			mTextOdds.SafeSetText(odds);

			if (!string.IsNullOrEmpty(imagePath))
			{
				ETCImageLoader.LoadSprite(ref mImageIcon, imagePath);
			}

			if (!string.IsNullOrEmpty(bgPath))
			{
				ETCImageLoader.LoadSprite(ref mImageBg, bgPath);
			}

			ETCImageLoader.LoadSprite(ref mImageResult, isWin ? mWinPath : mLosePath);

		}
	}
}