
using UnityEngine;

namespace Tenmove.Runtime.Unity
{
    [DisallowMultipleComponent]
    [AddComponentMenu("Tenmove/GameProfileModule")]
    public class TMGameProfileModuleComponent : TMModuleComponent
    {
        [SerializeField]
        private bool m_EnableProfiler;

        private ITMUnityGameProfileClient m_Simulator;

        private bool m_Display;
        private string m_IPAddress;

        protected sealed override void Awake()
        {
            base.Awake();
            m_Simulator = ModuleManager.GetModule<ITMUnityGameProfileClient>();
            if (null == m_Simulator)
            {
                Debugger.LogError("Simulator is invalid!");
                return;
            }
        }

        private void OnDestroy()
        {
            if (null != m_Simulator)
                m_Simulator.EndConnect();
        }

        private void OnGUI()
        {
            if (!m_EnableProfiler)
                return;

            Rect rect = new Rect(0, Screen.height - Screen.dpi * 0.5f, Screen.width * 0.5f, Screen.dpi * 0.5f);
            GUILayout.BeginArea(rect);

            int originButtonFontSize = GUI.skin.button.fontSize;
            GUI.skin.button.fontSize = 28;

            GUILayout.BeginHorizontal();
            string text = m_Display ? "隐藏配置◀" : "显示配置▶";
            if (GUILayout.Button(text, GUILayout.Width(Screen.dpi * 1.6f), GUILayout.Height(Screen.dpi * 0.5f)))
                m_Display = !m_Display;
            GUI.skin.button.fontSize = originButtonFontSize;

            if (m_Display)
                _DisplayConnectConfig();

            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }

        private void _DisplayConnectConfig()
        {
            int originButtonFontSize = GUI.skin.button.fontSize;
            GUI.skin.button.fontSize = 28;

            int originTextFeildFontSize = GUI.skin.textField.fontSize;
            TextAnchor originTextFeildAlignment = GUI.skin.textField.alignment;
            GUI.skin.textField.fontSize = 28;
            GUI.skin.textField.alignment = TextAnchor.MiddleLeft;

            int originBoxFontSize = GUI.skin.box.fontSize;
            TextAnchor originBoxFontAlignment = GUI.skin.box.alignment;
            GUI.skin.box.fontSize = 28;
            GUI.skin.box.alignment = TextAnchor.MiddleCenter;

            GUILayout.Box("请输入控制端IP：", GUILayout.Width(Screen.dpi * 3), GUILayout.Height(Screen.dpi * 0.5f));
            m_IPAddress = GUILayout.TextField(m_IPAddress, GUILayout.Height(Screen.dpi * 0.5f));

            NetIPAddress netIPAddress = NetIPAddress.InvalidAddress;
            if (NetIPAddress.IsValidIPPattern(m_IPAddress))
                netIPAddress = new NetIPAddress(m_IPAddress);

            bool orginDisable = GUI.enabled;
            GUI.enabled = NetIPAddress.InvalidAddress != netIPAddress;

            if (GUILayout.Button("链接", GUILayout.Width(Screen.dpi * 1.5f), GUILayout.Height(Screen.dpi * 0.5f)))
            {
                if (null != m_Simulator)
                    m_Simulator.EndConnect();

                m_Simulator.BeginConnect(netIPAddress, 9527);
            }

            GUI.enabled = orginDisable;
            GUI.skin.button.fontSize = originButtonFontSize;
            GUI.skin.textField.fontSize = originTextFeildFontSize;
            GUI.skin.textField.alignment = originTextFeildAlignment;
            GUI.skin.box.fontSize = originBoxFontSize;
            GUI.skin.box.alignment = originBoxFontAlignment;
        }
    }
}