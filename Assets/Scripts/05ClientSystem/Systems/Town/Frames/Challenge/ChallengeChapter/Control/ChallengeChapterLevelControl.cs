using System;
using System.Collections;
using System.Collections.Generic;
///////删除linq
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;
using ProtoTable;

namespace GameClient
{

    public delegate void OnChapterLevelButtonClick(int levelId,int dungeonId);

    public class ChallengeChapterLevelControl : MonoBehaviour
    {

        private List<ChallengeChapterLevelDataModel> _levelDataModelList;
        private OnChapterLevelButtonClick _onLevelButtonClick;
        private List<ChallengeChapterLevelItem> _levelItemList;

        [SerializeField] private Text addText;
        [SerializeField] private ComUIListScript levelItemList;
        [SerializeField] private string[] mDiffLaceIconPaths;

        private void Awake()
        {
            BindEvents();
        }

        private void OnDestroy()
        {
            UnBindEvents();
            ClearData();
        }

        private void BindEvents()
        {
            if (levelItemList != null)
            {
                levelItemList.Initialize();
                levelItemList.onItemVisiable += OnLevelItemVisible;
                levelItemList.OnItemUpdate += OnLevelItemUpdate;
            }
        }

        private void UnBindEvents()
        {
            if (levelItemList != null)
            {
                levelItemList.onItemVisiable -= OnLevelItemVisible;
                levelItemList.OnItemUpdate -= OnLevelItemUpdate;
            }
        }

        private void ClearData()
        {
            _levelDataModelList = null;
            _levelItemList = null;
            _onLevelButtonClick = null;
        }

        public void InitLevelControl(List<ChallengeChapterLevelDataModel> levelDataModelList,
            OnChapterLevelButtonClick onLevelButtonClick)
        {
            _levelDataModelList = levelDataModelList;
            _onLevelButtonClick = onLevelButtonClick;

            if (_levelDataModelList == null || _levelDataModelList.Count <= 0)
            {
                Logger.LogErrorFormat("LevelDataModel is null or count is zero ");
                return;
            }

            if (levelItemList != null)
                levelItemList.SetElementAmount(_levelDataModelList.Count);

            //更新当前难度的信息
            var curLevelId = 0;
            var curDungeonId = 0;
            for (var i = 0; i < _levelDataModelList.Count; i++)
            {
                if (_levelDataModelList[i] != null)
                {
                    if (_levelDataModelList[i].IsSelected == true)
                    {
                        curLevelId = i;
                        curDungeonId = _levelDataModelList[i].DungeonId;
                        break;

                    }
                }
            }

            UpdateExpAndDropAdd(curLevelId, curDungeonId);
        }

        private void OnLevelItemVisible(ComUIListElementScript item)
        {
            if (item == null)
                return;

            if (_levelDataModelList == null)
                return;

            if (levelItemList == null)
                return;

            if (item.m_index < 0 || item.m_index >= _levelDataModelList.Count)
                return;

            var levelDataModel = _levelDataModelList[item.m_index];
            var chapterLevelItem = item.GetComponent<ChallengeChapterLevelItem>();

            if (chapterLevelItem != null && levelDataModel != null)
            {
                string path = string.Empty;
                if (mDiffLaceIconPaths != null && mDiffLaceIconPaths.Length > item.m_index)
                {
                    path = mDiffLaceIconPaths[item.m_index];
                }
                chapterLevelItem.InitItem(levelDataModel, OnChapterLevelButtonClick, path);
            }
        }

        private void OnLevelItemUpdate(ComUIListElementScript item)
        {
            if (item == null)
                return;

            var chapterLevelItem = item.GetComponent<ChallengeChapterLevelItem>();
            if(chapterLevelItem != null)
                chapterLevelItem.OnUpdateLevelItemSelected();
        }


        private void OnChapterLevelButtonClick(int levelId, int dungeonId)
        {
            UpdateExpAndDropAdd(levelId,dungeonId);
            UpdateLevelItemList(dungeonId);

            if (_onLevelButtonClick != null)
                _onLevelButtonClick(levelId, dungeonId);
        }

        private void UpdateExpAndDropAdd(int levelId, int dungeonId)
        {
            //限时深渊，周常深渊入口和周常深渊的前置关卡
            if (DungeonUtility.IsLimitTimeHellDungeon(dungeonId) == true
                || DungeonUtility.IsWeekHellEntryDungeon(dungeonId) == true
                || DungeonUtility.IsWeekHellPreDungeon(dungeonId) == true)
            {
                var onlyKingHardLevelDropRate = ChallengeUtility.GetOnlyKingHardLevelDropRate();

                if (addText != null)
                {
                    var onlyKingHardLevelDropStr = string.Format(TR.Value("challenge_chapter_level_only_drop_add"),
                        onlyKingHardLevelDropRate);
                    addText.text = onlyKingHardLevelDropStr;
                }
                return;
            }

            var expAddRate = ChallengeUtility.GetExpRate(levelId);
            var dropAddRate = ChallengeUtility.GetDropRate(levelId);

            if (addText != null)
            {
                var str = string.Format(TR.Value("challenge_chapter_level_add"), expAddRate, dropAddRate);
                addText.text = str;
            }
        }

        //更新LevelItem的显示状态
        private void UpdateLevelItemList(int selectedDungeonId)
        {
            //更新数据
            for (var i = 0; i < _levelDataModelList.Count; i++)
            {
                var curLevelDataModel = _levelDataModelList[i];
                if (curLevelDataModel != null)
                {
                    if (curLevelDataModel.DungeonId != selectedDungeonId)
                        curLevelDataModel.IsSelected = false;
                }
            }

            //更新UI表现
            if(levelItemList != null)
                levelItemList.UpdateElement();
        }
    }
}
