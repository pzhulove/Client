using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scripts.UI;
using UnityEngine.UI;

namespace GameClient
{
    public class DeadLineReminderView : MonoBehaviour
    {
        [SerializeField]private ComUIListScript mElementUIList;
        [SerializeField]private Button mIngoreLessItemBtn;

        private void Awake()
        {
            BindUIEventSystem();
            InitElementUIList();

            if (mIngoreLessItemBtn != null)
            {
                mIngoreLessItemBtn.onClick.RemoveAllListeners();
                mIngoreLessItemBtn.onClick.AddListener(OnIngoreLessItemBtnClick);
            }
        }

        private void OnDestroy()
        {
            UnBindUIEventSystem();
            UnInitElementUIList();
        }

        public void InitView()
        {
            DeadLineReminderDataManager.GetInstance().Sort();
            UpdateElementAmount();
        }

        private void BindUIEventSystem()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.DeadLineReminderChanged, DeadLineReminderChanged);
        }

        private void UnBindUIEventSystem()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.DeadLineReminderChanged, DeadLineReminderChanged);
        }

        private void DeadLineReminderChanged(UIEvent uiEvent)
        {
            UpdateElementAmount();
        }

        private void InitElementUIList()
        {
            if (mElementUIList != null)
            {
                mElementUIList.Initialize();
                mElementUIList.onBindItem += OnBindItemDelegate;
                mElementUIList.onItemVisiable += OnItemVisiableDelegate;
            }
        }

        private void UnInitElementUIList()
        {
            if (mElementUIList != null)
            {
                mElementUIList.onBindItem -= OnBindItemDelegate;
                mElementUIList.onItemVisiable -= OnItemVisiableDelegate;
            }
        }

        private DeadLineReminderElement OnBindItemDelegate(GameObject itemObject)
        {
            return itemObject.GetComponent<DeadLineReminderElement>();
        }

        private void OnItemVisiableDelegate(ComUIListElementScript item)
        {
            var element = item.gameObjectBindScript as DeadLineReminderElement;
            if (element != null && item.m_index >= 0 && item.m_index < DeadLineReminderDataManager.GetInstance().GetDeadLineReminderModelList().Count)
            {
                var model = DeadLineReminderDataManager.GetInstance().GetDeadLineReminderModelList()[item.m_index];
                element.InitElement(model);
            }
        }

        private void UpdateElementAmount()
        {
            if (mElementUIList)
                mElementUIList.SetElementAmount(DeadLineReminderDataManager.GetInstance().GetDeadLineReminderModelList().Count);
        }

        private void OnIngoreLessItemBtnClick()
        {
            DeadLineReminderDataManager.GetInstance().DeleteFailureItem();
        }
    }
}