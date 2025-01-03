using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class SkillAttrItem : MonoBehaviour
    {
        [SerializeField] private Color mSingleColor;
        [SerializeField] private Color mDoubleColor;
        [SerializeField] private Image mImgBg;
        [SerializeField] private Text mTextName;
        [SerializeField] private Text mTextCur;
        [SerializeField] private Text mTextNext;
        [SerializeField] private Color mSameColor;
        [SerializeField] private Color mDiffColor;

        public void OnInit(int index, string name, string cur, string next)
        {
            mImgBg.color = index % 2 == 0 ? mDoubleColor : mSingleColor;
            mTextName.SafeSetText(name);
            mTextCur.SafeSetText(cur);
            mTextNext.SafeSetText(next);
            mTextNext.color = string.Equals(cur, next) ? mSameColor : mDiffColor;
        }
    }
}
