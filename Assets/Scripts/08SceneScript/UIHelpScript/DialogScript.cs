using UnityEngine;
using System.Collections;
using UnityEngine.UI;

class DialogScript : MonoBehaviour
{
    float lifeTime = 3.5f;
    GameClient.LinkParse compText = null;
	Text compNormalText = null;
	bool isShow = false;
    bool bUseLink = false;
    GameObject goPopDialog;

    public void Initialize(bool bUseLink)
    {
        this.bUseLink = bUseLink;
        goPopDialog = Utility.FindChild(gameObject, "PopUpDialog");
        goPopDialog.CustomActive(true);
        if (bUseLink)
        {
            compText = Utility.FindComponent<GameClient.LinkParse>(gameObject, "PopUpDialog/content", false);
        }
        else
        {
            compNormalText = Utility.FindComponent<Text>(gameObject, "PopUpDialog/content", false);
        }
    }

    public void ShowText(string text,bool bLink = true,float fLifeTime = 3.50f)
    {
        this.lifeTime = fLifeTime;
        if (IsInvoking())
        {
            CancelInvoke();
        }

        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }

        if(bUseLink)
        {
            if(compText != null)
            {
                compText.SetText(text,bLink);
                Invoke("DoHide", lifeTime);
            }
        }
        else
        {
            if (compNormalText != null)
            {
                compNormalText.text = text;
                Invoke("DoHide", lifeTime);
            }
        }
			
		isShow = true;
    }

    public void DoHide()
    {
		if (!isShow)
			return;
		isShow = false;
        gameObject.CustomActive(false);
    }

	public bool IsShow()
	{
		return isShow;
	}
}
