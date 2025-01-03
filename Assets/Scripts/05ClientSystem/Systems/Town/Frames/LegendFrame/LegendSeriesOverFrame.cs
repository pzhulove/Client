using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace GameClient
{
    class LegendSeriesOverFrameData
    {
       public ProtoTable.LegendMainTable mainItem;
    }

    class LegendSeriesOverFrame : ClientFrame
    {
        LegendSeriesOverFrameData data = null;

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/LegendFrame/LegendSeriesOverFrame";
        }

        public static void OpenLinkFrame(string strParam)
        {
            int iTableID = 0;
            if(!int.TryParse(strParam,out iTableID))
            {
                return;
            }

            ProtoTable.LegendMainTable mainItem = TableManager.GetInstance().GetTableItem<ProtoTable.LegendMainTable>(iTableID);
            if(null == mainItem)
            {
                return;
            }

            ClientSystemManager.GetInstance().OpenFrame<LegendSeriesOverFrame>(FrameLayer.Middle, new LegendSeriesOverFrameData
            {
                mainItem = mainItem
            });
        }

        protected override void _OnOpenFrame()
        {
            data = userData as LegendSeriesOverFrameData;

            _AddButton("Close", () => { frameMgr.CloseFrame(this); });

            GameObject goParent = Utility.FindChild(frame, "AwardArray");
            GameObject goPrefab = Utility.FindChild(frame, "AwardArray/Item");
            goPrefab.CustomActive(false);

            StateController comStateController = frame.GetComponent<StateController>();

            for (int i = 0; i < data.mainItem.missionIds.Count; ++i)
            {
                var missionItem = TableManager.GetInstance().GetTableItem<ProtoTable.MissionTable>(data.mainItem.missionIds[i]);
                if(null != missionItem && missionItem.MissionOnOff == 1)
                {
                    var awards = MissionManager.GetInstance().GetMissionAwards(missionItem.ID);
                    if(null != awards)
                    {
                        for(int j = 0; j < awards.Count; ++j)
                        {
                            var itemData = ItemDataManager.CreateItemDataFromTable(awards[j].ID);
                            if(null != itemData && awards[j].Num > 0)
                            {
                                itemData.Count = awards[j].Num;
                                GameObject goCurrent = GameObject.Instantiate(goPrefab);
                                if(null != goCurrent)
                                {
                                    Utility.AttachTo(goCurrent, goParent);
                                    goCurrent.CustomActive(true);
                                    ComItem comItem = CreateComItem(Utility.FindChild(goCurrent, "ItemParent").gameObject);
                                    comItem.Setup(itemData, null);
                                    Text name = Utility.FindComponent<Text>(goCurrent, "Name");
                                    name.text = itemData.GetColorName();
                                }
                            }
                        }
                    }
                }
            }

            if(null != comStateController)
            {
                comStateController.Key = data.mainItem.ID.ToString();
            }
        }

        protected override void _OnCloseFrame()
        {

        }
    }
}