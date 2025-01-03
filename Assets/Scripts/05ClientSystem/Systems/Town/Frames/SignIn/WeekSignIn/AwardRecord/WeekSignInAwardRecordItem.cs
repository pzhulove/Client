using System;
using System.Collections;
using System.Collections.Generic;
using Protocol;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;
using ProtoTable;

namespace GameClient
{

    public class WeekSignInAwardRecordItem : MonoBehaviour
    {
        private string _contentStr;

        [Space(10)]
        [HeaderAttribute("Content")]
        [Space(10)]
        [SerializeField] private LinkParse linkParseControl;


        private void OnDestroy()
        {
            _contentStr = null;
        }

        public void InitItem(string contentStr)
        {
            //内容，并展示
            _contentStr = contentStr;

            if (linkParseControl != null)
                linkParseControl.SetText(contentStr);
        }

    }
}
