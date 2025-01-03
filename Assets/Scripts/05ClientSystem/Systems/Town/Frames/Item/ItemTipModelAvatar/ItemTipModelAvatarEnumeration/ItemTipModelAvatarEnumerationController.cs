using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ProtoTable;
using Scripts.UI;

namespace GameClient
{

    public delegate void OnItemTipModelAvatarEnumerationItemClick(int index);

    public class ItemTipModelAvatarEnumerationController : MonoBehaviour
    {

        private int _selectedIndex;
        private int _maxItemNumber;
        private List<ItemTipModelAvatarEnumerationDataModel> _enumerationDataModelList;
        private OnItemTipModelAvatarEnumerationItemClick _onItemTipModelAvatarEnumerationItemClick;

        [Space(10)]
        [HeaderAttribute("ComPageChangeController")]
        [Space(10)]
        [SerializeField] private ComPageChangeController comPageChangeController;

        [Space(10)] [HeaderAttribute("ItemListScriptEx")] [Space(10)]
        [SerializeField] private ComUIListScriptEx enumerationItemListEx;

        [Space(10)] [HeaderAttribute("ItemPosition")] [Space(10)]
        [SerializeField] private GameObject itemListRoot;

        [SerializeField] private GameObject itemOnePositionGo;
        [SerializeField] private GameObject itemTwoPositionGo;
        [SerializeField] private GameObject itemThreePositionGo;
        [SerializeField] private GameObject itemFourPositionGo;
        [SerializeField] private GameObject itemFivePositionGo;


        private void Awake()
        {
            BindUiEvents();
        }

        private void OnDestroy()
        {
            UnBindUiEvents();
            ClearData();
        }

        private void BindUiEvents()
        {
            if (enumerationItemListEx != null)
            {
                enumerationItemListEx.Initialize();
                enumerationItemListEx.onItemVisiable += OnItemVisible;
                enumerationItemListEx.OnItemUpdate += OnItemUpdate;
                enumerationItemListEx.OnItemRecycle += OnItemRecycle;
            }
        }

        private void UnBindUiEvents()
        {
            if (enumerationItemListEx != null)
            {
                enumerationItemListEx.onItemVisiable -= OnItemVisible;
                enumerationItemListEx.OnItemUpdate -= OnItemUpdate;
                enumerationItemListEx.OnItemRecycle -= OnItemRecycle;
            }
        }

        private void ClearData()
        {
            _onItemTipModelAvatarEnumerationItemClick = null;
            if (_enumerationDataModelList != null)
            {
                _enumerationDataModelList.Clear();
                _enumerationDataModelList = null;
            }

            _selectedIndex = 0;
            _maxItemNumber = 0;
        }

        //多种展示基础职业
        public void InitController(List<int> professionIdList,
            int selectIndex,
            OnItemTipModelAvatarEnumerationItemClick onItemTipModelAvatarEnumerationItemClick)
        {
            if (professionIdList == null || professionIdList.Count <= 0)
                return;

            _selectedIndex = selectIndex;
            _onItemTipModelAvatarEnumerationItemClick = onItemTipModelAvatarEnumerationItemClick;
            _maxItemNumber = professionIdList.Count;

            if (_enumerationDataModelList == null)
                _enumerationDataModelList = new List<ItemTipModelAvatarEnumerationDataModel>();

            _enumerationDataModelList.Clear();
            for (var i = 0; i < professionIdList.Count; i++)
            {
                var professionId = professionIdList[i];
                var index = i + 1;
                var isSelected = (_selectedIndex == index) ? true : false;
                var enumerationDataModel = new ItemTipModelAvatarEnumerationDataModel()
                {
                    Index = index,
                    IsSelectedFlag = isSelected,

                    IsPlayerProfessionType = true,
                    ProfessionId = professionId,
                };
                _enumerationDataModelList.Add(enumerationDataModel);
            }

            InitView();
        }

        //多种展示的道具
        public void InitController(List<ItemTable> giftPackItemTableList,
            int selectItemTableIndex,
            OnItemTipModelAvatarEnumerationItemClick onItemTipModelAvatarEnumerationItemClick)
        {
            if (giftPackItemTableList == null || giftPackItemTableList.Count <= 0)
                return;

            _selectedIndex = selectItemTableIndex;
            _onItemTipModelAvatarEnumerationItemClick = onItemTipModelAvatarEnumerationItemClick;
            _maxItemNumber = giftPackItemTableList.Count;

            if (_enumerationDataModelList == null)
                _enumerationDataModelList = new List<ItemTipModelAvatarEnumerationDataModel>();

            _enumerationDataModelList.Clear();
            for (var i = 0; i < giftPackItemTableList.Count; i++)
            {
                var itemTable = giftPackItemTableList[i];
                var index = i + 1;
                var isSelected = (selectItemTableIndex == index) ? true : false;
                var enumerationDataModel = new ItemTipModelAvatarEnumerationDataModel()
                {
                    Index = index,
                    ItemTable = itemTable,
                    IsSelectedFlag = isSelected,
                };
                _enumerationDataModelList.Add(enumerationDataModel);
            }

            InitView();
        }

