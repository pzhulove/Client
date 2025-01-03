using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class BattleUIDungeonDamageBar : BattleUIBase
    {
        public BattleUIDungeonDamageBar() : base() { }

        #region ExtraUIBind
        private Slider mSlider = null;
        private Text mCountDown = null;
        private Text mDamageValue = null;

        protected override void _bindExUI()
        {
            mSlider = mBind.GetCom<Slider>("Slider");
            mCountDown = mBind.GetCom<Text>("CountDown");
            mDamageValue = mBind.GetCom<Text>("DamageValue");
        }

        protected override void _unbindExUI()
        {
            mSlider = null;
            mCountDown = null;
            mDamageValue = null;
        }
        #endregion

        protected override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/BattleUI/BattleUIComponent/BattleUIDungeonDamageBar";
        }

        //倒计时
        public void ChangeCountDown(int countTime)
        {
            mCountDown.text = countTime.ToString();
        }

        public void ShowDamageBar(bool isShow)
        {
            if (mBind == null) return;
            mBind.gameObject.CustomActive(true);
        }

        //显示伤害值进度条
        public void ChangeDamageData(float current, float max)
        {
            mSlider.value = current / max;
            string currStr = (current / 10000).ToString("#0.0");
            string maxStr = (max / 10000).ToString("#0.0");
            string value = string.Format("{0}W/{1}W", currStr, maxStr);
            mDamageValue.text = value;
        }
    }
}