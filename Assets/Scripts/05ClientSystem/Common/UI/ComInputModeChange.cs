using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[ExecuteAlways]
public class ComInputModeChange : MonoBehaviour {

    public Button mButton;
    public Text mText;
    public InputManager.ButtonMode mMode;


	void Start ()
    {
        _reloadButton();
        _updateText();
	}

    private void _reloadButton()
    {
        mButton.onClick.RemoveAllListeners();
        mButton.onClick.AddListener(() =>
        {
            if (BattleMain.instance != null)
            {
                //var input = BattleMain.instance.inputManager;
                //if (mMode != input.currentMode)
                //{
                //    input.currentMode = mMode;
                //    input.ReloadButtons();
                //}
            }
        });
    }

    private void _updateText()
    {
        mText.text = mMode.GetDescription();
    }

    void Update()
    {
#if UNITY_EDITOR
        _updateText();
#endif
    }
}
