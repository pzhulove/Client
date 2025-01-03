using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainBeActor
{
    public BeActor actor;
    public bool isHaveBuff;

    public ChainBeActor(BeActor _actor)
    {
        actor = _actor; ;
        isHaveBuff = false;
    }
}

public class ChainTarget
{
    public GeEffectEx effect;
    public BeActor fromActor;
    public BeActor toActor;

    public ChainTarget(BeActor _fromActor, BeActor _toActor, GeEffectEx _effect)
    {
        fromActor = _fromActor;
        toActor = _toActor;
        effect = _effect;
    }
    
    public bool IsEqual(BeActor _fromActor, BeActor _toActor)
    {
        if (fromActor != null &&
            _fromActor != null &&
            toActor != null &&
            _toActor != null &&
            fromActor.GetPID() == _fromActor.GetPID() && toActor.GetPID() == _toActor.GetPID())
        {
            return true;
        }
        return false;
    }
}

public class ChainHurtTime
{
     public int actorPid;
     public int repeatSec = 0;
     private int time = 0;
     public enum State
     {
         None,
         Update,
         Finish,
     }

     private State curState = State.None;

     public bool IsFinish
     {
         get
         {
             return curState == State.Finish;
         }
         
     }
     
     public ChainHurtTime()
     {
         curState = State.None;
     }

     public void ResetTime()
     {
         time = repeatSec;
         curState = State.Update;
     }
     
     public bool IsEqual(int pid)
     {
         if (actorPid == pid)
             return true;
         return false;
     }
     
     public void OnUpdate(int _deltaTime)
     {
         if (curState == State.None)
         {
             time = repeatSec;
             curState = State.Update;
         }
         if (curState == State.Update)
         {
             time -= _deltaTime;
             if (time <= 0)
             {
                 curState = State.Finish;
             }
         }
     }
}

// 雷晶体连线伤害
public class Mechanism10020 : BeMechanism
{
    string chainEffect = null;
    int entityResId;
    int flagBuffId;
    int maxDistance;
    int hurtID;
    int repeatSec;
    
    bool m_CanChain = false;
    public bool MCanChain
    {
        get => m_CanChain;
        set
        {
            if (m_CanChain != value)
            {
                m_CanChain = value;
                if (m_CanChain)
                {
                    AddChains();
                }
                else
                {
                    ClearChains();
                }
            }
        }
    }

    protected List<IBeEventHandle> monsterActorsHandles = new List<IBeEventHandle>();
    List<ChainBeActor> mChainBeActors = new List<ChainBeActor>();
    List<ChainTarget> mChainTargets = new List<ChainTarget>();
    List<BeActor> mTargetList = new List<BeActor>();
    List<ChainHurtTime> mTargetListTime = new List<ChainHurtTime>();
    
    readonly string strNode = "[actor]Body";
    readonly VInt defaultHeight = VInt.Float2VIntValue(0.0f);
    GameObject attachRoot = null;
    
    public Mechanism10020(int mid, int lv) : base(mid, lv)
    {
    }
    
    public override void OnInit()
    {
        chainEffect = data.StringValueA[0];
        entityResId = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        flagBuffId = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
        maxDistance = TableManager.GetValueFromUnionCell(data.ValueC[0], level);
        hurtID = TableManager.GetValueFromUnionCell(data.ValueD[0], level);
        repeatSec = TableManager.GetValueFromUnionCell(data.ValueE[0], level);
    }

    public override void OnStart()
    {
        sceneHandleA = owner.CurrentBeScene.RegisterEventNew(BeEventSceneType.onSummon, (args) =>
        {
            BeActor monster = (BeActor)args.m_Obj;
            BeActor monsterOwner = (BeActor)args.m_Obj2;
            if (monsterOwner.GetPID() == owner.GetPID())
            {
                AddChainBeActor(monster);
            }
        });
        
        handleD = owner.RegisterEventNew(BeEventType.onSummon, (args) =>
        {
            BeActor monster = args.m_Obj as BeActor;
            AddChainBeActor(monster);
        });
    }

