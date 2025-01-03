using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class ComRandomTreasureSelectMap : MonoBehaviour
    {        
        #region Model Params

        RandomTreasureMapModel mapModel = null;
        private bool bSelected = false;

        #endregion
        
        #region View Params

        [SerializeField]
        private Image mBgImg = null;
         [SerializeField]
        private Button mFuncBtn = null;
         [SerializeField]
         private Text mTitleName = null;
         [SerializeField]
         private RandomTreasureInfo mGoldeninfo = null;
         [SerializeField]
         private RandomTreasureInfo mSilverinfo = null;
        
        #endregion
        
        #region PRIVATE METHODS
        
        //Unity life cycle
        //void Awake()
        //{
            
        //}
        
        //Unity life cycle
        void Start () 
        {
            if(mFuncBtn)
            {
                mFuncBtn.onClick.AddListener(OnSelectMapBtnClick);
            }

            //放置加载白屏
            if (mBgImg)
            {
                mBgImg.color = new Color(1,1,1,0);
            }
        }
        
        //Unity life cycle
        //void Update () 
        //{
            
        //}
        
        //Unity life cycle
        void OnDestroy () 
        {
            if(mFuncBtn)
            {
                mFuncBtn.onClick.RemoveListener(OnSelectMapBtnClick);
            }
            mapModel = null;
            bSelected = false;
        }

        void OnSelectMapBtnClick()
        {
            if (mapModel != null && bSelected == false)
            {
                //先触发切换地图消息 ！！！  用于关闭前一张地图
                bSelected = true;
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnChangeTreasureDigSelectMap, this);
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnChangeTreasureDigMap, mapModel);
            }
        }

        void SetBackground()
        {
            if (this.mapModel == null)
            {
                return;
            }
            var mapTable = mapModel.localMapData;
            if (mapTable != null)
            {
                string imgPath = mapTable.AtlasResPath;
                ETCImageLoader.LoadSprite(ref mBgImg, imgPath);
            }
            //放置加载白屏
            if (mBgImg)
            {
                mBgImg.color = new Color(1, 1, 1, 1);
            }
        }

        void SetTitleName()
        {
            if (this.mapModel == null)
            {
                return;
            }
            var mapTable = mapModel.localMapData;
            if (mapTable != null)
            {
                if (mTitleName)
                {
                    mTitleName.text = mapTable.Name;
                }
            }
        }

        void SetGoldTreasureInfo()
        {
            if (this.mapModel == null)
            {
                return;
            }
            if (mGoldeninfo != null)
            {
                mGoldeninfo.SetInfoContent(mapModel.goldSiteNum.ToString());
            }
        }

        void SetSilverTreasureInfo()
        {
            if (this.mapModel == null)
            {
                return;
            }
            if (mSilverinfo != null)
            {
                mSilverinfo.SetInfoContent(mapModel.silverSiteNum.ToString());
            }
        }
        
        #endregion
        
        #region  PUBLIC METHODS

        public void RefreshView(RandomTreasureMapModel model)
        {
            this.mapModel = model;
            SetBackground();
            SetTitleName();
            SetGoldTreasureInfo();
            SetSilverTreasureInfo();
        }

        public RandomTreasureMapModel GetCurrMapModel()
        {
            return this.mapModel;
        }

        public bool IsSelected()
        {
            return bSelected;
        }

        public void ReleaseThisMapSelect()
        {
            bSelected = false;
        }

        #endregion
    }
}