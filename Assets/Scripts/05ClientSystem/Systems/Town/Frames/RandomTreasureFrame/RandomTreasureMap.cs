using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class RandomTreasureMap
    {
        public const string MAP_FRAME_PATH = "UIFlatten/Prefabs/RandomTreasureFrame/RandomTreasureMap";

        #region Model Params

        private RandomTreasureMapModel mData = null;

        private RandomTreasureFrame mRootFrame = null;
        private string tr_map_player_num = "{0}人";
        
        #endregion
        
        #region View Params

        private ComCommonBind mBind = null;
        private GameObject mParentGo = null;
        private GameObject mThisGo = null;
        private bool bShowed = false;

        //New add
        private Image mMapBgImg = null;
        private RandomTreasureInfo mPlayerInfo = null;
        private Text mName = null;


        //private Image mMapRouteImg = null;
        private Dictionary<int, ComRandomTreasureMapSite> mRandomTreasureMapSiteDic = new Dictionary<int, ComRandomTreasureMapSite>();
        private ComRandomTreasureMapSite mCurrentMapSite = null;

        private UnityEngine.Coroutine waitToInitComMapSites = null;

        #endregion
        
        #region PRIVATE METHODS

        private void _ClearData()
        {
            mRootFrame = null;
            mBind = null;
            mParentGo = null;
            mParentGo = null;
            bShowed = false;

            //New add
            mMapBgImg = null;
            mPlayerInfo = null;
            mName = null;

            //mMapRouteImg = null;
            if (mRandomTreasureMapSiteDic != null)
            {
                mRandomTreasureMapSiteDic.Clear();
                mRandomTreasureMapSiteDic = null;
            }
            mCurrentMapSite = null;

            mData = null;
            tr_map_player_num = "";

            if (waitToInitComMapSites != null)
            {
                GameFrameWork.instance.StopCoroutine(waitToInitComMapSites);
                waitToInitComMapSites = null;
            }
        }

        private void _InitFrame()
        {
            tr_map_player_num = TR.Value("random_treasure_map_player_num");

            //默认隐藏背景 防止初始化有白底图闪现
            if (mMapBgImg)
            {
                mMapBgImg.CustomActive(false);
            }

            if (waitToInitComMapSites != null)
            {
                GameFrameWork.instance.StopCoroutine(waitToInitComMapSites);
            }
            waitToInitComMapSites = GameFrameWork.instance.StartCoroutine(_WaitToInitComMapSites());
        }

        IEnumerator _WaitToInitComMapSites()
        {
            if (mRandomTreasureMapSiteDic == null)
            {
                mRandomTreasureMapSiteDic = new Dictionary<int, ComRandomTreasureMapSite>();
            }
            else
            {
                mRandomTreasureMapSiteDic.Clear();
            }
            var totalMapModelList = RandomTreasureDataManager.GetInstance().GetTotalMapModelList();
            if (totalMapModelList == null)
            {
                yield break;
            }
            for (int i = 0; i < totalMapModelList.Count; i++)
            {
                var mapModel = totalMapModelList[i];
                if (mapModel == null)
                {
                    continue;
                }
                int mapId = mapModel.mapId;
                if (mapModel.localMapData == null)
                {
                    continue;
                }
                string localRoutePath = mapModel.localMapData.MapRouteResPath;
                if (mThisGo)
                {
                    ComRandomTreasureMapSite comMapSite = Utility.GetComponetInChild<ComRandomTreasureMapSite>(mThisGo, localRoutePath);
                    if (comMapSite == null)
                    {
                        continue;
                    }
                    if (mRandomTreasureMapSiteDic.ContainsKey(mapId))
                    {
                        mRandomTreasureMapSiteDic[mapId] = comMapSite;
                    }
                    else
                    {
                        mRandomTreasureMapSiteDic.Add(mapId, comMapSite);
                    }
                    comMapSite.InitView();
                }
                yield return new WaitForEndOfFrame();
            }
            yield return null;
        }

        private void SetMapBackground()
        {
            if (mData == null)
            {
                return;
            }
            var mapTable = mData.localMapData;
            if (mapTable != null)
            {
                string imgPath = mapTable.MapResPath;
                if (!string.IsNullOrEmpty(imgPath))
                {
                    ETCImageLoader.LoadSprite(ref mMapBgImg, imgPath);
                    if (mMapBgImg)
                    {
                        mMapBgImg.CustomActive(true);
                    }
                }
                else
                {
                    if (mMapBgImg)
                    {
                        mMapBgImg.CustomActive(false);
                    }
                }
            }
        }

        //private void SetMapRouteImage()
        //{
        //    if (mData == null)
        //    {
        //        return;
        //    }
        //    var mapTable = mData.localMapData;
        //    if (mapTable != null)
        //    {
        //        string imgPath = mapTable.MapRouteResPath;
        //        if (!string.IsNullOrEmpty(imgPath))
        //        {
        //            ETCImageLoader.LoadSprite(ref mMapRouteImg, imgPath);
        //        }
        //        else
        //        {
        //            if (mMapRouteImg)
        //            {
        //                mMapRouteImg.CustomActive(false);
        //            }
        //        }
        //    }
        //}

        private void SetPlayerInfo()
        {
            if (mData == null)
            {
                return;
            }
            if (mPlayerInfo != null)
            {
                mPlayerInfo.SetInfoContent(string.Format(tr_map_player_num, mData.beInPlayerNum.ToString()));
            }
        }

        private void SetTitleName()
        {
            if (mData == null)
            {
                return;
            }
            var mapTable = mData.localMapData;
            if (mapTable != null)
            {
                if (mName)
                {
                    mName.text = mapTable.Name;
                }
            }
        }

        private void SetAllMapRoutesInActive()
        {
            if (mRandomTreasureMapSiteDic == null)
            {
                Logger.LogError("[RandomTreasureMap] - SetAllMapRoutesInActive mRandomTreasureMapSiteDic is null");
                return;
            }

            var enumerator = mRandomTreasureMapSiteDic.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var site = enumerator.Current.Value as ComRandomTreasureMapSite;
                if (site != null)
                {
                    site.CustomActive(false);
                }
            }
        }

        #endregion
        
        #region  PUBLIC METHODS

        public void Create(GameObject parent, RandomTreasureFrame frame)
        {
            this.mParentGo = parent;
            this.mRootFrame = frame;

            if (mThisGo == null)
            {
                mThisGo = AssetLoader.GetInstance().LoadResAsGameObject(MAP_FRAME_PATH);
            }
            if (mThisGo)
            {
                mBind = mThisGo.GetComponent<ComCommonBind>();
            }
            if (mBind != null)
            {
                mMapBgImg = mBind.GetCom<Image>("MapBgImg");
                mPlayerInfo = mBind.GetCom<RandomTreasureInfo>("PlayerInfo");
                //mName = mBind.GetCom<Text>("Name");

                //mMapRouteImg = mBind.GetCom<Image>("MapRouteImg");
                //mRandomTreasureMapSite = mBind.GetCom<ComRandomTreasureMapSite>("RandomTreasureMapSite");
            }
            Utility.AttachTo(mThisGo, mParentGo);

            //默认显示
            if (mThisGo)
            {
                mThisGo.CustomActive(false);
            }

            _InitFrame();
        }

        public void Destroy()
        {
            _ClearData();
        }

        public void ChangeCurrTreasureMapPlayerNum(RandomTreasureMapModel mapModel)
        {
            if (mData == null || mapModel == null)
            {
                return;
            }
            if (mData.mapId == mapModel.mapId)
            {
                mData.beInPlayerNum = mapModel.beInPlayerNum;
                SetPlayerInfo();
            }
        }

        /// <summary>
        /// 重置地图
        /// </summary>
        /// <param name="mapModel"></param>
        public void ResetCurrTreasureMapDig(RandomTreasureMapModel mapModel)
        {
            if (mData == null || mapModel == null)
            {
                return;
            }
            if (mData.mapId == mapModel.mapId)
            {
                RefreshData(mapModel);
            }
        }

        public void RefreshData(RandomTreasureMapModel mapModel)
        {
            if (mapModel == null)
            {
                Logger.LogError("[RandomTreasureMap] - RefreshData out data is null");
                return;
            }
            if (mRandomTreasureMapSiteDic == null)
            {
                Logger.LogError("[RandomTreasureMap] - RefreshData mRandomTreasureMapSiteDic is null");
                return;
            }

            this.mData = mapModel;

            //刷新地图点
            SetAllMapRoutesInActive();
            if (mRandomTreasureMapSiteDic.ContainsKey(mapModel.mapId))
            {
                ComRandomTreasureMapSite tempComMapSite = mRandomTreasureMapSiteDic[mapModel.mapId];
                if (tempComMapSite != null)
                {
                    tempComMapSite.CustomActive(true);
                    if (mRootFrame != null && !mRootFrame.BRefreshDigInfoDelay)
                    {
                        tempComMapSite.RefreshMapSite(mapModel);
                    }
                    mCurrentMapSite = tempComMapSite;
                }
            }

            //刷新地图其他数据
            SetMapBackground();
            //SetMapRouteImage();
            SetPlayerInfo();
            SetTitleName();
        }

        /// <summary>
        /// 尝试切换地图
        /// </summary>
        public void TryChangeTreasureMap(RandomTreasureMapModel mapModel)
        {
            if (mapModel == null)
            {
                return;
            }
            if (this.mData != null)
            {
                RandomTreasureDataManager.GetInstance().ReqCloseTreasureMap(this.mData);
            }
            if (mapModel != null)
            {
                RandomTreasureDataManager.GetInstance().ReqOpenTreasureMap(mapModel);
            }
        }

        public RandomTreasureMapModel GetCurrentMapModel()
        {
            return mData;
        }

        public void ChangedCurrTreasureDigSite(RandomTreasureMapDigSiteModel mapSiteModel)
        {
            if (mData == null || mapSiteModel == null)
            {
                return;
            }
            var mDataMapSiteData = mData.mapTotalDigSites;
            if (mDataMapSiteData == null)
            {
                Logger.LogError("[RandomTreasureMap] - _OnTreasureDigSiteChanged mData.mapTotalDigSites is null");
                return;
            }

            for (int i = 0; i < mDataMapSiteData.Count; i++)
            {
                if (mDataMapSiteData[i].mapId == mapSiteModel.mapId &&
                    mDataMapSiteData[i].index == mapSiteModel.index)
                {
                    if (mCurrentMapSite != null)
                    {
                        if(mRootFrame != null && mRootFrame.BRefreshDigInfoDelay)
                        {
                            return;
                        }
                        mCurrentMapSite.RefreshMapSite(mapSiteModel);
                    }
                    break;
                }
            }
        }

        public void RefreshDigSiteView(RandomTreasureMapDigSiteModel digSiteModel)
        {
            if (digSiteModel == null)
            {
                Logger.LogError("[RandomTreasureMap] - RefreshDigSiteView out data is null");
                return;
            }
            if (mCurrentMapSite == null)
            {
                Logger.LogError("[RandomTreasureMap] - RefreshDigSiteView mCurrentMapSite is null");
                return;
            }
            if (mRootFrame != null && mRootFrame.BRefreshDigInfoDelay)
            {
                return;
            }
            mCurrentMapSite.RefreshMapSite(digSiteModel);
        }

        public void RefreshDigSiteView()
        {
            if (this.mData == null)
            {
                return;
            }
            if (mCurrentMapSite == null)
            {
                return;
            }
            if (mRootFrame != null && mRootFrame.BRefreshDigInfoDelay)
            {
                return;
            }
            mCurrentMapSite.RefreshMapSite(this.mData);
        }

        public void Show()
        {
            //if (bShowed)
            //{
            //    return;
            //}
            if (mThisGo)
            {
                mThisGo.CustomActive(true);
            }
            bShowed = true;
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnTreasureDigMapOpen, this.mData);
        }

        public void Hide()
        {
            //if (!bShowed)
            //{
            //    return;
            //}
            if (mThisGo)
            {
                mThisGo.CustomActive(false);
            }
            bShowed = false;
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnTreasureDigMapClose, this.mData);
        }

        public bool IsShowed()
        {
            return bShowed;
        }

        #endregion
    }
}