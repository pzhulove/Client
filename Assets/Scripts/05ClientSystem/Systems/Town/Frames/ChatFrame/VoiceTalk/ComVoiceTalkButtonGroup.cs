using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class ComVoiceTalkButtonGroup : MonoBehaviour
    {
        [Header("请按顺序添加ComTalkButton")]
        [SerializeField]
        private List<ComVoiceTalkButton> talkBtns;

        //界面布局
        [SerializeField]
        private GameObject headRoot;
        [SerializeField]
        private GameObject expandRoot;
        [SerializeField]
        private GameObject hideRoot;        

        private ComVoiceTalkButton headTalkBtn;
        private List<ComVoiceTalkButton> expandTalkBtns;
        private List<ComVoiceTalkButton> hideTalkBtns;
        private void Awake()
        {
            if(hideTalkBtns == null)
            {
                hideTalkBtns = new List<ComVoiceTalkButton>();
            }
            if(talkBtns != null)
            {
                if(expandTalkBtns == null)
                {
                    expandTalkBtns = new List<ComVoiceTalkButton>();
                }
                for (int i = 0; i < talkBtns.Count; i++)
                {
                    var tBtn = talkBtns[i];
                    tBtn.sortIndex = i;             //设置序号
                    if(tBtn == null)
                        continue;
                    tBtn.group = this;              //设置组
                    if(i == 0)
                    {
                        headTalkBtn = tBtn;
                    }
                    else //除第一个外存入展开按钮列表
                    {
                        expandTalkBtns.Add(tBtn);                        
                    }
                }             
            }
            //初始化
            _RecoverGroup();
        }        

        private void OnDestroy() 
        {
            headTalkBtn = null;
            if(expandTalkBtns != null)
            {
                expandTalkBtns.Clear();
                expandTalkBtns = null;                
            }   
            if(hideTalkBtns != null)
            {
                hideTalkBtns.Clear();
                hideTalkBtns= null;                
            }          
        }

        public List<ComVoiceTalkButton> GetAllTalkBtns()
        {
            return talkBtns;
        }

        /// <summary>
        /// 初始化实时语音按钮 组
        /// </summary>
        /// <param name="cTalk"></param>
        /// <param name="talkSceneIds"> 游戏场景id需要大于1 符合设计 </param>
        public void UpdateAllTalkBtns(List<string> talkSceneIds)
        {
            if (null == talkBtns || null == talkSceneIds)
            {
                return;
            }
            for (int i = 0; i != Mathf.Min(talkBtns.Count, talkSceneIds.Count); i++)
            {
                var btn = talkBtns[i];
                if (btn == null)
                {
                    continue;
                }
                if (btn.bType == ComVoiceTalkButton.TalkBtnType.MicChannelOn)
                {
                    btn.param1 = talkSceneIds[i];
                }
            }
        }

        private void _ExpandGroup()
        {
            if(null == expandRoot)
            {
                return;
            }
            _ResetSortIndex(hideTalkBtns);
            if (hideTalkBtns != null)
            {
                for (int i = 0; i < hideTalkBtns.Count; i++)
                {
                    var btn = hideTalkBtns[i];
                    if (btn == null)
                    {
                        continue;
                    }
                    Utility.AttachTo(btn.gameObject, expandRoot);
                    if(expandTalkBtns != null && !expandTalkBtns.Contains(btn))
                    {
                        expandTalkBtns.Add(btn);
                    }
                }
                hideTalkBtns.Clear();
            }
        }

        private void _RecoverGroup()
        {
            if (null == hideRoot)
            {
                return;
            }
            _ResetSortIndex(expandTalkBtns);
            if (expandTalkBtns != null)
            {
                for (int i = 0; i < expandTalkBtns.Count; i++)
                {
                    var btn = expandTalkBtns[i];
                    if (btn == null)
                        continue;
                    Utility.AttachTo(btn.gameObject, hideRoot);
                    if(hideTalkBtns != null && !hideTalkBtns.Contains(btn))
                    {
                        hideTalkBtns.Add(btn);
                    }
                }
                expandTalkBtns.Clear();
            }
        }

        public void SetMicChannelOnTalkBtnSelected(string talkChannelId, bool bSelected)
        {
            if(null == talkBtns || talkBtns.Count <= 0)
            {
                return;
            }
            for (int i = 0; i < talkBtns.Count; i++)
            {
                var tBtn = talkBtns[i];
                if(tBtn == null || tBtn.bType != ComVoiceTalkButton.TalkBtnType.MicChannelOn)
                {
                    continue;
                }
                string tBtnChannelId = tBtn.param1 as string;
                if(string.IsNullOrEmpty(tBtnChannelId) || tBtnChannelId != talkChannelId)
                    continue; 
                SetComVoiceTalkBtnSelected(tBtn, bSelected);
            }
        }

        public void SetMicOffTalkBtnSelected(bool bSelected)
        {
            if(null == talkBtns || talkBtns.Count <= 0)
            {
                return;
            }
            for (int i = 0; i < talkBtns.Count; i++)
            {
                var tBtn = talkBtns[i];
                if(tBtn == null || tBtn.bType != ComVoiceTalkButton.TalkBtnType.MicAllOff)
                {
                    continue;
                }
                SetComVoiceTalkBtnSelected(tBtn, bSelected);
            }
        }

        public void SetComVoiceTalkBtnSelected(ComVoiceTalkButton comBtn, bool bSelected)
        {
            if(!bSelected)
            {
                _ExpandGroup();
                return;
            }
            if(null == comBtn)
            {
                return;
            }        
            _TrySetTalkBtnToHead(comBtn);
            _RecoverGroup();
        }

        private void _TrySetTalkBtnToHead(ComVoiceTalkButton talkBtn)
        {
            if(null == talkBtn)
                return;
            if(talkBtn == headTalkBtn)
            {
                return;
            }        
            _ResetAllTalkBtnsToExpandRoot();
            //_ResetSortIndex(expandTalkBtns);
            Utility.AttachTo(talkBtn.gameObject, headRoot);
            if(expandTalkBtns != null)
            {
                expandTalkBtns.Remove(talkBtn);
            }
            if(hideTalkBtns != null)
            {
                hideTalkBtns.Remove(talkBtn);
            }
            headTalkBtn = talkBtn;
        }


        //重置所有button 到 扩展结点 下
        private void _ResetAllTalkBtnsToExpandRoot()
        {
            if(expandTalkBtns == null)
                return;
            if(!expandTalkBtns.Contains(headTalkBtn))
            {
                expandTalkBtns.Insert(0, headTalkBtn);
            }
            if(hideTalkBtns != null && hideTalkBtns.Count > 0)
            {
                for (int i = 0; i < hideTalkBtns.Count; i++)
                {
                    var btn = hideTalkBtns[i];
                    if(btn == null)
                        continue;
                    if(!expandTalkBtns.Contains(btn))
                    {
                        expandTalkBtns.Add(btn);
                    }
                }
            }
        }

        private void _ResetSortIndex(List<ComVoiceTalkButton> unSortedBtnList)
        {
            if(null == unSortedBtnList || unSortedBtnList.Count <= 0)
            {
                return;
            }
            unSortedBtnList.Sort((x, y) => x.sortIndex.CompareTo(y.sortIndex));
            /*
            for (int i = 0; i < unSortedBtnList.Count; i++)
            {
                var unSortedBtn = unSortedBtnList[i];
                if(null == unSortedBtn)
                    continue;
                if (unSortedBtn.transform.GetSiblingIndex() != i)
                {
                    unSortedBtn.transform.SetSiblingIndex(i);
                }
            }*/
        }
    }
}
