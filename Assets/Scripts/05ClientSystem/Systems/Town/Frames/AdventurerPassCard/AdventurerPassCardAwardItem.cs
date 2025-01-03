using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;
using UnityEngine.Assertions;
using Protocol;
using ProtoTable;
using Network;

namespace GameClient
{
    // 冒险者通行证奖励item
    public class AdventurerPassCardAwardItem : MonoBehaviour
    {
        [SerializeField] Text lv = null;

        [SerializeField] Button btnGetAward = null;
        [SerializeField] Image getMarkNormal = null;
        [SerializeField] AdventurerPassCardPreviewItem mNormalAwardItem = null;
        [SerializeField] ComUIListScript highAwards = null;
        [SerializeField] Image getMarkHigh = null;
        [SerializeField] Text mTextRateCount = null;

        // const string copyNumPathForamt = "UI/Image/Packed/p_UI_ZhanLing.png:UI_Zhanling_Shuzi_";

        public void SetUp(object data, bool bShowMode = false)
        {
            if (data == null || !(data is AdventurerPassCardDataManager.AwardItemInfo))
            {
                return;
            }
            AdventurerPassCardDataManager.AwardItemInfo awardItemInfo = data as AdventurerPassCardDataManager.AwardItemInfo;
            lv.SafeSetText(TR.Value("adventurer_pass_card_lv", awardItemInfo.lv));
            btnGetAward.SafeSetOnClickListener(() =>
            {
                AdventurerPassCardDataManager.GetInstance().SendWorldAventurePassRewardReq((uint)awardItemInfo.lv);
            });
            //等级是否达标
            bool isLevelEnough = AdventurerPassCardDataManager.GetInstance().CardLv >= awardItemInfo.lv;
            //普通奖励是否领取
            bool isNormalAwardReceived = AdventurerPassCardDataManager.GetInstance().IsNormalAwardReceived(awardItemInfo.lv);
            //高级版是否解锁
            bool isUnlockHigh = AdventurerPassCardDataManager.GetInstance().GetPassCardType > AdventurerPassCardDataManager.PassCardType.Normal;
            //高级奖励是否领取
            bool isHighAwardReceived = AdventurerPassCardDataManager.GetInstance().IsHighAwardReceived(awardItemInfo.lv);

            if (!isLevelEnough)
            {
                lv.CustomActive(true);
                btnGetAward.CustomActive(false);
                getMarkNormal.CustomActive(false);
                getMarkHigh.CustomActive(false);
            }
            else
            {
                lv.CustomActive(isNormalAwardReceived);
                getMarkNormal.CustomActive(isNormalAwardReceived);
                // 这里还有个规则，如果普通通行证没有奖励，则不管有没有领取过都不显示领取勾勾
                if (awardItemInfo.normalAwards != null && awardItemInfo.normalAwards.Count == 0)
                {
                    getMarkNormal.CustomActive(false);
                }

                // 普通奖励没有领取 或者 高级通行证购买了且没有领取，则显示领取按钮
                btnGetAward.CustomActive(
                    !isNormalAwardReceived
                    || (isUnlockHigh && !isHighAwardReceived));
                getMarkHigh.CustomActive(isUnlockHigh && isHighAwardReceived);
            }

            // 大奖显示区显示规则有变化
            if (bShowMode)
            {
                lv.CustomActive(true);
                btnGetAward.CustomActive(false);
            }

            int count = 0;
            if (isUnlockHigh)
            {
                count = AdventurerPassCardDataManager.GetInstance().GetCanGetHighAwardItemCopies(awardItemInfo.lv);
            }
            mTextRateCount.CustomActive(count > 1);
            if (count > 1)
            {
                mTextRateCount.SafeSetText(string.Format("{0}倍", count));
                // if(mTextRateCount != null)
                // {
                //     Utility.SetImageIcon(copyNum.gameObject, string.Format("{0}{1}", copyNumPathForamt, count), true);
                // }
            }

            //普通奖励就1个
            if (awardItemInfo.normalAwards.Count > 0)
            {
                mNormalAwardItem.CustomActive(true);
                var itemInfo = awardItemInfo.normalAwards[0];
                ItemData itemData = ItemDataManager.CreateItemDataFromTable(itemInfo.itemID);
                if (itemData == null)
                {
                    Logger.LogErrorFormat("冒险通行证奖励找不到id={0}的道具", itemInfo.itemID);
                }
                else
                {
                    if (itemData.TableData != null && itemData.TableData.ExpireTime > 0)
                    {
                        itemData.DeadTimestamp = itemData.TableData.ExpireTime;
                    }
                    itemData.Count = itemInfo.itemNum;
                    mNormalAwardItem.OnInit(itemData, isLevelEnough && !isNormalAwardReceived, false);
                }
            }
            else
            {
                mNormalAwardItem.CustomActive(false);
            }
            UpdateAwardItemList(awardItemInfo.highAwards, isLevelEnough && isUnlockHigh && !isHighAwardReceived, isLevelEnough && !isUnlockHigh && !isHighAwardReceived);
        }

