using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

namespace GameClient
{
    sealed class ComicFrame : ClientFrame
    {
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Common/ComicFrame";
        }

        
        public override bool NeedMutex()
        {
            return false;
        }
        void ShowMainFrameAndInput(bool bShow)
        {   
            InputManager.instance.SetVisible(bShow);
        }
        protected override void _OnOpenFrame()
        {
            ShowMainFrameAndInput(false);
            
            
        }
        protected override void _OnCloseFrame()
        {          
        }
    }
}
