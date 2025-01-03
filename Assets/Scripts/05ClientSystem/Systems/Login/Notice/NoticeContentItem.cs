using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class NoticeContentItem : MonoBehaviour
    {
        [SerializeField] private NewSuperLinkText mSuperLinkText;
        [SerializeField] private string mTitleBgPath;
        [SerializeField] private Font mTitleFont;
        [SerializeField] private Font mNormalFont;

        private ImageEx mImageTitleBg;
        private string mStrContent;

        public void Init(string str, ClientFrame clientFrame, bool isTitleLine)
        {
            if (mSuperLinkText != null)
            {
                mSuperLinkText.SafeSetText(str);
            }

            mStrContent = str;

            if (isTitleLine)
            {
                mSuperLinkText.alignment = TextAnchor.UpperCenter;
                if (mImageTitleBg != null)
                {
                    _InitTitleBg();
                }
                else
                {
                    UIManager.instance.LoadObject(clientFrame, mTitleBgPath, null, _LoadSucc, typeof(GameObject),false, false);
                }
                mSuperLinkText.font = mTitleFont;
            }
            else
            {
                mSuperLinkText.alignment = TextAnchor.UpperLeft;
                mImageTitleBg.CustomActiveAlpha(false);
                mSuperLinkText.font = mNormalFont;
            }
        }

        private void _LoadSucc(string path, object asset, object userData)
        {
            var go = asset as GameObject;
            go.transform.SetParent(transform, false);
            go.transform.SetAsFirstSibling();
            mImageTitleBg = go.GetComponent<ImageEx>();
            _InitTitleBg();
        }

        private void _InitTitleBg()
        {
            if (mImageTitleBg == null)
            {
                return;
            }

            string strTemp = mStrContent.Replace("\n", "");
            mImageTitleBg.transform.rectTransform().sizeDelta = new Vector2(mSuperLinkText.CalEventTextSize(strTemp).x + 160, mSuperLinkText.CalEventTextSize(strTemp).y + 8);
            mImageTitleBg.CustomActiveAlpha(true);
        }
    }
}
