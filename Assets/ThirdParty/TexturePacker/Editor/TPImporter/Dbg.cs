using System;
using UnityEngine;

namespace TPImporter
{
	public class Dbg
	{
		public static bool enabled = false;

		public static void Log(string msg)
		{ 
			if (Dbg.enabled)
			{
				Debug.Log("TPImporter: " + msg);
			}
		}
	}
}
