using System;
using UnityEngine;

/// <summary>
/// 绘制标签
/// 如果没有添加该标签，默认是不会绘制编辑字段
/// </summary>
public class MultiPropertyGUIAttribute : Attribute
{
    // 绘制的描述文字
    public string description;
    // 绘制的序号，影响绘制顺序
    public int order;
    // 能否编辑
    public bool canEdit;
    // 是否绘制
    public bool needDraw;

    public Type type;

    public MultiPropertyGUIAttribute(bool draw)
    {
        description = "";
        canEdit = false;
        order = 0;
        needDraw = draw;

    }

    public MultiPropertyGUIAttribute(string description, int order, Type type)
    {
        this.description = description;
        this.canEdit = true;
        needDraw = true;
        this.order = order;
        this.type = type;
        
    }
    public MultiPropertyGUIAttribute(string description = "", int order = 1, bool canEdit = true,  bool draw = true)
    {
        this.description = description;
        this.canEdit = canEdit;
        this.order = order;
        needDraw = draw;
    }
}

/// <summary>
/// 文本样式标签
/// </summary>
public class FontStyleAttribute : Attribute
{
    // 字体大小
    public int fontSize;
    // 字体颜色
    public Color textColor;

    public FontStyleAttribute(int _fontSize, float r, float g, float b)
    {
        fontSize = _fontSize;
        textColor = new Color(r, g, b);
    }
}

/// <summary>
/// 条件显示节点，当该条件成立时显示
/// </summary>
public class ConditionDrawAttribute : Attribute
{
    public string name;
    public object value;
    public Option option;
    public enum Option
    {
        Equal,
        UnEqual,
    }

    public ConditionDrawAttribute(string name, object value, Option option)
    {
        this.name = name;
        this.value = value;
        this.option = option;
    }
}
