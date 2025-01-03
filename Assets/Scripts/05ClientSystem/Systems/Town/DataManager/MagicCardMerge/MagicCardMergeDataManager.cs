using System;
using System.Collections;
using System.Collections.Generic;
using Network;
using Protocol;
using ProtoTable;
using UnityEngine;


namespace GameClient
{

    public class MagicCardMergeDataManager : DataManager<MagicCardMergeDataManager>
    {

        //是否显示附魔卡合成类型不同的提示
        public bool IsNotShowMagicCardMergeOwnerDifferentTip = false;
        public bool IsNotShowMagicCardMergeLevelTip = false;

        //一件合成的提示界面
        public bool IsMagicCardOneKeyMergeUseWhiteCard = true;
        public bool IsMagicCardOneKeyMergeUseBlueCard = false;
        public bool IsMagicCardOneKeyMergeUserGold = false;
        public bool IsMagicCardOneKeyMergeUseNoBindItem = false;

        //一键合成的结果
        public SceneMagicCardCompOneKeyRes OneKeyMergeRes = null;

        #region Initialize
        public override void Initialize()
        {
            BindUiEvents();
            BindNetEvents();
        }

        public override void Clear()
        {
            UnBindUiEvents();
            UnBindNetEvents();
            ClearData();
        }

        private void BindUiEvents()
        {
        }

        private void UnBindUiEvents()
        {
        }

        private void BindNetEvents()
        {
            NetProcess.AddMsgHandler(SceneMagicCardCompOneKeyRes.MsgID,
                OnReceiveMagicCardOneKeyMergeRes);
        }

        private void UnBindNetEvents()
        {
            NetProcess.AddMsgHandler(SceneMagicCardCompOneKeyRes.MsgID,
                OnReceiveMagicCardOneKeyMergeRes);
        }

        private void ClearData()
        {
            IsNotShowMagicCardMergeOwnerDifferentTip = false;
            IsNotShowMagicCardMergeLevelTip = false;
            OneKeyMergeRes = null;
        }

        #endregion


        //发送一键合成的请求
        public void SendMagicCardOneKeyMergeReq()
        {
            SceneMagicCardCompOneKeyReq oneKeyMergeReq = new SceneMagicCardCompOneKeyReq();
            oneKeyMergeReq.isCompWhite = IsMagicCardOneKeyMergeUseWhiteCard == true ? (byte)1 : (byte)0;
            oneKeyMergeReq.isCompBlue = IsMagicCardOneKeyMergeUseBlueCard == true ? (byte) 1 : (byte) 0;
            oneKeyMergeReq.autoUseGold = IsMagicCardOneKeyMergeUserGold == true ? (byte) 1 : (byte) 0;
            oneKeyMergeReq.compNotBind = IsMagicCardOneKeyMergeUseNoBindItem == true ? (byte) 1 : (byte) 0;

            var netMgr = NetManager.Instance();
            if (netMgr != null)
                netMgr.SendCommand(ServerType.GATE_SERVER, oneKeyMergeReq);
        }

        //接受请求
        public void OnReceiveMagicCardOneKeyMergeRes(MsgDATA msgData)
        {
            OneKeyMergeRes = null;
            SceneMagicCardCompOneKeyRes res = new SceneMagicCardCompOneKeyRes();
            res.decode(msgData.bytes);
            
            if (res.code != (uint)ProtoErrorCode.SUCCESS)
            {
                //合成错误
                SystemNotifyManager.SystemNotify((int) res.code);
                return;
            }
            else
            {
                //打开合成结果
                OneKeyMergeRes = res;
                MagicCardMergeUtility.OnOpenMagicCardOneKeyMergeResultFrame();
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnOneKeyMergeSuccess);
            }
        }
        
        public void ResetOneKeyMergeSetting()
        {
            IsMagicCardOneKeyMergeUseWhiteCard = true;
            IsMagicCardOneKeyMergeUseBlueCard = false;
            IsMagicCardOneKeyMergeUserGold = false;
            IsMagicCardOneKeyMergeUseNoBindItem = false;
        }

        public void ResetOneKeyMergeRes()
        {
            OneKeyMergeRes = null;
        }


        /// <summary>
        /// 一键合成界面展示的附魔卡
        /// </summary>
        /// <returns></returns>
        public List<ItemData> GetEnchantmentCardMergeItemDataList()
        {
            List<ItemData> itemList = new List<ItemData>();

            var itemIds = ItemDataManager.GetInstance().GetItemsByType(ItemTable.eType.EXPENDABLE);
            for (int i = 0; i < itemIds.Count; ++i)
            {
                var itemData = MagicCardMergeUtility.GetMagicCardItem(itemIds[i]);
                if (itemData != null)
                {
                    if(itemData.Quality > ItemTable.eColor.BLUE)
                    {
                        continue;
                    }

                    if(itemData.Quality == ItemTable.eColor.WHITE)
                    {
                        if (IsMagicCardOneKeyMergeUseWhiteCard == false)
                            continue;
                        
                    }
                    else if(itemData.Quality == ItemTable.eColor.BLUE)
                    {
                        if (IsMagicCardOneKeyMergeUseBlueCard == false)
                            continue;
                    }

                    if(itemData.BindAttr == ItemTable.eOwner.NOTBIND)
                    {
                        if (IsMagicCardOneKeyMergeUseNoBindItem == false)
                            continue;
                    }

                    itemList.Add(itemData);
                }
            }

            itemList.Sort((x, y) => { return x.Quality - y.Quality; });

            return itemList;
        }
    }
}
