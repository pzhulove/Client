using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class ComAchievementGroupSubTabItem : MonoBehaviour
    {
        public Text label;
        public Text checkLabel;
        public StateController comState;
        ProtoTable.AchievementGroupSecondMenuTable menuItem;

        public void SetValue(string value)
        {
            if(null != label)
            {
                label.text = value;
            }
            if (null != checkLabel)
            {
                checkLabel.text = value;
            }
        }

        public void OnValueChanged(bool bValue)
        {
            if(null != comState)
            {
                comState.Key = bValue ? "Enable" : "Disable";
            }
            if(bValue)
            {
                if(null != menuItem)
                {
                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnAchievementSecondMenuTabChanged, menuItem);
                }
            }
        }

        public void OnItemVisible(int itemId)
        {
            menuItem = TableManager.GetInstance().GetTableItem<ProtoTable.AchievementGroupSecondMenuTable>(itemId);
            if(null != menuItem)
            {
                SetValue(menuItem.Name);
            }
        }

        void Awake()
        {

        }

        void OnDestroy()
        {

        }
    }
}