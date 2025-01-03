using System;
using UnityEngine;

/// <summary>
/// 枚举操作辅助类
/// </summary>
public class TMEnumHelper
{
    /// <summary>
    /// 传入指定参数获取改变后的枚举值
    /// </summary>
    /// <typeparam name="P">类型枚举</typeparam>
    /// <param name="dsc">下拉列表描述</param>
    /// <param name="p">当前选择值</param>
    /// <returns>返回修改后的值</returns>
    public static P GetChangeEnumType<P>(string dsc, P p,int length = 250)
    {
#if UNITY_EDITOR
        string[] stringArr = typeof(P).GetDescriptions();
        Array valueArr = typeof(P).GetValues();
        int index = Array.BinarySearch(valueArr, p);
        index = UnityEditor.EditorGUILayout.Popup(dsc, index, stringArr, GUILayout.Width(length));
        if (index >= 0)
        {
            return (P)valueArr.GetValue(index);
        }
        return default(P);
#else
        return default(P);;
#endif
    }
}
