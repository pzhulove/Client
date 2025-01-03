using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GameClient
{
    public class RandomTreasureMiniMap : MonoBehaviour
    {
        #region Model Params

        private UnityAction onBackBtnClick;

        #endregion
        
        #region View Params

         [SerializeField]
         private GameObject[] mMiniMapSites = null;
         [SerializeField]
         private GameObject mOverlayMask = null;
         [SerializeField]
         private Button mBackBtn = null;
        
        #endregion
        
        #region PRIVATE METHODS
        
        //Unity life cycle
        //void Awake()
        //{
            
        //}
        
        //Unity life cycle
        void Start () 
        {
            if (mBackBtn != null)
            {
                mBackBtn.onClick.AddListener(_OnBackBtnClick);
            }
        }
        
        //Unity life cycle
        //void Update () 
        //{
            
        //}
        
        //Unity life cycle
        void OnDestroy () 
        {
            if (mBackBtn != null)
            {
                mBackBtn.onClick.RemoveListener(_OnBackBtnClick);
            }
            onBackBtnClick = null;
        }

        void _OnBackBtnClick()
        {
            if (onBackBtnClick != null)
            {
                onBackBtnClick();
            }
        }

        #endregion
        
        #region  PUBLIC METHODS

        /// <summary>
        /// 刷新小地图
        /// </summary>
        /// <param name="mapIndex">传入地图序号 从1开始</param>
        public void RefreshView(int mapIndex)
        {
            if (mMiniMapSites == null)
            {
                Logger.LogError("[RandomTreasureMiniMap] - RefreshView mMiniMapSites is null");
                return;
            }
            if (mapIndex > mMiniMapSites.Length + 1 && mapIndex < 1)
            {
                Logger.LogError("[RandomTreasureMiniMap] - RefreshView mapIndex >= mMiniMapSites Length");
                return;
            }

            if (mOverlayMask)
            {
                var overLayRect = mOverlayMask.GetComponent<RectTransform>();
                var mapSiteGo = mMiniMapSites[mapIndex - 1];
                if(mapSiteGo)
                {
                    var mapSiteRect = mapSiteGo.GetComponent<RectTransform>();
                    if (overLayRect && mapSiteRect)
                    {
                        overLayRect.anchoredPosition = mapSiteRect.anchoredPosition;
                    }
                }
            }
        }

        public void BindFuncBtnEvent(UnityAction onFuncBtnClick)
        {
            this.onBackBtnClick = onFuncBtnClick;
        }

        #endregion
    }
}