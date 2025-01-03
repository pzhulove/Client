using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
	public class HorseGamblingShooterRecordItem : MonoBehaviour, IDisposable
	{
		[SerializeField] private Text mTextGameId;
		[SerializeField] private Text mTextOdds;
		[SerializeField] private Image mImageResult;
		[SerializeField] private Image mImageBg;
		[SerializeField] private string mWinSpritePath;
		[SerializeField] private string mLoseSpritePath;

		public void Init(string gameId, string odds, bool isWin, string bg)
		{
			mTextGameId.SafeSetText(gameId);
			mTextOdds.SafeSetText(odds);
			if (isWin)
			{
				ETCImageLoader.LoadSprite(ref mImageResult, mWinSpritePath);
			}
			else
			{
				ETCImageLoader.LoadSprite(ref mImageResult, mLoseSpritePath);
			}

			ETCImageLoader.LoadSprite(ref mImageBg, bg);
		}

		public void Dispose()
		{
		}
	}
}