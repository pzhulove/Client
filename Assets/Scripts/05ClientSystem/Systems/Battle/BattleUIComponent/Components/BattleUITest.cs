using UnityEngine;
using Network;
using UnityEngine.UI;

namespace GameClient
{
    /// <summary>
    /// Pvp相关UI
    /// </summary>
    public class BattleUITest : BattleUIBase
    {
        public BattleUITest() : base() { }

        protected override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/BattleUI/BattleUIComponent/BattleUITest";
        }

        private Text pingText;
        private Text mRobotInfo = null;
        private Button mChasingSwitch = null;

        protected float time = 0;
        protected override void _bindExUI()
        {
            mRobotInfo = mBind.GetCom<Text>("RobotInfo");
            if (!mRobotInfo.IsNull())
            {
                mRobotInfo.CustomActive(true);
            }
            mChasingSwitch = mBind.GetCom<Button>("ChasingSwitch");
            if (mChasingSwitch != null)
            {
                mChasingSwitch.gameObject.CustomActive(true);
                mChasingSwitch.onClick.AddListener(_onChasingModeSwitch);
            }
        }

        protected override void _unbindExUI()
        {
            mRobotInfo = null;
            if (mChasingSwitch != null)
            {
                mChasingSwitch.onClick.RemoveListener(_onChasingModeSwitch);
            }
            mChasingSwitch = null;
        }
        private void _onChasingModeSwitch()
        {
            if (FrameSync.instance.IsInChasingMode)
            {
                SystemNotifyManager.SysNotifyFloatingEffect("结束追帧");
                FrameSync.instance.EndChasingMode();
            }
            else
            {
                SystemNotifyManager.SysNotifyFloatingEffect("开始追帧");
                FrameSync.instance.StartInChasingMode();
            }
        }

        private void _onUpdatePing()
        {
            float delta = Time.realtimeSinceStartup - time;

            if (delta > 1.0f)
            {
                int iPing = NetManager.instance.GetPingToRelayServer();
                string strPing = "";
                string strPingNum = "" + iPing + "ms";
                if (iPing < 100)
                {
                    strPing = strPingNum;
                }
                else if (iPing < 200)
                {
                    strPing = "<color=yellow>" + strPingNum + "</color>";
                }
                else
                {
                    strPing = "<color=red>" + strPingNum + "</color>";
                }

                if (pingText != null)
                {
#if UNITY_EDITOR
                    if (FrameSync.instance != null)
                    {
                        strPing += string.Format(" ExecCmd: {0} RecvCmd: {1}\n UdpRecv: {2} JetterDelay: {3}\n Drift: {4}", FrameSync.instance.execCmdPerf.recentDelay,
                                        FrameSync.instance.recvCmdPerf.recentDelay, UdpClient.perf.recentDelay, FrameSync.instance.avgFrameDelay, Global.Settings.drift);
                    }
                    pingText.text = "Ping: " + strPing + " FPS: " + ComponentFPS.instance.GetFPS();
#endif
                }

#if UNITY_EDITOR
                var execCmdPerf = FrameSync.instance.execCmdPerf;
                var recvCmdPerf = FrameSync.instance.recvCmdPerf;
                string framePerfText1 = string.Format("Exec: Recent:{0} Average:{1} Max:{2}\n", execCmdPerf.recentDelay, execCmdPerf.averageDelay, execCmdPerf.maxDelay);
                string framePerfText2 = string.Format("Recv: Recent:{0} Average:{1} Max:{2}\n", recvCmdPerf.recentDelay, recvCmdPerf.averageDelay, recvCmdPerf.maxDelay);
                string framePerfText3 = string.Format("Udp: Recent:{0} Average:{1} Max:{2}\n", UdpClient.perf.recentDelay, UdpClient.perf.averageDelay, UdpClient.perf.maxDelay);
                if (pingText != null)
                    pingText.text = pingText.text + "\n" + framePerfText1 + framePerfText2 + framePerfText3;
#endif
                if (BattleMain.battleType == BattleType.ScufflePVP)
                {
                    GameStatisticManager.instance.dataStatistics.CollectPingStatistic(iPing);
                    GameStatisticManager.instance.dataStatistics.CollectFpsStatistic(ComponentFPS.instance.GetFPS());
                }
                time = Time.realtimeSinceStartup;
            }
        }



#if ROBOT_TEST
        //float m_robotTimeStamp = 0;
        //protected void UpdateDungeonRobot()
        //{
        //    mRobotInfo.text = FrameSync.instance.GetChasingOutputStr();
       //     return;   
        //    if (mRobotInfo.IsNull())return;
        //    var curTime = Time.realtimeSinceStartup;
        //    float durTime = curTime - m_robotTimeStamp;
        //    if(durTime > 1)
        //    {
        //        m_robotTimeStamp = curTime;
        //        int ping = NetManager.instance.GetPingToRelayServer();
        //        float memconsume = GC.GetTotalMemory(false) / (1024.0f * 1024);
        //        if (memconsume > AutoFightTestDataManager.instance.MaxMemoryInBattle)
        //        {
        //            AutoFightTestDataManager.instance.MaxMemoryInBattle = memconsume;
        //        }
        //        var execCmdPerf = FrameSync.instance.execCmdPerf;
        //        var recvCmdPerf = FrameSync.instance.recvCmdPerf;
        //        string framePerfText1 = BeUtility.Format("Exec: Recent:{0} Average:{1} Max:{2}\n", execCmdPerf.recentDelay, execCmdPerf.averageDelay, execCmdPerf.maxDelay);
        //        string framePerfText2 = BeUtility.Format("Recv: Recent:{0} Average:{1} Max:{2}\n", recvCmdPerf.recentDelay, recvCmdPerf.averageDelay, recvCmdPerf.maxDelay);
        //        string framePerfText3 = BeUtility.Format("Udp: Recent:{0} Average:{1} Max:{2}\n", UdpClient.perf.recentDelay, UdpClient.perf.averageDelay, UdpClient.perf.maxDelay);
        //        string info = BeUtility.Format("Ping :{0} Mem :{1}M\n", ping, memconsume.ToString("0.0"));
        //        mRobotInfo.text = BeUtility.Format("{0}{1}{2}{3}", info, framePerfText1, framePerfText2, framePerfText3);
        //    }
        //}
#endif
    }
}