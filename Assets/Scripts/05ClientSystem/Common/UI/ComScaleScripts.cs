using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ComScaleScripts : MonoBehaviour
{
    Vector3 baseScale = Vector3.one;

    public RectTransform mRect;

    public void SetBaseScale(float s)
    {
        baseScale = Vector3.one * s;
    }

    public void Scale(float s)
    {
        var scale = mRect.localScale;
        mRect.localScale = baseScale * s;

        ETCJoystick joystick = GetComponent<ETCJoystick>();
        if (joystick != null)
        {
            Image thumbImage = joystick.thumb.GetComponent<Image>();
            if (thumbImage != null)
            {
                thumbImage.transform.localScale = Vector3.one / s;
            }
        }
    }

    public void SetAlpha(float alpha)
    {
        ETCJoystick joystick = GetComponent<ETCJoystick>();
        if (joystick != null)
        {
            Image image = joystick.GetComponent<Image>();
            if (image != null)
            {
                image.CrossFadeAlpha(alpha, 0.1f, true);
            }
        }
    }

    public void SetThumbAlpha(float alpha)
    {
        ETCJoystick joystick = GetComponent<ETCJoystick>();
        if (joystick != null)
        {
            Image thumbImage = joystick.thumb.GetComponent<Image>();
            if (thumbImage != null)
            {
                thumbImage.CrossFadeAlpha(alpha, 0.1f, true);
            }
        }
    }

    public void SetDirActive(bool active)
    {
        ETCJoystick joystick = GetComponent<ETCJoystick>();
        if (joystick != null)
        {
            if (joystick.dirObj != null)
            {
                joystick.dirObj.CustomActive(active);
            }
        }
    }

}
