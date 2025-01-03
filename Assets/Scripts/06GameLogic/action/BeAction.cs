using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class BeAction {
	public delegate void Del();
	public BeActionManager manager;

	protected BeEntity target;
	protected int delay;
	private int _duration;
	protected int duration
	{
		get => _duration;
		set
		{
			// 避免除0
			int v = value;
			if (value == 0)
				v = 1;
			_duration = v;
		}
	}

	protected bool pause;
	protected Del finishCallback;

	protected bool running;
	protected int timeAcc;

	protected bool delayed;


	public void Start()
	{
		timeAcc = 0;
		running = true;
		delayed = false;

		OnStart();
	}

	public void Update(int deltaTime)
	{
		if (!running)
			return;

		OnTick(deltaTime);

		timeAcc += deltaTime;

		if (delay > 0 && !delayed)
		{
			if (timeAcc > delay)
			{
				delayed = true;
				timeAcc -= delay;
				return;
			}
			else {
				return;
			}
		}

		if(timeAcc > duration)
		{
			timeAcc = duration;
		}
		VFactor process = new VFactor(timeAcc,duration);
		//process = Mathf.Min(process, 1.0f);

		OnUpdate(process);

		if (timeAcc >= duration)
		{
			Stop();
			if (finishCallback != null)
				finishCallback();
		}
	}

	public void Stop()
	{
		running = false;
	}

	public bool IsRunning()
	{
		return running;
	}

	public void Pause()
	{
		pause = true;
	}

	public void Resume()
	{
		pause = false;
	}

	public bool IsPause()
	{
		return pause;
	}

	public void SetFinishCallback(Del callback)
	{
		finishCallback = callback;
	}


	public virtual void OnUpdate(VFactor process){}
	public virtual void OnTick(int deltaTime){}
	public virtual void OnStart(){}
		
}


public class BeActionSequence : BeAction
{
	protected List<BeAction> actionList = new List<BeAction>();
	protected int curIndex;

	public static BeActionSequence Create(params object[] args)
	{
        var seqAction = new BeActionSequence
        {
            duration = int.MaxValue
        };

        for (int i=0; i<args.Length; ++i)
			seqAction.actionList.Add(args[i] as BeAction);
			
		return seqAction;
	}


	public sealed override void OnStart ()
	{
		curIndex = -1;

		StartNext();
	}

	public sealed override void OnTick (int deltaTime)
	{
		var curAction = actionList[curIndex];
		if (!curAction.IsRunning())
		{
			StartNext();
		}
	}

	protected void StartNext()
	{
		curIndex++;
		if (curIndex >= actionList.Count)
		{
			Stop();
			return;
		}

		var action = actionList[curIndex];
		if (manager != null)
			manager.RunAction(action);
	}
}

public class BeDelay : BeAction
{
	public static BeDelay Create(int duration)
	{
        var delay = new BeDelay
        {
            duration = duration
        };

        return delay;
	}
}

public class BeMoveBy : BeAction
{
	protected VInt3 startPos;
	protected VInt3 deltaPos;
	protected bool ignoreBlcok = false;
    protected IEntityFilter filter = null;
    protected bool m_NeedUpdateBackPos = false;     //移动的位置是否记录到备份位置 用于解决裂波斩释放格挡玩家后 玩家瞬移到原来位置的BUG

    public static BeMoveBy Create(BeEntity entity, int dur, VInt3 curPos, VInt3 deltaPos, bool ignoreBlock=true, IEntityFilter fil = null,bool needUpdateBackPos = false)
	{
        BeMoveBy moveBy = new BeMoveBy
        {
            duration = dur,
            target = entity,

            startPos = curPos,
            deltaPos = deltaPos,
            ignoreBlcok = ignoreBlock,
            m_NeedUpdateBackPos = needUpdateBackPos
        };

        return moveBy;
	}

	public sealed override void OnUpdate (VFactor process)
	{
		var pos = startPos + deltaPos * process;

        if (target == null)
            return;
        if (!ignoreBlcok && target.CurrentBeScene.IsInBlockPlayer(pos))
            return;
        if (filter != null && !filter.isFit(target))
            return;
        target.SetPosition(pos, false, true, m_NeedUpdateBackPos);
    }
}


public class BeMoveTo : BeMoveBy
{
	public static new BeMoveTo Create(BeEntity entity, int dur, VInt3 curPos, VInt3 toPos, bool ignoreBlock=true,IEntityFilter fil = null)
	{
        BeMoveTo moveTo = new BeMoveTo
        {
            duration = dur,
            target = entity,

            startPos = curPos,
            deltaPos = toPos - curPos,
            ignoreBlcok = ignoreBlock,

            filter = fil,
        };

        return moveTo;
	}
}

