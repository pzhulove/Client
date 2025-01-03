using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;

namespace GameClient
{

    public delegate void OnComDropDownItemButtonClick(ComControlData comControlData);

    public class ComDropDownControl : MonoBehaviour
    {
        //数据：默认选中和下拉框
        private ComControlData _comControlData = null;
        private List<ComControlData> _comDropDownDataList = new List<ComControlData>();

        //点击回调
        private OnComDropDownItemButtonClick _onComDropDownItemButtonClick;
        private Action _onResetDropDownListAction;
        
        [SerializeField] private Text dropDownLabelName;            //默认选中名字
        [SerializeField] private Button dropDownButton;             //打开下拉单
        [SerializeField] private Button dropDownCloseButton;        //关闭掉下拉单

        [SerializeField] private ComUIListScript dropDownItemList;  //下拉单List
        [SerializeField] private GameObject dropDownItemListRoot;   //下拉单Root;
        [SerializeField] private RectTransform arrow;
        [SerializeField] private bool dropBtnOnLeft;
        [SerializeField] private bool showAtTop;
        private float dropLeftPos = -68.5f;
        private float textRightPos = 24f;
        private float dropRightPos = 62.6f;
        private float textLeftPos = -31f;
        
        

        #region DropDownData
        private void Awake()
        {
            BindUiEventSystem();
        }

        private void BindUiEventSystem()
        {
            if (dropDownButton != null)
            {
                dropDownButton.onClick.RemoveAllListeners();
                dropDownButton.onClick.AddListener(OnDropDownButtonClick);
            }

            if (dropDownCloseButton != null)
            {
                dropDownCloseButton.onClick.RemoveAllListeners();
                dropDownCloseButton.onClick.AddListener(OnDropDownCloseButton);
            }

            if (dropDownItemList != null)
            {
                dropDownItemList.onItemVisiable += OnDropDownItemVisible;
                dropDownItemList.OnItemUpdate += OnDropDownItemUpdate;
            }
        }

        private void OnDestroy()
        {
            UnBindUiEventSystem();
            ClearData();
        }

        private void UnBindUiEventSystem()
        {
            if (dropDownButton != null)
                dropDownButton.onClick.RemoveAllListeners();

            if (dropDownCloseButton != null)
                dropDownCloseButton.onClick.RemoveAllListeners();

            if (dropDownItemList != null)
            {
                dropDownItemList.onItemVisiable -= OnDropDownItemVisible;
                dropDownItemList.OnItemUpdate -= OnDropDownItemUpdate;
            }

        }

        private void ClearData()
        {
            _comControlData = null;
            _comDropDownDataList = null;
            _onComDropDownItemButtonClick = null;
            _onResetDropDownListAction = null;
        }
        #endregion

        #region InitDropDown
        //初始化通用的下拉框
        public void InitComDropDownControl(ComControlData comControlData/*默认选中菜单的数据*/,
            List<ComControlData> comDropDownDataList/*下拉菜单数据*/,
            OnComDropDownItemButtonClick onComDropDownItemButtonClick/*选中菜单回调*/,
            Action onResetDropDownAction = null)
        {
            ClearData();

            _comControlData = comControlData;
            if (_comControlData == null)
            {
                Logger.LogError("ComDropDownData is null");
                return;
            }

            _comDropDownDataList = comDropDownDataList;
            if (_comDropDownDataList == null || _comDropDownDataList.Count <= 0)
            {
                Logger.LogError("ComDropDownDataList is null");
                return;
            }

            _onComDropDownItemButtonClick = onComDropDownItemButtonClick;
            _onResetDropDownListAction = onResetDropDownAction;

            //设置Item是否被选中
            for (var i = 0; i < _comDropDownDataList.Count; i++)
            {
                _comDropDownDataList[i].IsSelected = _comDropDownDataList[i].Id == _comControlData.Id;
            }
            SetDorpPos();
            SetListDirection();
            InitDropDownControlView();
        }


