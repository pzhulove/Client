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
    public class BossMissionCompletePromptFrame : ClientFrame
    {
        #region val

        int iDungeonID = 0;
        #endregion

        #region ui bind
        private Image mBackground = null;
        private Image mCharactorIcon = null;
        private TextEx mTextName = null;

        #endregion

        #region override
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Mission/CompletePromptFrame";
        }

        protected override void _OnOpenFrame()
        {
            InvokeMethod.Invoke(this, 3.0f, () =>
            {
                frameMgr.CloseFrame(this);
            });

            iDungeonID = BattleMain.instance.GetDungeonManager().GetDungeonDataManager().id.dungeonID;
            UpdateWnd();           

            BindUIEvent();          
        }

        protected override void _OnCloseFrame()
        {
            InvokeMethod.RemoveInvokeCall(this);

            UnBindUIEvent();

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.BossMissionCompleteFrameClose);
        }

        protected override void _bindExUI()
        {
            mBackground = mBind.GetCom<Image>("Background");
            mCharactorIcon = mBind.GetCom<Image>("CharactorIcon");
            mTextName = mBind.GetCom<TextEx>("TextName");
        }

        protected override void _unbindExUI()
        {
            mBackground = null;
            mCharactorIcon = null;
            mTextName = null;
        }

        #endregion 

        #region method

        void BindUIEvent()
        {
            //UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.RefreshVerifyPwdErrorCount,_OnRefreshVerifyPwdErrorCount);
        }

        void UnBindUIEvent()
        {
            //UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.RefreshVerifyPwdErrorCount,_OnRefreshVerifyPwdErrorCount);
        }

        public void SetBackgroud(string spritePath)
        {       
            if(mBackground != null)
            {
                ETCImageLoader.LoadSprite(ref mBackground, spritePath);
            }            
        }

        public void SetCharactorSprite(string spritePath)
        {   
            if(mCharactorIcon == null)
            {
                return;
            }

            if (null == spritePath || spritePath.Length <= 0)
            {
                mCharactorIcon.sprite = null;
            }
            else
            {
                ETCImageLoader.LoadSprite(ref mCharactorIcon, spritePath);
            }

            RectTransform rect = mCharactorIcon.rectTransform;

            float originHight = rect.sizeDelta.y;

            mCharactorIcon.SetNativeSize();
            mCharactorIcon.CustomActive(null != mCharactorIcon.sprite);

            float newHight = rect.sizeDelta.y;

            rect.sizeDelta = rect.sizeDelta * (originHight / newHight);
            rect.localScale = Vector3.one;
        }

        private void _SetName(string name)
        {
            mTextName.SafeSetText(name);
        }

        private T _getDungeonTable<T>(int id)
        {
            var dungeonItem = TableManager.instance.GetTableItem<T>(id);
            if (dungeonItem != null)
            {
                return dungeonItem;
            }

            return default(T);
        }

        void UpdateWnd()
        {
            DungeonTable mDungeonTable = _getDungeonTable<DungeonTable>(BattleMain.instance.GetDungeonManager().GetDungeonDataManager().id.dungeonID);
            if(mDungeonTable != null)
            {
                SetBackgroud(mDungeonTable.TumbPath);
                SetCharactorSprite(mDungeonTable.TumbChPath);
                _SetName(mDungeonTable.Name);
            }            
        }

        #endregion

        #region ui event





        #endregion
    }
}
