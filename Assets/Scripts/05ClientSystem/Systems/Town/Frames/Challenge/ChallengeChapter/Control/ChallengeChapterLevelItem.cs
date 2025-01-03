using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;
using ProtoTable;
using Protocol;

namespace GameClient
{
    

    public class ChallengeChapterLevelItem : MonoBehaviour
    {

        private int _levelId;       //等级的ID
        private int _dungeonId;     //地下城的ID；


        private ChallengeChapterLevelDataModel _levelDataModel;
        private OnChapterLevelButtonClick _onChapterLevelButtonClick;
        private DungeonTable _dungeonTable;



        [Space(10)]
        [HeaderAttribute("Item")]

        [SerializeField] private Text levelNameText;
        [SerializeField] private Text recommendLevel;
        [SerializeField] private Text levelNameTextSelect;
        [SerializeField] private Text recommendLevelSelect;

        [SerializeField] private ImageEx imageLaceIcon;
        [SerializeField] private ImageEx imageLaceIconSelect;

        [SerializeField] private CanvasGroup canvasUnSelect;
        [SerializeField] private float canvasUnSelectUnLockAlpha;

        [SerializeField] private Button levelButton;
        [SerializeField] private Image itemSelectedFlag;

        [Space(10)] [HeaderAttribute("unPass")] [Space(10)] [SerializeField]
        private GameObject beforeLevelLockRoot;

        [SerializeField] private Text beforeLevelText;
        [SerializeField] private GameObject playerLevelLockRoot;
        [SerializeField] private Text playerLevelText;
        [SerializeField] private GameObject unPassedRoot;

        [Space(10)][HeaderAttribute("Score")]
        [Space(10)]
        [SerializeField] private GameObject scoreRoot;
        [SerializeField] private Image AImage;
        [SerializeField] private Image firstSImage;
        [SerializeField] private Image secondSImage;
        [SerializeField] private Image thirdSImage;

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
            if (levelButton != null)
            {
                levelButton.onClick.RemoveAllListeners();
                levelButton.onClick.AddListener(OnLevelButtonClick);
            }
        }

        private void UnBindEvents()
        {
            if (levelButton != null)
                levelButton.onClick.RemoveAllListeners();
        }

        private void ClearData()
        {
            _levelDataModel = null;
            _onChapterLevelButtonClick = null;
            _dungeonTable = null;
        }

        public void InitItem(ChallengeChapterLevelDataModel levelDataModel,
            OnChapterLevelButtonClick onLevelButtonClick, string path)
        {
            imageLaceIcon.SafeSetImage(path);
            imageLaceIconSelect.SafeSetImage(path);
            _levelDataModel = levelDataModel;
            _onChapterLevelButtonClick = onLevelButtonClick;

            if (_levelDataModel == null || _levelDataModel.DungeonId <= 0)
            {
                Logger.LogErrorFormat("DungeonLevelDataModel is null or DungeonId is zero");
                return;
            }

            _levelId = _levelDataModel.Index;
            _dungeonId = _levelDataModel.DungeonId;

            _dungeonTable = TableManager.GetInstance().GetTableItem<DungeonTable>(_levelDataModel.DungeonId);
            if (_dungeonTable == null)
            {
                Logger.LogErrorFormat("DungeonTable is null and chapterId is {0}",_levelDataModel.DungeonId);
                return;
            }
            
            InitContent();
        }

        private void InitContent()
        {
            if (DungeonUtility.IsLimitTimeHellDungeon(_dungeonId) == true
                || DungeonUtility.IsWeekHellEntryDungeon(_dungeonId) == true
                || DungeonUtility.IsWeekHellPreDungeon(_dungeonId) == true)
            {
                levelNameText.SafeSetText(ChallengeUtility.GetLevelName(3));
                levelNameTextSelect.SafeSetText(ChallengeUtility.GetLevelName(3));
            }
            else
            {
                levelNameText.SafeSetText(ChallengeUtility.GetLevelName(_levelId));
                levelNameTextSelect.SafeSetText(ChallengeUtility.GetLevelName(_levelId));
            }

            recommendLevel.SafeSetText(_dungeonTable.RecommendLevel);
            recommendLevelSelect.SafeSetText(_dungeonTable.RecommendLevel);

            OnUpdateLevelItemSelected();

            InitLevelItemUnLock();
        }

