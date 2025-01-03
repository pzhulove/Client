using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public enum BeadCardState
    {
        None,
        CanBeSet,
        HasBeenSet,
        Replace,
    }

    public delegate void OnBeadItemClick(PrecBead bead);
    public class BeadItemElement : MonoBehaviour
    {
        [SerializeField] private Toggle toggle;
        [SerializeField] private StateController mStateControl;
        [SerializeField] private GameObject mHasBeenSetBeadCardParent;
        [SerializeField] private Text mHasBeenSetBeadCardName;
        [SerializeField] private Text mHasBeenSetBeadCardAttr;

        private PrecBead mPrecBeadData;
        private OnBeadItemClick mOnBeadItemClick;
        private BeadCardState mBeadCardState;
        private ItemData mCurrentBeadCardItem = null; //装备身上已镶嵌的宝珠道具
        private ComItemNew mHasBeenSetBeadComItem;

        public PrecBead PrecBeadData
        {
            get { return mPrecBeadData; }
        }

        private void Awake()
        {
            if (toggle != null)
            {
                toggle.onValueChanged.RemoveAllListeners();
                toggle.onValueChanged.AddListener(OnToggleValueChanged);
            }
        }

        private void OnDestroy()
        {
            mPrecBeadData = null;
            mOnBeadItemClick = null;
            mBeadCardState = BeadCardState.None;
            mCurrentBeadCardItem = null;
            mHasBeenSetBeadComItem = null;

            if (toggle != null)
            {
                toggle.onValueChanged.RemoveListener(OnToggleValueChanged);
            }
        }

        public void InitBeadItemVisiable(PrecBead beadData, OnBeadItemClick onBeadItemClick,bool bIsSelected = false)
        {
            if (beadData == null)
            {
                return;
            }

            mPrecBeadData = beadData;
            mOnBeadItemClick = onBeadItemClick;

            RefreshBeadHoleInfo(mPrecBeadData, bIsSelected);
        }

        public void RefreshBeadHoleInfo(PrecBead beadData, bool bIsSelected = false)
        {
            if (beadData == null)
                return;

            mCurrentBeadCardItem = ItemDataManager.CreateItemDataFromTable(beadData.preciousBeadId);
            if (mCurrentBeadCardItem != null)
            {
                mCurrentBeadCardItem.BeadAdditiveAttributeBuffID = beadData.randomBuffId;
                mCurrentBeadCardItem.BeadPickNumber = beadData.pickNumber;
                mCurrentBeadCardItem.BeadReplaceNumber = beadData.beadReplaceNumber;
            }
            if (mCurrentBeadCardItem == null)
            {
                SetBeadCardState(BeadCardState.CanBeSet);
            }
            else
            {
                SetBeadCardState(BeadCardState.HasBeenSet);
            }

            if (toggle != null)
            {
                if (bIsSelected)
                {
                    if (toggle.isOn == true)
                        OnToggleValueChanged(bIsSelected);
                    else
                        toggle.isOn = true;
                }
                else
                {
                    toggle.isOn = false;
                }
            }
            
        }

        /// <summary>
        /// 设置装备宝珠孔的状态
        /// </summary>
        /// <param name="state">状态</param>
        /// <param name="toBeInlaidBead">待镶嵌的宝珠</param>
        public void SetBeadCardState(BeadCardState state,ItemData toBeInlaidBead = null)
        {
            mBeadCardState = state;
            switch (mBeadCardState)
            {
                case BeadCardState.None:
                    break;
                case BeadCardState.CanBeSet:
                    {
                        mStateControl.Key = "CanBeSet";
                    }
                    break;
                case BeadCardState.HasBeenSet:
                    {
                        mStateControl.Key = "HasBeenSet";
                        UpdateHasBeenSetBeadCardInfo();
                    }
                    break;
            }
        }
        

        #region HasBeenSet(已镶嵌了宝珠)

        private void UpdateHasBeenSetBeadCardInfo()
        {
            if (mHasBeenSetBeadComItem == null)
            {
                mHasBeenSetBeadComItem = ComItemManager.CreateNew(mHasBeenSetBeadCardParent);
            }

            mHasBeenSetBeadComItem.Setup(mCurrentBeadCardItem, null);

            mHasBeenSetBeadCardName.text = mCurrentBeadCardItem.GetColorName();

            //string attribute = BeadCardManager.GetInstance().GetAttributesDesc(mCurrentBeadCardItem.TableID);
            //int count = Regex.Matches(attribute, "\n").Count;

            //if (count > 2)
            //{
            //    mHasBeenSetBeadCardAttr.alignment = TextAnchor.UpperCenter;
            //}
            //else
            //{
            //    mHasBeenSetBeadCardAttr.alignment = TextAnchor.MiddleCenter;
            //}

            if (mCurrentBeadCardItem.BeadAdditiveAttributeBuffID > 0)
            {
                mHasBeenSetBeadCardAttr.text = BeadCardManager.GetInstance().GetAttributesDesc(mCurrentBeadCardItem.TableID) + "\n" +
                   string.Format("附加属性:{0}", BeadCardManager.GetInstance().GetBeadRandomAttributesDesc(mCurrentBeadCardItem.BeadAdditiveAttributeBuffID));
            }
            else
            {
                mHasBeenSetBeadCardAttr.text = BeadCardManager.GetInstance().GetAttributesDesc(mCurrentBeadCardItem.TableID);
            }
        }

        #endregion
        

        #region ButtonClick

        private void OnToggleValueChanged(bool value)
        {
            if (value)
            {
                if (mOnBeadItemClick != null)
                {
                    mOnBeadItemClick(mPrecBeadData);
                }
            }
        }
        
        #endregion

        public bool BeadStateIsHasBeenSet()
        {
            return mBeadCardState == BeadCardState.HasBeenSet || mBeadCardState == BeadCardState.Replace;
        }

        public void OnReSetToggleIsOn()
        {
            if (toggle != null)
                toggle.isOn = false;
        }
    }
}