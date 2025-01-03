#if !LOGIC_SERVER
using behaviac;

namespace BehaviorTreeMechanism
{
    public interface IMechanismBar
    {
        void SetMaxValue(int v);
        void SetCurValue(int v);
        void SetActive(bool isShow);
        void Destroy();
    }

    /// <summary>
    /// 机制UI条，用于对接行为树的护盾条，能量条等功能
    /// </summary>
    public abstract class MechanismBar : IMechanismBar
    {
        public static MechanismBar Create(behaviac.MethbarType type, BeActor actor)
        {
            switch (type)
            {
                case MethbarType.Methbar_shield:
                    return new MixShieldMBar(actor);
                case MethbarType.Methbar_yuansuyinji:
                    return new ElementTag(actor);
                default:
                    Logger.LogErrorFormat("使用了未实现的机制条类型，请添加:{0}", type);
                    break;
            }

            return null;
        }

        private BeActor m_Actor;

        protected MechanismBar(BeActor actor)
        {
            m_Actor = actor;
        }

        public abstract void SetMaxValue(int v);
        public abstract void SetCurValue(int v);
        public abstract void SetActive(bool isShow);
        public abstract void Destroy();

        protected bool IsAttachActor()
        {
            return m_Actor != null;
        }

        protected BeActor GetAttachActor()
        {
            return m_Actor;
        }
    }
}
#endif
