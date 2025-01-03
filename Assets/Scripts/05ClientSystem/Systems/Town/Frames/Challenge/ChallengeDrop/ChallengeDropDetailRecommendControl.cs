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

    public class ChallengeDropDetailRecommendControl : MonoBehaviour
    {

        private ActivityDungeonTable _activityDungeonTable;

        private List<int> _recommendItemIdList;
        private List<int> _bestItemIdList;

        [Space(10)] [HeaderAttribute("ItemListName")] [Space(10)] [SerializeField]
        private Text recommendDropItemText;
        [SerializeField] private Text bestDropItemText;
        [SerializeField] private Text otherDropItemText;

        [Space(10)]
        [HeaderAttribute("ItemList")]
        [Space(10)]
        [SerializeField] private ComUIListScript recommendDropItemList;
        [SerializeField] private ComUIListScript bestDropItemList;

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
            if (recommendDropItemList != null)
            {
                recommendDropItemList.Initialize();
                recommendDropItemList.onItemVisiable += OnRecommendDropItemVisible;
            }

            if (bestDropItemList != null)
            {
                bestDropItemList.Initialize();
                bestDropItemList.onItemVisiable += OnBestDropItemVisible;
            }
        }

        private void UnBindEvents()
        {
            if (recommendDropItemList != null)
                recommendDropItemList.onItemVisiable -= OnRecommendDropItemVisible;

            if (bestDropItemList != null)
                bestDropItemList.onItemVisiable -= OnBestDropItemVisible;
        }

        private void ClearData()
        {
            _activityDungeonTable = null;
            _recommendItemIdList = null;
            _bestItemIdList = null;
        }

        public void InitRecommendControl(ActivityDungeonTable activityDungeonTable)
        {
            _activityDungeonTable = activityDungeonTable;

            InitContent();
        }

        private void InitContent()
        {
            InitDropDetailItemTitle();

            InitDropDetailItemList();
        }

        private void InitDropDetailItemTitle()
        {
            if (recommendDropItemText != null)
                recommendDropItemText.text =
                    ChallengeUtility.GetDropDetailTitleName(ChallengeDropDetailType.RecommendItem);

            if (bestDropItemText != null)
                bestDropItemText.text = ChallengeUtility.GetDropDetailTitleName(ChallengeDropDetailType.BestItem);

            if (otherDropItemText != null)
                otherDropItemText.text = ChallengeUtility.GetDropDetailTitleName(ChallengeDropDetailType.OtherDropItem);
        }

        private void InitDropDetailItemList()
        {
            if (_activityDungeonTable == null)
                return;

            var jobTableId = PlayerBaseData.GetInstance().JobTableID;
            var baseJobTableId = ChallengeUtility.GetPlayerBaseJobTableId();

            //职业推荐
            if (_activityDungeonTable.DropShow1 != null)
            {
                _recommendItemIdList = _activityDungeonTable.DropShow1.ToList();
                for (var i = _recommendItemIdList.Count - 1; i >= 0; i--)
                {
                    //去除非本职业的道具
                    if (ChallengeUtility.IsRecommendItemByProfession(_recommendItemIdList[i], jobTableId, baseJobTableId) == false)
                        _recommendItemIdList.RemoveAt(i);
                }

                //最多显示6个
                if (recommendDropItemList != null)
                {
                    if (_recommendItemIdList == null || _recommendItemIdList.Count <= 0)
                        recommendDropItemList.SetElementAmount(0);
                    else if (_recommendItemIdList.Count > 6)
                        recommendDropItemList.SetElementAmount(6);
                    else
                        recommendDropItemList.SetElementAmount(_recommendItemIdList.Count);
                }
            }

            //极品掉落
            if (_activityDungeonTable.DropShow2 != null)
            {
                _bestItemIdList = _activityDungeonTable.DropShow2.ToList();
                for (var i = _bestItemIdList.Count - 1; i >= 0; i--)
                {
                    //去除非本职业的道具
                    if (ChallengeUtility.IsRecommendItemByProfession(_bestItemIdList[i], jobTableId, baseJobTableId) == false)
                        _bestItemIdList.RemoveAt(i);
                }

                //最多显示6个
                if (bestDropItemList != null)
                {
                    if (_bestItemIdList == null || _bestItemIdList.Count <= 0)
                        bestDropItemList.SetElementAmount(0);
                    else if (_bestItemIdList.Count > 6)
                        bestDropItemList.SetElementAmount(6);
                    else
                        bestDropItemList.SetElementAmount(_bestItemIdList.Count);
                }
            }
        }

        private void OnRecommendDropItemVisible(ComUIListElementScript item)
        {
            if (item == null)
                return;

            if (recommendDropItemList == null)
                return;

            if (_recommendItemIdList == null)
                return;

            if (item.m_index < 0 || item.m_index >= _recommendItemIdList.Count)
                return;

            var dropItemId = _recommendItemIdList[item.m_index];
            var chapterDropItem = item.GetComponent<ChallengeChapterDropItem>();

            if (chapterDropItem != null && dropItemId > 0)
            {
                chapterDropItem.InitItem(dropItemId);
            }
        }

        private void OnBestDropItemVisible(ComUIListElementScript item)
        {
            if (item == null)
                return;

            if (bestDropItemList == null)
                return;

            if (_bestItemIdList == null)
                return;

            if (item.m_index < 0 || item.m_index >= _bestItemIdList.Count)
                return;

            var dropItemId = _bestItemIdList[item.m_index];
            var chapterDropItem = item.GetComponent<ChallengeChapterDropItem>();

            if (chapterDropItem != null && dropItemId > 0)
            {
                chapterDropItem.InitItem(dropItemId);
            }
        }

    }
}
