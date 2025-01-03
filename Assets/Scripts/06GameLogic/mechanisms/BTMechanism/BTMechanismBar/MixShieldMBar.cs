#if !LOGIC_SERVER
using UnityEngine;

namespace BehaviorTreeMechanism
{
    /// <summary>
    /// 炼金 小鹿合体护盾条
    /// </summary>
    public class MixShieldMBar : MechanismBar
    {
        protected string GetBarPath()
        {
            return "UIFlatten/Prefabs/BattleUI/DungeonBar/DungeonCharactorHeadEnergy";
        }

        private ComDungeonCharactorBarEnergy m_EnergyBar;

        public MixShieldMBar(BeActor actor) : base(actor)
        {
            CreateUI();
        }

        private void CreateUI()
        {
            if (IsAttachActor())
            {
                var barObj = CGameObjectPool.instance.GetGameObject(GetBarPath(), enResourceType.UIPrefab,
                    (uint) GameObjectPoolFlag.ReserveLast);
                if (barObj == null)
                    return;

                m_EnergyBar = barObj.GetComponent<ComDungeonCharactorBarEnergy>();
                GetAttachActor().m_pkGeActor?.CreateBarRoot();
                if (GetAttachActor().m_pkGeActor?.mBarsRoot?.hRoot != null)
                {
                    GameObject barsRoot = GetAttachActor().m_pkGeActor.mBarsRoot.hRoot;
                    Utility.AttachTo(barObj, barsRoot);
                }
            }
        }

        public override void SetMaxValue(int v)
        {
            if (m_EnergyBar == null)
                return;
            m_EnergyBar.InitData(v);
        }

        public override void SetCurValue(int v)
        {
            if (m_EnergyBar == null)
                return;
            m_EnergyBar.RefreshData(v);
        }

        public override void SetActive(bool isShow)
        {
            if (m_EnergyBar == null)
                return;
            m_EnergyBar.GetGameObject().SetActive(isShow);
        }

        public override void Destroy()
        {
            if (m_EnergyBar == null)
                return;

            GameObject obj = m_EnergyBar.GetGameObject();
            CGameObjectPool.instance.RecycleGameObject(obj);
            m_EnergyBar = null;
        }
    }
}
#endif