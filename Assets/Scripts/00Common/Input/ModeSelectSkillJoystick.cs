#if !LOGIC_SERVER
using System.Collections.Generic;
using GameClient;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 模式选择摇杆
/// </summary>
public class ModeSelectSkillJoystick : SkillJoystick
{
    struct UIModeItem
    {
        public ComCommonBind bind;
        public Image Icon;
        public RectTransform Select;
        public TextEx Name;
        public float Angle;
    }
    private List<UIModeItem> m_ModeItems = new List<UIModeItem>();
    private int m_LastModeSelect;
    private int m_ModeSelectDegree;

    public override string PrefabPath => "UIFlatten/Prefabs/ETCInput/HGJoystickModeSelect";
    public override void OnStart()
    {
        base.OnStart();
        if (m_Skill == null)
            return;

        var modeSkill = m_Skill as IModeSelectSkill;
        if (modeSkill == null)
            return;
        var info = modeSkill.GetModeItemInfos();
        m_ModeSelectDegree = modeSkill.GetDegree();
        InitUI(info, m_ModeSelectDegree);
    }

    public override void OnMove(int vx, int vy, int degree)
    {
        int index = GetModeSelectIndexByDegree(degree);
        if (index == m_LastModeSelect)
            return;
        ChangeModeSelect(index);
    }

    public override void OnRelease(int degree, bool hasDrag, float duration)
    {
        if (m_Skill != null)
        {
            int index = GetModeSelectIndexByDegree(degree);
            InputManager.instance.CreateSkillFrameCommand(m_Skill.skillID, new SkillFrameCommand.SkillFrameData
            {
                type = SkillFrameCommand.SkillFrameDataType.Joystick_ModeIndex, 
                data1 = (uint) index
            });
        }
    }
    
    //改变选择的玩家
    private void ChangeModeSelect(int curIndex)
    {
        if(curIndex >= m_ModeItems.Count)
            return;
        
        m_ModeItems[m_LastModeSelect].Select.gameObject.SetActive(false);
        m_ModeItems[curIndex].Select.gameObject.SetActive(true);
        m_LastModeSelect = curIndex;
    }
    
    private int GetModeSelectIndexByDegree(int degree)
    {
        degree -= 180;
        for (int i = 0; i < m_ModeItems.Count; i++)
        {
            var item = m_ModeItems[i];
            if (degree > (item.Angle - m_ModeSelectDegree / 2) && degree < (item.Angle + m_ModeSelectDegree / 2))
            {
                return i;
            }
        }
        return 0;
    }

    private void InitUI(ModeSelectItemInfo[] infos, int degree)
    {
        var commonBind = m_JoystickGo.GetComponent<ComCommonBind>();
        if(commonBind == null)
            return;
        
        var modeSelectNode = commonBind.GetCom<RectTransform>("Node");
        var modeItemTemplate = commonBind.GetGameObject("Item");
        if (modeSelectNode == null || modeItemTemplate == null)
            return;
        
        for (int i = 0; i < infos.Length; i++)
        {
            var info = infos[i];
            UIModeItem item = default;
            if (i < m_ModeItems.Count)
            {
                item = m_ModeItems[i];
            }
            else
            {
                var itemGo = GameObject.Instantiate(modeItemTemplate, modeSelectNode, true);
                if (itemGo != null)
                {
                    item = new UIModeItem();
                    var bind = itemGo.GetComponent<ComCommonBind>();
                    item.Angle = degree * i * -1;
                    item.bind = bind;
                    item.Icon = bind.GetCom<Image>("Icon");
                    item.Select = bind.GetCom<RectTransform>("Select");
                    item.Name = bind.GetCom<TextEx>("Name");
                    m_ModeItems.Add(item);
                }
            }

            item.bind.gameObject.SetActive(true);
            var transform = item.bind.transform;
            var angles = transform.eulerAngles;
            angles.z = item.Angle;
            transform.eulerAngles = angles;
            item.Select.gameObject.SetActive(false);
            item.Name.text = info.Name;
            ETCImageLoader.LoadSprite(ref item.Icon, info.IconPath);
        }
    }

    public override void OnClear()
    {
        base.OnClear();
        m_ModeItems?.Clear();
    }
}
#endif
