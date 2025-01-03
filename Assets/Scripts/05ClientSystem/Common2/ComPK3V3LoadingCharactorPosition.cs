using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameClient;


[ExecuteAlways]
public class ComPK3V3LoadingCharactorPosition : MonoBehaviour 
{
	[HideInInspector]
	public Vector3 redTeamPosition = Vector3.zero;

	[HideInInspector]
	public Vector3 redTeamScale    = Vector3.zero;

	[HideInInspector]
	public Vector3 blueTeamPosition = Vector3.zero;

	[HideInInspector]
	public Vector3 blueTeamScale    = Vector3.zero;

	private RectTransform mRect = null;

	void Awake()
	{
		_init();
	}

	private void _init()
	{
		mRect = GetComponent<RectTransform>();
	}

	public void SetTeamType(BattlePlayer.eDungeonPlayerTeamType type)
	{
		if (null == mRect)
		{
			return ;
		}

		switch (type)
		{
			case BattlePlayer.eDungeonPlayerTeamType.eTeamBlue:
				mRect.localPosition = blueTeamPosition;
				mRect.localScale    = blueTeamScale;
				break;
			case BattlePlayer.eDungeonPlayerTeamType.eTeamRed:
				mRect.localPosition = redTeamPosition;
				mRect.localScale    = redTeamScale;
				break;
		}
	}
}

