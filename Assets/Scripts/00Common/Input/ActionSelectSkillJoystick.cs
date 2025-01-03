#if !LOGIC_SERVER

using System.Collections.Generic;
using GameClient;
using UnityEngine;
using UnityEngine.UI;

public class ActionSelectSkillJoystick : SkillJoystick
{
    class ActionSelectData
    {
        public GameObject skillObj;
        public GameObject highLightObj;
        public Image icon;
        public Image highLight_L;
        public Image highLight_R;

        public void SetActive(bool active)
        {
            if (skillObj != null)
            {
                skillObj.CustomActive(active);
            }
            if (highLightObj != null)
            {
                highLightObj.CustomActive(active);
            }
        }

        public void SetSelectState(bool state)
        {
            if (highLight_L != null)
            {
                highLight_L.color = state ? Color.white : Color.black;
            }
            if (highLight_R != null)
            {
                highLight_R.color = state ? Color.white : Color.black;
            }
        }

        public void SetIcon(string path)
        {
            icon.SafeSetImage(path);
        }
    }

    List<ActionSelectData> dataList = new List<ActionSelectData>();

    GameObject selectObj;

    int selectCount = 4;

    int actionCount;

    int curIndex;

    public override string PrefabPath => "UIFlatten/Prefabs/ETCInput/HGJoystickSelectAction";

    public override void OnStart()
    {
        InitData();
    }

    void InitData()
    {
        if (m_Skill == null)
        {
            return;
        }

        if (m_Skill.actionSelect == null || m_Skill.actionIconPath == null)
        {
            return;
        }

        var bind = m_JoystickGo.GetComponent<ComCommonBind>();

        if (bind == null)
        {
            return;
        }

        selectObj = bind.GetGameObject("Select");

        if (dataList.Count < selectCount)
        {
            dataList.Clear();
            for (int i = 0; i < selectCount; i++)
            {
                var data = new ActionSelectData()
                {
                    highLightObj = bind.GetGameObject($"HighLight{i}"),
                    skillObj = bind.GetGameObject($"Skill{i}"),
                    icon = bind.GetCom<Image>($"Icon{i}"),
                    highLight_L = bind.GetCom<Image>($"HighLight{i}_L"),
                    highLight_R = bind.GetCom<Image>($"HighLight{i}_R"),
                };
                dataList.Add(data);
            }
        }

        actionCount = m_Skill.actionIconPath.Length;
        if (actionCount > selectCount)
        {
            actionCount = selectCount;
        }
        SetActionSelectCount(actionCount);

        for (int i = 0; i < actionCount; i++)
        {
            dataList[i].SetIcon(m_Skill.actionIconPath[i]);
        }

        curIndex = 0;
        SetItemSelectState(0);
    }

    void SetItemSelectState(int index)
    {
        if (dataList == null)
        {
            return;
        }

        if (dataList.Count < selectCount)
        {
            return;
        }

        for (int i = 0; i < selectCount; i++)
        {
            dataList[i].SetSelectState(i == index);
        }

        int degree = GetDegreeByIndex(index);
        if (degree >= 0)
        {
            selectObj.CustomActive(true);
            selectObj.transform.rotation = Quaternion.Euler(Vector3.forward * (degree - 90));
        }
        else
        {
            selectObj.CustomActive(false);
        }
    }

    void SetActionSelectCount(int actionCount)
    {
        if (dataList == null)
        {
            return;
        }

        Logger.LogError($"actionCount {actionCount}");

        for (int i = 0; i < dataList.Count; i++)
        {
            dataList[i].SetActive(i < actionCount);
        }
    }

    public override void OnMove(int vx, int vy, int degree)
    {
        int index = GetIndexByDegree(degree);
        //if (index >= 0)
        {
            curIndex = index;
        }
        SetItemSelectState(curIndex);
    }

    public override void OnRelease(int degree, bool hasDrag, float duration)
    {
        if (m_Skill != null)
        {
            int index = GetIndexByDegree(degree);
            if (index >= 0)
            {
                InputManager.instance.CreateSkillFrameCommand(m_Skill.skillID, new SkillFrameCommand.SkillFrameData
                {
                    type = SkillFrameCommand.SkillFrameDataType.Joystick_ActionIndex,
                    data1 = (uint)index,
                });
            }
        }
    }

    int GetIndexByDegree(int degree)
    {
        int index = -1;
        if (degree > 150 && degree <= 210)
            index = 0;
        if (degree > 90 && degree <= 150)
            index = 1;
        if (degree > 30 && degree <= 90)
            index = 2;
        if (degree > 330 || degree <= 30)
            index = 3;

        if (index >= actionCount)
            index = -1;

        return index;
    }

    int GetDegreeByIndex(int index)
    {
        if (index >= actionCount)
            return -1;

        if (index == 0)
            return 180;
        if (index == 1)
            return 120;
        if (index == 2)
            return 60;
        if (index == 3)
            return 0;

        return -1;
    }

    public override void OnClear()
    {
        dataList.Clear();
    }
}

#endif