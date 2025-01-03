using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace GameClient
{
    public class ActiveAwardFrameData
    {
        public string title;
        public List<AwardItemData> datas;
    }

    class ActiveAwardFrame : ClientFrame
    {
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Activity/ActiveAwardFrame";
        }

        ActiveAwardFrameData awardsData = null;

        [UIControl("Title/Text", typeof(Text))]
        Text m_kTitle;
        [UIObject("Awards")]
        GameObject goParent;
        [UIObject("Awards/ItemParent")]
        GameObject goPrefabs;

        [UIEventHandle("ButtonTempBlue")]
        void _OnClickClose()
        {
            frameMgr.CloseFrame(this);
        }

        void _SetAwards()
        {
            if(awardsData == null)
            {
                return;
            }

            m_kTitle.text = awardsData.title;

            for(int i = 0; awardsData.datas != null && i < awardsData.datas.Count; ++i)
            {
                var award = awardsData.datas[i];
                if (award == null || award.Num <= 0)
                {
                    continue;
                }

                var itemData = ItemDataManager.GetInstance().GetCommonItemTableDataByID(award.ID);
                if (itemData != null)
                {
                    itemData.Count = award.Num;
                    GameObject goCurrent = GameObject.Instantiate(goPrefabs) as GameObject;
                    if (goCurrent != null)
                    {
                        Utility.AttachTo(goCurrent, goParent);
                        goCurrent.CustomActive(true);

                        ComItem comItem = CreateComItem(goCurrent);
                        comItem.Setup(itemData, _OnItemClicked);
                        comItem.gameObject.transform.SetAsFirstSibling();

                        Text kName = Utility.FindComponent<Text>(goCurrent,"Name");
                        kName.text = itemData.GetColorName();
                    }
                }
            }
        }

        void _OnItemClicked(GameObject obj, ItemData item)
        {
            if(item != null)
            {
                ItemTipManager.GetInstance().ShowTip(item);
            }
        }

        protected override void _OnOpenFrame()
        {
            awardsData = userData as ActiveAwardFrameData;
            goPrefabs.CustomActive(false);

            _SetAwards();
        }

        protected override void _OnCloseFrame()
        {
            awardsData = null;
        }
    }
}