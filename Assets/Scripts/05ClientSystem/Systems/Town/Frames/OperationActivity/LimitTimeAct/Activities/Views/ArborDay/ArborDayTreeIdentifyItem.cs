using System;
using ProtoTable;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GameClient
{
    public class ArborDayTreeIdentifyItem : MonoBehaviour
    {
        private int _treeIdentifyItemId;
        private ItemTable _itemTable;
        //树的编号
        private int _treeIndex;

        private string _treeIdentifyItemIdStr;

        [Space(10)] [HeaderAttribute("TreeRoot")] [Space(10)]
        [SerializeField] private Text treeIndexLabel;

        [SerializeField] private GameObject treeIdentifyItemRoot;
        [SerializeField] private GameObject treeIdentifyItemOwnerFlag;

        private void OnDestroy()
        {
            ClearData();
        }

        private void ClearData()
        {
            _treeIdentifyItemId = 0;
            _itemTable = null;
            _treeIdentifyItemIdStr = null;
            _treeIndex = 0;
        }

        private void OnEnable()
        {
            BindUiMessages();
        }

        private void OnDisable()
        {
            UnBindUiMessages();
        }

        #region UIEvent
        private void BindUiMessages()
        {
            //count值改变
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnCountValueChange,
                OnCountValueChangeChanged);
        }

        private void UnBindUiMessages()
        {
            //count值改变
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnCountValueChange,
                OnCountValueChangeChanged);
        }


        private void OnCountValueChangeChanged(UIEvent uiEvent)
        {
            if (_treeIdentifyItemId <= 0)
                return;

            if (uiEvent == null || uiEvent.Param1 == null)
                return;

            string countKey = (string)uiEvent.Param1;

            //成熟度
            if (string.Equals(countKey, _treeIdentifyItemIdStr) == true)
            {
                UpdateItemOwnerFlag();
            }
        }
        #endregion

        public void InitItem(int itemId, 
            string itemIdStr,
            int treeIndex)
        {
            _treeIdentifyItemId = itemId;
            _treeIdentifyItemIdStr = itemIdStr;
            _treeIndex = treeIndex;

            if (_treeIdentifyItemId <= 0)
                return;

            _itemTable = TableManager.GetInstance().GetTableItem<ItemTable>(_treeIdentifyItemId);
            if (_itemTable == null)
                return;

            InitView();
        }

        private void InitView()
        {
            InitItemBaseView();
            UpdateItemOwnerFlag();
        }

        private void InitItemBaseView()
        {
            //数目的编号
            if (treeIndexLabel != null)
            {
                var treeIndexStr = TR.Value("Arbor_Day_Tree_Index_Format", _treeIndex);
                treeIndexLabel.text = treeIndexStr;
            }

            if (treeIdentifyItemRoot != null)
            {

                var commonNewItem = treeIdentifyItemRoot.GetComponentInChildren<CommonNewItem>();
                if (commonNewItem == null)
                {
                    commonNewItem = CommonUtility.CreateCommonNewItem(treeIdentifyItemRoot);
                }

                if (commonNewItem != null)
                    commonNewItem.InitItem(_treeIdentifyItemId);
            }
        }

        private void UpdateItemOwnerFlag()
        {
            var counterValue = ArborDayUtility.GetCounterValueByCounterStr(_treeIdentifyItemIdStr);
            //获得过
            if (counterValue > 0)
            {
                CommonUtility.UpdateGameObjectVisible(treeIdentifyItemOwnerFlag, true);
            }
            else
            {
                CommonUtility.UpdateGameObjectVisible(treeIdentifyItemOwnerFlag, false);
            }
        }

        public void RecycleItem()
        {
            _treeIdentifyItemId = 0;
            _treeIdentifyItemIdStr = null;
            _itemTable = null;
        }

        public void UpdateItem()
        {
            if (_itemTable == null)
                return;

            UpdateItemOwnerFlag();
        }

       
    }
}
