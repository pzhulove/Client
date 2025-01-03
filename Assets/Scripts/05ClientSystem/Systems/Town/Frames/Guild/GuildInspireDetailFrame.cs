using UnityEngine.UI;
using Scripts.UI;
using Protocol;
using Network;
using System.Collections.Generic;

namespace GameClient
{
    class GuildInspireDetailFrame : ClientFrame
    {
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Guild/GuildInspireDetailFrame";
        }

        protected override void _OnOpenFrame()
        {
            SendInspireListReq();
            _RegisterUIEvent();
        }

        protected override void _OnCloseFrame()
        {
            _UnRegisterUIEvent();
            ClearData();
        }

        void ClearData()
        {
        }

        void _RegisterUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GuildInspireInfoUpdate, UpdateInterface);
        }

        void _UnRegisterUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GuildInspireInfoUpdate, UpdateInterface);
        }

        void UpdateInterface(UIEvent iEvent)
        {
            List<GuildBattleInspireInfo> ListData = GuildDataManager.GetInstance().GetGuildBattleInspireInfoList();

            for (int i = 0; i < ListData.Count; i++)
            {
                GuildBattleInspireInfo PreData = ListData[i];

                for (int j = i + 1; j < ListData.Count; j++)
                {
                    GuildBattleInspireInfo AftData = ListData[j];

                    if (ListData[j].inspireNum > ListData[i].inspireNum)
                    {
                        GuildBattleInspireInfo temp = new GuildBattleInspireInfo();

                        GuildDataManager.GetInstance().SetDataExchange(ref temp, ListData[i]);
                        GuildDataManager.GetInstance().SetDataExchange(ref PreData, ListData[j]);
                        GuildDataManager.GetInstance().SetDataExchange(ref AftData, temp);
                    }
                }
            }

            InitScrollListBind(ListData);
            RefreshListCount();
        }

        void InitScrollListBind(List<GuildBattleInspireInfo> ListData)
        {
            mMemList.Initialize();

            mMemList.onItemVisiable = (item) =>
            {
                if (item.m_index >= 0)
                {
                    UpdateScrollListBind(item, ListData);
                }
            };

            mMemList.OnItemRecycle = (item) =>
            {
                ComCommonBind combind = item.GetComponent<ComCommonBind>();
                if (combind == null)
                {
                    return;
                }
            };
        }

        void UpdateScrollListBind(ComUIListElementScript item, List<GuildBattleInspireInfo> ListData)
        {
            ComCommonBind combind = item.GetComponent<ComCommonBind>();
            if (combind == null)
            {
                return;
            }

            if (item.m_index < 0 || item.m_index >= ListData.Count)
            {
                return;
            }

            Text Name = combind.GetCom<Text>("Name");
            Text Num = combind.GetCom<Text>("Num");
            Image Back1 = combind.GetCom<Image>("Back1");
            Image Back2 = combind.GetCom<Image>("Back2");

            Back1.gameObject.CustomActive(item.m_index % 2 == 0);
            Back2.gameObject.CustomActive(item.m_index % 2 == 1);

            Name.text = ListData[item.m_index].playerName;
            Num.text = ListData[item.m_index].inspireNum.ToString();
        }

        void RefreshListCount()
        {
            List<GuildBattleInspireInfo> ListData = GuildDataManager.GetInstance().GetGuildBattleInspireInfoList();
            mMemList.SetElementAmount(ListData.Count);

            int TotalNum = 0;
            for(int i = 0; i < ListData.Count; i++)
            {
                TotalNum += (int)ListData[i].inspireNum;
            }

            mTotalNum.text = TotalNum.ToString();
        }

        void SendInspireListReq()
        {
            WorldGuildBattleInspireInfoReq req = new WorldGuildBattleInspireInfoReq();

            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, req);
        }

        #region ExtraUIBind
        private Button mBtClose = null;
        private ComUIListScript mMemList = null;
        private Text mTotalNum = null;

        protected override void _bindExUI()
        {
            mBtClose = mBind.GetCom<Button>("btClose");
            mBtClose.onClick.AddListener(_onBtCloseButtonClick);
            mMemList = mBind.GetCom<ComUIListScript>("MemList");
            mTotalNum = mBind.GetCom<Text>("TotalNum");
        }

        protected override void _unbindExUI()
        {
            mBtClose.onClick.RemoveListener(_onBtCloseButtonClick);
            mBtClose = null;
            mMemList = null;
            mTotalNum = null;
        }
        #endregion

        #region Callback
        private void _onBtCloseButtonClick()
        {
            frameMgr.CloseFrame(this);
        }
        #endregion
    }
}
