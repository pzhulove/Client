using System.Collections;
using System.Collections.Generic;
using GameClient;
using UnityEngine;
/// <summary>
/// 镜头拉近机制
/// </summary>
public class Mechanism1015 : BeMechanism
{
    private int offsetY = 0;
    private float speed = 3.0f;
    private float targetSize = 2.05f;
    private float restoreSpeed = 0; //镜头恢复速度
    private int skillId = 0;


    private float originalSize = 3.05f;
    private bool skillCancelFlag = false;

    public Mechanism1015(int mid, int lv) : base(mid, lv) { }

    public override void OnInit()
    {
        originalSize = 3.05f;
        skillCancelFlag = false;
        base.OnInit();
        offsetY = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        speed = TableManager.GetValueFromUnionCell(data.ValueB[0], level) / 1000.0f;
        targetSize = TableManager.GetValueFromUnionCell(data.ValueC[0], level) / 1000.0f;
        restoreSpeed = TableManager.GetValueFromUnionCell(data.ValueD[0], level) / 1000.0f;
        skillId = TableManager.GetValueFromUnionCell(data.ValueE[0],level);

        if (speed == 0)
        {
            speed = 3.0f;
        }

        if (targetSize == 0)
        {
            targetSize = 2.05f;
        }
    }

    public override void OnStart()
    {
        base.OnStart();
#if !LOGIC_SERVER
        if (owner.isLocalActor && owner.CurrentBeScene!=null && owner.CurrentBeScene.currentGeScene!=null && owner.CurrentBeScene.currentGeScene.GetCamera()!=null && owner.CurrentBeScene.currentGeScene.GetCamera().GetController()!=null)
        {
            var targetPos = Vector3.zero;
            if (offsetY != 0)
            {
                originalSize = owner.CurrentBeScene.currentGeScene.GetCamera().orthographicSize;
                Vector3 pos = owner.GetPosition().vector3;
                Transform transform = Camera.main.transform;
                var x = (pos.x - transform.position.x) * (1f - targetSize / originalSize);
                var z = (pos.z - transform.position.z + (owner.CurrentBeScene.currentGeScene.GetCamera().GetController().m_CtrlOffset.z - 1)) * (1f - targetSize / originalSize);
                targetPos = new Vector3(x, offsetY / 1000.0f, z);
            }
            owner.CurrentBeScene.currentGeScene.GetCamera().GetController().StartCameraPull(targetPos, speed, targetSize);
        }
#endif
        handleA = owner.RegisterEventNew(BeEventType.onSkillCancel, SkillCancel);
        handleB = owner.RegisterEventNew(BeEventType.onCastSkillFinish, SkillFinish);
    }

    private void SkillCancel(BeEvent.BeEventParam args)
    {
#if !LOGIC_SERVER
        if (!owner.isLocalActor)
            return;
        var id = args.m_Int;
        if (id != skillId)
            return;
        skillCancelFlag = true;
        owner.CurrentBeScene.currentGeScene.GetCamera().GetController().RestoreCameraPull(0);

#endif
    }

    private void SkillFinish(BeEvent.BeEventParam args)
    {
#if !LOGIC_SERVER
        if (!owner.isLocalActor)
            return;
        var id =  args.m_Int;
        if (id != skillId)
            return;
        skillCancelFlag = true;
        owner.CurrentBeScene.currentGeScene.GetCamera().GetController().RestoreCameraPull(0);

#endif
    }

    public override void OnFinish()
    {
        base.OnFinish();
        RestoreCamera();
    }
    private void RestoreCamera()
    {
#if !LOGIC_SERVER
        if (skillCancelFlag)
            return;
        if (!owner.isLocalActor)
            return;
        owner.CurrentBeScene.currentGeScene.GetCamera().GetController().RestoreCameraPull(restoreSpeed);
#endif

    }
}
