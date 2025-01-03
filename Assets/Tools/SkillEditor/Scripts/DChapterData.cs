using UnityEngine;
using System;


/// <summary>
/// none在最后，不要改顺序
/// </summary>
[Serializable]
public enum ChapterDungeonUnitType
{
    Normala,
    Important,
    Elite,
    None,       
}

[System.Serializable]
public class ChaptertDungeonUnit
{
    public int dungeonID;
    public Vector3 position;
    public Vector3 thumbOffset;
    public Vector3 angleTargetPosition;
    public Vector3 angleTargetRightPosition;
    public Vector3 angleSourcePosition;
    public ChapterDungeonUnitType chapterDungeonUnitType;
    public Vector3 iconPos;
}

public class DChapterData : ScriptableObject 
{
    public Vector2 offsetMin = Vector2.zero;
    public Vector2 offsetMax = Vector2.zero;


    public string backgroundPath;
    public string middlegroudnPath;
    public Vector3 middlePos = Vector3.zero;
    public string namePath;
    public string name;

    public int    nameNumberIdx = 1;

    public ChaptertDungeonUnit[] chapterList = new ChaptertDungeonUnit[0];
}
