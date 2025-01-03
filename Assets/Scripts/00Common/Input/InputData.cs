using UnityEngine;
using System.ComponentModel;

/// <summary>
/// 技能槽 和 按钮名字，键盘按键 映射
/// </summary>
[System.Serializable]
public class InputSlotKeyNameMap
{
    public int slot = 1;
    // 按键映射
    public KeyCode code = KeyCode.Space;
    // 预制体名字
    public string name = "";
}

/// <summary>
/// 技能槽 和 技能 的映射
/// </summary>
[System.Serializable]
public class InputSlotSkillMap
{
    public int slot = 1;
    // TODO according the skill id to find out the skill icon
    public int skillID = 0;
}

/// <summary>
/// 技能连招数据
/// </summary>
[System.Serializable]
public class InputSkillComboData
{
    public int slot;
    public int time; // ms
}

/// <summary>
/// 按钮数据
/// 
/// 按钮响应 和 触发技能槽 的映射
/// </summary>
[System.Serializable]
public class InputSlotClickNormalMap
{
    public string name;

    #region normal mode
    public int normalClickSlot = 1;
    #endregion
}

[System.Serializable]
public class InputSlotClickNormalCatlikeMap
{
    public string name;

    #region normal mode
    public int normalClickSlot = 1;
    public int normalClickCatlikeSlot = 1;
    #endregion
}



[System.Serializable]
public class InputSlotClickDragMap
{
    public string name;

    #region drag mode
    public bool isClick = true;
    public int dragClickSlot = 1;

    // 向下拉
    public bool isDragDown = false;
    public InputSkillComboData[] drageDownList = new InputSkillComboData[0];

    // 向上拉
    public bool isDragUp = false;
    public InputSkillComboData[] drageUpList = new InputSkillComboData[0];
    #endregion
}

public enum InputDirection
{
    [Description("←")]
    Left = 0,
    [Description("↑")]
    Top = 1,
    [Description("→")]
    Right = 2,
    [Description("↓")]
    Buttom = 3,
};

[System.Serializable]
public class InputSlotDirectionUnit
{
    public InputDirection dir;
    public int time;
}

[System.Serializable]
public class InputSkillDirectionComboData
{
    public int slot;
    public InputSlotDirectionUnit[] combo = new InputSlotDirectionUnit[0]; 
}

[System.Serializable]
public class InputSlotDirectionMap
{
    public string name;

    public bool isNormalSlot;
    public int normalClickSlot;

    public bool isDirectionCombo;
    public InputSkillDirectionComboData[] directionComboData = new InputSkillDirectionComboData[0]; 
}

[System.Serializable]
public class InputSlotContinueClickMap
{
    public string name;

    // TODO normal 
    public bool isNormal = true;
    public int normalClickSlot = 0;

    #region click mode
    public bool isConClick = false;
    public InputSkillComboData[] skillComboData = new InputSkillComboData[0];
    #endregion
}

public class InputBaseData : ScriptableObject
{
    public GameObject normalPrefab;
    public GameObject normalEightPrefab;
    public InputManager.ButtonMode buttonMode;
}

// global input data
public class InputData : InputBaseData {
    public InputSlotKeyNameMap[] inputData = new InputSlotKeyNameMap[0];
}
