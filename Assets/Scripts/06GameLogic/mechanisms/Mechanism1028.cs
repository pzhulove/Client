using System.Collections.Generic;

/// <summary>
/// 碰撞到场景边缘时 根据边缘的位置 进行反弹
/// </summary>
public class Mechanism1028 : BeMechanism
{
    public Mechanism1028(int mid, int lv) : base(mid, lv) { }

    private bool xReboundFlag = false;  //X轴碰撞反弹
    private bool yReboundFlag = false;  //y轴碰撞反弹
    private bool zSpeedReverseFlag = false;  //z轴速度反转

    public override void OnInit()
    {
        base.OnInit();
        xReboundFlag = TableManager.GetValueFromUnionCell(data.ValueA[0], level) == 0 ? false : true;
        yReboundFlag = TableManager.GetValueFromUnionCell(data.ValueB[0], level) == 0 ? false : true;
        zSpeedReverseFlag = TableManager.GetValueFromUnionCell(data.ValueC[0], level) == 0 ? false : true;
    }

    public override void OnStart()
    {
        base.OnStart();
        RegisterTouchBoundary();
    }

    /// <summary>
    /// 监听接触到场景边界
    /// </summary>
    private void RegisterTouchBoundary()
    {
        handleA = owner.RegisterEventNew(BeEventType.onXInBlock, (args) =>
        {
            XRebound();
        });

        handleB = owner.RegisterEventNew(BeEventType.onYInBlock, (args) =>
        {
            YRebound();
        });
    }

    /// <summary>
    /// X轴反弹
    /// </summary>
    private void XRebound()
    {
        if (!xReboundFlag)
            return;
        //X轴速度反转
        owner.SetMoveSpeedX(-owner.moveXSpeed);
        if (zSpeedReverseFlag)
        {
            owner.SetMoveSpeedZ(-owner.moveZSpeed);
        }
    }

    /// <summary>
    /// Y轴反弹
    /// </summary>
    private void YRebound()
    {
        if (!yReboundFlag)
            return;
        //Y轴速度反转
        owner.SetMoveSpeedY(-owner.moveYSpeed);
        if (zSpeedReverseFlag)
        {
            owner.SetMoveSpeedZ(-owner.moveZSpeed);
        }
    }
}
