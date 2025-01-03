using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using GameClient;

public class ComboCount : MonoBehaviour
{
    public Slider timeSlider;
    public Image imgCount;
    public ImageNumber imageNumber;

    public RectTransform mRootRect;

    public DOTweenAnimation[] allAnimate = new DOTweenAnimation[0];

    private Vector3 offsetMin;
    private Vector3 offsetMax;

    private float ComboDeltaTime = 1.25f;

    private float fDeltaTime = 0;

    void Awake()
    {
        if (null != mRootRect)
        {
            offsetMin = mRootRect.offsetMin;
            offsetMax = mRootRect.offsetMax;
        }
    }

    private void _resetCombo()
    {
        imageNumber.enabled = false;
        timeSlider.enabled = false;
        imgCount.enabled = false;

        if (null != mRootRect)
        {
            mRootRect.offsetMin = offsetMin;
            mRootRect.offsetMax = offsetMax;
        }
    }

    private void _playerAnimate()
    {
        for (int i = 0; i < allAnimate.Length; ++i)
        {
            if (null != allAnimate[i] && allAnimate[i].isActive)
            {
                allAnimate[i].DORestart();
            }
        }
    }

    void Start()
    {
        _resetCombo();
    }

    public void Feed(int combo)
    {
        fDeltaTime = ComboDeltaTime;

        if (combo > 1)
        {
            if (!imageNumber.enabled)
            {
                _playerAnimate();
            }

            imageNumber.enabled = true;
            imgCount.enabled = true;
            imageNumber.SetTextNumber(combo, true);

            if (combo > BattleDataManager.GetInstance().BattleInfo.maxComboCount)
            {
                BattleDataManager.GetInstance().BattleInfo.maxComboCount = combo;
            }
        }

        timeSlider.enabled = false;
    }

    public void StopFeed()
    {
        fDeltaTime = 0;

        _resetCombo();
    }

    void Update()
    {
        if (fDeltaTime > 0)
        {
            fDeltaTime -= Time.deltaTime;

            if (fDeltaTime > 0)
            {
                timeSlider.enabled = true;
                timeSlider.value = fDeltaTime / ComboDeltaTime;
            }
        }
    }
}
