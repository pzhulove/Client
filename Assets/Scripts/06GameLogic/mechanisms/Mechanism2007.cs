using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 黑色大地四大神兽链接BOSS机制
/// </summary>
public class Mechanism2007 : BeMechanism
{
    private readonly string chainEffect = "Effects/Monster_HMZD_zhenshou/Prefab/Eff_hmzd_zhenshou_shouhulian";
    private GameObject attachRoot = null;
    private int buffID = 0;
    private readonly VInt defaultHeight = VInt.Float2VIntValue(1.5f);
    private BeActor target = null;
    private GeEffectEx effect;
    private readonly int BossID = 30770021;
#if !SERVER_LOGIC
    private GameObject node;
#endif
    public Mechanism2007(int mid, int lv) : base(mid, lv) { }

    public override void OnInit()
    {
        buffID = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
    }

    public override void OnReset()
    {
        attachRoot = null;
        target = null;
        effect = null;
#if !SERVER_LOGIC
        node = null;
#endif
}

public override void OnStart()
    {
        base.OnStart();


        List<BeActor> list = GamePool.ListPool<BeActor>.Get();
        owner.CurrentBeScene.FindMonsterByID(list, BossID);
        if (list.Count > 0)
        {
            target = list[0];
            AddChain(target);
        }
        GamePool.ListPool<BeActor>.Release(list);
    }

    /// <summary>
    /// 移除链接
    /// </summary>
    public void RemoveChain()
    {
        if (target != null)
        {
            target.buffController.RemoveBuff(buffID);
            ClearEffect();
        }
    }

    public override void OnFinish()
    {
        base.OnFinish();
        RemoveChain();
    }

    /// <summary>
    /// 链接boss
    /// </summary>
    /// <param name="toActor"></param>
    protected void AddChain(BeActor toActor)
    {

        effect = owner.m_pkGeActor.CreateEffect(chainEffect, null, 99999, Vec3.zero);

#if !LOGIC_SERVER
        var goEffect = effect.GetRootNode();

        bool noStrNode = false;
        var go = GetAttachGameObject(owner, "[actor]Body", ref noStrNode);

        Battle.GeUtility.AttachTo(goEffect, go);

        if (noStrNode)
        {
            var effPos = effect.GetPosition();
            effPos.z += defaultHeight.scalar;
            effect.SetPosition(effPos);
        }

        var bind = goEffect.GetComponentInChildren<ComCommonBind>();
        if (bind != null)
        {
            var com = bind.GetCom<LightningChain>("lcScript");
            if (com != null)
            {
                var origin = toActor.m_pkGeActor.GetAttachNode("[actor]Orign");
                if (origin != null)
                {
                    if (node == null)
                    {
                        node = new GameObject("Node");
                    }
                    Utility.AttachTo(node, origin);
                    node.transform.localPosition = new Vector3(0, 1, 0);
                    com.target = node;
                    com.ForceUpdate();
                }

            }

            GameObject goNodeA = bind.GetGameObject("goNodeA");
            if (goNodeA != null)
            {
                var comOffset = goNodeA.GetComponent<OffsetChange>();
                if (comOffset == null)
                {
                    comOffset = goNodeA.AddComponent<OffsetChange>();
                    comOffset.LoopCount = 0;
                    comOffset.AStartTime = 0;
                    comOffset.AXSpeed = -5;
                    comOffset.AYSpeed = 0;
                    comOffset.BStartTime = 0;
                    comOffset.BXSpeed = 0;
                    comOffset.BYSpeed = 0;
                }
            }
        }
#endif
        toActor.buffController.TryAddBuff(buffID, -1, level);
    }

    protected GameObject GetAttachGameObject(BeActor actor, string nodeName, ref bool noStrNode)
    {
#if !SERVER_LOGIC
        attachRoot = actor.m_pkGeActor.GetAttachNode(nodeName);
        if (attachRoot == null)
        {
            noStrNode = true;
            attachRoot = actor.m_pkGeActor.GetEntityNode(GeEntity.GeEntityNodeType.Root);
        }
#endif
        return attachRoot;
    }

    private void ClearEffect()
    {
#if !LOGIC_SERVER

        if (effect != null)
        {
            var obj = effect.GetRootNode();

            if (obj != null)
            {
                var bind = obj.GetComponentInChildren<ComCommonBind>();
                if (bind != null)
                {
                    var com = bind.GetCom<LightningChain>("lcScript");
                    com.target = null;
                    com.SetVertexCount(0);
                }
            }

            owner.m_pkGeActor.DestroyEffect(effect);
        }
        if (node != null)
        {
            GameObject.Destroy(node);
            node = null;
        }

#endif
    }

}
