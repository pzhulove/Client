using UnityEngine;
using System.Collections;
using GameClient;
using UnityEngine.UI;
using Protocol;
using Network;

public class ServerWaitQueueUp : ClientFrame
{
    public override string GetPrefabPath()
    {
        return "UIFlatten/Prefabs/Login/ServerWaitQueueFrame";
    }

    private const float kDefaltWaitSec = 1.5f;
    private const int kDefaultWaitQuitMessageFrameCount = 3;

    private int mLeftTimeInSec = 0;
    private int mPlayerNum = 0;

#region ExtraUIBind
    private Button mQuitWaitQueueBtn = null;
    private Text mServerName = null;
    private Text mShowLeftPlayerNum = null;
    private Text mPreWaitTime = null;
    private GameObject mCheckQuitPlane = null;
    private Button mOkBtn = null;
    private Button mCancelBtn = null;
    private Button mChangeBtn = null;
    private ComTime mLeftTime = null;
    private GameObject mWaitTimeRoot = null;
    private GameObject mSuccessRoot = null;

    protected override void _bindExUI()
    {
        mQuitWaitQueueBtn = mBind.GetCom<Button>("QuitWaitQueueBtn");
        mQuitWaitQueueBtn.onClick.AddListener(_onQuitWaitQueueBtnButtonClick);
        mServerName = mBind.GetCom<Text>("ServerName");
        mShowLeftPlayerNum = mBind.GetCom<Text>("ShowLeftPlayerNum");
        mPreWaitTime = mBind.GetCom<Text>("PreWaitTime");
        mCheckQuitPlane = mBind.GetGameObject("CheckQuitPlane");
        mOkBtn = mBind.GetCom<Button>("OkBtn");
        mOkBtn.onClick.AddListener(_onOkBtnButtonClick);
        mCancelBtn = mBind.GetCom<Button>("CancelBtn");
        mCancelBtn.onClick.AddListener(_onCancelBtnButtonClick);
        mChangeBtn = mBind.GetCom<Button>("ChangeBtn");
        mChangeBtn.onClick.AddListener(_onChangeBtnButtonClick);
        mLeftTime = mBind.GetCom<ComTime>("leftTime");
        mWaitTimeRoot = mBind.GetGameObject("waitTimeRoot");
        mSuccessRoot = mBind.GetGameObject("successRoot");
    }

    protected override void _unbindExUI()
    {
        mQuitWaitQueueBtn.onClick.RemoveListener(_onQuitWaitQueueBtnButtonClick);
        mQuitWaitQueueBtn = null;
        mServerName = null;
        mShowLeftPlayerNum = null;
        mPreWaitTime = null;
        mCheckQuitPlane = null;
        mOkBtn.onClick.RemoveListener(_onOkBtnButtonClick);
        mOkBtn = null;
        mCancelBtn.onClick.RemoveListener(_onCancelBtnButtonClick);
        mCancelBtn = null;
        mChangeBtn.onClick.RemoveListener(_onChangeBtnButtonClick);
        mChangeBtn = null;
        mLeftTime = null;
        mWaitTimeRoot = null;
        mSuccessRoot = null;
    }
#endregion    

#region Callback
    private void _onQuitWaitQueueBtnButtonClick()
    {
        /* put your code in here */
        mCheckQuitPlane.SetActive(true);
    }

    private void _onCancelBtnButtonClick()
    {
        /* put your code in here */
        mCheckQuitPlane.SetActive(false);
    }

    private void _onOkBtnButtonClick()
    {
        /* put your code in here */
        mQuitQueueType = eQuitQueueType.UserCancel;
        _quitQueue();
    }
    
    private void _onChangeBtnButtonClick()
    {
        /* put your code in here */
        ClientSystemManager.instance.OpenFrame<ServerListFrame>();
    }
#endregion

    private enum eQuitQueueType
    {
        None,
        UserCancel,
        ServerListChanged,
    }

    private eQuitQueueType mQuitQueueType = eQuitQueueType.None;

    protected override void _OnOpenFrame()
    {
        mCheckQuitPlane.SetActive(false);

        mServerName.text = ClientApplication.adminServer.name;

        _setPlayerNum((uint)userData);

        _bindEvents();

        mQuitQueueType = eQuitQueueType.None;
    }

    private string[] kWaitTimeDesc = new string[] {
        "等待进入服务器，请稍候.",
        "等待进入服务器，请稍候..",
        "等待进入服务器，请稍候...",
    };

    private int mWaitTimeIdx = 0;

    private Text mWaitTimeDesc = null;

