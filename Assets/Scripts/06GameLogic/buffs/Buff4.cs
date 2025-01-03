using UnityEngine;
using System.Collections;

//石化
public class Buff4 : BeBuff {

	public Buff4(int bi, int buffLevel, int buffDuration, int attack = 0) : base(bi, buffLevel, buffDuration, attack)
	{}

	public override void OnFinish ()
	{
		DoWorkForInterval();
	}
}
