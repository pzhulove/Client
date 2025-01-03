using UnityEngine;
using UnityEngine.UI;
using ProtoTable;
using System;
using System.Collections.Generic;

namespace GameClient
{

    public delegate void OnPageChangeAction(int selectPageIndex);

    public class ComPageChangeController : MonoBehaviour
    {

        private int _selectPageIndex;
        private int _maxPageNumber;
        private OnPageChangeAction _onPageChangeAction;

        [Space(10)]
        [HeaderAttribute("PageChangeButton")]
        [Space(10)]
        [SerializeField] private Button prePageButton;
        [SerializeField] private UIGray prePageButtonGray;
        [SerializeField] private Button nextPageButton;
        [SerializeField] private UIGray nextPageButtonGray;

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
            if (prePageButton != null)
            {
                prePageButton.onClick.RemoveAllListeners();
                prePageButton.onClick.AddListener(OnPrePageButtonClick);
            }

            if (nextPageButton != null)
            {
                nextPageButton.onClick.RemoveAllListeners();
                nextPageButton.onClick.AddListener(OnNextPageButtonClick);
            }
        }

        private void UnBindUiEvents()
        {
            if(prePageButton != null)
                prePageButton.onClick.RemoveAllListeners();

            if(nextPageButton != null)
                nextPageButton.onClick.RemoveAllListeners();
        }

        private void ClearData()
        {
            _selectPageIndex = 0;
            _maxPageNumber = 0;
            _onPageChangeAction = null;
        }

        public void InitPageChangeController(int selectPageIndex,
            int maxPageNumber,
            OnPageChangeAction onPageChangeAction)
        {
            _selectPageIndex = selectPageIndex;
            _maxPageNumber = maxPageNumber;
            _onPageChangeAction = onPageChangeAction;

            UpdateButtonState();
        }

        public void UpdatePageChangeController(int selectPageIndex)
        {
            _selectPageIndex = selectPageIndex;
            UpdateButtonState();
        }
        
        private void OnPrePageButtonClick()
        {
            if (_selectPageIndex <= 1)
                return;

            _selectPageIndex -= 1;
            UpdateButtonState();
            if (_onPageChangeAction != null)
                _onPageChangeAction(_selectPageIndex);
        }

        private void OnNextPageButtonClick()
        {
            if (_selectPageIndex >= _maxPageNumber)
                return;

            _selectPageIndex += 1;
            UpdateButtonState();
            if (_onPageChangeAction != null)
                _onPageChangeAction(_selectPageIndex);
        }

        private void UpdateButtonState()
        {
            if (_selectPageIndex <= 1)
            {
                CommonUtility.UpdateButtonState(prePageButton, prePageButtonGray, false);
            }
            else
            {
                CommonUtility.UpdateButtonState(prePageButton, prePageButtonGray, true);
            }

            if (_selectPageIndex >= _maxPageNumber)
            {
                CommonUtility.UpdateButtonState(nextPageButton, nextPageButtonGray, false);
            }
            else
            {
                CommonUtility.UpdateButtonState(nextPageButton, nextPageButtonGray, true);
            }
        }

    }
}
