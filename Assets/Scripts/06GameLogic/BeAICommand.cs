using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameClient;

public enum AI_COMMAND
{
    WALK = 0,
    IDLE,
    ATTACK,
    SKILL,
    WALK_BACK,
    NONE,
}

public enum AI_COMMAND_STAT
{
    READY,
    RUNNING,
    STOP,
}

public enum AI_SPECIAL_SKILLID
{
	JUMP = 1,
	JUMP_BACK = 2,
	NORMAL_ATTACK_PRESS = 3,
	NORMAL_ATTACK_RELEASE = 4,
	USE_DRUG = 5,
}


public class AIInputData
{
	public struct Input
	{
		public int delay;
		public int skillID;
		public int pressTime;
		public int specialChoice;

		public Input(int sid, int d, int pt, int c)
		{
			delay = d;
			skillID = sid;
			pressTime = pt;
			specialChoice = c;
		}
	}

	public bool useAgent = false;
	//public List<Input> inputs = new List<Input>();
	public List<behaviac.Input> inputs = new List<behaviac.Input>();

	public AIInputData()
	{
		
	}

	public AIInputData(int sid, int d = 0, int pt=0, int c=0)
	{
		AddInput(sid, d, pt, c);
	}

    public void AddInput(int sid, int d = 0, int pt = 0, int c = 0, bool randomChangeDirection = false)
    {
        behaviac.Input _input = new behaviac.Input();
        _input.delay = d;
        _input.skillID = sid;
        _input.pressTime = pt;
        _input.specialChoice = c;
        _input.randomChangeDirection = randomChangeDirection;
        inputs.Add(_input);
    }

    public void AddInput(behaviac.Input input)
	{
		inputs.Add(input);
	}
}

public class BeAICommandPoolImp :   IObjectPool
{
	Dictionary<int/*AI_COMMAND*/, List<BeAICommand>> commandPool = new Dictionary<int, List<BeAICommand>> ();
	int newCount = 0;

	#region poolInfo

	string poolKey = "BeAICommandPool";
	string poolName = "AICommand池";

	int totalInst = 0;
	int remainInst = 0;

	public string GetPoolName ()
	{
		return poolName;
	}

	public string GetPoolInfo()
	{
		return string.Format ("{0}/{1}", remainInst, totalInst);
	}

	public string GetPoolDetailInfo ()
	{
		return "detailInfo";
	}
	#endregion

	public  void Init ()
	{
#if !SERVER_LOGIC 

		CPoolManager.GetInstance ().RegisterPool (poolKey, this);

 #endif

	}

	public BeAICommand GetAICommand(AI_COMMAND cmdType, BeEntity e)
	{
		int type = (int)cmdType;
		BeAICommand cmd = null;
		if (!commandPool.ContainsKey(type))
		{
			commandPool.Add (type, new List<BeAICommand> ());
		}

		var lst = commandPool [type];
		if (lst.Count > 0)
		{
			cmd = lst[lst.Count - 1];
			lst.RemoveAt (lst.Count - 1);
			remainInst--;
		}
		else
		{
			newCount++;
			totalInst++;

			//Logger.LogForAI ("aicommandpool newCount:{0} type:{1}", newCount, type);
			switch(cmdType)
			{
			case AI_COMMAND.ATTACK:
				cmd = new BeAIAttackCommand (e);
				break;
			case AI_COMMAND.IDLE:
				cmd = new BeAIIdleCommand (e);
				break;
			case AI_COMMAND.SKILL:
				cmd = new BeAISkillCommand (e);
				break;
			case AI_COMMAND.WALK:
				cmd = new BeAIWalkCommand (e);
				break;
			case AI_COMMAND.NONE:
				cmd = new BeAINoneCommand (e);
				break;
			}
		}

		//DebugPrint (string.Format("GetAICommand:{0}", type));

		cmd.Reset (e);
		return cmd;
	}

	public void PutAICommand(BeAICommand cmd)
	{
		if (!commandPool.ContainsKey((int)cmd.cmdType))
		{
#if UNITY_EDITOR  && !LOGIC_SERVER			
			Logger.LogErrorFormat("Can put back cmd!!!, type:{0}", cmd.cmdType);
#endif
			return;
		}

		if(commandPool[(int) cmd.cmdType].Contains(cmd))
		{
			return;
		}
		
		remainInst++;

		commandPool [(int)cmd.cmdType].Add (cmd);

		//DebugPrint ("PutAICommand");
	}

