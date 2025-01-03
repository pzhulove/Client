using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using GameClient;

public class UIInputCallback : MonoBehaviour
{
    public void ClearAllCharacter()
    {
#if UNITY_EDITOR
		if (!Global.Settings.isDebug)
			return;
        if (BattleMain.mode == eDungeonMode.SyncFrame) return;
        if (BattleMain.instance.GetDungeonManager().GetBeScene().state == BeSceneState.onFight)
        {
            BattleMain.instance.Main.ClearAllCharacter();
        }
#endif
    }
}
