using UnityEngine;
using System.Collections.Generic;
using Protocol;
using Network;
using UnityEngine.UI;
using Scripts.UI;
using System;
using ProtoTable;

namespace GameClient
{
    class FriendRecommentData
    {
       public List<RelationData> relationlist = new List<RelationData>();
    }
    class FriendRecommendedFrame : ClientFrame
    {
        FriendRecommentData data =null;

        bool m_bWaitSearchRet = false;
        bool m_bIsQuery = false;
        [SerializeField] private int ShowCnt = 6;

		#region ExtraUIBind
		private Button mClose = null;
		private Button mBtnChangeAll = null;
		private ComUIListScript comFriendRecommendList = null;
		private Button mBtnSearch = null;
		private UIGray mGrayChangeAll = null;
		private Text mLabelChangeAll = null;
		private InputField mInputField = null;
        private Button mBtnAddAll = null;
        private UIGray mGrayAddAll = null;
        private RectTransform mFriendParent = null;
        private ComRecommendFriendInfo mSearchFriendInfo = null;
        private Button mSearchFriendRoot = null;
        private ReplaceHeadPortraitFrame mMyHead = null;
        private Image mHeadIcon = null;

        protected override void _bindExUI()
		{
			mClose = mBind.GetCom<Button>("Close");
			if (null != mClose)
			{
				mClose.onClick.AddListener(_onCloseButtonClick);
			}
			mBtnChangeAll = mBind.GetCom<Button>("BtnChangeAll");
			if (null != mBtnChangeAll)
			{
				mBtnChangeAll.onClick.AddListener(_onBtnChangeAllButtonClick);
			}
            comFriendRecommendList = mBind.GetCom<ComUIListScript>("Friends");
			mBtnSearch = mBind.GetCom<Button>("BtnSearch");
			if (null != mBtnSearch)
			{
				mBtnSearch.onClick.AddListener(_onBtnSearchButtonClick);
			}
			mGrayChangeAll = mBind.GetCom<UIGray>("GrayChangeAll");
			mLabelChangeAll = mBind.GetCom<Text>("LabelChangeAll");
			mInputField = mBind.GetCom<InputField>("InputField");
            mBtnAddAll = mBind.GetCom<Button>("BtnAddAll");
            if (mBtnAddAll != null)
            {
                mBtnAddAll.onClick.AddListener(_OnBtnAddAllButtonClick);
            }
            mGrayAddAll = mBind.GetCom<UIGray>("GrayAddAll");
            mFriendParent = mBind.GetCom<RectTransform>("FriendParent");
            mSearchFriendInfo = mBind.GetCom<ComRecommendFriendInfo>("SearchFriendInfo");
            mSearchFriendRoot = mBind.GetCom<Button>("SearchFriendRoot");
            mSearchFriendRoot.onClick.AddListener(_onSearchFriendRootButtonClick);
            mMyHead = mBind.GetCom<ReplaceHeadPortraitFrame>("MyHead");
            mHeadIcon = mBind.GetCom<Image>("HeadIcon");
        }
		
		protected override void _unbindExUI()
		{
			if (null != mClose)
			{
				mClose.onClick.RemoveListener(_onCloseButtonClick);
			}
			mClose = null;
			if (null != mBtnChangeAll)
			{
				mBtnChangeAll.onClick.RemoveListener(_onBtnChangeAllButtonClick);
			}
			mBtnChangeAll = null;
            comFriendRecommendList = null;
			if (null != mBtnSearch)
			{
				mBtnSearch.onClick.RemoveListener(_onBtnSearchButtonClick);
			}
			mBtnSearch = null;
			mGrayChangeAll = null;
			mLabelChangeAll = null;
			mInputField = null;
            if (mBtnAddAll != null)
            {
                mBtnAddAll.onClick.RemoveListener(_OnBtnAddAllButtonClick);
            }
            mBtnAddAll = null;
            mGrayAddAll = null;
            mFriendParent = null;
            mSearchFriendInfo = null;
            mSearchFriendRoot.onClick.RemoveListener(_onSearchFriendRootButtonClick);
            mSearchFriendRoot = null;
            mMyHead = null;
            mHeadIcon = null;
        }
		#endregion
        #region Callback
        private void _onCloseButtonClick()
        {
            frameMgr.CloseFrame(this);
        }
        private void _onBtnChangeAllButtonClick()
        {
          if (iChangeCoolDown != 5)
            {
                return;
            }

            mBtnChangeAll.enabled = false;
            mGrayChangeAll.enabled = true;

            WorldRelationFindPlayersReq req = new WorldRelationFindPlayersReq();
            req.type = (byte)RelationFindType.Friend;
            req.name = "";
            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, req);

