#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Xml;
using behaviac;
using UnityEngine;
using Color = System.Drawing.Color;
using Debug = UnityEngine.Debug;

namespace BehaviorTreeMechanism
{
    public class BehaviorTreeLogInfo
    {
        public BTMLogDesc content;
        public StackFrame[] frames;
        public List<string> localVars;

        public BehaviorTreeLogInfo(BTMLogDesc log, List<string> vars, StackTrace stack)
        {
            content = log;
            frames = stack == null ? new StackTrace(true).GetFrames() : stack.GetFrames();
            localVars = vars;
        }
    }

    public class ActorBTMLogInfo
    {
        public BeActor actor;
        public Dictionary<int, BTMLogInfo> info = new Dictionary<int, BTMLogInfo>();
    }

    public class BTMLogInfo
    {
        public int mechanismId;
        public BeMechanism mechanism;
        public List<BehaviorTreeLogInfo> info = new List<BehaviorTreeLogInfo>();
    }

    public enum BTMLogType
    {
        Error,
        Event,
        Mechanism
    }

    public class BTMLogDesc
    {
        public DateTime time;
        public BTMLogType type;
        public string from;
        public behaviac.BehaviorNode node;
        public behaviac.BehaviorNode attach;
        public string log;

        public BTMLogDesc(BTMLogType _type, string _from, behaviac.BehaviorNode _node, behaviac.BehaviorNode _attach,
            string _log)
        {
            time = DateTime.Now;
            from = _from;
            type = _type;
            node = _node;
            attach = _attach;
            log = _log;
        }

        public override string ToString()
        {
            string fromStr = from;
            if (type == BTMLogType.Event)
                fromStr = string.Format("<color=yellow>{0}</color>", from);
            else if (type == BTMLogType.Mechanism)
                fromStr = string.Format("<color=green>{0}</color>", from);

            string nodeStr = string.Format("{0}:{1}", node.GetDisplayName(), node.GetId());
            if (attach != null)
                nodeStr += "-" + attach.GetId();
            if (BehaviorTreeLogData.m_NodeColorDic.TryGetValue(node.GetClassNameString(), out var color))
            {
                if (attach != null)
                    nodeStr = string.Format("<color=#{0}>{1}:{2}-{3}</color>", color, node.GetDisplayName(),
                        node.GetId(), attach.GetId());
                else
                    nodeStr = string.Format("<color=#{0}>{1}:{2}</color>", color, node.GetDisplayName(), node.GetId());
            }

            string printStr = string.Format("[{0}]\t\t[{1}]:{2}", nodeStr, fromStr, log);
            if (type == BTMLogType.Error)
                printStr = string.Format("<color=red>{0}</color>", printStr);
            return printStr;
        }

        public string[] ToStringArray()
        {
            string fromStr = from;
            if (type == BTMLogType.Event)
                fromStr = string.Format("<color=yellow>{0}</color>", from);
            else if (type == BTMLogType.Mechanism)
                fromStr = string.Format("<color=green>{0}</color>", from);

            string nodeStr = string.Format("{0}:{1}", node.GetDisplayName(), node.GetId());
            if (attach != null)
                nodeStr += "-" + attach.GetId();
            if (BehaviorTreeLogData.m_NodeColorDic.TryGetValue(node.GetClassNameString(), out var color))
            {
                if (attach != null)
                    nodeStr = string.Format("<color=#{0}>{1}:{2}-{3}</color>", color, node.GetDisplayName(),
                        node.GetId(), attach.GetId());
                else
                    nodeStr = string.Format("<color=#{0}>{1}:{2}</color>", color, node.GetDisplayName(), node.GetId());
            }

            string printStr = string.Format("[{0}]\t\t[{1}]:{2}", nodeStr, fromStr, log);
            if (type == BTMLogType.Error)
            {
                nodeStr = string.Format("<color=red>{0}</color>", nodeStr);
                fromStr = string.Format("<color=red>{0}</color>", fromStr);
                log = string.Format("<color=red>{0}</color>", log);
            }

            return new string[] {nodeStr, fromStr, log};
        }
    }

    public class BehaviorTreeLogData
    {
        public static BehaviorTreeLogData Instance = new BehaviorTreeLogData();

        // 机制行为树，日志数据
        private Dictionary<int, ActorBTMLogInfo> m_BTMLogMap = new Dictionary<int, ActorBTMLogInfo>();

        public void Init()
        {
            LoadNodeColor();
        }

        public Dictionary<int, ActorBTMLogInfo> GetData()
        {
            return m_BTMLogMap;
        }

        public void AddBTMLog(BeActor actor, BeMechanism mechanism, BTMLogDesc log, List<string> localVars,
            StackTrace stack = null)
        {
            if (!m_BTMLogMap.TryGetValue(actor.GetPID(), out var actorLogs))
            {
                actorLogs = new ActorBTMLogInfo();
                actorLogs.actor = actor;
                m_BTMLogMap.Add(actor.GetPID(), actorLogs);
            }

            if (!actorLogs.info.TryGetValue(mechanism.mechianismID, out var logs))
            {
                logs = new BTMLogInfo();
                logs.mechanismId = mechanism.mechianismID;
                logs.mechanism = mechanism;
                actorLogs.info.Add(mechanism.mechianismID, logs);
            }

            if (logs.info.Count < MaxLogCount)
                logs.info.Add(new BehaviorTreeLogInfo(log, localVars, stack));
        }

        public static int MaxLogCount = 10000;
        public static bool IsOpenProcessDebug = false;

        public void Clear()
        {
            m_BTMLogMap.Clear();
        }

        public static Dictionary<string, string> m_NodeColorDic;

        private void LoadNodeColor()
        {
#if !LOGIC_SERVER
            m_NodeColorDic = new Dictionary<string, string>();
            XmlDocument doc = new XmlDocument();
            XmlReaderSettings setting = new XmlReaderSettings();
            setting.IgnoreComments = true;
            string colorConfig = Application.dataPath +
                                 "/../../ExternalTool/BehaviacDesigner/behaviac/tools/designer/out/config.xml";
            if (!File.Exists(colorConfig))
                return;

            XmlReader reader = XmlReader.Create(colorConfig, setting);
            doc.Load(reader);
            var xn = doc.SelectSingleNode("Configuration");
            var themes = xn.SelectSingleNode("Themes");
            foreach (XmlNode i in themes.ChildNodes)
            {
                if (i.Name == "theme" && i.Attributes["Name"].Value == "Modern")
                {
                    foreach (var modernChildNode in i.ChildNodes)
                    {
                        XmlElement node = (XmlElement) modernChildNode;
                        string[] classes = node.Attributes["FullName"].Value.Split('.');
                        string nodeName = classes[classes.Length - 1];
                        string brushRGB = node.Attributes["Color"].Value;
                        string[] colors = brushRGB.Split(',');
                        if (colors.Length == 3)
                        {
                            int r = int.Parse(colors[0].Trim());
                            int g = int.Parse(colors[1].Trim());
                            int b = int.Parse(colors[2].Trim());
                            string str = r.ToString("x2") + g.ToString("x2") + b.ToString("x2");
                            if (!m_NodeColorDic.ContainsKey(nodeName))
                                m_NodeColorDic.Add(nodeName, str);
                        }
                    }
                }
            }
#endif
        }
    }
}
#endif
