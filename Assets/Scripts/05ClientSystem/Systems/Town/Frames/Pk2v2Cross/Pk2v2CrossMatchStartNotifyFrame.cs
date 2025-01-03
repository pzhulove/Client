using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;
using UnityEngine.Assertions;
using Protocol;
using ProtoTable;
using System.Collections;
using DG.Tweening;
using Network;

namespace GameClient
{
    // Pk2v2CrossMatchStartNotify
    public class Pk2v2CrossMatchStartNotifyFrame : GameFrame
    {
        #region inner define

        #endregion

        #region val
        float fTimeElasped = 0.0f;
        #endregion

        #region ui bind
        #endregion

        #region override

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Pk2v2Cross/Pk2v2CrossMatchStartNotify";
        }        

        protected override void OnOpenFrame()
        {
            fTimeElasped = 0.0f;
        }

        protected override void OnCloseFrame()
        {
            fTimeElasped = 0.0f;
        }

        public override bool IsNeedUpdate()
        {
            return true;
        }

        protected override void _OnUpdate(float timeElapsed)
        {
            fTimeElasped += timeElapsed;
            if (fTimeElasped >= 3.0f)
            {
                frameMgr.CloseFrame(this);
                fTimeElasped = 0.0f;
            }
        }

        #endregion

        #region method
     
        #endregion

        #region ui event    

        #endregion
    }
}
