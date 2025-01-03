using ProtoTable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace GameClient
{
    public class RoleAttrTipItem : MonoBehaviour
    {
        [SerializeField] private Text mTextName;
        [SerializeField] private Text mTextContent;
        AttrDescTable mTable;

        public void Init(string title, string content, bool isRich)
        {
            if (mTextName.supportRichText != isRich)
            {
                mTextName.supportRichText = isRich;
            }
            mTextName.SafeSetText(title);
            mTextContent.SafeSetText(content);
        }
    }
}
