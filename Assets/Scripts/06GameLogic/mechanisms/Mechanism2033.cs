using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 黑色大地魔幻场景变换机制
/// </summary>
public class Mechanism2033 : BeMechanism
{
    private int monsterID = 30780031;
    private string scenePath = "";
    private int transformTime = 1000;
    private int sceneTime = 15000;
#if !LOGIC_SERVER
    private GameObject scenePrefab = null;
    private MeshRenderer[] renders = null;
#endif
    private State state = State.NONE;
    private int mechanismBuffID = 0;
    private bool reverse = false;
    private readonly VInt3 scenePos = new VInt3(2.65f, 11.2f, 0);
    private readonly int skillID = 6273;
    enum State
    {
        START,
        TRANSFORM,
        END,
        NONE
    }

    public Mechanism2033(int id, int lv) : base(id, lv)
    { }

    public override void OnInit()
    {
        base.OnInit();
        scenePath = data.StringValueA[0];
        mechanismBuffID = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        sceneTime = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
        transformTime = TableManager.GetValueFromUnionCell(data.ValueC[0], level);
    }

    public override void OnReset()
    {
        scenePath = "";
        transformTime = 1000;
        sceneTime = 15000;

#if !LOGIC_SERVER
        scenePrefab = null;
        renders = null;
#endif

        state = State.NONE;
        mechanismBuffID = 0;
        reverse = false;
}

public override void OnStart()
    {
        base.OnStart();
        List<BeActor> list = GamePool.ListPool<BeActor>.Get();
        owner.CurrentBeScene.FindMonsterByID(list, monsterID);
        if (list.Count > 0)
        {
            BeActor boss = list[0];
            handleA = boss.RegisterEventNew(BeEventType.onDead, eventParam =>
            {
                OnBossDead();
            });

            handleB = boss.RegisterEventNew(BeEventType.onHPChange, (args) =>
            {//血量低于20%之后开始进行场景变换
                if (state != State.NONE) return;
                if (boss.sgGetCurrentState() == (int)ActionState.AS_IDLE || boss.sgGetCurrentState() == (int)ActionState.AS_RUN)
                {

                    VFactor f = new VFactor(200, GlobalLogic.VALUE_1000);
                    if (boss.GetEntityData().GetHPRate() < f)
                    {
                        SetBossPosition(boss);
                        boss.UseSkill(skillID);
                        owner.buffController.TryAddBuff(mechanismBuffID, -1);
                        state = State.START;
                        owner.delayCaller.DelayCall(transformTime, () =>
                        {
                            if (boss.IsDead()) return;
                            LoadEvilScene();
                            state = State.TRANSFORM;

                        });
                    }
                }
            });
        }
        GamePool.ListPool<BeActor>.Release(list);
    }

    private void SetBossPosition(BeActor actor)
    {
        actor.SetPosition(scenePos, true);
    }

    /// <summary>
    /// 死亡之后还原场景
    /// </summary>
    private void OnBossDead()
    {
        switch (state)
        {
            case State.START:
                owner.buffController.RemoveBuff(mechanismBuffID);
                break;
            case State.TRANSFORM:
                reverse = true;
                owner.delayCaller.DelayCall(1000, () =>
                {
                    owner.buffController.RemoveBuff(mechanismBuffID);
                    DestroyMagicScene();
                });
                break;
            case State.END:
                break;
            case State.NONE:
                break;
            default:
                break;
        }

    }

    /// <summary>
    /// 加载魔幻场景
    /// </summary>
    private void LoadEvilScene()
    {
#if !LOGIC_SERVER
        scenePrefab = AssetLoader.instance.LoadResAsGameObject(scenePath);
        if (scenePrefab != null)
        {
            Utility.AttachTo(scenePrefab, owner.CurrentBeScene.currentGeScene.GetSceneRoot());
            renders = scenePrefab.GetComponentsInChildren<MeshRenderer>();
            owner.CurrentBeScene.currentGeScene.AddToColorDescList(scenePrefab);
        }
#endif
    }

    private int timer = 0;
    private int reversTime = 0;
    /// <summary>
    /// 更新魔幻场景跟新
    /// </summary>
    /// <param name="deltaTime"></param>
    public override void OnUpdateGraphic(int deltaTime)
    {
        base.OnUpdateGraphic(deltaTime);
#if !LOGIC_SERVER
        if (renders == null || scenePrefab == null) return;
#endif
        if (reverse)
        {
            reversTime += deltaTime;
            if (reversTime<= 2000)
            {
                TransformScenePrefab(true);
            }
        }
        else
        {
            if (timer < sceneTime)
            {
                timer += deltaTime;
                TransformScenePrefab();
                reversValue = mainValue;
            }
        }
    }
    private float reversValue = 0;
    private float mainValue = 0;
    private void TransformScenePrefab(bool revers = false)
    {
#if !LOGIC_SERVER
        if (renders != null)
        {
            for (int i = 0; i < renders.Length; i++)
            {
                MeshRenderer render = renders[i];
                
                if (revers)
                {
                    mainValue = reversValue + (1.3f - reversValue) * (reversTime / 2000.0f);
                }
                else
                {
                    mainValue = 1.3f - 1.3f * (timer / (float)sceneTime);
                }
                mainValue = Mathf.Clamp(mainValue, 0, 1.3f);

                render.sharedMaterial.SetFloat("_MainValue", mainValue);
            }
        }
#endif
    }

    private void DestroyMagicScene()
    {
#if !LOGIC_SERVER
        if (scenePrefab != null)
        {
            owner.CurrentBeScene.currentGeScene.ClearColorDesc(scenePrefab);
            GameObject.Destroy(scenePrefab);
            scenePrefab = null;
        }
#endif
    }


    public override void OnFinish()
    {
        base.OnFinish();
        DestroyMagicScene();
    }
}
