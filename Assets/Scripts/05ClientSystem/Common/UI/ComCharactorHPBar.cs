using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using GameClient;
using Protocol;

public class ComCharactorHPBar : ComBaseComponet, IHPBar 
{
    public eHpBarType type = eHpBarType.Player;
    public float mRateLimit = 0.2f;

    #region ExtraUIBind
    private Image mIcon = null;
    private Text mLevel = null;
    private Text mName = null;
    private Text mHpText = null;
    private Text mCurHpText = null;
    private Text mMaxHpText = null;
    private Text mMpText = null;
    private Button mReborn = null;
    private GameObject mRebornRoot = null;
    private Image mHp = null;
    private Image mMp = null;
    private UIGray mIconGray = null;
    private GameObject mIconFg = null;
    private Image mSignal = null;
    private UIGray mSignalgray = null;
    private ComTeamChatMessage mChatMsg = null;
    //private Button mResistMagicBtn = null;
    //private Image mResistMagicIcon = null;
    //private GameObject mResistContent = null;
    private UIGray mRebornBtnGray = null;
    private GameObject mTeamleaderFlag = null;
    private ReplaceHeadPortraitFrame mReplaceHeadPortraitFrame = null;
    private Text resentment = null;
    private GameObject resentmentChange = null;
    private Text resentmentText = null;

    private int m_LastCurHpValue = 0;
    private int m_LastMaxHpValue = 0;

    protected override void _bindExUI()
    {
        mIcon = mBind.GetCom<Image>("icon");
        mLevel = mBind.GetCom<Text>("level");
        mName = mBind.GetCom<Text>("name");
        mHpText = mBind.GetCom<Text>("hpText");
        mCurHpText = mBind.GetCom<Text>("curHpText");
        mMaxHpText = mBind.GetCom<Text>("maxHpText");
        mMpText = mBind.GetCom<Text>("mpText");
        mReborn = mBind.GetCom<Button>("reborn");
        if (null != mReborn)
        {
            mReborn.onClick.AddListener(_onRebornButtonClick);
        }
        mRebornRoot = mBind.GetGameObject("rebornRoot");
        mHp = mBind.GetCom<Image>("hp");
        mMp = mBind.GetCom<Image>("mp");
        mIconGray = mBind.GetCom<UIGray>("iconGray");
        mIconFg = mBind.GetGameObject("iconFg");
        mSignal = mBind.GetCom<Image>("signal");
        mSignalgray = mBind.GetCom<UIGray>("signalgray");
        mChatMsg = mBind.GetCom<ComTeamChatMessage>("chatMsg");
        //mResistMagicBtn = mBind.GetCom<Button>("ResistMagicBtn");
        //if (null != mResistMagicBtn)
        //{
        //    mResistMagicBtn.onClick.AddListener(_onResistMagicBtnButtonClick);
        //}
        resentment = mBind.GetCom<Text>("resentment");
        resentmentChange = mBind.GetGameObject("YuanNianZhi");
        if(resentmentChange!=null)
           resentmentText = resentmentChange.GetComponentInChildren<Text>();
        //mResistMagicIcon = mBind.GetCom<Image>("ResistMagicIcon");
        //mResistContent = mBind.GetGameObject("ResistContent");
        mRebornBtnGray = mBind.GetCom<UIGray>("RebornBtnGray");
        mTeamleaderFlag = mBind.GetGameObject("teamleaderFlag");
        mReplaceHeadPortraitFrame = mBind.GetCom<ReplaceHeadPortraitFrame>("ReplaceHeadPortraitFrame");

        _bindEvent ();
    }

    protected override void _unbindExUI()
    {
		_unbindEvent ();

        mIcon = null;
        mLevel = null;
        mName = null;
        mHpText = null;
        mMpText = null;
        mCurHpText = null;
        mMaxHpText = null;
        if (null != mReborn)
        {
            mReborn.onClick.RemoveListener(_onRebornButtonClick);
        }
        mReborn = null;
        mRebornRoot = null;
        mHp = null;
        mMp = null;
        mIconGray = null;
        mIconFg = null;
        mSignal = null;
        mSignalgray = null;
        mChatMsg = null;
        //if (null != mResistMagicBtn)
        //{
        //    mResistMagicBtn.onClick.RemoveListener(_onResistMagicBtnButtonClick);
        //}
        //mResistMagicBtn = null;
        //mResistMagicIcon = null;
        //mResistContent = null;
        mRebornBtnGray = null;
        mTeamleaderFlag = null;
        mReplaceHeadPortraitFrame = null;
    }
    #endregion

