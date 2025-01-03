using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;
using ProtoTable;

namespace GameClient
{

    public enum CommonKeyBoardInputType
    {
        None = 0,
        ChangeNumber,
        Finished,
    }

    public enum KeyBoardNumberType
    {
        None = -1,
        Zero = 0,           //数字0
        One ,
        Two,
        Three,
        Four,
        Five,
        Six,
        Seven,
        Eight,
        Nine,
        Delete = 10,        //删除按钮
        Ensure = 11,        //确认按钮
    }

    public class CommonKeyBoardView : MonoBehaviour
    {

        //可以从配置中读取
        private readonly KeyBoardNumberType[] _keyBoardNumberTypeList =
        {
            KeyBoardNumberType.One,
            KeyBoardNumberType.Two,
            KeyBoardNumberType.Three,
            KeyBoardNumberType.Delete,
            KeyBoardNumberType.Four,
            KeyBoardNumberType.Five,
            KeyBoardNumberType.Six,
            KeyBoardNumberType.Zero,
            KeyBoardNumberType.Seven,
            KeyBoardNumberType.Eight,
            KeyBoardNumberType.Nine,
            KeyBoardNumberType.Ensure,
        };

        private CommonKeyBoardDataModel _keyBoardDataModel;

        private const int ColNumber = 4;            //每行的数量

        private Vector3 _baseVector3 = Vector3.zero;

        private ulong _currentValue = 0;
        private ulong _maxValue = 0;


        [Space(10)] [HeaderAttribute("widthAndHeight")] [Space(10)]
        [SerializeField] private float intervalWidth = 130;
        [SerializeField] private float intervalHeight = 130;

        [Space(10)] [HeaderAttribute("Content")] [Space(10)] [SerializeField]
        private GameObject firstKeyNumberGo;
        [SerializeField] private GameObject numberGo;
        [SerializeField] private GameObject deleteGo;
        [SerializeField] private GameObject ensureGo;

        [Space(10)] [HeaderAttribute("Bg")] [Space(10)] [SerializeField]
        private GameObject keyBoardContent;

        [Space(10)]
        [HeaderAttribute("Close")]
        [Space(10)]
        [SerializeField]
        private Button closeButton;

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
            if (closeButton != null)
            {
                closeButton.onClick.RemoveAllListeners();
                closeButton.onClick.AddListener(OnCloseFrame);
            }
        }

        private void UnBindEvents()
        {
            if (closeButton != null)
                closeButton.onClick.RemoveAllListeners();

        }

        private void ClearData()
        {
            _keyBoardDataModel = null;
            _currentValue = 0;
            _maxValue = 0;
        }

        #region InitKeyBoard
        public void InitView(CommonKeyBoardDataModel dataModel)
        {
            _keyBoardDataModel = dataModel;

            if (_keyBoardDataModel == null)
            {
                Logger.LogErrorFormat("KeyBoardDataModel is null");
                return;
            }
            
            InitViewData();

            InitKeyBoardPosition();
            InitKeyBoardNumber();
        }

        private void InitViewData()
        {
            if (firstKeyNumberGo != null)
                _baseVector3 = firstKeyNumberGo.transform.localPosition;

            _currentValue = _keyBoardDataModel.CurrentValue;
            _maxValue = _keyBoardDataModel.MaxValue;
        }

        private void InitKeyBoardPosition()
        {
            if (keyBoardContent != null)
                keyBoardContent.transform.localPosition = _keyBoardDataModel.Position;
        }

        private void InitKeyBoardNumber()
        {
            var keyBoardNumber = _keyBoardNumberTypeList.Length;

            for (var i = 0; i < keyBoardNumber; i++)
            {
                InitOneKeyBoardNumber(i, _keyBoardNumberTypeList[i]);
            }
        }

        private void InitOneKeyBoardNumber(int index, KeyBoardNumberType keyBoardNumType)
        {
            var row = index / ColNumber;
            var col = index % ColNumber;

            GameObject numberPrefab = null;
            if (keyBoardNumType == KeyBoardNumberType.Ensure)
            {
                if (ensureGo != null)
                {
                    numberPrefab = Instantiate(ensureGo) as GameObject;
                }
            }
            else if (keyBoardNumType == KeyBoardNumberType.Delete)
            {
                if(deleteGo != null)
                    numberPrefab = Instantiate(deleteGo) as GameObject;
            }
            else
            {
                if(numberGo != null)
                    numberPrefab = Instantiate(numberGo) as GameObject;
            }

            if (numberPrefab == null)
                return;

            numberPrefab.CustomActive(true);
            Utility.AttachTo(numberPrefab, keyBoardContent);
            numberPrefab.transform.localPosition = new Vector3(
                _baseVector3.x + col * intervalWidth,
                _baseVector3.y - row * intervalHeight,
                _baseVector3.z);

            var numberItem = numberPrefab.GetComponent<CommonKeyBoardNumberItem>();
            if (numberItem != null)
            {
                numberItem.InitItem(keyBoardNumType, OnNumberItemClicked);
            }
        }

        #endregion

        #region NumberItemClicked

        private void OnNumberItemClicked(KeyBoardNumberType keyBoardNumberType)
        {
            if (keyBoardNumberType == KeyBoardNumberType.Delete)
            {
                OnDeleteItem();
            }
            else if (keyBoardNumberType == KeyBoardNumberType.Ensure)
            {
                OnEnsureItem();
            }
            else
            {
                OnAddItem(keyBoardNumberType);
            }
        }

        private void OnDeleteItem()
        {
            if (_currentValue > 9)
            {
                _currentValue = _currentValue / 10;
            }
            else
            {
                _currentValue = 0;
            }

            OnSendChangeNumberEvent();
        }

        private void OnAddItem(KeyBoardNumberType keyBoardNumberType)
        {
            _currentValue = _currentValue * 10 + (ulong) keyBoardNumberType;
            if (_currentValue >= _maxValue)
                _currentValue = _maxValue;

            OnSendChangeNumberEvent();
        }

        private void OnSendChangeNumberEvent()
        {
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnCommonKeyBoardInput,
                CommonKeyBoardInputType.ChangeNumber, _currentValue);
        }

        //关闭界面
        private void OnEnsureItem()
        {
            OnCloseFrame();
        }

        #endregion


        private void OnCloseFrame()
        {
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnCommonKeyBoardInput,
                CommonKeyBoardInputType.Finished, _currentValue);

            CommonUtility.OnCloseCommonKeyBoardFrame();
        }
    }
}
