#if !LOGIC_SERVER
using UnityEngine;

namespace BehaviorTreeMechanism
{
    public enum ElementType
    {
        Light = 0,    // 雷
        Fire,         // 火
        Ice,           // 冰
        Count
    }
    /// <summary>
    /// 鬼剑士 元素标记
    /// </summary>
    public class ElementTag : MechanismBar
    {
        public ElementTag(BeActor actor) : base(actor)
        {
            CreateUI();
        }

        public override void SetMaxValue(int v)
        {
        }

        public override void SetCurValue(int v)
        {
            if (m_TagUI == null)
                return;

            if (v == 0)
            {
                SetActive(false);
                return;
            }
            SetActive(true);
            for (int i = (int) ElementType.Light; i < (int) ElementType.Count; i++)
            {
                m_TagUI.SetElementActive( i, (v & 1 << i) > 0);
            }
        }

        public override void SetActive(bool isShow)
        {
            if (m_TagUI == null)
                return;
            
            m_TagUI.gameObject.SetActive(isShow);
        }
        
        public override void Destroy()
        {
            if (m_TagUI == null)
                return;

            GameObject obj = m_TagUI.gameObject;
            CGameObjectPool.instance.RecycleGameObject(obj);
            m_TagUI = null;
        }

        private static string PrefabPath = "UIFlatten/Prefabs/BattleUI/DungeonBar/ElementTag";
        private ComElementTag m_TagUI;
        private void CreateUI()
        {
            if (!IsAttachActor())
                return;
            
            var barObj = CGameObjectPool.instance.GetGameObject(PrefabPath, enResourceType.UIPrefab, (uint) GameObjectPoolFlag.ReserveLast);
            if (barObj == null)
                return;

            m_TagUI = barObj.GetComponent<ComElementTag>();
            GetAttachActor().m_pkGeActor?.CreateBarRoot();
            if (GetAttachActor().m_pkGeActor?.mBarsRoot?.hRoot != null)
            {
                GameObject barsRoot = GetAttachActor().m_pkGeActor.mBarsRoot.hRoot;
                Utility.AttachTo(barObj, barsRoot);
            }
        }
    }
}
#endif