    private void _updateWaitTextTips()
    {
        if (null == mWaitTimeDesc)
        {
            mWaitTimeDesc = Utility.FindComponent<Text>("content/ItemParent/successRoot/PreWaitTime");
        }

        if (null == mWaitTimeDesc)
        {
            return ;
        }

        mWaitTimeDesc.text = kWaitTimeDesc[mWaitTimeIdx % kWaitTimeDesc.Length];

        mWaitTimeIdx++;
        mWaitTimeIdx %= kWaitTimeDesc.Length;
    }

    protected override void _OnCloseFrame()
    {
        _unbindEvents();

        mQuitQueueType = eQuitQueueType.None;
    }

    private void _bindEvents()
    {
        UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ServerListSelectChanged, _onSeverListChanged);
    }

    private void _unbindEvents()
    {
        UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ServerListSelectChanged, _onSeverListChanged);
    }

    private void _onSeverListChanged(UIEvent ui)
    {
        bool isChanged = (bool)ui.Param1;

        if (isChanged)
        {
            mQuitQueueType = eQuitQueueType.ServerListChanged;
            _quitQueue();
            // TODO 若重连的重复登录和这里的重复登录一起发生
            // 那么就有问题
        }
        else
        {
            // continue 
        }
    }

    [MessageHandle(GateNotifyLoginWaitInfo.MsgID)]
    void SyncWaitPlayerNum(MsgDATA data)
    {
        GateNotifyLoginWaitInfo loginWaitInfo = new GateNotifyLoginWaitInfo();
        loginWaitInfo.decode(data.bytes);

        Logger.LogProcessFormat("[登录] 等待玩家数目 {0}", loginWaitInfo.waitPlayerNum);

        _setPlayerNum(loginWaitInfo.waitPlayerNum);
    }

    private void _setPlayerNum(uint playerNum)
    {
        mLeftTimeInSec = (int)(playerNum * _getPerUserWaitTime());
        mPlayerNum = (int)playerNum;

        _updateWaitTimeTips();
        _updateWaitTime();
    }

    private float _getPerUserWaitTime()
    {
        return kDefaltWaitSec;

        ProtoTable.SystemValueTable valueData = TableManager.GetInstance().GetTableItem<ProtoTable.SystemValueTable>((int)ProtoTable.SystemValueTable.eType.SVT_SERVER_QUEUE_PERUSER_WAIT_SEC);
        if(valueData == null)
        {
            return kDefaltWaitSec;
        }

        return valueData.Value / 1000.0f;
    }

    private void _updateWaitTimeTips()
    {
        bool hasLeftTime = mLeftTimeInSec > 0;
        mWaitTimeRoot.SetActive(hasLeftTime);
        mSuccessRoot.SetActive(!hasLeftTime);
    }

    [MessageHandle(GateNotifyAllowLogin.MsgID)]
    void NotifyAllowLogin(MsgDATA data)
    {
        ClientSystemManager.instance.CloseFrame(this);
        ClientSystemManager.instance.CloseFrame<ServerListFrame>();

        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ServerLoginSuccess);
    }

    protected void _quitQueue()
    {
        ClientSystemManager.instance.CloseFrame(this);

        // send quit message
        GateLeaveLoginQueue quitQueueMsg = new GateLeaveLoginQueue();
        NetManager.instance.SendCommand(ServerType.GATE_SERVER, quitQueueMsg);

        Logger.LogProcessFormat("[登录] 发送退出排队消息");
        mTickCount = kDefaultWaitQuitMessageFrameCount;
    }

    public override bool IsNeedUpdate()
    {
        return true;
    }

    private float mTickTime = 0.0f;

    private int mTickCount = 0;

    protected override void _OnUpdate(float delta)
    {
        mTickTime += delta;

        if (mTickTime > 1.0f)
        {
            //if (mLeftTimeInSec >= 0)
            {
                _updateWaitTime();
            }

            mTickTime -= 1.0f;
        }

        switch (mQuitQueueType)
        {
            case eQuitQueueType.None:
                break;
            case eQuitQueueType.ServerListChanged:
                if (_isTickCountFinish())
                {
                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ServerLoginFail);
                    ClientSystemLoginUtility.StartLoginAfterVerify();
                    mQuitQueueType = eQuitQueueType.None;
                }
                break;
            case eQuitQueueType.UserCancel:
                if (_isTickCountFinish())
                {
                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ServerLoginFail);
                    mQuitQueueType = eQuitQueueType.None;
                }
                break;
        }
    }

    private bool _isTickCountFinish()
    {
        return mTickCount-- < 0;
    }

    private void _updateWaitTime()
    {
        mLeftTimeInSec--;

        if (mLeftTimeInSec >= 0)
        {
            mLeftTime.SetTime(mLeftTimeInSec * 1000);
        }
        else 
        {
            if (mPlayerNum > 1)
            {
                _setPlayerNum((uint)mPlayerNum);
            }
            else
            {
                _updateWaitTextTips();
                _updateWaitTimeTips();
            }
        }
    }
}
