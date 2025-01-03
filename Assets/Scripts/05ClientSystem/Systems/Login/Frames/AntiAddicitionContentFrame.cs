using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using DG.Tweening;
using Protocol;
using Network;
using System.Net;
using System.IO;
using System.Text;
using XUPorterJSON;
using Scripts.UI;

namespace GameClient
{
    public class AntiAddicitionContentFrame : ClientFrame
    {
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Login/Publish/AntiAddicition";
        }

#region ExtraUIBind
        private Button mClose = null;


        protected override void _bindExUI()
        {
            mClose = mBind.GetCom<Button>("close");
            mClose.onClick.AddListener(_onCloseButtonClick);
        }

        protected override void _unbindExUI()
        {
            mClose.onClick.RemoveListener(_onCloseButtonClick);
            mClose = null;
        }
#endregion    

#region Callback
        private void _onCloseButtonClick()
        {
            /* put your code in here */

            ClientSystemManager.instance.CloseFrame(this);
        }

        private void _stop()
        {
          
        }

        protected override void _OnOpenFrame()
        {
        }

        protected override void _OnCloseFrame()
        {
            _stop();
        }
#endregion 
       
    }
}
