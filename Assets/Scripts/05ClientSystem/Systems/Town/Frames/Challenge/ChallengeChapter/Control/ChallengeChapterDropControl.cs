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

    public class ChallengeChapterDropControl : MonoBehaviour
    {

        private int _chapterId;             //选中的具体的地下城ID， 可能不同的难度
        private int _mapItemId;         //活动地下城中配置的ID
        private List<int> _dropItemIdList;

        [SerializeField] private ComUIListScript dropItemList;
        [SerializeField] private Button dropDetailButton;
        [SerializeField] private Text specialDropTitleLabel;

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
            if (dropItemList != null)
            {
                dropItemList.Initialize();
                dropItemList.onItemVisiable += OnDropItemVisible;
            }

            if (dropDetailButton != null)
            {
                dropDetailButton.onClick.RemoveAllListeners();
                dropDetailButton.onClick.AddListener(OnDropDetailButton);
            }
        }

        private void UnBindEvents()
        {
            if (dropItemList != null)
                dropItemList.onItemVisiable -= OnDropItemVisible;

            if(dropDetailButton != null)
                dropDetailButton.onClick.RemoveAllListeners();
        }

        private void ClearData()
        {
            _chapterId = 0;
            _mapItemId = 0;
            _dropItemIdList = null;
        }

        public void InitDropControl(int chapterId, int mapItemId)
        {
            _chapterId = chapterId;
            _mapItemId = mapItemId;

            var dungeonTable = TableManager.GetInstance().GetTableItem<DungeonTable>(_chapterId);
            if (dungeonTable == null)
            {
                Logger.LogErrorFormat("DungeonTable is null and chapter id is {0}", _chapterId);
                return;
            }

            if (dungeonTable.DropItems == null || dungeonTable.DropItems.Count <= 0)
            {
                if (dropItemList != null)
                    dropItemList.SetElementAmount(0);

                return;
            }

            _dropItemIdList = dungeonTable.DropItems.ToList();

            if (dropItemList != null)
                dropItemList.SetElementAmount(_dropItemIdList.Count);

            if (specialDropTitleLabel != null)
            {
                //周常深渊显示必然掉落
                if (DungeonUtility.IsWeekHellEntryDungeon(_chapterId) == true
                    || DungeonUtility.IsWeekHellPreDungeon(_chapterId) == true)
                {
                    specialDropTitleLabel.text = TR.Value("challenge_chapter_week_activity_drop");
                    specialDropTitleLabel.gameObject.CustomActive(true);
                }
                else
                {
                    specialDropTitleLabel.gameObject.CustomActive(false);
                }
            }

            UpdateDropDetailButton();
        }

        private void UpdateDropDetailButton()
        {
            if (dropDetailButton != null)
            {
                //周常深渊前置关卡不显示详情按钮
                if (DungeonUtility.IsWeekHellPreDungeon(_chapterId) == true)
                {
                    dropDetailButton.gameObject.CustomActive(false);
                }
                else
                {
                    dropDetailButton.gameObject.CustomActive(true);
                }
            }
        }

        private void OnDropItemVisible(ComUIListElementScript item)
        {
            if (item == null)
                return;

            if (_dropItemIdList == null)
                return;

            if (dropItemList == null)
                return;

            if (item.m_index < 0 || item.m_index >= _dropItemIdList.Count)
                return;

            var dropItemId = _dropItemIdList[item.m_index];
            var chapterDropItem = item.GetComponent<ChallengeChapterDropItem>();

            if (chapterDropItem != null && dropItemId > 0)
            {
                chapterDropItem.InitItem(dropItemId, _chapterId);
            }
        }

        private void OnDropDetailButton()
        {
            ChallengeUtility.OnOpenChallengeDropDetailFrame(_mapItemId);
        }

    }
}