        public void ClearAndRefresh()
        {
            ClearData();
            
            if (_onResetDropDownListAction != null)
                _onResetDropDownListAction();

            if (dropDownItemListRoot != null)
                dropDownItemListRoot.CustomActive(false);

            if (dropDownCloseButton != null)
                dropDownCloseButton.gameObject.CustomActive(false);
            
            if (dropDownLabelName != null)
            {
                dropDownLabelName.SafeSetText("");
            }
        }

        void SetDorpPos()
        {
            if (null == dropDownLabelName || null == arrow)
            {
                return;
            }
            
            if (dropBtnOnLeft)
            {
                Vector3 dropPos = arrow.localPosition;
                Vector3 dropTextPos = dropDownLabelName.rectTransform.localPosition;
                
                arrow.localPosition = new Vector3(dropLeftPos,dropPos.y,dropPos.z);
                dropDownLabelName.rectTransform.localPosition = new Vector3(textRightPos,dropTextPos.y,dropTextPos.z);
            }
            
        }

        void SetListDirection()
        {
            if (null == dropDownItemListRoot || null == dropDownButton)
            {
                return;
            }
            
            float halfContentHeight = dropDownItemListRoot.transform.rectTransform().rect.height/2;
            float halfBtnHeight = dropDownButton.rectTransform().rect.height / 2;
            float height = halfContentHeight + halfBtnHeight;
            Vector2 listPos = dropDownItemListRoot.transform.rectTransform().anchoredPosition;
            if (showAtTop)
            {
                dropDownItemListRoot.transform.rectTransform().anchoredPosition = new Vector2(listPos.x,listPos.y + height);
            }
            else
            {
                dropDownItemListRoot.transform.rectTransform().anchoredPosition = new Vector2(listPos.x,listPos.y - height);
            }
            
        }

        private void SetArrowDirecton()
        {
            if (null != arrow)
            {
                Vector3 arrowScale = arrow.rectTransform().localScale;
                
                arrow.rectTransform().localScale = new Vector3(arrowScale.x,-arrowScale.y,arrowScale.z);
            }
        }

        //初始化UI
        private void InitDropDownControlView()
        {
            if (dropDownLabelName != null)
            {
                if (_comDropDownDataList.Count <= 0)
                {
                    dropDownLabelName.SafeSetText("");
                }
                else
                {
                    dropDownLabelName.SafeSetText(_comControlData.Name);
                }
            }


            if (dropDownItemListRoot != null)
                dropDownItemListRoot.gameObject.CustomActive(false);
        }

        #endregion

        #region DropDownButton
        private void OnDropDownButtonClick()
        {
            if (dropDownItemListRoot != null)
            {
                if (dropDownItemListRoot.gameObject.activeInHierarchy == true)
                {
                    UpdateDropDownItemListVisible(false);
                }
                else
                {
                    //第一次显示的时候进行初始化
                    if (dropDownItemList.IsInitialised() == false)
                    {
                        dropDownItemList.Initialize();
                        dropDownItemList.SetElementAmount(_comDropDownDataList.Count);
                    }

                    if (_comDropDownDataList != null)
                    {
                        if (_comDropDownDataList.Count > 0)
                        {
                            UpdateDropDownItemListVisible(true);
                            UpdateDropDownItemListPosition();    
                        }   
                    }
                    
                }
            }
        }

        private void OnDropDownItemButtonClick(ComControlData comControlData)
        {
            if (comControlData == null)
                return;

            //if (comControlData.Id == _comControlData.Id)
            //    return;

            _comControlData = comControlData;

            UpdateDropDownLabelName();

            DisableDropDownListRoot();

            if (_onComDropDownItemButtonClick != null)
            {
                _onComDropDownItemButtonClick(_comControlData);
            }

            UpdateComDropDownDataListSelectedInfo();
        }

