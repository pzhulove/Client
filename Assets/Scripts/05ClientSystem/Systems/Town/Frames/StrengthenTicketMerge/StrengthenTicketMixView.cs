using System;
using System.Collections;
using System.Collections.Generic;
using Scripts.UI;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class StrengthenTicketMixView : MonoBehaviour
    {
        [SerializeField] private ComUIListScript mDisplayItemList;
        [SerializeField] private Text mTextRateTip;
        //中间展示的道具id
        [SerializeField] private int mShowItemId = 200020301;
        //中间展示的道具
        [SerializeField] private ComItemNew mShowItem;

        //必要的材料id
        [SerializeField] private int mMustNeedItemId;
        //必要的材料
        [SerializeField] private ComItemNew mMustNeedItem;
        [SerializeField] private Text mMustNeedItemCount;
        [SerializeField] private SetComButtonCD mBtnCD;

        //选中的俩个道具下标
        private List<int> mSelectItemIndexList = new List<int>();
        //需要的俩个材料
        [SerializeField] private List<StrengthenTicketMixNeedItem> mNeedItems;
        //第一个选中的道具
        private int mSelectItemId1 {
            get 
            {
                if (mSelectItemIndexList.Count > 0 && mOwnerItemList.Count > mSelectItemIndexList[0])
                {
                    return mOwnerItemList[mSelectItemIndexList[0]].ticketItemData.TableID;
                }
                return 0;
            }
        }
        //第二个选中的道具
        private int mSelectItemId2 {
            get 
            {
                if (mSelectItemIndexList.Count > 1 && mOwnerItemList.Count > mSelectItemIndexList[1])
                {
                    return mOwnerItemList[mSelectItemIndexList[1]].ticketItemData.TableID;
                }
                return 0;
            }
        }

        //背包中的强化券道具
        List<StrengthenTicketFuseItemData> mOwnerItemList = new List<StrengthenTicketFuseItemData>();
        private Action mOnClickEvent;
        private UnityEngine.Coroutine waitToReqFuseTicket = null;

        public void OnInit(Action onclickEvent)
        {
            mOnClickEvent = onclickEvent;
            _OnInitItem();
            _OnInitList();
            OnUpdateList();
            OnUpdateMustNeedItemCount();
        }

        //初始化道具信息
        private void _OnInitItem()
        {
            var itemData = ItemDataManager.CreateItemDataFromTable(mShowItemId);
            mShowItem.Setup(itemData, null, true);
            var itemData2 = ItemDataManager.CreateItemDataFromTable(mMustNeedItemId);
            mMustNeedItem.Setup(itemData2, null, true);
        }

        //初始化列表
        private void _OnInitList()
        {
            mDisplayItemList.Initialize();
            mDisplayItemList.onItemVisiable = _OnItemShow;
            mDisplayItemList.OnItemUpdate = _OnItemShow;
            mDisplayItemList.onItemSelected = _OnItemSelect;
        }
        private void _OnItemSelect(ComUIListElementScript item)
        {
            if (null == item || item.m_index > mOwnerItemList.Count)
                return;
            if (mSelectItemIndexList.Count >= 2)
            {
                //已经选好了俩个材料了
                return;
            }
            var script = item.GetComponent<StrengthenTicketMixItem>();
            if (null == script)
                return;
            if (!script.IsCountEnough())
            {
                //道具数量不足
                return;
            }
            mSelectItemIndexList.Add(item.m_index);
            OnUpdateList();
        }
        private void _OnItemShow(ComUIListElementScript item)
        {
            if (null == item || item.m_index > mOwnerItemList.Count)
                return;
            var script = item.GetComponent<StrengthenTicketMixItem>();
            if (null == script)
                return;
            script.OnInit(mOwnerItemList[item.m_index], mSelectItemId1, mSelectItemId2);
        }

        //重置界面 取消选中
        public void OnViewClear()
        {
            mSelectItemIndexList.Clear();
            OnUpdateList();
        }

        //更新道具列表
        private void OnUpdateList()
        {
            mOwnerItemList = StrengthenTicketMergeDataManager.GetInstance().GetStrengthenTicketFuseItemDatas();
            mDisplayItemList.UpdateElementAmount(mOwnerItemList.Count);
            //道具材料必然是更随道具列表一起更新
            _OnUpdateNeedItem();
        }

        //更新选中的材料
        private void _OnUpdateNeedItem()
        {
            mNeedItems[0].OnInit(mSelectItemId1, _OnClickCancelSelect);
            mNeedItems[1].OnInit(mSelectItemId2, _OnClickCancelSelect);
            _OnUpdateView();
        }

        //更新界面文本
        private void _OnUpdateView()
        {
            if (mSelectItemIndexList.Count >= 2)
            {
                var model = StrengthenTicketMergeDataManager.GetInstance().CalculateMixRes(mOwnerItemList[mSelectItemIndexList[0]], mOwnerItemList[mSelectItemIndexList[1]]);
                mTextRateTip.SafeSetText(TR.Value("strengthen_merge_ticket_preview_tip", model.predictMinLevel, model.perdictMinPercent, model.predictMaxLevel, model.perdictMaxPercent));
            }
            else
            {
                mTextRateTip.SafeSetText(TR.Value("strengthen_merge_select_left_tickets_fuse"));
            }
        }
        //取消选中材料
        private void _OnClickCancelSelect(int itemId)
        {
            if (itemId == mSelectItemId1)
                mSelectItemIndexList.Remove(mSelectItemIndexList[0]);
            else if (itemId == mSelectItemId2)
                mSelectItemIndexList.Remove(mSelectItemIndexList[1]);
            OnUpdateList();
        }

        //更新必要材料的数量
        public void OnUpdateMustNeedItemCount()
        {
            int ownerCount = ItemDataManager.GetInstance().GetOwnedItemCount(mMustNeedItemId, false);
            if (ownerCount > 0)
            {
                mMustNeedItemCount.SafeSetText(TR.Value("strengthen_merge_mix_need_item_enough_count", ownerCount, 1));
            }
            else
            {
                mMustNeedItemCount.SafeSetText(TR.Value("strengthen_merge_mix_need_item_unenough_count", ownerCount, 1));
            }
        }

        public void OnClickMix()
        {
            if (mBtnCD == null || mBtnCD.IsBtnWork() == false)
            {
                return;
            }
            bool isCanMix = mSelectItemIndexList.Count >= 2 && ItemDataManager.GetInstance().GetOwnedItemCount(mMustNeedItemId, false) > 0;
            if (!isCanMix)
            {
                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("strengthen_merge_material_not_enough"));
                return;
            }
            bool enabled = StrengthenTicketMergeDataManager.GetInstance().checkEnableMix(mOwnerItemList[mSelectItemIndexList[0]], mOwnerItemList[mSelectItemIndexList[1]],
                    _StartReqFuseTickets,
                    () =>
                    {
                        mBtnCD.StopBtCD();
                    });
            if (enabled == false)
            {
                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("strengthen_merge_material_not_enough"));
            }
            mBtnCD.StartBtCD();
        }

        void _StartReqFuseTickets(ulong aGUID, ulong bGUID)
        {
            if (waitToReqFuseTicket != null)
            {
                GameFrameWork.instance.StopCoroutine(waitToReqFuseTicket);
            }
            waitToReqFuseTicket = GameFrameWork.instance.StartCoroutine(_WaitToReqFuseTickets(aGUID, bGUID));
        }

        IEnumerator _WaitToReqFuseTickets(ulong aGUID, ulong bGUID)
        {
            if (null != mOnClickEvent)
                mOnClickEvent();
            //播完一次动画才请求
            float animDuration = _PlayToProcessStageAnims();
            yield return Yielders.GetWaitForSeconds(animDuration);
            StrengthenTicketMergeDataManager.GetInstance().ReqFuseStrengthenTicket(aGUID, bGUID);
        }
        float _PlayToProcessStageAnims()
        {
            return 1.0f;
        }
    }
}
