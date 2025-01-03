using UnityEngine.UI;
using Protocol;
using UnityEngine;

namespace GameClient
{
    /// <summary>
    /// 暂停按钮
    /// </summary>
    public class BattleUIPauseBtn : BattleUIBase
    {
        #region ExtraUIBind
        private Button mPauseButton = null;

        protected override void _bindExUI()
        {
            mPauseButton = mBind.GetCom<Button>("PauseButton");
            mPauseButton.onClick.AddListener(_onPauseButtonButtonClick);
        }

        protected override void _unbindExUI()
        {
            mPauseButton.onClick.RemoveListener(_onPauseButtonButtonClick);
            mPauseButton = null;
        }
        #endregion

        #region Callback
        private void _onPauseButtonButtonClick()
        {
            ClientSystemManager.instance.OpenFrame<PauseFrame>(FrameLayer.Middle);

            if (!BattleMain.IsModeMultiplayer(BattleMain.mode))
                BattleMain.instance.GetDungeonManager().PauseFight();
        }
        #endregion

        protected override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/BattleUI/BattleUIComponent/BattleUIPauseBtn";
        }

        public void HidePauseButton(bool isHide = true)
        {
            mPauseButton.CustomActive(!isHide);
        }
    }
}
