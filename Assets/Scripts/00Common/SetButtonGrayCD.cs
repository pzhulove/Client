using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetButtonGrayCD : MonoBehaviour
{

    // Use this for initialization
    public float ButtonCD = 5f;
    public bool BtIsOver = false;
    public Text CDText = null;
    private float curTime = 0.0f;
    private float lastTime = 0.0f;
    private int grayStr = 0;
    void Start()
    {
        curTime = Time.time;
        lastTime = Time.time;
        grayStr = (int)ButtonCD;
        if (CDText)
        {
            CDText.text = grayStr.ToString() + "S";
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (BtIsOver == true)
        {
            this.gameObject.CustomActive(false);
            curTime = Time.time;
            lastTime = Time.time;
            return;
        }
        curTime = Time.time;
        if (curTime - lastTime > 1.0f)
        {
            lastTime = curTime;
            grayStr --;
            CDText.text = grayStr.ToString() + "S";
        }
        if (grayStr<=0)
        {
            BtIsOver = true;
        }
    }

    public void StartGrayCD()
    {
        BtIsOver = false;
        grayStr = (int)ButtonCD;
        this.gameObject.CustomActive(true);
        if (CDText)
        {
            CDText.text = grayStr.ToString() + "S";
        }
    }
}