using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;
using UnityEngine.Assertions;
using Protocol;
using ProtoTable;
using System.Collections;
using DG.Tweening;
using Network;

namespace GameClient
{
    // 精英地下城预览界面
    public class EliteDungeonPreviewFrame : ClientFrame
    {
        #region inner define

        #endregion

        #region val
        List<int> evaluationItemDatas = null;
        List<int> awardItemDatas = null;
        #endregion

        #region ui bind
        private ComUIListScript mAwardItems = null;
        private ComUIListScript mEvaluationItems = null; 
        private Slider mUnlockProcess = null;
        private Text mUnlockValue = null;
        private ComChapterDungeonUnit mChapterSelectUnit = null;

        #endregion

        #region override

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Chapter/Normal/EliteDungeonPreview";
        }

        protected override void _OnOpenFrame()
        {
            InitEvaluationItem();
            InitAwardItem();

            UpdateUI();

            BindUIEvent();
        }

        protected override void _OnCloseFrame()
        {
            UnBindUIEvent();      
        }

        protected override void _bindExUI()
        {
            mAwardItems = mBind.GetCom<ComUIListScript>("awardItems");
            mEvaluationItems = mBind.GetCom<ComUIListScript>("evaluationItems");
            mUnlockProcess = mBind.GetCom<Slider>("unlockProcess");
            mUnlockValue = mBind.GetCom<Text>("unlockValue");
            mChapterSelectUnit = mBind.GetCom<ComChapterDungeonUnit>("ChapterSelectUnit");
        }

        protected override void _unbindExUI()
        {
            mAwardItems = null;
            mEvaluationItems = null;
            mUnlockProcess = null;
            mUnlockValue = null;
            mChapterSelectUnit = null;
        }

        #endregion

        #region method

        void BindUIEvent()
        {
            //UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.UpdateAventurePassStatus, _OnUpdateAventurePassStatus);
        }

        void UnBindUIEvent()
        {
            //UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.UpdateAventurePassStatus, _OnUpdateAventurePassStatus);
        }

        void UpdateUI()
        {
            UpdateEvaluationItem();
            UpdateAwardItem();

            List<int> ids = GetCurrentChapterNormalDungeonIDs();
            int count = 0;
            if (ids != null && ids.Count > 0)
            {
                count = ids.Count;
            }

            mUnlockValue.SafeSetText(string.Format("{0}/{1}", GetSSSDungeonNum(), count));
            float percent = 1.0f;
            
            if (count > 0)
            {
                percent = ((float)GetSSSDungeonNum()) / ((float)count);
            }
            mUnlockProcess.SafeSetValue(percent);

            if(mChapterSelectUnit != null)
            {
                List<int> dungeonIds = null;
                ChapterSelectFrame chapterSelect = ClientSystemManager.instance.GetFrame(typeof(ChapterSelectFrame)) as ChapterSelectFrame;
                if (null != chapterSelect)
                {
                    dungeonIds = chapterSelect.GetCurrentChapterDungeonIDs();
                }

                if(dungeonIds != null)
                {
                    int eliteDungeonID = dungeonIds.Find((id) => { return TeamUtility.IsEliteDungeonID(id); });
                    DungeonTable item = TableManager.instance.GetTableItem<DungeonTable>(eliteDungeonID);
                    if (null != item)
                    {
                        mChapterSelectUnit.SetName(item.Name, item.RecommendLevel);
                        mChapterSelectUnit.SetType(ComChapterDungeonUnit.eMissionType.None);
                        mChapterSelectUnit.SetBackgroud(item.TumbPath);
                        mChapterSelectUnit.SetCharactorSprite(item.TumbChPath);
                        mChapterSelectUnit.ShowEliteBg(true);
                    }
                }               
            }
        }

        public static Protocol.DungeonScore GetBestScore(int dungeonID)
        {
            DungeonScore score = DungeonScore.C;

            DungeonID tempID = new DungeonID(dungeonID);
            if (tempID == null)
            {
                return score;
            }

            DungeonID id = new DungeonID(tempID.dungeonID);
            if (id == null)
            {
                return score;
            }
            
            int curTopHard = ChapterUtility.GetDungeonTopHard(tempID.dungeonIDWithOutDiff);
            for (int i = 0; i <= curTopHard; ++i)
            {
                id.diffID = i;
                if (ChapterUtility.GetDungeonBestScore(id.dungeonID) >= score)
                {
                    score = ChapterUtility.GetDungeonBestScore(id.dungeonID);
                }
            }

            return score;
        }

        public static bool HasSSS(int dungeonID)
        {           
            return GetBestScore(dungeonID) == DungeonScore.SSS;
        }

        public static int GetSSSDungeonNum()
        {
            List<int> ids = GetCurrentChapterNormalDungeonIDs();
            int count = 0;
            if (ids != null)
            {
                for(int i = 0;i < ids.Count;i++)
                {
                    if (HasSSS(ids[i]))
                    {
                        count++;
                    }
                }
            }

            return count;
        }

