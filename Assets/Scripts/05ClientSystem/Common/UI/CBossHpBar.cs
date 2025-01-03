using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

using DG.Tweening;
using System;

[ExecuteAlways]
public class CBossHpBar : MonoBehaviour, IHPBar
{
    public eHpBarType type = eHpBarType.Boss;

    public CBossHpFadeWhite whiteBar;

    public CanvasGroup root = null;

    [SerializeField] private Slider _sliderFg;
    [SerializeField] private Image _sliderFgImage;
    [SerializeField] private Slider _sliderBg;
    [SerializeField] private Image _sliderBgImage;
    [SerializeField] private Slider _sliderButtom;
    [SerializeField] private Image _sliderButtomImage;
    [SerializeField] private Slider _sliderTop;
    [SerializeField] private Image _sliderTopImage;
    [SerializeField] private DOTweenAnimation _deadTween;

    public Image imgIcon;

    public Text _txtLevelCount;
    public Text txtName;
    public Text txtLeftBar;
    public Text buffName;

    public DOTweenAnimation shakeTween;
    
    public float fWhiteFadeOutTime = 1.5f;
    public float fFullBloodAnimateTime = 3.0f;
    public float fOneAmountBloodAnimateTime = 3.0f;

    /// <summary>
    /// 总血量
    /// </summary>
    public int iSumHP = 1;

    /// <summary>
    /// 当前血条血量
    /// </summary>
    private int iPerBarAmount;

    /// <summary>
    /// 当前真实剩余血量 
    /// </summary>
    private int iLeftHP;
    
    public enum BloodState { None, Alive, Dead, Remove, WaitDeadFlag, Destroy }

    private BloodState mBloodState = BloodState.None;
    
    private int mTmpHP = 0;

    public Sprite[] SpriteList;

    public void Init(int sumHp, int sumMp, int singleHpValue,int resistValue = 0)
    {
        mBloodState = BloodState.Alive;

        _removeDeadEffect();
        
        iSumHP = sumHp;
        iLeftHP = sumHp;

        mTmpHP = iSumHP;
        mAnimateHPValue = iSumHP;

        iPerBarAmount = singleHpValue;

        var curBarLeftHp = sumHp % singleHpValue;

        // 使用原始的宽度
        if (singleHpValue < 0)
        {
            iPerBarAmount = iSumHP;
        }

        if (iSumHP < singleHpValue)
        {
            iPerBarAmount = iSumHP;
        }

		_setFgByHp(iSumHP);
		_setBgByHp(iSumHP);
		_updateAnimate(iSumHP);
    }

    private void _removeDeadEffect()
    {
        if (_deadTween != null)
        {
            _deadTween.CustomActive(false);
            _deadTween.DOPause();
        }
    }

    public void SetIcon(Sprite sprite, Material material)
    {
        if (null == this)
        {
            return;
        }

        if (sprite != null)
        {
            imgIcon.sprite = sprite;
            imgIcon.material = material;
        }
    }

    public void SetName(string name, int level)
    {
        _setNameData(name);
        _setLevelData(level);

        _updateTxtName();
    }

    public void SetName(string name)
    {
        _setNameData(name);
        _updateTxtName();
    }

    public void SetLevel(int level)
    { 
        _setLevelData(level);
        _updateTxtName();
    }

    private void _setLevelData(int level)
    {
        if (mLevelCount != level)
        {
            mLevelCount = level;
            mIsNameLevelDirty = true;
        }
    }

    public void _setNameData(string name)
    {
        if (mNameString != name)
        {
            mNameString = name;
            mIsNameLevelDirty = true;
        }
    }

    private int mLevelCount = -1;
    private string mNameString = string.Empty;
    private bool mIsNameLevelDirty;

    private void _updateTxtName()
    {
        if (null == this)
        {
            return;
        }

        if (!mIsNameLevelDirty)
        {
            return;
        }

        mIsNameLevelDirty = false;
        // _txtLevelCount.text = mLevelCount.ToString();
        // txtName.text = mNameString;
        txtName.text = string.Format("Lv.{0} {1}", mLevelCount, mNameString);
    }

    public eHpBarType GetBarType()
    {
        return type; 
    }