public class BeScaleBy : BeAction
{
	protected VInt startScale;
	protected VInt deltaScale;

	public static BeScaleBy Create(BeEntity entity, int dur, VInt curScale, VInt deltaScale)
	{
        BeScaleBy scaleBy = new BeScaleBy
        {
            duration = dur,
            target = entity,

            startScale = curScale,
            deltaScale = deltaScale
        };

        return scaleBy;
	}

	public sealed override void OnUpdate (VFactor process)
	{
		VInt scale = startScale + deltaScale.i * process;
		if (target != null)
		{
			target.SetScale(scale);
		}
	}
}

public class BeScaleTo : BeScaleBy
{
	public static new BeScaleTo Create(BeEntity entity, int dur, VInt curScale, VInt toScale)
	{
        BeScaleTo scaleTo = new BeScaleTo
        {
            duration = dur,
            target = entity,

            startScale = curScale,
            deltaScale = toScale - curScale
        };

        return scaleTo;
	}
}



public class BeSimpleAction : BeAction
{
	
#if !SERVER_LOGIC 

	protected GameObject targetGameObject;

 #endif

}

public class BeSimpleMoveBy : BeSimpleAction
{
	protected Vector3 startPos;
	protected Vector3 deltaPos;

	public static BeSimpleMoveBy Create(GameObject entity, int dur, Vector3 curPos, Vector3 deltaPos, int delay = 0)
	{
        BeSimpleMoveBy moveBy = new BeSimpleMoveBy
        {
            duration = dur,
            delay = delay,
#if !SERVER_LOGIC

            targetGameObject = entity,

#endif

            startPos = curPos,
            deltaPos = deltaPos
        };

        return moveBy;
	}

	public sealed override void OnUpdate (VFactor process)
	{
		
#if !SERVER_LOGIC 
		var pos = startPos + deltaPos * process.single;
		if (targetGameObject != null)
		{
			targetGameObject.transform.localPosition = pos;
		}

 #endif

	}
}

public class BeSimpleScaleBy : BeSimpleAction
{
	protected VInt startScale;
	protected VInt deltaScale;
	protected Vector3 tmpScale;

	public static BeSimpleScaleBy Create(GameObject entity, int dur, VInt curScale, VInt deltaScale, int delay = 0)
	{
        BeSimpleScaleBy scaleBy = new BeSimpleScaleBy
        {
            duration = dur,
            delay = delay,
#if !SERVER_LOGIC

            targetGameObject = entity,

#endif

            startScale = curScale,
            deltaScale = deltaScale
        };
        return scaleBy;
	}

	public sealed override void OnUpdate (VFactor process)
	{
#if !SERVER_LOGIC 

		float scale = startScale.scalar + deltaScale.scalar * process.single;
		tmpScale.x = scale;
		tmpScale.y = scale;
		tmpScale.z = scale;
		if (targetGameObject != null)
		{
			targetGameObject.transform.localScale = tmpScale;
		}

 #endif

	}
}

public class BeSimpleOpaqueBy : BeSimpleAction
{
	protected float startOpaque;
	protected float deltaOpaque;

#if !SERVER_LOGIC 

	protected MaskableGraphic maskableGraphic;
	protected Color tmpColor;


 #endif


	public static BeSimpleOpaqueBy Create(GameObject entity, int dur, float curOpaque, float deltaOpaque, int delay = 0)
	{
        BeSimpleOpaqueBy opaqueBy = new BeSimpleOpaqueBy
        {
            duration = dur,
            delay = delay,
#if !SERVER_LOGIC

            targetGameObject = entity,

#endif

            startOpaque = curOpaque,
            deltaOpaque = deltaOpaque
        };

        return opaqueBy;
	}

	public sealed override void OnStart ()
	{
#if !SERVER_LOGIC 

		if (targetGameObject != null)
		{
			var text = targetGameObject.GetComponentInChildren<Text>(true);
			if (text != null)
			{
				maskableGraphic = text as MaskableGraphic;
			}

			var image = targetGameObject.GetComponentInChildren<Image>(true);
			if (image != null)
			{
				maskableGraphic = image as MaskableGraphic;
			}

			if (maskableGraphic != null)
			{
				tmpColor = maskableGraphic.color;
				tmpColor.a = startOpaque;
				maskableGraphic.color = tmpColor;
			}
		}

 #endif


	}

	public sealed override void OnUpdate (VFactor process)
	{
#if !SERVER_LOGIC 

		float opaque = startOpaque + deltaOpaque * process.single;
		tmpColor.a = opaque;
		if (maskableGraphic != null)
		{
			maskableGraphic.color = tmpColor;
		}

 #endif

	}
}