	public void Clear()
	{
		commandPool.Clear ();
		totalInst = 0;
		remainInst = 0;
	}

	public void DebugPrint(string pre)
	{
		string content = pre + "\n";
		foreach(var p in commandPool)
		{
			content += string.Format ("{0}:{1}\n", p.Key, p.Value.Count);
		}

		Logger.LogErrorFormat (content);
	}
}


[LoggerModel("AI")]
public class BeAICommand {
    public AI_COMMAND cmdType = AI_COMMAND.NONE;
    public int duraction = 0;
    protected AI_COMMAND_STAT state = AI_COMMAND_STAT.READY;
    public BeEntity entity;
    public BeAIManager aiManager;

    protected int timeAcc = 0;

    //指令优先级测试版，Command与AI冲突问题
    protected bool bCanClose = true;
    protected string debugInfo;


    public BeAICommand(BeEntity e,string debugInfo = "AICommand")
    {
        entity = e;
        aiManager = entity.aiManager;
        this.debugInfo = debugInfo;
    }

	public virtual void Reset(BeEntity e)
	{
		entity = e;
		aiManager = entity.aiManager;
		duraction = 0;
		state = AI_COMMAND_STAT.READY;
		timeAcc = 0;
	}

    public bool IsAlive()
    {
        return state == AI_COMMAND_STAT.RUNNING;
    }

    public bool IsCanClose()
    {
        return bCanClose;
    }
    public void Tick(int deltaTime)
    {
        if (state != AI_COMMAND_STAT.RUNNING)
            return;

        timeAcc += deltaTime;
        OnTick(deltaTime);

        if (timeAcc >= duraction)
        {
			OnFinish();
            End();
        }
    }

    public void Execute()
    {
        Logger.LogForAI("start ai command type:{0}", cmdType.ToString());
        state = AI_COMMAND_STAT.RUNNING;
        OnExecute();
    }

    public void End()
    {
	    state = AI_COMMAND_STAT.STOP;
		Logger.LogForAI("end ai command type:{0}", cmdType.ToString());
        //aiManager.DoNothing();
        OnEnd();

        aiManager.ClrCurrentCommand();
		entity.CurrentBeBattle.BeAICommandPool.PutAICommand (this);
    }

    public virtual void OnTick(int deltaTime)
    {
        //Logger.Log("OnTick from base BeAICommand.");
    }

    public virtual void OnExecute()
    {
        //Logger.Log("OnExecute from base BeAICommand.");
    }

    public virtual void OnEnd()
    {
        //aiManager.tree.StopAllCoroutines();
        //Logger.Log("OnEnd from base BeAICommand.");
    }

	public virtual void OnFinish()
	{
		
	}
    
    public virtual string DebugInfo()
    {
        return debugInfo;
    }

    public void SetDebugInfo(string debugInfo)
    {
        this.debugInfo = debugInfo;
    }
}

[LoggerModel("AI")]
public class BeAINoneCommand : BeAICommand
{
    public BeAINoneCommand(BeEntity e):base(e,"AINoneCommand")
    {
        cmdType = AI_COMMAND.NONE;
    }

	public void Init()
	{
		
	}

    public override void OnExecute()
    {
        if (entity != null)
        {
            entity.ResetMoveCmd();
            entity.dontSetFace = false;

            var actor = entity as BeActor;
            if (actor != null)
            {
                actor.ChangeRunMode(false);
                Logger.Log("execute none cmd");
            }
            else
            {
                Logger.LogError("entity is not BeActor!");
            } 
        }
        else
        {
            Logger.LogErrorFormat("entity is null!");
        }
    }
}

[LoggerModel("AI")]
public class BeAIIdleCommand : BeAICommand
{
    public BeAIIdleCommand(BeEntity e):base(e,"AIIdleCommand")
    {
		cmdType = AI_COMMAND.IDLE;
    }

	public void Init(int dur)
	{
		duraction = dur;
	}

    public override void OnExecute()
    {
        if (entity != null)
        {
            entity.Reset();
            Logger.LogFormat("execute idle cmd");
        }
        else
        {
            Logger.LogErrorFormat("entity is null!");
        }        
    }

