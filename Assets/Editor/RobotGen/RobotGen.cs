using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GameClient;
using ProtoTable;

public class RobotGen : Singleton<RobotGen> {

    class ItemIndex
    {
        // 品质
        int color;
        // 部位
        int part;
    }
    
    List<ItemTable>[] items = new List<ItemTable>[100];
    List<Robot> robots = new List<Robot>();
    List<string> firstNameList = new List<string>();
    List<string> secondNameList = new List<string>();

    
    Dictionary<int, int> occuMaterialRecommend = new Dictionary<int, int>();
    Dictionary<int, List<ItemTable>> itemMap = new Dictionary<int, List<ItemTable>>();

    // 初始化护甲精通配置
    private void InitEquipMaster()
    {
        var equipMasters = TableManager.instance.GetTable<EquipMasterTable>();
        foreach(var itr in equipMasters)
        {
            var data = (EquipMasterTable)itr.Value;
            if(data.IsMaster > 0)
            {
                int key = data.JobID * 1000 + data.Part;
                if(!occuMaterialRecommend.ContainsKey(key))
                {
                    occuMaterialRecommend[key] = data.MaterialType;
                }
            }
        }
    }

    int MakeEquipKey(ItemTable.eColor color, ItemTable.eSubType part)
    {
        return (int)part * 100 + (int)color;
    }

    private void InitEquips()
    {
        var itemTable = TableManager.instance.GetTable<ItemTable>();
        foreach (var elem in itemTable)
        {
            ItemTable item = (ItemTable)elem.Value;

            if (item.Type != ItemTable.eType.EQUIP && item.Type != ItemTable.eType.FASHION)
            {
                continue;
            }

			// 400000001~499999999是吃鸡道具，需要排除
            if(item.ID / 100000000 == 4)
            {
                continue;
            }
			
            int key = MakeEquipKey(item.Color, item.SubType);
            if (!itemMap.ContainsKey(key))
            {
                itemMap.Add(key, new List<ItemTable>());
                
            }

            itemMap[key].Add(item);
        }
    }

    private int GetRecommendMaterial(int occ, int part)
    {
        int key = occ * 1000 + part;
        if(occuMaterialRecommend.ContainsKey(key))
        {
            return occuMaterialRecommend[key];
        }

        return 0;
    }

	// Use this for initialization
	public override void Init () {

        // 初始化职业护甲精通
        InitEquipMaster();

        // 初始化道具列表
        InitEquips();

        for (int i = 0; i < items.Length; i++)
        {
            items[i] = new List<ItemTable>();
        }

        var itemTable = TableManager.instance.GetTable<ItemTable>();
        
        foreach(var elem in itemTable)
        {
            ItemTable item = (ItemTable)elem.Value;
            
            if(item.Type != ItemTable.eType.EQUIP && item.Type != ItemTable.eType.FASHION)
            {
                continue;
            }

            if(item.Color <= ItemTable.eColor.WHITE || item.Color >= ItemTable.eColor.PINK)
            {
                continue;
            }

            items[(int)item.SubType].Add(item);
        }

        var nameTable = TableManager.GetInstance().GetTable<ProtoTable.NameTable>();
        var enumerator = nameTable.GetEnumerator();
        while (enumerator.MoveNext())
        {
            var nameItem = enumerator.Current.Value as ProtoTable.NameTable;
            if (nameItem.NameType == 0)
            {
                firstNameList.Add((enumerator.Current.Value as ProtoTable.NameTable).NameText);
            }
            else
            {
                secondNameList.Add((enumerator.Current.Value as ProtoTable.NameTable).NameText);
            }
        }
    }

    public void Start()
    {
        /*var jobTable = TableManager.instance.GetTable<JobTable>();
        foreach (var elem in jobTable)
        {
            JobTable job = elem.Value as JobTable;
            if (job == null || job.Open == 0)
            {
                continue;
            }

            for (int i = 2; i <= 12; i++)
            {
                int level = i * 5;
                if(level < 15 && job.JobType == 1 || 
                    level >= 15 && job.JobType == 0)
                {
                    continue;
                }

                for (int j = 0; j < 10; j++)
                {
                    var robot = GenRobot((ActorOccupation)job.ID, level);
                    if(robot != null)
                    {
                        robots.Add(robot);
                    }
                }
           
            }
        }*/

        var jobTable = TableManager.instance.GetTable<JobTable>();
        foreach (var elem in jobTable)
        {
            JobTable job = elem.Value as JobTable;
            if (job == null || job.Open == 0)
            {
                continue;
            }

            for (int i = 2; i <= 12; i++)
            {
                int level = i * 5;
                if (level < 15 && job.JobType == 1 ||
                    level >= 15 && job.JobType == 0)
                {
                    continue;
                }

                for (int j = 0; j < 100; j++)
                {
                    for(var hard = RobotHardType.Easy; hard <= RobotHardType.Hard; hard++)
                    {
                        var robot = GenRobotNew(hard, (ActorOccupation)job.ID, level);
                        if (robot != null)
                        {
                            robots.Add(robot);
                        }
                    }
                    
                }

            }
        }

        Save();

        //GenRobotNew(RobotHardType.Hard, ActorOccupation.SwordSoulMan, 30);
    }


