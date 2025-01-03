using System;
using System.Collections;
using System.Reflection;
using System.ComponentModel;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EItemType = ProtoTable.ItemTable.eType;
using Network;
using Protocol;
using UnityEngine.EventSystems;
using ProtoTable;


namespace GameClient
{
    public class HelpFrame : ClientFrame
    {
//         public enum HelpType
//         {
//             HT_NONE = 0,
//             HT_STRENTH,         //装备强化
//             HT_ADDMAGIC,        //装备附魔
//             HT_SEAL,            //装备封装
//             HT_MAGICCOPMOSE,    //附魔卡合成
//             HT_QUALITYCHANGE,   //品级调整
//             HT_FASHIONCOMPOSE,  //时装合成
//             HT_DIMENSION,       //次元石
//             HT_FOLLOW,          //随从
//             HT_SKILL,           //技能
//             HT_TITLE,           //称号
//             HT_GUILD,           //工会
//             HT_RELATION,          //好友
//             HT_TEAM,            //组队    
//             HT_MISSION,         //任务
//             HT_MALL,            //商城
//             HT_SHOP,            //商店
//             HT_ITEMSELLMALL,    //物品拍卖行
//             HT_GOLDSELLMALL,    //金币拍卖行
//             HT_CELEBRATE,       //开服狂欢
//             HT_MAGICJAR,        //魔罐
//             HT_GOLDJAR,         //金罐
// 
//             HT_WORRIERTOWER,    //勇者塔
//             HT_MINOTAUR,        //牛头怪乐园
//             HT_MAGICCRYSTAL,    //魔晶矿脉
//             HT_GOBLINTRATRURE,  //哥布林宝藏
//             HT_ABYSS,           //深渊
//             HT_ANCIENT,         //远古
//             HT_STORAGE,         //仓库
//             HT_EQUIP_FORGE,     //装备打造
//             HT_FATIGUE,         //体力
//             HT_GUILD_BATTLE,    //工会战
// 			HT_RED_PACK,         //红包
//             HT_PKRANK,          //段位
//             HT_SEASON_LEVEL_ATTR,   // 赛季段位属性
// 			HT_WUDAOHUI,        //武道会罐子
//             HT_GUILD_BATTLE_DETAIL,    // 公会领地争夺战
//             HT_PACKAGE,         //背包
//             HT_MULTI_DECOMPOSE, //一键分解
// 			HT_MAGICJARSHOP, //魔罐商店
// 			HT_GOLDJARSHOP, //金罐商店
// 			HT_FASHIONCHOOSE, //时装属性选择
//             HT_LIMITTIME_MALLGIFT,//商城时限礼包
// 			HT_PETFOLLOW,//宠物
// 			HT_TEACHER,//师徒
// 			HT_GIVE,//赠送道具
//             HT_PEARLINLAY,      //宝珠镶嵌
// 			HT_SHANGJIN,//赏金武道大会
// 			HT_3V3PK,//3V3决斗
//             HT_CreatGuild,//创建公会
//             HT_OPPOPrivilege,//OPPO特权
//             HT_NewYearRedPack,//新年红包活动
//             HT_VIVOPrivilege, //VIVO特权
//             HT_TRANSFER, //  传家
//             HT_TREASURE_LOTTERY,//全民夺宝
//             HT_EQUIP_RECOVERY,//装备回收
// 			HT_GUILD_BATTLE2,//跨服领地
//             HT_FINANCIALPLAN, //理财计划
//             HT_SUMMERACTIVITY,//夏日活动
//             HT_QIXI_QUEQIAO,//七夕鹊桥
//             HT_LEVEL_RESISTMAGIC,//抗魔值
//             HT_3v3_JIFENSAI, //3v3积分争霸赛
//             HT_RANDOM_TREASURE,//月卡随机宝箱
//             HT_HorseGambling,//赌马
//             HT_DROP_PROGRESS,//关卡掉落进度
//             HT_FORTIFIED_VOUCHER,//魔导器
//             HT_CHRISTMASSNOWMAN,//圣诞雪人
//             HT_AT_BASEINFORMATION,  //冒险队基础信息, 67
//             HT_AT_BLESS_SHOP,       //冒险队的赐福商店 68
//             HT_AT_BOUNTY_SHOP,    //冒险队的赏金商店  69
//             HT_BEADUPGRADE, //宝珠升级
//             HT_GOBLIN_SHOP,//地精商店 71
//             HT_WEAPONLEASE,//武器租赁
//             HT_GUILD_DUNGEON, // 公会副本
//             HT_GUILD_DUNGEON_DAMAGE_RANK, // 公会副本伤害排行
//             HT_AUCTION_NEW,//新拍卖行
//             HT_BEADREPLACE,//宝珠置换
//             HT_ARTIFACTJAR_DISCOUNT,//神器罐子抽奖界面//77
//             HT_ARTIFACTJAR_REWARD,//神器罐子的奖励界面//78
//             HT_LANTERNFESTIVAL,   //元宵挂灯笼活动
//             HT_EQUIPUPGRADE,      //装备升级
//             HT_BLACKMARKETMERCHAN,//黑市商人
//             HT_WEEKOFTENHELL_RESISTMAGIC,    //周常深渊抗魔值
//             HT_APRILFOOLSDAYACTIVITY,        //愚人节活动
//             HT_GUILD_AUCTION,                // 公会拍卖
//             HT_WORLD_AUCTION,                // 世界拍卖
//             HT_WEAPONATTACKATTR,             //武器攻击属性  
//             HT_GUILD_EMBLEM,                 // 公会徽记
//             HT_GUILD_DUNGEON_BOSS_DIFF_SET,  // 公会副本boss难度设置
//             HT_GUILD_BUILDING_MANAGER,       // 公会建筑管理
//             HT_ENCHANTMENTCARUPGRADE,       // 附魔卡升级
//             HT_GUILD_MANOR,                 // 公会领地
//             HT_GUILD_CROSS_MANOR,           // 公会跨服领地
//             HT_AT_DRAGONBOATFESTIVALACTIVITY,//端午节活动
//             HT_GUILD_RED_PACKET,             //公会红包
//             HT_MATERIALSSYNTHESIS,           //材料合成
//             HT_MONTHLY_SIGN_IN,              //月签到
//             HT_SKILLDAMAGE,             //技能伤害面板
//             HT_ULTIMATE_CHALLENGE,             //终极试炼
//             HT_BATTLE_POTION_SET,       //药品配置
//             HT_INSCRIPTION,             //铭文
//             HT_FASHIONDECOMPOSE,        //时装分解
//             HT_FLYINGGIFTPACKACTIVITY,  //飞升献礼活动
//             HT_ELITE_DUNGEON,  //精英地下城
//             HT_ADVENTURER_PASS_CARD,  //冒险者通行证
//             HT_2V2_CROSS_MELEE,  //2v2跨服乱斗
//             HT_TREASURECONVERSION,//宝珠转换
//             HT_SAMEEQUIPMENTCONVER,//同套装转化
//             HT_CROSSEQUIPMENTCONVER,//跨套装转化
//             HT_WARRIORRECUIT,//勇士招募
//             HT_LIMITTIMEGROUPBUYACTIVITY, // 限时团购
//             HT_MallLIMITTIMEGROUPBUYACTIVITY,//商城限时团购
//         }
        
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Common/ComHelp.prefab";
        }

        protected override void _OnOpenFrame()
        {
            _Initialize();
        }

        protected override void _OnCloseFrame()
        {
            _Clear();
        }

        protected void _Initialize()
        {
            HelpFrameContentTable.eHelpType showType = HelpFrameContentTable.eHelpType.HT_NONE;
            if (userData != null)
            {
                showType = (HelpFrameContentTable.eHelpType)userData;
            }

            ComHelp help = frame.GetComponent<ComHelp>();

            if (showType == HelpFrameContentTable.eHelpType.HT_AUCTION_NEW 
                && ServerSceneFuncSwitchManager.GetInstance().IsTypeFuncLock(Protocol.ServiceType.SERVICE_AUCTION_TREAS))
            {
                //根据服务器开关 关：不显示珍品描述
                if (help != null)
                {
                    help.SetExtraTypeContent((uint)showType);
                }
            }
            else
            {
                if (help != null)
                {
                    help.SetType((uint)showType);
                }
            }
        }

        protected void _Clear()
        {
            
        }

        #region ctrl callback
        
        [UIEventHandle("Back/Button")]
        void _OnCloeFrame()
        {
            frameMgr.CloseFrame(this);
        }
        
        #endregion
    }
}
