using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class FailBossInfo : MonoBehaviour {

    public Image icon;
    public Text level;
    public Text name;
    public Text hp;
    public Slider hpSlider;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetBossInfo(BossInfo info)
    {
        this.icon.sprite = info.icon;
        this.icon.material = info.material;
        this.level.text = string.Format("Lv.{0}", info.level);
        this.name.text = info.name;
        this.hpSlider.value = info.hpRate;
        this.hp.text = string.Format("{0}%", (int)(info.hpRate * 100));
    }
}
