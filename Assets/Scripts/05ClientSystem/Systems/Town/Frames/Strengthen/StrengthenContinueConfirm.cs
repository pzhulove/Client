using UnityEngine;
using UnityEngine.UI;
using System.Collections;
namespace GameClient
{
    class StrengthenContinueConfirm : ClientFrame
    {
        StrengthenConfirmData m_data;
        Text m_kHintText;
        bool m_bHasSend;
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/SmithShop/StrengthenContinueConfirm";
        }

        protected override void _OnOpenFrame()
        {
            m_bHasSend = false;
            m_data = userData as StrengthenConfirmData;
            m_kHintText = Utility.FindComponent<Text>(frame, "middle/Text");
            if (m_data.ItemData.EquipType == EEquipType.ET_COMMON)
            {
                m_kHintText.text = string.Format(TR.Value("strengthen_cs_hint"), m_data.ItemData.GetColorName(), m_data.TargetStrengthenLevel);
            }
            else if (m_data.ItemData.EquipType == EEquipType.ET_REDMARK)
            {
                m_kHintText.text = string.Format(TR.Value("growth_cs_hint"), m_data.ItemData.GetColorName(), m_data.TargetStrengthenLevel);
            }
			
        }

        protected override void _OnCloseFrame()
        {

        }

        [UIEventHandle("close")]
        void OnClickCancel()
        {
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.EndContineStrengthen);
            frameMgr.CloseFrame(this);
        }

        [UIEventHandle("ok")]
        void OnClickOk()
        {
            if(m_bHasSend)
            {
                return;
            }
            m_bHasSend = true;
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.BeginContineStrengthen);
            frameMgr.CloseFrame(this);
        }
    }
}