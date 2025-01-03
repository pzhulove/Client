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
    public class MwRankFrame : ClientFrame
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
        MwRankListData selfRankData;
        List<MwRankListData> mRedPackRankList = new List<MwRankListData>();

        private Coroutine mCoroutine = null;
        private Coroutine mTimeCoroutine = null;

        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/RedPack/MwRankFrame";
        }

        protected sealed override void _OnOpenFrame()
        {
            
            InitInterface();
            StartCoroutine();
        }

        protected sealed override void _OnCloseFrame()
        {
            ClearData();
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
            req.url = Global.MW_RANK_ADDRESS;

            yield return req;

            if (req.GetResult() == BaseWaitHttpRequest.eState.Success)
            {
                //解析Json存入集合中
                mRedPackRankList = req.GetResultJson<List<MwRankListData>>();

                RefreshRankListCount(mRedPackRankList);
                selfRankData = FindSelfRankData(mRedPackRankList);
                UpdateSelfRankInfo(selfRankData);
            }
        }

        //查找自己是否上榜
        MwRankListData FindSelfRankData(List<MwRankListData> data)
        {
            MwRankListData mSelfRankData = null;
            if (data == null)
            {
                Logger.LogError("全平台名望榜拉取数据失败...");
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

        void InitInterface()
        {
            TimeIntrval = 0.0f;
            LeftTimeIntrval = 0.0f;

            InitRankListScrollBind();
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

            MwRankListData RankInfo = mRedPackRankList[item.m_index] as MwRankListData;

            Image RankImg = combind.GetCom<Image>("RankImg");
            Text Rank = combind.GetCom<Text>("Rank");
            Text Name = combind.GetCom<Text>("Name");
            Text Num = combind.GetCom<Text>("Num");
            Text ServerName = combind.GetCom<Text>("ServerName");
            Text Level = combind.GetCom<Text>("Level");
            Text Occ = combind.GetCom<Text>("Occ");

            if (RankInfo.rank >= 1 && RankInfo.rank <= 3)
            {
                LoadSpriteByRank(RankInfo.rank, ref RankImg);
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

            Name.text = RankInfo.role_name;

            ServerName.text = RankInfo.server_name;

            Level.text = RankInfo.level.ToString();

            var jobItem = TableManager.GetInstance().GetTableItem<ProtoTable.JobTable>(RankInfo.occ);
            if (jobItem != null)
            {
                Occ.text = jobItem.Name;
            }

            Num.text = RankInfo.total_mw.ToString();
        }

        void RefreshRankListCount(List<MwRankListData> mRedPackRankList)
        {
            if(mRedPackRankList == null)
            {
                Logger.LogAssetFormat("全平台名望从网页拉取数据失败...");
                return;
            }

            mRankListScript.SetElementAmount(mRedPackRankList.Count);

            mTips.gameObject.CustomActive(mRedPackRankList.Count <= 0);

            if(mRedPackRankList.Count <= 0){
                mUpdateTime.text = "暂未更新最新排行榜，排行榜每日凌晨更新";
            }else{
                mUpdateTime.text = "排行榜更新时间：" + mRedPackRankList[0].time;
            }
        }

        void _ShowPlayerScore()
        {
            int score = 0;
            var tmpEquip = ItemDataManager.GetInstance().GetItemsByPackageType(EPackageType.WearEquip);
            if (tmpEquip != null)
            {
                foreach (var uid in tmpEquip)
                {
                    ItemData item = ItemDataManager.GetInstance().GetItem(uid);
                    if (item != null)
                    {
                        score += item.finalRateScore;
                    }
                }
            }

            //装备的时装
            tmpEquip = ItemDataManager.GetInstance().GetItemsByPackageType(EPackageType.WearFashion);
            if (tmpEquip != null)
            {
                foreach (var uid in tmpEquip)
                {
                    ItemData item = ItemDataManager.GetInstance().GetItem(uid);
                    if (item != null)
                    {
                        score += item.finalRateScore;
                    }
                }
            }

            mSelfRedPackNum.text = score + "";
        }

        void UpdateSelfRankInfo(MwRankListData selfData)
        {
            if (selfData == null)
            {
                mSelfRankImg.gameObject.CustomActive(false);
                mSelfRank.text = "未上榜";

                _ShowPlayerScore();
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

                mSelfRedPackNum.text = selfData.total_mw.ToString();
                mSelfRankImg.gameObject.CustomActive(selfData.rank >= 1 && selfData.rank <= 3);
            }
            
            mSelfServerName.text = ClientApplication.adminServer.name;
            mSelfLevel.text = PlayerBaseData.GetInstance().Level.ToString();

            var jobItem = TableManager.GetInstance().GetTableItem<ProtoTable.JobTable>(PlayerBaseData.GetInstance().JobTableID);
            if (jobItem != null)
            {
                mSelfOcc.text = jobItem.Name;
            }

            mSelfName.text = PlayerBaseData.GetInstance().Name;         
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
                StartCoroutine();
            }

            if(LeftTimeIntrval >= LeftRequestTimeIntrval)
            {
                LeftTimeIntrval = 0.0f;
            }
        }

        #region ExtraUIBind
        private Button mBtClose = null;
        private ComUIListScript mRankListScript = null;
        private Image mSelfRankImg = null;
        private Text mSelfRank = null;
        private Text mSelfName = null;
        private Text mSelfRedPackNum = null;

        private Text mSelfServerName = null;
        private Text mSelfLevel = null;
        private Text mSelfOcc = null;

        private Text mTips = null;
        private Text mUpdateTime = null;

        protected sealed override void _bindExUI()
        {
            mBtClose = mBind.GetCom<Button>("btClose");
            mBtClose.onClick.AddListener(_onBtCloseButtonClick);
            mRankListScript = mBind.GetCom<ComUIListScript>("RankListScript");
            mSelfRankImg = mBind.GetCom<Image>("SelfRankImg");
            mSelfRank = mBind.GetCom<Text>("SelfRank");
            mSelfName = mBind.GetCom<Text>("SelfName");
            mSelfRedPackNum = mBind.GetCom<Text>("SelfRedPackNum");

            mSelfServerName = mBind.GetCom<Text>("SelfServerName");
            mSelfLevel = mBind.GetCom<Text>("SelfLevel");
            mSelfOcc = mBind.GetCom<Text>("SelfOcc");

            mTips = mBind.GetCom<Text>("Tips");

            mUpdateTime = mBind.GetCom<Text>("updateTime");
        }

        protected sealed override void _unbindExUI()
        {
            mBtClose.onClick.RemoveListener(_onBtCloseButtonClick);
            mBtClose = null;
            mRankListScript = null;
            mSelfRankImg = null;
            mSelfRank = null;
            mSelfName = null;
            mSelfRedPackNum = null;
            mSelfServerName = null;
            mSelfLevel = null;
            mSelfOcc = null;
            mTips = null;
            mUpdateTime = null;
        }
        #endregion

        #region Callback
        private void _onBtCloseButtonClick()
        {
            frameMgr.CloseFrame(this);
        }
        #endregion
    }
}