    [MenuItem("[TM工具集]/转表工具/生成PK机器人")]
    public static void Gen()
    {
        RobotGen.instance.Init();
        RobotGen.instance.Start();
    }

    private void Save()
    {
        try
        {
            /*string[] lines = new string[robots.Count];
            int index = 0;
            foreach (var robot in robots)
            {
                lines[index++] = robot.ToString();
            }

            string filePath = DataUnitPath.SERVER_TEXT_PATH + "/RobotTable.txt";
            File.WriteAllLines(filePath, lines, Encoding.UTF8);*/

            string filePath = DataUnitPath.SERVER_TEXT_PATH + "/RobotTable.txt";
            using (StreamWriter sw = new StreamWriter(filePath))
            {
                foreach (var robot in robots)
                {
                    string str = robot.ToString();
                    sw.WriteLine(str);
                }

                sw.Close();
            }
        }
        catch(IOException e)
        {
            Logger.LogError(e.ToString());
        }
        
    }

    private Robot GenRobot(ActorOccupation occu, int level)
    {
        Robot robot = new Robot();
        robot.name = GenName();
        robot.level = level;
        robot.occu = occu;
        robot.skills = GenSkills(occu, level);
        if(robot.skills == "")
        {
            return null;
        }

        // 生成装备
        for (int i = (int)ItemTable.eSubType.WEAPON; i <= (int)ItemTable.eSubType.BRACELET; i++)
        {
            var equip = GenEquip(GetOldOccu(occu), level, (ItemTable.eSubType)i);
            if(equip == null)
            {
                continue;
            }

            robot.equips.Add(equip.ID);
        }

        // 生成时装
        /*for (int i = (int)ItemTable.eSubType.FASHION_HAIR; i <= (int)ItemTable.eSubType.FASHION_EPAULET; i++)
        {
            var equip = GenEquip(GetOldOccu(occu), level, (ItemTable.eSubType)i);
            if (equip == null)
            {
                continue;
            }

            robot.equips.Add(equip.ID);
        }*/

        return robot;
    }

    private ItemTable GenEquip(ActorOccupation occu, int level, ItemTable.eSubType type)
    {
        bool hasEquip = true;
        int prob = Random.Range(0, 100);

        if (type >= ItemTable.eSubType.FASHION_HAIR && type <= ItemTable.eSubType.FASHION_EPAULET && prob < 40)
        {
            hasEquip = true;
        }

        if(!hasEquip)
        {
            return null;
        }
        
        List<ItemTable> equips = new List<ItemTable>();
        foreach(var item in items[(int)type])
        {
            if(item.Occu.Count != 0)
            {
                if(item.Occu.FindIndex((int Occu)=> { return Occu == 0 || Occu == (int)occu; }) == -1)
                {
                    continue;
                }
            }

            if(item.NeedLevel > level || item.NeedLevel < level - 10)
            {
                continue;
            }

            if(type < ItemTable.eSubType.FASHION_HAIR || type > ItemTable.eSubType.FASHION_EPAULET)
            {
                if(item.Color != ItemTable.eColor.BLUE)
                {
                    continue;
                }
            }

            equips.Add(item);
        }

        if(equips.Count == 0)
        {
            return null;
        }

        int index = Random.Range(0, equips.Count - 1);
        return equips[index];
    }

    List<ItemTable> GetEquips(ItemTable.eColor color, ItemTable.eSubType part)
    {
        int key = MakeEquipKey(color, part);
        if(itemMap.ContainsKey(key))
        {
            return itemMap[key];
        }

        return null;
    }

