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

    public class ChallengeDropDetailItem : MonoBehaviour
    {

        private int _dungeonId;
        private DungeonTable _dungeonTable;

        private ChallengeDropDetailType _dropDetailType;

        private List<int> _dropDetailItemList;


        [Space(10)]
        [HeaderAttribute("Item")]
        [Space(10)]
        [SerializeField] private Text detailNameText;

        [SerializeField] private ComUIListScript dropItemList;

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
        }

        private void UnBindEvents()
        {
            if (dropItemList != null)
                dropItemList.onItemVisiable -= OnDropItemVisible;
        }

        private void ClearData()
        {
            _dungeonTable = null;
            _dungeonId = 0;
            _dropDetailItemList = null;
            _dropDetailType = ChallengeDropDetailType.None;
        }

        public void InitItem(int dungeonId, ChallengeDropDetailType dropDetailType)
        {
            _dungeonId = dungeonId;
            _dropDetailType = dropDetailType;

            _dungeonTable = TableManager.GetInstance().GetTableItem<DungeonTable>(_dungeonId);

            if (_dungeonTable == null)
            {
                Logger.LogErrorFormat("DungeonTable is null and dungeonId is {0}", _dungeonId);
                return;
            }

            if (dropDetailType == ChallengeDropDetailType.None)
            {
                Logger.LogErrorFormat("DropDetailType is None ");
                return;
            }
         
            InitContent();
            
        }

        private void InitContent()
        {
            InitDropDetailItemTitle();

            InitDropDetailItemList();
        }

        private void InitDropDetailItemTitle()
        {
            string dropTitle = ChallengeUtility.GetDropDetailTitleName(_dropDetailType);
            if (detailNameText != null)
                detailNameText.text = dropTitle;
        }

        private void InitDropDetailItemList()
        {
            _dropDetailItemList = _dungeonTable.DropItems.ToList();

            int itemCount = 0;
            if (_dropDetailItemList != null)
                itemCount = _dropDetailItemList.Count;

            if (dropItemList != null)
                dropItemList.SetElementAmount(itemCount);
        }

        private void OnDropItemVisible(ComUIListElementScript item)
        {
            if (item == null)
                return;

            if (_dropDetailItemList == null)
                return;

            if (dropItemList == null)
                return;

            if (item.m_index < 0 || item.m_index >= _dropDetailItemList.Count)
                return;

            var dropItemId = _dropDetailItemList[item.m_index];
            var chapterDropItem = item.GetComponent<ChallengeChapterDropItem>();

            if (chapterDropItem != null && dropItemId > 0)
            {
                chapterDropItem.InitItem(dropItemId, _dungeonId);
            }
        }
        
    }
}
