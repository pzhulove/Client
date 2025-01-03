using UnityEngine;
using System.Collections;

public enum PlayerInfoColor
{
    TOWN_PLAYER = 0,
    TOWN_OTHER_PLAYER,
    PK_PLAYER,
    PK_OTHER_PLAYER,
    LEVEL_PLAYER,
    TOWN_NPC,
    BOSS,
    ELITE_MONSTER,
    SUMMON_MONSTER,
    PREFIX_MONSTER,
}

public class CPlayerInfo : MonoBehaviour {
   
    public Color cTownPlayer = Color.white;
    public Color cTownOtherPlayer = Color.white; //new Color(179.0f / 255.0f, 179.0f / 255.0f, 179.0f / 255.0f);
    public Color cPkPlayer = Color.red;
    public Color cPkOtherPlayer = Color.blue;
    public Color cLevelPlayer = Color.white;
    public Color cTownNPCs = new Color(0.9686f, 0.8392f, 0.3529f, 1.0f);
    public Color cBoss = Color.magenta;
    public Color cEliteMonster = Color.green;
    public Color cSummonMonster = Color.blue;

    public Color GetColor(PlayerInfoColor type)
    {
        switch(type)
        {
            case PlayerInfoColor.LEVEL_PLAYER:
                return cLevelPlayer;
            case PlayerInfoColor.TOWN_PLAYER:
                return cTownPlayer;
            case PlayerInfoColor.TOWN_OTHER_PLAYER:
                return cTownOtherPlayer;
            case PlayerInfoColor.PK_PLAYER:
                return cPkPlayer;
            case PlayerInfoColor.PK_OTHER_PLAYER:
                return cPkOtherPlayer;
            case PlayerInfoColor.TOWN_NPC:
                return cTownNPCs;
            case PlayerInfoColor.BOSS:
                return cBoss;
            case PlayerInfoColor.ELITE_MONSTER:
                return cEliteMonster;
            case PlayerInfoColor.SUMMON_MONSTER:
                return cSummonMonster;
        }

        return Color.white;
    }
}
    