    private ItemTable GenEquipNew(ActorOccupation occu, int level, ItemTable.eColor color, ItemTable.eSubType part)
    {
        var mainOccu = GetOldOccu(occu);
        var recommendMaterial = GetRecommendMaterial((int)occu, (int)part);
        List<ItemTable> items = GetEquips(color, part);
        if(items == null)
        {
            return null;
        }

        bool isFashion = false;
        if(part >= ItemTable.eSubType.FASHION_HAIR && part <= ItemTable.eSubType.FASHION_EPAULET)
        {
            isFashion = true;
        }


        RobotConfigTable robotConfig = null;
        if (isFashion)
        {
            robotConfig = GetRobotConfig(occu, level);
            if(robotConfig == null)
            {
                return null;
            }
        }

        List<ItemTable> priorityEquips = new List<ItemTable>();
        List<ItemTable> equips = new List<ItemTable>();
        foreach (var item in items)
        {
            if (item.Occu.Count != 0)
            {
                if (item.Occu.FindIndex((int Occu) => { return Occu == 0 || Occu == (int)mainOccu || Occu == (int)occu; }) == -1)
                {
                    continue;
                }
            }

            if (!isFashion && (item.NeedLevel > level || item.NeedLevel < level - 10))
            {
                continue;
            }

            // 去掉透明天空套
            if(item.IsTransparentFashion > 0)
            {
                continue;
            }

            if (part >= ItemTable.eSubType.HEAD && part <= ItemTable.eSubType.BOOT)
            {
                if((int)item.ThirdType != (int)recommendMaterial)
                {
                    continue;
                }
            }

            if (item.Occu.Count != 0)
            {
                if (item.Occu.FindIndex((int Occu) => { return Occu == (int)occu; }) != -1)
                {
                    priorityEquips.Add(item);
                    continue;
                }
            }

            // 不在机器人规定的时装配置里
            if(isFashion && robotConfig != null && !robotConfig.Fashions.Contains(item.ID))
            {
                continue;
            }

            equips.Add(item);
        }

        if (equips.Count == 0 && priorityEquips.Count == 0)
        {
            return null;
        }

        if(priorityEquips.Count > 0)
        {
            int index1 = Random.Range(0, priorityEquips.Count - 1);
            return priorityEquips[index1];
        }

        int index = Random.Range(0, equips.Count - 1);
        return equips[index];
    }

    private string GenName()
    {
        int iRandomFirst = UnityEngine.Random.Range(0, firstNameList.Count - 1);
        int iRandomSecond = UnityEngine.Random.Range(0, secondNameList.Count - 1);
        string name = firstNameList[iRandomFirst] + secondNameList[iRandomSecond];
        return name;
    }

    RobotConfigTable GetRobotConfig(ActorOccupation occu, int level)
    {
        var robotConfig = TableManager.instance.GetTable<RobotConfigTable>();
        foreach (var elem in robotConfig)
        {
            var config = elem.Value as RobotConfigTable;
            if (config == null || config.Occu != (int)occu)
            {
                continue;
            }

            if (config.Level != level)
            {
                continue;
            }

            return config;
        }

        return null;
    }

    private string GenSkills(ActorOccupation occu, int level)
    {
        List<int> tableIdList = new List<int>();
        var robotConfig = TableManager.instance.GetTable<RobotConfigTable>();
        foreach (var elem in robotConfig)
        {
            var config = elem.Value as RobotConfigTable;
            if(config == null || config.Occu != (int)occu)
            {
                continue;
            }

            if(config.Level > level || config.Level < level - 10)
            {
                continue;
            }
            tableIdList.Add(config.ID);
        }

        if(tableIdList.Count == 0)
        {
            return "";
        }

        string ret = "";
        int index = Random.Range(0, tableIdList.Count - 1);
        Dictionary<int, int> skillMap = new Dictionary<int, int>();
        var skills = TableManager.instance.GetTableItem<RobotConfigTable>(tableIdList[index]).Skills;
        if (skills == null)
            return "";
        foreach (var str in skills)
        {
            int skillId = str;
            var skillData = TableManager.instance.GetTableItem<SkillTable>(skillId);
            if(skillData == null || skillData.LevelLimit > level)
            {
                continue;
            }

            if(skillMap.ContainsKey(skillId))
            {
                continue;
            }

            int skillLevel = 1;
            while((skillLevel - 1) * skillData.LevelLimitAmend + skillData.LevelLimit <= level && skillLevel <= skillData.TopLevelLimit)
            {
                skillLevel++;
            }
            skillLevel--;
            if(skillLevel == 0)
            {
                continue;
            }

            if(ret != "")
            {
                ret += "|";
            }
            ret += skillId.ToString() + ":" + skillLevel;

            skillMap.Add(skillId, skillLevel);
        }

        return ret;
    }

