using Network;
using Protocol;
using UnityEngine;
using ProtoTable;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Collections.Generic;
using Scripts.UI;

namespace GameClient
{
    public class RedPackRankListFrame : ClientFrame
    {
        int Item1ID = 0;
        int Item2ID = 0;
        int TicketNum = 0;
        int RankIng = 0;//排行榜前多少名

        float RequestTimeIntrval = 300.0f;
        float TimeIntrval = 0.0f;

        float LeftRequestTimeIntrval = 20.0f;
        float LeftTimeIntrval = 0.0f;

        List<RedPacketTable> m_arrBuyTable = new List<RedPacketTable>();

        List<BaseSortListEntry> RedPackRankList = new List<BaseSortListEntry>();
        BaseSortListEntry selfEntry = new BaseSortListEntry();
        RedPackRankListData selfRankData;
        List<RedPackRankListData> mRedPackRankList = new List<RedPackRankListData>();

        private Coroutine mCoroutine = null;
        private Coroutine mTimeCoroutine = null;

        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/RedPack/RedPackRankListFrame";
        }

        protected sealed override void _OnOpenFrame()
        {
            _InitBuyTableData();
           // SendRankListReq();
            InitInterface();
            BindUIEvent();
            StartCoroutine();
        }

        protected sealed override void _OnCloseFrame()
        {
            ClearData();
            UnBindUIEvent();
            StopCoroutine();
            StopTimeCoroutine();
        }

        /// <summary>
        /// 等待几秒拉取排名数据
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        private IEnumerator WaitTime(float time)
        {
            yield return Yielders.GetWaitForSeconds(time);

            StartCoroutine();

            yield break;
        }

        //请求服务器提供的网址拉取全服发红包前50名的玩家数据
        private IEnumerator WaitHttp()
        {
            BaseWaitHttpRequest req = new BaseWaitHttpRequest();
            req.url = ClientApplication.redPackRankServerPath;

            yield return req;

            if (req.GetResult() == BaseWaitHttpRequest.eState.Success)
            {
                //解析Json存入集合中
                mRedPackRankList = req.GetResultJson<List<RedPackRankListData>>();

                RefreshRankListCount(mRedPackRankList);
                selfRankData = FindSelfRankData(mRedPackRankList);
                UpdateSelfRankInfo(selfRankData);
            }
        }

        //查找自己是否上榜
        RedPackRankListData FindSelfRankData(List<RedPackRankListData> data)
        {
            RedPackRankListData mSelfRankData = null;
            if (data == null)
            {
                Logger.LogErrorFormat("全平台发红包网页拉取数据失败...");
            }
            
            for (int i = 0; i < data.Count; i++)
            {
                var RedPackRankItemData = data[i];
                if (RedPackRankItemData == null)
                {
                    continue;
                }

                //服务器Id不等于自己的服务器ID 跳过
                if (RedPackRankItemData.zone_id != ClientApplication.playerinfo.serverID)
                {
                    continue;
                }
                ulong role_id = 0;
                ulong.TryParse(RedPackRankItemData.role_id, out role_id);
                //不等于自己的RoleID 跳过
                if (role_id != PlayerBaseData.GetInstance().RoleID)
                {
                    continue;
                }

                mSelfRankData = RedPackRankItemData;
                break;
            }

            return mSelfRankData;
        }

        void StartCoroutine()
        {
            StopCoroutine();
            mCoroutine = GameFrameWork.instance.StartCoroutine(WaitHttp());
        }

        void StartTimeCoroutine()
        {
            StopTimeCoroutine();
            mTimeCoroutine = GameFrameWork.instance.StartCoroutine(WaitTime(0.2f));
        }

        void StopCoroutine()
        {
            if (mCoroutine != null)
            {
                GameFrameWork.instance.StopCoroutine(mCoroutine);
            }

            mCoroutine = null;
        }

        void StopTimeCoroutine()
        {
            if (mTimeCoroutine != null)
            {
                GameFrameWork.instance.StopCoroutine(mTimeCoroutine);
            }

            mTimeCoroutine = null;
        }

