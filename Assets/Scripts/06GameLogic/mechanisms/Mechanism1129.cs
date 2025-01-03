using System;
using System.Collections.Generic;
using GameClient;
using UnityEngine;

/// <summary>
/// 将Owner的模型移动到屏幕中心点对应的场景坐标点 并且将模型进行缩放
/// </summary>
public class Mechanism1129 : BeMechanism
{
    public Mechanism1129(int mid, int lv) : base(mid, lv)
    {
    }

#if !LOGIC_SERVER
    private Vector3 _addModelScale = Vector3.zero; //增加的模型缩放比 
    private float _zPosOffset = 5;    //Y轴坐标偏移

    public override void OnInit()
    {
        base.OnInit();
        _addModelScale.x = TableManager.GetValueFromUnionCell(data.ValueA[0], level) / 1000.0f;
        _addModelScale.y = TableManager.GetValueFromUnionCell(data.ValueA[0], level) / 1000.0f;
        _addModelScale.z = TableManager.GetValueFromUnionCell(data.ValueA[0], level) / 1000.0f;
        
        _zPosOffset = TableManager.GetValueFromUnionCell(data.ValueB[0], level) / 1000.0f;
    }

    public override void OnStart()
    {
        base.OnStart();
        owner.StopShock();
        _InitRegisterEvent();
        if (owner.isLocalActor)
        {
            SetModelScale();
            SetModelPos();
            SetCamera();
        }
    }

    private void _InitRegisterEvent()
    {
        handleNewA = owner.RegisterEventNew(BeEventType.onChangeShock, _OnChangeShock);
    }

    private void _OnChangeShock(BeEvent.BeEventParam param)
    {
        param.m_Bool = false;
    }

    public override void OnFinish()
    {
        base.OnFinish();
        if (owner.isLocalActor)
        {
            SetModelScale(true);
            SetModelPos(true);
            SetCamera(true);
        }
    }

    /// <summary>
    /// 设置模型缩放比
    /// </summary>
    private void SetModelScale(bool isRestore = false)
    {
        if (owner.m_pkGeActor == null)
            return;
        var curScale = owner.m_pkGeActor.GetActorNodeScale();
        if (!isRestore)
            owner.m_pkGeActor.SetActorNodeScale1(curScale + _addModelScale);
        else
            owner.m_pkGeActor.SetActorNodeScale1(curScale - _addModelScale);
    }

    /// <summary>
    /// 设置模型的位置
    /// </summary>
    private void SetModelPos(bool isRestore = false)
    {
        if (owner.m_pkGeActor == null)
            return;
        if (owner.m_pkGeActor.goFootInfo != null)
            owner.m_pkGeActor.goFootInfo.CustomActive(isRestore);
        if (owner.m_pkGeActor.goInfoBar != null)
            owner.m_pkGeActor.goInfoBar.CustomActive(isRestore);
        owner.m_pkGeActor.SetHeadInfoVisible(isRestore);

        if (!isRestore)
        {
            var setActorPos = Vector3.zero;
            var curCharactorPos = owner.m_pkGeActor.GetPosition();
            Vector3 targetPos = GetBattlePosBySceneCenterPos();

            //需要减去当前的位置坐标  才能得到绝对的中心坐标
            setActorPos.x = owner.GetFace() ? curCharactorPos.x - targetPos.x : targetPos.x - curCharactorPos.x;
            setActorPos.y = curCharactorPos.y;
            setActorPos.z = _zPosOffset - curCharactorPos.z;

            owner.m_pkGeActor.SetActorPosition(setActorPos);
        }
        else
            owner.m_pkGeActor.SetActorPosition(Vector3.zero);
    }

    private void SetCamera(bool isRestore = false)
    {
        if (owner.CurrentBeScene == null || owner.CurrentBeScene.currentGeScene == null ||
            owner.CurrentBeScene.currentGeScene.GetCamera() == null)
            return;
        var cameraCtrl = owner.CurrentBeScene.currentGeScene.GetCamera().GetController();
        if (cameraCtrl == null)
            return;
        cameraCtrl.SetPause(!isRestore);
        var pos = cameraCtrl.GetCameraPosition();
        pos.z = -5;
        cameraCtrl.SetCameraPosition(pos);
    }

    /// <summary>
    /// 通过屏幕中心点坐标  获取对应的场景坐标
    /// </summary>
    private Vector3 GetBattlePosBySceneCenterPos()
    {
        if (ClientSystemManager.instance == null || ClientSystemManager.instance.UICamera == null)
            return Vector3.zero;
        var screenPos = ClientSystemManager.instance.UICamera.WorldToScreenPoint(Vector3.zero);
        var battleScenePos = Camera.main.ScreenToWorldPoint(screenPos);
        return battleScenePos;
    }
#endif
}