using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public struct BeTimeLineHandler
{
    public delegate void Del(BeStates state);

    public int iFrames;
    public Del onFrame;

    public BeTimeLineHandler(int frames, Del func)
    {
        iFrames = frames;
        onFrame = func;
    }
}

public struct BeEventsHandler
{
    public delegate void Del(BeStates state);

    public int iEvents;
    public Del onEvent;

    public BeEventsHandler(int events, Del func)
    {
        iEvents = events;
        onEvent = func;
    }
}

public class BeStates {

    public delegate void Del(BeStates state);
    public delegate void Del2(BeStates state, int t);

    public int iStateID;
    public int uiTimeOut;
    public SeFlag kTags;
    public SeFlag kExTags;
    public BeStatesGraph pkGraph;

    public Del OnEnter;
    public Del2 OnLeave;
    public Del OnTimeOut;
    public Del2 OnTick;

    public List<BeTimeLineHandler> OnTimeLineHandler = new List<BeTimeLineHandler>();
    public List<BeEventsHandler> OnEventHandler = new List<BeEventsHandler>();

	public int priority;
	public bool canCover;

	public bool isForceSwitch;


    public BeStates(int stateid, int tag = 0, 
        Del enterfunc = null,
        Del timeoutfunc = null,
        Del2 tickfunc = null, 
        Del2 leavefunc = null
        )
    {
        iStateID = stateid;
        kTags = new SeFlag(tag);
        kExTags = new SeFlag();
        pkGraph = null;
        uiTimeOut = -1;

        OnEnter = enterfunc;
        OnLeave = leavefunc;
        OnTimeOut = timeoutfunc;
        OnTick = tickfunc;

		priority = 1;
		canCover = true;
    }


}

public struct BeStateData
{
    public BeStateData(int iState)
    {
        _State = iState;
        _StateData = 0;
        _StateData2 = 0;
        _StateData3 = 0;
        _StateData4 = 0;
        _StateData5 = 0;
        _StateData6 = 0;
        _StateData7 = 0;
        _HurtAction = 0;
        _ExTag = 0;
        _timeout = 0;
        _timeoutForce = false;
        isForceSwitch = false;
    }

    //public BeStateData(
    //    int iState,
    //    int iStateData,
    //    int fStateData2,
    //    int fStateData3,
    //    int fStateData4,
    //    int iExTag,
    //    int fTimeout = 0,
    //    bool timeOutForce = false)
    //{
    //    _State = iState;
    //    _StateData = iStateData;
    //    _StateData2 = fStateData2;
    //    _StateData3 = fStateData3;
    //    _StateData4 = fStateData4;
    //    _ExTag = iExTag;
    //    _timeout = fTimeout;
    //    _timeoutForce = timeOutForce;
    //    isForceSwitch = false;
    //}

    public int  _State;
    public int  _StateData;
    public int  _StateData2;
    public int  _StateData3;
    public int  _StateData4;
    public int  _StateData5;
    public int  _StateData6;
    public int  _StateData7;
    public int  _HurtAction;
    public int  _ExTag;
    public int  _timeout;
    public bool _timeoutForce;
	public bool isForceSwitch;
}
