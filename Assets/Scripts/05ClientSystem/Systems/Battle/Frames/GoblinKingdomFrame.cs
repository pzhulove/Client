using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class GoblinKingdomFrame : ClientFrame
    {
        #region ExtraUIBind
        private Text mTime = null;
        private Text mNum = null;
        private RectTransform mRoom2 = null;


        private RectTransform mRoom1;
        private Text time;
        protected override void _bindExUI()
        {
            mTime = mBind.GetCom<Text>("Time");
            mNum = mBind.GetCom<Text>("Num");
            mRoom2 = mBind.GetCom<RectTransform>("Room2");

            mRoom1 = mBind.GetCom<RectTransform>("Room1");
            time = mBind.GetCom<Text>("time");
        }

        protected override void _unbindExUI()
        {
            mTime = null;
            mNum = null;
            mRoom2 = null;
        }
        #endregion

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/BattleUI/GoblinKingdomFrame";
        }

        protected override void _OnOpenFrame()
        {
            mRoom2.gameObject.CustomActive(true);
        }

        public void SetRoom()
        {
            mRoom1.gameObject.SetActive(true);
            mRoom2.gameObject.CustomActive(false);
        }

        public void SetTime(string text)
        {
            if (time == null)
                return;
            time.text = text;
        }

        public void SetTimeText(string text)
        {
            if (mTime == null)
                return;
            mTime.text = text;
        }

        public void SetNumText(string text)
        {
            if (mTime == null)
                return;
            mNum.text = text;
        }
    }
}
