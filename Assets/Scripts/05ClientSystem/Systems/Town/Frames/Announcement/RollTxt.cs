using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RollTxt : MonoBehaviour
{
    //支持中文
    private string txt = "1234567werweerty74874651rtyrghdfgdgdfg891234wrew56789";
    public string showTxt;
    public int showLength = 8;
    public int txtLength;
    public float rollSpeed = 0.1f;
    private int indexMax;
    // Use this for initialization
    void Start()
    {
        txtLength = txt.Length;
        showTxt = txt.Substring(0, showLength);
        indexMax = txtLength - showLength + 1;
    }

    // Update is called once per frame
    void Update()
    {
        GetShowTxt();
    }

    void OnGUI()
    {
        GUI.Box(new Rect(200, 200, 150, 20), showTxt);
    }

    void GetShowTxt()
    {
        if (showLength >= txtLength)
        {
            showTxt = txt;
        }
        else if (showLength < txtLength)
        {
            int startIndex = 0;
            startIndex = (int)(Mathf.PingPong(Time.time * rollSpeed, 1) * indexMax);
            showTxt = txt.Substring(startIndex, showLength);
        }
    }
}
