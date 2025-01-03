using System;
using System.Collections.Generic;
using GameClient;
using UnityEngine;

/// <summary>
/// 将当前的正交相机改成透视相机
/// </summary>
public class Mechanism1128 : BeMechanism
{
    public Mechanism1128(int mid, int lv) : base(mid, lv)
    {
    }

#if !LOGIC_SERVER
    private GeCameraDesc _cameraDesc;
    private GeCameraDesc _cameraDescBackUp;

    private Vector3 _cameraControlOffsetBackUp = Vector3.zero;
    
    private int _cameraCullingMask = 0;
    private int _3DUIModel = 14;
    private int _actorNodeLayerBackUp = 0;

    public override void OnInit()
    {
        base.OnInit();
        _cameraDesc.IsOrthographic = false;
        _cameraDesc.FOV = TableManager.GetValueFromUnionCell(data.ValueA[0], level) / 1000.0f;
        _cameraDesc.NearPlane = TableManager.GetValueFromUnionCell(data.ValueB[0], level) / 1000.0f;
        _cameraDesc.FarPlane = TableManager.GetValueFromUnionCell(data.ValueC[0], level) / 1000.0f;
    }

    public override void OnStart()
    {
        base.OnStart();
        if (owner.isLocalActor)
        {
            RecordCameraDesc();
            RecordCameraControlYOffset();
            SetCameraControlOffset();
            SetCameraDesc(_cameraDesc);
            SetCameraCullingMask((1 << _3DUIModel));
            RecordActorNodeLayer(); 
            SetOwnerModelLayer(_3DUIModel);
        }
        RegisterBattleeExit();
    }

    public override void OnFinish()
    {
        base.OnFinish();
        ResetData();
    }

    private void RegisterBattleeExit()
    {
        if (owner.CurrentBeScene != null)
        {
            SceneHandleNewA = owner.CurrentBeScene.RegisterEventNew(BeEventSceneType.onBattleExit, OnBattleExit);
        }
    }

    /// <summary>
    /// 监听战斗退出事件
    /// </summary>
    /// <param name="param"></param>
    private void OnBattleExit(BeEvent.BeEventParam param)
    {
        ResetData();
    }

    private void ResetData()
    {
        if (!owner.isLocalActor)
            return;
        SetCameraControlOffset(true);
        SetCameraDesc(_cameraDescBackUp);
        SetCameraCullingMask(_cameraCullingMask);
        SetOwnerModelLayer(_actorNodeLayerBackUp);
    }

    private void RecordCameraDesc()
    {
        if (owner.CurrentBeScene == null)
            return;
        if (owner.CurrentBeScene.currentGeScene == null)
            return;
        if (owner.CurrentBeScene.currentGeScene.GetCamera() == null)
            return;
        _cameraDescBackUp = owner.CurrentBeScene.currentGeScene.GetCamera().GetCameraDesc();
        _cameraCullingMask = owner.CurrentBeScene.currentGeScene.GetCamera().GetCullingMask();
    }

    private void SetCameraDesc(GeCameraDesc desc)
    {
        if (owner.CurrentBeScene == null)
            return;
        if (owner.CurrentBeScene.currentGeScene == null)
            return;
        if (owner.CurrentBeScene.currentGeScene.GetCamera() == null)
            return;
        owner.CurrentBeScene.currentGeScene.GetCamera().SetCameraDesc(desc);
    }
    
    private void RecordCameraControlYOffset()
    {
        if (owner.CurrentBeScene == null)
            return;
        if (owner.CurrentBeScene.currentGeScene == null)
            return;
        if (owner.CurrentBeScene.currentGeScene.GetCamera().GetController() == null)
            return;
        _cameraControlOffsetBackUp =  owner.CurrentBeScene.currentGeScene.GetCamera().GetController().GetCameraPosition();
    }

    private void SetCameraControlOffset(bool isStore = false)
    {
        if (owner.CurrentBeScene == null)
            return;
        if (owner.CurrentBeScene.currentGeScene == null)
            return;
        if (owner.CurrentBeScene.currentGeScene.GetCamera().GetController() == null)
            return;
        var cameraControl = owner.CurrentBeScene.currentGeScene.GetCamera().GetController();
        cameraControl.SetPause(!isStore);
        if (isStore)
            cameraControl.SetCameraPos(_cameraControlOffsetBackUp);
        else
        {
            var newOffset = _cameraControlOffsetBackUp;
            newOffset.y = 4.42f;
            cameraControl.SetCameraPos(newOffset);
        }
    }

    private void SetCameraCullingMask(int mask)
    {
        owner.CurrentBeScene.currentGeScene.GetCamera().SetCullingMask(mask);
    }

    private void RecordActorNodeLayer()
    {
        if (owner.m_pkGeActor == null)
            return;
        _actorNodeLayerBackUp = owner.m_pkGeActor.GetActorNodeLayer();
    }

    /// <summary>
    /// 设置自己的模型层级
    /// </summary>
    private void SetOwnerModelLayer(int layer)
    {
        if (owner.m_pkGeActor == null)
            return;
        owner.m_pkGeActor.SetActorNodeLayer(layer);
    }
#endif
}