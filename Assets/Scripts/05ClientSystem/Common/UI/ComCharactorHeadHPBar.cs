using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using GameClient;

public class ComCharactorHeadHPBar : MonoBehaviour, IHPBar {

    public Slider mHp;

    public eHpBarType mType;
    public float mRateLimit = 0.2f;

    private int mMaxHpValue;
    private int mHpValue;

	public void Damage(int value, bool withAnimate = false)
    {
		SetActive(true);

        mHpValue -= value;
        mHpValue = Mathf.Clamp(mHpValue, 0, mMaxHpValue);

        var rate = mHpValue * 1.0f / mMaxHpValue;

        _setHpRate(rate);
        _delayHidden();
    }

    private enum eState
    {
        None,
        Show,
        Hidden,
    }

    eState mState = eState.None;
    public float mTimeHidden = 2f;
    private float mTime = 0;

    private void _delayHidden()
    {
        if (mType == eHpBarType.Monster)
        {
            mState = eState.Show;
            mTime = mTimeHidden;
        }
    }

    void Update()
    {
        if (mState == eState.Show)
        {
            mTime -= Time.deltaTime;

            if (mTime < 0)
            {
                mState = eState.Hidden;
                SetActive(false);
            }
        }
    }


    private void _setHpRate(float rate)
    {
        rate = Mathf.Clamp01(rate);

        // TODO 这里会有问题。 需要根据是否是是主玩家来区分处理
        if (mType == eHpBarType.Player)
        {
            var battleUIPve = BattleUIHelper.GetBattleUIComponent<BattleUIPve>();
            if (null != battleUIPve)
            {
                battleUIPve.ShowDeadTips(rate <= mRateLimit);
            }
        }

        if (mHp)
        {
            mHp.value = rate;
        }
    }

    public eHpBarType GetBarType()
    {
        return mType;
    }

    public bool GetHidden()
    {
        return this.gameObject.activeSelf;
    }

    public void Init(int hp, int mp, int maxHp = -1,int resistValue = 0)
    {
        mHpValue = hp;
        mMaxHpValue = hp;

        _setHpRate(1.0f);
    }

    public void SetActive(bool active)
    {
        if (null == this)
        {
            return;
        }
        this.gameObject.CustomActive(active);
    }

    public void SetHidden(bool hidden)
    {
        SetActive(hidden);
    }

    public void SetMPRate(float percent)
    {
    }

    public void SetName(string name, int level)
    {
    }

    public void SetName(string name)
    {

    }

    public void SetLevel(int level)
    {

    }

    public void Unload()
    {
    }

	public void SetHP(int curHP, int maxHP)
	{
        mHpValue = curHP;
        mMaxHpValue = maxHP;
        var rate = mHpValue * 1.0f / mMaxHpValue;
        _setHpRate(rate);
    }
	public void SetMP(int curMP, int maxMP)
	{

	}

    public void SetIcon(Sprite headIcon, Material material)
    {

    }

    public void InitResistMagic(int resistValue,BeActor player)
    {

    }
    public void SetBuffName(string text)
    {

    }
}

