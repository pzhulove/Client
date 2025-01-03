using UnityEngine.UI;
using Protocol;
using ProtoTable;
using UnityEngine;
using System.Collections.Generic;

namespace GameClient
{
    class TownStatueTalkFrame : ClientFrame
    {
        byte StatueType = 0;

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/MainFrameTown/TownStatueTalkFrame";
        }

        protected override void _OnOpenFrame()
        {
            if(userData != null)
            {
                StatueType = (byte)userData;
            }        

            List<FigureStatueInfo> StatueInfoList = GuildDataManager.GetInstance().GetTownStatueInfo();

            bool bFind = false;
            for(int i = 0; i < StatueInfoList.Count; i++)
            {
                FigureStatueInfo StatueInfo = StatueInfoList[i];

                if (StatueInfo.statueType != StatueType)
                {
                    continue;
                }

                if (StatueInfo.statueType <= 0)
                {
                    //mNpcIcon.gameObject.CustomActive(false);
                    mTalkContent.text = TR.Value("Town_Statue_No_Talk");
                    mNpcName.text = "";
                }
                else
                {
                    JobTable JobData = TableManager.GetInstance().GetTableItem<JobTable>(StatueInfo.occu);
                    if (JobData != null)
                    {
                        if (!string.IsNullOrEmpty(JobData.JobHalfBody) && JobData.JobHalfBody != "-")
                        {
                            // mNpcIcon.sprite = AssetLoader.instance.LoadRes(JobData.JobHalfBody, typeof(Sprite)).obj as Sprite;
                            ETCImageLoader.LoadSprite(ref mNpcIcon, JobData.JobHalfBody);
                        }
                    }

                    mNpcIcon.gameObject.CustomActive(true);
                    mTalkContent.text = TR.Value("Town_Statue_Talk", StatueInfo.guildName, StatueInfo.name);
                    mNpcName.text = StatueInfo.name;
                }

                bFind = true;
                break;
            }

            if(!bFind)
            {
                //mNpcIcon.gameObject.CustomActive(false);
                mTalkContent.text = TR.Value("Town_Statue_No_Talk");
                mNpcName.text = "";
            }
        }

        protected override void _OnCloseFrame()
        {
            ClearData();
        }

        void ClearData()
        {
            StatueType = 0;
        }

        #region ExtraUIBind
        private Button mBtOk = null;
        private Text mTalkContent = null;
        private Text mNpcName = null;
        private Image mNpcIcon = null;

        protected override void _bindExUI()
        {
            mBtOk = mBind.GetCom<Button>("BtOk");
            mBtOk.onClick.AddListener(_onBtOkButtonClick);
            mTalkContent = mBind.GetCom<Text>("TalkContent");
            mNpcName = mBind.GetCom<Text>("NpcName");
            mNpcIcon = mBind.GetCom<Image>("NpcIcon");
        }

        protected override void _unbindExUI()
        {
            mBtOk.onClick.RemoveListener(_onBtOkButtonClick);
            mBtOk = null;
            mTalkContent = null;
            mNpcName = null;
            mNpcIcon = null;
        }
        #endregion

        #region Callback
        private void _onBtOkButtonClick()
        {
            frameMgr.CloseFrame(this);
        }
        #endregion
    }
}
