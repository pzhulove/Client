using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetButtonCD : MonoBehaviour {

    // Use this for initialization
    public float ButtonCD = 0.5f;
    public bool BtIsWork = false;
    public UIGray uiGray = null;
    public Button btn = null;
    public Text mText = null;

    private float curTime = 0.0f;
    private float lastTime = 0.0f;
    private string oldText = null;
	void Start ()
    {
        BtIsWork = true;
        curTime = 0;
        lastTime = 0;
	}
	
	// Update is called once per frame
	void Update ()
    {
        if(BtIsWork == true)
        {
            curTime = Time.time;
            lastTime = Time.time;
            if(uiGray != null && btn != null )
            {
                uiGray.enabled = false;
                btn.interactable = true;
            }
            return;
        }
        curTime = Time.time;
        if (curTime - lastTime > ButtonCD)
        {
            lastTime = curTime;
            BtIsWork = true;
            if(oldText != null && oldText != "")
            {
                mText.text = oldText;
            }
        }
        else
        {
            float mTime = ButtonCD - (curTime - lastTime);
            int m_ITime = (int)mTime;
            if(mText != null)
            {
                mText.text = oldText + m_ITime.ToString() + "S";
            }
        }
    }

    public void StartBtCD()
    {
        BtIsWork = false;
        if(uiGray != null && btn != null && mText != null)
        {
            uiGray.enabled = true;
            btn.interactable = false;
            oldText = mText.text;
        }
    }
}
