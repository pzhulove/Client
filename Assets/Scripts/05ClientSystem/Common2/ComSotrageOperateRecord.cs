using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Protocol;

namespace GameClient
{
    class SotrageOperateRecordData
    {
        public string value;
        public bool measured;
        public float w;
        public float h;
        public GuildStorageOpRecord record;
    }

    class ComSotrageOperateRecord : MonoBehaviour
    {
        public LinkParse linkParse;
        SotrageOperateRecordData recordsData;

        public void OnCreate()
        {

        }

        public void OnItemVisible(object data)
        {
            recordsData = data as SotrageOperateRecordData;
            if(null != linkParse)
            {
                if (null == recordsData)
                {
                    linkParse.SetText(string.Empty, false);
                }
                else
                {
                    linkParse.SetText(recordsData.value,true);
                }
            }
        }

        void OnDestroy()
        {
            recordsData = null;
            linkParse = null;
        }
    }
}