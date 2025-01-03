using UnityEngine;
using System.Collections;
///////删除linq
using System.Collections.Generic;


public class CombineSkillUnit
{
    public int time;
    public int slot;
}

public class InputButtonSliderMode
{
    Queue<Queue<CombineSkillUnit>> queue = new Queue<Queue<CombineSkillUnit>>();
    Queue<CombineSkillUnit> lastSkillList = null;
    CombineSkillUnit lastSkillUnit = null;
    
    public void AddComboSkill(CombineSkillUnit[] comboList)
    {
        if (comboList == null)
        {
            return ;
        }
        
        Queue<CombineSkillUnit> list = new Queue<CombineSkillUnit>();
        
        for (int i = 0; i < comboList.Length; i++) {
            if (comboList[i] != null) {
                list.Enqueue(comboList[i]);                
            }
        }
        
        queue.Enqueue(list);
    }
    
    public void Update(int delta)
    {
        while (queue.Count > 0)
        {
            if (lastSkillList == null)
            {
                lastSkillList = queue.Dequeue();
            }
            
            while (lastSkillList != null && lastSkillList.Count > 0)
            {
                if (lastSkillUnit == null) {
                    lastSkillUnit = lastSkillList.Dequeue();
                }
                
                lastSkillUnit.time -= delta;
                
                if (lastSkillUnit.time <= 0) {
                    // check the slot skill can use
                    
                    // if can use 
                    // use the skill by the slot
                    int slot = lastSkillUnit.slot;
                    
                    // if can't use, let the skill is null, do the queue's next 
                    
                    lastSkillList = null;
                }
            }
        }
    }
}
