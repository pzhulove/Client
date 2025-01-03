using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameClient;
public class SkillComboItem : MonoBehaviour
{

    public GameObject arrow;
    public GameObject unSelectState;
    public Image skillIcon;
    public Image mask;
    public Image cdImage;
    public Text phase;
    public GameObject phaseBg;
    public GameObject state1;
    public GameObject state2;
    public GameObject state3;
    public GameObject effect;

    [NonSerialized]
    public int skillID;
    [NonSerialized]
    public bool startCD = false;

    [NonSerialized]
    public int groupID = 0;
    private float totaltime = 0;
    public static Action timeOutCallBack;
    private bool showArrow = true;
    public void InitItem(int groupID, int skillD, string desc, float waitInputTime, int sourceID = 0, int phase = 0, bool showArrow = true)
    {
        this.groupID = groupID;
        state1.CustomActive(showArrow);
        this.showArrow = showArrow;
        phaseBg.CustomActive(phase != 0);
        this.phase.CustomActive(phase != 0);
        this.phase.text = phase.ToString();
        totaltime = waitInputTime / 1000.0f;
        mask.CustomActive(false);
        skillID = skillD;
        var skillData = TableManager.GetInstance().GetTableItem<ProtoTable.SkillTable>(sourceID == 0 ? skillD : sourceID);
        TrainingSkillComboBattle battle = BattleMain.instance.GetBattle() as TrainingSkillComboBattle;
        if (battle.IsLastCombo(groupID))
        {
            arrow.CustomActive(false);
        }
        ETCImageLoader.LoadSprite(ref skillIcon, skillData.Icon);
    }

    public void StartComboCD()
    {

        if (BattleMain.battleType != BattleType.TrainingSkillCombo) return;
        time = 0;
        startCD = true;
        cdImage.gameObject.CustomActive(startCD);
    }

    public void StopComboCD()
    {

        if (BattleMain.battleType != BattleType.TrainingSkillCombo) return;

        startCD = false;
        cdImage.gameObject.CustomActive(startCD);
        ShowPassSelect();
    }

    float time = 0;
    void Update()
    {
        if (startCD)
        {
            time += Time.deltaTime;
            cdImage.fillAmount = (totaltime - time) / totaltime;
            if (time > totaltime)
            {
                if (timeOutCallBack != null)
                    timeOutCallBack();
                time = 0;
                StopComboCD();
                ShowError();
            }
        }
    }


    public void SelectCurrent()
    {
        mask.CustomActive(false);
    }

    public void ShowEffect()
    {
        effect.CustomActive(true);
    }

    private void ShowPassSelect()
    {
        state1.CustomActive(false);
        state2.CustomActive(true);
        state3.CustomActive(false);
    }

    private void ShowError()
    {
        state1.CustomActive(false);
        state2.CustomActive(false);
        state3.CustomActive(true);
    }

}
