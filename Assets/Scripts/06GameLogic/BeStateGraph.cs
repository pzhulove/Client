using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Diagnostics;
using GameClient;

//public class BeEntity;

public enum BeStatesPhase
{
    SSTATESGRAPH_NONE = 0,
    SSTATESGRAPH_RUN = 1,
    SSTATESGRAPH_PAUSE = 2,
    SSTATESGRAPH_STOP = 3
}

public class BeStatesGraph
{
	
    protected BeStatesPhase m_iGraphState;
    protected List<BeStates> m_vkStates = new List<BeStates>();

    protected uint m_uiCurrentTimeSinceStart;
    protected uint m_uiCurrentStatesTime;
    protected int m_iCurrentStatesIndex;
    protected uint m_uiTimeSinceStart;

    protected BeStateData m_kCurState;
    protected List<BeStateData> m_vStateStack = new List<BeStateData>();

    public int m_lastState;

    //protected THandle m_kHandle;
    public BeEntity m_pkEntity;

#if UNITY_EDITOR && !LOGIC_SERVER
    public class StateRecordInfo
    {
        public string content;
        public bool isShow;
        public StackFrame[] frames;
    }
	public Queue<StateRecordInfo> statesChangeRecord = new Queue<StateRecordInfo>();
    public List<string> skillUseRecord = new List<string>();
#endif

    public static string[] ActionNames =
    {
        "AS_IDLE",
        "AS_ATTACK",
        "AS_WALK",
        "AS_RUN",
        "AS_HURT",
        "AS_JUMP",
        "AS_JUMPBACK",
        "AS_RUNATTACK",
        "AS_JUMPATTACK",
        "AS_FALL",
        "AS_FALLCLICK",
        "AS_FALLGROUND",
        "AS_SKILL",
        "AS_BUSY",
        "AS_CASTSKILL",
        "AS_GRAPED",
        "AS_DEAD",
        "AS_GETUP",
        "AS_BIRTH",
        "AS_WIN",
		"AS_ROLL"
    };

    public static int SGStatesID(ref BeStates pkState)
    {
        return pkState.iStateID;
    }

    public static void SGAddEventHandler2States(BeStates pkState, BeEventsHandler evenhandler)
    {
        pkState.OnEventHandler.Add(evenhandler);
    }

    public static void SGAddTimeLineHandler2States(BeStates pkState, BeTimeLineHandler timelines)
    {
        pkState.OnTimeLineHandler.Add(timelines);
    }

    public BeStatesGraph()
    {
        Logger.Log("this is BeStatesGraph1 constructor");
    }

	public void Reset()
	{
		/*
		 * protected BeStatesPhase m_iGraphState;
    protected List<BeStates> m_vkStates = new List<BeStates>();

    protected uint m_uiCurrentTimeSinceStart;
    protected uint m_uiCurrentStatesTime;
    protected int m_iCurrentStatesIndex;
    protected uint m_uiTimeSinceStart;

    protected BeStateData m_kCurState;
    protected List<BeStateData> m_vStateStack = new List<BeStateData>();

    //protected THandle m_kHandle;
    public BeEntity m_pkEntity;
		*/
		m_iGraphState = BeStatesPhase.SSTATESGRAPH_NONE;
		//m_vkStates.Clear ();
		m_uiCurrentTimeSinceStart = 0;
		m_uiCurrentStatesTime = 0;
		m_iCurrentStatesIndex = 0;
		m_uiTimeSinceStart = 0;
		m_vStateStack.Clear ();

	}

    public virtual bool Start(int iStartState = 0)
    {
        if (m_iGraphState == BeStatesPhase.SSTATESGRAPH_RUN)
        {
            return false;
        }

        if (SwitchStates(new BeStateData(iStartState)))
        {
            m_iGraphState = BeStatesPhase.SSTATESGRAPH_RUN;
            return true;
        }

        return false;
    }

    public virtual void Stop()
    {
        m_iGraphState = BeStatesPhase.SSTATESGRAPH_STOP;
    }

    public BeStatesPhase GetGraphState()
    {
        return m_iGraphState;
    }

    public int GetCurrentState()
    {
        BeStates state = GetCurrentStateData();
        return state.iStateID;
    }

    public BeStates GetCurrentStateData()
    {
        return _getCurrentStateData();
    }

    public uint GetCurrentStatesTime()
    {
        return m_uiCurrentStatesTime;
    }

