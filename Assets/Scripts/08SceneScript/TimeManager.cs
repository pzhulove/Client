using UnityEngine;
using System.Collections;
using Protocol;
using Network;
using System;

namespace GameClient
{
    public class TimeManager : DataManager<TimeManager>
    {
        #region delegate
        #endregion

        float m_fStartTime;
        uint m_nUnixTime;
        double m_dUnixTime;

        public uint GetServerTime()
        {
            return m_nUnixTime + (uint)(Time.realtimeSinceStartup - m_fStartTime);
        }

        public double GetServerDoubleTime()
        {
            return m_dUnixTime + (double)(Time.realtimeSinceStartup - m_fStartTime);
        }

        public string GetTimeT()
        {
            DateTime dateTime = Function.ConvertIntDateTime(GetServerTime());
            return dateTime.ToString("T", System.Globalization.DateTimeFormatInfo.InvariantInfo);
        }

        #region process
        public override void Initialize()
        {

        }

        public override void Clear()
        {

        }

        public override void OnApplicationStart()
        {
            
        }

        public override void OnApplicationQuit()
        {
            
        }

        public void OnRecvGateSyncServerTime(MsgDATA msgData)
        {
            GateSyncServerTime kGateSyncServerTime = new GateSyncServerTime();
            kGateSyncServerTime.decode(msgData.bytes);

            m_fStartTime = Time.realtimeSinceStartup;
            m_nUnixTime = kGateSyncServerTime.time;
            m_dUnixTime = (double)m_nUnixTime;

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ServerTimeChanged);
        }

        #endregion
    }
}