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

    public class ChallengeDropDetailView : MonoBehaviour
    {
        private int _baseDungeonId;
        private ActivityDungeonTable _activityDungeonTable;

        private List<int> _otherDropItemIdList;

        [SerializeField] private ChallengeDropDetailRecommendControl recommendControl;

        [Space(10)] [HeaderAttribute("OtherDropItem")] [Space(10)] [SerializeField]
        private ComUIListScript otherDropItemList;


        //[SerializeField] private ComUIListScript dropTypeList;
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
            if (otherDropItemList != null)
            {
                otherDropItemList.Initialize();
                otherDropItemList.onItemVisiable += OnOtherDropItemVisible;
            }
        }

        private void UnBindEvents()
        {
            if (otherDropItemList != null)
                otherDropItemList.onItemVisiable -= OnOtherDropItemVisible;
        }

        private void ClearData()
        {
            _baseDungeonId = 0;
            _activityDungeonTable = null;
            _otherDropItemIdList = null;
        }

        public void InitView(int dungeonId)
        {
            _baseDungeonId = dungeonId;
            _activityDungeonTable = ChallengeUtility.GetActivityDungeonTableByDungeonId(_baseDungeonId);
            
            if (_activityDungeonTable == null )
            {
                Logger.LogErrorFormat("ActivityDungeonTable is null");
                return;
            }

            if (recommendControl != null)
                recommendControl.InitRecommendControl(_activityDungeonTable);

            //其他掉落
            _otherDropItemIdList = _activityDungeonTable.DropShow3.ToList();
            if (otherDropItemList != null)
            {
                if (_otherDropItemIdList == null || _otherDropItemIdList.Count <= 0)
                    otherDropItemList.SetElementAmount(0);
                else
                    otherDropItemList.SetElementAmount(_otherDropItemIdList.Count);
            }
        }

        private void OnOtherDropItemVisible(ComUIListElementScript item)
        {
            if (item == null)
                return;

            if (_otherDropItemIdList == null)
                return;

            if (otherDropItemList == null)
                return;

            if (item.m_index < 0 || item.m_index >= _otherDropItemIdList.Count)
                return;

            var dropItemId = _otherDropItemIdList[item.m_index];
            var chapterDropItem = item.GetComponent<ChallengeChapterDropItem>();

            if (chapterDropItem != null && dropItemId > 0)
            {
                chapterDropItem.InitItem(dropItemId);
            }
        }


    }
}
