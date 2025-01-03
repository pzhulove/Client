using Network;
using Protocol;
using Scripts.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
  


    public class SendGiftFrame : ClientFrame
    {
        private ComUIListScript mItemUIList;
        private Button mSendBtn;
        private Button mCloseBtn;
        private UIGray mUIGray;
        private Text mRemainGiftNumTxt;
        private Text mBeSendTotalNumTxt;
        private CanvasGroup mNoTipCanvas;
        private ItemData mItemData;
        private List<FriendPresentInfo> mDataList=new List<FriendPresentInfo> ();
        private FriendPresentInfo mSelectData;

        private int mBeSengTotalLimitNum = 0;

        #region UI Base
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/OperateActivity/LimitTime/SendGiftFrame";
        }


        protected override void _OnOpenFrame()
        {
            base._OnOpenFrame();
            mItemData = (ItemData)userData;
            if (mItemUIList != null && !mItemUIList.IsInitialised())
            {
                mItemUIList.Initialize();
                mItemUIList.onItemVisiable += _OnItemVisiable;
                mItemUIList.OnItemUpdate += _OnItemVisiable;
                mItemUIList.OnItemRecycle += _OnItemRecycle;
            }
            mSendBtn.CustomActive(false);
            mNoTipCanvas.alpha = 0f;
            //红包活动相关
            NetProcess.AddMsgHandler(WorldGetItemFriendPresentInfosRes.MsgID, _OnFriendPresentInfosRet);
            NetProcess.AddMsgHandler(WorldItemFriendPresentRes.MsgID, _OnSendFriendPresentRet);
            //剩余礼物数量
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ItemNotifyRemoved, _OnUpdateLeftGiftNum);
            if (mItemData != null)
            {
                _ShowLeftGiftNum();
                _RequestFriendPresentInfosReq(mItemData.TableID);
            }
        }

       
        protected override void _OnCloseFrame()
        {
            base._OnCloseFrame();
            mSelectData = null;
            mItemData = null;
            mDataList.Clear();

            //红包活动相关
            NetProcess.RemoveMsgHandler(WorldGetItemFriendPresentInfosRes.MsgID, _OnFriendPresentInfosRet);
            NetProcess.RemoveMsgHandler(WorldItemFriendPresentRes.MsgID, _OnSendFriendPresentRet);
            //剩余礼物数量
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ItemNotifyRemoved, _OnUpdateLeftGiftNum);

        }

        protected override void _bindExUI()
        {
            mItemUIList = mBind.GetCom<ComUIListScript>("ItemList");

            mSendBtn = mBind.GetCom<Button>("SendBtn");
            mSendBtn.SafeAddOnClickListener(_OnSendBtnClick);

            mCloseBtn = mBind.GetCom<Button>("btClose");
            mCloseBtn.SafeAddOnClickListener(_OnCloseBtnClick);

            mUIGray = mBind.GetCom<UIGray>("SendBtnGray");

            mRemainGiftNumTxt = mBind.GetCom<Text>("RemianGiftCountTxt");
            mNoTipCanvas = mBind.GetCom<CanvasGroup>("NoTip");

            mBeSendTotalNumTxt = mBind.GetCom<Text>("BeSendTotalNumTxt");
        }

       

        protected override void _unbindExUI()
        {
            mItemUIList = null;
            mSendBtn.SafeRemoveOnClickListener(_OnSendBtnClick);
            mSendBtn = null;

            mCloseBtn.SafeRemoveOnClickListener(_OnCloseBtnClick);
            mCloseBtn = null;

            mUIGray = null;

            mRemainGiftNumTxt = null;

            mNoTipCanvas = null;

            mBeSendTotalNumTxt = null;
        }
        #endregion


        private void _OnItemVisiable(ComUIListElementScript item)
        {
           
            if(item!=null&&item.m_index>=0&item.m_index< mDataList.Count)
            {
                SendGiftItem sendGift= item.GetComponent<SendGiftItem>();
                if(sendGift!=null)
                {
                    sendGift.SendData(mDataList[item.m_index],_OnSelect, mSelectData);
                }
            }
        }
        private void _OnItemRecycle(ComUIListElementScript item)
        {
            if (item != null && item.m_index >= 0 & item.m_index < mDataList.Count)
            {
                SendGiftItem sendGift = item.GetComponent<SendGiftItem>();
                if (sendGift != null)
                {
                    sendGift.OnRecycle();
                }
            }
        }

        private void _OnSelect(FriendPresentInfo data)
        {
            mSelectData = data;
            
        }

        private void _OnSendBtnClick()
        {
            if(mItemData==null)
            {
                Logger.LogError("mItemData is null");
                return;
            }
            int count = ItemDataManager.GetInstance().GetItemCountInPackage(mItemData.TableID);
            if (mSelectData == null)
            {
                SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("NoSelected_Tip"));
            }else if(count<=0)
            {
                SystemNotifyManager.SystemNotify(2400004);
            }
            else
            {
                if(mSelectData!=null)
                {
                    _SendFriendPresentReq(mItemData.GUID,mSelectData.friendId, (UInt32)mItemData.TableID);
                }
                else
                {
                    Logger.LogError(" mSelectData is null");
                }
                
            }
        }
        private void _OnUpdateLeftGiftNum(UIEvent uiEvent)
        {
            if (uiEvent == null || uiEvent.Param1 == null)
                return;
            if(mItemData==null)
            {
                Logger.LogError("mItemData is null");
                return;
            }
            uint itemID = (uint)uiEvent.Param1;
            if((int)itemID == mItemData.TableID)
            {
                _ShowLeftGiftNum();
            }
        }
        /// <summary>
        /// 显示剩余礼物数量
        /// </summary>
        private void _ShowLeftGiftNum()
        {
            if(mItemData!=null)
            {
                int count = ItemDataManager.GetInstance().GetItemCountInPackage(mItemData.TableID);
                mRemainGiftNumTxt.SafeSetText(string.Format(TR.Value("LeftGiftItemNum"), count));
            }
            else
            {
                Logger.LogError("ShowLeftGiftNum mItemData is null");
            }
        }

        /// <summary>
        ///  显示我被赠送的数量
        /// </summary>
        private void _ShowBeSendNum(int curNum,int limitNum)
        {
            if(curNum > limitNum)
            {
                curNum = limitNum;
                Logger.LogError(string.Format("我被赠送的总数量超过的限制，当前的数量:{0},总的数量:{1}", curNum, limitNum));
            }
            mBeSendTotalNumTxt.SafeSetText(string.Format(TR.Value("BeSendTotalNum"), curNum, limitNum));
        }
        private void _OnCloseBtnClick()
        {
            ClientSystemManager.GetInstance().CloseFrame<SendGiftFrame>();
        }

        private int _Sort(FriendPresentInfo x, FriendPresentInfo y)
        {
            if (x.beSendedTimes > y.beSendedTimes)
            {
                return -1;
            }
            else if (x.beSendedTimes < y.beSendedTimes)
            {
                return 1;
            }
            else
            {
                if (x.isOnline > y.isOnline)
                {
                    return -1;
                }
                else if (x.isOnline < y.isOnline)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
        }
        #region 收发服务器消息

        /// <summary>
        /// 拉取赠送好友信息列表
        /// </summary>
        private void _RequestFriendPresentInfosReq(int itemId)
        {
            WorldGetItemFriendPresentInfosReq kSend = new WorldGetItemFriendPresentInfosReq();
            kSend.dataId = (UInt32)itemId;
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, kSend);
            
        }

        /// <summary>
        /// 赠送给好友红包
        /// </summary>
        private void _SendFriendPresentReq(ulong itemGUID,UInt64 friendId,UInt32 itemTableId)
        {
            WorldItemFriendPresentReq kSend = new WorldItemFriendPresentReq();
            kSend.itemId = itemGUID;
            kSend.friendId = friendId;
            kSend.itemTypeId = itemTableId;
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, kSend);
        }

        /// <summary>
        /// 获取赠送好友信息列表
        /// </summary>
        /// <param name="obj"></param>
        private void _OnFriendPresentInfosRet(MsgDATA msg)
        {
            WorldGetItemFriendPresentInfosRes res = new WorldGetItemFriendPresentInfosRes();
            res.decode(msg.bytes);
            if(mItemData==null)
            {
                Logger.LogError("mItemData is null");
                return;
            }
            if(res.dataId==mItemData.TableID)
            {
                for (int i = 0; i < res.presentInfos.Length; i++)
                {
                    FriendPresentInfo presentInfo = res.presentInfos[i];
                    mDataList.Add(presentInfo);
                }

                if (mUIGray != null && mSendBtn != null)
                {
                    mUIGray.enabled = (mDataList.Count == 0);
                    mSendBtn.interactable = (mDataList.Count != 0);
                }
                mSendBtn.CustomActive(true);
                mDataList.Sort(_Sort);
                if (mItemUIList != null)
                {
                    mItemUIList.SetElementAmount(mDataList.Count);
                }
                mNoTipCanvas.alpha = 1f;
                _ShowBeSendNum((int)res.recvedTotal,(int)res.recvedTotalLimit);
            }

        }

        /// <summary>
        /// 赠送给好友红包的返回
        /// </summary>
        /// <param name="msg"></param>
        private void _OnSendFriendPresentRet(MsgDATA msg)
        {
            WorldItemFriendPresentRes res = new WorldItemFriendPresentRes();
            res.decode(msg.bytes);
            if(mItemData!=null)
            {
                if(mItemData.TableID!=res.itemTypeId)
                {
                    Logger.LogErrorFormat(string.Format("发送礼物包给好友，服务器返回的id和本地id不一样 本地礼物的id={0},服务器发来的Id={1}", mItemData.TableID, res.itemTypeId));
                    return;
                }
            }
            if (res.retCode == 0)
            {
                for (int i = 0; i < mDataList.Count; i++)
                {
                    if (mDataList[i].friendId == res.presentInfos.friendId)
                    {
                        mDataList[i] = res.presentInfos;
                    }
                }
                if (mItemUIList != null)
                {
                   // mItemUIList.SetElementAmount(mDataList.Count);
                    mItemUIList.UpdateElementAmount(mDataList.Count);
                }
                SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("SendSucess_Tip"));
            }else 
            {
                SystemNotifyManager.SystemNotify((int)res.retCode);
            }
           
        }
        #endregion

    }
}
