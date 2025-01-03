using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CStateBar : MonoBehaviour
{
    public enum eBarColor
    {
        None = -1,
        Yellow,
        Red
    }

    Text text;
    Text timer;
    Slider slider;
    Image barImage;

    public CanvasGroup root;
    public Sprite[] spriteList;

    public void Awake()
    {
        text = transform.Find("board/text").GetComponent<Text>();
        timer = transform.Find("board/timer").GetComponent<Text>();
        slider = transform.Find("board/bar").GetComponent<Slider>();
        barImage = transform.Find("board/bar").GetComponent<Image>();

        SetActive(false);
    }

    public void SetActive(bool active)
    {
        if (null != this && null != root)
        {
            root.alpha = (active) ? 1.0f : 0.0f;
        }
    }

    public bool GetActive()
    {
        if (null != this && null != root)
        {
            return root.alpha == 1.0f;
        }
        return false;
    }

    public void SetStateBarInfo(string t, eBarColor c)
    {
        if (text != null)
        {
            text.text = t;
        }

        if (barImage != null && spriteList.Length > (int)c)
        {
            barImage.sprite = spriteList[(int)c];
        }
    }

    public void SetPercent(float percent)
    {
        if (slider != null)
        {
            slider.value = percent;
        }
    }

    public void SetTimeText(int time)
    {
        if (time < 0)
        {
            timer.text = "";
        }
        else if (timer != null)
        {
            //int ms = time % 1000;
            //time /= 1000;
            //int m = time / 60;
            //int s = time % 60;
            //timer.text = m + ":" + s;

            timer.text = (time / 1000f).ToString("F1");
        }
    }
}