    public uint GetCurrentTimeSinceStart()
    {
        return m_uiCurrentTimeSinceStart;
    }

    public virtual void InitStatesGraph()
    {
        Logger.Log("child class willl implement this function!!!!");
    }

    public virtual void UpdateStates(int iDeltaTime)
    {

        if (GetGraphState() != BeStatesPhase.SSTATESGRAPH_RUN)
        {
            return;
        }

        if (iDeltaTime < 0)
            iDeltaTime = 0;

        m_uiCurrentStatesTime += (uint)iDeltaTime;
        m_uiCurrentTimeSinceStart += (uint)iDeltaTime;
        m_uiTimeSinceStart += (uint)iDeltaTime;

        BeStates rkState = GetCurrentStateData();

		if (rkState.OnTick != null)
        {
            rkState.OnTick(rkState, iDeltaTime);
        }

        if (rkState.uiTimeOut != -1 && m_uiCurrentStatesTime >= rkState.uiTimeOut)
        {
            if (rkState.OnTimeOut != null)
            {
                Logger.Log("time out!!!! time:" + m_uiCurrentStatesTime + " state name:" + ActionNames[rkState.iStateID] + " ID:" + m_pkEntity.m_iID);
                rkState.OnTimeOut(rkState);
            }
            else
            {
                LocomoteState();
            }
        }
    }

	public void ResetCurrentStateTime(bool force = false)
	{
		BeStates rkState = GetCurrentStateData();
		if (rkState.uiTimeOut != -1 || force)
		{
			m_uiCurrentStatesTime = 0;
		}
			
	}

