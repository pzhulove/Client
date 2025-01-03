using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ProtoTable;
using System;

namespace GameClient
{
    public class PKSeasonDetaiFashionItem : MonoBehaviour,IDisposable
    {
        [SerializeField]private Text mName;
        [SerializeField]private GameObject mItemRoot;
        [SerializeField]private string mDesc="至尊王者送时装";
        private ComItem comItem;
        public void InitItem()
        {
            var systemValueTable = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType2.SVT_DUELREWARD_SHOW_ID);
            int fashionId = systemValueTable.Value;
            var fashinItemData = ItemDataManager.CreateItemDataFromTable(fashionId);
            if (fashinItemData == null)
            {
                Logger.LogErrorFormat("[PKSeasonDetaiFashionItem]检查系统数值Id = {0}的Value = {1}是否异常", systemValueTable.ID, systemValueTable.Value);
            }
            if (comItem == null)
            {
                comItem = ComItemManager.Create(mItemRoot);
            }
            comItem.Setup(fashinItemData, OnItemClick);

            mName.text = mDesc;
        }

        private void OnItemClick(GameObject obj, ItemData item)
        {
            ClientSystemManager.GetInstance().OpenFrame<PlayerTryOnFrame>(FrameLayer.Middle, item.TableID);
        }

        public void Dispose()
        {
            if (comItem != null)
            {
                comItem = null;
            }
        }

        private void OnDestroy()
        {
            Dispose();
        }
    }
}