            if (mLabelChangeAll != null)
            {
                mLabelChangeAll.text = TR.Value("relation_change_all_desc_param", iChangeCoolDown);
                --iChangeCoolDown;
            }

            InvokeMethod.Invoke(this, 1.0f, _ChangeAllCoolDown);
        }
        private void _onBtnSearchButtonClick()
        {
            if (string.IsNullOrEmpty(mInputField.text))
            {
                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("relation_search_name_empty"));
                return;
            }

            if (m_bWaitSearchRet)
            {
                return;
            }
            m_bWaitSearchRet = true;
            m_bIsQuery = true;
            InvokeMethod.Invoke(this, 0.50f, () =>
            {
                m_bWaitSearchRet = false;
            });

            WorldQueryPlayerReq kCmd = new WorldQueryPlayerReq();
            kCmd.roleId = 0;
            OtherPlayerInfoManager.GetInstance().QueryPlayerType = WorldQueryPlayerType.WQPT_FRIEND;
            kCmd.name = mInputField.text;
            NetManager.instance.SendCommand(ServerType.GATE_SERVER, kCmd);
        }
        private void _OnBtnAddAllButtonClick()
        {
            _OnAddAll();
        }
        
        private void _onSearchFriendRootButtonClick()
        {
            /* put your code in here */
            if (mSearchFriendRoot != null)
            {
                mSearchFriendRoot.CustomActive(false);
            }

        }
        #endregion
        protected sealed override void _OnOpenFrame()
        {
            RelationDataManager.GetInstance().ClearQueryInfo();
            // InitFriendRecommendUIList();
            UpdateMyFriends();
            _RegisterUIEvent();
            SendReconmendInfo();
            UpdateHeadPortraitFrame();
            SetPlayerIcon();
            iChangeCoolDown = 5;
        }

        protected sealed override void _OnCloseFrame()
        {
            InvokeMethod.RemoveInvokeCall(this);
            RelationDataManager.GetInstance().ClearQueryInfo();
            data = null;
            _UnRegisterUIEvent();
            iChangeCoolDown = 5;
        }

        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/RelationFrame/FriendRecommendedFrame";
        }

        protected void _RegisterUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnRecievRecommendFriend, _OnRecommendFriend);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnRecvQueryPlayer, _OnRecvQueryPlayer);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnRelationChanged, _OnRelationChanged);
        }

        protected void _UnRegisterUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnRecievRecommendFriend, _OnRecommendFriend);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnRecvQueryPlayer, _OnRecvQueryPlayer);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnRelationChanged, _OnRelationChanged);
        }
        
        void _OnRecommendFriend(UIEvent uiEvent)
        {
            UIEventRecievRecommendFriend recvList = uiEvent as UIEventRecievRecommendFriend;

            data = new FriendRecommentData();
            data.relationlist = new List<RelationData>();
            data.relationlist.AddRange(recvList.m_friendList);
            // comFriendRecommendList.SetElementAmount(data.relationlist.Count);
            UpdateMyFriends();
            mGrayAddAll.enabled = false;
            mBtnAddAll.enabled = true;
        }

        void UpdateHeadPortraitFrame()
        {
            if (mMyHead != null)
            {
                if (HeadPortraitFrameDataManager.WearHeadPortraitFrameID != 0)
                {
                    mMyHead.ReplacePhotoFrame(HeadPortraitFrameDataManager.WearHeadPortraitFrameID);
                }
                else
                {
                    mMyHead.ReplacePhotoFrame(HeadPortraitFrameDataManager.iDefaultHeadPortraitID);
                }
            }
        }

        void SetPlayerIcon()
        {
            string path = "";

            JobTable jobData = TableManager.GetInstance().GetTableItem<JobTable>(PlayerBaseData.GetInstance().JobTableID);
            if (jobData != null)
            {
                ResTable resData = TableManager.GetInstance().GetTableItem<ResTable>(jobData.Mode);
                if (resData != null)
                {
                    path = resData.IconPath;
                }
            }
            if (mHeadIcon != null)
                ETCImageLoader.LoadSprite(ref mHeadIcon, path);
        }

        void UpdateMyFriends()
        {
            if (null == data)
            {
                return;
            }

            if (null == mFriendParent)
            {
                return;
            }

            for (int i = 0; i < mFriendParent.childCount; i++)
            {
                var go = mFriendParent.GetChild(i).gameObject;
                if (null != go)
                {
                    go.CustomActive(false);   
                }
            }
            
            if (data.relationlist.Count <= 0)
            {
                return;
            }

            for (int i = 0; i < data.relationlist.Count; i++)
            {
                if (i >= ShowCnt)
                {
                    continue;
                }

                var itemGo = mFriendParent.GetChild(i).gameObject;
                if (null != itemGo)
                {
                    var itemData = itemGo.GetComponent<ComRecommendFriendInfo>();
                    if (null == itemData)
                    {
                        continue;
                    }
                    itemData.OnItemVisible(data.relationlist[i]);
                    itemGo.CustomActive(true);
                }
            }
            
        }

        void _OnRecvQueryPlayer(UIEvent uiEvent)
        {
            m_bWaitSearchRet = false;
            m_bIsQuery = false;
            //if (data.eRelationOptionType == RelationOptionType.ROT_RECOMMAND)
            {
                UIEventRecvQueryPlayer recvEvent = uiEvent as UIEventRecvQueryPlayer;
                if (null != recvEvent)
                {
                    var relation =
                        new RelationData
                        {
                            uid = recvEvent._info.id,
                            name = recvEvent._info.name,
                            level = recvEvent._info.level,
                            occu = recvEvent._info.occu,
                            isOnline = 1,
                            type = 0,
                            vipLv = recvEvent._info.vipLevel,
                            status = 0,
                            seasonLv = recvEvent._info.seasonLevel,
                        };
                    // data.relationlist = new List<RelationData>();
                    // data.relationlist.Add(new RelationData
                    // {
                    //     uid = recvEvent._info.id,
                    //     name = recvEvent._info.name,
                    //     level = recvEvent._info.level,
                    //     occu = recvEvent._info.occu,
                    //     isOnline = 1,
                    //     type = 0,
                    //     vipLv = recvEvent._info.vipLevel,
                    //     status = 0,
                    //     seasonLv = recvEvent._info.seasonLevel,
                    // });

                    // comFriendRecommendList.SetElementAmount(data.relationlist.Count);
                    bool inList = false;
                    for (int i = 0; i < data.relationlist.Count; i++)
                    {
                        if (data.relationlist[i] != null)
                        {
                            if (data.relationlist[i].occu == relation.occu)
                            {
                                inList = true;
                                break;
                            }
                        }
                    }

                    if (inList)
                    {
                        ShowSearchFriend(relation);
                        UpdateMyFriends();
                        mGrayAddAll.enabled = false;
                        mBtnAddAll.enabled = true;   
                    }
                }
            }
        }

        void ShowSearchFriend(RelationData data)
        {
            mSearchFriendRoot.CustomActive(false);
            if (null == data)
            {
                return;
            }
            mSearchFriendRoot.CustomActive(true);
            if (mSearchFriendInfo != null)
            {
                mSearchFriendInfo.OnItemVisible(data);
            }

        }

        void _OnRelationChanged(UIEvent uiEvent)
        {
            var relationData = uiEvent.Param1 as RelationData;
            if (relationData != null)
            {
                if (relationData.type == (int)RelationType.RELATION_FRIEND)
                {
                    if (data != null && data.relationlist != null)
                    {
                        for (int i = 0; i < data.relationlist.Count; i++)
                        {
                            var mData = data.relationlist[i];
                            if (mData.uid != relationData.uid)
                            {
                                continue;
                            }

                            data.relationlist.Remove(mData);
                            break;
                        }

                        // if (comFriendRecommendList != null)
                        // {
                        //     comFriendRecommendList.SetElementAmount(data.relationlist.Count);
                        // }
                        UpdateMyFriends();
                    }
                }
            }
        }
        public void SendReconmendInfo()
        {
            WorldRelationFindPlayersReq req = new WorldRelationFindPlayersReq();
            req.type = (byte)RelationFindType.Friend;
            req.name = "";
            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, req);
        }
        public bool IsQuerying()
        {
            return m_bIsQuery;
        }
        private void InitFriendRecommendUIList()
        {
            comFriendRecommendList.Initialize();
            comFriendRecommendList.onBindItem = (GameObject gameObject) =>
            {
                return gameObject.GetComponent<ComRecommendFriendInfo>();
            };
            comFriendRecommendList.onItemVisiable = (ComUIListElementScript item) =>
            {
                if (item != null)
                {
                    var current = item.gameObjectBindScript as ComRecommendFriendInfo;
                    if (current != null)
                    {
                        if (item.m_index >= 0 && item.m_index < data.relationlist.Count)
                        {
                            current.OnItemVisible(data.relationlist[item.m_index]);
                        }
                    }
                }
            };
        }

        private void SetFriendRecommendElementAmount()
        {
            if (data != null)
            {
                comFriendRecommendList.SetElementAmount(data.relationlist.Count);
            }
        }

        void _OnAddAll()
        {
            if(data != null)
            {
                if (null != data.relationlist && data.relationlist.Count > 0)
                {
                    mGrayAddAll.enabled = true;
                    mBtnAddAll.enabled = false;

                    for (int i = 0; i < data.relationlist.Count; ++i)
                    {
                        string name = data.relationlist[i].name;
                        ulong uid = data.relationlist[i].uid;
                        if (this != null)
                        {
                            InvokeMethod.Invoke(this, i * 0.20f, () =>
                            {
                                RelationDataManager.GetInstance().AddQueryInfo(uid);

                                SceneRequest req = new SceneRequest();
                                req.type = (byte)RequestType.RequestFriendByName;
                                req.targetName = name;
                                NetManager netMgr = NetManager.Instance();
                                netMgr.SendCommand(ServerType.GATE_SERVER, req);

                                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.RelationAddRecommendFriendMsgSended, uid);
                            });
                        }
                    }
                }
            }
        }

        #region changeAll
        int iChangeCoolDown = 5;
        void _ChangeAllCoolDown()
        {
            if (mLabelChangeAll != null)
            {
                mLabelChangeAll.text = TR.Value("relation_change_all_desc_param", iChangeCoolDown);
            }
            --iChangeCoolDown;

            if (iChangeCoolDown == -1)
            {
                if (mLabelChangeAll != null)
                {
                    mLabelChangeAll.text = TR.Value("relation_change_all_desc", "");
                }
                iChangeCoolDown = 5;
                mBtnChangeAll.enabled = true;
                mGrayChangeAll.enabled = false;
            }
            else
            {
                InvokeMethod.Invoke(this, 1.0f, _ChangeAllCoolDown);
            }
        }
        #endregion
    }
}
