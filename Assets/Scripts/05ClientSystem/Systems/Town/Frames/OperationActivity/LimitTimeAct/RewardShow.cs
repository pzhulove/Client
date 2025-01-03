using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using ProtoTable;
using Protocol;
using Network;
using System.Collections;

namespace GameClient
{
    
    class RewardShow : ClientFrame
    {
        int tableID = -1;
        List<int> rewardID = new List<int>();
        List<int> importantRewardID = new List<int>();
        List<int> rewardNum = new List<int>();
        List<int> importantRewardNum = new List<int>();
        const string rewardIconRewardPath = "UIFlatten/Prefabs/OperateActivity/LimitTime/ActivityLimitTimelAwardIcon";
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/OperateActivity/LimitTime/RewardShow";
        }
        protected override void _OnOpenFrame()
        {
            if (userData == null)
            {
                Logger.LogErrorFormat("需要传入一个奖品链表");
                return;
            }
            else
            {
                tableID = (int)userData;
                InitData();
            }
        }
        private void InitData()
        {
            rewardID.Clear();
            importantRewardID.Clear();
            rewardNum.Clear();
            importantRewardNum.Clear();
            InitIcon();
        }

        private void InitIcon()
        {
            var rewardPoolTableData = TableManager.GetInstance().GetTable<RewardPoolTable>();
            var enumerator = rewardPoolTableData.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var rewardPooldata = enumerator.Current.Value as RewardPoolTable;
                if (rewardPooldata.DrawPrizeTableID == tableID && rewardPooldata.IsImportant != 1)
                {
                    rewardID.Add(rewardPooldata.ItemID);
                    rewardNum.Add(rewardPooldata.ItemNum);
                }
                else if (rewardPooldata.DrawPrizeTableID == tableID && rewardPooldata.IsImportant == 1)
                {
                    importantRewardID.Add(rewardPooldata.ItemID);
                    importantRewardNum.Add(rewardPooldata.ItemNum);
                }
            }
            if (rewardID.Count == 0 && importantRewardID.Count == 0)
            {
                Logger.LogErrorFormat("id为{0}的奖池表为空", tableID);
            }
            else
            {
                mItemRoot.CustomActive(false);
                mImportantItemRoot.CustomActive(false);
                for (int i = 0; i < rewardID.Count; i++)
                {
                    mItemRoot.CustomActive(true);
                    ItemData ItemDetailData = ItemDataManager.CreateItemDataFromTable(rewardID[i]);
                    if (null == ItemDetailData)
                    {
                        continue;
                    }
                    GameObject rewardIconGo = AssetLoader.instance.LoadResAsGameObject(rewardIconRewardPath);
                    if (rewardIconGo == null)
                    {
                        return;
                    }
                    else
                    {
                        Utility.AttachTo(rewardIconGo, mItemRoot);
                    }
                    ItemDetailData.Count = rewardNum[i];
                    ComItem comitem = CreateComItem(rewardIconGo);
                    int result = (int)rewardID[i];
                    comitem.Setup(ItemDetailData, (GameObject Obj, ItemData sitem) => { OnShowTips(result); });
                }
                for (int i = 0; i < importantRewardID.Count; i++)
                {
                    mImportantItemRoot.CustomActive(true);
                    ItemData ItemDetailData = ItemDataManager.CreateItemDataFromTable(importantRewardID[i]);
                    if (null == ItemDetailData)
                    {
                        continue;
                    }
                    GameObject rewardIconGo = AssetLoader.instance.LoadResAsGameObject(rewardIconRewardPath);
                    if (rewardIconGo == null)
                    {
                        return;
                    }
                    else
                    {
                        Utility.AttachTo(rewardIconGo, mImportantItemRoot);
                    }
                    ItemDetailData.Count = importantRewardNum[i];
                    ComItem comitem = CreateComItem(rewardIconGo);
                    int result = (int)importantRewardID[i];
                    comitem.Setup(ItemDetailData, (GameObject Obj, ItemData sitem) => { OnShowTips(result); });
                }
            }
            
            StartCoroutine(DonateResult());
        }

        IEnumerator DonateResult()
        {
            yield return null;
            mScrollView.verticalNormalizedPosition = 1;
        }
        /// <summary>
        /// 显示tips
        /// </summary>
        /// <param name="result"></param>
        void OnShowTips(int result)
        {
            ItemData ItemDetailData = ItemDataManager.CreateItemDataFromTable(result);
            ItemTipManager.GetInstance().ShowTip(ItemDetailData);
        }

        protected override void _OnCloseFrame()
        {
            ClearData();
        }

        void ClearData()
        {
            tableID = -1;
            rewardID.Clear();
            importantRewardID.Clear();
            rewardNum.Clear();
            importantRewardNum.Clear();
            StopCoroutine(DonateResult());
        }
        #region ExtraUIBind
        private ScrollRect mScrollView = null;
        private GameObject mImportantItemRoot = null;
        private GameObject mItemRoot = null;

        protected override void _bindExUI()
        {
            mScrollView = mBind.GetCom<ScrollRect>("ScrollView");
            mImportantItemRoot = mBind.GetGameObject("importantItemRoot");
            mItemRoot = mBind.GetGameObject("itemRoot");
        }

        protected override void _unbindExUI()
        {
            mScrollView = null;
            mImportantItemRoot = null;
            mItemRoot = null;
        }
        #endregion
    }
}