using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;
using UnityEngine.Assertions;
using Protocol;
using ProtoTable;
using System.Collections;
using DG.Tweening;

namespace GameClient
{
    public class Pk3v3CrossOpenNotifyFrame : ClientFrame
    { 
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Pk3v3Cross/Pk3v3CrossOpenNotify";
        }

        float fTimeElasped = 0.0f;

        protected override void _OnOpenFrame()
        {
            fTimeElasped = 0.0f;            
        }

        protected override void _OnCloseFrame()
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
            if(fTimeElasped >= 3.0f)
            {
                frameMgr.CloseFrame(this);
                fTimeElasped = 0.0f;
            }
        }
    }
}
