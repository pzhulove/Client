using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameClient
{
    public class ScriptLabelDeclare
    {
        public static string[] declares = new string[]
        {
            "FunctionFrame",
            "SmithShopFrame",
            "ChatFrame",
            //add your code to here
        };
    }

    /***************************************************************
     * every enum var must have a fixed value , such as var xxx = 0*
     * *************************************************************/

    public enum ComScriptLabel
    {
        //sample 1
        Label_FunctionFrame_Button_Merge = 0,
        Label_FunctionFrame_Button_Link = 1,
        Label_FunctionFrame_Button_Fuck = 2,
        Label_FunctionFrame_State_Win = 3,
        Label_FunctionFrame_State_Lose = 4,
        Label_FunctionFrame_State_Draw = 5,
        //sample 2
        Label_SmithShopFrame_Button_Merge = 0,
        //add your code to here
        Label_ChatFrame_State_None = 0,
        Label_ChatFrame_State_NoTeam = 1,
        Label_ChatFrame_State_NoGuild = 2,
        Label_ChatFrame_Button_JoinTeam = 3,
        Label_ChatFrame_Button_JoinGuild = 4,
        Label_ChatFrame_chatListContent = 5,       
        Label_ChatFrame_State_NoTeamCopy = 6,
        Label_ChatFrame_Button_JoinTeamCopy = 7,
    }
}