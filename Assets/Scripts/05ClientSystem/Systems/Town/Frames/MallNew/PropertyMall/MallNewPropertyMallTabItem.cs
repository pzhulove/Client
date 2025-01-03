using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ProtoTable;
using Protocol;

namespace GameClient
{
    //道具商城的Tab类型,只是起到提示的作用
    public enum MallNewPropertyMallTabType
    {
        None = -1,                  
        Recommend,      //推荐            商城表中ID:  13
        Consume,        //消耗品                       3
        Material,       //材料                         14
        Gold,           //金币                         5

        Item,           //道具                         17
        Function,       //功能                         18
        Exchange,       //兑换                         19
        Medicine,       //药品                         20
    }

    [Serializable]
    public class MallNewPropertyMallTabData
    {
        public int Index;
        public MallNewPropertyMallTabType PropertyMallTabType;
        public int MallTypeTableId;
    }

    public delegate void OnPropertyMallTabValueChanged(int index, int mallTypeTableId);


    public class MallNewPropertyMallTabItem : MonoBehaviour
    {

        private OnPropertyMallTabValueChanged onToggleValueChanged = null;

        private bool _isSelected = false;
        private MallNewPropertyMallTabData _propertyMallTabData = null;

        [SerializeField] private Text nameText;
        [SerializeField] private Color mColorUnselect;
        [SerializeField] private Color mColorSelect;
        [SerializeField] private Toggle toggle;
        [SerializeField] private Image mImgUnSelect;

        private void Awake()
        {
            _isSelected = false;
            _propertyMallTabData = null;
            BindUiEventSystem();
        }

        private void BindUiEventSystem()
        {
            if (toggle != null)
            {
                toggle.onValueChanged.RemoveAllListeners();
                toggle.onValueChanged.AddListener(OnTabClicked);
            }
        }

        private void OnDestroy()
        {
            UnBindUiEventSystem();
        }

        private void UnBindUiEventSystem()
        {
            if (toggle != null)
            {
                toggle.onValueChanged.RemoveAllListeners();
            }
        }

        public void InitData(MallNewPropertyMallTabData propertyMallTabData, 
            OnPropertyMallTabValueChanged toggleValueChanged = null,
            bool isSelected = false)
        {
            _propertyMallTabData = propertyMallTabData;
            if(_propertyMallTabData == null)
                return;

            var mallTypeTable = TableManager.GetInstance()
                .GetTableItem<MallTypeTable>(_propertyMallTabData.MallTypeTableId);
            if (mallTypeTable == null)
            {
                Logger.LogErrorFormat("MallTypeTable is null and mallTypeTableId is {0}",
                    _propertyMallTabData.MallTypeTableId);
            }

            if (nameText != null)
            {
                nameText.text = mallTypeTable.MainTypeName;
            }
            
            onToggleValueChanged = toggleValueChanged;

            if (isSelected == true)
            {
                if (toggle != null)
                {
                    toggle.isOn = true;
                }
            }
        }

        private void OnTabClicked(bool value)
        {
            if(_propertyMallTabData == null)
                return;
            mImgUnSelect.CustomActive(!value);
            nameText.color = value ? mColorSelect : mColorUnselect;
            //避免重复点击时，View重复更新
            if (_isSelected == value)
                return;
            _isSelected = value;

            if (value == true)
            {
                if (onToggleValueChanged != null)
                {
                    onToggleValueChanged(_propertyMallTabData.Index, _propertyMallTabData.MallTypeTableId);
                }
            }
        }

    }
}
