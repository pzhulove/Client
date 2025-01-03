using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace GameClient
{
    public class DungeonMouFinishFrame : ClientFrame, IDungeonFinish
    {
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Battle/Finish/DungeonMouFinish";
        }
        #region ExtraUIBind
        private Button mClose = null;
        private ComItemList mComItems = null;
        private ComExpBar mExpBar = null;
        private Text mExpValue = null;
        private Text mName = null;
        private Text title = null;
        private RectTransform text = null;
        private GameObject finalTestContainer;
        private GameObject b;
        private GameObject t;
        private FinalTestPlayerInfo info;
        private TextEx mGoldReward = null;
        protected override void _bindExUI()
        {
            mClose = mBind.GetCom<Button>("close");
            mClose.onClick.AddListener(_onCloseButtonClick);
            mComItems = mBind.GetCom<ComItemList>("ComItems");
            mExpBar = mBind.GetCom<ComExpBar>("ExpBar");
            mExpValue = mBind.GetCom<Text>("ExpValue");
            mName = mBind.GetCom<Text>("Name");
            title = mBind.GetCom<Text>("title");
            text = mBind.GetCom<RectTransform>("text");
            finalTestContainer = mBind.GetGameObject("finalTest");
            b = mBind.GetGameObject("b");
            t = mBind.GetGameObject("t");
            info = mBind.GetCom<FinalTestPlayerInfo>("PlayerSelfHPBar");
            mGoldReward = mBind.GetCom<TextEx>("goldReward");
        }

        protected override void _unbindExUI()
        {
            mClose.onClick.RemoveListener(_onCloseButtonClick);
            mClose = null;
            mComItems = null;
            mExpBar = null;
            mExpValue = null;
            mName = null;
            mGoldReward = null;
        }
        #endregion


        #region Callback
        private void _onCloseButtonClick()
        {
            /* put your code in here */
            _onClose();
        }
        #endregion

        private void _onExpUpdate(UIEvent ui)
        {
            _setExp(PlayerBaseData.GetInstance().Exp, false);
        }


        protected override void _OnOpenFrame()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ExpChanged, _onExpUpdate);

            _setExp(BattleDataManager.GetInstance().originExp, true);
            SetTitle();
            SetPlayerInfo();
        }
        private void SetPlayerInfo()
        {
            if (b == null || t == null || finalTestContainer == null) return;
            if (BattleMain.instance != null)
            {
                FinalTestBattle battle = BattleMain.instance.GetBattle() as FinalTestBattle;
                b.CustomActive(battle == null);
                t.CustomActive(battle == null);
                finalTestContainer.CustomActive(battle != null);
                info.gameObject.CustomActive(battle != null);
                if (battle != null && battle.playerInfo!=null)
                {                    
                    info.SetPlayerInfo(battle.playerInfo);
                }
            }
        }

        private void SetTitle()
        {
            if (BattleMain.instance != null)
            {
                FinalTestBattle battle = BattleMain.instance.GetBattle() as FinalTestBattle;
                if (battle != null && title != null && battle.tableData != null)
                {
                    text.gameObject.SetActive(false);
                    title.gameObject.SetActive(true);
                    if (TableManager.instance.IsLastFloor(battle.tableData.ID))
                    {
                        title.text = "通关成功";
                        title.color = new Color(0, 1.0f, 0, 0);                                                               
                    }
                    else
                    {
                        title.text = "挑战成功";
                        title.color = new Color(1.0f,1.0f,1.0f,0);                      
                    }
                    ClientSystemManager.instance.delayCaller.DelayCall(2000, () => 
                    {
                        if(title!=null)
                          title.DOFade(1.0f, 0.2f);
                    });
                }
            }
        }

        protected override void _OnCloseFrame()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ExpChanged, _onExpUpdate);
        }

        public void SetName(string name)
        {
            mName.text = name;
        }

        public void SetBestTime(int time)
        {
        }

        public void SetCurrentTime(int time)
        {
        }

        public void SetDiff(int diff)
        {
        }

        public void SetExtraGoldReward(uint num)
        {
            mGoldReward.CustomActive(true);
            mGoldReward.text = string.Format("+ {0}", num);
        }

        public void SetDrops(ComItemList.Items[] items)
        {
            if (mComItems != null)
                mComItems.SetItems(items);
        }

        private void _setExp(ulong val, bool force)
        {
            if (null != mExpBar)
            {
                mExpBar.SetExp(val, force, exp =>
                {
                    return TableManager.instance.GetCurRoleExp(exp);
                });
            }
        }

        public void SetExp(ulong exp)
        {
            if (mExpValue != null)
                mExpValue.text = exp.ToString();
            _onExpUpdate(null);
        }

        public void SetFinish(bool isFinish)
        {
        }

        public void SetLevel(int lvl)
        {
        }

        private void _onClose()
        {
            ClientSystemManager.instance.CloseFrame(this);
            ClientSystemManager.instance.SwitchSystem<ClientSystemTown>();
        }
    }
    public class FinalPlayerInfo
    {
        public Sprite icon;
        public Material material;
        public int level;
        public string name;
        public int hp;
        public int maxHp;
        public int addHp;
        public int mp;
        public int maxMp;
        public int addMp;
    }
}