	public override void OnFinish()
	{
		if (entity != null && entity.aiManager != null)
		{
			//IDLE完重置寻路
			if (entity.aiManager != null)
			{
				entity.aiManager.ResetDestinationSelect();
			}
		}
	}
}

#region temp
//临时用的给新手引导，后面修改 
public class BeAISimpleWalkCommand : BeAICommand
{
    public VInt3 targetPos;
    public VInt tolerance;
     

	public BeAISimpleWalkCommand(BeEntity e,VInt3 targetPosition, VInt tolerance):base(e,"AISimpleWalkCommand")
    {
        this.targetPos = targetPosition;
        this.tolerance = tolerance;
        bCanClose = false;
    }

    public override void OnExecute()
    {
        int x = entity.GetPosition().x - targetPos.x;
        if (x < 0)
        {
            aiManager.DoWalk(BeAIManager.MoveDir.RIGHT, true);
        }
        else 
        {
            aiManager.DoWalk(BeAIManager.MoveDir.LEFT, true);
        }

        bCanClose = false;
    }

 
    public override void OnTick(int deltaTime)
    {
        if (IsNearTargetPosition())
        {
            End();
			if (entity != null && entity.aiManager != null)
			{
				var aiManager = entity.aiManager as BeActorAIManager;
				if (aiManager != null)
				{
					aiManager.ResetDestinationSelect();
					//aiManager.owner.ClearMoveSpeed();
				}

			}
            //BehaviorManager.instance.RestartBehavior(aiManager.tree);
        }
    }

    public bool IsNearTargetPosition()
    {
        int distance = tolerance.i;
        VInt3 pos = entity.GetPosition();
        return (((Mathf.Abs(targetPos.x - pos.x)) <= distance) && (Mathf.Abs(targetPos.y - pos.y) <= distance));
    }
}
#endregion
public class BeAIWalkCommand : BeAICommand
{
    public VInt3 targetPos;
    public VInt tolerance;
    public bool moveBack;
    public bool isRun;
	public bool moveW = false;
	bool pathFinding = false;
    bool bypass = false;
	public bool stopable = true;

    int walkUpdateInterval = 500;
    int walkTimeAcc = 500;

	VInt3 lastPosition;
	bool start = false;
	int stuckTimeAcc = 0;
	int stuckInterval = 1000;

    List<int> steps = null;
    VInt3 lastPos;
    BeAIManager.MoveDir lastDir;
    public static readonly VInt2 grid = new VInt2(0.25f, 0.25f);

	bool walkFinish = false;

    public List<int> Steps => steps;
    public VInt3 LastPos => lastPos;

    public BeAIManager.DestinationType destinationType = BeAIManager.DestinationType.GO_TO_TARGET;

	public BeAIWalkCommand(BeEntity e):base(e,"AIWalkCommand")
    {
        
    }

	public override void Reset (BeEntity e)
	{
		base.Reset (e);
		targetPos = VInt3.zero;
		tolerance = 0;
		moveBack = false;
		isRun = false;
		pathFinding = false;
		bypass = false;
		moveW = false;
		stopable = true;
		walkUpdateInterval = 500;
		walkTimeAcc = 500;
		lastPosition = VInt3.zero;
		start = false;
		stuckTimeAcc = 0;
		stuckInterval = 1000;
		steps = null;
		lastPos = VInt3.zero;
		lastDir = BeAIManager.MoveDir.COUNT;
		destinationType = BeAIManager.DestinationType.GO_TO_TARGET;
		walkFinish = false;

	}