    #region Callback
    private void _onRebornButtonClick()
    {
        /* put your code in here */

        byte mainPlayerSeat = BattleMain.instance.GetPlayerManager().GetMainPlayer().playerInfo.seat;
        int id = BattleMain.instance.GetDungeonManager().GetDungeonDataManager().id.dungeonID;

        DungeonUtility.StartRebornProcess(mainPlayerSeat, mSeat, id);
    }
#endregion

    protected void _bindEvent()
    {
        UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.BattlePlayerDead, _onUpdatePlayerStatus);
        UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.BattlePlayerAlive, _onUpdatePlayerStatus);
        UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.BattlePlayerBack, _onUpdatePlayerStatus);
        UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.BattlePlayerLeave, _onUpdatePlayerStatus);
        UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.BattlePlayerInfoChange, _onUpdatePlayerStatus);

        UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.TeamNotifyChatMsg, _updateTeamChatMsg);
    }

    protected void _unbindEvent()
    {
        UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.BattlePlayerDead, _onUpdatePlayerStatus);
        UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.BattlePlayerAlive, _onUpdatePlayerStatus);
        UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.BattlePlayerBack, _onUpdatePlayerStatus);
        UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.BattlePlayerLeave, _onUpdatePlayerStatus);
        UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.BattlePlayerInfoChange, _onUpdatePlayerStatus);

        UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.TeamNotifyChatMsg, _updateTeamChatMsg);
    }

    private void _updateTeamChatMsg(UIEvent ui)
    {
        UInt64 roleID = (UInt64)ui.Param1;
        string word = (string)ui.Param2;

        BattlePlayer player = BattleMain.instance.GetPlayerManager().GetPlayerBySeat(mSeat);
        if (null != player && player.playerInfo.roleId == roleID)
        {
            mChatMsg.SetMessage(word);
        }
    }


    protected bool _canReborn(byte seat)
    {
        BattlePlayer player = BattleMain.instance.GetPlayerManager().GetPlayerBySeat(mSeat);
        var battle = BattleMain.instance.GetBattle();
        if (battle != null && !battle.CanReborn())
        {
            return false;
        }
        if (null != player)
        {
            if (BattlePlayer.eNetState.Online != player.netState)
            {
                return false;
            }
        }

        int id = BattleMain.instance.GetDungeonManager().GetDungeonDataManager().id.dungeonID;
        return DungeonUtility.CanReborn(id, false);
    }

    public bool IsDungeonRebornLimit()
    {
        if (BattleMain.instance == null || BattleMain.instance.GetPlayerManager() == null)
            return false;
        BattlePlayer player = BattleMain.instance.GetPlayerManager().GetPlayerBySeat(mSeat);
        if (player == null)
            return false;
        int id = BattleMain.instance.GetDungeonManager().GetDungeonDataManager().id.dungeonID;
        int dungeonRebornCount = DungeonUtility.GetDungeonRebornCount(id);
        bool isBattleRebornCountOk = true;
        var battle = BattleMain.instance.GetBattle();
        if (battle != null && battle.IsRebornCountLimit())
        {
            isBattleRebornCountOk = battle.GetLeftRebornCount() > 0;
        }
        if (isBattleRebornCountOk)
        {
            if (dungeonRebornCount <= 0)
                return false;
            int actorDungeonCount = player.playerActor.dungeonRebornCount;
            return actorDungeonCount >= dungeonRebornCount;
        }
        return true;
    }

    protected void _onUpdatePlayerStatus(UIEvent ui)
    {
        BattlePlayer player = BattleMain.instance.GetPlayerManager().GetPlayerBySeat(mSeat);
        if (null != player)
        {
            Logger.LogProcessFormat("[玩家状态更新] {0}, {1}, netQuality {2}", mSeat, player.state, player.netQuality);

       
            switch(player.state)
            {
                case BattlePlayer.EState.Dead:
                    mIconGray.enabled = true;
                    mIconFg.SetActive(true);
                    if (_canReborn(mSeat))
                    {
                        mRebornRoot.SetActive(true);
                        //判断是否超过地下城限制复活次数
                        if (IsDungeonRebornLimit())
                        {
                            mRebornBtnGray.enabled = true;
                            mReborn.enabled = false;
                        }
                    }
                    else
                        mRebornRoot.SetActive(false);

                    if (player == BattleMain.instance.GetPlayerManager().GetMainPlayer())
                    {
                        var battleUIPve = BattleUIHelper.GetBattleUIComponent<BattleUIPve>();
                        if (null != battleUIPve)
                        {
                            battleUIPve.ShowDeadTips(false);
                        }
                    }

                    break;
                case BattlePlayer.EState.Normal:

                    mIconGray.enabled = false;
                    mIconFg.SetActive(false);
                    mRebornRoot.SetActive(false);
                    //SetActive(true);
                    break;
            } 

            switch (player.netQuality)
            {
                case BattlePlayer.eNetQuality.Bad:
                    // mSignal.sprite = mBind.GetSprite("signalbad");
                    mBind.GetSprite("signalbad", ref mSignal);
                    break;
                case BattlePlayer.eNetQuality.Good:
                    // mSignal.sprite = mBind.GetSprite("signalgood");
                    mBind.GetSprite("signalgood", ref mSignal);
                    break;
                case BattlePlayer.eNetQuality.Best:
                    // mSignal.sprite = mBind.GetSprite("signalbest");
                    mBind.GetSprite("signalbest", ref mSignal);
                    break;
                case BattlePlayer.eNetQuality.Off:
                    // mSignal.sprite = mBind.GetSprite("signalbad");
                    mBind.GetSprite("signalbad", ref mSignal);
                    break;
            }

            switch (player.netState)
            {
                case BattlePlayer.eNetState.Online:
                    mSignalgray.enabled = false;
                    break;
                case BattlePlayer.eNetState.Offline:
                case BattlePlayer.eNetState.Quit:
                    mIconGray.enabled = true;
                    mSignalgray.enabled = true;
                    break;
            }

        }
    }


    private byte mSeat;

    public void SetSeat(byte seat)
    {
        mSeat = seat;

        _updateTeamLeaderFlag();
    }

    public eHpBarType GetBarType()
    {
        return type;
    }

    public void SetHPRate(float rate)
    {
        if (null == this ||
            null == this.mHp ||
            null == this.mMaxHpText ||
            null == this.mCurHpText)
        {
            return;
        }

        mHp.fillAmount = Mathf.Clamp01(rate);

        int hp = Math.Max(0, mCurHp);

        if(m_LastCurHpValue != hp)
        {
            mCurHpText.text = hp.ToString();
            m_LastCurHpValue = hp;
        }

        if(m_LastMaxHpValue != mMaxHp)
        {
            mMaxHpText.text = mMaxHp.ToString();
            m_LastMaxHpValue = mMaxHp;
        }

        if (BattleMain.instance.GetPlayerManager().GetMainPlayer().playerInfo.seat == mSeat)
        {
            var battleUIPve = BattleUIHelper.GetBattleUIComponent<BattleUIPve>();
            if (null != battleUIPve)
            {
                battleUIPve.ShowDeadTips(rate <= mRateLimit);
            }
        }
    }

    public void SetMPRate(float rate)
    {
        if (null == this
         || null == this.mMp
         || null == this.mMpText)
        {
            return;
        }

        mMp.fillAmount = Mathf.Clamp01(rate);

        int mp = (int)(mMp.fillAmount * mMaxMp);
        mp = Math.Max(0, mp);

        //mMpText.text = string.Format("{0}/{1}", mp, mMaxMp);
    }

    public void SetIcon(Sprite sprite, Material material)
    {
        if (null == this || null == this.mIcon)
        {
            return;
        }

        if (mIcon && sprite != null)
        {
            mIcon.sprite = sprite;
            mIcon.material = material;
        }
    }

    public void SetName(string name, int level)
    {
        if (null == this)
        {
            return;
        }

        SetName(name);
        SetLevel(level);
    }

    public void SetName(string name)
    {
        if (null == this)
        {
            return;
        }

        if (mName)
        {
            mName.text = string.Format("{0}", name);
        }
    }

    public void SetLevel(int level)
    {
        if (null == this)
        {
            return;
        }

        if (mLevel != null)
        {
            mLevel.text = string.Format("{0}", level);
        }
    }

	public void SetHP(int curHP, int maxHP)
	{
		mMaxHp = maxHP;
		mCurHp = curHP;
		SetHPRate(curHP/(float)maxHP);
	}

	public void SetMP(int curMP, int maxMP)
	{
		mMaxMp = maxMP;
		mCurMp = maxMP;
		SetMPRate(curMP/(float)maxMP);
	}

    private int mMaxHp;
    private int mMaxMp;

    private int mCurHp;
    private int mCurMp;

    public void Init(int maxHp, int maxMp, int count = 1, int resistVale = 0)
    {
        mMaxHp = mCurHp = maxHp;
        mMaxMp = mCurMp = maxMp;

        SetHP(mCurHp, maxHp);
        SetMP(mCurMp, maxMp);

        //Damage(0);

        SetMPRate(1.0f);
        //InitResistMagic(resistVale);

		if (mSignal != null)
			mSignal.transform.parent.gameObject.CustomActive(BattleMain.IsModeMultiplayer(BattleMain.mode));

        _updateTeamLeaderFlag();
    }

    private void _updateTeamLeaderFlag()
    {
        if (null != mTeamleaderFlag)
        {
            BattlePlayer player = BattleMain.instance.GetPlayerManager().GetPlayerBySeat(mSeat);

            mTeamleaderFlag.SetActive(false);

            if (null != player && null != player.playerInfo && TeamDataManager.GetInstance().IsTeamLeaderByRoleID(player.playerInfo.roleId))
            {
                mTeamleaderFlag.SetActive(true);
            }
        }
    }

    private void _updateHpMpValue()
    {
        BattlePlayer player = BattleMain.instance.GetPlayerManager().GetPlayerBySeat(mSeat);

        if (null == player)
        {
            return;
        }

        if (null == player.playerActor)
        {
            return ;
        }

        BeEntityData data = player.playerActor.GetEntityData();
        if (null == data)
        {
            return;
        }

        SetHP(data.GetHP(), data.GetMaxHP());
        SetMP(data.GetMP(), data.GetMaxMP());
    }
    private void UpdateResentment()
    {
        if (resentment == null)
            return;
        BattlePlayer player = BattleMain.instance.GetPlayerManager().GetPlayerBySeat(mSeat);

        if (null == player)
        {
            return;
        }

        if (null == player.playerActor)
        {
            return;
        }
        Mechanism2004 mechanism = player.playerActor.GetMechanism(5300) as Mechanism2004;
        if (mechanism == null)
            return;
        resentment.CustomActive(true);
        resentment.text = mechanism.GetResentmentValue().ToString();
    }


    public void Damage(int value, bool withAiniate = false)
    {
        if (value > 0)
        {
            _updateHpMpValue();
        }
    }

    public void SetActive(bool active)
    {
        //if (active && state == BattlePlayer.EState.Offline)
        //	return;

        if (null != this && gameObject != null)
        {
            gameObject.CustomActive(active);
        }
    }

    public void Unload()
    {
    }

    private bool mHidden = false;
    public void SetHidden(bool hidden)
    {
        mHidden = hidden;
    }

    public bool GetHidden()
    {
        return mHidden;
    }

    private float mCountTime = 0.0f;

    public void Update()
    {
        mCountTime += Time.deltaTime;

        if (mCountTime > 0.1f)
        {
            mCountTime = 0;

            _updateHpMpValue();
            UpdateResentment();
        }
        UpdateResentmentChange();
    }

    bool flag = false;
    float calcTime = 0;
    private void UpdateResentmentChange()
    {
        calcTime += Time.deltaTime;
        if (calcTime > 1.0f && flag)
        {
            resentmentChange.CustomActive(false);
            flag = false;
            calcTime = 0;
        }
    }

    public void ShowResentmentChange(int value)
    {
        if (resentmentText != null)
        {
            if (resentmentChange != null)
            {
               Image image = resentmentChange.GetComponent<Image>();
               image.color =value<0? Color.red:Color.white;
            }
            string s = value > 0 ? "+" : "";
            resentmentText.text =s+ value.ToString();
            resentmentChange.CustomActive(true);
            calcTime = 0;
            flag = true;
        }
    }

    private int[] ResistMagicBuffID = new int[] { 820001, 820002, 820003 };

    public void InitResistMagic(int value,BeActor actor)
    {
        int dungeonId = BattleMain.instance.GetDungeonManager().GetDungeonDataManager().id.dungeonID;
        int dungeonResistValue = DungeonUtility.GetDungeonResistMagicValueById(dungeonId);
        if (dungeonResistValue == 0)
            return;
        SetResistIcon(value, dungeonResistValue,actor);
    }
    
    //private void _onResistMagicBtnButtonClick()
    //{
    //    if (BattleMain.instance == null || BattleMain.instance.GetPlayerManager() == null)
    //        return;
    //    BattlePlayer player = BattleMain.instance.GetPlayerManager().GetPlayerBySeat(mSeat);
    //    if (player == null || player.playerActor == null)
    //        return;
    //    ResistTipsData tipsData = new ResistTipsData();
    //    VFactor resistMagicRate = player.playerActor.attribute.GetResistMagicRate();
    //    tipsData.rateValue = (resistMagicRate * GlobalLogic.VALUE_100).integer;
    //    tipsData.hpBarIndex = GetResistIndex();
    //    if (ClientSystemManager.instance.IsFrameOpen<ResistMagicTipsFrame>())
    //    {
    //        ResistMagicTipsFrame resistTipsFrame = ClientSystemManager.instance.GetFrame(typeof(ResistMagicTipsFrame)) as ResistMagicTipsFrame;
    //        //再次点击关闭
    //        if (resistTipsFrame.ResistIndex == tipsData.hpBarIndex)
    //        {
    //            resistTipsFrame.Close();
    //            return;
    //        }
    //        resistTipsFrame.RefreshData(tipsData);
    //    }
    //    else
    //    {
    //        ClientSystemManager.instance.OpenFrame<ResistMagicTipsFrame>(FrameLayer.Top, tipsData);
    //    }
    //}

    void SetResistIcon(int value,int dungeonValue,BeActor player)
    {
        if (null == player)
        {
            return;
        }
        
        for (int i = 0;i< ResistMagicBuffID.Length; ++i)
        {
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.BattleBuffCancel, null, player, ResistMagicBuffID[i]);
        }
        if (value < dungeonValue)
        {
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.BattleBuffAdded, null, player, ResistMagicBuffID[0]);
        }
        else if (value == dungeonValue)
        {
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.BattleBuffAdded, null, player, ResistMagicBuffID[1]);
        }
        else
        {
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.BattleBuffAdded, null, player, ResistMagicBuffID[2]);
        }
    }

    //int GetResistIndex()
    //{
    //    int index = 0;
    //    Vector3 pos = gameObject.transform.position;
    //    if (pos.x > -100)
    //        index = 3;
    //    else if (pos.x > -300)
    //        index = 2;
    //    else
    //        index = 1;
    //    return index;
    //}

    public void SetBuffName(string text)
    {

    }
    /// <summary>
    /// 设置头像框
    /// </summary>
    /// <param name="playerLabelInfo"></param>
    public void SetHeadPortraitFrame(PlayerLabelInfo playerLabelInfo)
    {
        if (playerLabelInfo == null)
        {
            Logger.LogAssetFormat(" 脚本[ComCharactorHPBar] 方法[SetHeadPortraitFrame] 参数[PlayerLabelInfo] 传过来的玩家标签信息为空");
            return;
        }

        if (playerLabelInfo.headFrame != 0)
        {
            mReplaceHeadPortraitFrame.ReplacePhotoFrame((int)playerLabelInfo.headFrame);
        }
        else
        {
            mReplaceHeadPortraitFrame.ReplacePhotoFrame(HeadPortraitFrameDataManager.iDefaultHeadPortraitID);
        }
    }
}
