using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameClient
{
    public class HeadPortraitFrame : ClientFrame
    {
        private HeadPortraitTabType mCurrentSelectHeadPortraitTabType = HeadPortraitTabType.HPTT_ALL;

        #region ExtraUIBind
        private HeadPortraitFrameView mHeadPortraitFrameView = null;

        protected sealed override void _bindExUI()
        {
            mHeadPortraitFrameView = mBind.GetCom<HeadPortraitFrameView>("HeadPortraitFrameView");
        }

        protected sealed override void _unbindExUI()
        {
            mHeadPortraitFrameView = null;
        }
        #endregion

        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/PersonalSettingFrame/HeadPortraitFrame/HeadPortraitFrame";
        }

        protected sealed override void _OnOpenFrame()
        {
            RegistUIEventHandle();

            UpdateWearHeadPortraitFrameID();

            mHeadPortraitFrameView.InitView(OnHeadPortraitTabItemClick);
        }

        protected sealed override void _OnCloseFrame()
        {
            UnRegistUIEventHandle();
            mCurrentSelectHeadPortraitTabType = HeadPortraitTabType.HPTT_ALL;
        }

        private void RegistUIEventHandle()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.UseHeadPortraitFrameSuccess, UpdateHeadPortraitItemInfo);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.HeadPortraitFrameNotify, UpdateHeadPortraitItemInfo);
        }

        private void UnRegistUIEventHandle()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.UseHeadPortraitFrameSuccess, UpdateHeadPortraitItemInfo);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.HeadPortraitFrameNotify, UpdateHeadPortraitItemInfo);
        }

        private void UpdateHeadPortraitItemInfo(UIEvent uiEvent)
        {
            UpdateWearHeadPortraitFrameID();
            UpdateHeadProtraitItem(mCurrentSelectHeadPortraitTabType);
        }

        private void OnHeadPortraitTabItemClick(HeadPortraitTabDataModel data)
        {
            if (data == null)
            {
                return;
            }
            mCurrentSelectHeadPortraitTabType = data.tabType;
            
            UpdateHeadProtraitItem(mCurrentSelectHeadPortraitTabType, true);
        }

        private void UpdateHeadProtraitItem(HeadPortraitTabType tabType, bool isResetIndex = false)
        {
            var headPortraitItemList = HeadPortraitFrameDataManager.GetInstance().GetHeadPortraitItemList(tabType);

            if (headPortraitItemList != null)
            {
                mHeadPortraitFrameView.UpdateHeadProtraitItem(headPortraitItemList, isResetIndex);
            }
            else
            {
                mHeadPortraitFrameView.UpdateHeadProtraitItem(new List<HeadPortraitItemData>(), isResetIndex);
            }
        }

        private void UpdateWearHeadPortraitFrameID()
        {
            if (HeadPortraitFrameDataManager.WearHeadPortraitFrameID == 0)
            {
                HeadPortraitFrameDataManager.WearHeadPortraitFrameID = HeadPortraitFrameDataManager.iDefaultHeadPortraitID;
            }
        }
    }
}

