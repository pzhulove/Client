using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public delegate void OnInvitedTabClick(InvitedTabData tabData);

    public class TeamInvitedTabItem : MonoBehaviour
    {
        [SerializeField]private Text mName;
        [SerializeField]private Toggle mToggle;

        private OnInvitedTabClick mOnInvitedTabClick;
        private bool bIsSelected = false;
        private InvitedTabData mInvitedTabData;

        private void Awake()
        {
            if (mToggle != null)
            {
                mToggle.onValueChanged.RemoveAllListeners();
                mToggle.onValueChanged.AddListener(OnTabClick);
            }
        }

        private void OnDestroy()
        {
            bIsSelected = false;
            mInvitedTabData = null;

            if (mToggle != null)
            {
                mToggle.onValueChanged.RemoveAllListeners();
            }
        }

        public void InitTab(InvitedTabData invitedTabData, OnInvitedTabClick onInvitedTabClick, bool isSelected)
        {
            mInvitedTabData = invitedTabData;
            mOnInvitedTabClick = onInvitedTabClick;

            if (mInvitedTabData == null)
            {
                return;
            }

            if (mName != null)
            {
                mName.text = mInvitedTabData.mTabName;
            }

            if (isSelected == true)
            {
                if (mToggle != null)
                {
                    mToggle.isOn = true;
                }
            }
        }

        private void OnTabClick(bool value)
        {
            if (mInvitedTabData == null)
            {
                return;
            }

            if (value == bIsSelected)
            {
                return;
            }

            bIsSelected = value;

            if (value == true)
            {
                if (mOnInvitedTabClick != null)
                {
                    mOnInvitedTabClick(mInvitedTabData);
                }
            }
        }
    }
}