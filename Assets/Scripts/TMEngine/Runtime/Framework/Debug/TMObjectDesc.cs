using System;

namespace Tenmove.Runtime
{
    public struct ObjectDesc
    {
        readonly string m_Name;
        readonly bool m_Locked;
        readonly DateTime m_LastUseTime;
        readonly int m_SpawnCount;
        readonly bool m_InUse;

        public ObjectDesc(string name, bool locked, long timeStamp, int spawnCount, bool inUse)
        {
            m_Name = name ?? string.Empty;
            m_Locked = locked;
            m_InUse = inUse;
            m_LastUseTime = Utility.Time.TicksToDateTime(timeStamp);
            m_SpawnCount = spawnCount;
        }


        public string Name
        {
            get { return m_Name; }
        }

        public bool IsLocked
        {
            get { return m_Locked; }
        }

        public bool IsInUse
        {
            get { return m_InUse; }
        }

        public DateTime LastUseTime
        {
            get { return m_LastUseTime; }
        }

        public int SpawnCount
        {
            get { return m_SpawnCount; }
        }

    }
}