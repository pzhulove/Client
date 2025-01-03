using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class GuildSetJoinLvItem : MonoBehaviour
    {
        [SerializeField]
        private Text mTxt;

        [SerializeField]
        private RectTransform mTrans;

        [SerializeField]
        private Color mHColor;

        [SerializeField]
        private int mHFontSize;

        [SerializeField]
        private Color mNColor;

        [SerializeField]
        private int mNFontSize;
        public void Init(string content)
        {
            mTxt.SafeSetText(content);
        }

        public void SetH(bool isH)
        {
            if(isH)
            {
                mTxt.SafeSetColor(mHColor);
                mTxt.SafeSetFontSize(mHFontSize);
            }
            else
            {
                mTxt.SafeSetColor(mNColor);
                mTxt.SafeSetFontSize(mNFontSize);
            }
        }
        public RectTransform GetRectTransform()
        {
            return mTrans;
        }
    }

}