        private void InitLevelItemUnLock()
        {

            ResetLevelItemStateFlag();

            ComChapterDungeonUnit.eState dungeonState = ChapterUtility.GetDungeonState(_dungeonId);

            //关卡难度解锁的状态
            //默认解锁，未通关
            eChapterNodeState levelItemState = eChapterNodeState.Unlock;
            switch (dungeonState)
            {
                //未解锁
                case ComChapterDungeonUnit.eState.Locked:
                    levelItemState = eChapterNodeState.Lock;
                    break;
                //通过
                case ComChapterDungeonUnit.eState.LockPassed:
                case ComChapterDungeonUnit.eState.Passed:
                    levelItemState = eChapterNodeState.Passed;
                    break;
                //解锁，未通过
                case ComChapterDungeonUnit.eState.Unlock:
                    if (PlayerBaseData.GetInstance().Level >= _dungeonTable.MinLevel)
                    {
                        levelItemState = eChapterNodeState.Unlock;
                    }
                    else
                    {
                        levelItemState = eChapterNodeState.LockLevel;
                    }
                    break;
            }

            switch (levelItemState)
            {
                //解锁，未通关
                case eChapterNodeState.Unlock:
                    if (unPassedRoot != null)
                        unPassedRoot.gameObject.CustomActive(true);
                    break;
                //通关
                case eChapterNodeState.Passed:
                    if (scoreRoot != null)
                        scoreRoot.gameObject.CustomActive(true);
                    //显示分数
                    UpdateLevelItemScore();
                    break;
                //未解锁，前置关卡评价不足
                case eChapterNodeState.Lock:
                    if (DungeonUtility.IsLimitTimeHellDungeon(_dungeonId) == true
                        || DungeonUtility.IsWeekHellEntryDungeon(_dungeonId) == true
                        || DungeonUtility.IsWeekHellPreDungeon(_dungeonId) == true)
                    {
                        if (beforeLevelLockRoot != null)
                        {
                            beforeLevelLockRoot.gameObject.CustomActive(true);
                        }
                        if (canvasUnSelect != null)
                        {
                            canvasUnSelect.alpha = canvasUnSelectUnLockAlpha;
                        }
                        if (beforeLevelText != null)
                        {
                            beforeLevelText.text = string.Format(TR.Value("challenge_chapter_special_hell_unlock"));
                        }
                    }
                    else
                    {
                        if (beforeLevelLockRoot != null)
                        {
                            beforeLevelLockRoot.gameObject.CustomActive(true);
                        }
                        if (canvasUnSelect != null)
                        {
                            canvasUnSelect.alpha = canvasUnSelectUnLockAlpha;
                        }

                        if (beforeLevelText != null)
                        {
                            var preLevelName = ChallengeUtility.GetPreLevelName(_levelId);
                            beforeLevelText.text = string.Format(TR.Value("challenge_chapter_pre_level_unLock"),
                                preLevelName);
                        }
                    }
                    break;
                //未解锁，等级不足
                case eChapterNodeState.LockLevel:
                    if (playerLevelLockRoot != null)
                    {
                        playerLevelLockRoot.gameObject.CustomActive(true);
                    }
                    if (canvasUnSelect != null)
                    {
                        canvasUnSelect.alpha = canvasUnSelectUnLockAlpha;
                    }
                    if (playerLevelText != null)
                    {
                        var minLevelStr = _dungeonTable.MinLevel.ToString();
                        playerLevelText.text = string.Format(TR.Value("challenge_chapter_player_level_unLock"),
                            minLevelStr);
                    }
                    break;
            }
        }

        private void UpdateLevelItemScore()
        {
            //关卡难度得分
            DungeonScore levelItemScore = ChapterUtility.GetDungeonBestScore(_dungeonId);

            switch (levelItemScore)
            {
                case DungeonScore.SSS:
                    SetImageVisible(firstSImage, true);
                    SetImageVisible(secondSImage, true);
                    SetImageVisible(thirdSImage, true);
                    break;
                case DungeonScore.SS:
                    SetImageVisible(secondSImage, true);
                    SetImageVisible(thirdSImage, true);
                    break;
                case DungeonScore.S:
                    SetImageVisible(secondSImage, true);
                    break;
                case DungeonScore.A:
                case DungeonScore.B:
                case DungeonScore.C:
                    SetImageVisible(AImage, true);
                    break;
            }
        }

        //Item是否被选中
        public void OnUpdateLevelItemSelected()
        {
            if (_levelDataModel != null)
            {
                if (itemSelectedFlag != null)
                    itemSelectedFlag.gameObject.CustomActive(_levelDataModel.IsSelected);
            }
        }

        private void OnLevelButtonClick()
        {
            if (_levelDataModel != null)
            {
                //如果已经选中，再次点击，直接返回
                if (_levelDataModel.IsSelected == true)
                {
                    return;
                }

                _levelDataModel.IsSelected = true;
                _onChapterLevelButtonClick(_levelDataModel.Index, _levelDataModel.DungeonId);
            }
        }

        private void SetImageVisible(Image sImage, bool flag)
        {
            if (sImage != null)
                sImage.gameObject.CustomActive(flag);
        }

        private void ResetLevelItemStateFlag()
        {
            if (beforeLevelLockRoot != null)
            {
                beforeLevelLockRoot.gameObject.CustomActive(false);
            }

            if (canvasUnSelect != null)
            {
                canvasUnSelect.alpha = 1;
            }

            if (playerLevelLockRoot != null)
            {
                playerLevelLockRoot.gameObject.CustomActive(false);
            }

            if (canvasUnSelect != null)
            {
                canvasUnSelect.alpha = 1;
            }

            if (unPassedRoot != null)
                unPassedRoot.gameObject.CustomActive(false);

            if (scoreRoot != null)
                scoreRoot.gameObject.CustomActive(false);

            if (AImage != null)
                AImage.gameObject.CustomActive(false);

            if (firstSImage != null)
                firstSImage.gameObject.CustomActive(false);

            if (secondSImage != null)
                secondSImage.gameObject.CustomActive(false);

            if (thirdSImage != null)
                thirdSImage.gameObject.CustomActive(false);
        }
    }
}
