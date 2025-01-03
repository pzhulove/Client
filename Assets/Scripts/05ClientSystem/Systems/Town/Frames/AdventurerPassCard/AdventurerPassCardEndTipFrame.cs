using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class AdventurerPassCardEndTipFrame : ClientFrame
    {
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/AdventurerPassCard/AdventurerPassCardEndTipFrame";
        }

        protected override void _OnOpenFrame()
        {
            if (null != mTextTip)
            {
                int leftTime = AdventurerPassCardDataManager.GetInstance().GetSeasonLeftTime();
                string leftTimeStr;
                if (leftTime / (24 * 3600) > 0)
                {
                    leftTimeStr = TR.Value("adventurer_pass_card_end_tip_day", leftTime / (24 * 3600));
                }
                else
                {
                    leftTimeStr = TR.Value("adventurer_pass_card_end_tip_time", leftTime / 3600, leftTime % 3600 / 60, leftTime % 60);
                }
                mTextTip.SafeSetText(TR.Value("adventurer_pass_card_end_tip", leftTimeStr));
            }
        }

        protected override void _OnCloseFrame()
        {
        }

        private Text mTextTip;
        protected override void _bindExUI()
        {
            mTextTip = mBind.GetCom<Text>("TextTip");
        }

        protected override void _unbindExUI()
        {
            mTextTip = null;
        }
    }
}
