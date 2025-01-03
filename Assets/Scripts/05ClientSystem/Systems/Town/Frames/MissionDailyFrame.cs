using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
///////删除linq
using ProtoTable;

namespace GameClient
{
    class MissionDailyFrameData
    {
        public MissionDailyFrame.OnRedPointChanged onRedPointChanged;
    }

    class MissionDailyFrame : ClientFrame
    {
        public delegate void OnVisibleChanged(bool bNeedVisible);
        public delegate void OnRedPointChanged(bool bNeedRedPoint);
        OnRedPointChanged onRedPointChanged;

        public static bool IsOpened()
        {
            bool bOpened = false;
            var FuncUnlockdata = TableManager.GetInstance().GetTableItem<FunctionUnLock>((int)FunctionUnLock.eFuncType.DailyTask);
            if (FuncUnlockdata == null ||
                FuncUnlockdata.FinishLevel <= PlayerBaseData.GetInstance().Level)
            {
                bOpened = true;
            }
            return bOpened;
        }

        public static bool CheckRedPoint()
        {
            return DailyMissionList.HasFinishedDailyTask() || GetChestRedPoint() > 0;
        }

        public static int GetRedPointCount()
        {
            return DailyMissionList.GetFinishedDailyTask() + GetChestRedPoint();
        }

        public static int GetChestRedPoint()
        {
            var datas = MissionManager.GetInstance().MissionScoreDatas;
            int iRedCnt = 0;
            for (int i = 0; i < datas.Count; ++i)
            {
                if (datas[i].missionScoreItem.Score <= MissionManager.GetInstance().Score && !MissionManager.GetInstance().AcquiredChestIDs.Contains(datas[i].missionScoreItem.ID))
                {
                    ++iRedCnt;
                }
            }
            return iRedCnt;
        }

        public static void BindGlobalListener()
        {
            PlayerBaseData.GetInstance().onLevelChanged += LevelChanged;
        }
        public static void UnBindGlobalListener()
        {
            PlayerBaseData.GetInstance().onLevelChanged -= LevelChanged;
        }
        public static void LevelChanged(int iPre, int iCur)
        {
            if(onVisibleChanged != null)
            {
                onVisibleChanged.Invoke(IsOpened());
            }
        }

        public static OnVisibleChanged onVisibleChanged;

        public static MissionDailyFrame Open(OnRedPointChanged onRedPointChanged,GameObject goParent)
        {
            MissionDailyFrame frame = null;
            MissionDailyFrameData data = new MissionDailyFrameData
            {
                onRedPointChanged = onRedPointChanged,
            };
            var clientFrame = ClientSystemManager.GetInstance().OpenFrame<MissionDailyFrame>(goParent, data);
            if(clientFrame!=null)
            {
                frame = clientFrame as MissionDailyFrame;
            }
            return frame;
        }

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Mission/MissionDailyFrame";
        }

        //[UIEventHandle("Title/Close")]
        void OnClickClose()
        {
            frameMgr.CloseFrame(this);
        }

        DailyMissionList m_kDailyMissionList = new DailyMissionList();

        protected override void _OnOpenFrame()
        {
            onRedPointChanged = (userData as MissionDailyFrameData).onRedPointChanged;

            m_kDailyMissionList.Initialize(this, Utility.FindChild(frame,"DailyContent"), _CheckDailyMission);
            _LoadMissionScoreItems();
            _UpdateMissionScore();

            MissionManager.GetInstance().onDailyScoreChanged += OnDailyScoreChanged;
            MissionManager.GetInstance().onChestIdsChanged += OnChestIdsChanged;
        }

        void _CheckDailyMission(bool bCheck)
        {
            if(onRedPointChanged != null)
            {
                onRedPointChanged.Invoke(bCheck);
            }
        }

        protected override void _OnCloseFrame()
        {
            onRedPointChanged = null;
            m_kDailyMissionList.UnInitialize();

            MissionManager.GetInstance().onDailyScoreChanged -= OnDailyScoreChanged;
            MissionManager.GetInstance().onChestIdsChanged -= OnChestIdsChanged;

            m_akMissionScoreItems.DestroyAllObjects();
            goMissionScoreItemParent = null;
        }

        #region OnUpdateMission
        int _SortedItem(MissionManager.SingleMissionInfo left, MissionManager.SingleMissionInfo right)
        {
            if (left.status != right.status)
            {
                if (left.status == (int)Protocol.TaskStatus.TASK_OVER)
                {
                    return 1;
                }

                if (right.status == (int)Protocol.TaskStatus.TASK_OVER)
                {
                    return -1;
                }

                return (int)right.status - (int)left.status;
            }

            if (left.missionItem.SortID != right.missionItem.SortID)
            {
                return left.missionItem.SortID - right.missionItem.SortID;
            }

            if (left.taskID != right.taskID)
            {
                return left.taskID < right.taskID ? -1 : 1;
            }

            return 0;
        }
        #endregion

        #region MissionScoreData
        class MissionScoreItem : CachedObject
        {
            GameObject goLocal;
            GameObject goPrefab;
            GameObject goParent;
            MissionManager.MissionScoreData data;
            MissionFrameNew THIS;

            DG.Tweening.DOTweenAnimation tween;
            Text score;
            ComChangeColor comChangeColor;
            Image image;
            GameObject goEffect;
            Button button;
            UIGray boxGray;
            GameObject goCheck;
            //MissionScoreRedBinder comBinder;

            public override void OnDestroy()
            {
                goLocal = null;
                goPrefab = null;
                goParent = null;
                data = null;
                THIS = null;
                comChangeColor = null;
                score = null;
                image = null;

                if (button != null)
                {
                    button.onClick.RemoveAllListeners();
                    button = null;
                }
                boxGray = null;
                //comBinder = null;
                tween = null;
            }

