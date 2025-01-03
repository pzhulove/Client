using UnityEngine;
using System.Collections;

[ExecuteAlways]
public class ETCButtonModeSlider : MonoBehaviour {

    public float radius = 300.0f;
    public Vector2 offset;
    public ETCButton centerButton;

    public ETCButton[] buttonList = new ETCButton[0];

	// Use this for initialization
	void Start () {
	
	}

    void UpdateButtonPosition()
    {
        if (buttonList == null) return;

        int len = buttonList.Length;

        for (int i = 0; i < len; i++)
        {
            var button = buttonList[i];
            button.anchor = ETCBase.RectAnchor.BottonRight;

            float h = Mathf.Cos(Mathf.Deg2Rad * 90 / (len - 1) * i) * radius;
            float w = Mathf.Sin(Mathf.Deg2Rad * 90 / (len - 1) * i) * radius;

            button.anchorOffet = new Vector2(w, h) + offset;
            button.isSwipeOut = true;
        }

        centerButton.anchorOffet = offset;
    }
	
	// Update is called once per frame
	void Update () {
        UpdateButtonPosition();
	}
}
