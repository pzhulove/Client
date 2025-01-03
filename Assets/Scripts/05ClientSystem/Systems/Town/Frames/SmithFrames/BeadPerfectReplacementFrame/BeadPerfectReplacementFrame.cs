using ProtoTable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameClient
{
   public class BeadPerfectReplacementModel
    {
        public ItemData mEquipItemData; 
        public BeadPerfectReplacementModel(ItemData mItemData)
        {
            this.mEquipItemData = mItemData;
        }
    }

    public class BeadPerfectReplacementFrame : ClientFrame
    {
        BeadPerfectReplacementModel mModelData;
        BeadItemModel mBeadItemModel = null;
        ItemData mBeadItemData = null;
        #region ExtraUIBind
        private BeadPerfectReplacementView mBeadPerfectReplacementView = null;

        protected sealed override void _bindExUI()
        {
            mBeadPerfectReplacementView = mBind.GetCom<BeadPerfectReplacementView>("BeadPerfectReplacementView");
        }

        protected sealed override void _unbindExUI()
        {
            mBeadPerfectReplacementView = null;
        }
        #endregion

        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/SmithShop/BeadPerfectReplacementFrame/BeadPerfectReplacementFrame";
        }

        protected sealed override void _OnOpenFrame()
        {
            mModelData = userData as BeadPerfectReplacementModel;
            if (mModelData != null)
            {
                mBeadPerfectReplacementView.InitView(mModelData, _OnMountedItemSelect, _OnUnMountedItemSelect, _OnSendSceneReplacePreciousBeadReq, OnCloseClick);
            }
        }

        protected sealed override void _OnCloseFrame()
        {
            base._OnCloseFrame();
            mModelData = null;
            mBeadItemModel = null;
            mBeadItemData = null;
        }

        void _OnMountedItemSelect(BeadItemModel model)
        {
            mBeadItemModel = model;
            ItemSimpleData mSimpleData = _GetExpandItemData(model.beadItemData);
            if (mBeadPerfectReplacementView != null)
            {
                mBeadPerfectReplacementView.UpdateExpendGoldInfo(mSimpleData);
                mBeadPerfectReplacementView.RefreshBeadItemList();
            }
        }

        void _OnUnMountedItemSelect(ItemData data)
        {
            mBeadItemData = data;
        }

        ItemSimpleData _GetExpandItemData(ItemData mBeadItemData)
        {
            ItemSimpleData data = null;
            if (mBeadItemData != null)
            {
                var mBeadTable = TableManager.GetInstance().GetTableItem<BeadTable>(mBeadItemData.TableID);
                if (mBeadTable != null)
                {
                    ReplacejewelsTable mReplaceJewelsTabel = BeadCardManager.GetInstance().GetBeadReplaceJewelsTableData(mBeadTable.Color, mBeadTable.Level, mBeadTable.BeadType);
                    if (mReplaceJewelsTabel != null)
                    {
                        data = new ItemSimpleData(mReplaceJewelsTabel.CostItem, mReplaceJewelsTabel.CostNum);
                    }
                }
            }

            return data;
        }

        void _OnSendSceneReplacePreciousBeadReq()
        {
            if (mBeadItemModel == null)
            {
                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("NoSelectInalyBeadDesc"));
                return;
            }

            if (mBeadItemData == null)
            {
                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("NoSelectedReplaceBeadDesc"));
                return;
            }

            ItemSimpleData mSimpleData = _GetExpandItemData(mBeadItemData);
            int mTotalCount = ItemDataManager.GetInstance().GetOwnedItemCount(mSimpleData.ItemID);
            int mCount = mSimpleData.Count;
            if (mTotalCount < mCount)
            {
                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("equip_upgrade_nomoney_notice"));
                return;
            }

            SendSceneReplacePreciousBeadReq();
        }

        void SendSceneReplacePreciousBeadReq()
        {
            AdjustResultFrame.AdjustResultFrameData sendData = new AdjustResultFrame.AdjustResultFrameData();
            sendData.callback += () => { BeadCardManager.GetInstance().OnSendSceneReplacePreciousBeadReq((byte)mBeadItemModel.eqPrecHoleIndex, mBeadItemModel.equipItemData.GUID, mBeadItemData.GUID); };

            var mRightItemData = ItemDataManager.GetInstance().GetCommonItemTableDataByID(mBeadItemModel.beadItemData.TableID);
            string leftStr = "";
            string rightStr = "";

            if (mBeadItemData.BeadAdditiveAttributeBuffID > 0)
            {
                leftStr = string.Format("[<color=#809CB3FF>附加属性:</color>{0}]", BeadCardManager.GetInstance().GetBeadRandomAttributesDesc(mBeadItemData.BeadAdditiveAttributeBuffID));
            }
           
            if (mBeadItemModel.buffID > 0)
            {
                rightStr = string.Format("[<color=#809CB3FF>附加属性:</color>{0}]", BeadCardManager.GetInstance().GetBeadRandomAttributesDesc(mBeadItemModel.buffID));
            }
           
            sendData.desc = string.Format("使用[{0}]{1}的[{2}]\n置换\n[{3}]{4}的[{5}]\n被置换的宝珠将退回背包，确定要置换？",
                BeadCardManager.GetInstance().GetAttributesDesc((int)mBeadItemData.TableID),
                leftStr,
                mBeadItemData.GetColorName(),
                BeadCardManager.GetInstance().GetAttributesDesc(mBeadItemModel.beadItemData.TableID),
                rightStr,
                mRightItemData.GetColorName());

            ClientSystemManager.GetInstance().OpenFrame<AdjustResultFrame>(FrameLayer.Middle, sendData);
        }

        void OnCloseClick()
        {
            frameMgr.CloseFrame(this);
        }
    }
}

