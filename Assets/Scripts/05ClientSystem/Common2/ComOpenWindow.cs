using UnityEngine;
using System.Collections;

namespace GameClient
{
    class ComOpenWindow : MonoBehaviour
    {
        public string typeName;

        public void Open()
        {
            var type = typeof(GameClient.ClientFrame).Assembly.GetType(typeName);
            if(null == type)
            {
                return;
            }

            var methodInfo = type.GetMethod("CommandOpen");
            if(null == methodInfo)
            {
                return;
            }

            methodInfo.Invoke(null,new object[] { null });

            if (typeName == "GameClient.LegendFrame")
            {
                GameStatisticManager.GetInstance().DoStartUIButton("Legend");
            }
            else if (typeName == "GameClient.ActiveGroupMainFrame")
            {
                GameStatisticManager.GetInstance().DoStartUIButton("Achievement_1");
            }
            
        }
    }
}