using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class BattleUIRedPacket : BattleUIBase
    {
        #region ExtraUIBind
        private GameObject mBtnRedPacket = null;
        private Text mTextRedPacketCount = null;

        protected override void _bindExUI()
        {
            mBtnRedPacket = mBind.GetGameObject("BtnRedPacket");
            mTextRedPacketCount = mBind.GetCom<Text>("TextRedPacketCount");
        }

        protected override void _unbindExUI()
        {
            mBtnRedPacket = null;
            mTextRedPacketCount = null;
        }
        #endregion

        protected override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/BattleUI/BattleUIComponent/BattleUIRedPacket";
        }

        protected override void OnEnter()
        {
            base.OnEnter();

            _UpdateRedPacket();

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.RedPacketOpenSuccess, _OnRedPacketOpenSuccess);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.RedPacketGet, _OnNewRedPacketGet);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.RedPacketDelete, _OnDeleteRedPacket);
        }

        protected override void OnExit()
        {
            base.OnExit();

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.RedPacketOpenSuccess, _OnRedPacketOpenSuccess);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.RedPacketGet, _OnNewRedPacketGet);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.RedPacketDelete, _OnDeleteRedPacket);
        }


        private void _UpdateRedPacket()
        {
            if (BattleMain.IsModePvP(BattleMain.battleType) == false)
            {
                int nCount = RedPackDataManager.GetInstance().GetWaitOpenCount();
                if (mBtnRedPacket != null)
                {
                    mBtnRedPacket.gameObject.SetActive(nCount > 0/* && RedPackDataManager.GetInstance().CheckNewYearActivityOpen()*/);
                }

                if (mTextRedPacketCount != null)
                {
                    mTextRedPacketCount.gameObject.SetActive(nCount > 1/* && RedPackDataManager.GetInstance().CheckNewYearActivityOpen()*/);
                    mTextRedPacketCount.text = nCount.ToString();
                }
            }
            else
            {
                if (mBtnRedPacket != null)
                {
                    mBtnRedPacket.gameObject.SetActive(false);
                }

                if (mTextRedPacketCount != null)
                {
                    mTextRedPacketCount.gameObject.SetActive(false);
                }
            }
        }



        protected void _OnRedPacketOpenSuccess(UIEvent a_event)
        {
            if (BattleMain.IsModePvP(BattleMain.battleType))
            {
                if (ClientSystemManager.instance.IsFrameOpen<OpenRedPacketFrame>() == false)
                {
                    ClientSystemManager.instance.OpenFrame<OpenRedPacketFrame>(FrameLayer.Middle, a_event.Param1);
                }
            }

            _UpdateRedPacket();
        }

        protected void _OnNewRedPacketGet(UIEvent a_event)
        {
            _UpdateRedPacket();
        }

        protected void _OnDeleteRedPacket(UIEvent a_event)
        {
            _UpdateRedPacket();
        }
    }
}