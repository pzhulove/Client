using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace GameClient
{
    class TalkAwardFrame : ClientFrame
    {
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Mission/TalkAwardFrame";
        }

        [UIControl("TaskName", typeof(Text))]
        Text Name;
        [UIObject("ItemParent")]
        GameObject goParent;

        protected override void _OnOpenFrame()
        {
            string param = userData as string;
            int iId = 0;
            if(!int.TryParse(param,out iId))
            {
                return;
            }
            var missionItem = TableManager.GetInstance().GetTableItem<ProtoTable.MissionTable>(iId);
            if(missionItem == null)
            {
                return;
            }

            Name.text = missionItem.TaskName;

            var awards = MissionManager.GetInstance().GetMissionAwards(iId);
            for (int j = 0; j < awards.Count; ++j)
            {
                ItemData itemData = ItemDataManager.GetInstance().GetCommonItemTableDataByID(awards[j].ID);
                if(itemData != null)
                {
                    itemData.Count = awards[j].Num;
                    itemData.StrengthenLevel = awards[j].StrengthenLevel;
                    itemData.EquipType = (EEquipType)awards[j].EquipType;

                    ComItem comItem = CreateComItem(goParent);
                    if(comItem != null)
                    {
                        comItem.Setup(itemData, (GameObject obj, ItemData item)=>
                        {
                            if(item != null)
                            {
                                ItemTipManager.GetInstance().ShowTip(item);
                            }
                        });
                    }
                }
            }
        }

        protected override void _OnCloseFrame()
        {
            ItemTipManager.GetInstance().CloseAll();
        }
    }
}