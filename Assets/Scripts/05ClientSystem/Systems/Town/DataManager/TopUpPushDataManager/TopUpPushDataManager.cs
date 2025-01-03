using Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Network;


namespace GameClient
{
    public class TopUpPushDataModel
    {
        /// <summary>
        /// 截止时间戳
        /// </summary>
        public uint validTimesTamp;

        public List<TopUpPushItemData> mItems;
    }

    public class TopUpPushItemData
    {
        /// <summary>
        /// 推荐ID
        /// </summary>
        public int pushId;
        public int itemId;
        public int itemCount;
        /// <summary>
        /// 购买次数
        /// </summary>
        public int buyTimes;
        /// <summary>
        /// 最大购买次数
        /// </summary>
        public int maxTimes;
        /// <summary>
        /// 原价
        /// </summary>
        public int price;
        /// <summary>
        /// 折扣价
        /// </summary>
        public int disCountPrice;
        public int lastTimestamp;
    }

    public class TopUpPushDataManager : DataManager<TopUpPushDataManager>
    {

        private TopUpPushDataModel model;

        private bool isUpdate = false;

        private bool isSend = false;

        float TimeIntrval = 0.0f;

        float sendTime = 0.0f;

        //请求次数
        int senNumber = 0;

        //截止时间
        public uint validTimesTamp;

        private bool mLoginTopUpPushIsOpen = false;
        /// <summary>
        /// 首次登录充值推送是否打开
        /// </summary>
        public bool LoginTopUpPushIsOpen
        {
            get { return mLoginTopUpPushIsOpen; }
            set { mLoginTopUpPushIsOpen = value; }
        }

        public sealed override EEnterGameOrder GetOrder()
        {
            return EEnterGameOrder.Default;
        }

        public sealed override void Clear()
        {
            UnRegisterNetHandler();
            model = null;
            isUpdate = false;
            isSend = false;
            mLoginTopUpPushIsOpen = false;
            TimeIntrval = 0.0f;
            sendTime = 0.0f;
            senNumber = 0;
        }

        public sealed override void Initialize()
        {
            isUpdate = false;
            isSend = false;
            RegisterNetHandler();
        }


        public sealed override void Update(float a_fTime)
        {
            TimeIntrval += a_fTime;
           
            //每隔5分钟请求一下推送道具列表
            if (TimeIntrval >= 3600.0f)
            {
                SendWorldGetRechargePushItemsReq();
                TimeIntrval = 0;
            }

            if (isSend == true)
            {
                sendTime += a_fTime;
                if (sendTime >= 5.0f)
                {
                    SendWorldGetRechargePushItemsReq();
                    isSend = false;
                    sendTime = 0;
                }
            }
            
            if (isUpdate)
            {
                uint timer = validTimesTamp - TimeManager.GetInstance().GetServerTime();
                if (timer <= 0)
                {
                    isUpdate = false;
                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.TopUpPushButtonClose);

                    //活动结束，如果充值推送界面开着，就强行关闭
                    if (ClientSystemManager.GetInstance().IsFrameOpen<TopUpPushFrame>())
                    {
                        ClientSystemManager.GetInstance().CloseFrame<TopUpPushFrame>();
                    }
                }
            }
        }

        /// <summary>
        /// 得到充值推送数据
        /// </summary>
        /// <returns></returns>
        public TopUpPushDataModel GetTopUpPushDataModel()
        {
            return model;
        }

        private void RegisterNetHandler()
        {
            NetProcess.AddMsgHandler(WorldGetRechargePushItemsRes.MsgID, OnSyncWorldGetRechargePushItemsRes);
            NetProcess.AddMsgHandler(WorldBuyRechargePushItemsRes.MsgID, OnSyncWorldBuyRechargePushItemsRes);
        }

        private void UnRegisterNetHandler()
        {
            NetProcess.RemoveMsgHandler(WorldGetRechargePushItemsRes.MsgID, OnSyncWorldGetRechargePushItemsRes);
            NetProcess.RemoveMsgHandler(WorldBuyRechargePushItemsRes.MsgID, OnSyncWorldBuyRechargePushItemsRes);
        }