            public override void OnCreate(object[] param)
            {
                goParent = param[0] as GameObject;
                goPrefab = param[1] as GameObject;
                data = param[2] as MissionManager.MissionScoreData;
                THIS = param[3] as MissionFrameNew;

                if (goPrefab == null)
                {
                    return;
                }

                if (goLocal == null)
                {
                    goLocal = goPrefab;
                    //Utility.AttachTo(goLocal, goParent);

                    boxGray = Utility.FindComponent<UIGray>(goLocal, "Box");
                    score = Utility.FindComponent<Text>(goLocal, "Label");
                    comChangeColor = goLocal.GetComponent<ComChangeColor>();
                    //image = goLocal.GetComponent<Image>();
                    image = Utility.FindComponent<Image>(goLocal, "Box");
                    button = Utility.FindComponent<Button>(goLocal, "Box");
                    //comBinder = Utility.FindComponent<MissionScoreRedBinder>(goLocal, "Bg/RedPoint");
                    goEffect = Utility.FindChild(goLocal, "Effect");
                    tween = Utility.FindComponent<DG.Tweening.DOTweenAnimation>(goLocal, "Box");
                    goCheck = Utility.FindChild(goLocal, "Check");

                    button.onClick.RemoveAllListeners();
                    button.onClick.AddListener(_OnOpenChest);
                }
                Enable();
                _Update();
            }

            void _OnOpenChest()
            {
                MissionScoreAwardFrame.Open(data.missionScoreItem.ID);
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
                if (param != null && param.Length > 0)
                {
                    data = param[0] as MissionManager.MissionScoreData;
                }
                _Update();
            }

            public override void SetAsLastSibling()
            {
                if (goLocal != null)
                {
                    goLocal.transform.SetAsLastSibling();
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

            public bool CanAcquire
            {
                get
                {
                    bool bCanAcquire = data.missionScoreItem.Score <= MissionManager.GetInstance().Score && !MissionManager.GetInstance().AcquiredChestIDs.Contains(data.missionScoreItem.ID);
                    return bCanAcquire;
                }
            }

            void _Update()
            {
                if (data != null)
                {
                    score.text = data.missionScoreItem.Name;
                    data.bOpen = data.missionScoreItem.Score <= MissionManager.GetInstance().Score && MissionManager.GetInstance().AcquiredChestIDs.Contains(data.missionScoreItem.ID);
                    comChangeColor.SetColor(data.missionScoreItem.Score <= MissionManager.GetInstance().Score);
                    // image.sprite = data.GetIcon();
                    data.GetIcon(ref image);
                    //comBinder.LinkID = data.missionScoreItem.ID;
                    goEffect.CustomActive(data.missionScoreItem.Score <= MissionManager.GetInstance().Score && !MissionManager.GetInstance().AcquiredChestIDs.Contains(data.missionScoreItem.ID));
                    goCheck.CustomActive(MissionManager.GetInstance().AcquiredChestIDs.Contains(data.missionScoreItem.ID));
                    if (boxGray != null)
                    {
                        boxGray.SetEnable(MissionManager.GetInstance().AcquiredChestIDs.Contains(data.missionScoreItem.ID));
                    }

                    var bounds = RectTransformUtility.CalculateRelativeRectTransformBounds(goParent.transform);
                    float width = goParent.transform.rectTransform().rect.width;
                    goLocal.transform.localPosition = new Vector3(width * data.fPostion - 24.0f, goLocal.transform.localPosition.y, 0.0f);
                    if (goEffect.activeSelf)
                    {
                        tween.DOPlay();
                    }
                    else
                    {
                        tween.DOPause();
                        tween.target.transform.localScale = Vector3.one;
                    }
                }
            }
        }
        CachedObjectDicManager<int, MissionScoreItem> m_akMissionScoreItems = new CachedObjectDicManager<int, MissionScoreItem>();
        GameObject goMissionScoreItemParent;
        void _LoadMissionScoreItems()
        {
            if (goMissionScoreItemParent == null)
            {
                goMissionScoreItemParent = Utility.FindChild(frame, "ScoreBar");
            }
            m_akMissionScoreItems.RecycleAllObject();
            var datas = MissionManager.GetInstance().MissionScoreDatas;
            for (int i = 0; i < datas.Count; ++i)
            {
                GameObject goLocal = Utility.FindChild(frame, "ScoreBar/Item_" + i);
                var current = m_akMissionScoreItems.Create(datas[i].missionScoreItem.ID, new object[] { goMissionScoreItemParent, goLocal, datas[i], this });
                if (current != null)
                {
                    current.SetAsLastSibling();
                }
            }
        }
        [UIControl("HYD/ImageNumber",typeof(UINumber))]
        UINumber m_kScore;
        [UIControl("ScoreBar/Front", typeof(Slider))]
        Slider m_kSlider;

        void _UpdateMissionScore()
        {
            m_kScore.Value = MissionManager.GetInstance().Score;
            float fValue = MissionManager.GetInstance().Score * 1.0f / MissionManager.GetInstance().MaxScore;
            fValue = Mathf.Clamp01(fValue);
            m_kSlider.value = fValue;
        }

        void OnDailyScoreChanged(int score)
        {
            _UpdateMissionScore();
            m_akMissionScoreItems.RefreshAllObjects(null);

            _CheckDailyMission(m_kDailyMissionList.CheckRedPoint());
        }

        void OnChestIdsChanged()
        {
            _UpdateMissionScore();
            m_akMissionScoreItems.RefreshAllObjects(null);
            _CheckDailyMission(m_kDailyMissionList.CheckRedPoint());
        }
        #endregion
    }
}
