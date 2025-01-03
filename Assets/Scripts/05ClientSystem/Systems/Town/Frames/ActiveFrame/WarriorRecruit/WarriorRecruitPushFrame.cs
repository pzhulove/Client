using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class WarriorRecruitPushFrame : ClientFrame
    {
        private List<ComCommonBind> mComBindList = new List<ComCommonBind>();

        #region ExtraUIBind
        private Button mClose = null;
        private Button mGo = null;
        private ComCommonBind mComItem4 = null;
        private ComCommonBind mComItem3 = null;
        private ComCommonBind mComItem2 = null;
        private ComCommonBind mComItem1 = null;

        protected override void _bindExUI()
        {
            mClose = mBind.GetCom<Button>("Close");
            mClose.onClick.AddListener(_onCloseButtonClick);
            mGo = mBind.GetCom<Button>("Go");
            mGo.onClick.AddListener(_onGoButtonClick);
            mComItem4 = mBind.GetCom<ComCommonBind>("ComItem4");
            mComItem3 = mBind.GetCom<ComCommonBind>("ComItem3");
            mComItem2 = mBind.GetCom<ComCommonBind>("ComItem2");
            mComItem1 = mBind.GetCom<ComCommonBind>("ComItem1");

            mComBindList.Add(mComItem1);
            mComBindList.Add(mComItem2);
            mComBindList.Add(mComItem3);
            mComBindList.Add(mComItem4);
        }

        protected override void _unbindExUI()
        {
            mClose.onClick.RemoveListener(_onCloseButtonClick);
            mClose = null;
            mGo.onClick.RemoveListener(_onGoButtonClick);
            mGo = null;
            mComItem4 = null;
            mComItem3 = null;
            mComItem2 = null;
            mComItem1 = null;

            if (mComBindList != null)
                mComBindList.Clear();
        }
        #endregion

        #region Callback
        private void _onGoButtonClick()
        {
            ActiveManager.GetInstance().OpenActiveFrame(9380, 8800);
            _onCloseButtonClick();
        }

        private void _onCloseButtonClick()
        {
            frameMgr.CloseFrame(this);
        }
        #endregion

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Activity/WarriorRecruit/WarriorRecruitPushFrame";
        }

        protected override void _OnOpenFrame()
        {
            base._OnOpenFrame();
            InitInterface();
            WarriorRecruitDataManager.GetInstance().SendWorldQueryHirePushReq(1);
        }

        protected override void _OnCloseFrame()
        {
            base._OnCloseFrame();
            FollowingOpenQueueManager.GetInstance().NotifyCurrentOrderClosed();
        }

        private void InitInterface()
        {
            for (int i = 0; i < WarriorRecruitDataManager.GetInstance().mRecruitmentBonusPreview_OldPlayerList.Count; i++)
            {
                if (i <= mComBindList.Count)
                {
                    int itemId = WarriorRecruitDataManager.GetInstance().mRecruitmentBonusPreview_OldPlayerList[i];
                    ItemData itemData = ItemDataManager.CreateItemDataFromTable(itemId);
                    if (itemData == null)
                        continue;

                    ComCommonBind bind = mComBindList[i];
                    if (bind == null)
                    {
                        continue;
                    }

                    Text name = bind.GetCom<Text>("Name");
                    Image icon = bind.GetCom<Image>("Icon");
                    Button iconBtn = bind.GetCom<Button>("Iconbtn");
                    Image background = bind.GetCom<Image>("Backgroud");

                    if (name != null)
                    {
                        name.text = itemData.GetColorName();
                    }

                    if (background != null)
                    {
                        ETCImageLoader.LoadSprite(ref background, itemData.GetQualityInfo().Background);
                    }

                    if (icon != null)
                    {
                        ETCImageLoader.LoadSprite(ref icon, itemData.Icon);
                    }

                    if (iconBtn != null)
                    {
                        iconBtn.onClick.RemoveAllListeners();
                        iconBtn.onClick.AddListener(() => { ItemTipManager.GetInstance().ShowTip(itemData); });
                    }
                }
            }
        }
    }
}