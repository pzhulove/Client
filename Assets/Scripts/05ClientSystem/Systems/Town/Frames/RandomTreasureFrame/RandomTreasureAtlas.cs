using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class RandomTreasureAtlas : ClientFrame
    {
        public const string SELECT_SHOW_MAP_RES_PATH = "UIFlatten/Prefabs/RandomTreasureFrame/RandomTreasureSelectMap";
		public const string ATLET_FRAME_PATH = "UIFlatten/Prefabs/RandomTreasureFrame/RandomTreasureAtlas";

        #region Model Params

        List<RandomTreasureMapModel> mapModelList = null;
        RandomTreasureMapModel mThisMapModel = null;

        #endregion
        
        #region View Params

        List<ComRandomTreasureSelectMap> mRandomTreasureSelectMapList = new List<ComRandomTreasureSelectMap>();
        private ComRandomTreasureSelectMap mThisSelectMap = null;
        #endregion
        
        #region PRIVATE METHODS

        protected override void _OnOpenFrame()
        {
            if (userData != null)
            {
                mThisMapModel = userData as RandomTreasureMapModel;
            }

            _BindUIEvent();
            _InitView();

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnTreasureAtlasOpen);
        }

        protected override void _OnCloseFrame()
        {
            _ClearData();
            _UnBindUIEvent();

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnTreasureAtlasClose);
        }

        public override string GetPrefabPath()
        {
            return ATLET_FRAME_PATH;
        }

        private void _BindUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnTreasureAtlasInfoSync, _OnTreasureSyncAtlasInfo);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnChangeTreasureDigSelectMap, _OnChangeTreasureDigSelectMap);
        }

        private void _UnBindUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnTreasureAtlasInfoSync, _OnTreasureSyncAtlasInfo);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnChangeTreasureDigSelectMap, _OnChangeTreasureDigSelectMap);
        }

        private void _InitView()
        {
            mapModelList = RandomTreasureDataManager.GetInstance().GetTotalMapModelList();
            if (mapModelList == null)
            {
                return;
            }
            if (mRandomTreasureSelectMapList == null)
            {
                mRandomTreasureSelectMapList = new List<ComRandomTreasureSelectMap>();
            }
            else
            {
                mRandomTreasureSelectMapList.Clear();
            }
            for (int i = 0; i < mapModelList.Count; i++)
            {
                var mapModel = mapModelList[i];
                if (mapModel == null)
                {
                    Logger.LogErrorFormat("[RandomTreasureAtlas] - _InitView mapModel is null, index is {0}", i);
                    continue;
                }
                GameObject selectMapGo = AssetLoader.GetInstance().LoadResAsGameObject(SELECT_SHOW_MAP_RES_PATH);
                Utility.AttachTo(selectMapGo, mContent);
                if (selectMapGo == null)
                {
                    continue;
                }
                ComRandomTreasureSelectMap selectMap = selectMapGo.GetComponent<ComRandomTreasureSelectMap>();
                if (mRandomTreasureSelectMapList != null && selectMap != null)
                {
                    mRandomTreasureSelectMapList.Add(selectMap);
                }
            }

            //if (mComUIFade != null)
            //{
            //    mComUIFade.InitView();
            //}

            if (mSelectMapMask)
            {
                mSelectMapMask.transform.SetAsLastSibling();
            }
        }

        private void _RefreshData()
        {
            mapModelList = RandomTreasureDataManager.GetInstance().GetTotalMapModelList();
            if (mapModelList == null)
            {
                return;
            }
            if (mRandomTreasureSelectMapList == null)
            {
                return;
            }
            int comSelectMapListCount = mRandomTreasureSelectMapList.Count;
            int mapModelListCount = mapModelList.Count;
            if (comSelectMapListCount != mapModelListCount)
            {
                Logger.LogErrorFormat("[RandomTreasureAtlas] - RefreshData selectMap count is {0}, mapModel count is {1}", comSelectMapListCount, mapModelListCount);
                return;
            }
            for (int i = 0; i < mapModelListCount; i++)
            {
                var mapModel = mapModelList[i];
                var selectMap = mRandomTreasureSelectMapList[i];
                if (selectMap == null)
                {
                    continue;
                }
                if (selectMap != null)
                {
                    selectMap.RefreshView(mapModel);
                }
            }
        }

        private void _ClearData()
        {
            if (mapModelList != null)
            {
                mapModelList.Clear();
                mapModelList = null;
            }
            mThisMapModel = null;

            if (mRandomTreasureSelectMapList != null)
            {
                for (int i = 0; i < mRandomTreasureSelectMapList.Count; i++)
                {
                    //GameObject.Destroy(mRandomTreasureSelectMapList[i].gameObject);
                    var selectMap = mRandomTreasureSelectMapList[i];
                    if (selectMap != null)
                    {
                        selectMap.ReleaseThisMapSelect();
                    }
                }
                mRandomTreasureSelectMapList.Clear();
            }
            mThisSelectMap = null;
        }

        private void _ResetSelectMaskPos(bool bShow)
        {
            if (mSelectMapMask)
            {
                if (bShow == false)
                {
                    mSelectMapMask.CustomActive(false);
                    return;
                }
                if (mThisSelectMap == null)
                {
                    return;
                }
                var maskRect = mSelectMapMask.GetComponent<RectTransform>();
                var selectMapRect = this.mThisSelectMap.gameObject.GetComponent<RectTransform>();
                if (maskRect == null || selectMapRect == null)
                {
                    return;
                }
                maskRect.anchoredPosition = selectMapRect.anchoredPosition;
                mSelectMapMask.CustomActive(true);
            }
        }

        private void _ResetSelectMaskEffect(bool bShow)
        {
            if (mSelectMapMaskEffUI)
            {
                if (bShow)
                {
                    mSelectMapMaskEffUI.CustomActive(false);
                    mSelectMapMaskEffUI.CustomActive(true);
                }
                else {
                    mSelectMapMaskEffUI.CustomActive(false);
                }                
            }
        }

        private void _TrySetChangeSelectMap(ComRandomTreasureSelectMap selectMap)
        {
            if (selectMap == null)
            {
                return;
            }
            this.mThisSelectMap = selectMap;
            _ResetSelectMaskPos(true);
            //_ResetSelectMaskEffect(true);
        }

        private void _TrySetChangeSelectMap()
        {
            if (mRandomTreasureSelectMapList == null)
            {
                return;
            }
            //刷新当前大地图选中的状态 //只针对第一个地图，只在第一次打开当前界面时，将第一个地图赋值给当前地图缓存
            if (this.mThisSelectMap == null && this.mThisMapModel != null)
            {
                for (int i = 0; i < mRandomTreasureSelectMapList.Count; i++)
                {
                    var selectMap = mRandomTreasureSelectMapList[i];
                    if (selectMap == null)
                    {
                        continue;
                    }
                    if (selectMap.GetCurrMapModel() == null)
                    {
                        continue;
                    }
                    if (selectMap.GetCurrMapModel().mapId == mThisMapModel.mapId)
                    {
                        this.mThisSelectMap = selectMap;
                        break;
                    }
                }
            }
            _ResetSelectMaskPos(true);
            //_ResetSelectMaskEffect(false);
        }

        #region Callback

        private void _OnTreasureSyncAtlasInfo(UIEvent uiEvent)
        {
            _RefreshData();
            _TrySetChangeSelectMap();
        }

        //从大地图中选择 具体地图 成功切换的消息
        private void _OnChangeTreasureDigSelectMap(UIEvent uiEvent)
        {
            if (uiEvent == null)
            {
                return;
            }
            ComRandomTreasureSelectMap selectMap = uiEvent.Param1 as ComRandomTreasureSelectMap;
            if (selectMap == null)
            {
                return;
            }
            _TrySetChangeSelectMap(selectMap);
        }

        #endregion

        #endregion

        #region  PUBLIC METHODS

        //public void Show(bool bDelay = false)
        //{
        //    //如果当前界面未显示
        //    if (bShowed)
        //    {
        //        return;
        //    }

        //    RandomTreasureDataManager.GetInstance().ReqTotalAtlasInfo();
        //    if (mComUIFade)
        //    {
        //        mComUIFade.StartOpenAtlas(
        //        () =>
        //        {
        //            if (mThisGo)
        //            {
        //                mThisGo.CustomActive(true);
        //            }
        //            if (mContent)
        //            {
        //                mContent.CustomActive(true);
        //            }
        //        },
        //        () =>
        //        {
        //            if (mMaskClose)
        //            {
        //                mMaskClose.enabled = true;
        //            }
        //            bShowed = true;
        //            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnTreasureAtlasOpen);
        //        }, bDelay);
        //    }
        //}

        //public void Hide(bool bDelay = false)
        //{
        //    //如果当前界面未显示
        //    if (!bShowed)
        //    {
        //        return;
        //    }

        //    if (mComUIFade)
        //    {
        //        mComUIFade.StartCloseAtlas(
        //        () =>
        //        {
        //            if (mMaskClose)
        //            {
        //                mMaskClose.enabled = false;
        //            }
        //            //_ResetSelectMaskEffect(false);
        //            //_ResetSelectMaskPos(false);
        //        },
        //        () =>
        //        {
        //            if (mContent)
        //            {
        //                mContent.CustomActive(false);
        //            }
        //            if (mThisGo)
        //            {
        //                mThisGo.CustomActive(false);
        //            }
        //            bShowed = false;
        //            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnTreasureAtlasClose);
        //        }, bDelay);
        //    }
        //}

        #endregion

        #region ExtraUIBind
        private Button mMaskClose = null;
        private ComRandomTreasureUIFade mComUIFade = null;
        private GameObject mContent = null;
        private GameObject mSelectMapMask = null;
        private GameObject mSelectMapMaskEffUI = null;
        private Button mBtnClose = null;

        private Button mParentBlackMask = null;

        protected override void _bindExUI()
        {
            mMaskClose = mBind.GetCom<Button>("MaskClose");
            if (null != mMaskClose)
            {
                mMaskClose.onClick.AddListener(_onMaskCloseButtonClick);
            }
            mComUIFade = mBind.GetCom<ComRandomTreasureUIFade>("ComUIFade");
            mContent = mBind.GetGameObject("Content");
            mSelectMapMask = mBind.GetGameObject("SelectMapMask");
            mSelectMapMaskEffUI = mBind.GetGameObject("SelectMapMaskEffUI");
            mBtnClose = mBind.GetCom<Button>("BtnClose");
            if (null != mBtnClose)
            {
                mBtnClose.onClick.AddListener(_onBtnCloseButtonClick);
            }

            if (blackMask != null)
            {
                mParentBlackMask = blackMask.GetComponent<Button>();
                if (mParentBlackMask)
                {
                    mParentBlackMask.onClick.AddListener(_onBtnParentMaskClick);
                }
            }
        }

        protected override void _unbindExUI()
        {
            if (null != mMaskClose)
            {
                mMaskClose.onClick.RemoveListener(_onMaskCloseButtonClick);
            }
            mMaskClose = null;
            mComUIFade = null;
            mContent = null;
            mSelectMapMask = null;
            mSelectMapMaskEffUI = null;
            if (null != mBtnClose)
            {
                mBtnClose.onClick.RemoveListener(_onBtnCloseButtonClick);
            }
            mBtnClose = null;

            if (null != mParentBlackMask)
            {
                mParentBlackMask.onClick.RemoveListener(_onBtnParentMaskClick);
            }
            mParentBlackMask = null;
        }
        #endregion

        #region Callback
        private void _onMaskCloseButtonClick()
        {
            /* put your code in here */
            this.Close();
        }
        private void _onBtnCloseButtonClick()
        {
            /* put your code in here */
            //this.Close();
        }

        private void _onBtnParentMaskClick()
        {
            if (this.GetState() == EFrameState.Open)
            {
                this.Close();
            }
        }
        #endregion
    }
}