using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Protocol;
using ProtoTable;

namespace GameClient
{

    public class AdventureTeamExpeditionTimeToggle : MonoBehaviour
    {
        [SerializeField] private Text mTimeText;
        [SerializeField] private Toggle mTimeChangeToggle;
        [SerializeField] private UIGray mUIGray;
        //[SerializeField] private GameObject mSelectObj;
        [SerializeField] private byte tempTime;

        private bool isEnable = true;
        private bool isSelect;

        private bool useOnekey = false;

        private void Awake()
        {
            ClearData();
        }

        private void OnDestroy()
        {
            ClearData();
        }

        public void InitItemView(byte time, bool flag, bool useOnekey = false)
        {
            tempTime = time;
            isEnable = flag;
            mTimeChangeToggle.onValueChanged.AddListener(_OnChangeTimeToggleClick);
            isSelect = false;
            this.useOnekey = useOnekey;
        }

        public void UpdateItemInfo()
        {
            mTimeText.text = TR.Value("adventure_team_expedition_dispatch_hour",tempTime.ToString());
            if (mTimeChangeToggle.isOn)
            {
                //mTimeText.color = Color.yellow;
                mUIGray.enabled = false;
            }
            else if (isEnable)
            {
                //mTimeText.color = Color.white;
                mUIGray.enabled = false;
            }
            else
            {
                //mTimeText.color = Color.white;
                mUIGray.enabled = true;
            }
            mTimeChangeToggle.enabled = isEnable;
            if (mTimeChangeToggle)
            {
                bool tempFlag = mTimeChangeToggle.enabled;
                mTimeChangeToggle.enabled = true;
                mTimeChangeToggle.enabled = tempFlag;
            }
        }

        public void ChangeToggleState(bool isOn)
        {
            mTimeChangeToggle.isOn = isOn;
        }

        private void _OnChangeTimeToggleClick(bool isOn)
        {
            if (isOn == isSelect)
                return;
            isSelect = isOn; //避免多次点击做判断

            if (isOn)
            {
                //mTimeText.color = Color.yellow;
                AdventureTeamDataManager.GetInstance().SetEpxeditionTime(tempTime, useOnekey);
            }
            else
            {
                //mTimeText.color = Color.white;
            }
        }

        public void OnItemRecycle()
        {
            ClearData();
            mTimeChangeToggle.onValueChanged.RemoveListener(_OnChangeTimeToggleClick);
            mTimeChangeToggle.isOn = false;
        }

        private void ClearData()
        {
            tempTime = 0;
        }
    }
}
