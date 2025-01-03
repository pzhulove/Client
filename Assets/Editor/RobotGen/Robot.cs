using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameClient;
using ProtoTable;

public enum RobotHardType
{
    Easy = 1,
    Normal,
    Hard
}

public class Robot {
    static int id_seed = 0;
    public int id = ++id_seed;
    public string name = "";
    public int level;
    public ActorOccupation occu;
    public RobotHardType hard;
    public List<int> equips = new List<int>();
    public string skills = "-";

    public override string ToString()
    {
        string equipStr = "-";
        foreach(var equip in equips)
        {
            if(equipStr == "-")
            {
                equipStr = equip.ToString();
            }
            else
            {
                equipStr = equipStr + "|" + equip.ToString();
            }
        }

        return string.Format("{0}\t{1}\t{6}\t{2}\t{3}\t{4}\t{5}", id, name, level, (int)occu, equipStr, skills, (int)hard);
    }
}
