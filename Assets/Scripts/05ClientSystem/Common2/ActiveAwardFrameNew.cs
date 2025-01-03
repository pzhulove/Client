using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace GameClient
{
    public class ActiveAwardFrameDataNew
    {
        public string title;
        public List<AwardItemData> datas;
        public bool canGet;
    }

    class ActiveAwardFrameNew : ClientFrame
    {
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Activity/ActiveAwardFrameNew";
        }

        ActiveAwardFrameDataNew awardsData = null;

        [UIControl("Title/Text", typeof(Text))]
        Text m_kTitle;
        [UIObject("Awards")]
        GameObject goParent;
        [UIObject("Awards/ItemParent")]
        GameObject goPrefabs;
        [UIObject("ButtonTempBlue")]
        GameObject GetBt;


        [UIEventHandle("ButtonTempBlue")]
        void _OnClickClose()
        {
            ActiveManager.GetInstance().SendSubmitActivity(1940);
            frameMgr.CloseFrame(this);
        }

        void _SetAwards()
        {
            if (awardsData == null)
            {
                return;
            }
            if (awardsData.canGet)
            {
                var uiGray = GetBt.GetComponent<UIGray>();
                if (uiGray == null)
                {
                    Logger.LogErrorFormat("can not find uigray");
                    return;
                }
                else
                {
                    GetBt.GetComponent<Button>().enabled = true;
                    uiGray.enabled = false;
                }
            }
            else
            {
                var uiGray = GetBt.GetComponent<UIGray>();
                if (uiGray == null)
                {
                    Logger.LogErrorFormat("can not find uigray");
                    return;
                }
                else
                {
                    GetBt.GetComponent<Button>().enabled = false;
                    uiGray.enabled = true;
                }
            }
            m_kTitle.text = awardsData.title;

            for (int i = 0; awardsData.datas != null && i < awardsData.datas.Count; ++i)
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

                        Text kName = Utility.FindComponent<Text>(goCurrent, "Name");
                        kName.text = itemData.GetColorName();
                    }
                }
            }
        }

        void _OnItemClicked(GameObject obj, ItemData item)
        {
            if (item != null)
            {
                ItemTipManager.GetInstance().ShowTip(item);
            }
        }

        protected override void _OnOpenFrame()
        {
            if(userData == null)
            {
                return;
            }
            else
            {
                //var AwardData = userData as ActiveAwardFrameDataNew;
                //awardsData.canGet = AwardData.canGet;
                //for(int i=0;i<AwardData.datas.Count;i++)
                //{
                //    awardsData.datas.Add(AwardData.datas[i]);
                //}
                //awardsData.title = AwardData.title;
                awardsData = userData as ActiveAwardFrameDataNew;
                goPrefabs.CustomActive(false);

                _SetAwards();
            }        }

        protected override void _OnCloseFrame()
        {
            awardsData = null;
        }
    }
}