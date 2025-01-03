using Network;
using Protocol;
using Scripts.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class RandomTreasureFrame : ClientFrame
    {
        public enum ChildFrameType
        {
            Map,
            Atlas,
            Raffle,
        }
        
        #region Model Params

        ChildFrameType mSelectedFrameType = ChildFrameType.Map;

        RandomTreasureMap mMapFrameObj = null;

        string tr_record_info_format = "";
        string tr_gold_treasure_desc = "";
        string tr_silver_treasure_desc = "";
        string tr_player_name_format = "";
        string tr_change_map_btn_cd = "";

        bool canCloseFrame = true;

        private bool bRefreshRecordInfoDelay = false;                               //是否开启延迟刷新记录
        private bool bRefreshItemCountDelay = false;                                //是否开启延迟刷新使用道具数量

        private bool bRefreshDigInfoDelay = false;                                  //是否开启延迟刷新挖宝点
        public bool BRefreshDigInfoDelay
        {
            get { return bRefreshDigInfoDelay; }
        }

        #endregion
        
        #region View Params
        #endregion
        
        #region PRIVATE METHODS

        protected override void _OnOpenFrame()
        {
            _BindNetEvent();
            _BindUIEvent();
            _InitFrame();
        }

        protected override void _OnCloseFrame()
        {
            _CloaseAllAttachFrame();
            _ClearFrame();
            _UnBindUIEvent();
            _UnBindNetEvent();
        }

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/RandomTreasureFrame/RandomTreasureFrame";
        }

        private void _InitFrame()
        {
            tr_record_info_format = TR.Value("random_treasure_record_format");
            tr_gold_treasure_desc = TR.Value("random_treasure_gold_box");
            tr_silver_treasure_desc = TR.Value("random_treasure_silver_box");
            tr_player_name_format = TR.Value("random_treasure_player_name_format");
            tr_change_map_btn_cd = TR.Value("random_treasure_change_map_btn_cd");

            if (mComMiniMap != null)
            {
                mComMiniMap.BindFuncBtnEvent(() => {
                    _OpenRandomTreasureAtlas();
                });
            }

            if (mNocache)
            {
                mNocache.CustomActive(true);
            }

            if (mMapFrameObj == null)
            {
                mMapFrameObj = new RandomTreasureMap();
                mMapFrameObj.Create(mMapRoot, this);
            }

            RandomTreasureDataManager.GetInstance().ReqOpenFirstTreasureMap();
            RandomTreasureDataManager.GetInstance().ReqMapRecordInfo();
            RandomTreasureDataManager.GetInstance().ReqTotalAtlasInfo();
        }

        private void _RefreshFrame()
        {
            switch (mSelectedFrameType)
            {
                case ChildFrameType.Map:
                    break;
                case ChildFrameType.Atlas:
                    break;
                case ChildFrameType.Raffle:
                    break;
            }
        }

        private void _CloaseAllAttachFrame()
        {
            _CloseSelectedRandomTreasureMap();
            _CloseRandomTreasureAtlas();
            _CloseRandomTreasureRaffle();
        }

        private void _ClearFrame()
        {
            mSelectedFrameType = ChildFrameType.Map;

            if (mMapFrameObj != null)
            {
                mMapFrameObj.Destroy();
                mMapFrameObj = null;
            }

            tr_record_info_format = "";
            tr_gold_treasure_desc = "";
            tr_silver_treasure_desc = "";
            tr_player_name_format = "";
            tr_change_map_btn_cd = "";

            canCloseFrame = true;
            bRefreshRecordInfoDelay = false;
            bRefreshItemCountDelay = false;
            bRefreshDigInfoDelay = false;

            //新增 置空
            if (mRecordScrollList != null)
            {
                mRecordScrollList.SetElementAmount(0);
            }
        }

        private void _OpenSelectedRandomTreasureMap(RandomTreasureMapModel mapModel)
        {
            if (mMapRoot == null)
            {
                Logger.LogErrorFormat("[RandomTreasureFrame] - _OpenSelectedRandomTreasureMap mMapRoot is null");
                return;
            }
            if (mapModel == null)
            {
                return;
            }
            mMapFrameObj.RefreshData(mapModel);
            mMapFrameObj.Show();
        }

        private void _CloseSelectedRandomTreasureMap()
        {
            if (mMapFrameObj != null)
            {
                mMapFrameObj.Hide();
            }
        }

        private void _OpenRandomTreasureAtlas()
        {
            RandomTreasureMapModel currentMapModel = null;
            if(mMapFrameObj != null)
            {
                currentMapModel = mMapFrameObj.GetCurrentMapModel();
            }

            RandomTreasureAtlas randomTreasureAtlas = null;
            if (ClientSystemManager.GetInstance().IsFrameOpen<RandomTreasureAtlas>())
            {
                randomTreasureAtlas = ClientSystemManager.GetInstance().GetFrame(typeof(RandomTreasureAtlas)) as RandomTreasureAtlas;
            }
            else
            {
                randomTreasureAtlas = ClientSystemManager.GetInstance().OpenFrame<RandomTreasureAtlas>(FrameLayer.Middle, currentMapModel) as RandomTreasureAtlas;
            }
            if (randomTreasureAtlas != null)
            {
                RandomTreasureDataManager.GetInstance().ReqTotalAtlasInfo();
            }
        }

        private void _CloseRandomTreasureAtlas()
        {
            if (ClientSystemManager.GetInstance().IsFrameOpen<RandomTreasureAtlas>())
            {
                ClientSystemManager.GetInstance().CloseFrame<RandomTreasureAtlas>();
            }
        }

        private void _OpenRandomTreasureRaffle(RandomTreasureMapDigSiteModel digSiteModel)
        {
            RandomTreasureRaffle randomTreasureRaffle = null;
            if (ClientSystemManager.GetInstance().IsFrameOpen<RandomTreasureRaffle>())
            {
                randomTreasureRaffle = ClientSystemManager.GetInstance().GetFrame(typeof(RandomTreasureRaffle)) as RandomTreasureRaffle;
            }
            else
            {
                randomTreasureRaffle = ClientSystemManager.GetInstance().OpenFrame<RandomTreasureRaffle>(FrameLayer.Middle, digSiteModel) as RandomTreasureRaffle;
            }
            if (randomTreasureRaffle != null && digSiteModel != null)
            {
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnWatchRefreshDigSite, digSiteModel);                    
            }
        }

        private void _CloseRandomTreasureRaffle()
        {
            if (ClientSystemManager.GetInstance().IsFrameOpen<RandomTreasureRaffle>())
            {
                ClientSystemManager.GetInstance().CloseFrame<RandomTreasureRaffle>();
            }
        }

        private void _BindNetEvent()
        {
            RandomTreasureDataManager.GetInstance().AddNetEventListener();
        }

        private void _UnBindNetEvent()
        {
            RandomTreasureDataManager.GetInstance().RemoveNetEventListener();
        }

        private void _RefreshGoldAndSilverKeyNum()
        {
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnTresureItemCountChanged);
        }

        private void _RefreshRecordView()
        {
            List<RandomTreasureMapRecordModel> recordModel = RandomTreasureDataManager.GetInstance().GetMapRecordInfoList();
            if (recordModel == null)
            {
                return;
            }
            if (mRecordScrollList == null)
            {
                return;
            }
            if (mRecordScrollList.IsInitialised() == false)
            {
                mRecordScrollList.Initialize();
                mRecordScrollList.onBindItem = (GameObject go) => {
                    LinkParse linkText = go.GetComponent<LinkParse>();
                    return linkText;
                };
            }
            mRecordScrollList.onItemVisiable = (var) =>
            {
                if (var == null)
                {
                    return;
                }
                int iIndex = var.m_index;
                if (iIndex >= 0 && iIndex < recordModel.Count)
                {
                    LinkParse link = var.gameObjectBindScript as LinkParse;
                    if (link == null)
                    {
                        return;
                    }
                    int cIndex = recordModel.Count - 1 - iIndex;
                    RandomTreasureMapRecordModel model = recordModel[cIndex];
                    if (model == null)
                    {
                        return;
                    }
                    if(link.m_kTarget == null)
                    {
                        return;
                    }
                    string treasureTypeName = "";
                    if(model.digType == DigType.DIG_GLOD)
                    {
                        treasureTypeName = tr_gold_treasure_desc;
                    }
                    else if (model.digType == DigType.DIG_SILVER)
                    {
                        treasureTypeName = tr_silver_treasure_desc;
                    }
                    string playerName = string.Format(tr_player_name_format, model.playerName);
                    if (model.itemSData == null)
                    {
                        return;
                    }
                    string itemName = string.Format("{{I 0 {0} 0}} * {1}", model.itemSData.ItemID, model.itemSData.Count);
                    link.SetText(string.Format(tr_record_info_format, playerName, model.mapName, treasureTypeName, itemName));
                }
            };

            mRecordScrollList.SetElementAmount(recordModel.Count);
			if(recordModel.Count > 0)
			{
            	//倒序
            	mRecordScrollList.EnsureElementVisable(0);
				//mRecordScrollList.EnsureElementVisable(recordModel.Count - 1);
			}
            Logger.LogProcessFormat("[RandomTreasureFrame] - _RefreshRecordView");
        }

        /// <summary>
        /// 尝试切换到一个相比于当前地图，更满足条件的地图
        /// </summary>
        private void _TryChangeSuitableTreasureMap()
        {
            if(mMapFrameObj == null)
            {
                return;
            }
            RandomTreasureMapModel currMapModel = mMapFrameObj.GetCurrentMapModel();
            if (currMapModel == null)
            {
                Logger.LogError("[RandomTreasureFrame] - _TryChangeSuitableTreasureMap currMapModel is null !!! ");
                return;
            }
            List<RandomTreasureMapModel> goldMapModelList = RandomTreasureDataManager.GetInstance().GetTreasureTypeMapModelList(Protocol.DigType.DIG_GLOD, true, currMapModel);
            List<RandomTreasureMapModel> silverMapModelList = RandomTreasureDataManager.GetInstance().GetTreasureTypeMapModelList(Protocol.DigType.DIG_SILVER, true, currMapModel);
            if (goldMapModelList == null || silverMapModelList == null)
            {
                return;
            }

            RandomTreasureMapModel suitMapModel = null;
            int randomIndex = 0;
            if (goldMapModelList.Count > 0)
            {
                randomIndex = UnityEngine.Random.Range(0, goldMapModelList.Count - 1);
                suitMapModel = goldMapModelList[randomIndex];
            }
            else if (goldMapModelList.Count == 0 && silverMapModelList.Count > 0)
            {
                randomIndex = UnityEngine.Random.Range(0, silverMapModelList.Count - 1);
                suitMapModel = silverMapModelList[randomIndex];
            }
            else if (goldMapModelList.Count == 0 && silverMapModelList.Count == 0)
            {
                List<RandomTreasureMapModel> filterCurrMapModelList = RandomTreasureDataManager.GetInstance().GetFilterMapModelList(currMapModel);
                if (filterCurrMapModelList == null || filterCurrMapModelList.Count <= 0)
                {
                    return;
                }
                randomIndex = UnityEngine.Random.Range(0, filterCurrMapModelList.Count - 1);
                suitMapModel = filterCurrMapModelList[randomIndex];
            }
            if (suitMapModel != null)
            {
                mMapFrameObj.TryChangeTreasureMap(suitMapModel);
            }
        }

        #region UI Event

        private void _BindUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnOpenTreasureDigMap, _OnOpenTreasureDigMap);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnTreasureDigMapOpen, _OnTreasureDigMapOpen);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnTreasureDigMapClose, _OnTreasureDigMapClose);

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnChangeTreasureDigMap, _OnChangeTreasureDigMap);

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnTreasureMapPlayerNumSync, _OnTreasureMapPlayerNumChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnTreasureMapDigReset, _OnTreasureMapDigReset);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnTreasureDigSiteChanged, _OnTreasureDigSiteChanged);

            //UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnTreasureAtlasInfoSync, _OnTreasureSyncAtlasInfo);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnWatchTreasureDigSite, _OnWatchTreasureDigSite);
            //UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnOpenTreasureDigSite, _OnOpenTreasureDigSite);

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnTreasureRaffleStart, _OnTreasureRaffleStart);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnTreasureRaffleEnd, _OnTreasureRaffleEnd);

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnTreasureRecordInfoSync, _OnTreasureRecordInfoSync);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnTreasureRecordInfoChanged, _OnTreasureRecordInfoChanged);

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnTreasureAtlasOpen, _OnTreasureAtlasOpen);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnTreasureAtlasClose, _OnTreasureAtlasClose);

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnTreasureItemBuyRes, _OnSyncWorldMallBuySucceed);

            //UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ItemTakeSuccess, _OnTreasureTakeItemCountChanged);
            //UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ItemCountChanged, _OnTreasureItemCountChanged);
        }

        private void _UnBindUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnOpenTreasureDigMap, _OnOpenTreasureDigMap);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnTreasureDigMapOpen, _OnTreasureDigMapOpen);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnTreasureDigMapClose, _OnTreasureDigMapClose);

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnChangeTreasureDigMap, _OnChangeTreasureDigMap);

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnTreasureMapPlayerNumSync, _OnTreasureMapPlayerNumChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnTreasureMapDigReset, _OnTreasureMapDigReset);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnTreasureDigSiteChanged, _OnTreasureDigSiteChanged);

            //UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnTreasureAtlasInfoSync, _OnTreasureSyncAtlasInfo);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnWatchTreasureDigSite, _OnWatchTreasureDigSite);
            //UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnOpenTreasureDigSite, _OnOpenTreasureDigSite);

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnTreasureRaffleStart, _OnTreasureRaffleStart);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnTreasureRaffleEnd, _OnTreasureRaffleEnd);

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnTreasureRecordInfoSync, _OnTreasureRecordInfoSync);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnTreasureRecordInfoChanged, _OnTreasureRecordInfoChanged);

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnTreasureAtlasOpen, _OnTreasureAtlasOpen);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnTreasureAtlasClose, _OnTreasureAtlasClose);

            //快捷购买
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnTreasureItemBuyRes, _OnSyncWorldMallBuySucceed);

            //UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ItemTakeSuccess, _OnTreasureTakeItemCountChanged);
            //UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ItemCountChanged, _OnTreasureItemCountChanged);
        }

        //开始刷新地图
        private void _OnOpenTreasureDigMap(UIEvent uiEvent)
        {
            if (uiEvent == null)
            {
                return;
            }
            var mapModel = uiEvent.Param1 as RandomTreasureMapModel;
            _OpenSelectedRandomTreasureMap(mapModel);

            //关闭大地图选择
            _CloseRandomTreasureAtlas();

            if (mNocache)
            {
                mNocache.CustomActive(false);
            }
        }

        //private void _OnCloseTreasureDigMap(UIEvent uiEvent)
        //{
        //    if (uiEvent == null)
        //    {
        //        return;
        //    }
        //    RandomTreasureMapModel model = uiEvent.Param1 as RandomTreasureMapModel;
        //    if (model != null)
        //    {
        //        RandomTreasureDataManager.GetInstance().ReqCloseTreasureMap(model);
        //    }
        //}

        //地图开启后事件
        private void _OnTreasureDigMapOpen(UIEvent uiEvent)
        {
            if (uiEvent == null)
            {
                return;
            }
            RandomTreasureMapModel model = uiEvent.Param1 as RandomTreasureMapModel;
            if (model != null)
            {
                if (mComMiniMap != null)
                {
                    mComMiniMap.RefreshView(model.mapId);
                }
            }
        }

        //地图关闭后事件
        private void _OnTreasureDigMapClose(UIEvent uiEvent)
        {
            if (uiEvent == null)
            {
                return;
            }
            RandomTreasureMapModel model = uiEvent.Param1 as RandomTreasureMapModel;
            if (model != null)
            {
                RandomTreasureDataManager.GetInstance().ReqCloseTreasureMap(model);
            }
        }

        //从大地图中选择 具体地图 成功切换的消息
        private void _OnChangeTreasureDigMap(UIEvent uiEvent)
        {
            if (uiEvent == null)
            {
                return;
            }

            RandomTreasureMapModel mapModel = uiEvent.Param1 as RandomTreasureMapModel;
            if (mapModel == null)
            {
                return;
            }
            if (mMapFrameObj != null)
            {
                mMapFrameObj.TryChangeTreasureMap(mapModel);
            }
        }

        private void _OnTreasureMapPlayerNumChanged(UIEvent uiEvent)
        {
            if (uiEvent == null)
            {
                return;
            }
            RandomTreasureMapModel mapModel = uiEvent.Param1 as RandomTreasureMapModel;
            if (mMapFrameObj != null)
            {
                mMapFrameObj.ChangeCurrTreasureMapPlayerNum(mapModel);
            }
        }

        private void _OnTreasureMapDigReset(UIEvent uiEvent)
        {
            if (uiEvent == null)
            {
                return;
            }
            RandomTreasureMapModel mapModel = uiEvent.Param1 as RandomTreasureMapModel;
            if (mMapFrameObj != null)
            {
                mMapFrameObj.ResetCurrTreasureMapDig(mapModel);
            }
        }

        private void _OnTreasureDigSiteChanged(UIEvent uiEvent)
        {
            if (uiEvent == null)
            {
                return;
            }
            RandomTreasureMapDigSiteModel mapSiteModel = uiEvent.Param1 as RandomTreasureMapDigSiteModel;
            if (mMapFrameObj != null)
            {
                mMapFrameObj.ChangedCurrTreasureDigSite(mapSiteModel);
            }
        }

        private void _OnWatchTreasureDigSite(UIEvent uiEvent)
        {
            RandomTreasureMapDigSiteModel model = uiEvent.Param1 as RandomTreasureMapDigSiteModel;
            if(model == null)
            {
                return;
            }
            _OpenRandomTreasureRaffle(model);
        }

        private void _OnTreasureRaffleStart(UIEvent uiEvent)
        {
            //抽奖动画 开始时 开启刷新延迟
            bRefreshDigInfoDelay = true;
            bRefreshRecordInfoDelay = true;
            bRefreshItemCountDelay = true;

            canCloseFrame = false;
        }

        private void _OnTreasureRaffleEnd(UIEvent uiEvent)
        {
            if (uiEvent == null)
            {
                Logger.LogError("[RandomTreasureFrame] - _OnTreasureRaffleEnd UIEvent is null");
                return;
            }
            bool needEndRefresh = false;
            if (uiEvent.Param1 != null)
            {
                needEndRefresh = (bool)uiEvent.Param1;
            }
            bool notifyRecord = false;
            if (uiEvent.Param2 != null)
            {
                notifyRecord = (bool)uiEvent.Param2;
            }
            //抽奖动画播放完成，需要刷新，并且关闭抽奖界面
            if (needEndRefresh)
            {
                if (mMapFrameObj != null)
                {
                    bRefreshDigInfoDelay = false;
                    mMapFrameObj.RefreshDigSiteView();                    
                }
            }
            if (notifyRecord)
            {
                _RefreshRecordView();
            }

            _RefreshGoldAndSilverKeyNum();

            canCloseFrame = true;
        }

        private void _OnTreasureRecordInfoChanged(UIEvent uiEvent)
        {
            //刷新是否处于延迟状态，触发一次后释放
            if (bRefreshRecordInfoDelay)
            {
                bRefreshRecordInfoDelay = false;
                return;
            }
            _RefreshRecordView();
        }

        private void _OnTreasureRecordInfoSync(UIEvent uiEvent)
        {
            _RefreshRecordView();
        }

        private void _OnSyncWorldMallBuySucceed(UIEvent uiEvent)
        {
            _RefreshGoldAndSilverKeyNum();
        }

        private void _OnTreasureAtlasOpen(UIEvent uiEvent)
        {
            if (mComMiniMap != null)
            {
                mComMiniMap.CustomActive(false);
            }
        }

        private void _OnTreasureAtlasClose(UIEvent uiEvent)
        {
            if (mComMiniMap != null)
            {
                mComMiniMap.CustomActive(true);
            }
        }

        #endregion

        #endregion

        #region  PUBLIC METHODS

        #endregion

		#region ExtraUIBind
		private Button mCloseBtn = null;
		private RandomTreasureMiniMap mComMiniMap = null;
		private GameObject mRecordRoot = null;
		private GameObject mMapRoot = null;
		private GameObject mAtlasRoot = null;
		private GameObject mRaffleRoot = null;
		private ComUIListScript mRecordScrollList = null;
		private RandomTreasureInfo mGoldenInfo = null;
		private RandomTreasureInfo mSilverInfo = null;
		private Button mAccquireBtn_Golden = null;
		private Button mAccquireBtn_Silver = null;
		private GameObject mNocache = null;
		private Button mMapSelectBtn = null;
		private UIGray mMapSelectBtnGray = null;
		private SetComButtonCD mMapSelectBtnCD = null;
		
		protected override void _bindExUI()
		{
			mCloseBtn = mBind.GetCom<Button>("CloseBtn");
			if (null != mCloseBtn)
			{
				mCloseBtn.onClick.AddListener(_onCloseBtnButtonClick);
			}
			mComMiniMap = mBind.GetCom<RandomTreasureMiniMap>("ComMiniMap");
			mRecordRoot = mBind.GetGameObject("RecordRoot");
			mMapRoot = mBind.GetGameObject("MapRoot");
			mAtlasRoot = mBind.GetGameObject("AtlasRoot");
			mRaffleRoot = mBind.GetGameObject("RaffleRoot");
			mRecordScrollList = mBind.GetCom<ComUIListScript>("RecordScrollList");
			mGoldenInfo = mBind.GetCom<RandomTreasureInfo>("goldenInfo");
			mSilverInfo = mBind.GetCom<RandomTreasureInfo>("silverInfo");
			mAccquireBtn_Golden = mBind.GetCom<Button>("accquireBtn_Golden");
			if (null != mAccquireBtn_Golden)
			{
				mAccquireBtn_Golden.onClick.AddListener(_onAccquireBtn_GoldenButtonClick);
			}
			mAccquireBtn_Silver = mBind.GetCom<Button>("accquireBtn_Silver");
			if (null != mAccquireBtn_Silver)
			{
				mAccquireBtn_Silver.onClick.AddListener(_onAccquireBtn_SilverButtonClick);
			}
			mNocache = mBind.GetGameObject("nocache");
			mMapSelectBtn = mBind.GetCom<Button>("MapSelectBtn");
			if (null != mMapSelectBtn)
			{
				mMapSelectBtn.onClick.AddListener(_onMapSelectBtnButtonClick);
			}
			mMapSelectBtnGray = mBind.GetCom<UIGray>("MapSelectBtnGray");
			mMapSelectBtnCD = mBind.GetCom<SetComButtonCD>("MapSelectBtnCD");
		}
		
		protected override void _unbindExUI()
		{
			if (null != mCloseBtn)
			{
				mCloseBtn.onClick.RemoveListener(_onCloseBtnButtonClick);
			}
			mCloseBtn = null;
			mComMiniMap = null;
			mRecordRoot = null;
			mMapRoot = null;
			mAtlasRoot = null;
			mRaffleRoot = null;
			mRecordScrollList = null;
			mGoldenInfo = null;
			mSilverInfo = null;
			if (null != mAccquireBtn_Golden)
			{
				mAccquireBtn_Golden.onClick.RemoveListener(_onAccquireBtn_GoldenButtonClick);
			}
			mAccquireBtn_Golden = null;
			if (null != mAccquireBtn_Silver)
			{
				mAccquireBtn_Silver.onClick.RemoveListener(_onAccquireBtn_SilverButtonClick);
			}
			mAccquireBtn_Silver = null;
			mNocache = null;
			if (null != mMapSelectBtn)
			{
				mMapSelectBtn.onClick.RemoveListener(_onMapSelectBtnButtonClick);
			}
			mMapSelectBtn = null;
			mMapSelectBtnGray = null;
			mMapSelectBtnCD = null;
		}
		#endregion

        #region Callback
        private void _onCloseBtnButtonClick()
        {
            /* put your code in here */
            if (!canCloseFrame)
            {
                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("randrom_treasure_raffle_anim_playing"));
                return;
            }
            this.Close();
        }
        private void _onAccquireBtn_GoldenButtonClick()
        {
            /* put your code in here */
            ItemComeLink.OnLink(RandomTreasureDataManager.GetInstance().Gold_Treasure_Item_Id, 0, false);
        }
        private void _onAccquireBtn_SilverButtonClick()
        {
            /* put your code in here */
            ItemComeLink.OnLink(RandomTreasureDataManager.GetInstance().Silver_Treasure_Item_Id, 0, false);
        }
        private void _onMapSelectBtnButtonClick()
        {
            if (mMapSelectBtnCD != null && mMapSelectBtnCD.IsBtnWork() == false)
            {
                SystemNotifyManager.SysNotifyTextAnimation(tr_change_map_btn_cd);
                return;
            }
            //开始切换按钮CD
            if (mMapSelectBtnCD != null)
            {
                mMapSelectBtnCD.StartBtCD();
            }

            //TODO
            _TryChangeSuitableTreasureMap();
        }
        #endregion
    }
}
