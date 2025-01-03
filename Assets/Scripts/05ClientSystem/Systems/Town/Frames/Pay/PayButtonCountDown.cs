using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PayButtonCountDown : MonoBehaviour {

    public delegate void OnCDOverHandler();
    public OnCDOverHandler onCDOverHandler;

    public int countdownTime;
    public Text countDownText;
    public bool bCDOver = false;

    private float mTimer;
    private int mCDTimer;

    void Awake()
    {
        if (bCDOver)
        {
            if (countDownText)
            {
                countDownText.gameObject.CustomActive(false);
            }
        }

        mCDTimer = countdownTime;
    }

    void Update()
    {
        if (!bCDOver && mCDTimer > 0)
        {
            mTimer += Time.unscaledDeltaTime;
            if (mTimer > 1f)
            {
                mCDTimer -= 1;
                SetCountDownText();
                if (mCDTimer <= 1)
                {
                    if (countDownText)
                    {
                        countDownText.gameObject.CustomActive(false);
                    }
                    if (onCDOverHandler != null)
                    {
                        onCDOverHandler();
                    }
                    bCDOver = true;
                    mCDTimer = countdownTime;
                }
                mTimer = 0f;
            }
        }
    }

    public void StartCountDown()
    {
        bCDOver = false;
        SetCountDownText();
    }

    public void StopCountDown()
    {
        bCDOver = true;
        if (countDownText)
        {
            countDownText.gameObject.CustomActive(false);
        }
    }

    void SetCountDownText()
    {
        if (countDownText)
        {
            countDownText.gameObject.CustomActive(true);
            countDownText.text = string.Format("{0}S", mCDTimer);
        }
    }
}
