using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 在场景中心创建全屏特效
/// </summary>
public class Mechanism1026 : BeMechanism
{
    GeEffectEx baipingEffect = null;
    string effectPath = "Effects/Hero_Jianhun/Eff_jianhun_juexing/Prefeb/Eff_jianhun_juexing_inbaiping";
    public Mechanism1026(int mid, int lv) : base(mid, lv) { }

    public override void OnInit()
    {
        base.OnInit();
        effectPath = data.StringValueA[0];
    }

    public override void OnStart()
    {
        base.OnStart();
#if !LOGIC_SERVER
        if (owner.isLocalActor)
        {
            //显示白屏特效
            Vec3 pos = owner.CurrentBeScene.GetSceneCenterPosition().vec3;
            var vec = new Vector3(Screen.width / 2.0f, Screen.height / 2.0f, 0);
            Ray ray = Camera.main.ScreenPointToRay(vec);
            var t = -ray.origin.y / ray.direction.y;
            var worldPos = ray.GetPoint(t);
            var offsetPos = new Vec3(worldPos.x, owner.CurrentBeScene.logicZSize.fy, 0);
            baipingEffect = owner.CurrentBeScene.currentGeScene.CreateEffect(effectPath, 99999f, offsetPos, 2);
        }
#endif
    }

    public void RemoveEffect()
    {
#if !LOGIC_SERVER

        if (owner.isLocalActor)
        {

            if (baipingEffect != null)
            {
                owner.CurrentBeScene.currentGeScene.DestroyEffect(baipingEffect);
                baipingEffect = null;
            }
        }
#endif
    }

    public override void OnFinish()
    {
        RemoveEffect();
        base.OnFinish();
    }

    public override void OnDead()
    {
        RemoveEffect();
        base.OnDead();
    }
}
