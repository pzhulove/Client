using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ComMouCountTips : MonoBehaviour {

    public Image mNumber;
    public Text mName;

    public Sprite[] mNumbers;

    public int monsterID;

    public void SetMonsterNumber()
    {
        var monsterData = TableManager.instance.GetTableItem<ProtoTable.UnitTable>(monsterID);
        if (null == monsterData)
        {
            mName.text = monsterData.Name;
        }

        if (BattleMain.instance != null)
        {
			List<BeActor> list = GamePool.ListPool<BeActor>.Get();
			BattleMain.instance.Main.FindMonsterByID(list, monsterID);
            if (list.Count > 0)
            {
                int cnt = list.Count % 10;
                mNumber.sprite = mNumbers[cnt];
                mNumber.SetNativeSize();
            }

			GamePool.ListPool<BeActor>.Release(list);
        }
    }
}
