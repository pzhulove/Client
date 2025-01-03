using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG;
using GameClient;
using DG.Tweening;

public class FinalTestPlayerInfo : MonoBehaviour {

    public Text lv;
    public Image icon;
    public Text name;
    public Slider hpSlider;
    public Text hp;
    public Slider mpSlider;
    public Text mp;
    private FinalPlayerInfo info;
    // Use this for initialization
    void Start () {
        Invoke("PlayTween",5.0f);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetPlayerInfo(FinalPlayerInfo info)
    {
        this.info = info;
        lv.text = ""+ info.level.ToString();
        icon.sprite = info.icon;
        icon.material = info.material;
        name.text = info.name;
        hpSlider.value = (float)info.hp / info.maxHp;
        hp.text = (int)((info.hp /(float)info.maxHp)*100)+"%";
        mpSlider.value = (float)info.mp / info.maxMp;
        mp.text = (int)((info.mp / (float)info.maxMp) * 100)+"%";
    }

    public void PlayTween()
    {
        if (info != null)
        {
            float value = (info.hp + info.addHp) / (float)info.maxHp;
            hpSlider.DOValue(value, 1.0f);
            float _value = (info.mp + info.addMp) / (float)info.maxMp;
            mpSlider.DOValue(_value, 1.0f);

            int number = (int)((info.hp / (float)info.maxHp) * 100);
            int endNumber = (int)(((info.hp + info.addHp) / (float)info.maxHp) * 100);
            Tween t = DOTween.To(() => number, x => number = x, endNumber, 1);
            t.OnUpdate(() => UpdateHpTween(number));

            int _number = (int)((info.mp / (float)info.maxMp) * 100);
            int _endNumber = (int)(((info.mp + info.addMp) / (float)info.maxMp) * 100);
            Tween _t = DOTween.To(() => _number, x => _number = x, _endNumber, 1);
            _t.OnUpdate(() => UpdateMpTween(_number));
        }
    }

    private void UpdateHpTween(int num)
    {
        num = Mathf.Clamp(num, 0, 100);
        hp.text = num+"%";
    }

    private void UpdateMpTween(int num)
    {
        num = Mathf.Clamp(num, 0, 100);
        mp.text = num + "%"; ;
    }
}
