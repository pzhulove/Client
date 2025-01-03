using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class FairDuelEntranceFrame : ClientFrame
    {


        private Button mCloseBtn;

        private Button mHelpBtn;

        private Button mGoBtn;

        private PkWaitingRoomData mRoomData = new PkWaitingRoomData();

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/FairDuel/FairDuelEntranceFrame";
        }
        protected override void _OnOpenFrame()
        {
            if(userData!=null)
            {
                mRoomData = (PkWaitingRoomData)userData;
            }
          
        }

        protected override void _OnCloseFrame()
        {
            //if(mRoomData!=null)
            //{
            //    mRoomData.Clear();
            //}
        }

        #region ExtraUIBind
        protected override void _bindExUI()
        {
            mCloseBtn=  mBind.GetCom<Button>("Close");
            if(mCloseBtn!=null)
            {
                mCloseBtn.onClick.AddListener(OnCloseBtnClick);
            }

            mHelpBtn = mBind.GetCom<Button>("Help");
            if (mHelpBtn != null)
            {
                mHelpBtn.onClick.AddListener(OnHelpBtnClick);
            }

            mGoBtn = mBind.GetCom<Button>("Go");
            if (mGoBtn != null)
            {
                mGoBtn.onClick.AddListener(OnGoBtnClick);
            }

        }

        protected override void _unbindExUI()
        {
            if (mCloseBtn != null)
            {
                mCloseBtn.onClick.RemoveListener(OnCloseBtnClick);
                mCloseBtn = null;
            }

            mHelpBtn = mBind.GetCom<Button>("Help");
            if (mHelpBtn != null)
            {
                mHelpBtn.onClick.RemoveListener(OnHelpBtnClick);
                mHelpBtn = null;
            }

            mGoBtn = mBind.GetCom<Button>("Go");
            if (mGoBtn != null)
            {
                mGoBtn.onClick.RemoveListener(OnGoBtnClick);
                mGoBtn = null;
            }
        }
        #endregion

        #region 按钮事件
        /// <summary>
        ///去公平竞技场场景
        /// </summary>
        private void OnGoBtnClick()
        {
            ClientSystemTown systemTown = ClientSystemManager.GetInstance().GetCurrentSystem() as ClientSystemTown;
            if (systemTown == null)
            {
                Logger.LogError("Current System is not Town!!!");
                return;
            }
            if(mRoomData==null)
            {
                Logger.LogError("mRoomData is null");
                return;
            }
            GameFrameWork.instance.StartCoroutine(systemTown._NetSyncChangeScene(new SceneParams
            {
                currSceneID = mRoomData.CurrentSceneID,
                currDoorID = 0,
                targetSceneID = 6033,
                targetDoorID = 0,
            }, false));
            frameMgr.CloseFrame(this);
        }
        /// <summary>
        /// 打开规则详情界面
        /// </summary>
        private void OnHelpBtnClick()
        {
            ClientSystemManager.GetInstance().OpenFrame<FairDuelHelpFrame>(FrameLayer.Middle);
        }
        
        /// <summary>
        /// 关闭界面
        /// </summary>
        private void OnCloseBtnClick()
        {
            frameMgr.CloseFrame(this);
        }
        #endregion
        


    }
}
