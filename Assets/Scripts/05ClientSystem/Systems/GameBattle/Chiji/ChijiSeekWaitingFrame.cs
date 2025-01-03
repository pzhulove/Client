using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class ChijiSeekWaitingFrame : ClientFrame
    {
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Chiji/ChijiSeekWaitFrame";
        }

        protected override void _OnOpenFrame()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.UpdateChijiPrepareScenePlayerNum, OnUpdateChijiPrepareScenePlayerNum);
            UpdateCountDownDesc();
        }

        protected override void _OnCloseFrame()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.UpdateChijiPrepareScenePlayerNum, OnUpdateChijiPrepareScenePlayerNum);
        }

        private void OnUpdateChijiPrepareScenePlayerNum(UIEvent uiEvent)
        {
            UpdateCountDownDesc();
        }

        private void UpdateCountDownDesc()
        {
            mCountDown.text = TR.Value("Chiji_Seek_Wait_Desc", ChijiDataManager.GetInstance().PrepareScenePlayerNum, ChijiDataManager.GetInstance().PrepareSceneMaxPlayerNum);
        }

        #region ExtraUIBind
        private Text mCountDown = null;

        protected override void _bindExUI()
        {
            mCountDown = mBind.GetCom<Text>("CountDown");
        }

        protected override void _unbindExUI()
        {
            mCountDown = null;
        }
        #endregion
    }
}