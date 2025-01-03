using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class JournalItem : MonoBehaviour
    {
        [SerializeField]
        private GameObject mDateRoot;
        [SerializeField]
        private Text mDateTxt;
        [SerializeField]
        private GameObject mInfoRoot;
        [SerializeField]
        private LinkParse mInfoTxt;
        public void Init(JournalDataItem dataItem)
        {
            if (dataItem == null) return;
            if(dataItem.JournalType==EJournalType.Date)
            {
                mDateRoot.CustomActive(true);
                mInfoRoot.CustomActive(false);
                mDateTxt.SafeSetText(dataItem.Contnet);
            }
            else if(dataItem.JournalType==EJournalType.Info)
            {
                mDateRoot.CustomActive(false);
                mInfoRoot.CustomActive(true);
                if(mInfoTxt!=null)
                {
                    mInfoTxt.SetText(dataItem.Contnet);
                }
            }
        }

       
    }
}



