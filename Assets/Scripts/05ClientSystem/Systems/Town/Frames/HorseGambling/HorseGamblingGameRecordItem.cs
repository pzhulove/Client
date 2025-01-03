using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
	public class HorseGamblingGameRecordItem : MonoBehaviour
	{
		[SerializeField] private Image mImageBg;
		[SerializeField] private Text mText1;
		[SerializeField] private Image mImageIcon;
		[SerializeField] private Text mText2;
		[SerializeField] private Text mText3;
		[SerializeField] private Text mText4;

		public void Init(string text1, string text2, string text3, string text4, string imagePath, string bgPath)
		{
			mText1.SafeSetText(text1);
			mText2.SafeSetText(text2);
			mText3.SafeSetText(text3);
			mText4.SafeSetText(text4);

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