    public virtual bool SwitchStates(BeStateData s)
    {
        BeStateData rkStateData = s;

        for (int i = 0; i < m_vkStates.Count; ++i)
        {
            BeStates rkState = m_vkStates[i];
            if (rkState.iStateID == rkStateData._State)
            {
                BeStates pkCurrentState = GetCurrentStateData();

                //检查优先级
                //if (pkCurrentState.priority > rkState.priority || (pkCurrentState.priority == rkState.priority && !pkCurrentState.canCover))
                //{
                //	Logger.LogErrorFormat("can't switch from {0} to {1}", (ActionState)pkCurrentState.iStateID, (ActionState)rkState.iStateID);
                //	Logger.LogErrorFormat("priority cur {0} -> to {1}", pkCurrentState.priority, rkState.priority);
                //	return false;
                //}

                if (pkCurrentState != null)
                {
                    if (pkCurrentState.OnLeave != null)
                        pkCurrentState.OnLeave(pkCurrentState, rkStateData._State);
                }

                m_lastState = pkCurrentState.iStateID;

                var targetStateIndex = i;

				pkCurrentState.uiTimeOut = -1;
				rkState.uiTimeOut = -1;

                m_iCurrentStatesIndex = i;
                m_uiCurrentStatesTime = 0;
                m_uiCurrentTimeSinceStart = 0;
                m_pkEntity.ClearState();
                
                m_pkEntity.TriggerEventNew(BeEventType.onStateChange, new EventParam(){ m_Int = rkState.iStateID });

				if (m_pkEntity.IsProcessRecord())
				{
					var pos = m_pkEntity.GetPosition();


					string hpInfo = "";
					if (m_pkEntity.GetEntityData() != null)
					{
						hpInfo = string.Format("hp:{0} mp:{1}", m_pkEntity.GetEntityData().GetHP(), m_pkEntity.GetEntityData().GetMP());
					}
                    if (m_pkEntity.GetRecordServer().IsProcessRecord())
                    {
                        m_pkEntity.GetRecordServer().RecordProcess("PID:{0}-{1} SWITCH STATE {2} pos:({3},{4},{5}) speed:({11},{12},{13}) face:{6} duration:{7} force:{8} forcexy({9},{10}), {14},{15}",
                            m_pkEntity.m_iID, m_pkEntity.GetName(), ActionNames[rkState.iStateID],
                            pos.x, pos.y, pos.z, m_pkEntity.GetFace(), rkStateData._timeout, rkStateData.isForceSwitch, m_pkEntity.forceX, m_pkEntity.forceY,
                            m_pkEntity.moveXSpeed, m_pkEntity.moveYSpeed, m_pkEntity.moveZSpeed, hpInfo, m_pkEntity.GetAllStatTag()
                        );
                        m_pkEntity.GetRecordServer().Mark(0x8779803, new int[]
                      {
                            m_pkEntity.m_iID, pos.x, pos.y, pos.z,m_pkEntity.moveXSpeed.i, m_pkEntity.moveYSpeed.i, m_pkEntity.moveZSpeed.i,
                            m_pkEntity.GetFace() ? 1:0, rkStateData._timeout,
                            rkStateData.isForceSwitch ? 1: 0, m_pkEntity.forceX.i, m_pkEntity.forceY.i,m_pkEntity.GetAllStatTag(),
                            m_pkEntity.attribute != null ?m_pkEntity.attribute.GetHP() :0,
                            m_pkEntity.attribute != null ?m_pkEntity.attribute.GetMP() :0,
                      }, ActionNames[rkState.iStateID], m_pkEntity.GetName());
                        // Mark:0x8779803 PID:{0}-{16} SWITCH STATE {15} pos:({1},{2},{3}) speed:({4},{5},{6}) face:{7} duration:{8} force:{9} forcexy({10},{11}),{12},hp:{13} mp:{14}

                    }
                }

				#if UNITY_EDITOR  && !LOGIC_SERVER
                    if (Global.Settings.isDebug)
                    {
                        //Logger.LogErrorFormat("StackTrace!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!\n");
                        if ((m_pkEntity as BeActor) != null)
                        {
                            var pos = m_pkEntity.GetPosition();
                            string log = string.Format("[{0}]:  pos:({1},{2},{3}) speed:({10},{11},{12}) face:{4} duration:{6} time:{5} force:{7} forcexy({8},{9})",
                                ActionNames[rkState.iStateID], pos.x, pos.y, pos.z, m_pkEntity.GetFace(), 
                                Time.realtimeSinceStartup, rkStateData._timeout, rkStateData.isForceSwitch,m_pkEntity.forceX, m_pkEntity.forceY,
                                m_pkEntity.moveXSpeed, m_pkEntity.moveYSpeed, m_pkEntity.moveZSpeed
                            );
                            StackTrace st = new StackTrace(true);
                            StateRecordInfo info = new StateRecordInfo();
                            info.content = log;
                            info.frames = st.GetFrames();
                            statesChangeRecord.Enqueue(info);
                           
                            while (statesChangeRecord.Count > 10)
                            {
                                statesChangeRecord.Dequeue();
                            }
                        }
                    }
#endif

#if LOGIC_SERVER
                if (m_pkEntity.CurrentBeBattle != null && rkState.iStateID == (int)ActionState.AS_GETUP && m_pkEntity.CurrentBeBattle.GetBattleType() == BattleType.PVP3V3Battle && m_pkEntity.attribute != null && m_pkEntity.attribute.GetHP() <= 0)
                {
                    
                    if (m_pkEntity.GetRecordServer() != null && m_pkEntity.IsProcessRecord())
                    {
                        m_pkEntity.GetRecordServer().RecordProcess("[Enter GetUP]:{0}", RecordServer.GetStackTraceModelName());
                    }
                }
#endif

                /*				if (m_pkEntity.m_iID != 1 && (m_pkEntity as BeActor) != null && m_pkEntity.GetName()=="Saigehate")
                                {
                                    var pos = m_pkEntity.GetPosition();
                                    Logger.LogErrorFormat("switch states to {0} pos:({1},{2},{3}) face:{4} time:{5}", ActionNames[rkState.iStateID], pos.x, pos.y, pos.z, m_pkEntity.GetFace(), Time.realtimeSinceStartup);
                                }
                */
                rkState.kExTags = new SeFlag(rkStateData._ExTag);

				//是否是强制切换
				rkState.isForceSwitch = rkStateData.isForceSwitch;


                if (rkState.OnEnter != null)
                    rkState.OnEnter(rkState);

				if (rkStateData._timeout > 0)
				{
					//如果还是原来的状态才切换
					if(targetStateIndex == m_iCurrentStatesIndex)
						SetCurrentStatesTimeout(rkStateData._timeout, rkStateData._timeoutForce);
				}

                m_pkEntity.TriggerEventNew(BeEventType.onStateChangeEnd, new EventParam(){m_Int = rkState.iStateID });

                break;
            }
        }

        return true;
    }

    public virtual void FireEvents2CurrentStates(int iEvents)
    {
        BeStates rkState = GetCurrentStateData();
        for (int i = 0; i < rkState.OnEventHandler.Count; ++i)
        {
            var it = rkState.OnEventHandler[i];
            if (it.iEvents == iEvents)
            {
                if (it.onEvent != null)
                    it.onEvent(rkState);
                return;
            }
        }
    }

