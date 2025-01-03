using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;


namespace GameClient
{
    class SingleItemData
    {
        public string key;
        public string value;
    }

    class ActorShowPkPointData
    {
        public List<SingleItemData> akItemData;
    }

    class ActorShowPkPoint :ClientFrame
    {
        public static void Open(FrameLayer eLayer,object data)
        {
            if(ClientSystemManager.GetInstance().IsFrameOpen<ActorShowPkPoint>())
            {
                ClientSystemManager.GetInstance().CloseFrame<ActorShowPkPoint>();
            }
            ClientSystemManager.GetInstance().OpenFrame<ActorShowPkPoint>(eLayer,data);
        }
        Button m_kButton;
        ActorShowPkPointData m_kData;

        public override string GetPrefabPath()
        {
            return "UI/Prefabs/ActorGroup/ActorShowPkPoint";
        }

        GameObject m_goParent;
        GameObject m_goPrefab;
        protected override void _OnOpenFrame()
        {
            m_kButton = frame.GetComponent<Button>();
            m_kButton.onClick.RemoveAllListeners();
            m_kButton.onClick.AddListener(() => { frameMgr.CloseFrame(this); });

            m_goParent = Utility.FindChild(frame, "main/ItemArray");
            m_goPrefab = Utility.FindChild(m_goParent, "backItem");
            m_goPrefab.CustomActive(false);

            m_kData = userData as ActorShowPkPointData;

            for (int i = 0; i < m_kData.akItemData.Count; ++i)
            {
                var itemData = m_kData.akItemData[i];
                var current = GameObject.Instantiate(m_goPrefab);
                Utility.AttachTo(current, m_goParent);

                current.CustomActive(true);
                var key = Utility.FindComponent<Text>(current, "Key");
                var value = Utility.FindComponent<Text>(current, "Value");
                var back = current.GetComponent<Image>();

                back.enabled = (i & 1) > 0;
                key.text = itemData.key;
                value.text = itemData.value;
            }
        }

        protected override void _OnCloseFrame()
        {
            m_kButton.onClick.RemoveAllListeners();
            m_kButton = null;
            m_kData = null;
        }
    }
}