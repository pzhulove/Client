using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameClient;
using ProtoTable;
public class SkillComboFrame : ClientFrame
{
    ComboData[] tabelList;
    private GameObject skillComboItem;
    public GameObject skillComboContainer;
    public override string GetPrefabPath()
    {
        return "UIFlatten/Prefabs/BattleUI/SkillComboFrame";
    }

    protected override void _OnOpenFrame()
    {
        base._OnOpenFrame();
    }

    protected override void _bindExUI()
    {
        base._bindExUI();
        skillComboItem = mBind.GetGameObject("skillComboItem");
        skillComboContainer = mBind.GetGameObject("skillComboItemContainer");
    }

    protected override void _OnCloseFrame()
    {
        base._OnCloseFrame();
        DestroySkillComboItem();
    }

    protected override void _unbindExUI()
    {
        base._unbindExUI();
    }



    #region 技能连招UI
    List<SkillComboItem> comboItemList = new List<SkillComboItem>();

    public void InitSkillComboUI(int jobID, int roomID, bool showArrow = true)
    {
        DestroySkillComboItem();
        TrainingSkillComboBattle battle = BattleMain.instance.GetBattle() as TrainingSkillComboBattle;
        if (battle == null) return;
        tabelList = battle.teachData.datas;
        for (int i = 0; i < tabelList.Length; i++)
        {
            ComboData data = tabelList[i];
            if (data.showUI == 0) continue;
            GameObject obj = GameObject.Instantiate(skillComboItem);
            obj.SetActive(true);
            Utility.AttachTo(obj, skillComboContainer);
            SkillComboItem item = obj.GetComponent<SkillComboItem>();
            if (item != null)
            {
                item.InitItem(data.skillGroupID, data.skillID, "", data.waitInputTime, data.sourceID, data.phase, showArrow);
                comboItemList.Add(item);
            }

        }

    }

    private void DestroySkillComboItem()
    {
        for (int i = 0; i < comboItemList.Count; i++)
        {
            GameObject.Destroy(comboItemList[i].gameObject);
        }
        comboItemList.Clear();
    }

    public SkillComboItem GetShowItem(int index)
    {
        if (index >= comboItemList.Count) return null;
        return comboItemList[index];
    }

    public List<SkillComboItem> GetComboItemList()
    {
        return comboItemList;
    }

    public void ShowComboItemEffect(int id)
    {
        for (int i = 0; i < comboItemList.Count; i++)
        {
            if (comboItemList[i].groupID == id)
            {
                   comboItemList[i].ShowEffect();
            }
        }

    }

    #endregion
}