    private int mAnimateHPValue = 0;

    private int mFadeMin = 0;
    private int mFadeMax = 0;

	public void Damage(int damage, bool withAnimate)
    {
        _updateTmpHP(damage);

        if (mBloodState != BloodState.Alive)
        {
            return;
        }

        mFadeMax = iLeftHP;

        iLeftHP -= damage;
        iLeftHP = Mathf.Clamp(iLeftHP, 0, iSumHP);

        mFadeMin = iLeftHP;

        if (damage > 0)
        {
			if (withAnimate) 
			{
				_startFade (mFadeMin, mFadeMax);
			} 
			else
			{
				_setDamageWithOutAnimate ();
			}
        }
        else
        {
			_setDamageWithOutAnimate ();
        }

        if (iLeftHP <= 0)
        {
            iLeftHP = 0;
            mBloodState = BloodState.Dead;
        }

        if (damage > 0 && withAnimate)
        {
            _shakeBloodBar();
        }
    }

	private void _setDamageWithOutAnimate()
	{
		mTmpHP = iLeftHP;
		mAnimateHPValue = iLeftHP;
		_updateAnimate(iLeftHP);
		_setFgByHp(iLeftHP);
	}

    private void _startFade(int minHP, int maxHP)
    {
        float midRate = 1.0f;
        float midRightRate = 0.0f;

        int diffHp = maxHP - minHP;
        if (diffHp >= _getCurrentBarAmount())
        {
            midRate = 0.0f;
            midRightRate = 1.0f;
        }
        else 
        {
            midRate = _getFgRateByHP(minHP);
            midRightRate = _getFgRateByHP(maxHP);
        }

        mAnimateHPValue = maxHP;

		_setFgByHp(minHP);

        if (null != whiteBar)
        {
            whiteBar.SetValue(midRate, midRightRate, fWhiteFadeOutTime, ()=>{ _fadeCB(minHP); });
        }
    }

    private void _fadeCB(int minHP)
    {
        if (mAnimateHPValue > minHP) 
		{
			_setBgByHp(minHP);

            mAnimateHPValue = minHP;
        }
    }

    public float _deadEffectTime = 0.5f;
    private float _curDeadEffectTime;

    public void Update()
    {
        if (mBloodState == BloodState.None)
        {
            return;
        }

        if (mTmpHP > mAnimateHPValue)
        {
            mTmpHP -= _getRealAnimateSpeed();

            if (mTmpHP < mAnimateHPValue)
            {
                mTmpHP = mAnimateHPValue;
            }

            _updateAnimate(mTmpHP);
        }

        if (mBloodState == BloodState.Remove)
        {
            mBloodState = BloodState.WaitDeadFlag;
            _curDeadEffectTime = _deadEffectTime;
            if (_deadTween != null)
            {
                _deadTween.CustomActive(true);
                _deadTween.DORestart();
            }
        }
        
		if (mBloodState == BloodState.WaitDeadFlag)
		{
            if (_isCurrentHiddent())
            {
                mBloodState = BloodState.Destroy;
                _removeDeadEffect();
            }
            else 
            {
                _curDeadEffectTime -= Time.deltaTime;

                if (_curDeadEffectTime < 0)
                {
                    if (mTmpHP <= 0)
                    {
                        mBloodState = BloodState.Destroy;
                        _removeDeadEffect();
                    }
                    else
                    {
                        mBloodState = BloodState.Remove;
                    }
                }
            }
		}

        if (mBloodState == BloodState.Destroy)
        {
            if (null != this && null != this.gameObject)
            {
                SetActive(false);
            }

            mBloodState = BloodState.None;
        }
    }

