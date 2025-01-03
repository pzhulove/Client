using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine.UI;
using GameClient;

public class ComShowText : MonoBehaviour
{
    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {

    }

    Text control;
    string text;
    float speed;
    float delay;
    IClientFrame frame;
    
    public void Begin(Text control, string text, float speed, float delay, IClientFrame frame)
    {
        control.text = "";
        this.control = control;
        this.text = text;
        this.speed = speed;
        this.delay = delay;
        this.frame = frame;
        StartCoroutine(_update());
    }

    int getShowCount(float delta)
    {
        if (speed <= 0)
        {
            return text.Length;
        }
        return (int)(delta / speed);
    }
    
    IEnumerator _update()
    {
        float begin = Time.realtimeSinceStartup;
        int countshow = getShowCount(Time.deltaTime);
        if (countshow >= text.Length)
        {
            control.text = text;
        }
        else
        {
            while (control.text.Length < text.Length)
            {
                while (control.text.Length < countshow && control.text.Length < text.Length)
                {
                    control.text += text[control.text.Length];
                }
                yield return Yielders.EndOfFrame;
                countshow = getShowCount(Time.realtimeSinceStartup - begin);
            }
        }

        if(delay < 0)
        {
            yield break;
        }

        yield return Yielders.GetWaitForSeconds(delay);
        frame.Close();
        yield break;
    }

}