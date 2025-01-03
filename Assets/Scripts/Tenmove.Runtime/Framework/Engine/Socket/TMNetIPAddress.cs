

namespace Tenmove.Runtime
{
    using System;

    public struct NetIPAddress : IEquatable<NetIPAddress>
    {
        public static readonly NetIPAddress InvalidAddress;
        private static readonly byte[] InvalidValue;
        private readonly byte[] m_Value;
        
        static NetIPAddress()
        {
            InvalidValue = new byte[] { 0, 0, 0, 0 };
            InvalidAddress = new NetIPAddress(InvalidValue);
        }

        public NetIPAddress(byte[] value)
        {
            if (null != value && 4 == value.Length)
            {
                m_Value = new byte[4];
                for (int i = 0, icnt = m_Value.Length; i < icnt; ++i)
                    m_Value[i] = value[i];
                return;
            }
            else
                Debugger.LogWarning("Parameter 'value' is not a valid ip address pattern!");

            m_Value = InvalidValue;
        }

        public string Pattern
        {
            get { return ToString(); }
        }

        public byte[] Value
        {
            get { return new byte[4]{ m_Value[0], m_Value[1], m_Value[2], m_Value[3]}; }
        }

        public NetIPAddress(byte first,byte second,byte third,byte forth)
        {
            m_Value = new byte[4];
            m_Value[0] = first;
            m_Value[1] = second;
            m_Value[2] = third;
            m_Value[3] = forth;
        }

        public NetIPAddress(string pattern)
        {
            m_Value = _Parse(pattern);
        }

        public bool Equals(NetIPAddress other)
        {
            for(int i = 0,icnt = m_Value.Length;i<icnt;++i)
            {
                if (m_Value[i] != other.m_Value[i])
                    return false;
            }

            return true;
        }

        public static bool IsValidIPPattern(string pattern)
        {
            if (!string.IsNullOrEmpty(pattern))
            {
                string[] sub = pattern.Split('.');
                if (null != sub && 4 == sub.Length)
                { 
                    for (int i = 0, icnt = sub.Length; i < icnt; ++i)
                    {
                        int outValue = 0;
                        if (!int.TryParse(sub[i], out outValue))
                            return false;
                        else
                        {
                            if (outValue != (outValue & 0xff))
                                return false;
                        }
                    }

                    return true;
                }
            }

            return false;
        }

        public override bool Equals(object other)
        {
            return Equals((NetIPAddress)other);
        }

        public override int GetHashCode()
        {
            return m_Value.GetHashCode();
        }

        static public bool operator ==(NetIPAddress left, NetIPAddress right)
        {
            return left.Equals(right);
        }

        static public bool operator !=(NetIPAddress left, NetIPAddress right)
        {
            return !left.Equals(right);
        }

        public override string ToString()
        {
            return string.Format("{0}.{1}.{2}.{3}", m_Value[0], m_Value[1], m_Value[2], m_Value[3]);
        }

        static private byte[] _Parse(string pattern)
        {
            byte[] res = new byte[4] { 0, 0, 0, 0 };
            if (!string.IsNullOrEmpty(pattern))
            {
                string[] sub = pattern.Split('.');
                if(null != sub && 4 == sub.Length)
                {
                    for(int i = 0,icnt = sub.Length;i<icnt;++i)
                    {
                        int outValue = 0;
                        if (!int.TryParse(sub[i], out outValue))
                        {
                            Debugger.LogWarning("Parameter 'pattern:[value:{0}]' is not a valid ip address pattern!", pattern);
                            return res;
                        }
                        else
                            res[i] = (byte)outValue;
                    }

                    return res;
                }
                else
                    Debugger.LogWarning("Parameter 'pattern:[value:{0}]' is not a valid ip address pattern!", pattern);
            }
            else
                Debugger.LogWarning("Parameter 'pattern' can not be null or empty string!");

            return res;
        }
    }
}