    private void _updateAnimate(int midHpValue)
	{
        int amount = _getCurrentBarAmount();
        int cnt = _getAnimateHPStandCount(midHpValue);

        if (cnt > 0)
        {
            if (cnt == 1)
            {
                if (null != _sliderBg)
                    _sliderBg.value = _getBgRateByHP(midHpValue);
                if (_sliderBgImage != null)
                    _sliderBgImage.sprite = _getBgSprite(midHpValue);
            }
            else 
            {
                if (null != _sliderBg)
                    _sliderBg.value = _getBgRateByHP(midHpValue);
                if (_sliderBgImage != null)
                    _sliderBgImage.sprite = _getBgSprite(midHpValue);
            }
        }
        else 
        {
            if (null != _sliderBg)
            {
                _sliderBg.value = 0.0f;
            }
        }

        _updateTop2Indx(cnt);

        if (null != _sliderFg)
            _sliderFg.value = _getFgRateByHP(midHpValue);
        if (_sliderFgImage != null)
            _sliderFgImage.sprite = _getFgSprite(midHpValue);

        _updateLeftBarCountByHp(midHpValue);
        _setBgByHp(midHpValue);
    }

    private void _updateTop2Indx(int i)
    {
        if (i == 0)
        {
            if (null != _sliderTop)
            {
                _sliderTop.gameObject.SetActive(true);
                _sliderTop.transform.SetAsLastSibling();
            }
        }
        else if (i == 1)
        {
            if (null != _sliderTop)
            {
                _sliderTop.gameObject.SetActive(true);
                _sliderTop.transform.SetAsLastSibling();
            }

            if (null != _sliderFgImage)
            {
                _sliderFgImage.transform.SetAsLastSibling();
            }
        }
        else 
        {
            if (null != _sliderTop)
            {
                _sliderTop.gameObject.SetActive(false);
                _sliderTop.transform.SetAsFirstSibling();
            }
        }
    }

    private int _getAnimateHPStandCount(int hp)
    {
        int leftHP = _getLeftHPInCurrentBar(hp);
        int diffHP = hp - mAnimateHPValue;
        int amount = _getCurrentBarAmount(); 

		if (diffHP > leftHP)
        {
            int lastDiff = (diffHP - leftHP);

            if (lastDiff % amount == 0)
            {
                return lastDiff / amount;
            }
            else 
            {
                return lastDiff / amount + 1;
            }
        }

        return 0;
    }

    private void _updateTmpHP(int damage)
    {
        int amount = _getCurrentBarAmount();
        int diff = mTmpHP - mAnimateHPValue;
        if (diff > amount * 2)
        {
            if (0 == mAnimateHPValue % amount && 0 != mAnimateHPValue)
            {
                mTmpHP = mAnimateHPValue + amount;
            }
            else 
            {
                mTmpHP = mAnimateHPValue / amount * amount;
            }

            _updateAnimate(mTmpHP);
        }
    }

    private int mRealAnimateSpeed = 0;

    private int _getRealAnimateSpeed()
    {
        int amount = _getCurrentBarAmount();
        int diff = mTmpHP - mAnimateHPValue;

        float speed = (1.0f * amount / fOneAmountBloodAnimateTime);
        float ntime = diff / speed;

        if (diff / amount >= 2 || mAnimateHPValue == 0)
        {
            mRealAnimateSpeed = (int)(iSumHP / fFullBloodAnimateTime * Time.deltaTime + 1);
        }
        else if (ntime < fFullBloodAnimateTime * 2)
        {
            mRealAnimateSpeed = (int)(speed * Time.deltaTime + 1);
        }
        else 
        {
            mRealAnimateSpeed = (int)(speed * Time.deltaTime + 1);
        }

        return mRealAnimateSpeed;
    }


    public void SetActive(bool active)
    {
        if (null != this && null != root)
        {
			root.alpha = (active) ? 1.0f : 0.0f;
        }
    }

    public void Unload()
    {
        this.Damage(this.iLeftHP,true);
        if (_isCurrentHiddent())
        {
            mBloodState = BloodState.Destroy;
            _removeDeadEffect();
        }
        else
        {
            mBloodState = BloodState.Remove;
        }
    }

    private bool _isCurrentHiddent()
    {
        return (null != this && null != root && Mathf.Approximately(root.alpha, 0.0f));
    }

    private bool mHidden = true;
    public void SetHidden(bool hidden)
    {
        mHidden = hidden;
    }

    public bool GetHidden()
    {
        return mHidden;
    }

    private int _getCurrentBarAmount()
    {
        return iPerBarAmount;
    }

