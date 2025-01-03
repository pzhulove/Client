using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ComPK3V3LoadingCharactorPosition))]
public class ComPK3V3LoadingCharactorPositionEditor : Editor 
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		ComPK3V3LoadingCharactorPosition com = this.target as ComPK3V3LoadingCharactorPosition;

		EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("保存红队-左", "miniButton"))
        {
			if (null != com)
			{
				com.redTeamPosition = com.rectTransform().localPosition;
				com.redTeamScale = com.rectTransform().localScale;
				EditorUtility.SetDirty(com);
                com.SetTeamType(BattlePlayer.eDungeonPlayerTeamType.eTeamRed);
			}
        }

        if (GUILayout.Button("保存蓝队-右", "miniButton"))
        {
			if (null != com)
			{
				com.blueTeamPosition = com.rectTransform().localPosition;
				com.blueTeamScale = com.rectTransform().localScale;
				EditorUtility.SetDirty(com);
				com.SetTeamType(BattlePlayer.eDungeonPlayerTeamType.eTeamBlue);
			}
        }

		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("看下红队-左", "miniButton"))
        {
			if (null != com)
			{
                com.SetTeamType(BattlePlayer.eDungeonPlayerTeamType.eTeamRed);
			}
        }

        if (GUILayout.Button("看下蓝队呗-右", "miniButton"))
        {
			if (null != com)
			{
				com.SetTeamType(BattlePlayer.eDungeonPlayerTeamType.eTeamBlue);
			}
        }

		EditorGUILayout.EndHorizontal();

    }
}
