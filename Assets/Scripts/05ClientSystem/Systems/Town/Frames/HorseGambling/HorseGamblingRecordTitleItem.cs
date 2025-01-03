using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{

	public class HorseGamblingRecordTitleItem : MonoBehaviour, IDisposable
	{

		[SerializeField] private Text mTextName;

		public void Init(string name)
		{
			gameObject.CustomActive(true);
			mTextName.SafeSetText(name);
		}

		public void Dispose()
		{
			gameObject.CustomActive(false);
		}
	}
}