	public void Init(int dur, VInt3 targetPosition, VInt tolerance, bool run = false, bool moveBack = false, bool pathFinding = false, bool bypass = false, bool moveW = false)
	{
		duraction = dur;
		targetPos = targetPosition;

		VInt2 xrange = new VInt2(aiManager.owner.CurrentBeScene.sceneData.GetLogicXSize());
		VInt2 yrange = new VInt2(aiManager.owner.CurrentBeScene.sceneData.GetLogicZSize());

		targetPos.x = Mathf.Clamp(targetPos.x, xrange.x, xrange.y);
		targetPos.y = Mathf.Clamp(targetPos.y, yrange.x, yrange.y);

		this.tolerance = tolerance;
		this.moveBack = moveBack;
		isRun = run;
		cmdType = AI_COMMAND.WALK;

		this.bypass = bypass;
		if (this.bypass)
		{
			walkUpdateInterval = 2000;
			walkTimeAcc = walkUpdateInterval;

		}
		else if(moveW)
		{
			this.moveW = moveW;
			walkUpdateInterval = 2000;
			walkTimeAcc = walkUpdateInterval;
			stuckInterval = aiManager.FrameRandom.InRange(500, 2000);
		}


		lastDir = BeAIManager.MoveDir.COUNT;

		this.pathFinding = pathFinding;
		if (pathFinding)
		{
			var owner = aiManager.owner;

			VInt3 start = owner.GetPosition();

			DGrid startGrid = owner.CurrentBeScene.CalGridByPosition(start);
			DGrid targetGrid = owner.CurrentBeScene.CalGridByPosition(targetPos);

			bool ret = aiManager.DoPathFinding(startGrid, targetGrid, aiManager.steps);
			if (!ret)
			{
				this.pathFinding = false;
			}
			else
			{
				steps = aiManager.steps;

				if (/*RecordServer.GetInstance().IsRecord()*/owner!=null && owner.IsProcessRecord() )
				{
					
	                string path = string.Format("found path ==> count:{0}; values:", steps.Count);
	                for (int i = 0; i < steps.Count; ++i)
	                {
	                    path += string.Format("{0} ", steps[i]);
	                }
                    if (owner.GetRecordServer().IsProcessRecord())
                    {
                        owner.GetRecordServer().RecordProcess("[BATTLE]ai path:{0} {1}", path, owner.GetInfo());
                        owner.GetRecordServer().Mark(0x8779790, owner.GetEntityRecordAttribute(), path, owner.GetName());
                        // Mark:0x8779790 [BATTLE]ai path: {12} PID:{0},Name:{13},Pos:({1},{2},{3}),Speed:({4},{5},{6}),Face:{7},Hp:{8},Mp:{9},Flag:{10},curSkillId:{11}
                    }

                    //Logger.Log(path);
                    //Logger.LogForAI("path find steps:{0}", steps.Count);
                }


			}	
		}    
	}

    public override void OnExecute()
    {
        if (entity != null)
        {
            var actor = entity as BeActor;
            if (actor != null)
            {
				actor.ResetMoveCmd();
                actor.ChangeRunMode(isRun);
                DoNextMove();
            }
            else
            {
                Logger.LogError("entity is not BeActor!");
            }
        }
        else
        {
            Logger.LogErrorFormat("entity is null!");
        }
    }

