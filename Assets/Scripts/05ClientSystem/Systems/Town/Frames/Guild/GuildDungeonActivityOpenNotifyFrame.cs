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
    public class GuildDungeonActivityOpenNotifyFrame : ClientFrame
    { 
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Guild/GuildDungeonActivityOpenNotify";
        }

        float fTimeElasped = 0.0f;

        protected override void _OnOpenFrame()
        {
            fTimeElasped = 0.0f;            
        }

        protected override void _OnCloseFrame()
        {
            fTimeElasped = 0.0f;

            ClientSystemManager.GetInstance().OpenFrame<JoinGuildDungeonActivityFrame>();
        }

        public override bool IsNeedUpdate()
        {
            return false;
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
