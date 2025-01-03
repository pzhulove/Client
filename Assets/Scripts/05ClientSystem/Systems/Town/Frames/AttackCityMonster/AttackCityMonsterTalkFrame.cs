using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class AttackCityMonsterTalkFrame : ClientFrame
    {
        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/AttackCityMonster/AttackCityMonsterTalkFrame";
        }

        protected override void _OnOpenFrame()
        {
            base._OnOpenFrame();

            if (mAttackCityMonsterTalkView != null)
            {
                var npgGuid = (UInt64) userData;
                mAttackCityMonsterTalkView.InitData(npgGuid);
            }
        }

        #region ExtraUIBind
        private AttackCityMonsterTalkView mAttackCityMonsterTalkView = null;

        protected override void _bindExUI()
        {
            mAttackCityMonsterTalkView = mBind.GetCom<AttackCityMonsterTalkView>("AttackCityMonsterTalkView");
        }

        protected override void _unbindExUI()
        {
            mAttackCityMonsterTalkView = null;
        }
        #endregion
    }
}