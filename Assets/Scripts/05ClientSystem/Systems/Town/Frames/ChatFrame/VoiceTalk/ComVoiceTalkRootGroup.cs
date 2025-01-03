using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class ComVoiceTalkRootGroup : MonoBehaviour
    {
        [System.Serializable]
        public class ComVoiceTalkRootParam
        {
            public ComVoiceTalk.ComVoiceTalkType talkType;
            public GameObject talkRoot;
        }      

        [SerializeField]
        private List<ComVoiceTalkRootParam> mTalkRootList; 

        public GameObject GetTalkRootByTalkType(ComVoiceTalk.ComVoiceTalkType talkType)
        {
            if(null == mTalkRootList)
            {
                return null;
            }
            for (int i = 0; i < mTalkRootList.Count; i++)
            {
                var tr = mTalkRootList[i];
                if(tr == null)
                    continue;
                if(tr.talkType == talkType)
                {
                    return tr.talkRoot;
                }
            }
            return null;
        }
    }
}
