using UnityEngine;
using System.Collections;

// user input dat
public class InputUserData : InputBaseData 
{
    public InputSlotSkillMap[] slotMap = new InputSlotSkillMap[0];

    public int normalDataLen = 0;
    public InputSlotClickNormalMap[] normalData = new InputSlotClickNormalMap[0];

    public int normalEightDataLen = 0;
    public InputSlotClickNormalMap[] normalEightData = new InputSlotClickNormalMap[0];
}