    public bool DoNextMove()
    {
        if (pathFinding)
        {
            if (steps != null)
            {
				if (steps.Count <= 0)
                {
					walkFinish = true;
					End();
					return false;
				}

				var pos = aiManager.owner.GetPosition();
				if (aiManager.owner.CurrentBeScene != null)
					pos = aiManager.owner.CurrentBeScene.CalPositionToGridPosition(pos);
				lastPos = pos;//aiManager.owner.GetPosition();
                int dir = steps[0];
                steps.RemoveAt(0);
                lastDir = (BeAIManager.MoveDir)dir;

				var owner = aiManager.owner;

				if (owner != null && owner.IsProcessRecord())
				{
					owner.GetRecordServer().RecordProcess("[AI]PID:{0}-{1} do next move pos:({2},{3},{4}) dir:{5}", aiManager.owner.m_iID, aiManager.owner.GetName(),  lastPos.x, lastPos.y, lastPos.z, lastDir);
                    owner.GetRecordServer().Mark(0x8779791, new int[]
                    {
                        aiManager.owner.m_iID,
                        lastPos.x,
                        lastPos.y,
                        lastPos.z,
                        (int)lastDir
                    }, aiManager.owner.GetName());
                    // Mark:0x8779791 [AI]PID:{0}-{5} do next move pos:({1},{2},{3}) dir:{4}
                }


                Logger.LogFormat("do next move, pos:({0},{1},{2}), dir:{3}", lastPos.x, lastPos.y, lastPos.z, lastDir.ToString());
                aiManager.DoWalk(lastDir, true);

                return true;
            }
            else
            {
                Logger.LogError("do next move, steps is null!");
                return false;
            }
        }
		else if (bypass)
		{
            VInt3 curPos = aiManager.owner.GetPosition();

			if (lastDir == BeAIManager.MoveDir.COUNT)
			{
				BeAIManager.MoveDir nextDir = BeAIManager.MoveDir.RIGHT_TOP;

				if (targetPos.x >= curPos.x)
				{
					if (targetPos.y >= curPos.y )
						nextDir = BeAIManager.MoveDir.RIGHT_TOP;
					else 
						nextDir = BeAIManager.MoveDir.RIGHT_DOWN;
				}
				else
				{
					if (targetPos.y >= curPos.y )
						nextDir = BeAIManager.MoveDir.LEFT_TOP;
					else
						nextDir = BeAIManager.MoveDir.LEFT_DOWN;
				}

                Logger.LogFormat("do bypass, dir:{0}", nextDir.ToString());

				lastDir = nextDir;
				aiManager.DoWalk(nextDir, true);
			}
			else
			{
				BeAIManager.MoveDir nextDir = BeAIManager.MoveDir.RIGHT_TOP;
				if (targetPos.x >= curPos.x)
				{
					if (lastDir == BeAIManager.MoveDir.RIGHT_TOP)
						nextDir = BeAIManager.MoveDir.RIGHT_DOWN;
					else if (lastDir == BeAIManager.MoveDir.RIGHT_DOWN)
						nextDir = BeAIManager.MoveDir.RIGHT_TOP;
				}
				else
				{
					if (lastDir == BeAIManager.MoveDir.LEFT_DOWN)
						nextDir = BeAIManager.MoveDir.LEFT_TOP;
					else if (lastDir == BeAIManager.MoveDir.LEFT_TOP)
						nextDir = BeAIManager.MoveDir.LEFT_DOWN;
				}

                Logger.LogFormat("do bypass, dir:{0}", nextDir.ToString());

                lastDir = nextDir;
				aiManager.DoWalk(nextDir, true);
			}
		}
		else if(moveW)
		{
			VInt3 curPos = aiManager.owner.GetPosition();

			if (lastDir == BeAIManager.MoveDir.COUNT)
			{
				BeAIManager.MoveDir nextDir = BeAIManager.MoveDir.RIGHT_TOP;

				if (targetPos.x >= curPos.x)
				{
					if (targetPos.y >= curPos.y)
					{
						nextDir = BeAIManager.MoveDir.RIGHT_TOP;
					}
					else
					{
						nextDir = BeAIManager.MoveDir.RIGHT_DOWN;
					}
				}
				else
				{
					if (targetPos.y >= curPos.y)
					{
						nextDir = BeAIManager.MoveDir.LEFT_TOP;
					}
					else
					{
						nextDir = BeAIManager.MoveDir.LEFT_DOWN;
					}
				}

				//Logger.LogFormat("do bypass, dir:{0}", nextDir.ToString());

				lastDir = nextDir;
				aiManager.DoWalk(nextDir, true);
			}
			else
			{
				BeAIManager.MoveDir nextDir = BeAIManager.MoveDir.RIGHT_TOP;
				if (targetPos.x >= curPos.x)
				{
					if (targetPos.y <= curPos.y)
					{
						nextDir = BeAIManager.MoveDir.RIGHT_DOWN;
					}
					else
					{
						nextDir = BeAIManager.MoveDir.RIGHT_TOP;
					}
				}
				else
				{
					if (targetPos.y <= curPos.y)
					{
						nextDir = BeAIManager.MoveDir.LEFT_DOWN;
					}
					else
					{
						nextDir = BeAIManager.MoveDir.LEFT_TOP;
					}
				}

				//Logger.LogProcessFormat("do bypass, dir:{0}", PathUtility.GetDirString(nextDir));

				lastDir = nextDir;
				aiManager.DoWalk(nextDir, true);
			}
		}

        return false;
    }

	public void DoOppositeMove()
	{
		walkFinish = true;
		End();
	}

