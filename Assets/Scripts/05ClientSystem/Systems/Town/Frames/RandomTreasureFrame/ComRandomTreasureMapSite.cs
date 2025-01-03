 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class ComRandomTreasureMapSite : MonoBehaviour
    {
        public const string SITE_BUTTON_RES_PATH = "UIFlatten/Prefabs/RandomTreasureFrame/RandomTreasureSiteBtn";
        
        #region Model Params

        private bool bInited = false;

        #endregion
        
        #region View Params

        [SerializeField]
        private GameObject[] mDigSitePositions = null;

        private List<ComRandomTreasureSiteBtn> mDigSiteBtnList = new List<ComRandomTreasureSiteBtn>();
        
        #endregion
        
        #region PRIVATE METHODS
        
        //Unity life cycle
        //void Awake()
        //{
            
        //}
        
        //Unity life cycle
        //void Start () 
        //{

        //}
        
        //Unity life cycle
        //void Update () 
        //{
            
        //}
        
        //Unity life cycle
        void OnDestroy()
        {
            if (mDigSiteBtnList != null)
            {
                for (int i = 0; i < mDigSiteBtnList.Count; i++)
                {
                    GameObject.Destroy(mDigSiteBtnList[i].gameObject);
                }
                mDigSiteBtnList.Clear();
                mDigSiteBtnList = null;
            }

            bInited = false;
        }

        private bool _CheckMapSitePosMatchBtnCount()
        {
            bool bMatch = false;
            if (mDigSiteBtnList == null)
            {
                Logger.LogError("[ComRandomTreasureMapSite] - RefreshMapSite mDigSiteBtnList is null");
                return bMatch;
            }
            if (mDigSitePositions == null)
            {
                Logger.LogError("[ComRandomTreasureMapSite] - RefreshMapSite mDigSitePositions is null");
                return bMatch;
            }
            if (mDigSitePositions.Length != mDigSiteBtnList.Count)
            {
                Logger.LogError("[ComRandomTreasureMapSite] - Start mDigSitePositions.Length != mDigSiteBtnList.Count");
                return bMatch;
            }
            return true;
        }
        
        #endregion
        
        #region  PUBLIC METHODS

        public void InitView()
        {
		    if (bInited)
            {
                return;
            }
            if (mDigSitePositions != null)
            {
                if (mDigSiteBtnList != null)
                {
                    mDigSiteBtnList.Clear();
                }

                for (int i = 0; i < mDigSitePositions.Length; i++)
                {
                    mDigSitePositions[i].CustomActive(false);
                    GameObject siteGo = AssetLoader.GetInstance().LoadResAsGameObject(SITE_BUTTON_RES_PATH);
                    Utility.AttachTo(siteGo, mDigSitePositions[i]);
                    if (siteGo)
                    {
                        siteGo.transform.localPosition = Vector3.zero;
                    }
                    if (siteGo == null)
                    {
                        continue;
                    }
                    ComRandomTreasureSiteBtn bind = siteGo.GetComponent<ComRandomTreasureSiteBtn>();
                    if (bind != null && mDigSiteBtnList != null)
                    {
                        mDigSiteBtnList.Add(bind);
                    }
                }

                if (mDigSitePositions.Length != mDigSiteBtnList.Count)
                {
                    Logger.LogError("[ComRandomTreasureMapSite] - Start mDigSitePositions.Length != mDigSiteBtnList.Count");
                }
            }
            this.CustomActive(false);
            bInited = true;
        }

        /// <summary>
        /// 刷新当前地图单个位置
        /// </summary>
        /// <param name="mapSiteModel"></param>
        public void RefreshMapSite(RandomTreasureMapDigSiteModel mapSiteModel)
        {
            if (mapSiteModel == null)
            {
                Logger.LogError("[ComRandomTreasureMapSite] - RefreshMapSite mapSiteModel is null");
                return;
            }
            if (_CheckMapSitePosMatchBtnCount() == false)
            {
                return;
            }
            GameObject root = null;
            int digSiteModelIndex = mapSiteModel.index;
            if (digSiteModelIndex < mDigSitePositions.Length && digSiteModelIndex < mDigSiteBtnList.Count)
            {
                root = mDigSitePositions[digSiteModelIndex];
                mDigSiteBtnList[digSiteModelIndex].Refresh(mapSiteModel, root);
            }
        }

        /// <summary>
        /// 刷新当前地图全部位置
        /// </summary>
        /// <param name="mapModels"></param>
        public void RefreshMapSite(RandomTreasureMapModel mapModels)
        {
            if (mapModels == null || mapModels.mapTotalDigSites == null)
            {
                Logger.LogError("[ComRandomTreasureMapSite] - RefreshMapSite mapModel is null");
                return;
            }
            if (_CheckMapSitePosMatchBtnCount() == false)
            {
                return;
            }
            //if (mapModels.mapTotalDigSites.Count > mDigSitePositions.Length)
            //{
            //    Logger.LogErrorFormat("[ComRandomTreasureMapSite] - RefreshMapSite mapModel count {0} > sitePosCount {1}", mapModels.mapTotalDigSites.Count, mDigSitePositions.Length);
            //    return;
            //}

            for (int i = 0; i < mapModels.mapTotalDigSites.Count; i++)
            {
                var digSiteModel = mapModels.mapTotalDigSites[i];
                if (digSiteModel == null)
                {
                    continue;
                }
                GameObject root = null;
                int digSiteModelIndex = digSiteModel.index;
                if (digSiteModelIndex < mDigSitePositions.Length && digSiteModelIndex < mDigSiteBtnList.Count)
                {
                    root = mDigSitePositions[digSiteModelIndex];
                    mDigSiteBtnList[digSiteModelIndex].Refresh(digSiteModel, root);
                }
            }
        }
        
        #endregion


#if UNITY_EDITOR

        public void DebugInitView()
        {
            DestroyDebugGO();
            if (mDigSitePositions != null)
            {
                for (int i = 0; i < mDigSitePositions.Length; i++)
                {
                    GameObject siteGo = AssetLoader.GetInstance().LoadResAsGameObject(SITE_BUTTON_RES_PATH);
                    if (siteGo)
                    {
                        siteGo.name = "Debug_GO_"+i.ToString();
                    }
                    Utility.AttachTo(siteGo, mDigSitePositions[i]);
                }
            }
        }

        public void DestroyDebugGO()
        {
            if (mDigSitePositions != null)
            {
                for (int i = 0; i < mDigSitePositions.Length; i++)
                {
                    GameObject posGo = mDigSitePositions[i];
                    if (posGo && posGo.transform.childCount > 0)
                    {
                        GameObject child = posGo.transform.GetChild(0).gameObject;
                        if (child)
                        {
                            GameObject.DestroyImmediate(child);
                        }
                    }
                }
            }
        }

#endif
    }
}