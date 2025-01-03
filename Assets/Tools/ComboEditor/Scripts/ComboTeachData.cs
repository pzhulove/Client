using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
[System.Serializable]
public class ComboData
{
    [SerializeField]
#if UNITY_EDITOR
    [FieldLabel("技能GroupID")]
#endif
    public int skillGroupID;
    [SerializeField]
#if UNITY_EDITOR
    [FieldLabel("技能ID")]
#endif
    public int skillID;
    [SerializeField]
#if UNITY_EDITOR
    [FieldLabel("技能等级")]
#endif
    public int skillLevel;
    [SerializeField]
#if UNITY_EDITOR
    [FieldLabel("技能按钮位置")]
#endif
    public int skillSlot;
    [SerializeField]
#if UNITY_EDITOR
    [FieldLabel("技能播放多久后暂停")]
#endif
    public int skillTime;
    [SerializeField]
#if UNITY_EDITOR
    [FieldLabel("等待多长时间执行下一个")]
#endif
    public int waitInputTime;
    [SerializeField]
#if UNITY_EDITOR
    [FieldLabel("移动方向")]
#endif
    public int moveDir = -1;
    [SerializeField]
#if UNITY_EDITOR
    [FieldLabel("移动时间")]
#endif
    public int moveTime = -1;
    [SerializeField]
#if UNITY_EDITOR
    [FieldLabel("移动停止idle时间")]
#endif
    public int idleTime = -1;
    [SerializeField]
#if UNITY_EDITOR
    [FieldLabel("是否顯示")]
#endif
    public int showUI = 1;
    [SerializeField]
#if UNITY_EDITOR
    [FieldLabel("面向")]
#endif
    public int faceRight = 1;
    [SerializeField]
#if UNITY_EDITOR
    [FieldLabel("combo技能的第一个ID")]
#endif
    public int sourceID = 0;
    [SerializeField]
#if UNITY_EDITOR
    [FieldLabel("需要命中的是第几")]
#endif
    public int phase = 0;
}

public class ComboTeachData : ScriptableObject {

    public string dataName;
    public int roomID;
    public bool Advanced;
    public string buffID;
    public Vector3 playerPos = new Vector3();
    public Vector3 monsterPos = new Vector3();
    public ComboData[] datas = new ComboData[0] { };
}
#if UNITY_EDITOR
/// <summary>
/// 能让字段在inspect面板显示中文字符
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
public class FieldLabelAttribute : PropertyAttribute
{
    public string label;//要显示的字符
    public FieldLabelAttribute(string label)
    {
        this.label = label;
        //获取你想要绘制的字段（比如"技能"）
    }

}
[CustomPropertyDrawer(typeof(FieldLabelAttribute))]
public class FieldLabelDrawer : PropertyDrawer
{
    private FieldLabelAttribute FLAttribute
    {
        get { return (FieldLabelAttribute)attribute; }
        ////获取你想要绘制的字段
    }
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        //在这里重新绘制
        EditorGUI.PropertyField(position, property, new GUIContent(FLAttribute.label), true);

    }
}
#endif