    void AddChainBeActor(BeActor actor)
    {
        if (actor != null && actor.GetEntityData().MonsterIDEqual(entityResId))
        {
            var chainBeActor = new ChainBeActor(actor);
            mChainBeActors.Add(chainBeActor);
            var handle1 = actor.RegisterEventNew(BeEventType.onAddBuff, (args2) =>
            {
                BeBuff buff = (BeBuff) args2.m_Obj;
                if (buff.buffID == flagBuffId)
                {
                    CanChain(actor, true);
                }
            });
            monsterActorsHandles.Add(handle1);
            var handle2 = actor.RegisterEventNew(BeEventType.onRemoveBuff, (args2) =>
            {
                BeBuff buff = (BeBuff) args2.m_Obj;
                if (buff.buffID == flagBuffId)
                {
                    CanChain(actor, false);
                }
            });
            monsterActorsHandles.Add(handle2);
        }
        if (MCanChain)
        {
            AddChains();
        }
    }

    protected void CanChain(BeActor actor,bool flag)
    {
        foreach (var item in mChainBeActors)
        {
            if (actor.GetPID() == item.actor.GetPID())
            {
                item.isHaveBuff = flag;
                break;
            }
        }
        foreach (var item in mChainBeActors)
        {
            if (item.isHaveBuff)
            {
                MCanChain = true;
                return;
            }
        }

        MCanChain = false;
    }

    protected void AddChains()
    {
        for (int i = 0; i < mChainBeActors.Count; i++)
        {
            for (int j = i + 1; j < mChainBeActors.Count; j++)
            {
                if (i != j)
                {
                    var actor1 = mChainBeActors[i].actor;
                    var actor2 = mChainBeActors[j].actor;
                    bool isAdd = true;
                    foreach (var item in mChainTargets)
                    {
                        if (item.IsEqual(actor1, actor2))
                        {
                            isAdd = false;
                            break;
                        }
                    }

                    if (isAdd)
                    {
                        var effect = AddChain(actor1, actor2);
                        ChainTarget target = new ChainTarget(actor1, actor2, effect);
                        mChainTargets.Add(target);
                    }
                }
            }
        }
    }
    
