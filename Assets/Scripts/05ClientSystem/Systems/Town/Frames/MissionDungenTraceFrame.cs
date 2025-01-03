using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using DG.Tweening;
using ProtoTable;
using System.Collections.Generic;

namespace GameClient
{
    class MissionDungenFrame : ClientFrame
    {
        #region ExtraUIBind
        private Button mBtn_Open = null;
        private DOTweenAnimation mDoTween = null;
        private GameObject mGo_Mask = null;
        private CanvasGroup mMissionDungenTraceCanvasGroup = null;
        private bool isOpen = false;

        protected override void _bindExUI()
        {
            mBtn_Open = mBind.GetCom<Button>("Btn_Open");
            if (null != mBtn_Open)
            {
                mBtn_Open.onClick.AddListener(_onBtn_OpenButtonClick);
            }
            mDoTween = mBind.GetCom<DOTweenAnimation>("DoTween");
            mGo_Mask = mBind.GetGameObject("Go_Mask");
            mMissionDungenTraceCanvasGroup = mBind.GetCom<CanvasGroup>("MissionDungenTraceCanvasGroup");
        }

        protected override void _unbindExUI()
        {
            if (null != mBtn_Open)
            {
                mBtn_Open.onClick.RemoveListener(_onBtn_OpenButtonClick);
            }
            mBtn_Open = null;
            mDoTween = null;
            mGo_Mask = null;
            mMissionDungenTraceCanvasGroup = null;
        }
        #endregion

        #region Callback
        private void _onBtn_OpenButtonClick()
        {
            /* put your code in here */
            _Move(!isOpen);
        }
        #endregion

        public static void Open()
        {
            if (ClientSystem.IsTargetSystem<ClientSystemBattle>())
            {
                if (!ClientSystemManager.instance.IsFrameOpen<MissionDungenFrame>())
                {
                    ClientSystemManager.instance.OpenFrame<MissionDungenFrame>(FrameLayer.Bottom);
                }
            }
        }

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Mission/MissionDungenTrace";
        }

        //GameObject goMask;
        //Button btnMoveIn;
        //Button btnMoveOut;
        //DOTweenAnimation m_kTween;
        //GameObject goArrowOut;
        //GameObject goArrowIn;

        protected override void _OnOpenFrame()
        {
            MissionManager.GetInstance().onAddNewMission += OnAddNewMission;
            MissionManager.GetInstance().onUpdateMission += OnUpdateMission;
            MissionManager.GetInstance().onDeleteMission += OnDeleteMission;

            _InitMissionTraceObjects();


            //goMask = Utility.FindChild(frame, "MissionDungenTrace/mask");
            //btnMoveOut = Utility.FindComponent<Button>(frame, "MissionDungenTrace/ArrowOut");
            //if (btnMoveOut != null)
            //{
            //    btnMoveOut.onClick.RemoveAllListeners();
            //    btnMoveOut.onClick.AddListener(() =>
            //    {
            //        _Move(true);
            //    });
            //}


            //btnMoveIn = Utility.FindComponent<Button>(frame, "MissionDungenTrace/ArrowIn");

            //if (btnMoveIn != null)
            //{
            //    btnMoveIn.onClick.RemoveAllListeners();
            //    btnMoveIn.onClick.AddListener(() =>
            //    {
            //        _Move(false);
            //    });
            //}


            //m_kTween = frame.GetComponentInChildren<DOTweenAnimation>();
            //goArrowOut = Utility.FindChild(frame, "MissionDungenTrace/ArrowOut");
            //goArrowIn = Utility.FindChild(frame, "MissionDungenTrace/ArrowIn");

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ClientBattleMainFadeInFadeOut, _onFadeInFadeOut);

