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
    public class DungeonDeadTowerFailFrame : ClientFrame, IDungeonFinish
    {

#region Override
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Battle/Finish/DungeonDeadTowerFail";
        }
#endregion

#region ExtraUIBind
        private Text mLevel = null;
        private Button mBack = null;
        private Button mOnUplvlSkill = null;
        private Button mOnStrenghEquip = null;
        private Button mOnGetEquip = null;
        private Button mOnSummonHelp = null;

        protected override void _bindExUI()
        {
            mLevel = mBind.GetCom<Text>("level");
            mBack = mBind.GetCom<Button>("back");
            mBack.onClick.AddListener(_onBackButtonClick);
            mOnUplvlSkill = mBind.GetCom<Button>("onUplvlSkill");
            mOnUplvlSkill.onClick.AddListener(_onOnUplvlSkillButtonClick);
            mOnStrenghEquip = mBind.GetCom<Button>("onStrenghEquip");
            mOnStrenghEquip.onClick.AddListener(_onOnStrenghEquipButtonClick);
            mOnGetEquip = mBind.GetCom<Button>("onGetEquip");
            mOnGetEquip.onClick.AddListener(_onOnGetEquipButtonClick);
            mOnSummonHelp = mBind.GetCom<Button>("onSummonHelp");
            mOnSummonHelp.onClick.AddListener(_onOnSummonHelpButtonClick);
        }

        protected override void _unbindExUI()
        {
            mLevel = null;
            mBack.onClick.RemoveListener(_onBackButtonClick);
            mBack = null;
            mOnUplvlSkill.onClick.RemoveListener(_onOnUplvlSkillButtonClick);
            mOnUplvlSkill = null;
            mOnStrenghEquip.onClick.RemoveListener(_onOnStrenghEquipButtonClick);
            mOnStrenghEquip = null;
            mOnGetEquip.onClick.RemoveListener(_onOnGetEquipButtonClick);
            mOnGetEquip = null;
            mOnSummonHelp.onClick.RemoveListener(_onOnSummonHelpButtonClick);
            mOnSummonHelp = null;
        }
#endregion   

#region Callback
        private void _onBackButtonClick()
        {
            /* put your code in here */
            _onBack();
        }

        private void _onOnUplvlSkillButtonClick()
        {
            /* put your code in here */

            ClientSystemManager.instance.Push2FrameStack(new OpenClientFrameStackCmd(typeof(SkillFrame)));
            _onBack();
        }

        private void _onOnStrenghEquipButtonClick()
        {
            /* put your code in here */
            ClientSystemManager.instance.Push2FrameStack(new OpenClientFrameStackCmd(typeof(SmithShopNewFrame)));
            _onBack();
        }

        private void _onOnGetEquipButtonClick()
        {
            /* put your code in here */
            ClientSystemManager.instance.Push2FrameStack(new OpenClientFrameStackCmd(typeof(PocketJarFrame)));
            _onBack();
        }

        private void _onOnSummonHelpButtonClick()
        {
            /* put your code in here */
            //ClientSystemManager.instance.Push2FrameStack(new OpenClientFrameStackCmd(typeof(MagicJarFrame)));
            _onBack();
        }
#endregion

        public void SetLevel(int level)
        {
            if (null != mLevel)
            {
                mLevel.text = string.Format("{0}", level);
            }
        }

        public void SetName(string name)
        {
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
        }

        public void SetDiff(int diff)
        {
        }

        public void SetFinish(bool isFinish)
        {
        }

        private void _onBack()
        {
            frameMgr.CloseFrame(this);

            ClientSystemManager.instance.SwitchSystem<ClientSystemTown>();
        }
    }
}
