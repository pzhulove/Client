using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;
using UnityEngine.Assertions;
using EJarType = ProtoTable.JarBonus.eType;
using Protocol;
using ProtoTable;

namespace GameClient
{

    class JarAwardsDetailFrame : ClientFrame
    {
        private ComUIListScript mComUIList = null;
        private List<ItemData> mAwards = null;

        #region ClientFrame
        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Jar/JarAwardsDetailFrame";
        }

        protected sealed override void _OnOpenFrame()
        {
            _RegisterUIEvent();
            BindUIEvent();

            if (userData != null && userData is List<ItemData>)
            {
                mAwards = userData as List<ItemData>;
                _InitWnd();
            }
        }

        private void _InitWnd()
        {
            if (mComUIList != null && mAwards != null)
            {
                mComUIList.SetElementAmount(mAwards.Count);
            }
        }

        private void _OnItemUpdate(ComUIListElementScript item)
        {
            if (item == null)
            {
                return;
            }

            if (mAwards == null)
            {
                return;
            }

            if (mAwards.Count <= item.m_index || mAwards[item.m_index] == null)
            {
                return;
            }

            var script = item.GetComponentInChildren<ComItemNew>();
            if (script != null)
            {
                script.Setup(mAwards[item.m_index], null, true);
                if (mAwards[item.m_index].Count > 0)
                {
                    script.SetCount(mAwards[item.m_index].Count.ToString());
                }
            }
        }

        protected sealed override void _OnCloseFrame()
        {
            UnBindUIEvent();
            _UnRegisterUIEvent();

        }
        
        protected override void _bindExUI()
        {
            mComUIList = mBind.GetCom<ComUIListScript>("ComUIList");
            if (mComUIList != null)
            {
                mComUIList.Initialize();
                mComUIList.onItemVisiable = _OnItemUpdate;
                mComUIList.OnItemUpdate = _OnItemUpdate;
            }
        }

        protected override void _unbindExUI()
        {
            mComUIList = null;

            mAwards = null;
        }
        #endregion


        

        #region UIEvent
        //UI系统之间事件
        protected void _RegisterUIEvent()
        {
        }

        protected void _UnRegisterUIEvent()
        {
        }

        //UI控件之间事件的绑定，应该分离到View
        void BindUIEvent()
        {

        }

        void UnBindUIEvent()
        {
        }

        public sealed override bool IsNeedUpdate()
        {
            return false;
        }

        protected sealed override void _OnUpdate(float timeElapsed)
        {
        }

        #endregion

        //[UIEventHandle("BG/Title/Close")]
        void _OnCloseClicked()
        {
            frameMgr.CloseFrame(this);
        }
    }
}