            m_bStatus = false;
            m_bStateIsIn = false;
            _RecreateAllTraceObjects();
        }

        void _onFadeInFadeOut(UIEvent ui)
        {
            bool isOpen = (bool)ui.Param1;
            _Move(isOpen);
        }

        public void Move(bool isOpen)
        {
            _Move(isOpen);
        }

        bool m_bStateIsIn = false;
        bool m_bStatus = false;

        void _Move(bool bIn)
        {
            if (m_bStateIsIn == bIn)
            {
                return;
            }
            m_bStateIsIn = bIn;
            if (mGo_Mask != null)
                mGo_Mask.CustomActive(true);

            if (!bIn)
            {
                //m_kTween.DORewind();
                if (mDoTween != null)
                    mDoTween.DOPlayById("MoveOut");
            }
            else
            {
                if (mDoTween != null)
                {
                    mDoTween.DORewind();
                    mDoTween.DOPlayById("MoveIn");
                }
                if (mMissionDungenTraceCanvasGroup != null)
                    mMissionDungenTraceCanvasGroup.alpha = 1;
            }

            isOpen = bIn;

            var lists = MissionManager.GetInstance().GetMissionDungenTraceList();
            bool bNeedShow = lists != null && lists.Count > 0;

            //mBtn_Open.CustomActive(bIn && bNeedShow);
            //mBtn_Open.CustomActive(!bIn && bNeedShow);
            m_bStatus = bIn;

            InvokeMethod.Invoke(this, 0.50f, () =>
            {
                //mBtn_Open.CustomActive(!bIn && bNeedShow);
                //mBtn_Open.CustomActive(bIn && bNeedShow);
                if (mGo_Mask != null)
                    mGo_Mask.CustomActive(false);
                m_bStatus = !bIn;

                if (!bIn)
                {
                    if (mMissionDungenTraceCanvasGroup != null)
                        mMissionDungenTraceCanvasGroup.alpha = 0;
                }
            });
        }

        protected override void _OnCloseFrame()
        {
            MissionManager.GetInstance().onAddNewMission -= OnAddNewMission;
            MissionManager.GetInstance().onUpdateMission -= OnUpdateMission;
            MissionManager.GetInstance().onDeleteMission -= OnDeleteMission;

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ClientBattleMainFadeInFadeOut, _onFadeInFadeOut);

            m_akMissionTraceObjects.Clear();

            //if (btnMoveOut != null)
            //{
            //    btnMoveOut.onClick.RemoveAllListeners();
            //    btnMoveOut = null;
            //}

            //if (btnMoveIn != null)
            //{
            //    btnMoveIn.onClick.RemoveAllListeners();
            //    btnMoveIn = null;
            //}

            InvokeMethod.RemoveInvokeCall(this);
        }

        #region MissionTraceObject
        void _InitMissionTraceObjects()
        {
            m_goMissionParent = Utility.FindChild(frame, "MissionDungenTrace/ScrollView/ViewPort/Content");
            m_goMissionPrefab = Utility.FindChild(m_goMissionParent, "Prefab");
            m_goMissionPrefab.CustomActive(false);
        }
        GameObject m_goMissionParent;
        GameObject m_goMissionPrefab;
        class MissionTraceObject : CachedObject
        {
            GameObject goLocal;
            GameObject goPrefab;
            GameObject goParent;
            int iId;
            MissionDungenFrame THIS;

            LinkParse content;
            Text name;
            Image icon;
            Button link;

            MissionManager.SingleMissionInfo value;
            ProtoTable.MissionTable missionItem;

            public override void OnCreate(object[] param)
            {
                goParent = param[0] as GameObject;
                goPrefab = param[1] as GameObject;
                iId = (int)param[2];
                THIS = param[3] as MissionDungenFrame;

                if (goLocal == null)
                {
                    goLocal = GameObject.Instantiate(goPrefab);
                    Utility.AttachTo(goLocal, goParent);

                    content = Utility.FindComponent<LinkParse>(goLocal, "ScrollView/ViewPort/Content");

                    name = Utility.FindComponent<Text>(goLocal, "Horizen/Name");
                    icon = Utility.FindComponent<Image>(goLocal, "Horizen/Icon");

                    link = goLocal.GetComponent<Button>();
                }
                Enable();
                SetAsLastSibling();
                _Update();
            }
            public override void OnRecycle()
            {
                if (goLocal != null)
                {
                    goLocal.CustomActive(false);
                }
            }
            public override void OnDecycle(object[] param)
            {
                OnCreate(param);
            }
            public override void OnRefresh(object[] param)
            {
                _Update();
            }
            public override void SetAsLastSibling()
            {
                if (goLocal != null)
                {
                    goLocal.transform.SetAsFirstSibling();
                }
            }
            public override void Enable()
            {
                if (goLocal != null)
                {
                    goLocal.CustomActive(true);
                }
            }
            public override void Disable()
            {
                if (goLocal != null)
                {
                    goLocal.CustomActive(false);
                }
            }
            public override bool NeedFilter(object[] param)
            {
                return false;
            }

            void _Update()
            {
                if (iId != 0)
                {
                    missionItem = TableManager.GetInstance().GetTableItem<ProtoTable.MissionTable>(iId);
                    MissionManager.GetInstance().taskGroup.TryGetValue((uint)iId, out value);
                    if (value != null)
                    {
                        //设置任务图标
                        // icon.sprite = AssetLoader.instance.LoadRes(Utility.GetMissionIcon(value.missionItem.TaskType), typeof(Sprite)).obj as Sprite;
                        ETCImageLoader.LoadSprite(ref icon, Utility.GetBattleMissionIcon(value.missionItem.TaskType));
                        //设置任务名称
                        name.text = MissionManager.GetInstance().GetMissionName((uint)missionItem.ID) + MissionManager.GetInstance().GetMissionNameAppendBystatus(value.status, value.missionItem.ID);
                        //设置任务内容
                        if (content != null)
                        {
                            content.SetText(Utility.ParseMissionText(iId, true), true);
                        }
                        //设置点击链接
                        if (link != null)
                        {
                            link.onClick.RemoveAllListeners();
                            //link.onClick.AddListener(() => { THIS.OnClickLink(iId); });
                        }
                    }
                }
            }
        }
        CachedObjectListManager<MissionTraceObject> m_akMissionTraceObjects = new CachedObjectListManager<MissionTraceObject>();
        #endregion

        void OnAddNewMission(UInt32 iTaskID)
        {
            _OnUpdateMission(iTaskID);
        }

        void OnUpdateMission(UInt32 iTaskID)
        {
            _OnUpdateMission(iTaskID);
        }

        void OnDeleteMission(UInt32 iTaskID)
        {
            _OnUpdateMission(iTaskID);
        }

        void _OnUpdateMission(UInt32 iTaskID)
        {
            ProtoTable.MissionTable missionItem = TableManager.GetInstance().GetTableItem<ProtoTable.MissionTable>((Int32)iTaskID);
            if (missionItem == null)
            {
                return;
            }

            if (missionItem.TaskType != ProtoTable.MissionTable.eTaskType.TT_MAIN &&
                missionItem.TaskType != ProtoTable.MissionTable.eTaskType.TT_BRANCH &&
                missionItem.TaskType != ProtoTable.MissionTable.eTaskType.TT_CYCLE &&
                missionItem.TaskType != ProtoTable.MissionTable.eTaskType.TT_CHANGEJOB &&
                missionItem.TaskType != ProtoTable.MissionTable.eTaskType.TT_AWAKEN &&
                missionItem.TaskType != ProtoTable.MissionTable.eTaskType.TT_TITLE)
            {
                return;
            }

            _RecreateAllTraceObjects();
        }

        void _RecreateAllTraceObjects()
        {
            m_akMissionTraceObjects.RecycleAllObject();

            var lists = MissionManager.GetInstance().GetMissionDungenTraceList();
            if (lists != null && lists.Count > 0)
            {
                for (int i = 0; i < lists.Count; ++i)
                {
                    m_akMissionTraceObjects.Create(new object[] { m_goMissionParent, m_goMissionPrefab, lists[i], this });
                }
                //mBtn_Open.CustomActive(m_bStatus);
                //mBtn_Open.CustomActive(!m_bStatus);
            }
            else
            {
                //mBtn_Open.CustomActive(false);
                //mBtn_Open.CustomActive(false);
            }
        }
    }
}