        /// <summary>
        /// 获取充值推送道具列表请求
        /// </summary>
        public void SendWorldGetRechargePushItemsReq()
        {
            WorldGetRechargePushItemsReq req = new WorldGetRechargePushItemsReq();
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);
        }

        /// <summary>
        /// /// <summary>
        /// 获取充值推送道具列表返回
        /// </summary>
        /// </summary>
        /// <param name="msg"></param>
        private void OnSyncWorldGetRechargePushItemsRes(MsgDATA msg)
        {
            WorldGetRechargePushItemsRes res = new WorldGetRechargePushItemsRes();
            res.decode(msg.bytes);

            if (res.retCode != (uint)ProtoErrorCode.SUCCESS)
            {
                SystemNotifyManager.SystemNotify((int)res.retCode);
            }
            else
            {
                model = new TopUpPushDataModel();
                model.mItems = new List<TopUpPushItemData>();

                for (int i = 0; i < res.itemVec.Length; i++)
                {
                    var resItemVec = res.itemVec[i];

                    int timeStamp = (int)(resItemVec.validTimestamp - TimeManager.GetInstance().GetServerTime());

                    if (timeStamp <= 0)
                    {
                        continue;
                    }

                    model.validTimesTamp = resItemVec.validTimestamp;
                    validTimesTamp = resItemVec.validTimestamp;

                    TopUpPushItemData data = new TopUpPushItemData();
                    data.pushId = (int)resItemVec.pushId;
                    data.itemId = (int)resItemVec.itemId;
                    data.itemCount = (int)resItemVec.itemCount;
                    data.buyTimes = (int)resItemVec.buyTimes;
                    data.maxTimes = (int)resItemVec.maxTimes;
                    data.price = (int)resItemVec.price;
                    data.disCountPrice = (int)resItemVec.discountPrice;
                    data.lastTimestamp = (int)resItemVec.lastTimestamp;

                    model.mItems.Add(data);
                }

                if (model.mItems.Count > 0)
                {
                    if (CheckItemIsAfterBuying(model.mItems) == true)
                    {
                        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.TopUpPushButtonClose);
                    }
                    else
                    {
                        isUpdate = true;

                        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.TopUpPushButoonOpen);
                    }
                }

                senNumber++;

                if (senNumber <= 1)
                {
                    isSend = true;
                }
            }
        }

        /// <summary>
        /// 购买充值推送道具请求
        /// </summary>
        public void OnSendWorldBuyRechargePushItemsReq(int pushId)
        {
            WorldBuyRechargePushItemsReq req = new WorldBuyRechargePushItemsReq();
            req.pushId = (uint)pushId;

            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);
        }

        /// <summary>
        /// 购买充值推送道具返回
        /// </summary>
        private void OnSyncWorldBuyRechargePushItemsRes(MsgDATA msg)
        {
            WorldBuyRechargePushItemsRes res = new WorldBuyRechargePushItemsRes();
            res.decode(msg.bytes);

            if (res.retCode != (uint)ProtoErrorCode.SUCCESS)
            {
                SystemNotifyManager.SystemNotify((int)res.retCode);
            }
            else
            {
                if (model != null && model.mItems != null)
                {
                    model.mItems.Clear();

                    for (int i = 0; i < res.itemVec.Length; i++)
                    {
                        var resItemVec = res.itemVec[i];

                        int timeStamp = (int)(resItemVec.validTimestamp - TimeManager.GetInstance().GetServerTime());

                        if (timeStamp <= 0)
                        {
                            continue;
                        }

                        model.validTimesTamp = resItemVec.validTimestamp;
                        validTimesTamp = resItemVec.validTimestamp;

                        TopUpPushItemData data = new TopUpPushItemData();
                        data.pushId = (int)resItemVec.pushId;
                        data.itemId = (int)resItemVec.itemId;
                        data.itemCount = (int)resItemVec.itemCount;
                        data.buyTimes = (int)resItemVec.buyTimes;
                        data.maxTimes = (int)resItemVec.maxTimes;
                        data.price = (int)resItemVec.price;
                        data.disCountPrice = (int)resItemVec.discountPrice;
                        data.lastTimestamp = (int)resItemVec.lastTimestamp;

                        model.mItems.Add(data);
                    }

                    if (CheckItemIsAfterBuying(model.mItems) == true)
                    {
                        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.TopUpPushButtonClose);
                    }

                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.TopUpPushBuySuccess);
                }
            }
        }

        /// <summary>
        /// 检查推送出来的道具是否购买完毕
        /// </summary>
        /// <returns></returns>
        private bool CheckItemIsAfterBuying(List<TopUpPushItemData> items)
        {
            if (items == null)
            {
                return false;
            }

            //是否买完
            bool isAfterBuying = false;
            int number = 0;
            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];
                //剩余次数
                int iResidueDegree = item.maxTimes - item.buyTimes;
                if (iResidueDegree > 0)
                {
                    continue;
                }

                number++;
            }

            if (number == items.Count)
            {
                isAfterBuying = true;
            }
            else
            {
                isAfterBuying = false;
            }
            return isAfterBuying;
        }

        /// <summary>
        /// 检查第一次登录是否推送
        /// </summary>
        /// <returns></returns>
        public bool CheckFirstLoginIsPush()
        {
            bool isPush = false;
            if (model != null && model.mItems != null && model.mItems.Count > 0)
            {
                for (int i = 0; i < model.mItems.Count; i++)
                {
                    var data = model.mItems[i];
                    if (data.lastTimestamp != 0)
                    {
                        continue;
                    }

                    isPush = true;
                    break;
                }
            }

            return isPush;
        }
    }
}

