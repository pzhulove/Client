using UnityEngine;
using System.Collections;

public class CHPBar : MonoBehaviour {

    RectTransform rectBG;
    RectTransform rectBar;

    GameObject objBG = null;
    GameObject objBar = null;

    int totalWidth;

    float percent = 1.0f;
    float offsetPercent = 0;

	// Use this for initialization
	void Start () {
        objBG = transform.Find("bg").gameObject;
        objBar = transform.Find("bar").gameObject;

        rectBG = objBG.GetComponent<RectTransform>();
        rectBar = objBar.GetComponent<RectTransform>();

        RectTransform rectSelf = GetComponent<RectTransform>();
        totalWidth = (int)rectSelf.rect.width;


        SetPercent(percent);
	}

	// Update is called once per frame
	void Update () {
	
	}


    public void SetPercent(float per)
    {
        percent = per;

        if (rectBar != null)
            SetBarShow(rectBar, (int)(percent * totalWidth), (int)(offsetPercent * totalWidth));
    }

    private void SetBarShow(RectTransform rt, int width, int offset)
    {
        var r = rt.offsetMin;
        r.x = offset;
        rt.offsetMin = r;

        r = rt.offsetMax;
        r.x = totalWidth - (width + offset);
        rt.offsetMax = -r;
    }
}