    public override void OnTick(int deltaTime)
    {
        if (pathFinding)
        {
	        VInt3 curPos = aiManager.owner.GetPosition();
	        if(aiManager.owner.CurrentBeScene != null)
				curPos = aiManager.owner.CurrentBeScene.CalPositionToGridPosition(curPos);
            VInt3 dis = curPos - lastPos;
            if (Mathf.Abs(dis.x) >= grid.x || Mathf.Abs(dis.y) >= grid.y)
            {
                DoNextMove();
            }
        }
		else if(moveW)
		{
			stuckTimeAcc += deltaTime;
			if (stuckTimeAcc > stuckInterval)
			{

				DoNextMove();

				stuckTimeAcc -= stuckInterval;
			}
		}
        else
        {
            walkTimeAcc += deltaTime;
            if (walkTimeAcc > walkUpdateInterval)
            {
                walkTimeAcc -= walkUpdateInterval;
                if (!bypass)
                    aiManager.DoWalk(targetPos);
            }
			CheckStuck(deltaTime);
        }

		

        if (IsNearTargetPosition())
        {
			walkFinish = true;
            End();
            if (entity != null && entity.aiManager != null)
			{
				var aiManager = entity.aiManager as BeActorAIManager;
				if (aiManager != null)
				{
					aiManager.ResetDestinationSelect();
				}
			}
        }
    }

	static readonly VInt tolerance001 = new VInt(0.01f);
	public void CheckStuck(int delta)
	{
		VInt3 pos = aiManager.owner.GetPosition();
		if (!start)
		{
			lastPosition = pos;
			start = true;
		}
		else {
			if (Mathf.Abs(pos.x - lastPosition.x)<tolerance001 && Mathf.Abs(pos.y - lastPosition.y)<tolerance001)
			{
				stuckTimeAcc += delta;
				if (stuckTimeAcc > stuckInterval)
				{
					//Logger.LogErrorFormat("stuck beyond time:{0}!!!!!!!!!!!!!!!!!!!!!!", stuckTimeAcc);

					if (bypass || moveW)
						DoNextMove();
					else
						DoOppositeMove();

					stuckTimeAcc = 0;
				}
			}
			else {
				lastPosition = pos;
				stuckTimeAcc = 0;
			}
		}
	}

    public bool IsNearTargetPosition()
    {
        int distance = tolerance.i;
        VInt3 pos = entity.GetPosition();
        return (((Mathf.Abs(targetPos.x - pos.x)) <= distance) && (Mathf.Abs(targetPos.y - pos.y) <= distance));
    }

	public override void OnFinish ()
	{
		if (entity != null && entity.aiManager != null)
		{
			//重置寻路
			if (entity.aiManager != null)
			{
				entity.aiManager.ResetDestinationSelect();
			}
		}
	}

    public override void OnEnd()
    {
        entity?.TriggerEventNew(BeEventType.onAIMoveEnd, new EventParam());
		if(walkFinish)
		{
			entity.ResetMoveCmd();
		}
    }
}

public class BeAIAttackCommand : BeAICommand
{
    public BeAIAttackCommand(BeEntity e):base(e,"AIAttackCommand")
    {
        cmdType = AI_COMMAND.ATTACK;
    }

	public void Init()
	{
		
	}

    public override void OnExecute()
    {
        if (entity != null)
        {
            int normalAttackID = entity.GetEntityData().normalAttackID;
            Logger.LogFormat("execute attack cmd, attack id:{0}", normalAttackID);
            var actor = entity as BeActor;
            if (actor != null && actor.CanUseSkill(normalAttackID))
            {
                Logger.Log("do it");
                BeStateData state = new BeStateData((int)ActionState.AS_CASTSKILL) { _StateData = normalAttackID };
                entity.Locomote(state);
            }
        }
    }
}

public class BeAISkillCommand : BeAICommand
{
	public int skillTimeAcc = 0;
    public int skillID;
	public AIInputData inputSeq = null;
	public int inputExecuteCount = 0;

    public BeAISkillCommand(BeEntity e) : base (e,"AISkillCommand")
	{
		

/*		string tmp = "";
		for(int i=0; i<inputData.inputs.Count; ++i)
		{
			tmp += " " + inputData.inputs[i].skillID+"_"+inputData.inputs[i].delay;
		}
		Logger.LogErrorFormat("BeAISkillCommand {0}", tmp);*/
	}

	public override void Reset (BeEntity e)
	{
		base.Reset (e);
		skillTimeAcc = 0;
		skillID = 0;
		inputSeq = null;
		inputExecuteCount = 0;
	}

	public void Init(AIInputData inputData)
	{
		cmdType = AI_COMMAND.SKILL;
		inputSeq = inputData;
		duraction = 10000;
    }