        [SerializeField] private GameObject mObjHighItemRoot;
        [SerializeField] private AdventurerPassCardPreviewItem mHighItem1;
        private AdventurerPassCardPreviewItem mHighItem2;
        void UpdateAwardItemList(List<AdventurerPassCardDataManager.ItemInfo> rewardItems, bool canGet, bool isLock)
        {
            if (rewardItems == null || rewardItems.Count <= 0)
            {
                return;
            }
            //第一个道具
            {
                ItemData itemData = ItemDataManager.CreateItemDataFromTable(rewardItems[0].itemID);
                if (itemData == null)
                {
                    return;
                }
                if (itemData.TableData != null && itemData.TableData.ExpireTime > 0)
                {
                    itemData.DeadTimestamp = itemData.TableData.ExpireTime;
                }
                itemData.Count = rewardItems[0].itemNum;
                mHighItem1.OnInit(itemData, canGet, isLock);
            }
            //第二个道具
            if (rewardItems.Count > 1)
            {
                ItemData itemData = ItemDataManager.CreateItemDataFromTable(rewardItems[1].itemID);
                if (itemData == null)
                {
                    return;
                }
                if (itemData.TableData != null && itemData.TableData.ExpireTime > 0)
                {
                    itemData.DeadTimestamp = itemData.TableData.ExpireTime;
                }
                itemData.Count = rewardItems[1].itemNum;
                if (null == mHighItem2)
                {
                    mHighItem2 = GameObject.Instantiate(mHighItem1);
                    Utility.AttachTo(mHighItem1.gameObject, mObjHighItemRoot);
                }
                mHighItem2.OnInit(itemData, canGet, isLock);
            }
            mHighItem2.CustomActive(rewardItems.Count > 1);

            // highAwards.Initialize();
            // highAwards.onItemVisiable = (item) =>
            // {
            //     if (item == null || item.m_index >= rewardItems.Count)
            //         return;
            //     AdventurerPassCardPreviewItem comItem = item.GetComponent<AdventurerPassCardPreviewItem>();
            //     if (comItem == null || rewardItems == null)
            //         return;
            //     ItemData itemData = ItemDataManager.CreateItemDataFromTable(rewardItems[item.m_index].itemID);
            //     if (itemData == null)
            //     {
            //         return;
            //     }
            //     if (itemData.TableData != null && itemData.TableData.ExpireTime > 0)
            //     {
            //         itemData.DeadTimestamp = itemData.TableData.ExpireTime;
            //     }
            //     itemData.Count = rewardItems[item.m_index].itemNum;
            //     comItem.OnInit(itemData, canGet, isLock);
            // };
            // highAwards.SetElementAmount(rewardItems.Count);
        }
    }
}
