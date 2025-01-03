using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{

    public class ChallengeChapterFrame : ClientFrame
    {

        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Challenge/ChallengeChapterFrame";
        }

        protected override void _OnOpenFrame()
        {
            base._OnOpenFrame();

            if (mChallengeChapterView != null)
            {
                ChallengeChapterParamDataModel chapterParamData = null;
                if (userData != null)
                    chapterParamData = (ChallengeChapterParamDataModel) userData;
                mChallengeChapterView.InitView(chapterParamData);
            }
        }

        #region ExtraUIBind
        private ChallengeChapterView mChallengeChapterView = null;

        protected override void _bindExUI()
        {
            mChallengeChapterView = mBind.GetCom<ChallengeChapterView>("ChallengeChapterView");
        }

        protected override void _unbindExUI()
        {
            mChallengeChapterView = null;
        }
        #endregion
        
    }

}
