using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;

namespace GameClient
{
    //下拉单的类型
    public enum CommonNewDropDownListType
    {
        None,
        DownType,
        UpType,
    }

    public delegate void OnCommonNewDropDownItemButtonClick(ComControlData comControlData);

    public class CommonNewDropDownControl : MonoBehaviour
    {
        //数据：默认选中 下拉框
        private ComControlData _commonNewControlData;
        private List<ComControlData> _commonNewDropDownDataList;

        //点击回调
        private OnCommonNewDropDownItemButtonClick _onCommonNewDropDownItemButtonClick;
        private Action _onResetDropDownListAction;

        //下拉单控制器
        private CommonNewDropDownListView _commonNewDropDownListView;

        [Space(20)]
        [HeaderAttribute("Content")]
        [Space(10)]
        [SerializeField] private Text dropDownLabelName;            //默认选中名字
        [SerializeField] private Button dropDownButton;             //打开下拉单
        [SerializeField] private Image defaultArrow;                     //默认箭头
        [SerializeField] private Image selectedArrow;                   //选中箭头

        [Space(20)]
        [HeaderAttribute("DropDownListType")]
        [Space(10)]
        [SerializeField] private CommonNewDropDownListType dropDownListType = CommonNewDropDownListType.DownType;

        [Space(20)] [HeaderAttribute("DropDownListRoot")] [Space(10)]
        [SerializeField] private GameObject dropDownListRoot;
        [SerializeField] private string dropDownListPath;

        [Space(20)]
        [HeaderAttribute("dropDownListClose")]
        [Space(10)]
        [SerializeField] private Button dropDownListCloseButton;        //关闭掉下拉单


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

            if (dropDownListCloseButton != null)
            {
                dropDownListCloseButton.onClick.RemoveAllListeners();
                dropDownListCloseButton.onClick.AddListener(OnDropDownListCloseButton);
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

            if (dropDownListCloseButton != null)
                dropDownListCloseButton.onClick.RemoveAllListeners();
        }

        private void ClearData()
        {
            _commonNewControlData = null;
            _commonNewDropDownDataList = null;

            _onCommonNewDropDownItemButtonClick = null;
            _onResetDropDownListAction = null;

            _commonNewDropDownListView = null;
        }
        #endregion

        #region InitDropDown
        //初始化通用的下拉框
        public void InitComDropDownControl(ComControlData comControlData,
            List<ComControlData> comDropDownDataList,
            OnCommonNewDropDownItemButtonClick onCommonNewDropDownItemButtonClick,
            Action onResetDropDownAction = null)
        {
            ClearData();

            _commonNewControlData = comControlData;
            if (_commonNewControlData == null)
            {
                Logger.LogError("CommonNewControlData is null");
                return;
            }

            _commonNewDropDownDataList = comDropDownDataList;
            if (_commonNewDropDownDataList == null || _commonNewDropDownDataList.Count <= 0)
            {
                Logger.LogError("ComDropDownDataList is null");
                return;
            }

            _onCommonNewDropDownItemButtonClick = onCommonNewDropDownItemButtonClick;
            _onResetDropDownListAction = onResetDropDownAction;

            InitCommonNewDropDownControl();
        }

        //初始化UI
        private void InitCommonNewDropDownControl()
        {
            UpdateDropDownLabelName();
            UpdateDropDownList(false);
        }

        #endregion

        #region DropDownButton

        private void OnDropDownButtonClick()
        {
            if (_onResetDropDownListAction != null)
                _onResetDropDownListAction();

            if (_commonNewDropDownDataList == null || _commonNewDropDownDataList.Count <= 0)
                return;

            if (_commonNewDropDownListView == null)
            {
                UpdateDropDownList(true);

                var dropDownListPrefab = AssetLoader.GetInstance().LoadResAsGameObject(dropDownListPath);
                if (dropDownListPrefab != null)
                {
                    dropDownListPrefab.transform.SetParent(dropDownListRoot.transform, false);

                    UpdateDropDownListPrefabPosition(dropDownListPrefab);

                    _commonNewDropDownListView = dropDownListPrefab.GetComponent<CommonNewDropDownListView>();

                    //初始化List
                    _commonNewDropDownListView.InitCommonNewDropDownListView(
                        _commonNewControlData,
                        _commonNewDropDownDataList,
                        OnCommonNewDropDownItemButtonClick);
                }
            }
            else
            {
                if (dropDownListRoot.gameObject.activeInHierarchy == true)
                {
                    UpdateDropDownList(false);
                }
                else
                {
                    UpdateDropDownList(true);
                    _commonNewDropDownListView.UpdateSelectedItemPosition();
                }
            }
        }

        private void OnCommonNewDropDownItemButtonClick(ComControlData comControlData)
        {
            if (comControlData == null)
                return;

            UpdateDropDownList(false);

            _commonNewControlData = comControlData;
            UpdateDropDownLabelName();

            if (_onCommonNewDropDownItemButtonClick != null)
                _onCommonNewDropDownItemButtonClick(_commonNewControlData);
        }

        #endregion

        //外部直接更新选中的Item
        public void UpdateCommonNewDropDownSelectedItem(ComControlData selectedControlData)
        {
            if (selectedControlData == null)
                return;

            //更新数值
            _commonNewControlData = selectedControlData;

            //更新展示
            UpdateDropDownLabelName();

            //更新List列表
            if (_commonNewDropDownListView != null)
            {
                _commonNewDropDownListView.UpdateCommonNewDropDownDataList(_commonNewControlData);
                _commonNewDropDownListView.UpdateCommonNewDropDownItemList();
            }
        }

        #region Helper

        //更新DropDownList的位置
        private void UpdateDropDownListPrefabPosition(GameObject dropDownListPrefab)
        {
            if (dropDownListPrefab == null)
                return;

            var prefabRtf = (dropDownListPrefab.transform as RectTransform);
            var listRootRtf = (dropDownListRoot.transform as RectTransform);

            if (prefabRtf == null || listRootRtf == null)
                return;

            //下面
            if (dropDownListType == CommonNewDropDownListType.DownType)
            {
                dropDownListPrefab.transform.localPosition = new Vector3(dropDownListPrefab.transform.localPosition.x,
                    dropDownListPrefab.transform.localPosition.y -
                    (prefabRtf.sizeDelta.y + listRootRtf.sizeDelta.y) / 2.0f,
                    dropDownListPrefab.transform.localPosition.z);
            }
            else if (dropDownListType == CommonNewDropDownListType.UpType)
            {
                //上面
                dropDownListPrefab.transform.localPosition = new Vector3(dropDownListPrefab.transform.localPosition.x,
                    dropDownListPrefab.transform.localPosition.y +
                    (prefabRtf.sizeDelta.y + listRootRtf.sizeDelta.y) / 2.0f,
                    dropDownListPrefab.transform.localPosition.z);
            }
        }

        private void OnDropDownListCloseButton()
        {
            UpdateDropDownList(false);
        }

        //选中的内容
        private void UpdateDropDownLabelName()
        {
            if (dropDownLabelName != null)
                dropDownLabelName.text = _commonNewControlData.Name;
        }

        //更新显示与否的内容
        public void UpdateDropDownList(bool flag)
        {
            CommonUtility.UpdateButtonVisible(dropDownListCloseButton, flag);
            CommonUtility.UpdateGameObjectVisible(dropDownListRoot, flag);
            CommonUtility.UpdateImageVisible(selectedArrow, flag);
            CommonUtility.UpdateImageVisible(defaultArrow, !flag);
        }
        #endregion

    }
}