        private void OnDropDownCloseButton()
        {
            UpdateDropDownItemListVisible(false);
        }
        #endregion

        #region ItemList
        //List显示
        private void UpdateDropDownItemListVisible(bool flag)
        {

            if (_onResetDropDownListAction != null)
                _onResetDropDownListAction();

            if (dropDownItemListRoot != null)
                dropDownItemListRoot.CustomActive(flag);

            if (dropDownCloseButton != null)
                dropDownCloseButton.gameObject.CustomActive(flag);
            
            SetArrowDirecton();
        }

        //List位置更新
        private void UpdateDropDownItemListPosition()
        {
            if (dropDownItemList != null)
                dropDownItemList.ResetContentPosition();
            MoveDropDownListToSelectedItem();
        }

        //List位置移动
        private void MoveDropDownListToSelectedItem()
        {
            if (dropDownItemList == null)
                return;

            var index = GetSelectedFilterDataIndex();

            if (index <= 1)
            {
                dropDownItemList.MoveElementInScrollArea(0, true);
            }
            else if (index >= _comDropDownDataList.Count - 2)
            {
                dropDownItemList.MoveElementInScrollArea(_comDropDownDataList.Count - 1, true);
            }
            else
            {
                dropDownItemList.MoveElementInScrollArea(index + 1, true);
            }
        }

        private void OnDropDownItemVisible(ComUIListElementScript item)
        {
            if (item == null)
                return;

            if (dropDownItemList == null)
                return;

            if (_comDropDownDataList == null)
                return;

            if (item.m_index < 0 || item.m_index >= _comDropDownDataList.Count)
                return;

            var dropDownData = _comDropDownDataList[item.m_index];
            var dropDownItem = item.GetComponent<ComDropDownItem>();

            if (dropDownItem != null && dropDownData != null)
            {
                dropDownItem.InitItem(dropDownData, OnDropDownItemButtonClick);
            }
        }

        private void OnDropDownItemUpdate(ComUIListElementScript item)
        {
            if (item == null)
                return;

            var comDropDownItem = item.GetComponent<ComDropDownItem>();
            if (comDropDownItem != null)
                comDropDownItem.UpdateComDropDownItem();
        }

        #endregion

        #region Helper

        private void UpdateComDropDownDataListSelectedInfo()
        {
            if (_comControlData == null)
                return;

            if (_comDropDownDataList == null || _comDropDownDataList.Count <= 0)
                return;

            for (var i = 0; i < _comDropDownDataList.Count; i++)
            {

                if (_comDropDownDataList[i] != null
                    && _comDropDownDataList[i].Id == _comControlData.Id)
                {
                    _comDropDownDataList[i].IsSelected = true;
                }
                else
                {
                    _comDropDownDataList[i].IsSelected = false;
                }
            }

            if (dropDownItemList != null)
                dropDownItemList.UpdateElement();
        }

        private void DisableDropDownListRoot()
        {
            UpdateDropDownItemListVisible(false);
        }


        //选中的内容
        private void UpdateDropDownLabelName()
        {
            if (dropDownLabelName != null)
                dropDownLabelName.text = _comControlData.Name;
        }

        //选中Item的索引
        private int GetSelectedFilterDataIndex()
        {

            if (_comDropDownDataList == null || _comDropDownDataList.Count <= 0)
                return 0;

            if (_comControlData == null)
                return 0;

            for (var i = 0; i < _comDropDownDataList.Count; i++)
            {
                var curDropDownData = _comDropDownDataList[i];
                if (curDropDownData == null)
                    continue;

                if (curDropDownData.Id == _comControlData.Id)
                {
                    return i;
                }
            }

            return 0;
        }

        public void ResetDropDownList()
        {
            if (dropDownItemListRoot != null)
                dropDownItemListRoot.gameObject.CustomActive(false);

            if (dropDownButton != null)
                dropDownButton.gameObject.CustomActive(false);
        }

        #endregion

    }
}