    private int _getLeftCurrentBarIndex(int leftHP)
    {
        if (leftHP <= 0)
        {
            return 0;
        }

        return leftHP / _getCurrentBarAmount();
    }

    private int _getLeftHPInCurrentBar(int leftHP)
    {
        int amount = _getCurrentBarAmount();
		int leftInOne = leftHP % amount;

        if (leftInOne == 0)
        {
            if (leftHP == 0)
            {
                return 0;
            }
            else
            {
                return amount;
            }
        }

        return leftInOne;
    }

    private int _getBgIndex(int leftValue)
    {
		if (leftValue <= _getCurrentBarAmount ()) 
		{
			return -1;
		}

		leftValue = leftValue - leftValue % _getCurrentBarAmount ();
	
        return _getSpriteIdxByHP(leftValue);
    }

    private int _getFgIndex(int leftValue)
    {
        return _getSpriteIdxByHP(leftValue);
    }

    private int _getSpriteIdxByHP(int leftHP)
    {
        int leftBarCount = _getLeftCurrentBarIndex(leftHP);

        int amount = _getCurrentBarAmount();
        int leftAmount = _getLeftHPInCurrentBar(leftHP);

        if (amount == leftAmount)
        {
            leftBarCount--;
        }

        if (leftBarCount < 0)
        {
            leftBarCount = 0;
        }

        return leftBarCount % SpriteList.Length;
    }


    private Sprite _getFgSprite(int leftValue)
    {
        int fgIdx = _getFgIndex(leftValue);
        return SpriteList[fgIdx];
    }

    private Sprite _getBgSprite(int leftValue)
    {
        int bgIdx = _getBgIndex(leftValue);
        if (bgIdx < 0)
        {
            return null;
        }
        else 
        {
            return SpriteList[bgIdx];
        }
    }

    private void _shakeBloodBar()
    {
        if (shakeTween != null)
        {
            shakeTween.DORestart();
        }
    }

    private int _lastidx = -1;

    private void _updateLeftBarCount(int idx)
    {
        if (txtLeftBar != null)
        {
            //数据没有改变时 不刷新UI
            if (_lastidx == idx) return;
            _lastidx = idx;

            if (idx <= 0)
            {
                txtLeftBar.text = "";
            }
            else
            {
                txtLeftBar.text = string.Format("x{0}", idx);
            }
        }
    }

    private void _updateLeftBarCountByHp(int leftHP)
    {
        int barIdx = _getLeftCurrentBarIndex(leftHP);
        _updateLeftBarCount(barIdx);
    }

    private void _setFgByHp(int leftHP)
    {
        Sprite sp = _getFgSprite(leftHP);
        if (null != _sliderTopImage)
        {
            _sliderTopImage.sprite = sp;
        }

        if (null != _sliderTop)
        {
            _sliderTop.value = _getFgRateByHP(leftHP);
        }
    }

    private void _setBgByHp(int leftHP)
    {
        Sprite sp = _getBgSprite(leftHP);
        if (null != sp)
        {
            if (null != _sliderButtomImage)
            {
                _sliderButtomImage.sprite = sp;
            }

            if (null != _sliderButtom)
            {
                _sliderButtom.value = _getBgRateByHP(leftHP);
            }
        }
        else 
        {
            if (null != _sliderButtom)
            {
                _sliderButtom.value = 0.0f;
            }
        }
    }

    private float _getBgRateByHP(int leftHP)
    {
        int amount = _getCurrentBarAmount();
        if (leftHP < amount)
        {
            return Mathf.Clamp01(leftHP * 1.0f / amount);
        }
        else
        {
            return 1.0f;
        }
    }

    private float _getFgRateByHP(int leftHP)
    {
        return 1.0f * _getLeftHPInCurrentBar(leftHP) / _getCurrentBarAmount();
    }

	public void SetMPRate(float percent)
	{
		
	}

	public void SetHP(int curHP, int maxHP)
	{
		
	}

	public void SetMP(int curMP, int maxMP)
	{
		
	}

    public void InitResistMagic(int resistValue,BeActor player)
    {

    }

    public void SetBuffName(string text)
    {
        if(buffName!=null)
        buffName.text = text;
    }
}
