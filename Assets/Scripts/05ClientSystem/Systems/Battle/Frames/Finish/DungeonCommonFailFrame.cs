using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using UnityEngine.UI;
using Protocol;
using Network;
using System.Diagnostics;

namespace GameClient
{
    public class DungeonCommonFailFrame : ClientFrame, IDungeonFinish
    {
#region Override
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Battle/Finish/DungeonCommonFailFinish";
        }
#endregion

#region ExtraUIBind
        private Button mClose = null;
        private ComItemList mComItems = null;
        private ComExpBar mExpBar = null;
        private Text mExpValue = null;
        private Text mName = null;
        private GameObject tip = null;
        private GameObject finalTestContainer;
        private FailBossInfo boss01Info;
        private FailBossInfo boss02Info;
        private GameObject t;
        private GameObject b;
        protected override void _bindExUI()
        {
            mClose = mBind.GetCom<Button>("close");
            mClose.onClick.AddListener(_onCloseButtonClick);
            mComItems = mBind.GetCom<ComItemList>("ComItems");
            mExpBar = mBind.GetCom<ComExpBar>("ExpBar");
            mExpValue = mBind.GetCom<Text>("ExpValue");
            mName = mBind.GetCom<Text>("Name");
            tip = mBind.GetGameObject("tip");
            t = mBind.GetGameObject("t");
            b = mBind.GetGameObject("b");
            finalTestContainer = mBind.GetGameObject("finalTest");
            boss01Info = mBind.GetCom<FailBossInfo>("HPBar_BOSS_NEW01");
            boss02Info = mBind.GetCom<FailBossInfo>("HPBar_BOSS_NEW02");
        }

        protected override void _unbindExUI()
        {
            mClose.onClick.RemoveListener(_onCloseButtonClick);
            mClose = null;
            mComItems = null;
            mExpBar = null;
            mExpValue = null;
            mName = null;
            tip = null;
        }
#endregion   

#region Callback
        private void _onCloseButtonClick()
        {
            /* put your code in here */
            _onBack();
        }
#endregion

        protected override void _OnOpenFrame()
        {
            _setExp(PlayerBaseData.GetInstance().Exp, true);
            SetTip();
            SetBossInfo();
        }

        private void SetTip()
        {
            if (BattleMain.battleType == BattleType.FinalTestBattle)
            {
                tip.CustomActive(true);
            }
        }
        private void _setExp(ulong val, bool force)
        {
            if (mExpBar != null)
            {
                mExpBar.SetExp(val, force, exp =>
                {
                    return TableManager.instance.GetCurRoleExp(exp);
                });
            }
        }
        private void SetBossInfo()
        {
           var battle =  BattleMain.instance.GetBattle() as FinalTestBattle;
            if (t == null || b == null || finalTestContainer == null) return;
            if (battle != null)
            {
                t.CustomActive(false);
                b.CustomActive(false);
                finalTestContainer.CustomActive(true);
                boss01Info.gameObject.CustomActive(battle.bossInfo01 != null);
                boss02Info.gameObject.CustomActive(battle.bossInfo02 != null);
                if (battle.bossInfo01 != null && boss01Info)
                {
                    boss01Info.SetBossInfo(battle.bossInfo01);
                }
                if (battle.bossInfo02 != null && boss02Info)
                {
                    boss02Info.SetBossInfo(battle.bossInfo02);
                }
            }
            else
            {
                finalTestContainer.CustomActive(false);
                t.CustomActive(true);
                b.CustomActive(true);
            }
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

        public void SetDrops(GameClient.ComItemList.Items[] items)
        {
        }

        public void SetExp(ulong exp)
        {
            mExpValue.text = exp.ToString();
            _setExp(PlayerBaseData.GetInstance().Exp + exp, false);
        }

        public void SetDiff(int diff)
        {
        }

        public void SetFinish(bool isFinish)
        {
        }

        public void SetLevel(int level)
        {
        }

        private void _onBack()
        {
            frameMgr.CloseFrame(this);
            ClientSystemManager.instance.SwitchSystem<ClientSystemTown>();
        }
    }
}
public class BossInfo
{
    public Sprite icon;
    public Material material;
    public string name;
    public int level;
    public float hpRate;
}