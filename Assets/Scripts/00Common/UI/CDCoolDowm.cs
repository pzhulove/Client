using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CDCoolDowm : MonoBehaviour {

    private Image cdImage;
    private Text cdText;

    private float totalTime = 10;

    private bool startCD = false;

    public float surplusTime = 0;

    void Start () {

        cdImage = GetComponent<Image>();

        cdText = GetComponentInChildren<Text>();
	}

    float timer = 0;
	// Update is called once per frame
	void Update () {

        if (startCD)
        {
            timer += Time.deltaTime;
            surplusTime = totalTime - timer;
            if (surplusTime < 0)
            {
                startCD = false;
                timer = 0;
                this.gameObject.SetActive(false);
                return;
            }
            cdImage.fillAmount = surplusTime / totalTime;
            cdText.text = (Mathf.CeilToInt( surplusTime)).ToString();

        }
		
	}

    public void StartCD(float totalTime)
    {
        this.totalTime = totalTime;
        startCD = true;
        this.gameObject.SetActive(true);
    }

    //重置CD
    public void ResetCD()
    {
        this.totalTime = 0;
        startCD = true;
    }

}
