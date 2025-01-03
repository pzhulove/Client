using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/*
 * 例子:
 * BeMoveBy moveBy = BeMoveBy.Create(actor, 5000, actor.GetPostion(), new VInt3(5, 0 ,0));
	actor.actionManager.RunAction(moveBy);

	BeScaleBy scaleBy = BeScaleBy.Create(actor, 2000, actor.GetScale(), 1.0f);
	actor.actionManager.RunAction(scaleBy);

	BeScaleTo scaleTo = BeScaleTo.Create(actor, 1000, actor.GetScale(), 1.5f);
	scaleTo.SetFinishCallback(()=>{
		BeScaleTo scaleTo2 = BeScaleTo.Create(actor, 1000, actor.GetScale(), 1.0f);
		actor.actionManager.RunAction(scaleTo2);
	});
	actor.actionManager.RunAction(scaleTo);
*/
public class BeActionManager {
	List<BeAction> actionList = new List<BeAction>();

    public void Init()
    {
        //actionList = new List<BeAction>();
    }
    public void Deinit()
    {
        actionList.Clear();
    }

    public void RunAction(BeAction action)
	{
		action.manager = this;
		action.Start();
		actionList.Add(action);	
	}

	private bool CheckCanRemove(BeAction item)
	{
		if (!item.IsRunning())
			return true;
		return false;
	}

	public void Update(int deltaTime)
	{
        bool bDirty = false;
		for(int i=0; i<actionList.Count; ++i)
		{
			BeAction action = actionList[i];
            if(CheckCanRemove(action))
            {
                bDirty = true;
            }

            if (!action.IsPause())
				action.Update(deltaTime);
		}

        if (bDirty)
            _RemoveAction();
    }

    void _RemoveAction()
    {
        actionList.RemoveAll(CheckCanRemove);
    }

	public void Pause()
	{
		for(int i=0; i<actionList.Count; ++i)
		{
			BeAction action = actionList[i];
			if (action.IsRunning())
				action.Pause();
		}
	}

	public void Resume()
	{
		for(int i=0; i<actionList.Count; ++i)
		{
			BeAction action = actionList[i];
			if (action.IsRunning() && action.IsPause())
				action.Resume();
		}
	}

	public void StopAll()
	{
		for(int i=0; i<actionList.Count; ++i)
		{
			BeAction action = actionList[i];
			if (action.IsRunning())
				action.Stop();
		}
	}
	public BeAction GetRunningAction()
	{
		for(int i=0; i<actionList.Count; ++i)
		{
			BeAction action = actionList[i];
			if (action.IsRunning())
				return action;
		}

		return null;
	}

    public List<BeAction> GetActionList()
    {
        return actionList;
    }
}
