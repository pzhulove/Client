using ProtoTable;
using Scripts.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Protocol;

namespace GameClient
{
    public class RecruitListFrame : ClientFrame
    {
        #region ExtraUIBind
        private Button mBtClose = null;
        private ComUIListScript mUIList = null;

        protected sealed override void _bindExUI()
        {
            mBtClose = mBind.GetCom<Button>("btClose");
            mBtClose.onClick.AddListener(_onBtCloseButtonClick);
            mUIList = mBind.GetCom<ComUIListScript>("UIList");
        }

        protected sealed override void _unbindExUI()
        {
            mBtClose.onClick.RemoveListener(_onBtCloseButtonClick);
            mBtClose = null;
            mUIList = null;
        }
        #endregion

        #region Callback
        private void _onBtCloseButtonClick()
        {
            frameMgr.CloseFrame(this);
        }
        #endregion

        private List<RecruitPlayerInfo> recruitPlayerInfoList = new List<RecruitPlayerInfo>();

        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Activity/WarriorRecruit/RecruitListFrame";
        }

        protected sealed override void _OnOpenFrame()
        {
            InitUIList();

            recruitPlayerInfoList = userData as List<RecruitPlayerInfo>;

            mUIList.SetElementAmount(recruitPlayerInfoList.Count);
        }

        protected sealed override void _OnCloseFrame()
        {
            UnInitUIList();

            recruitPlayerInfoList.Clear();
        }

        private void InitUIList()
        {
            if (mUIList != null)
            {
                mUIList.Initialize();
                mUIList.onBindItem += OnBindItemDelegate;
                mUIList.onItemVisiable += OnItemVisiableDelegate;
                mUIList.OnItemRecycle += OnItemRecycleDelegate;
            }
        }

        private void UnInitUIList()
        {
            if (mUIList != null)
            {
                mUIList.onBindItem -= OnBindItemDelegate;
                mUIList.onItemVisiable -= OnItemVisiableDelegate;
                mUIList.OnItemRecycle -= OnItemRecycleDelegate;
            }
        }

        private ComCommonBind OnBindItemDelegate(GameObject itemObject)
        {
            return itemObject.GetComponent<ComCommonBind>();
        }

        private void OnItemVisiableDelegate(ComUIListElementScript item)
        {
            var comBind = item.gameObjectBindScript as ComCommonBind;

            if (comBind != null && item.m_index >= 0 && item.m_index < recruitPlayerInfoList.Count)
            {
                Text name = comBind.GetCom<Text>("name");
                Text level = comBind.GetCom<Text>("level");
                Text state = comBind.GetCom<Text>("state");
                Image icon = comBind.GetCom<Image>("icon");
                Button teamBtn = comBind.GetCom<Button>("btteam");

                RecruitPlayerInfo info = recruitPlayerInfoList[item.m_index];
              
                if (name != null)
                {
                    name.text = info.name;
                }

                if (level != null)
                {
                    level.text = string.Format("Lv.{0}", info.level);
                }

                if (icon != null)
                {
                    string path = "";

                    JobTable jobData = TableManager.GetInstance().GetTableItem<JobTable>(info.occu);
                    if (jobData == null)
                    {
                        return;
                    }

                    ResTable resData = TableManager.GetInstance().GetTableItem<ResTable>(jobData.Mode);
                    if (resData == null)
                    {
                        return;
                    }

                    path = resData.IconPath;

                    ETCImageLoader.LoadSprite(ref icon, path);
                }

                if (state != null)
                {
                    state.text = GetStatesStrByType(info.online);
                }

                if (teamBtn != null)
                {
                    teamBtn.CustomActive(info.online != (byte)FriendMatchStatus.Offlie);

                    teamBtn.onClick.RemoveAllListeners();

                    int iIndex = item.m_index;
                    teamBtn.onClick.AddListener(() => { _OnClickOk(iIndex); });
                }
            }
        }

        private void OnItemRecycleDelegate(ComUIListElementScript item)
        {
            ComCommonBind combind = item.gameObjectBindScript as ComCommonBind;
            if (combind == null)
            {
                return;
            }

            Button Btn = combind.GetCom<Button>("btteam");
            Btn.onClick.RemoveAllListeners();
        }

        private void _OnClickOk(int iIndex)
        {
            if (iIndex < 0 || iIndex >= recruitPlayerInfoList.Count)
            {
                return;
            }

            RecruitPlayerInfo info = recruitPlayerInfoList[iIndex];

            // 组队
            TeamDataManager.GetInstance().TeamInviteOtherPlayer(info.userId);
        }

        private string GetStatesStrByType(byte state)
        {
            string str = "";

            //if ((FriendMatchStatus)state == FriendMatchStatus.Busy)
            //{
            //    str = string.Format(TR.Value("Friens_Busy_State"), "忙碌");
            //}
            //else if ((FriendMatchStatus)state == FriendMatchStatus.Offlie)
            //{
            //    str = "下线";
            //}
            //else
            //{
            //    str = "在线";
            //}

            if (state == 0)
            {
                str = TR.Value("RecruitList_Offline_State");
            }
            else
            {
                str = TR.Value("RecruitList_Online_State");
            }

            return str;
        }
    }
}