    protected void ClearChains()
    {
        for(int i=0; i<mChainTargets.Count; ++i)
        {
            var chainTarget = mChainTargets[i];
#if !LOGIC_SERVER
            try
            {
                var effect = chainTarget.effect;
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
            catch (System.Exception e)
            {
#if UNITY_EDITOR && !LOGIC_SERVER
                Logger.LogErrorFormat("mechanism10010 Clear err:{0}", e.ToString());
#endif
            }
#endif
        }
        mChainTargets.Clear();
    }

    public override void OnUpdate(int deltaTime)
     {
         base.OnUpdate(deltaTime);
         if (MCanChain)
         {
             owner.CurrentBeScene.FindMainActor(mTargetList);
             foreach (var toActor in mTargetList)
             {
                 DoHurt(toActor);
             }
             foreach (var targetTime in mTargetListTime)
             {
                 targetTime.OnUpdate(deltaTime);
             }
         }
         else
         {
             mTargetListTime.Clear();
         }
     }
    protected GameObject GetAttachGameObject(BeActor actor, string nodeName, ref bool noStrNode)
    {

#if !LOGIC_SERVER
        if (actor == null || actor.m_pkGeActor == null)
            return null;
        attachRoot = actor.m_pkGeActor.GetAttachNode(nodeName);
        if (attachRoot == null)
        {
            noStrNode = true;
            attachRoot = actor.m_pkGeActor.GetEntityNode(GeEntity.GeEntityNodeType.Root);
        }

#endif
        return attachRoot;
    }
    
    protected GeEffectEx AddChain(BeActor fromActor, BeActor toActor)
	{
        var effect = owner.m_pkGeActor.CreateEffect(chainEffect, null, 99999, Vec3.zero);
#if !LOGIC_SERVER
		var goEffect = effect.GetRootNode();

		bool noStrNode = false;
		var go = GetAttachGameObject(fromActor, strNode, ref noStrNode);
        

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
				bool hasStrNode = false;
				com.target = GetAttachGameObject(toActor, strNode, ref hasStrNode);
                com.offect = new Vector3(0.0f, 0.0f, 1.5f);
				com.ForceUpdate();
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
        return effect;
    }

    public ChainHurtTime GetTargetListTime(int pid)
    {
        foreach (var targetListTime in mTargetListTime)
        {
            if (targetListTime.IsEqual(pid))
                return targetListTime;
        }

        return null;
    }

    public bool isInChainTarget(BeActor toActor)
    {
        if (toActor != null)
        {
            for (int i = 0; i < mChainBeActors.Count; i++)
            {
                for (int j = i + 1; j < mChainBeActors.Count; j++)
                {
                    if (i != j)
                    {
                        var actor1 = mChainBeActors[i].actor;
                        var actor2 = mChainBeActors[j].actor;
                        var point1 = actor1.GetPosition();
                        var point2 = actor2.GetPosition();
                        if (PointToSegDist(point1, point2, toActor.GetPosition()) <= maxDistance)
                        {
                            return true;
                        }
                    }
                }
            }
        }

        return false;
    }
    
    public void DoHurt(BeActor toActor)
    {
        bool isIn = isInChainTarget(toActor);
        var hitPos = toActor.GetPosition();
        if (isIn)
        {
            hitPos.z += VInt.one.i;
            var targetListTime = GetTargetListTime(toActor.GetPID());
            if (targetListTime != null)
            {
                if (targetListTime.IsFinish)
                {
                    bool face = toActor.GetFace();
                    owner._onHurtEntity(toActor, hitPos, hurtID);
                    toActor.SetFace(face);
                    targetListTime.ResetTime();
                }
            }
            else
            {
                bool face = toActor.GetFace();
                owner._onHurtEntity(toActor, hitPos, hurtID);
                toActor.SetFace(face);
                ChainHurtTime chainHurtTime = new ChainHurtTime();
                chainHurtTime.actorPid = toActor.GetPID();
                chainHurtTime.repeatSec = repeatSec;
                mTargetListTime.Add(chainHurtTime);
            }
        }
        else
        {
            var targetListTime = GetTargetListTime(toActor.GetPID());
            if (targetListTime != null)
            {
                mTargetListTime.Remove(targetListTime);
            }
        }
    }
    
    /************************************************计算点到线段的距离**************************************************
                             /P                                          /P                           P\
                            /                                       　　/                              　\
                           /                                       　　/                               　 \
                          /                                          /                                    \
                        A ----C-------B                    A--------B   C                           C     A ----------B

    长度：                        CP                                BP                                        AP
    计算：d = dot(AP,AB)/|AB|^2
    判断依据：                    if(0<d<1)                    if(d>1)                                if(d<0)
    ************************************************计算点到线段的距离**************************************************/
    /**
    @brief 
    @param[VInt3] point_A    点A的位置坐标（x,y,z）|(x,y)
    @param[VInt3] point_B    点B的位置坐标（x,y,z）|(x,y)
    @param[VInt3] point_C    点C的位置坐标（x,y,z）|(x,y)
    @param[int]             dot        表示点C与线段AB的相对位置        此为返回值   if(0<dot<1)点C在线段AB的中间区域 if(dot>1)点C在线段AB的右边区域 if(dot<0)点C在线段AB的左边区域
    @param[VInt3]         point_C    点C的位置坐标（x,y,z）|(x,y)    此为返回值
    @return                int        返回点C到线段AB的最近距离    
    */
    public int PointToSegDist(VInt3 point_A, VInt3 point_B, VInt3 point_P)
    {
        var AP = point_P - point_A;
        var AB = point_B - point_A;
        long APx = AP.x;
        long APy = AP.y;
        long APz = AP.z;
        long ABx = AB.x;
        long ABy = AB.y;
        long ABz = AB.z;
        var dot = (((APx * ABx) + (APy * ABy)) + (APz * ABz));
        
        int length;
        if (dot > AB.sqrMagnitudeLong)
        {
            //距离为BP
            var BP = point_P - point_B; 
            length = BP.magnitude;
        }
        else if(dot < 0)
        {
            //距离为AP 
            length = AP.magnitude;
        }
        else
        {
            //距离为PC
            VFactor vf = VFactor.NewVFactor(dot, AB.sqrMagnitudeLong);
            var AC = VInt3.zero;
            if(AB.sqrMagnitudeLong != 0) 
                AC = AB * vf;
            var point_C = AC  + point_A;
            var PC = point_P - point_C; 
            length = PC.magnitude;
        }

        return length;
    }
    
    public override void OnReset()
    {
        ClearData();
    }
    public override void OnFinish()
    {
        ClearData();
    }

    void ClearData()
    {
        m_CanChain = false;
        ClearChains();
        foreach (var handle in monsterActorsHandles)
        {
            if(handle != null)
            {
                handle.Remove();
            }
        }
        monsterActorsHandles.Clear();
        mChainBeActors.Clear();
        mChainTargets.Clear();
        mTargetList.Clear();
        mTargetListTime.Clear();
    }
}
