using UnityEngine;
//using System.Collections.Generic;
using System.Text;
using UnityEngine.UI;
using UnityEngine.Events;
using Protocol;
using Network;
using System.Diagnostics;
using System;
using DG.Tweening;
using System.Collections;


namespace GameClient
{
    [LoggerModel("Chapter")]
    public class DungeonTipInfoFrame : ClientFrame
    {
        private GameObject mRoot;

        public sealed override string GetPrefabPath()
        {
            return "UI/Prefabs/Battle/DungeonTipUnitInfo";
        }

        protected sealed override void _OnLoadPrefabFinish()
        {

        }

        protected sealed override void _OnOpenFrame()
        {
            _updateRoot();
        }

        protected sealed override void _OnCloseFrame()
        {
            mRoot = null;
        }

        private new void _updateRoot()
        {
            if (null == mRoot)
            {
                mRoot = Utility.FindGameObject(frame, "Root", true);
            }
        }
    }
}
