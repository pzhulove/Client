using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace GameClient
{
    class LegendFinishFrameData
    {
        public ProtoTable.MissionTable missionItem;
        public int iSeriesID;
        public ProtoTable.LinkTable linkItem;
    }

    class LegendFinishFrame : ClientFrame
    {
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/LegendFrame/LegendFinishFrame";
        }

        public static void OpenLinkFrame(string strParam)
        {
            var tokens = strParam.Split('|');
            if(tokens.Length < 2)
            {
                return;
            }

            int iID = 0;
            if(!int.TryParse(tokens[0], out iID))
            {
                return;
            }

            int iSeriesID = 0;
            if(!int.TryParse(tokens[1],out iSeriesID))
            {
                return;
            }

            ProtoTable.LinkTable linkItem = null;
            if(tokens.Length == 3)
            {
                int iLinkId = 0;
                if(int.TryParse(tokens[1], out iLinkId))
                {
                    linkItem = TableManager.GetInstance().GetTableItem<ProtoTable.LinkTable>(iLinkId);
                }
            }

            var missionItem = TableManager.GetInstance().GetTableItem<ProtoTable.MissionTable>(iID);
            if(null == missionItem)
            {
                return;
            }

            ClientSystemManager.GetInstance().OpenFrame<LegendFinishFrame>(FrameLayer.Middle, new LegendFinishFrameData
            {
                missionItem = missionItem,
                linkItem = linkItem,
                iSeriesID = iSeriesID,
            });
        }


        LegendFinishFrameData data = null;
        protected override void _OnOpenFrame()
        {
            data = userData as LegendFinishFrameData;

            _AddButton("Bg/BG/Close",
                () => 
                {
                    frameMgr.CloseFrame(this);
                });

            if(null != data)
            {
                GameObject goItemParent = Utility.FindChild(frame, "Bg/BG/ItemParent");
                ComItem comItem = CreateComItem(goItemParent);
                var awards = MissionManager.GetInstance().GetMissionAwards(data.missionItem.ID);
                ItemData itemData = null;
                if(null != awards && awards.Count > 0)
                {
                    itemData = ItemDataManager.CreateItemDataFromTable(awards[0].ID);
                    itemData.Count = awards[0].Num;
                }
                if(null != comItem)
                {
                    comItem.Setup(itemData, (GameObject obj, ItemData item)=>
                    {
                        ItemTipManager.GetInstance().ShowTip(item, null);
                    });
                }
                Text name = Utility.FindComponent<Text>(frame, "Bg/BG/Name");
                if(null != itemData)
                {
                    name.text = itemData.GetColorName();
                }
                Text desc = Utility.FindComponent<Text>(frame, "Bg/BG/Desc");
                desc.text = TR.Value("legend_mission_finish_hint", data.missionItem.TaskName);
            }
        }

        protected override void _OnCloseFrame()
        {
            if(null != data.linkItem)
            {
#if UNITY_EDITOR
                //Logger.LogErrorFormat("LegendFinishFrame LinkInfo = {0}",data.linkItem.LinkInfo);
#endif

                if (null != data.missionItem)
                {
                    if (Utility.IsLegendSeriesOverOnce(data.iSeriesID, data.missionItem.ID))
                    {
                        ActiveManager.GetInstance().OnClickLinkInfo(data.linkItem.LinkInfo, null);
                    }
                }
            }
            data = null;
        }
    }
}