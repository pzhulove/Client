using UnityEngine;

[System.Serializable]
public class DDungeonMapUnitData
{
    public int posx;
    public int posy;

    public int dungeonid;

    public DDungeonData dungeon;
}

public class DDungeonMapData : ScriptableObject
{
    public int weith;
    public int heigth;

    public int dungeonid;

    public DDungeonMapUnitData[] dungeonList = new DDungeonMapUnitData[0];
}
