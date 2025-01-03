using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameClient
{
    public enum PetActionNameType
    {
        None,
        Idle,
        HungerIdle,     //Eidle
        OpenEggIdle,        //BIdle
        HungerWalk,     //EWalk
        SpecialIdle,    //Sidle
        Walk,   //Walk
        LastAction, //使用上一次的动作
    }

    public class BePetActionSwitchHelper
    {
        /// <summary>
        /// 通过枚举名称获取技能配置文件名称
        /// </summary>
        public static string GetActionNameByType(PetActionNameType type)
        {
            string str = null;
            switch (type)
            {
                case PetActionNameType.Idle:
                    str = "Idle";
                    break;
                case PetActionNameType.HungerIdle:
                    str = "Eidle";
                    break;
                case PetActionNameType.SpecialIdle:
                    str = "Sidle";
                    break;
                case PetActionNameType.OpenEggIdle:
                    str = "Bidle";
                    break;
                case PetActionNameType.Walk:
                    str = "Walk";
                    break;
                case PetActionNameType.HungerWalk:
                    str = "Ewalk";
                    break;
            }
            return str;
        }
    }

    

    /// <summary>
    /// 城镇中宠物使用技能配置文件播放动作和特效
    /// </summary>
    public class BeTownPetActionPlay : BeTownActionPlay
    {

    }
}