using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public delegate void MallNewIntergralMallTabClick(int mallTypeTable);
    public class MallNewIntergralMallTabItem : MonoBehaviour
    {
        [SerializeField] private Text nameText;
        [SerializeField] private Color mColorUnselect;
        [SerializeField] private Color mColorSelect;
        [SerializeField]private Toggle mTog;
        [SerializeField] private Image mImgUnSelect;

        private bool mIsSeleted = false;
        private MallNewIntergralMallTabClick mMallNewIntergralMallTabClick = null;
        private MallNewIntergralMallTabData mTabData = null;

        private void Awake()
        {
            if (mTog != null)
            {
                mTog.onValueChanged.RemoveAllListeners();
                mTog.onValueChanged.AddListener(OnTabClick);
            }
        }

        private void OnDestroy()
        {
            mIsSeleted = false;
            mMallNewIntergralMallTabClick = null;
            mTabData = null;
        }

        public void InitData(MallNewIntergralMallTabData tabData, MallNewIntergralMallTabClick onClick,bool isSeleted)
        {
            mTabData = tabData;
            
            if (mTabData == null) return;

            var mallTypeTable = TableManager.GetInstance().GetTableItem<ProtoTable.MallTypeTable>(tabData.mallTypeTableId);
            if (mallTypeTable == null)
            {
                Logger.LogErrorFormat("MallTypeTable is null and mallTypeTableId is {0}",
                    tabData.mallTypeTableId);
            }

            if (nameText != null)
                nameText.text = mallTypeTable.MainTypeName;

            mMallNewIntergralMallTabClick = onClick;

            if (isSeleted == true)
            {
                if (mTog != null)
                {
                    mTog.isOn = true;
                }
            }
        }

        private void OnTabClick(bool value)
        {
            if (mTabData == null) return;
            mImgUnSelect.CustomActive(!value);
            nameText.color = value ? mColorSelect : mColorUnselect;
            if (mIsSeleted == value)
            {
                return;
            }

            mIsSeleted = value;

            if (value == true)
            {
                if (mMallNewIntergralMallTabClick != null)
                {
                    mMallNewIntergralMallTabClick.Invoke(mTabData.mallTypeTableId);
                }
            }
        }
    }
}