using System;
using System.Collections;
using System.Reflection;
using System.ComponentModel;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EItemType = ProtoTable.ItemTable.eType;
using Network;
using Protocol;
using UnityEngine.EventSystems;
using ProtoTable;


namespace GameClient
{
    class CheckPointHelpFrame : ClientFrame
    {
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Common/CheckPointHelpFrame.prefab";
        }

        protected override void _OnOpenFrame()
        {
            _Initialize();
        }

        protected override void _OnCloseFrame()
        {
            _Clear();
        }

        protected void _Initialize()
        {
            string str = "";
            if (userData != null)
            {
                str = (string)userData;
            }

            ComHelp help = frame.GetComponent<ComHelp>();
            if(help != null)
            {
                if (help.textObj != null)
                {
                    Text text = help.textObj.GetComponent<Text>();
                    text.text = str.Replace("\\n", "\n");
                }
            }
        }

        protected void _Clear()
        {
            
        }

        #region ctrl callback
        
        [UIEventHandle("Back/Button")]
        void _OnCloeFrame()
        {
            frameMgr.CloseFrame(this);
        }
        
        #endregion
    }
}
