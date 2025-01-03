using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{

//head 2			12	Head 头巾      0

//UpperBody 4		14	Clothes 上装      1

//chest 6			16	Plastron 胸饰      2

//LowerBody 5		15	Trousers 下装      3

//Waist 3			13	Waist 腰饰      4

    //时装商城位置
    public enum FashionMallPositionTabType
    {
        None = -1,
        Head = 0,       //头巾              对应ItemTable SubType 12    头饰
        Clothes = 1,    //衣服                                    14    上装
        Plastron = 2,   //胸饰                                    16    胸饰
        Trousers = 3,   //裤子                                    15    下装
        Waist = 4,      //腰饰                                    13    腰饰
        Weapon = 5,     //武器
        Wing = 6,       //翅膀
    }

    [Serializable]
    public class FashionMallPositionTabData
    {
        public int Index;
        public FashionMallPositionTabType PositionTabType;
    }

    public delegate void OnFashionMallPositionTabValueChanged(int index, FashionMallPositionTabType positionTabType);

    public class FashionMallPositionTabItem : MonoBehaviour
    {
        // private const string ImagePath = "UI/Image/Packed/p_UI_Shop.png:UI_Shop_Tubiao{0}";
        private OnFashionMallPositionTabValueChanged _onToggleValueChanged = null;

        private bool _isSelected = false;
        private FashionMallPositionTabData _positionTabData = null;

        [SerializeField]
        private Toggle toggle;
        [SerializeField] private Text mTextName;
        [SerializeField] private Color mColorSelect;
        [SerializeField] private Color mColorUnselect;
        

        // [SerializeField] private Image normalImage;
        // [SerializeField] private Image selectedImage;

        private void Awake()
        {
            _isSelected = false;
            _positionTabData = null;
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

        public void InitData(FashionMallPositionTabData positionTabData,
            OnFashionMallPositionTabValueChanged toggleValueChanged = null,
            bool isSelected = false)
        {
            _isSelected = false;
            _positionTabData = positionTabData;
            if (_positionTabData == null)
                return;

            InitTabItemImage();

            _onToggleValueChanged = toggleValueChanged;

            if (isSelected == true)
            {
                if (toggle != null)
                {
                    toggle.isOn = true;
                }
            }
        }

        private void InitTabItemImage()
        {
            // string normalImagePath = "";
            // string selectedImagePath = "";
            var textNameStr = TR.Value("Fashion_Mall_Single_tab_Head");
            switch (_positionTabData.PositionTabType)
            {
                case FashionMallPositionTabType.Head:
                    textNameStr = TR.Value("Fashion_Mall_Single_tab_Head");
                    break;
                case FashionMallPositionTabType.Clothes:
                    textNameStr = TR.Value("Fashion_Mall_Single_tab_Clothes");
                    break;
                case FashionMallPositionTabType.Plastron:
                    textNameStr = TR.Value("Fashion_Mall_Single_tab_Plastron");
                    break;
                case FashionMallPositionTabType.Trousers:
                    textNameStr = TR.Value("Fashion_Mall_Single_tab_Trousers");
                    break;
                case FashionMallPositionTabType.Waist:
                    textNameStr = TR.Value("Fashion_Mall_Single_tab_Waist");
                    break;
            }

            if (mTextName != null)
                mTextName.text = textNameStr;

            // ETCImageLoader.LoadSprite(ref normalImage, normalImagePath);
            // ETCImageLoader.LoadSprite(ref selectedImage, selectedImagePath);
        }

        private void OnTabClicked(bool value)
        {
            if (_positionTabData == null)
                return;

            ////避免重复点击时，View重复更新
            //if (_isSelected == value)
            //    return;
            _isSelected = value;
            mTextName.color = value ? mColorSelect : mColorUnselect;
            if (value == true)
            {
                if (_onToggleValueChanged != null)
                {
                    _onToggleValueChanged(_positionTabData.Index, _positionTabData.PositionTabType);
                }
            }
        }
    }
}