	/*
    public BeAISkillCommand(BeEntity e, int si) : base(e)
    {
		duraction = 10000;
        cmdType = AI_COMMAND.SKILL;
        skillID = si;

		inputSeq = new AIInputData(si, 0);
    }*/
				
	public override void OnTick(int deltaTime)
	{
		if (inputSeq.inputs.Count <= 0) {
			duraction = 0;
			return;
		}

		skillTimeAcc += deltaTime;
		if (skillTimeAcc >= inputSeq.inputs[0].delay)
		{
			skillTimeAcc -= inputSeq.inputs[0].delay;
			ExecuteInput(deltaTime);
		}

	}

	public override void OnEnd ()
	{
		base.OnEnd ();
		var actor = entity as BeActor;
		if (actor != null)
			actor.SetAttackButtonState(ButtonState.RELEASE);
	}

	void ExecuteInput(int deltaTime)
	{
		if (inputSeq.inputs.Count <= 0)
			return;

		var inputData = inputSeq.inputs[0];
        inputSeq.inputs.RemoveAt(0);
		inputExecuteCount++;
		skillTimeAcc = 0;

        //Logger.LogForAI("execute input {0} at {1}", inputData.skillID, timeAcc);

        /*		string tmp = "";
                for(int i=0; i<inputSeq.inputs.Count; ++i)
                {
                    tmp += " " + inputSeq.inputs[i].skillID;
                }
                Logger.LogErrorFormat("execute input {0} at {1} list:{2}", inputData.skillID, Time.realtimeSinceStartup, tmp);*/

        int skillID = inputData.skillID;

		if (entity != null && skillID > 0)
		{
			Logger.LogForAI("execute skill cmd, skill id:{0}", skillID);
			BeEntity aiTarget = entity.aiManager.aiTarget;
			if (aiTarget != null)
			{
				Logger.Log("set face");
				if (inputExecuteCount == 1)
					entity.SetFace(aiTarget.GetPosition().x < entity.GetPosition().x, true);
			}

			var actor = entity as BeActor;
			if (actor != null)
			{
                BeSkill beSkill = actor.GetSkill(skillID);

                //决斗场机器人
                if (actor.isPkRobot && inputData.PKRobotComboCheck)
                {
                    //当前技能处于CD中
                    bool stopFlag = false;
                    if (beSkill != null && beSkill.isCooldown)
                        stopFlag = true;
                    if (aiTarget != null && (!aiTarget.HasTag((int)AState.ACS_FALL) || aiTarget.HasTag((int)AState.AST_FALLGROUND)))
                        stopFlag = true;
                    if (stopFlag)
                    {
                        aiManager.StopCurrentCommand();
                        aiManager.pkRobotWander = true;
                        aiManager.ResetDestinationSelect();
                        return;
                    }
                }

                if (actor.aiManager != null && !actor.aiManager.CanAIUseSkill(skillID))
				{					
					return;
				}

                bool forceCastSkill = false;
                bool isComboSkill = actor.IsComboSkill(skillID);
                if (beSkill != null && isComboSkill)
                {                                                         
                    if (beSkill.comboSkillSourceID != skillID)
                    {
                        BeSkill curSkill = actor.GetCurrentSkill();
                        if (curSkill != null)
                            forceCastSkill = curSkill.comboSkillSourceID == beSkill.comboSkillSourceID;
                        if (inputData.randomChangeDirection)
                        {
                            actor.SetFace(actor.FrameRandom.Range100() >= 50, true);
                        }
                    }
                }

                if (inputExecuteCount > 1 && !forceCastSkill)
                {
                    if (skillID > (int)AI_SPECIAL_SKILLID.USE_DRUG && !actor.aiManager.CanUseSkill(skillID))
                    {
                        bool nextJump = false;
                        if (actor.sgGetCurrentState() == (int)ActionState.AS_CASTSKILL)
                        {
                            var skill = actor.GetSkill(skillID);
                            if (skill != null && actor.GetCurSkillID() == skillID)
                            {
                                if (skill.buttonState != ButtonState.RELEASE)
                                {
                                    nextJump = true;
                                }
                            }
                        }

                        if (!nextJump)
                        {
                            duraction = 0;
                            return;
                        }
                    }

                }

                //判断无色晶体技能是否可以使用
                if (!actor.aiManager.CanCost(skillID))
                    return;

                if (actor.sgGetCurrentState() == (int)ActionState.AS_CASTSKILL)
                {
                    BeSkill curSkill = actor.GetCurrentSkill();
                    BeSkill _skill = actor.GetSkill(skillID);
                    if (curSkill != null && _skill != null)
                    {
                        //if (!curSkill.CanInterrupt(skillID) && curSkill.comboSkillSourceID != _skill.comboSkillSourceID)
                        //修改技能打断
                        if (!_skill.CanInterrupt(curSkill.skillID) || curSkill.comboSkillSourceID != _skill.comboSkillSourceID)
                        {
                            return;
                        }
                    }
                }

                actor.ResetMoveCmd();

				if (inputData.skillID == (int)AI_SPECIAL_SKILLID.JUMP)//1
				{
					if (actor.CanJump())
					{
						BeStateData state = new BeStateData((int)ActionState.AS_JUMP);
						actor.Locomote(state);
					}
				}
				else if (inputData.skillID  == (int)AI_SPECIAL_SKILLID.JUMP_BACK)//2
				{
					if (actor.CanJumpBack())
					{
						BeStateData state = new BeStateData((int)ActionState.AS_JUMPBACK);
						actor.Locomote(state);
					}
				}
				else if (inputData.skillID  == (int)AI_SPECIAL_SKILLID.NORMAL_ATTACK_PRESS)//3
				{
					actor.SetAttackButtonState(ButtonState.PRESS);
                    if (inputData.PKRobotComboCheck)
                        actor.SetAttackCheckFlag(true);
                }
				else if (inputData.skillID  == (int)AI_SPECIAL_SKILLID.NORMAL_ATTACK_RELEASE)//4
				{
					actor.SetAttackButtonState(ButtonState.RELEASE);
                    actor.SetAttackCheckFlag(false);
                }
				else if (inputData.skillID  == (int)AI_SPECIAL_SKILLID.USE_DRUG)
				{
					if (actor.isLocalActor)
					{
#if !LOGIC_SERVER
                        var battleUI = BattleUIHelper.GetBattleUIComponent<BattleUIDrug>();
                        if (battleUI != null)
                            battleUI.UseDrug();
#endif
                    }
                }
				//随从技能
				else if (inputData.skillID == Global.HELP_SKILL_ID)
				{
					if (actor.CanUseSkill(inputData.skillID))
						actor.UseHelpSkill();
				}
				else if (inputData.specialChoice > 0)
				{
					var skill = actor.GetSkill(skillID);
					if (skill != null)
					{
						skill.specialChoice = inputData.specialChoice - 1;
					}
					actor.UseSkill(skillID);
				}
				else {

					actor.SetAttackButtonState(ButtonState.RELEASE);

					bool flag = false;
					if (actor.sgGetCurrentState() == (int)ActionState.AS_CASTSKILL)
					{
						var skill = actor.GetSkill(skillID);

                        if (skill != null && skill.buttonState == ButtonState.RELEASE)
                        {
                            if (actor.TriggerComboSkills(skillID))
                                return;
                        }

                        if (skill != null && actor.GetCurSkillID()== skillID)
						{
							if (skill.buttonState == ButtonState.RELEASE)
							{
								skill.SetButtonPressAgain();
								actor.GetStateGraph().FireEvents2CurrentStates((int)EventCommand.EVENT_COMMAND_PRESS_BUTTON_AGAIN);
								flag = true;
							}
							else {
								skill.SetButtonRelease();
								actor.GetStateGraph().FireEvents2CurrentStates((int)EventCommand.EVENT_COMMAND_RELEASE_SKILL_BUTTON);
								flag = true;
							}
						}
					}

					if (!flag) {

                        actor.UseSkill(skillID, forceCastSkill);   
						var skill = actor.GetSkill(skillID);
						if (skill != null)
						{
                            if (inputData.pressTime <= 0)
                            {
                                skill.SetButtonRelease();
                                // 
                                if (skill.joystickMode == SkillJoystickMode.FREE)
                                {
                                    actor.delayCaller.DelayCall(100, () =>
                                    {
                                        actor.GetStateGraph().FireEvents2CurrentStates((int)EventCommand.EVENT_COMMAND_RELEASE_SKILL_BUTTON);
                                    });

                                }
                            }
                        }
					}
				}
			}
		}
	}
}