        public static List<int> GetCurrentChapterNormalDungeonIDs()
        {
            List<int> ids = new List<int>();

            ChapterSelectFrame chapterSelect = ClientSystemManager.instance.GetFrame(typeof(ChapterSelectFrame)) as ChapterSelectFrame;
            if (null != chapterSelect)
            {
                ids = chapterSelect.GetCurrentChapterDungeonIDs();
            }

            if (ids != null)
            {
                ids.RemoveAll((id) =>
                {
                    return TeamUtility.IsEliteDungeonID(id);
                });
            }

            return ids;
        }

        void InitEvaluationItem()
        {
            if (mEvaluationItems == null)
            {
                return;
            }

            mEvaluationItems.Initialize();

            mEvaluationItems.onBindItem = (GameObject go) =>
            {
                EliteDungeonPreviewEvaluationItem item = null;
                if (go)
                {
                    item = go.GetComponent<EliteDungeonPreviewEvaluationItem>();
                }

                return item;
            };

            mEvaluationItems.onItemVisiable = (var1) =>
            {
                if (var1 == null)
                {
                    return;
                }

                EliteDungeonPreviewEvaluationItem item = var1.gameObjectBindScript as EliteDungeonPreviewEvaluationItem;
                if (item != null && evaluationItemDatas != null && var1.m_index < evaluationItemDatas.Count)
                {
                    item.SetUp(evaluationItemDatas[var1.m_index],var1.m_index);
                }
            };
        }

        public static ComChapterDungeonUnit.eState GetDungeonState(int dungeonID)
        {
            DungeonID did = new DungeonID(0);
            if (did == null)
            {
                return ComChapterDungeonUnit.eState.Locked;
            }

            did.dungeonID = ChapterUtility.GetReadyDungeonID(dungeonID);
            ComChapterDungeonUnit.eState state = ChapterUtility.GetDungeonState(did.dungeonID);

            // 有种情况要处理下，某个关卡的普通难度没有通关，但是其他难度通关了
            if(state == ComChapterDungeonUnit.eState.Unlock)
            {
                DungeonScore dungeonScore = GetBestScore(dungeonID);
                if(dungeonScore >= DungeonScore.B)
                {
                    return ComChapterDungeonUnit.eState.Passed;
                }
            }

            return state;
        }

        void CalEvaluationItemDatas()
        {
            evaluationItemDatas = new List<int>();
            if (evaluationItemDatas == null)
            {
                return;
            }

            evaluationItemDatas = GetCurrentChapterNormalDungeonIDs();

            // 先按照id从小到大排序
            // 然后把SSS的移到后面去
            evaluationItemDatas.Sort((a, b) =>
            {
                return a.CompareTo(b);
            });

            System.Predicate<int> func = (id) => 
            {
                DungeonScore dungeonScore = GetBestScore(id);
                return dungeonScore == DungeonScore.SSS;
            };

            List<int> SSSIDs = evaluationItemDatas.FindAll(func);
            evaluationItemDatas.RemoveAll(func);
            evaluationItemDatas.AddRange(SSSIDs);

            return;
        }

        void UpdateEvaluationItem()
        {
            if (mEvaluationItems == null)
            {
                return;
            }

            CalEvaluationItemDatas();

            if (evaluationItemDatas != null)
            {
                mEvaluationItems.SetElementAmount(evaluationItemDatas.Count);
            }
        }

        void InitAwardItem()
        {
            if (mAwardItems == null)
            {
                return;
            }

            mAwardItems.Initialize();

            mAwardItems.onBindItem = (GameObject go) =>
            {
                EliteDungeonPreviewAwardItem item = null;
                if (go)
                {
                    item = go.GetComponent<EliteDungeonPreviewAwardItem>();
                }

                return item;
            };

            mAwardItems.onItemVisiable = (var1) =>
            {
                if (var1 == null)
                {
                    return;
                }

                EliteDungeonPreviewAwardItem item = var1.gameObjectBindScript as EliteDungeonPreviewAwardItem;
                if (item != null && awardItemDatas != null && var1.m_index < awardItemDatas.Count)
                {
                    item.SetUp(awardItemDatas[var1.m_index]);
                }
            };
        }

        void CalAwardItemDatas()
        {
            awardItemDatas = new List<int>();
            if (awardItemDatas == null)
            {
                return;
            }

            List<int> dungeonIds = null;
            ChapterSelectFrame chapterSelect = ClientSystemManager.instance.GetFrame(typeof(ChapterSelectFrame)) as ChapterSelectFrame;
            if (null != chapterSelect)
            {
                dungeonIds = chapterSelect.GetCurrentChapterDungeonIDs();
            }

            if(dungeonIds != null)
            {
                int eliteDungeonID = dungeonIds.Find((id) => { return TeamUtility.IsEliteDungeonID(id); });

                DungeonTable dungeonTable = TableManager.GetInstance().GetTableItem<DungeonTable>(eliteDungeonID);
                if (dungeonTable != null)
                {
                    awardItemDatas.AddRange(dungeonTable.DropItems);
                }
            }          

            return;
        }

        void UpdateAwardItem()
        {
            if (mAwardItems == null)
            {
                return;
            }

            CalAwardItemDatas();

            if (awardItemDatas != null)
            {
                mAwardItems.SetElementAmount(awardItemDatas.Count);
            }
        }

        #endregion

        #region ui event
        // 
        //         void _OnUpdateAventurePassStatus(UIEvent uiEvent)
        //         {
        //             return;
        //         }       

        #endregion
    }
}
