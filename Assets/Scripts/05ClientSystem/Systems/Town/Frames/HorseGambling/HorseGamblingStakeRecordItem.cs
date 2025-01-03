using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
	public class HorseGamblingStakeRecordItem : MonoBehaviour
	{
		[SerializeField] private Image mImageBg;
		[SerializeField] private Text mTextGameId;
		[SerializeField] private Image mImageIcon;
		[SerializeField] private Text mTextName;
		[SerializeField] private Text mTextOdds;
		[SerializeField] private Text mTextStake;
		[SerializeField] private Text mTextProfit;

		public void Init(string gameId, string name, string odds, string stake, int profit, string imagePath, string bgPath)
		{
			mTextGameId.SafeSetText(gameId);
			mTextName.SafeSetText(name);
			mTextOdds.SafeSetText(odds);
			mTextStake.SafeSetText(stake);
			if (profit == -1)
			{
				mTextProfit.SafeSetText(TR.Value("horse_gambling_profit_not_lottery"));
			}
			else
			{
				mTextProfit.SafeSetText(profit.ToString());
			}

			if (!string.IsNullOrEmpty(imagePath))
			{
				ETCImageLoader.LoadSprite(ref mImageIcon, imagePath);
			}

			if (!string.IsNullOrEmpty(bgPath))
			{
				ETCImageLoader.LoadSprite(ref mImageBg, bgPath);
			}
		}
	}
}