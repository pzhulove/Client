using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameClient
{
	public class HorseGamblingBattleAnim : MonoBehaviour
	{
		[SerializeField] private GameObject mEffet;
		[SerializeField] private GameObject mStaticEffet;

		public void Play()
		{
			mEffet.CustomActive(true);
			mStaticEffet.CustomActive(false);
		}

		public void Stop()
		{
			mEffet.CustomActive(false);
			mStaticEffet.CustomActive(true);
		}

	}
}