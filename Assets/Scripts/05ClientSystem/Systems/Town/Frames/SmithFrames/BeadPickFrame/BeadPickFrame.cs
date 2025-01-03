using Network;
using Protocol;
using ProtoTable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameClient
{
    public class BeadPickModel
    {
        public PrecBead mPrecBead;//孔数据
        public ItemData mEquipItemData;
        public List<BeadPickItemModel> mBeadPickItemList;
        public BeadPickModel(PrecBead mPrecBead, ItemData mEquipItemData)
        {
            this.mPrecBead = mPrecBead;
            this.mEquipItemData = mEquipItemData;

            var mBeadTable = TableManager.GetInstance().GetTableItem<BeadTable>(this.mPrecBead.preciousBeadId);
            if (mBeadTable == null)
            {
                Logger.LogErrorFormat("[BeadPickFrame]  BeadPickModel 构造函数中beadItemDate.TableID为空");
            }
            mBeadPickItemList = BeadCardManager.GetInstance().GetBeadExpendItemModel((int)mBeadTable.Color, mBeadTable.Level, mBeadTable.BeadType);
        }
    }

    /// <summary>
    /// 宝珠摘除消耗道具数据
    /// </summary>
    public class BeadPickItemModel
    {
        public int mExpendItemID;//消耗道具ID
        public int mExpendCount;//消耗数量
        public int mPickSuccessRate;//摘取成功率
        public int mBeadPickTotleNumber;//宝珠摘取总数量
        public BeadPickItemModel(int expendItemID,int expendCount,int pickSuccessRate,int pickTotleNumber)
        {
            this.mExpendItemID = expendItemID;
            this.mExpendCount = expendCount;
            this.mPickSuccessRate = pickSuccessRate;
            this.mBeadPickTotleNumber = pickTotleNumber;
        }
    }

    public class BeadPickFrame : ClientFrame
    {
        BeadPickModel mDate = null;
        int pestleId = 0;
        int mExpendItemSuccessRate = 0;
        int mBeadRemainPickNumber = 0;//宝珠剩余摘取次数
        protected const string mPrefabPath = "UIFlatten/Prefabs/SmithShop/BeadPickFrame/BeadPickFrame";

        #region ExtraUIBind
        private BeadPickView mBeadPickView = null;

        protected sealed override void _bindExUI()
        {
            mBeadPickView = mBind.GetCom<BeadPickView>("BeadPickView");
        }

        protected sealed override void _unbindExUI()
        {
            mBeadPickView = null;
        }
        #endregion
        public sealed override string GetPrefabPath()
        {
            return mPrefabPath;
        }

        protected sealed override void _OnOpenFrame()
        {
            base._OnOpenFrame();

            mDate = userData as BeadPickModel;
            if (mDate == null)
            {
                return;
            }

            if (mBeadPickView != null)
            {
                mBeadPickView.Init(mDate,OnCloseClick, OnOkBtnClick);
            }
            
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnSelectPickBeadExpendItem, _OnSelectPickBeadExpendItem);
        }

        protected sealed override void _OnCloseFrame()
        {
            base._OnCloseFrame();
            mDate = null;
            
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnSelectPickBeadExpendItem, _OnSelectPickBeadExpendItem);
            pestleId = 0;
            mExpendItemSuccessRate = 0;
            mBeadRemainPickNumber = 0;
        }

        void _OnSelectPickBeadExpendItem(UIEvent iEvent)
        {
            pestleId = (int)iEvent.Param1;
            mExpendItemSuccessRate = (int)iEvent.Param2;
            mBeadRemainPickNumber = (int)iEvent.Param3;
        }
        
        void OnCloseClick()
        {
            frameMgr.CloseFrame(this);
        }

        void OnOkBtnClick()
        {
            var mTable = TableManager.GetInstance().GetTableItem<ItemTable>(pestleId);
            if (mTable == null)
            {
                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("BeadPickFrame_SelectExpand"));
                return;
            }

            ItemData mItemData = ItemDataManager.GetInstance().GetCommonItemTableDataByID(mTable.ID);
            if (mItemData == null)
            {
                return;
            }
            
            int rate = mExpendItemSuccessRate / 100;
            string content = "";
            if (rate < 100)
            {
                content = TR.Value("bead_pick_des", TR.Value("Bead_red_color"), rate);
            }
            else
            {
                content = TR.Value("bead_pick_des", TR.Value("Bead_Green_color"), rate);
            }

            SystemNotifyManager.SysNotifyMsgBoxOkCancel(content, () =>
            {
                BeadCardManager.GetInstance().SendSceneExtirpePreciousBeadReq((byte)mDate.mPrecBead.index, mDate.mEquipItemData.GUID, (uint)pestleId);
            });
        }
    }
}

