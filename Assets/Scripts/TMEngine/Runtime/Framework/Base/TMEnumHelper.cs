
namespace Tenmove.Runtime
{
    /// <summary>
    /// 千万不要用readonly修饰该类
    /// </summary>
    /// <typeparam name="E"></typeparam>
    public struct EnumHelper<E>
    {
        private int m_Flags;

        public EnumHelper(E flag)
        {
            m_Flags = flag.GetHashCode();
        }

        public EnumHelper(uint flag)
        {
            m_Flags = (int)flag;
        }

        public EnumHelper(EnumHelper<E> flag)
        {
            m_Flags = flag.m_Flags;
        }

        public bool HasFlag(E flag)
        {
            return 0 != (flag.GetHashCode() & m_Flags);
        }

        public bool HasFlag(int flag)
        {
            return 0 != (flag & m_Flags);
        }

        public void AddFlag(E flag)
        {
            m_Flags |= flag.GetHashCode();
        }

        public void AddFlag(EnumHelper<E> flag)
        {
            m_Flags |= flag.m_Flags;
        }

        public void RemoveFlag(E flag)
        {
            m_Flags &= ~(flag.GetHashCode());
        }

        public void RemoveFlag(EnumHelper<E> flag)
        {
            m_Flags &= ~flag.m_Flags;
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return m_Flags.GetHashCode();
        }

        public static EnumHelper<E> operator + (EnumHelper<E> _left,E _right)
        {
            EnumHelper< E > newFlag = new EnumHelper<E>(_left);
            newFlag.AddFlag(_right);
            return newFlag;
        }

        public static EnumHelper<E> operator +(E _left, EnumHelper<E> _right)
        {
            EnumHelper<E> newFlag = new EnumHelper<E>((uint)_left.GetHashCode());
            newFlag.AddFlag(_right);
            return newFlag;
        }

        public static EnumHelper<E> operator +(EnumHelper<E> _left, EnumHelper<E> _right)
        {
            EnumHelper<E> newFlag = new EnumHelper<E>(_left);
            newFlag.AddFlag(_right);
            return newFlag;
        }

        public static EnumHelper<E> operator -(EnumHelper<E> _left, E _right)
        {
            EnumHelper<E> newFlag = new EnumHelper<E>(_left);
            newFlag.RemoveFlag(_right);
            return newFlag;
        }

        public static EnumHelper<E> operator -(E _left, EnumHelper<E> _right)
        {
            EnumHelper<E> newFlag = new EnumHelper<E>((uint)_left.GetHashCode());
            newFlag.RemoveFlag(_right);
            return newFlag;
        }

        public static EnumHelper<E> operator -(EnumHelper<E> _left, EnumHelper<E> _right)
        {
            EnumHelper<E> newFlag = new EnumHelper<E>(_left);
            newFlag.RemoveFlag(_right);
            return newFlag;
        }

        public static bool operator == (EnumHelper<E> _left, EnumHelper<E> _right)
        {
            return _left.m_Flags == _right.m_Flags;
        }

        public static bool operator !=(EnumHelper<E> _left, EnumHelper<E> _right)
        {
            return _left.m_Flags != _right.m_Flags;
        }

        public static bool operator ==(EnumHelper<E> _left, E _right)
        {
            return _left.m_Flags == _right.GetHashCode();
        }

        public static bool operator !=(EnumHelper<E> _left, E _right)
        {
            return _left.m_Flags != _right.GetHashCode();
        }

        public static bool operator ==(EnumHelper<E> _left, int _right)
        {
            return _left.m_Flags == _right;
        }

        public static bool operator !=(EnumHelper<E> _left, int _right)
        {
            return _left.m_Flags != _right;
        }

        public static implicit operator EnumHelper<E>(uint value)
        {
            return new EnumHelper<E>(value); 
        }

        public static implicit operator uint(EnumHelper<E> flags)
        {
            return (uint)flags.m_Flags;
        }

        public static implicit operator int(EnumHelper<E> flags)
        {
            return (int)flags.m_Flags;
        }
    }

}