        private void InitView()
        {
            var enumerationItemCount = 0;
            if (_enumerationDataModelList != null)
                enumerationItemCount = _enumerationDataModelList.Count;

            if (enumerationItemListEx != null)
            {
                enumerationItemListEx.SetElementAmount(enumerationItemCount);
            }

            //更新位置
            UpdateEnumerationItemListPosition();

            InitPageController();
        }

        public void OnEnableController(int selectedIndex)
        {
            _selectedIndex = selectedIndex;

            UpdateEnumerationItemList();
            UpdatePageController();
        }

        #region PageController
        private void InitPageController()
        {
            if (comPageChangeController != null)
                comPageChangeController.InitPageChangeController(_selectedIndex,
                    _maxItemNumber,
                    OnItemPageChangeAction);
        }

        private void UpdatePageController()
        {
            if (comPageChangeController != null)
                comPageChangeController.UpdatePageChangeController(_selectedIndex);
        }

        //切页按钮
        private void OnItemPageChangeAction(int selectedIndex)
        {
            _selectedIndex = selectedIndex;

            UpdateEnumerationItemList();

            if (_onItemTipModelAvatarEnumerationItemClick != null)
                _onItemTipModelAvatarEnumerationItemClick(selectedIndex);
        }
        #endregion

        #region EnumerationItem
        public void UpdateEnumerationItemList()
        {
            if (_enumerationDataModelList == null || _enumerationDataModelList.Count <= 0)
                return;

            //更新数据
            for (var i = 0; i < _enumerationDataModelList.Count; i++)
            {
                var enumerationDataModel = _enumerationDataModelList[i];
                if (enumerationDataModel == null)
                    continue;

                if (enumerationDataModel.Index == _selectedIndex)
                {
                    enumerationDataModel.IsSelectedFlag = true;
                }
                else
                {
                    enumerationDataModel.IsSelectedFlag = false;
                }
            }

            //更新Item
            if (enumerationItemListEx != null)
                enumerationItemListEx.UpdateElement();
        }

        //ItemList中道具点击
        private void OnItemClickAction(int selectedIndex)
        {
            _selectedIndex = selectedIndex;
            
            UpdateEnumerationItemList();

            UpdatePageController();

            if (_onItemTipModelAvatarEnumerationItemClick != null)
                _onItemTipModelAvatarEnumerationItemClick(selectedIndex);
        }
        #endregion

        #region UIEvents

        private void OnItemVisible(ComUIListElementScript item)
        {
            if (item == null)
                return;

            if (enumerationItemListEx == null)
                return;

            if (_enumerationDataModelList == null || _enumerationDataModelList.Count <= 0)
                return;

            var enumerationDataModel = _enumerationDataModelList[item.m_index];
            var enumerationItem = item.GetComponent<ItemTipModelAvatarEnumerationItem>();
            if (enumerationItem != null && enumerationDataModel != null)
            {
                enumerationItem.InitItem(enumerationDataModel,
                    OnItemClickAction);
            }
        }

        private void OnItemUpdate(ComUIListElementScript item)
        {
            if (item == null)
                return;

            var enumerationItem = item.GetComponent<ItemTipModelAvatarEnumerationItem>();
            if(enumerationItem != null)
                enumerationItem.UpdateItem();
        }

        private void OnItemRecycle(ComUIListElementScript item)
        {
            if (item == null)
                return;

            var enumerationItem = item.GetComponent<ItemTipModelAvatarEnumerationItem>();
            if(enumerationItem != null)
                enumerationItem.RecycleItem();
        }

        #endregion

        #region Helper
        //根据Item的数量更新ItemList的位置
        private void UpdateEnumerationItemListPosition()
        {
            if (_maxItemNumber >= 6)
                return;

            if (itemListRoot == null)
                return;

            //当前的位置
            var itemListLocalPosition = itemListRoot.transform.localPosition;
            var currentLocalPositionX = itemListLocalPosition.x;

            //如果当前的数量是1-5个，修改X的位置
            if (_maxItemNumber == 1)
            {
                if (itemOnePositionGo != null)
                    currentLocalPositionX = itemOnePositionGo.transform.localPosition.x;
            }
            else if (_maxItemNumber == 2)
            {
                if (itemTwoPositionGo != null)
                    currentLocalPositionX = itemTwoPositionGo.transform.localPosition.x;
            }
            else if (_maxItemNumber == 3)
            {
                if (itemThreePositionGo != null)
                    currentLocalPositionX = itemThreePositionGo.transform.localPosition.x;
            }
            else if (_maxItemNumber == 4)
            {
                if (itemFourPositionGo != null)
                    currentLocalPositionX = itemFourPositionGo.transform.localPosition.x;
            }
            else if (_maxItemNumber == 5)
            {
                if (itemFivePositionGo != null)
                    currentLocalPositionX = itemFivePositionGo.transform.localPosition.x;
            }

            itemListRoot.transform.localPosition = new Vector3(currentLocalPositionX,
                itemListLocalPosition.y,
                itemListLocalPosition.z);

        }
        #endregion

    }
}
