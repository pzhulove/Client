using UnityEngine;
using System.Collections;
using ProtoTable;

namespace GameClient
{
    public class SwitchFunctionUtility 
    {
        /// <summary>
        /// 判断功能是否开放
        ///
        /// 如果在表格中查找不到，返回true
        /// </summary>
        public static bool IsOpen(int id)
        {
            SwitchClientFunctionTable tb = TableManager.instance.GetTableItem<SwitchClientFunctionTable>(id);

            if (null != tb)
            {
                return tb.Open;
            }

            return true;
        }

        /// <summary>
        /// 受击Shader是否开启
        /// </summary>
        public static bool IsHitShaderOpen
        {
            get
            {
                return IsOpen(201);
            }

        }

        public static bool IsPKOpen
        {
            get
            {
                return IsOpen(200);
            }
        }
    }
}
