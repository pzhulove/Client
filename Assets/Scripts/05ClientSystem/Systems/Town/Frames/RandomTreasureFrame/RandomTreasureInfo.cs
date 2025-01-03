using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class RandomTreasureInfo : MonoBehaviour
    {
        #region Model Params

        private ComItem comItem = null;

        public delegate void TitleBtnClickHandle();
        public TitleBtnClickHandle onTitleBtnClick;

        #endregion
        
        #region View Params

        [SerializeField]
        private Image mInfoTitleImg = null;
        [SerializeField]
        private Button mInfoTitleImgBtn = null;
        [SerializeField]
        private Text mInfoContent = null;
        
        #endregion
        
        #region PRIVATE METHODS
        
        //Unity life cycle
        //void Awake()
        //{
            
        //}
        
        //Unity life cycle
        void Start () 
        {
            if (mInfoTitleImgBtn)
            {
                mInfoTitleImgBtn.onClick.AddListener(_OnTitleBtnClick);
            }
        }
        
        //Unity life cycle
        //void Update () 
        //{
            
        //}
        
        //Unity life cycle
        void OnDestroy () 
        {
            if (comItem != null)
            {
                ComItemManager.Destroy(comItem);
                comItem = null;
            }

            if (mInfoTitleImgBtn)
            {
                mInfoTitleImgBtn.onClick.RemoveListener(_OnTitleBtnClick);
            }

            onTitleBtnClick = null;
        }

        void _OnTitleBtnClick()
        {
            if (onTitleBtnClick != null)
            {
                onTitleBtnClick();
            }
        }

        #endregion
        
        #region  PUBLIC METHODS

        public void SetInfoContent(string content)
        {
            if (mInfoContent)
            {
                mInfoContent.text = content;
            }
        }

        public void SetInfoTitleImg(string resPath)
        {
            if (mInfoTitleImg && !string.IsNullOrEmpty(resPath))
            {
                ETCImageLoader.LoadSprite(ref mInfoTitleImg, resPath);
            }
        }

        public void SetInfoImgWithItem(ItemData itemData)
        {
            if (comItem == null && mInfoTitleImg)
            {
                comItem = ComItemManager.Create(mInfoTitleImg.gameObject);
                comItem.Setup(itemData, Utility.OnItemClicked);
            }
        }
        
        #endregion
    }
}