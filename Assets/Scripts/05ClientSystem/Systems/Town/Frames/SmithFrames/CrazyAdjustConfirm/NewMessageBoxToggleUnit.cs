using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class NewMessageBoxToggleUnit : MonoBehaviour
    {
        [SerializeField] private Toggle mToggle;
        [SerializeField] private Text mToggleText;

        private ToggleEvent mToggleEvent;
        private Button mEventBtn;
        private bool tempToggleState;

        private void Awake()
        {
            ClearData();
        }

        private void OnDestroy()
        {
            ClearData();
        }

        public void InitBaseData(ToggleEvent tempEvent,Button tempBtn)
        {
            mToggleEvent = tempEvent;
            mEventBtn = tempBtn;
            tempToggleState = false;
        }

        public void UpdateItemInfo()
        {
            if (mToggleEvent != null) 
            {
                if (mToggleText != null) 
                {
                    mToggleText.text = mToggleEvent.toggleText;
                }
                if (mToggle != null) 
                {
                    mToggle.onValueChanged.AddListener(flag =>
                    {
                        if (flag == tempToggleState) return;
                        tempToggleState = flag;
                        if (flag)
                        {
                            mEventBtn.onClick.AddListener(mToggleEvent.toggleEvent);
                        }
                        else
                        {
                            mEventBtn.onClick.RemoveListener(mToggleEvent.toggleEvent);
                        }
                    });
                }
            }
        }

        public void OnItemRecycle()
        {
            ClearData();
        }

        private void ClearData()
        {
            if(mToggle != null)
            {
                mToggle.isOn = false;
            }
            if (mEventBtn != null && mToggleEvent!= null)
            {
                mEventBtn.onClick.RemoveListener(mToggleEvent.toggleEvent);
            }
            mToggleEvent = null;
        }
    }

}
