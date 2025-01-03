using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill3217 : BeSkill {

    private IBeEventHandle handle = null;
    private float targetSize = 2.05f;
    private float originalSize = 3.05f;
    private Vector3 targetPos;
    private bool flag = false;
    private float speed = 3.0f;
    public Skill3217(int sid, int skillLevel) : base(sid, skillLevel)
    {

    }

    public override void OnStart()
    {
        time = 0;
#if !LOGIC_SERVER
        if (Camera.main != null)
        {
            originalSize = Camera.main.orthographicSize;
        }
#endif
        handle = owner.RegisterEventNew(BeEventType.onSkillCurFrame, (args) =>
        //handle = owner.RegisterEvent(BeEventType.onSkillCurFrame, (args) =>
        {
#if !LOGIC_SERVER
            if (owner.isLocalActor)
            {
                string flag = args.m_String;
                if (flag == "hideScene" && Camera.main != null && Camera.main.transform != null)
                {
                    this.flag = true;
                 //   Camera.main.orthographicSize = targetSize;
                    Vector3 pos = owner.GetPosition().vector3;
                    Transform transform = Camera.main.transform;
                    var x = (pos.x - transform.position.x) * (1f - targetSize / originalSize);

                    if(owner.CurrentBeScene!=null 
                    && owner.CurrentBeScene.currentGeScene!=null 
                    && owner.CurrentBeScene.currentGeScene.GetCamera()!=null
                    && owner.CurrentBeScene.currentGeScene.GetCamera().GetController() != null)
                    {
                        var z = (pos.z - transform.position.z + (owner.CurrentBeScene.currentGeScene.GetCamera().GetController().m_CtrlOffset.z - 1)) * (1f - targetSize / originalSize);            
                        // transform.localPosition = new Vector3(x, 0, z);
                        targetPos = new Vector3(x, 0, z);
                    }
                }
                else if (flag == "showScene" && Camera.main != null && Camera.main.transform != null)
                {
                    ResetCamera();
                }
            }
#endif
        });

    }
    int time = 0;
    public override void OnUpdate(int iDeltime)
    {
#if !LOGIC_SERVER
        if (owner.isLocalActor)
        {
            base.OnUpdate(iDeltime);
            time += iDeltime;
            if (flag)
            {
                Camera.main.transform.localPosition = Vector3.Lerp(Camera.main.transform.localPosition, targetPos, time / 1000.0f * speed);
                Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, targetSize, time / 1000.0f * speed);
            }
        }
#endif
    }

    private void RemoveHandle()
    {
        if (handle != null)
        {
            handle.Remove();
            handle = null;
        }
    }

    public override void OnCancel()
    {
        ResetCamera();
        RemoveHandle();
        base.OnCancel();
    }

    public override void OnFinish()
    {
        ResetCamera();
        RemoveHandle();
        base.OnFinish();
    }

    private void ResetCamera()
    {
#if !LOGIC_SERVER
        if (!owner.isLocalActor)
            return;
        this.flag = false;
        var main = Camera.main;
        if (main != null)
        {
            main.orthographicSize = originalSize;
            main.transform.localPosition = Vector3.zero;   
        }
#endif
    }
}
