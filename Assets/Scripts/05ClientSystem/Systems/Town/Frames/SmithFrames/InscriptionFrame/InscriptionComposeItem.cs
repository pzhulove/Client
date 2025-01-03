using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;
using UnityEngine.Assertions;
using Protocol;
using ProtoTable;
using DG.Tweening;


namespace GameClient
{
    // 用于显示在合成区的铭文item
    public class InscriptionComposeItem : MonoBehaviour
    {
        [SerializeField]private Image backGround;
        [SerializeField]private Image icon;
        [SerializeField]private GameObject lockedRoot;
        [SerializeField]private Button close;

        private ItemData itemData;

        private void Awake()
        {
            if (close != null)
            {
                close.onClick.RemoveAllListeners();
                close.onClick.AddListener(OnCloseClick);
            }
        }

        private void OnDestroy()
        {
            itemData = null;
        }

        private void SetBackGround()
        {
            if (backGround != null)
            {
                backGround.color = itemData.GetQualityInfo().Col;
            }
        }

        private void SetIcon()
        {
            if (icon != null)
            {
                ETCImageLoader.LoadSprite(ref icon, itemData.Icon);
                icon.CustomActive(true);
            }
        }

        private void SetCloseBtnState(bool isFlag)
        {
            close.CustomActive(isFlag);
        }

        private void SetIconState(bool isFlag)
        {
            icon.CustomActive(isFlag);
        }

        private void SetLockedRootState(bool isFlag)
        {
            lockedRoot.CustomActive(isFlag);
        }

        private void SetBackGroudState(bool isFlag)
        {
            backGround.CustomActive(isFlag);
        }

        public void SetUp(ItemData itemData)
        {
            this.itemData = itemData;

            if (this.itemData != null)
            {
                SetBackGround();
                SetIcon();
                SetBackGroudState(true);
                SetCloseBtnState(true);
            }
            else
            {
                SetBackGroudState(false);
                SetIconState(false);
                SetCloseBtnState(false);
            }

            SetLockedRootState(false);
        }

        public void SetupSlot()
        {
            SetBackGroudState(false);
            SetLockedRootState(true);
            SetCloseBtnState(false);
        }

        private void OnCloseClick()
        {
            SetCloseBtnState(false);
            if (itemData != null)
            {
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnCloseSynthesisIncriptionChanged, itemData.TableID);
            }
        }
    }
}