        void ClearData()
        {
            TimeIntrval = 0.0f;
            LeftTimeIntrval = 0.0f;

            m_arrBuyTable.Clear();
            RedPackRankList.Clear();

            selfEntry.name = "";
            selfEntry.id = 0;
            selfEntry.ranking = 0;
            selfRankData = null;
            mRedPackRankList.Clear();
        }

        protected void BindUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.RedPacketSendSuccess, _OnSendSuccess);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ActivityUpdate, _OnActivityUpdate);
        }

        protected void UnBindUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.RedPacketSendSuccess, _OnSendSuccess);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ActivityUpdate, _OnActivityUpdate);
        }

        void _OnSendSuccess(UIEvent iEvent)
        {
            StartTimeCoroutine();
        }

        void _OnActivityUpdate(UIEvent iEvent)
        {
            UpdateSendRedPackButton();
            UpdateShowLeftTime();
        }

        void OnShowItem1DetailData()
        {
            ItemData ItemDetailData = ItemDataManager.CreateItemDataFromTable(Item1ID);
            if (ItemDetailData == null)
            {
                return;
            }

            ItemTipManager.GetInstance().ShowTip(ItemDetailData);
        }

        void OnShowItem2DetailData()
        {
            ItemData ItemDetailData = ItemDataManager.CreateItemDataFromTable(Item2ID);
            if (ItemDetailData == null)
            {
                return;
            }

            ItemTipManager.GetInstance().ShowTip(ItemDetailData);
        }

        void InitInterface()
        {
            TimeIntrval = 0.0f;
            LeftTimeIntrval = 0.0f;

            InitRankListScrollBind();
            InitItem1();
            InitItem2();

            mRankDes.text = TR.Value("RedPackRankDesc", RankIng);
            mActivityEndDesc.text = TR.Value("RedPackRankActivityEndDesc",RankIng);
            mTittleNeedTicketNum.text = TicketNum.ToString();

            UpdateSendRedPackButton();
            UpdateShowLeftTime();
        }

        void InitItem1()
        {
            ItemData Item1Data = ItemDataManager.CreateItemDataFromTable(Item1ID);
            if (Item1Data == null)
            {
                return;
            }

            ComItem ShowItem = mItem1Pos.GetComponentInChildren<ComItem>();

            if (ShowItem == null)
            {
                ShowItem = CreateComItem(mItem1Pos);
            }

            ShowItem.Setup(Item1Data, (GameObject Obj, ItemData sitem) => { OnShowItem1DetailData(); });
        }

        void InitItem2()
        {
            ItemData Item2Data = ItemDataManager.CreateItemDataFromTable(Item2ID);
            if (Item2Data == null)
            {
                return;
            }

            ComItem ShowItem = mItem2Pos.GetComponentInChildren<ComItem>();

            if (ShowItem == null)
            {
                ShowItem = CreateComItem(mItem2Pos);
            }

            ShowItem.Setup(Item2Data, (GameObject Obj, ItemData sitem) => { OnShowItem2DetailData(); });
        }

        void _InitBuyTableData()
        {
            var data1 = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType2.SVT_NEW_YEAR_RED_PACKET_TITLE_ID);
            Item1ID = data1.Value;

            var data2 = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType2.SVT_NEW_YEAR_RED_PACKET_REWARD_ID);
            Item2ID = data2.Value;

            var data3 = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType2.SVT_NEW_YEAR_RED_PACKET_TITLE_NEED_NUM);
            TicketNum = data3.Value;

            //策划配表前多少名数据
            var data4 = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType2.SVT_NEW_YEAR_RED_PACKET_REWARD_RANK);
            if (data4 != null)
            {
                RankIng = data4.Value;
            }

            //隔多长时间刷新排行列表策划填表
            var data5 = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType2.SVT_NEW_YEAR_RED_PACKET_RANK_REFRESH_TIME);
            if (data5 != null)
            {
                RequestTimeIntrval = data5.Value;
            }
            var iter = TableManager.GetInstance().GetTable<RedPacketTable>().GetEnumerator();
            while (iter.MoveNext())
            {
                RedPacketTable table = iter.Current.Value as RedPacketTable;
                if (table.Type == RedPacketTable.eType.NEW_YEAR && table.SubType == RedPacketTable.eSubType.Buy)
                {
                    m_arrBuyTable.Add(table);
                }
            }

            if (m_arrBuyTable.Count > 0)
            {
                m_arrBuyTable.Sort((a, b) => { return a.TotalMoney - b.TotalMoney; });
            }
        }

        void InitRankListScrollBind()
        {
            mRankListScript.Initialize();

            mRankListScript.onItemVisiable = (item) =>
            {
                if (item.m_index >= 0)
                {
                    UpdateRankListScrollBind(item);
                }
            };
        }

        void UpdateRankListScrollBind(ComUIListElementScript item)
        {
            ComCommonBind combind = item.GetComponent<ComCommonBind>();
            if (combind == null)
            {
                return;
            }

            if (item.m_index < 0 || item.m_index >= mRedPackRankList.Count || mRedPackRankList == null)
            {
                return;
            }

            RedPackRankListData RankInfo = mRedPackRankList[item.m_index] as RedPackRankListData;

            Image RankImg = combind.GetCom<Image>("RankImg");
            Text Rank = combind.GetCom<Text>("Rank");
            Text Name = combind.GetCom<Text>("Name");
            Text Num = combind.GetCom<Text>("Num");
            GameObject EffectPos = combind.GetGameObject("EffectPos");

            //string EffectPath = "middle/left/Scroll View/Viewport/Content/RankEle_{0}/EffectPos/EffUI_Xinnian_ph0{1}(Clone)";
            //GameObject RankEffect = null;

//             for(int i = 0; i < 3; i++)
//             {
//                 RankEffect = Utility.FindGameObject(frame, string.Format(EffectPath, item.m_index, i + 1), false);
// 
//                 if (RankEffect != null)
//                 {
//                     GameObject.Destroy(RankEffect);
//                 }
//             }

            if (RankInfo.rank >= 1 && RankInfo.rank <= 3)
            {
                LoadSpriteByRank(RankInfo.rank, ref RankImg);
                //LoadRankEffect(RankEffect, EffectPos, RankInfo.ranking);
            }

            RankImg.gameObject.CustomActive(RankInfo.rank >= 1 && RankInfo.rank <= 3);

            if (RankInfo.rank > 3 && RankInfo.rank <= RankIng)
            {
                Rank.text = TR.Value("color_green", RankInfo.rank);
            }
            else if (RankInfo.rank > RankIng)
            {
                Rank.text = RankInfo.rank.ToString();
            }
            else
            {
                Rank.text = "";
            }

            Name.text = TR.Value("RedPackRankServerNameOrRoleName",RankInfo.server_name, RankInfo.role_name);
            Num.text = RankInfo.total_money.ToString();
        }

        void RefreshRankListCount(List<RedPackRankListData> mRedPackRankList)
        {
            if(mRedPackRankList == null)
            {
                Logger.LogAssetFormat("全平台发红包从网页拉取数据失败...");
                return;
            }

            mRankListScript.SetElementAmount(mRedPackRankList.Count);

            mTips.gameObject.CustomActive(mRedPackRankList.Count <= 0);
        }

        void UpdateSelfRankInfo(RedPackRankListData selfData)
        {
            if (selfData == null)
            {
                mSelfRankImg.gameObject.CustomActive(false);
                mSelfRank.text = "未上榜";
                mSelfRedPackNum.text = CountDataManager.GetInstance().GetCount("new_year_red_packet_num").ToString();
            }
            else
            {
                if(selfData.rank > 3 && selfData.rank <= RankIng)
                {
                    mSelfRank.text = TR.Value("color_green", selfData.rank);
                }
                else if (selfData.rank > RankIng)
                {
                    mSelfRank.text = selfData.rank.ToString();
                }
                else
                {
                    mSelfRank.text = "";
                    LoadSpriteByRank(selfData.rank, ref mSelfRankImg);
                }

                mSelfRedPackNum.text = selfData.total_money.ToString();
                mSelfRankImg.gameObject.CustomActive(selfData.rank >= 1 && selfData.rank <= 3);
            }
            
            mSelfName.text = TR.Value("RedPackRankServerNameOrRoleName",ClientApplication.adminServer.name, PlayerBaseData.GetInstance().Name);         
        }

        void UpdateSendRedPackButton()
        {
            ActiveManager.ActiveData activeData = ActiveManager.GetInstance().GetActiveData(RedPackDataManager.GetInstance().NewYearRedPackActivityID);

            if (activeData != null && activeData.mainInfo.state == (byte)StateType.Running)
            {
                mSendRedPackGray.gameObject.CustomActive(false);
                mSendRedPackEffect.gameObject.CustomActive(true);
            }
            else
            {
                mSendRedPackGray.gameObject.CustomActive(true);
                mSendRedPackEffect.gameObject.CustomActive(false);
            }
        }

        void UpdateShowLeftTime()
        {
            ActiveManager.ActiveData activeData = ActiveManager.GetInstance().GetActiveData(RedPackDataManager.GetInstance().NewYearRedPackActivityID);

            if (activeData != null && activeData.mainInfo.state == (byte)StateType.Running)
            {
                mLeftTime.text = Function.GetLeftTimeStr(activeData.mainInfo.dueTime, ShowtimeType.NewYearRedPack);
            }

            mLeftTime.gameObject.CustomActive(activeData != null && activeData.mainInfo.state == (byte)StateType.Running);
        }

        void LoadSpriteByRank(int iRank, ref Image img)
        {
            if (iRank == 1)
            {
                ETCImageLoader.LoadSprite(ref img, "UI/Image/Packed/p_UI_RedEnvelope.png:UI_Hongbao_Tubiao_Biaoshi_01");
            }
            else if (iRank == 2)
            {
                ETCImageLoader.LoadSprite(ref img, "UI/Image/Packed/p_UI_RedEnvelope.png:UI_Hongbao_Tubiao_Biaoshi_02");
            }
            else
            {
                ETCImageLoader.LoadSprite(ref img, "UI/Image/Packed/p_UI_RedEnvelope.png:UI_Hongbao_Tubiao_Biaoshi_03");
            }
        }

        void LoadRankEffect(GameObject child, GameObject Parent, int Rank)
        {
            child = AssetLoader.instance.LoadResAsGameObject(string.Format("Effects/UI/Prefab/EffUI_Xinnian/EffUI_Xinnian_ph0{0}", Rank));

            if (child != null)
            {
                Utility.AttachTo(child, Parent);
            }
        }

        void SendRankListReq()
        {
            WorldSortListReq msg = new WorldSortListReq();

            msg.type = SortListDecoder.MakeSortListType(SortListType.SORTLIST_NEW_YEAR_RED_PACKET);
            msg.start = 0;
            msg.num = 50;

            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, msg);
        }

        [MessageHandle(WorldSortListRet.MsgID)]
        void OnRankListRes(MsgDATA msg)
        {
            WorldSortListRet res = new WorldSortListRet();
            res.decode(msg.bytes);

            int pos = 0;
            BaseSortList arrRecods = SortListDecoder.Decode(msg.bytes, ref pos, msg.bytes.Length);

            if(arrRecods == null)
            {
                Logger.LogError("arrRecods decode fail");
                return;
            }

            RedPackRankList.Clear();

            RedPackRankList = arrRecods.entries;
            selfEntry = arrRecods.selfEntry;
            
        }

        public sealed override bool IsNeedUpdate()
        {
            return true;
        }

        protected sealed override void _OnUpdate(float timeElapsed)
        {
            TimeIntrval += timeElapsed;
            LeftTimeIntrval += timeElapsed;

            if (TimeIntrval >= RequestTimeIntrval)
            {
                TimeIntrval = 0.0f;
                //SendRankListReq();
                StartCoroutine();
            }

            if(LeftTimeIntrval >= LeftRequestTimeIntrval)
            {
                LeftTimeIntrval = 0.0f;
                UpdateShowLeftTime();
            }
        }

        #region ExtraUIBind
        private Button mBtClose = null;
        private Button mBtSendRedPack = null;
        private Text mLeftTime = null;
        private ComUIListScript mRankListScript = null;
        private Image mSelfRankImg = null;
        private Text mSelfRank = null;
        private Text mSelfName = null;
        private Text mSelfRedPackNum = null;
        private GameObject mItem1Pos = null;
        private GameObject mItem2Pos = null;
        private Text mTittleNeedTicketNum = null;
        private Text mTips = null;
        private Image mSendRedPackGray = null;
        private GameObject mSendRedPackEffect = null;
        private Text mRankDes = null;
        private Text mActivityEndDesc = null;

        protected sealed override void _bindExUI()
        {
            mBtClose = mBind.GetCom<Button>("btClose");
            mBtClose.onClick.AddListener(_onBtCloseButtonClick);
            mBtSendRedPack = mBind.GetCom<Button>("btSendRedPack");
            mBtSendRedPack.onClick.AddListener(_onBtSendRedPackButtonClick);
            mLeftTime = mBind.GetCom<Text>("LeftTime");
            mRankListScript = mBind.GetCom<ComUIListScript>("RankListScript");
            mSelfRankImg = mBind.GetCom<Image>("SelfRankImg");
            mSelfRank = mBind.GetCom<Text>("SelfRank");
            mSelfName = mBind.GetCom<Text>("SelfName");
            mSelfRedPackNum = mBind.GetCom<Text>("SelfRedPackNum");
            mItem1Pos = mBind.GetGameObject("Item1Pos");
            mItem2Pos = mBind.GetGameObject("Item2Pos");
            mTittleNeedTicketNum = mBind.GetCom<Text>("TittleNeedTicketNum");
            mTips = mBind.GetCom<Text>("Tips");
            mSendRedPackGray = mBind.GetCom<Image>("SendRedPackGray");
            mSendRedPackEffect = mBind.GetGameObject("SendRedPackEffect");
            mRankDes = mBind.GetCom<Text>("RankDes");
            mActivityEndDesc = mBind.GetCom<Text>("ActivityEndDesc");
        }

        protected sealed override void _unbindExUI()
        {
            mBtClose.onClick.RemoveListener(_onBtCloseButtonClick);
            mBtClose = null;
            mBtSendRedPack.onClick.RemoveListener(_onBtSendRedPackButtonClick);
            mBtSendRedPack = null;
            mLeftTime = null;
            mRankListScript = null;
            mSelfRankImg = null;
            mSelfRank = null;
            mSelfName = null;
            mSelfRedPackNum = null;
            mItem1Pos = null;
            mItem2Pos = null;
            mTittleNeedTicketNum = null;
            mTips = null;
            mSendRedPackGray = null;
            mSendRedPackEffect = null;
            mRankDes = null;
            mActivityEndDesc = null;
        }
        #endregion

        #region Callback
        private void _onBtCloseButtonClick()
        {
            frameMgr.CloseFrame(this);
        }
        private void _onBtSendRedPackButtonClick()
        {
            ActiveManager.ActiveData activeData = ActiveManager.GetInstance().GetActiveData(RedPackDataManager.GetInstance().NewYearRedPackActivityID);
            //全平台发红包未找到活动数据
            if (activeData == null)
            {
                return;
            }

            //活动已结束
            if(activeData.mainInfo.state != (byte)StateType.Running)
            {
                SystemNotifyManager.SysNotifyFloatingEffect("发送红包活动已结束");
                return;
            }

            //当前等级小于活动开放等级，无法领取红包
            if (PlayerBaseData.GetInstance().Level < activeData.mainInfo.level)
            {
                SystemNotifyManager.SysNotifyFloatingEffect(string.Format("等级不足{0}级,无法发送红包", activeData.mainInfo.level));
                return;
            }

            //红包金额对应的红包个数
            if(m_arrBuyTable.Count <= 0)
            {
                return;
            }

            SendRedPacketFrame.sendRedPackType = SendRedPackType.NewYear;
            ClientSystemManager.GetInstance().OpenFrame<SendRedPacketFrame>(FrameLayer.Middle, m_arrBuyTable);
        }
        #endregion
    }
}
