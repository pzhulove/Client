using UnityEngine.UI;
using Scripts.UI;
using System.Collections.Generic;
using UnityEngine;
using EItemType = ProtoTable.ItemTable.eType;
using ProtoTable;
using Protocol;

namespace GameClient
{
    class EquipSubmitResultFrame : ClientFrame
    {
        const string submitItemPath = "UIFlatten/Prefabs/EquipRecovery/ResultItem";
        int scoreSum = 0;
        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/EquipRecovery/EquipSubmitResultFrame";
        }
        protected sealed override void _OnOpenFrame()
        {
            _InitUI();
        }
        protected sealed override void _OnCloseFrame()
        {

        }
        void _RegisterUIEvent()
        {
            //UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.EquipSubmitSuccess, _OnEquipSubmitSuccess);
        }

        void _UnRegisterUIEvent()
        {
            //UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.EquipSubmitSuccess, _OnEquipSubmitSuccess);
        }

        void _InitUI()
        {
            _InitRecord();
        }

        void _InitRecord()
        {
            List<EqRecScoreItem> rewardList = EquipRecoveryDataManager.GetInstance().submitResult;
            for(int i=0;i<rewardList.Count;i++)
            {
                _CreateSubmitItem(rewardList[i]);
            }
        }
        void _CreateSubmitItem(EqRecScoreItem scoreItem)
        {
            ItemData itemData = ItemDataManager.GetInstance().GetItem(scoreItem.uid);
            if(itemData == null)
            {
                return;
            }

            GameObject submitItem = AssetLoader.instance.LoadResAsGameObject(submitItemPath);
            if (submitItem == null)
            {
                return;
            }
            
            ItemData ItemDetailData = ItemDataManager.CreateItemDataFromTable(itemData.TableID);
            if (null == ItemDetailData)
            {
                return;
            }
            var mBind = submitItem.GetComponent<ComCommonBind>();
            if (mBind == null)
            {
                return;
            }
            var mIconRoot = mBind.GetGameObject("itemRoot");
            var mIconScore = mBind.GetCom<Text>("itemScore");
            ComItem comitem = mIconRoot.GetComponentInChildren<ComItem>();
            if (comitem == null)
            {
                comitem = CreateComItem(mIconRoot);
            }
            int result = itemData.TableID;
            comitem.Setup(ItemDetailData, (GameObject Obj, ItemData sitem) => { _OnShowTips(result); });
            scoreSum += (int)scoreItem.score;
            mIconScore.text = scoreItem.score.ToString();
            Utility.AttachTo(submitItem, mContentRoot);
        }

        void _OnShowTips(int result)
        {
            ItemData ItemDetailData = ItemDataManager.CreateItemDataFromTable(result);
            ItemTipManager.GetInstance().ShowTip(ItemDetailData);
        }

        #region ExtraUIBind
        private GameObject mContentRoot = null;
        private Button mClose = null;

        protected override void _bindExUI()
        {
            mContentRoot = mBind.GetGameObject("ContentRoot");
            mClose = mBind.GetCom<Button>("Close");
            mClose.onClick.AddListener(_onCloseButtonClick);
        }

        protected override void _unbindExUI()
        {
            mContentRoot = null;
            mClose.onClick.RemoveListener(_onCloseButtonClick);
            mClose = null;
        }
        #endregion

        #region Callback
        private void _onCloseButtonClick()
        {
            /* put your code in here */
            frameMgr.CloseFrame(this);
        }
        #endregion
    }
}