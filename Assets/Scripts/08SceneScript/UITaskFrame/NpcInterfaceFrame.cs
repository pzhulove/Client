using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using ProtoTable;

namespace GameClient
{
    public sealed class NpcInterfaceFrame : ClientFrame
    {
        public static UInt64 NpcGuid = 0;
        public static Int32 NpcId = 0;
        public static List<NpcInteractionData> Datas = null;

        private List<NpcInteractionData> _functionDatas = null;

        private UIPrefabWrapper mDialogSecondViewWrapper = null;
        private NpcDialogSecondView mNpcDialogSecondView = null;
        private GameObject mGoPrefab = null;
        private CanvasGroup mCanvasPrefabParent = null;

        public static void TryCloseFrame(Int32 curNpcId, UInt64 curNpcGuid = 0)
        {
            if(NpcInterfaceFrame.NpcGuid == curNpcGuid && NpcInterfaceFrame.NpcId == curNpcId)
            {
                NpcInterfaceFrame.NpcGuid = 0;
                NpcInterfaceFrame.NpcId = 0;
                Datas = null;
                ClientSystemManager.GetInstance().CloseFrame<NpcInterfaceFrame>();

                NpcTable tableData = TableManager.GetInstance().GetTableItem<NpcTable>(curNpcId);
                if(tableData != null && tableData.Function == NpcTable.eFunction.Chiji)
                {
                    ClientSystemManager.GetInstance().CloseFrame<ChijiHandInEquipmentFrame>();
                    ClientSystemManager.GetInstance().CloseFrame<ChijiNpcDialogFrame>();
                }
            }
        }
        public static void OpenFrame(Int32 curNpcId,List<NpcInteractionData> curDatas, UInt64 curNpcGuid = 0)
        {
            //如果已经进入一个npc的感应区域，再同时进入到另一个感应区域的时候，直接返回
            //修复同时进入到两个相应区域会出现的bug
            if(NpcInterfaceFrame.NpcGuid != 0 || NpcInterfaceFrame.NpcId != 0)
                return;

            if (curNpcGuid != NpcInterfaceFrame.NpcGuid || curNpcId != NpcInterfaceFrame.NpcId ||
                curDatas != NpcInterfaceFrame.Datas || ClientSystemManager.GetInstance().IsFrameOpen<NpcInterfaceFrame>() == false)
            {
                if (ClientSystemManager.GetInstance().IsFrameOpen<NpcInterfaceFrame>())
                {
                    ClientSystemManager.GetInstance().CloseFrame<NpcInterfaceFrame>();
                }

                NpcInterfaceFrame.NpcGuid = curNpcGuid;
                NpcInterfaceFrame.NpcId = curNpcId;
                NpcInterfaceFrame.Datas = curDatas;
                ClientSystemManager.GetInstance().OpenFrame<NpcInterfaceFrame>(FrameLayer.Bottom, curDatas);
            }
        }

        Button button;
        Image kIcon;

        protected sealed override bool GetNeedOpenSound()
        {
            return false;
        }

        protected sealed override bool GetNeedCloseSound()
        {
            return false;
        }

        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/FunctionFrame/NpcInterfaceFrame";
        }

        void OnNpcRelationMissionChanged(UIEvent uiEvent)
        {
            if((int)uiEvent.Param1 == NpcId)
            {
                _RefreshNpcInteractionItems();
            }
        }

        void _OnOpenDialogSecondView(UIEvent uiEvent)
        {
            if (uiEvent == null || uiEvent.Param1 == null || !(uiEvent.Param1 is List<NpcInteractionData>))
            {
                return;
            }

            List<NpcInteractionData> dialogSecondDatas = uiEvent.Param1 as List<NpcInteractionData>;

            if (mNpcDialogSecondView == null)
            {
                if (mDialogSecondViewWrapper != null)
                {
                    GameObject go = mDialogSecondViewWrapper.LoadUIPrefab();
                    go.transform.SetParent(mDialogSecondViewWrapper.transform, false);
                    mNpcDialogSecondView = go.GetComponent<NpcDialogSecondView>();
                    mNpcDialogSecondView.Init(dialogSecondDatas, _ShowNpcDialogSecondView);
                }
            }
            else
            {
                mNpcDialogSecondView.Init(dialogSecondDatas, _ShowNpcDialogSecondView);
            }
            _ShowNpcDialogSecondView(true);
        }

        private void _ShowNpcDialogSecondView(bool isShow)
        {
            mCanvasPrefabParent.CustomActive(!isShow);
            mDialogSecondViewWrapper.CustomActive(isShow);
        }

        CachedObjectListManager<NpcInteractionItem> m_akNpcInteractionItems = new CachedObjectListManager<NpcInteractionItem>();

        protected override void _OnOpenFrame()
        {
            _functionDatas = userData as List<NpcInteractionData>;
            _RegisterUIEvent();

            _RefreshNpcInteractionItems();
        }

        private void _RegisterUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.NpcRelationMissionChanged, OnNpcRelationMissionChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OpenDialogSecondView, _OnOpenDialogSecondView);
        }

        private void _UnRegisterUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.NpcRelationMissionChanged, OnNpcRelationMissionChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OpenDialogSecondView, _OnOpenDialogSecondView);
        }

        protected override void _bindExUI()
        {
            base._bindExUI();

            if (mBind == null)
            {
                return;
            }

            mDialogSecondViewWrapper = mBind.GetCom<UIPrefabWrapper>("NpcDialogSecondViewParent");
            mGoPrefab = mBind.GetGameObject("FunctionItem");
            mCanvasPrefabParent = mBind.GetCom<CanvasGroup>("ItemParent");
        }

        protected override void _unbindExUI()
        {
            base._unbindExUI();

            mDialogSecondViewWrapper = null;
            mNpcDialogSecondView = null;
            mGoPrefab = null;
            mCanvasPrefabParent = null;
        }

        protected override void _OnCloseFrame()
        {
            NpcGuid = 0;
            NpcId = 0;
            Datas = null;
            m_akNpcInteractionItems.Clear();
            _UnRegisterUIEvent();
        }

        void _RefreshNpcInteractionItems()
        {
            if (_functionDatas != null)
            {
                if (mGoPrefab != null)
                {
                    mGoPrefab.CustomActive(false);

                    m_akNpcInteractionItems.RecycleAllObject();
                    for (int i = 0; i < _functionDatas.Count; ++i)
                    {
                        m_akNpcInteractionItems.Create(new object[] { mCanvasPrefabParent.gameObject, mGoPrefab, _functionDatas[i], this });
                    }
                }
            }
        }
    }
}