    public virtual void AddStates2Graph(BeStates rkStates)
    {
        rkStates.pkGraph = this;
        m_vkStates.Add(rkStates);
    }

    public virtual void Locomote(BeStateData rkStateData, bool bForce = false)
    {

    }

    public void SetCurrentStatesTimeout(int uiTimes2Out, bool force = false)
    {
        if (uiTimes2Out == -1)
        {
            uiTimes2Out = 2147483647;
        }

        BeStates pkState = GetCurrentStateData();
        if (pkState != null)
        {
            bool flag = true;

            if (force)
            {
                pkState.uiTimeOut = uiTimes2Out;
                return;
            }

            BDEntityActionInfo actionInfo = m_pkEntity.m_cpkCurEntityActionInfo;
            if (actionInfo != null)
            {
                if (actionInfo.skillTotalTime > 0)
                {
                    var param = m_pkEntity.TriggerEventNew(BeEventType.onChangeSkillTime, new EventParam(){ m_Vint3  = new VInt3(actionInfo.skillID, actionInfo.skillTotalTime, GlobalLogic.VALUE_1000) });
                    pkState.uiTimeOut = param.m_Vint3.y * VFactor.NewVFactor(param.m_Vint3.z, GlobalLogic.VALUE_1000);

                    flag = false;
                }
            }

            if (flag || force)
            {
                pkState.uiTimeOut = uiTimes2Out;
            }

        }
    }

    public bool CurrentStateHasTag(int iTag)
    {
        BeStates pkState = GetCurrentStateData();
        if (pkState != null)
        {
            return pkState.kTags.HasFlag(iTag);
        }
        return false;
    }

    public void SetCurrentStateTag(int iTag, bool bSet = true)
    {
        BeStates pkState = GetCurrentStateData();
        if (pkState != null)
        {
            if (bSet)
                pkState.kTags.SetFlag(iTag);
            else
                pkState.kTags.ClearFlag(iTag);
        }
    }

    public void ResetCurrentStateTag()
    {
        BeStates pkState = GetCurrentStateData();
        if (pkState != null)
        {
            pkState.kTags.Clear();
        }
    }

    public void ResetStateTag(int iStat)
    {
        for (int i = 0; i < m_vkStates.Count; ++i)
        {
            if (m_vkStates[i].iStateID == iStat)
            {
                m_vkStates[i].kTags.Clear();
            }
        }
    }

    public bool CurrentStateHasExTag(int iTag)
    {
        BeStates pkState = GetCurrentStateData();
        if (pkState != null)
        {
            return pkState.kExTags.HasFlag(iTag);
        }
        return false;
    }

    public void SetCurrentStateExTag(int iTag, bool bSet = true)
    {
        BeStates pkState = GetCurrentStateData();
        if (pkState != null)
        {
            if (bSet)
                pkState.kExTags.SetFlag(iTag);
            else
                pkState.kExTags.ClearFlag(iTag);
        }
    }

    public void ResetCurrentStateExTag(int iTag)
    {
        BeStates pkState = GetCurrentStateData();
        if (pkState != null)
        {
            pkState.kExTags.Clear();
        }
    }

    //正常出栈
    public virtual void LocomoteState()
    {
        BeStateData rkData = PopState();
        Locomote(rkData, true);
    }

    public bool HasStateInStack(int state)
    {
        for(int i=0; i<m_vStateStack.Count; ++i)
        {
            if (m_vStateStack[i]._State == state)
                return true;
        }

        return false;
    }

    public void PushState(ref BeStateData rkData)
    {
        m_vStateStack.Add(rkData);
    }

    public BeStateData PopState()
    {
        BeStateData data = new BeStateData();
        if (m_vStateStack.Count > 0)
        {
            data = m_vStateStack[m_vStateStack.Count - 1];
            m_vStateStack.RemoveAt(m_vStateStack.Count - 1);

        }

        return data;
    }

    public BeStateData GetTopState()
    {
        BeStateData data = new BeStateData();
        if (m_vStateStack.Count > 0)
        {
            data = m_vStateStack[m_vStateStack.Count - 1];

        }

        return data;
    }

    public void ClearStateStack()
    {
        m_vStateStack.Clear();
    }

    protected BeStates _getCurrentStateData()
    {
        return m_vkStates[m_iCurrentStatesIndex];
    }

}
