using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class StrengthenTicketMergeFrameView : MonoBehaviour
    {
		[SerializeField] private GameObject mObjLeftRoot;
		[SerializeField] private GameObject mObjRightRoot;
		[SerializeField] private GameObject mObjBuffRoot;
		[SerializeField] private GameObject mObjCompoundRoot;
		[SerializeField] private GameObject mObjMixRoot;
		[SerializeField] private string mBuffViewPrefabPath = "UIFlatten/Prefabs/StrengthenTicketMerge/StrengthenTicketBuffView";
		[SerializeField] private string mCompoundViewPrefabPath = "UIFlatten/Prefabs/StrengthenTicketMerge/StrengthenTicketCompoundView";
		[SerializeField] private string mMixViewPrefabPath = "UIFlatten/Prefabs/StrengthenTicketMerge/StrengthenTicketMixView";
		[SerializeField] private ComDefaultBindItemIds mComBindItemIds;
        //标签页遮挡
        [SerializeField] private GameObject mObjTabMask;
        [SerializeField] private List<Toggle> mTabToggleList;

        //左侧道具列表
        private StrengthenTicketMergeLeftItem mLeftView;
        //右侧操作界面
        private StrengthenTicketMergeRightItem mRightView;
        //buff界面
        private StrengthenTicketBuffView mBuffView;
        //合成界面
        private StrengthenTicketCompoundView mCompoundView;
        //融合界面
        private StrengthenTicketMixView mMixView;
        //当前打开的页签类型
        private StrengthenTicketMergeType mMergeType = StrengthenTicketMergeType.Count;
        public StrengthenTicketMergeType MergeType
        {
            get { return mMergeType; }
        }

        //当前合成流程
        private StrengthenTicketMergeStage mCurrMergeStage = StrengthenTicketMergeStage.NotReady;
        public StrengthenTicketMergeStage CurrMergeStage
        {
            get { return mCurrMergeStage; }
            set { mCurrMergeStage = value; }
        }
        
        public void OnInit()
        {
            mObjTabMask.CustomActive(false);
            OnToggleTouch(StrengthenTicketMergeType.Buff);
        }

        public void OnUninit()
        {
            if (mLeftView != null)
            {
                mLeftView.Destroy();
                mLeftView = null;
            }
            if (mRightView != null)
            {
                mRightView.Destroy();
                mRightView = null;
            }
        }

        //展示强化券界面 隐藏buff界面（材料合成与强化券融合
        private void _ShowTicketView()
        {
            mObjLeftRoot.CustomActive(true);
            mObjRightRoot.CustomActive(true);
            mObjBuffRoot.CustomActive(false);
            mObjCompoundRoot.CustomActive(false);
            mObjMixRoot.CustomActive(false);
            _ShowLeftItem();
            _ShowRightItem();
        }
        //展示左侧道具列表
        private void _ShowLeftItem()
        {
            if (mLeftView == null)
            {
                mLeftView = new StrengthenTicketMergeLeftItem();
                mLeftView.Create(this, mObjLeftRoot);
            }
            mLeftView.Show();
        }
        //展示右侧操作面板
        private void _ShowRightItem()
        {
            if (mRightView == null)
            {
                mRightView = new StrengthenTicketMergeRightItem();
                mRightView.Create(this, mObjRightRoot);
            }
            mRightView.Show();
        }

        //展示buff界面 隐藏强化券界面
        private void _ShowBuffView()
        {
            mObjLeftRoot.CustomActive(false);
            mObjRightRoot.CustomActive(false);
            mObjCompoundRoot.CustomActive(false);
            mObjBuffRoot.CustomActive(true);
            mObjMixRoot.CustomActive(false);
            // 创建buffview
            if (null == mBuffView)
            {
                var obj = AssetLoader.GetInstance().LoadResAsGameObject(mBuffViewPrefabPath);
                Utility.AttachTo(obj, mObjBuffRoot);
                mBuffView = obj.GetComponent<StrengthenTicketBuffView>();
                if (null == mBuffView)
                {
                    Logger.LogError("祈福界面预制体没有挂上StrengthenTicketBuffView");
                    return;
                }
                mBuffView.OnInit(OnToggleTouch);
            }
        }

        //展示合成界面 隐藏其他界面
        private void _ShowmCompoundView()
        {
            mObjLeftRoot.CustomActive(false);
            mObjRightRoot.CustomActive(false);
            mObjCompoundRoot.CustomActive(true);
            mObjBuffRoot.CustomActive(false);
            mObjMixRoot.CustomActive(false);
            // 创建view
            if (null == mCompoundView)
            {
                var obj = AssetLoader.GetInstance().LoadResAsGameObject(mCompoundViewPrefabPath);
                Utility.AttachTo(obj, mObjCompoundRoot);
                mCompoundView = obj.GetComponent<StrengthenTicketCompoundView>();
                if (null == mCompoundView)
                {
                    Logger.LogError("祈福界面预制体没有挂上StrengthenTicketCompoundView");
                    return;
                }
                mCompoundView.OnInit(_OnShowMask);
            }
        }

        //展示合成界面 隐藏其他界面
        private void _ShowMixView()
        {
            mObjLeftRoot.CustomActive(false);
            mObjRightRoot.CustomActive(false);
            mObjCompoundRoot.CustomActive(false);
            mObjBuffRoot.CustomActive(false);
            mObjMixRoot.CustomActive(true);
            // 创建view
            if (null == mMixView)
            {
                var obj = AssetLoader.GetInstance().LoadResAsGameObject(mMixViewPrefabPath);
                Utility.AttachTo(obj, mObjMixRoot);
                mMixView = obj.GetComponent<StrengthenTicketMixView>();
                if (null == mMixView)
                {
                    Logger.LogError("祈福界面预制体没有挂上StrengthenTicketMixView");
                    return;
                }
                mMixView.OnInit(_OnShowMask);
            }
        }

        

        private void _OnShowMask()
        {
            mObjTabMask.CustomActive(true);
        }

#region 消息通知
        //合成强化券折扣次数
        public void OnUpdateDisCount()
        {
            if (null != mCompoundView)
            {
                mCompoundView.OnUpdateDisCount();
            }
        }
        //强化券合成选择的券
        public void OnStrengthenTicketMergeSelectTicket(UIEvent param)
        {
            if (MergeType == StrengthenTicketMergeType.Material)
            {
                StrengthenTicketMaterialMergeModel mergeModel = param.Param1 as StrengthenTicketMaterialMergeModel;
                if (mRightView != null)
                {
                    mRightView.SetStrengthenTicketMergeSelectTicket(mergeModel);
                }
            }
        }

        //强化券融合移除选择的券
        public void OnStrengthenTicketFuseRemoveTicket(UIEvent param)
        {
            if (MergeType == StrengthenTicketMergeType.StrengthenTicket)
            {
                if (mRightView != null)
                {
                    mRightView.SetStrengthenTicketFuseRemoveTicket();
                }
                if (mLeftView != null)
                {
                    if (param != null && param.Param1 != null)
                    {
                        StrengthenTicketFuseItemData ticketFuseItemData = param.Param1 as StrengthenTicketFuseItemData;

                        mLeftView.SetStrengthenTicketFuseRemoveTicket(ticketFuseItemData);
                    }
                }
            }
        }

        //强化券融合添加选择的券
        public void OnStrengthenTicketFuseAddTicket(UIEvent param)
        {
            if (MergeType == StrengthenTicketMergeType.StrengthenTicket)
            {
                if (mRightView != null)
                {
                    mRightView.SetStrengthenTicketFuseAddTicket();
                }
                if (mLeftView != null)
                {
                    mLeftView.SetStrengthenTicketFuseAddTicket();
                }
            }
        }

        //强化券合成成功
        public void OnStrengthenTicketMergeSuccess(UIEvent param)
        {
            // if (MergeType == StrengthenTicketMergeType.Material)
            // {
            //     if (mRightView != null)
            //     {
            //         mRightView.SetStrengthenTicketMergeSuccess();
            //     }
            // }
            //更新合成界面的材料
            if (null != mCompoundView)
            {
                mCompoundView.UpdateMaterialItem();
            }
            mObjTabMask.CustomActive(false);
        }

        //强化券融合成功
        public void OnStrengtheTicketFuseSuccess(UIEvent param)
        {
            // if (MergeType == StrengthenTicketMergeType.StrengthenTicket)
            // {
            //     if (mRightView != null)
            //     {
            //         mRightView.SetStrengthenTicketFuseSuccess();
            //     }
            //     if (mLeftView != null)
            //     {
            //         mLeftView.SetStrengthenTicketFuseSuccess();
            //     }
            // }
            if (null != mMixView)
            {
                mMixView.OnViewClear();
            }
            mObjTabMask.CustomActive(false);
        }

        //强化券合成失败
        public void OnStrengthenTicketMergeFailed(UIEvent param)
        {
            // if (MergeType == StrengthenTicketMergeType.Material)
            // {
            //     if (mRightView != null)
            //     {
            //         mRightView.SetStrengthenTicketMergeFailed();
            //     }
            // }
            //更新合成界面的材料
            if (null != mCompoundView)
            {
                mCompoundView.UpdateMaterialItem();
            }
            mObjTabMask.CustomActive(false);
        }

        //强化券融合失败
        public void OnStrengthenTicketFuseFailed(UIEvent param)
        {
            // if (MergeType == StrengthenTicketMergeType.StrengthenTicket)
            // {
            //     if (mRightView != null)
            //     {
            //         mRightView.SetStrengthenTicketFuseFailed();
            //     }
            // }
            if (null != mMixView)
            {
                mMixView.OnViewClear();
            }
            mObjTabMask.CustomActive(false);
        }

        //强化券融合预计概率生成
        public void OnStrengthenTicketFuseCalPercent(UIEvent param)
        {
            if (MergeType == StrengthenTicketMergeType.StrengthenTicket)
            {
                if (mRightView != null)
                {
                    mRightView.SetStrengthenTicketFuseCalPercent();
                }
            }
        }

        //强化券合成 选中第一个
        public void OnStrengthenTicketMergeSelectReset(UIEvent param)
        {
            if (MergeType == StrengthenTicketMergeType.Material)
            {
                if (mLeftView != null)
                {
                    mLeftView.SetResetItemSelect();
                }
            }
        }

        //开始强化券融合时禁止点击
        public void OnStrengthenTicketStartMerge(UIEvent param)
        {
            mObjTabMask.CustomActive(true);
        }

        //金币数量变化
        public void OnGoldChanged(UIEvent param)
        {
            // if (mRightView != null)
            // {
            //     mRightView.RefreshView(null);
            // }
            //更新合成界面的材料
            if (null != mCompoundView)
            {
                mCompoundView.UpdateMaterialItem();
            }
        }

        //绑金数量变化
        public void OnBindGoldChanged(UIEvent param)
        {
            // if (mRightView != null)
            // {
            //     mRightView.RefreshView(null);
            // }
            //更新合成界面的材料
            if (null != mCompoundView)
            {
                mCompoundView.UpdateMaterialItem();
            }
        }

        //商城购买道具成功
        public void OnStrengthenTicketMallBuySuccess(UIEvent param)
        {
            // if (mRightView != null)
            // {
            //     mRightView.RefreshView(null);
            // }
            //更新合成界面的材料
            if (null != mCompoundView)
            {
                mCompoundView.UpdateMaterialItem();
            }
        }

#endregion

#region 获取配置数据
        //材料合成默认展示, 除特殊物品
        public List<int> GetMergeBindItemIds()
        {
            if (mComBindItemIds == null)
            {
                return new List<int>();
            }
            return mComBindItemIds.mergeBindItemIds;
        }
        //材料合成默认展示，特殊物品
        public List<int> GetSpecialMergeBindItemIds()
        {
            if (mComBindItemIds == null)
            {
                return new List<int>();
            }
            return mComBindItemIds.specialMergeItemIds;
        }
        //融合默认展示
        public List<int> GetFuseBindItemIds()
        {
            if (mComBindItemIds == null)
            {
                return new List<int>();
            }
            return mComBindItemIds.fuseBindItemIds;
        }
        //前往获取默认
        public int GetGotoGetBindItemId()
        {
            if (mComBindItemIds == null)
            {
                return 0;
            }
            return mComBindItemIds.getBindItemId;
        }
        //合成或融合时，只需要展示需要数量的道具ids
        public List<int> GetMergeOnlyShowNeedCountItemIds()
        {
            if (mComBindItemIds == null)
            {
                return new List<int>();
            }
            return mComBindItemIds.onlyShowNeedCountItemIds;
        }

        public int GetFuseMaterialGridCount()
        {
            if (mComBindItemIds == null)
            {
                return 0;
            }
            int ticketFuseCount = GetFuseTicketCount();
            int fuseItemCount = 0;
            var fuseItemIds = GetFuseBindItemIds();
            if (fuseItemIds != null)
            {
                fuseItemCount = fuseItemIds.Count;
            }
            return ticketFuseCount + fuseItemCount;
        }

        public int GetFuseTicketCount()
        {
            if (mComBindItemIds == null)
            {
                return 2;
            }
            return mComBindItemIds.fuseTicketCount;
        }

        public float GetWaitToLoadEffectPlaneTime()
        {
            if (mComBindItemIds == null)
            {
                return 0f;
            }
            return mComBindItemIds.waitToLoadEffectPlane;
        }

        public float GetWaitToSelectMaterialFirstItemTime()
        {
            if (mComBindItemIds == null)
            {
                return 0.2f;
            }
            return mComBindItemIds.waitToSelectMaterialFirstItem;
        }

        public List<int> GetNeedFastBuyItemIds()
        {
            if (mComBindItemIds == null)
            {
                return new List<int>();
            }
            return mComBindItemIds.needFastButItemIds;
        }

        //点击某个toggle
        public void OnToggleTouch(StrengthenTicketMergeType type)
        {
            int index = (int)type;
            if (mTabToggleList.Count <= index)
                return;
            if (mTabToggleList[index].isOn)
            {
                switch(type)
                {
                    case StrengthenTicketMergeType.Buff:
                        OnClickTabBuff(true);
                        return;
                    case StrengthenTicketMergeType.Material:
                        OnClickTabCompound(true);
                        return;
                    case StrengthenTicketMergeType.StrengthenTicket:
                        OnClickTabMix(true);
                        return;
                }
            }
            else
                mTabToggleList[index].isOn = true;
        }
#endregion

#region 按钮事件
        //点击打开buff界面
        public void OnClickTabBuff(bool value)
        {
            if (value)
            {
                //合成过程无法切换界面
                if (CurrMergeStage == StrengthenTicketMergeStage.Process)
                {
                    SystemNotifyManager.SysNotifyTextAnimation(TR.Value("strengthen_merge_be_process_stage"));
                    return;
                }
                mMergeType = StrengthenTicketMergeType.Buff;
                _ShowBuffView();
            }
        }

        //点击打开材料合成
        public void OnClickTabCompound(bool value)
        {
            if (value)
            {
                //合成过程无法切换界面
                if (CurrMergeStage == StrengthenTicketMergeStage.Process)
                {
                    SystemNotifyManager.SysNotifyTextAnimation(TR.Value("strengthen_merge_be_process_stage"));
                    return;
                }
                mMergeType = StrengthenTicketMergeType.Material;
                _ShowmCompoundView();
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnStrengthenTicketMergeSelectType, (int)mMergeType);
            }
        }

        //点击打开强化券融合
        public void OnClickTabMix(bool value)
        {
            if (value)
            {
                //合成过程无法切换界面
                if (CurrMergeStage == StrengthenTicketMergeStage.Process)
                {
                    SystemNotifyManager.SysNotifyTextAnimation(TR.Value("strengthen_merge_be_process_stage"));
                    return;
                }
                mMergeType = StrengthenTicketMergeType.StrengthenTicket;
                _ShowMixView();
                //_ShowTicketView();
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnStrengthenTicketMergeSelectType, (int)mMergeType);
            }
        }

        //点击关闭界面
        public void OnClickClose()
        {
            ClientSystemManager.GetInstance().CloseFrame<StrengthenTicketMergeFrame>();
        }
#endregion
    }
}
