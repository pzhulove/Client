using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace GameClient
{
    public class DungeonGoldRushFinishFrame : ClientFrame, IDungeonFinish
    {
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Battle/Finish/DungeonGoldRushFinish";
        }
        
        #region ExtraUIBind
        private Button mClose = null;
        private TextEx mGoldReward = null;
        private TextEx mBaseGold = null;
        
        protected override void _bindExUI()
        {
            mClose = mBind.GetCom<Button>("close");
            mClose.onClick.AddListener(_onCloseButtonClick);
            mGoldReward = mBind.GetCom<TextEx>("goldReward");
            mBaseGold = mBind.GetCom<TextEx>("baseGold");
        }

        protected override void _unbindExUI()
        {
            mClose.onClick.RemoveListener(_onCloseButtonClick);
            mClose = null;
            mGoldReward = null;
            mBaseGold = null;
        }
        #endregion
        
        #region Callback
        private void _onCloseButtonClick()
        {
            /* put your code in here */
            _onClose();
        }
        #endregion
        
        private uint mTotalGold;
        private uint mLastGold;
        private bool mStartUpdateGold;

        protected override void _OnOpenFrame()
        {
            mLastGold = mTotalGold = 0;
            mStartUpdateGold = false;
            InvokeMethod.Invoke(6f, StartUpdateGold);
        }

        protected override void _OnCloseFrame()
        {
            InvokeMethod.RemoveInvokeCall(StartUpdateGold);
        }

        public override bool IsNeedUpdate()
        {
            return true;
        }

        protected override void _OnUpdate(float timeElapsed)
        {
            UpdateGold();
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

        public void SetDiff(int diff)
        {
        }

        public void SetExp(ulong exp)
        {
            
        }

        public void SetExtraGoldReward(uint num)
        {
            mTotalGold += num;
            mGoldReward.text = string.Format("+{0}", num);
        }

        public void SetDrops(ComItemList.Items[] items)
        {
            if (items == null)
                return;
            foreach (var item in items)
            {
                if (item.id == Global.GOLD_ITEM_ID2)
                {
                    mLastGold = item.count;
                    mTotalGold += item.count;
                    mBaseGold.text = item.count.ToString();
                    break;
                }
            }
        }

        private void StartUpdateGold()
        {
            mStartUpdateGold = true;
            mGoldReward.CustomActive(false);
        }
        
        private void UpdateGold()
        {
            if (!mStartUpdateGold || mTotalGold <= mLastGold)
                return;
            uint chaseGold = mTotalGold - mLastGold;
            uint chaseStep = 1;
            while (chaseStep * 10 < chaseGold)
            {
                chaseStep *= 10;
            }

            mLastGold += chaseStep;
            mBaseGold.text = mLastGold.ToString();
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
    
}
