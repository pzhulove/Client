using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Scripts.UI;
using System;

namespace GameClient
{
    class CommonClientFrame : ClientFrame
    {
        [UIEventHandle("C/Close")]
        protected void OnClose()
        {
            Close();
        }

        protected override void _OnOpenFrame()
        {
            
        }

        protected override void _OnCloseFrame()
        {
            
        }

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Login/Publish/UplevelShow";
        }
    }
}
