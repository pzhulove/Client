using GamePool;
using Network;
using Protocol;
using System;
using System.Collections;
using System.Collections.Generic;

interface MessageUnit
{
    void Start();
    void End();
    void Update(int deltaTime);
    bool CanRemove();
    void OnRecycle();
}

public class MessageSender
{
    List<MessageUnit> trunkList = new List<MessageUnit>();
    List<MessageUnit> pendingList = new List<MessageUnit>();

    public MessageSender()
    {

    }

    public void SendMessage<T0, T1>(T0 _request, T1 _result, ServerType _serverType, Action<MsgDATA, object> _action = null, object _userData = null, bool _needResend = false, int _timeOut = 3500)
        where T0 : class, Protocol.IProtocolStream, Protocol.IGetMsgID
        where T1 : class, Protocol.IProtocolStream, Protocol.IGetMsgID
    {
        var item = new MessageSenderUnit<T0,T1>();
        item.Init(_request, _result, _serverType, _action, _userData, _needResend, _timeOut);
        item.Start();
        pendingList.Add(item);
    }

    public void Update(int deltaTime)
    {
        if(pendingList.Count > 0)
        {
            trunkList.AddRange(pendingList);
            pendingList.Clear();
        }

        bool bCanRemove = false;
        for(int i = 0; i < trunkList.Count; ++i)
        {
            var item = trunkList[i];

            if(item == null)
            {
                continue;
            }

            item.Update(deltaTime);
            if (item.CanRemove())
            {
                bCanRemove = true;
            }
        }

        if (bCanRemove)
        {
            RemoveCanRemoveUnit(trunkList);
        }
    }

    public void Clear()
    {
        _ClearMessageList(trunkList);
        _ClearMessageList(pendingList);
    }

    private void _ClearMessageList(List<MessageUnit> list)
    {
        for(int i = 0; i < list.Count; ++i)
        {
            list[i].End();
        }
        list.Clear();
    }

    private void RemoveCanRemoveUnit(List<MessageUnit> list)
    {
        for (int i = list.Count - 1; i >= 0; --i)
        {
            MessageUnit cur = list[i];
            if (cur.CanRemove())
            {
                list.RemoveAt(i);
            }
        }
    }
}

public class MessageSenderUnit<T0,T1>: MessageUnit
    where T0 : class, Protocol.IProtocolStream, Protocol.IGetMsgID
    where T1 : class, Protocol.IProtocolStream, Protocol.IGetMsgID
{
    private const int kWaitCount = 1;
    private const int kResendCount = 2;
    private const int kResendTimeout = 3500;

    //send&receive
    private T0 mRequest;
    private T1 mResult;
    private ServerType mServerType;
    private Action<MsgDATA,object> mReceiveCallBack;
    private List<MsgDATA> mDataList;
    private int mSendResult;
    private object mUserData;

    //resend
    private bool mNeedResend;
    private int mWaitTime;
    private int mTempWaitTime;
    private int mTempWaitCount;
    private int mTempResendCount;

    private bool canRemove;

    private Action<MsgDATA> OnMessageReceive;

    public MessageSenderUnit()
    {
        mDataList = new List<MsgDATA>();
        mSendResult = -1;

        OnMessageReceive = new Action<MsgDATA>(_OnMessageReceived);
    }

    public void Init(T0 _req, T1 _res, ServerType _serverType, Action<MsgDATA,object> _action = null, object _userData = null, bool _resend = false,int _timeOut = kResendTimeout)
    {
        mRequest = _req;
        mResult = _res;
        mServerType = _serverType;
        mReceiveCallBack = _action;
        mNeedResend = _resend;
        mUserData = _userData;

        mTempWaitTime = _timeOut < 0 ? kResendTimeout : _timeOut;

        mDataList.Clear();
        mSendResult = -1;
        canRemove = false;

        Register();
    }

    public void Start()
    {
        if(mRequest != null)
        {
            mSendResult = NetManager.Instance().SendCommand(mServerType, mRequest);
        }
    }

    public void End()
    {
        canRemove = true;

        UnRegister();

        if (IsMessageReceived())
        {
            mReceiveCallBack(mDataList[0], mUserData);
        }
    }

    public void OnRecycle()
    {
        mRequest = null;
        mResult = null;
        mServerType = ServerType.INVALID;
        mReceiveCallBack = null;
        mNeedResend = false;
        mUserData = null;

        mDataList.Clear();
        mSendResult = -1;
        canRemove = false;
    }

    public void Update(int deltaTime)
    {
        if (canRemove) return;

        if (!IsMessageReceived()) 
        {
            if (mTempWaitTime > 0) 
            {
                mTempWaitTime -= deltaTime;
            }
            if (mTempWaitTime <= 0) 
            {
                if (!mNeedResend)
                {
                    //end
                    End();
                }
                mTempWaitTime = mWaitTime < 0 ? kResendTimeout : mWaitTime;

                mTempWaitCount++;
                if(mTempWaitCount >= kWaitCount)
                {
                    mTempWaitCount = 0;
                    mTempResendCount++;
                    if(mTempResendCount > kResendCount && mWaitTime > 0)
                    {
                        //end
                        End();
                    }

                    if(mRequest != null && -1 == mSendResult)
                    {
                        mSendResult = NetManager.Instance().SendCommand(mServerType, mRequest);
                    }
                }
            }
        }

        if (IsMessageReceived())
        {
            End();
        }
    }

    public bool CanRemove()
    {  
        return canRemove; 
    }

    protected void _OnMessageReceived(MsgDATA data)
    {
        if (data.id == mResult.GetMsgID())
        {
            mDataList.Add(data);
        }
    }

    void Register()
    {
        if(mResult != null)
        {
            NetProcess.AddMsgHandler(mResult.GetMsgID(), OnMessageReceive);
        }
    }

    void UnRegister()
    {
        if(mResult != null)
        {
            NetProcess.RemoveMsgHandler(mResult.GetMsgID(), OnMessageReceive);
        }
    }

    bool IsMessageReceived()
    {
        if(mDataList.Count != 0)
        {
            return true;
        }
        return false;
    }
}
