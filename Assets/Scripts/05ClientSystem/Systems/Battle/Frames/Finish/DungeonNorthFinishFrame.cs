using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine.UI;
using Protocol;
using Network;
using System.Diagnostics;
using DG.Tweening;
using System;

namespace GameClient
{
    public class DungeonNorthFinishFrame : ClientFrame, IDungeonFinish
    {
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Battle/Finish/DungeonNorthFinish";
        }

#region ExtraUIBind
        private GameObject mGotListRoot = null;
        private GameObject mGotListRoot2 = null;
        private ComItemList mComItems = null;
        private ComItemList mComItems2 = null;
        private Button mBack = null;
        private ComExpBar mExpBar = null;
        private Text mExpValue = null;
        private Text mName = null;

        protected override void _bindExUI()
        {
            mGotListRoot = mBind.GetGameObject("GotListRoot");
            mGotListRoot2 = mBind.GetGameObject("GotListRoot2");
            mComItems = mBind.GetCom<ComItemList>("ComItems");
            mComItems2 = mBind.GetCom<ComItemList>("ComItems2");
            mBack = mBind.GetCom<Button>("Back");
            mBack.onClick.AddListener(_onBackButtonClick);
            mExpBar = mBind.GetCom<ComExpBar>("ExpBar");
            mExpValue = mBind.GetCom<Text>("ExpValue");
            mName = mBind.GetCom<Text>("Name");
        }

        protected override void _unbindExUI()
        {
            mGotListRoot = null;
            mGotListRoot2 = null;
            mComItems = null;
            mComItems2 = null;
            mBack.onClick.RemoveListener(_onBackButtonClick);
            mBack = null;
            mExpBar = null;
            mExpValue = null;
            mName = null;
        }
#endregion   



#region Callback
        private void _onBackButtonClick()
        {
            /* put your code in here */
            _onClose();
        }
#endregion

        private void _onExpUpdate(UIEvent ui)
        {
            _setExp(PlayerBaseData.GetInstance().Exp, false);
        }

        protected override void _OnOpenFrame()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ExpChanged, _onExpUpdate);
            _setExp(BattleDataManager.GetInstance().originExp, true);

        }

        protected override void _OnCloseFrame()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ExpChanged, _onExpUpdate);
        }

        private void _setExp(ulong val, bool force)
        {
            if (null != mExpBar)
            {
                mExpBar.SetExp(val, force, exp =>
                {
                    return TableManager.instance.GetCurRoleExp(exp);
                });
            }
        }

        public void SetName(string name)
        {
            mName.text = name;
        }

        public void SetBestTime(int time)
        {
        }

        public void SetCurrentTime(int time)
        {
        }

        public void SetDiff(int diff)
        {
        }

        public void SetDrops(ComItemList.Items[] items)
        {
            List<ComItemList.Items> allItems = new List<ComItemList.Items>(items);

            mComItems.SetItems(allItems.ToArray());

            if (ChapterNorthFrame.sMuti > 1)
            {
                for (int i = 0; i < allItems.Count; ++i)
                {
                    allItems[i].count *= (uint)(ChapterNorthFrame.sMuti - 1);
                }

                mComItems2.SetItems(allItems.ToArray());
                mGotListRoot2.SetActive (true);
            }
        }

        public void SetExp(ulong exp)
        {
            mExpValue.text = exp.ToString();
            _onExpUpdate(null);
        }

        public void SetFinish(bool isFinish)
        {
            //mGotRoot.SetActive(isFinish);
            //mGotListRoot.SetActive(isFinish);
            //mGotRoot2.SetActive(isFinish && ChapterNorthFrame.sMuti > 1);
            //mGotListRoot2.SetActive(isFinish && ChapterNorthFrame.sMuti > 1);
            //mGrayCom.enabled = !isFinish;

            //if (null != mResulte)
            //{
            //    mResulte.sprite = isFinish ? mBind.GetSprite("success") : mBind.GetSprite("fail");
            //}
        }

        public void SetLevel(int lvl)
        {

        }

        private void _onClose()
        {
            ClientSystemManager.instance.CloseFrame(this);
            ClientSystemManager.instance.SwitchSystem<ClientSystemTown>();
        }
    }
}
