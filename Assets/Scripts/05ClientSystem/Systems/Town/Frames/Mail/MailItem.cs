using Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public delegate void OnMailItemClick(MailTitleInfo info);

    public class MailItem : MonoBehaviour
    {
        [SerializeField]private Text mMailTitle;
        [SerializeField]private Text mMailSender;
        [SerializeField]private Text mMailSendTime;
        [SerializeField]private Text mMailTimeLeft;
        [SerializeField]private GameObject mNewMailPrompt;
        [SerializeField]private GameObject mAttachIcon;
        [SerializeField]private GameObject mSelectBack;
        [SerializeField] private GameObject mOpenIcon;
        [SerializeField] private GameObject mCloseIcon;

        private MailTitleInfo mMailTitleInfo = null;
        private float fUpdateInterval = 0f;

        public MailTitleInfo GetMailTitleInfo
        {
            get { return mMailTitleInfo; }
            set { mMailTitleInfo = value; }
        }

        public void OnItemChangeDisplay(bool bSelected)
        {
            if (mSelectBack != null)
            {
                mSelectBack.CustomActive(bSelected);
            }
        }

        public void UpdateItemVisiable(MailTitleInfo mailTitleInfo)
        {
            mMailTitleInfo = mailTitleInfo;

            UpdateMailItemInfo(mMailTitleInfo);
        }

        private void UpdateMailItemInfo(MailTitleInfo mailTitleInfo)
        {
            if (mailTitleInfo == null)
            {
                return;
            }

            mMailTitle.text = mailTitleInfo.title;

            mMailSender.text = mailTitleInfo.sender;

            mMailSendTime.text = Function.GetBeginTimeStr(mailTitleInfo.date);

            CalSelMailLeftTime(mailTitleInfo);
            
            if (mailTitleInfo.status == 0)
            {
                mOpenIcon.CustomActive(false);
                mCloseIcon.CustomActive(true);
            }
            else
            {
                mOpenIcon.CustomActive(true);
                mCloseIcon.CustomActive(false);
            }
            
            mNewMailPrompt.gameObject.CustomActive(mailTitleInfo.status == 0);

            mAttachIcon.CustomActive(mailTitleInfo.hasItem == 1);
        }

        private void CalSelMailLeftTime(MailTitleInfo mailTitleInfo)
        {
            if (mailTitleInfo == null)
            {
                return;
            }

            if (mailTitleInfo.deadline - mailTitleInfo.date > 0)
            {
                mMailTimeLeft.text = Function.GetLeftTimeStr(mailTitleInfo.date, mailTitleInfo.deadline - mailTitleInfo.date);
            }
        }

        private void Update()
        {
            fUpdateInterval += Time.deltaTime;

            if (fUpdateInterval <= 60f)
            {
                return;
            }

            fUpdateInterval = 0f;

            CalSelMailLeftTime(mMailTitleInfo);
        }

        private void OnDestroy()
        {
            mMailTitleInfo = null;
            fUpdateInterval = 0f;
        }
    }
}

