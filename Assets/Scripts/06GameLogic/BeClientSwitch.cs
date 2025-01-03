using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProtoTable;

//客户端开关表相关数据
namespace GameClient
{
    public enum ClientSwitchType
    {
        SkillCanSummon = 33,        //判断能否使用召唤技能（优化）
        HpBarSetLayer = 34,         //血条显示与隐藏（优化）
        SkillPre = 35,              //技能资源预加载（优化）
        SummonNewFindPos = 38,      //召唤怪物使用新的寻找非阻挡点算法   
        XuanWuEndToFallProtect = 39,//玄武抓取完以后 使被抓取单位进入浮空保护 
        TrainRemoveMechanism = 40,  //练习场重置练习时 移除实体身上的机制
        PaladinAttackCharge = 41,   //驱魔师普攻第三阶段能否蓄力 
        PkAbnormalBuffProtect = 42, //决斗场异常Buff保护开关
        AutoFightTriggerDoor = 52, //自动战斗过门传送以后不来回晃动
        HardProtect = 61,   //僵直保护是否开启
        OnRemove = 64,  //技能实现的怪物过门移除的时候先调用OnDead函数
        Strain = 65,    //束缚buff控制X轴移动
        NewbieGuideJumpBack = 101,  //第一场战斗快速点击两次后跳会导致游戏卡死的bug
        HideObjUseSetlayer = 102,     //隐藏GameObject使用设置层级的方式
        GeAttachSetLayerBug = 106,    //挂件隐藏节点不会设置层级的bug
        GeDangNewAlgorithms = 109, //格挡新算法
        OpenChasingMode = 112,//开启追帧
    }

    public class BeClientSwitch
    {
        protected static Dictionary<int, bool> m_RocordDataDic = new Dictionary<int, bool>();   //缓存下数据 避免频繁读表

        /// <summary>
        /// 判断该客户端功能是否启用 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool FunctionIsOpen(ClientSwitchType type)
        {
            int key = (int)type;
            if (m_RocordDataDic.ContainsKey(key))
                return m_RocordDataDic[key];
            var data = TableManager.instance.GetTableItem<SwitchClientFunctionTable>((int)type);
            if (data == null)
                return true;
            m_RocordDataDic.Add(key, data.Open);
            return data.Open;
        }
    }
}