using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FpsShow : MonoBehaviour
{
    private Rect _fpsRect = new Rect(25, Screen.height - 60, 100, 60);
    private int _fps = 0;
    private Color _fpsColor = Color.white;
    private int _frameNumber = 0;
    private float _lastShowFPSTime = 0f;
    public bool showFps = false;
    // Update is called once per frame
    void Update()
    {
        if (showFps)
        {
            _frameNumber += 1;
            float time = Time.realtimeSinceStartup - _lastShowFPSTime;
            if (time >= 1)
            {
                _fps = (int)(_frameNumber / time);
                _frameNumber = 0;
                _lastShowFPSTime = Time.realtimeSinceStartup;
            }
        }
    }
    private void OnGUI()
    {
        _fpsRect = new Rect(25, Screen.height - 60, 100, 60);
        if (_fps >= 18)
        {
            _fpsColor = Color.Lerp(Color.yellow, Color.green, (_fps - 18) / 12f);
        }
        else if (_fps >= 10)
        {
            _fpsColor = Color.Lerp(Color.red, Color.yellow, (_fps - 10) / 8f);
        }
        else
            _fpsColor = Color.red;
        Color color = GUI.contentColor;
        GUIStyle style = GUI.skin.GetStyle("label");
        int fontSize = style.fontSize;
        GUI.contentColor = _fpsColor;
        style.fontSize = 20;
        GUI.Label(_fpsRect, "FPS:" + _fps.ToString());
        GUI.contentColor = color;
        style.fontSize = fontSize;
    }

}
