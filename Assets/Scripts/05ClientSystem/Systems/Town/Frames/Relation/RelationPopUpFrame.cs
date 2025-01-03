using Network;
using Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
///////删除linq
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    class CommonPlayerInfo
    {
        public enum CommonPlayerType
        {
            CPT_NONE = 0,
            CPT_RECOMEND,       //推荐好友玩家
            CPT_COMMON,         //普通玩家
            CPT_PRIVAT_CHAT,    //私聊玩家
            CPT_BLACK,          //黑名单玩家
            CPT_TEACHER_REAL = 50,   // 当前的师父
            CPT_TEACHER,             // 向我推荐的师父
            CPT_PUPIL_REAL,               //徒弟
            CPT_PUPIL_APPLY,              //向我申请的徒弟
            CPT_DEL_PRIVATE_CHAT,         //聊天界面 - 和已经在私聊的人所要弹出的菜单
            CPT_CLASSMATE,                //师徒系统中的同门
            CPT_TEAMDUPLICATION,                //团本
        }

        public GameObject m_friendPrefab;        // 显示对象

        Button m_addFriendBtn;
        Button m_button;
        Button m_menu;
        Text m_stranger;
        Image m_redPt;

        public ulong m_uid;
        public string m_name;
        public UInt16 m_level;
        public byte m_occu;
        public UInt16 m_vipLv;
        public CommonPlayerType m_type;
        public byte m_isOnline;
        public byte m_relationType;

        public CommonPlayerInfo(ulong uid, string name, UInt16 lv, byte occu, CommonPlayerType type, bool dirty, byte online, byte relationType, byte vipLv)
        {
            m_uid = uid;
            m_name = name;
            m_level = lv;
            m_occu = occu;
            m_type = type;
            m_vipLv = vipLv;
            m_isOnline = online;
            m_relationType = relationType;

            m_friendPrefab = AssetLoader.instance.LoadRes("UIFlatten/Prefabs/Common/ComPlayerInfo").obj as GameObject;
            m_friendPrefab.SetActive(true);

            m_addFriendBtn = Utility.FindComponent<Button>(m_friendPrefab, "AddFriend");
            m_menu = Utility.FindComponent<Button>(m_friendPrefab, "Menu");
            m_stranger = Utility.FindComponent<Text>(m_friendPrefab, "Stranger");
            m_button = Utility.FindComponent<Button>(m_friendPrefab, "Button");
            m_redPt = Utility.FindComponent<Image>(m_friendPrefab, "RedPt");
            m_redPt.gameObject.SetActive(dirty);

            m_button.onClick.AddListener(OnClickButton);
            m_addFriendBtn.onClick.AddListener(OnClickAddFriendBtn);
            m_menu.onClick.AddListener(OnMenu);

            SetupInfo();
        }
        ~CommonPlayerInfo()
        {
            Finatial();
        }


        public void Finatial()
        {
            m_addFriendBtn.onClick.RemoveListener(OnClickAddFriendBtn);
            m_menu.onClick.RemoveListener(OnMenu);

            m_friendPrefab = null;
            m_addFriendBtn = null;
            
        }
        

        private void SetupInfo()
        {
            ProtoTable.JobTable job = TableManager.instance.GetTableItem<ProtoTable.JobTable>(m_occu);
            if (job != null)
            {
                ProtoTable.ResTable resData = TableManager.GetInstance().GetTableItem<ProtoTable.ResTable>(job.Mode);
                if (resData != null)
                {
                    Image headIcon = Utility.FindComponent<Image>(m_friendPrefab, "Image");
                    // headIcon.sprite = Utility.createSprite(resData.IconPath);
                    Utility.createSprite(resData.IconPath, ref headIcon);

                    UIGray gray = Utility.FindComponent<UIGray>(m_friendPrefab, "Image");
                    gray.enabled = m_isOnline > 0 ? false : true;
                }

                Text OccuDesc = Utility.FindComponent<Text>(m_friendPrefab, "Job");
                OccuDesc.text = job.Name;
            }

            Text name = Utility.FindComponent<Text>(m_friendPrefab, "Name");
            name.text = string.Format("{0}", m_name);

            Text Lv = Utility.FindComponent<Text>(m_friendPrefab, "Lv/LvText");
            Lv.text = string.Format("Lv.{0}", m_level);

            Text Viplv = Utility.FindComponent<Text>(m_friendPrefab, "VipLv");
            Viplv.text = string.Format("贵{0}", m_vipLv);
            SetupType();
        }

        private void SetupType()
        {
            if (m_type == CommonPlayerType.CPT_RECOMEND)
            {
                m_addFriendBtn.gameObject.SetActive(true);
                m_menu.gameObject.SetActive(false);
                m_stranger.gameObject.SetActive(false);
            }
            else if(m_type == CommonPlayerType.CPT_COMMON)
            {
                m_addFriendBtn.gameObject.SetActive(false);
                m_menu.gameObject.SetActive(true);
                m_stranger.gameObject.SetActive(false);
            }
            else if(m_type == CommonPlayerType.CPT_PRIVAT_CHAT)
            {
                m_addFriendBtn.gameObject.SetActive(false);
                m_menu.gameObject.SetActive(true);
                m_stranger.gameObject.SetActive(m_relationType == (byte)RelationType.RELATION_STRANGER ? true : false);
            }
        }

        //加好友
        public void OnClickAddFriendBtn()
        {
            UIGray gray = Utility.FindComponent<UIGray>(m_friendPrefab, "AddFriend");
            Text btnTxt = Utility.FindComponent<Text>(m_friendPrefab, "AddFriend/Text");
            if(gray.enabled == true)
            {
                return;
            }

            var functionData = TableManager.GetInstance().GetTableItem<ProtoTable.FunctionUnLock>((int)ProtoTable.FunctionUnLock.eFuncType.Friend);
            if (null != functionData && PlayerBaseData.GetInstance().Level < functionData.FinishLevel)
            {
                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("relation_add_friend_need_lv", functionData.FinishLevel));
                return;
            }

            SceneRequest req = new SceneRequest();
            req.type = (byte)RequestType.RequestFriendByName;
            req.targetName = m_name;
            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, req);

            gray.enabled = true;
            btnTxt.text = "已申请";
            m_addFriendBtn.enabled = false;
        }

        private void OnMenu()
        {
            RelationData data = new RelationData();
            data.uid = m_uid;
            data.name = m_name;
            data.level = m_level;
            data.occu = m_occu;
            data.type = m_relationType;
            data.vipLv = (byte)m_vipLv;

            RelationMenuData menuData = new RelationMenuData();
            menuData.m_data = data;
            menuData.type = m_type;

            UIEventSystem.GetInstance().SendUIEvent(new UIEventShowFriendSecMenu(menuData));
        }

        private void OnClickButton()
        {
            if(m_type == CommonPlayerType.CPT_PRIVAT_CHAT)
            {
                RelationData data = new RelationData();
                data.uid = m_uid;
                data.name = m_name;
                data.level = m_level;
                data.occu = m_occu;
                data.type = m_relationType;
                data.isOnline = m_isOnline;
                data.vipLv = (byte)m_vipLv;
                RelationDataManager.GetInstance().OnAddPriChatList(data, false);

                UIEventSystem.GetInstance().SendUIEvent(new UIEventPrivateChat(data, false));
            }
        }
    }

    class RelationPopupFram : ClientFrame
    {
        GameObject m_element;
        RelationData[] m_list;
        List<CommonPlayerInfo> m_players;
        float m_timer;
        int m_refreshCd;

        [UIControl("Dlg/Button")]
        Button m_refreshBtn;
        [UIControl("Dlg/Button/Text")]
        Text m_refreshText;

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Friends/PopupDialog";
        }

        protected override void _OnOpenFrame()
        {
            base._OnOpenFrame();
            
            _RegUIEvent();

            _ChangeRefreshState(true);
            m_players = new List<CommonPlayerInfo>();
        }

        public override bool IsNeedUpdate()
        {
            return true;
        }

        protected override void _OnUpdate(float timeElapsed)
        {
            if(m_refreshCd == 0)
            {
                return;
            }

            m_timer += timeElapsed;
            if(m_timer > 1.0)
            {
                m_refreshCd -= 1;
                _UpdateRefreshTime();
                if(m_refreshCd == 0)
                {
                    _ChangeRefreshState(true);
                }

                m_timer = 0;
            }
        }

        public void SetData(RelationData[] list)
        {
            m_list = list;
            _InitElement();
        }

        protected override void _OnCloseFrame()
        {
            _ClearPlayerList();
            _UnRegUIEvent();
        }

        protected void _RegUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnRecvQueryPlayer, _OnRecvQueryPlayer);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnRecievRecommendFriend, _OnRecievRecommendFriend);
        }

        protected void _UnRegUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnRecvQueryPlayer, _OnRecvQueryPlayer);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnRecievRecommendFriend, _OnRecievRecommendFriend);
        }

        protected void _InitData()
        {
            m_list = userData as RelationData[];
        }

        protected void _InitElement()
        {
            if (m_list == null )
            {
                return;
            }

            GameObject content = Utility.FindGameObject(frame,  "Dlg/Scroll View/Viewport/Content");
            

            for (int i = 0; i < m_list.Length; ++i)
            {
                CommonPlayerInfo player = new CommonPlayerInfo(m_list[i].uid, m_list[i].name, m_list[i].level, 
                    m_list[i].occu, CommonPlayerInfo.CommonPlayerType.CPT_RECOMEND, false, 1, m_list[i].type, m_list[i].vipLv);
                m_players.Add(player);

                player.m_friendPrefab.transform.SetParent(content.transform, false);
            }
        }

        protected void _ClearPlayerList()
        {
            if(m_players == null)
            {
                return;
            }

            for (int i = 0; i < m_players.Count; ++i)
            {
                CGameObjectPool.instance.RecycleGameObject(m_players[i].m_friendPrefab);
                m_players[i].Finatial();
            }

            m_players.Clear();
        }
        
        [UIEventHandle("Dlg/Title/Button")]
        void OnClickClose()
        {
            _OnClickClose();
        }

        protected UnityEngine.Coroutine m_ct;
        [UIEventHandle("Dlg/AddAllBtn")]
        void OnClickAddAllBtn()
        {
            if(m_ct != null)
            {
                return;
            }
           
            m_ct = GameFrameWork.instance.StartCoroutine(_AddBatchFriend());
        }

        public IEnumerator _AddBatchFriend()
        {
            for (int i = 0; i < m_players.Count; ++i)
            //for (int i = 0; i < m_players.Count(); ++i)
            {
                m_players[i].OnClickAddFriendBtn();

                yield return Yielders.GetWaitForSeconds(0.1f);
            }
        }

        [UIEventHandle("Button")]
        void OnClickOutSide()
        {
            _OnClickClose();
        }

        [UIEventHandle("Dlg/Button")]
        void OnRefreshBtn()
        {
            if (m_refreshCd > 0)
            {
                return;
            }

            WorldRelationFindPlayersReq req = new WorldRelationFindPlayersReq();
            req.type = (byte)RelationFindType.Friend;
            req.name = "";
            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, req);
        }

        protected void _OnClickClose()
        {
            if (m_ct != null)
            {
                GameFrameWork.instance.StopCoroutine(m_ct);
                m_ct = null;
            }
            frameMgr.CloseFrame(this);
        }

        protected void _OnRecvQueryPlayer(UIEvent uiEvent)
        {
            UIEventRecvQueryPlayer recvEvent = uiEvent as UIEventRecvQueryPlayer;
            GameObject content = Utility.FindGameObject(frame, "Dlg/Scroll View/Viewport/Content");
            
            CommonPlayerInfo player = new CommonPlayerInfo(recvEvent._info.id, recvEvent._info.name, 
                recvEvent._info.level, recvEvent._info.occu, CommonPlayerInfo.CommonPlayerType.CPT_RECOMEND, false, 1, 0, recvEvent._info.vipLevel);
            m_players.Add(player);

            player.m_friendPrefab.transform.SetParent(content.transform, false);

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnQueryEnd);
        }

        protected void _OnRecievRecommendFriend(UIEvent uiEvent)
        {
            UIEventRecievRecommendFriend recvList = uiEvent as UIEventRecievRecommendFriend;
            _ClearPlayerList();
            SetData(recvList.m_friendList);

            _ChangeRefreshState(false);
        }

        protected void _ChangeRefreshState(bool validate)
        {
            UIGray gray = m_refreshBtn.GetComponent<UIGray>();
            if(null == gray)
            {
                return;
            }

            if (validate)
            {
                m_refreshBtn.enabled = true;
                gray.enabled = false;
                m_refreshText.text = "换一批";
                m_refreshCd = 0;
            }
            else
            {
                m_refreshBtn.enabled = false;
                gray.enabled = true;
                m_refreshText.text = string.Format("换一批5s");
                m_refreshCd = 5;

                if (m_ct != null)
                {
                    GameFrameWork.instance.StopCoroutine(m_ct);
                    m_ct = null;
                }
            }
        }

        protected void _UpdateRefreshTime()
        {
            m_refreshText.text = string.Format("换一批{0}s", m_refreshCd);
        }
    }
}
