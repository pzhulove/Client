using UnityEngine;
using System.Collections;

using UnityEngine.UI;

[ExecuteAlways]
public class CPKHPBar : MonoBehaviour {

    public enum PKBarType { Left, Right};

    public PKBarType type = PKBarType.Left;
    private PKBarType lastType = PKBarType.Left;
    public System.Action action = null;
    void Awake()
    {
        _bindExUI();
    }

    void OnDestroy()
    {
        _unbindExUI();
    }

    public ComCommonBind mBind;

#region ExtraUIBind
    private Text mName = null;
    private Image mIcon = null;
    private Text mHpValue = null;
    private Slider mHpBar = null;
    private Slider mMpBar = null;
    private Slider mProtectBlue = null;
    private GameObject mProtectBlueRoot = null;
    private Slider mProtectYellow = null;
    private GameObject mProtectYellowRoot = null;
    private Slider mProtectRed = null;
    private GameObject mProtectRedRoot = null;
    private Button btn = null;
    protected void _bindExUI()
    {
        mName = mBind.GetCom<Text>("name");
        mIcon = mBind.GetCom<Image>("icon");
        mHpValue = mBind.GetCom<Text>("hpValue");
        mHpBar = mBind.GetCom<Slider>("hpBar");
        mMpBar = mBind.GetCom<Slider>("mpBar");
        mProtectBlue = mBind.GetCom<Slider>("protectBlue");
        mProtectBlueRoot = mBind.GetGameObject("protectBlueRoot");
        mProtectYellow = mBind.GetCom<Slider>("protectYellow");
        mProtectYellowRoot = mBind.GetGameObject("protectYellowRoot");
        mProtectRed = mBind.GetCom<Slider>("protectRed");
        mProtectRedRoot = mBind.GetGameObject("protectRedRoot");
        btn = mIcon.GetComponent<Button>();
        if (btn == null) return;
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(()=> 
        {
            if (action != null)
            {
                action();
            }
        });
    }

    protected void _unbindExUI()
    {
        mName = null;
        mIcon = null;
        mHpValue = null;
        mHpBar = null;
        mMpBar = null;
        mProtectBlue = null;
        mProtectBlueRoot = null;
        mProtectYellow = null;
        mProtectYellowRoot = null;
        mProtectRed = null;
        mProtectRedRoot = null;
    }
#endregion   
    public float GetHPPercent()
    {
        if (null != mHpBar)
        {
            return mHpBar.value;
        }

        return 0.0f;
    }

    public float GetMPPercent()
    {
        if (null != mMpBar)
        {
            return mMpBar.value;
        }

        return 0.0f;
    }

    public void SetHPPercent(float percent)
    {
        if (null != mHpBar)
        {
            mHpBar.value = Mathf.Clamp01(percent);
        }
    }

    public void SetHPValue(int cur, int max)
    {
        if (null != mHpValue)
        {
            mHpValue.text = string.Format("{0}/{1}", cur, max);
        }
    }

    public void SetMPPercent(float percent)
    {
        if (null != mMpBar)
        {
            mMpBar.value = Mathf.Clamp01(percent);
        }
    }

    public void SetNameText(string name, string area)
    {
        if (null != mName)
        {
            mName.text = string.Format("{0} <color=orange><b>{1}</b></color>", name, area);
        }
    }

    public void SetIcon(Sprite sprite, Material material)
    {
        if (null != mIcon && sprite != null)
        {
            mIcon.sprite = sprite;
            mIcon.material = material;
        }
    }

    public void HiddentAllProtectTips()
    {
        mProtectRedRoot.SetActive(false);
        mProtectBlueRoot.SetActive(false);
        mProtectYellowRoot.SetActive(false);
    }

    private bool _isValidValue(float percent)
    {
        float np = mHpBar.value - percent;
        if (np < 0.0f || np > 1.0f)
        {
            return false;
        }

        return true;
    }

    public void ShowProtectStand(bool show, float percent=0)
    {
		if (show)
		{
			if (!_isValidValue(percent)) { return ; }

			mProtectYellowRoot.CustomActive(true);
			mProtectYellow.value = mHpBar.value - percent;
		}
		else 
		{
			mProtectYellowRoot.CustomActive(false);
		}

        
    }

    public void ShowProtectFloat(bool show, float percent=0)
    {
        if (show)
        {
            if (!_isValidValue(percent)) { return ; }

            mProtectBlueRoot.CustomActive(true);
            mProtectBlue.value = mHpBar.value - percent;
        }
        else 
        {
            mProtectBlueRoot.CustomActive(false);
        }
    }

    public void ShowProtectGround(bool show, float percent=0)
    {
		if (show)
		{
			if (!_isValidValue(percent)) { return ; }

			mProtectRedRoot.CustomActive(true);
			mProtectRed.value = mHpBar.value - percent;
		}
		else 
		{
			mProtectRedRoot.CustomActive(false);
		}
    }
}
