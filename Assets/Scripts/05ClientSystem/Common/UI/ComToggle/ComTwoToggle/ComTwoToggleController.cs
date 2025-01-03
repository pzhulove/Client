using System;
using ProtoTable;
using UnityEngine;
using UnityEngine.UI;
using Protocol;

namespace GameClient
{

    public delegate void OnTwoToggleClickAction(int index);

    public class ComTwoToggleController : MonoBehaviour
    {

        private int _firstToggleId = 0;
        private int _secondToggleId = 0;
        private string _firstToggleNameStr;
        private string _secondToggleNameStr;
        private OnTwoToggleClickAction _onTwoToggleClickAction;
        private int _defaultSelectToggleId = 0;

        [Space(15)] [HeaderAttribute("firstToggle")] [Space(15)]
        [SerializeField] private Toggle firstToggle;
        [SerializeField] private Text firstToggleNormalLabel;
        [SerializeField] private Text firstToggleSelectLabel;

        [Space(15)]
        [HeaderAttribute("secondToggle")]
        [Space(15)]
        [SerializeField] private Toggle secondToggle;
        [SerializeField] private Text secondToggleNormalLabel;
        [SerializeField] private Text secondToggleSelectLabel;

        #region Init
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
            if (firstToggle != null)
            {
                firstToggle.onValueChanged.RemoveAllListeners();
                firstToggle.onValueChanged.AddListener(OnFirstToggleClicked);
            }

            if (secondToggle != null)
            {
                secondToggle.onValueChanged.RemoveAllListeners();
                secondToggle.onValueChanged.AddListener(OnSecondToggleClicked);
            }

        }

        private void UnBindUiEvents()
        {
            if(firstToggle != null)
                firstToggle.onValueChanged.RemoveAllListeners();

            if(secondToggle != null)
                secondToggle.onValueChanged.RemoveAllListeners();
        }

        private void ClearData()
        {
            _firstToggleId = 0;
            _secondToggleId = 0;
            _onTwoToggleClickAction = null;
            _defaultSelectToggleId = 0;
        }
        #endregion

        public void InitTwoToggleController(int firstToggleId, string firstToggleNameStr,
            int secondToggleId, string secondToggleNameStr,
            OnTwoToggleClickAction onTwoToggleClickAction,
            int defaultSelectToggleId = 0)
        {
            _firstToggleId = firstToggleId;
            if (_firstToggleId <= 0)
                _firstToggleId = 1;

            _firstToggleNameStr = firstToggleNameStr;

            _secondToggleId = secondToggleId;
            if (_secondToggleId <= 0)
                _secondToggleId = 2;

            _secondToggleNameStr = secondToggleNameStr;

            _onTwoToggleClickAction = onTwoToggleClickAction;

            InitTwoToggleView();
        }

        private void InitTwoToggleView()
        {
            if (firstToggleNormalLabel != null)
                firstToggleNormalLabel.text = _firstToggleNameStr;
            if (firstToggleSelectLabel != null)
                firstToggleSelectLabel.text = _firstToggleNameStr;

            if (secondToggleNormalLabel != null)
                secondToggleNormalLabel.text = _secondToggleNameStr;
            if (secondToggleSelectLabel != null)
                secondToggleSelectLabel.text = _secondToggleNameStr;
            
            //默认选中第二个
            if (_defaultSelectToggleId == _secondToggleId)
            {
                if (secondToggle != null)
                {
                    secondToggle.isOn = false;
                    secondToggle.isOn = true;
                }
            }
            else
            {
                if (firstToggle != null)
                {
                    firstToggle.isOn = false;
                    firstToggle.isOn = true;
                }
            }

        }

        #region BindEvents

        private void OnFirstToggleClicked(bool value)
        {
            if (value == false)
                return;

            if (_onTwoToggleClickAction == null)
                return;

            _onTwoToggleClickAction.Invoke(_firstToggleId);
        }

        private void OnSecondToggleClicked(bool value)
        {
            if (value == false)
                return;

            if (_onTwoToggleClickAction == null)
                return;

            _onTwoToggleClickAction.Invoke(_secondToggleId);
        }
        #endregion

    }
}
