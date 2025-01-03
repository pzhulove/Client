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
    public class ChallengeModelDropControl : MonoBehaviour
    {

        private DungeonModelTable _dungeonModelTable;
        private List<int> _dropItemIdList;
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
            _dropItemIdList = null;
            _dungeonModelTable = null;
        }

        public void InitModelControl(DungeonModelTable dungeonModelTable)
        {
            _dungeonModelTable = dungeonModelTable;
            if (_dungeonModelTable == null)
            {
                Logger.LogErrorFormat("DungeonModelTable is null ");
                return;
            }

            if (_dungeonModelTable.DropShow == null)
            {
                gameObject.CustomActive(false);
            }
            else
            {
                gameObject.CustomActive(true);
                _dropItemIdList = _dungeonModelTable.DropShow.ToList();

                if (dropItemList != null)
                    dropItemList.SetElementAmount(_dropItemIdList.Count);
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
                chapterDropItem.InitItem(dropItemId);
            }
        }
        
    }
}