    ActorOccupation GetOldOccu(ActorOccupation occu)
    {
        var jobTable = TableManager.instance.GetTableItem<JobTable>((int)occu);
        if(jobTable == null || jobTable.prejob == 0)
        {
            return occu;
        }

        return (ActorOccupation)jobTable.prejob;
    }

    ItemTable.eColor GetRobotItemColor(RobotHardType hard, ItemTable.eSubType part)
    {
        if(part == ItemTable.eSubType.WEAPON)
        {
            // 武器

            if (hard == RobotHardType.Easy)
            {
                return ItemTable.eColor.BLUE;
            }
            else if(hard == RobotHardType.Normal)
            {
                return ItemTable.eColor.PINK;
            }
            else
            {
                return ItemTable.eColor.YELLOW;
            }
        }
        else if(part >= ItemTable.eSubType.HEAD && part <= ItemTable.eSubType.BRACELET)
        {
            // 防具
            if (hard == RobotHardType.Easy)
            {
                return ItemTable.eColor.BLUE;
            }
            else if (hard == RobotHardType.Normal)
            {
                return ItemTable.eColor.PINK;
            }
            else
            {
                return ItemTable.eColor.YELLOW;
            }
        }
        else if(part >= ItemTable.eSubType.FASHION_HEAD && part <= ItemTable.eSubType.FASHION_EPAULET)
        {
            // 时装
            if (hard == RobotHardType.Easy)
            {
                return ItemTable.eColor.PURPLE;
            }
            else if (hard == RobotHardType.Normal)
            {
                return ItemTable.eColor.PURPLE;
            }
            else
            {
                return ItemTable.eColor.PINK;
            }
        }

        return ItemTable.eColor.BLUE;
    }

    void shuffle(ref List<ItemTable.eSubType> data)
    {
        for(int i = 0; i < data.Count; i++)
        {
            int swapIndex = Random.Range(0, data.Count - 1);

            var tmp = data[i];
            data[i] = data[swapIndex];
            data[swapIndex] = tmp;
        }
    }

    int GetFashionNum(RobotHardType hard)
    {
        if (hard == RobotHardType.Easy)
        {
            return Random.Range(0, 3);
        }
        else if (hard == RobotHardType.Normal)
        {
            return 5;
        }
        else
        {
            return 6;
        }
    }

    private Robot GenRobotNew(RobotHardType hard, ActorOccupation occu, int level)
    {
        Robot robot = new Robot();
        robot.name = GenName();
        robot.hard = hard;
        robot.level = level;
        robot.occu = occu;
        robot.skills = GenSkills(occu, level);
        if (robot.skills == "")
        {
            return null;
        }

        // 生成装备
        for (int i = (int)ItemTable.eSubType.WEAPON; i <= (int)ItemTable.eSubType.BRACELET; i++)
        {
            var part = (ItemTable.eSubType)i;
            var color = GetRobotItemColor(hard, part);
            var equip = GenEquipNew(occu, level, color, part);
            if (equip == null)
            {
                // 如果找不到对应的装备，就去找蓝色的装备，继续生成一次
                equip = GenEquipNew(occu, level, ItemTable.eColor.BLUE, part);
                if(equip == null)
                {
                    continue;
                }
            }

            robot.equips.Add(equip.ID);
        }

        // 生成时装
        List <ItemTable.eSubType> fashionParts = new List<ItemTable.eSubType> {
            ItemTable.eSubType.FASHION_HEAD,
            ItemTable.eSubType.FASHION_SASH,
            ItemTable.eSubType.FASHION_CHEST,
            ItemTable.eSubType.FASHION_LEG,
            ItemTable.eSubType.FASHION_EPAULET};
        shuffle(ref fashionParts);

        int fashionNum = GetFashionNum(hard);
        if(hard == RobotHardType.Hard)
        {
            //fashionParts.Add(ItemTable.eSubType.FASHION_HAIR);
            fashionNum--;
        }

        for (int i = 0; i < fashionNum ; i++)
        {
            var part = fashionParts[i];
            var color = GetRobotItemColor(hard, part);
            var equip = GenEquipNew(occu, level, color, part);
            if (equip == null)
            {
                continue;
            }

            robot.equips.Add(equip.ID);
        }

        return robot;
    }
}
