using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 根据当前模型动作创建残影机制
/// </summary>
public class Mechanism78 : BeMechanism
{
    private float _time;
    private Color32 _color = new Color32(255,255,255,255);

    public Mechanism78(int sid, int skillLevel) : base(sid, skillLevel) { }

    public override void OnInit()
    {
        _time = TableManager.GetValueFromUnionCell(data.ValueA[0], level) / 1000f;
        _color.r = (byte)TableManager.GetValueFromUnionCell(data.ValueB[0], level);
        _color.g = (byte)TableManager.GetValueFromUnionCell(data.ValueB[1], level);
        _color.b = (byte)TableManager.GetValueFromUnionCell(data.ValueB[2], level);
    }

    public override void OnStart()
    {
        owner.m_pkGeActor.CreateSnapshot(_color, _time);
    }
    
}
