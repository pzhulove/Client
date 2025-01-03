using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using Protocol;

namespace GameClient
{
    public static class CounterKeys
    {
        /// <summary>
        /// 上一次打的死亡之塔
        /// </summary>
        public static string COUNTER_TOWER = "tower";

        /// <summary>
        /// 今日死亡之塔最高层
        /// </summary>
        public static string COUNTER_TOWER_TOP_FLOOR = "tower_top_floor";

        /// <summary>
        /// 今日死亡之塔使用时间
        /// </summary>
        public static string COUNTER_TOWER_USED_TIME = "tower_used_time";

        /// <summary>
        /// 死亡之塔打到最高层时剩余血量
        /// </summary>
        public static string COUNTER_TOWER_REMAIN_HP = "tower_remain_hp";

        /// <summary>
        /// 死亡之塔打到最高层时剩余蓝量
        /// </summary>
        public static string COUNTER_TOWER_REMAIN_MP = "tower_remain_mp";

        /// <summary>
        /// 死亡之塔历史最高层
        /// </summary>
        public static string COUNTER_TOWER_TOP_FLOOR_TOTAL = "tower_top_floor_total";

        /// <summary>
        /// 死亡之塔历史最高层使用时间
        /// </summary>
        public static string COUNTER_TOWER_USED_TIME_TOTAL = "tower_used_time_total";

        /// <summary>
        /// 死亡之塔重置次数
        /// </summary>
        public static string COUNTER_TOWER_RESET_REMAIN_TIMES = "tower_reset_remain_times";

        /// <summary>
        /// 死亡之塔首次通关奖励领取情况掩码
        /// </summary>
        public static string COUNTER_TOWER_AWARD_MASK = "tower_award_mask";

        /// <summary>
        /// 匹配积分最高分
        /// </summary>
        public static string COUNTER_BEST_MATCH_SCORE = "best_match_score";

        /// <summary>
        /// 金币拍卖次数
        /// </summary>
        public static string COUNTER_SELL_GOLD_NUM = "gold_auction_num";

        /// <summary>
        /// 免费洗练
        /// </summary>
        public static string COUNTER_RETINUE_FREE_NUM = "retinue_free_num";

        /// <summary>
        /// 洗练次数
        /// </summary>
        public static string COUNTER_RETINUE_SKILL_NUM = "retinue_skill_num";

        /// <summary>
        /// 世界/附近频道频繁发言计数
        /// </summary>
        public static string CHAT_HI_FRQ_COUNT = "speak_times";

        /// <summary>
        /// 关卡每日次数(前缀)
        /// </summary>
        public static string DUNGEON_DAILY_COUNT_PREFIX = "dungeon_";

        /// <summary>
        /// 地下城每日购买次数(前缀）
        /// </summary>
        public static string DUNGEON_DAILY_BUY_PREFIX = "d_buy_";

        /// <summary>
        /// 地下城每日打过次数(前缀）
        /// </summary>
        public static string DUNGEON_DAILY_USED_PREFIX = "d_used_";

        /// <summary>
        /// 今日赠送次数
        /// </summary>
        public static string DAY_GIFT_COUNT = "gift_day";
        /// <summary>
        /// 本月补签次数
        /// </summary>
        public static string SIGN_IN_RP = "signInRp";

        /// <summary>
        /// 本月累计签到次数
        /// </summary>
        public static string SIGN_IN_TT = "signInTt";

        /// <summary>
        /// 今日从决斗场获得决斗币数量
        /// </summary>
        public static string COUNTER_DAY_PKCOIN_FROM_RACE = "day_pkcoin";

        /// <summary>
        /// 积分每日奖励重置KEY
        /// </summary>
        public static string COUNTER_REWARD_MATCH_DIALY = "reward_match_daily";

        /// 活动相关

        /// <summary>
        /// 每日在线时长(s)
        /// </summary>
        public static string DAY_ONLINE_TIME = "5000_DayOnline";
        /// <summary>
        /// 签到经历总天数
        /// </summary>
        public static string SIGN_IN_COUNT = "SIDayCount";
        /// <summary>
        /// 签到补签次数
        /// </summary>
        public static string SIGN_IN_RP_COUNT = "3000_SIRp";
        /// <summary>
        /// 签到免费补签次数
        /// </summary>
        public static string SIGN_IN_RP_FREE_COUNT = "3000_SIRpFree";
        /// <summary>
        /// 签到补签消耗
        /// </summary>
        public static string SIGN_IN_RP_COST = "3000_SIRpCost";

        /// <summary>
        /// 签到总次数
        /// </summary>
        public static string SIGN_IN_TT_COUNT = "3000_SITt";

        /// <summary>
        /// 每日Vip深渊免费次数
        /// </summary>
        public static string COUNTER_VIP_FREE_HELL_TIMES = "vip_free_hell_times";

        /// <summary>
        /// 每日Vip黑市商店刷新次数
        /// </summary>
        public static string COUNTER_VIP_SHOP_FRESH_TIMES = "vip_shop_refresh_times";

        /// <summary>
        /// 每日Vip免费复活次数
        /// </summary>
        public static string COUNTER_VIP_REVIVE_TIMES = "vip_revive_times";

        /// <summary>
        /// VIP购买礼包的计数BIT
        /// </summary>
        public static string COUNTER_VIP_GIFT_BUY_BIT = "vip_gift_buy_bit";

        /// <summary>
        /// VIP免费打开黄金宝箱次数
        /// </summary>
        public static string COUNTER_VIP_FREE_GOLD_CHEST_TIMES = "vip_gold_chest_times";

        /// <summary>
        /// VIP购买死亡之塔额外次数
        /// </summary>
        public static string COUNTER_VIP_TOWER_PAY_TIMES = "vip_tower_pay_times";

        /// <summary>
        /// 最大死亡塔层数
        /// </summary>
        public static string COUNTER_MAX_DEATH_TOWER_LEVEL = "tower_top_floor_total";

        /// <summary>
        /// 最大PIPEI积分
        /// </summary>
        public static string COUNTER_MAX_FIGHT_SCORE = "best_match_score";

        /// <summary>
        /// 金币拍卖行金币最大拍卖次数
        /// </summary>
        public static string COUNTER_MAX_AUCTION_GOLD_SELLNUM = "gold_auction_num";

        /// <summary>
        /// 决斗币每日获得上限
        /// </summary>
        public static string COUNTER_MAX_DAILY_TOTAL_PKCOIN = "day_pkcoin";

        /// <summary>
        /// VIP每日免费复活已经使用次数
        /// </summary>
        public static string COUNTER_VIP_FREEREBORN_USENUM = "vip_revive_times";

        /// <summary>
        /// VIP每日深渊免费次数
        /// </summary>
        public static string COUNTER_VIP_FREEHELL_LEFTNUM = "vip_free_hell_times";

        /// <summary>
        /// VIP每日黑市商店剩余刷新次数
        /// </summary>
        public static string COUNTER_VIP_FREESHOPREFRESH_LEFTNUM = "vip_shop_refresh_times";


        /// <summary>
        /// 循环任务 当前环数
        /// </summary>
        public static string COUNTER_CUR_LOOP_TIMES = "cycle_task_count";

        /// <summary>
        /// 循环任务刷新次数
        /// </summary>
        public static string COUNTER_CYCLE_REFRESH_TIMES = "cycle_refresh_count";

        /// <summary>
        /// 道具限制使用次数前缀
        /// </summary>
        public static string COUNTER_ITEM_DAYUSE_PRE = "item_dayuse_";

        /// <summary>
        /// 团本通行证每周使用次数
        /// </summary>
        public static string COUNTER_ITEM_WEEKUSE_PRE = "team_copy_item_use_";

        /// <summary>
        /// 每日活跃度
        /// </summary>

        public static string COUNTER_ACTIVITY_VALUE = "daily_act_score";

        /// <summary>
        /// 免费聊天次数
        /// </summary>

        public static string COUNTER_WORLD_FREE_CHAT_TIMES = "daily_free_wchat";

        /// <summary>
        /// 现在是否可以配置pvp技能
        /// </summary>
        public static string COUNTER_SKILL_CAN_USE_PVP = "pvp_skill_config";

        /// <summary>
        /// 深渊抽奖的抽奖次数
        /// </summary>
        public static string COUNTER_LOTTERY_TIME = "1500_draw_prize_num";

        /// <summary>
        /// 万圣节活动的抽奖次数
        /// </summary>
        public static string COUNTER_LOTTERY_ONLINE_TIME = "6300_draw_prize_num";

        /// <summary>
        /// 预约活动的硬币数量
        /// </summary>
        public static string COUNTER_ACTIVITY_COIN_NUM = "app_coin";

        /// <summary>
        /// 代币兑换活动的消耗深渊票数量
        /// </summary>
        public static string COUNTER_ACTIVITY_FATIGUE_TICKET_NUM = "_cost_fatigue";

        /// <summary>
        /// 兑换活动代币数量
        /// </summary>
        public static string COUNTER_ACTIVITY_FATIGUE_COIN_NUM = "_fatigue_tokens";

        /// <summary>
        /// 装备回收，当前奖励的积分
        /// </summary>
        public static string EQUIP_RECOVERY_REWARD_SCORE = "equip_reco_score";

        /// <summary>
        /// 每周已经回收的次数
        /// </summary>
        public static string EQUIP_RECOVERY_WEEK_COUNT = "equip_reco_times";

        /// <summary>
        /// 玩家是否为自动填充时装合成的状态
        /// </summary>
        public static string FASHION_MERGE_AUTO_EQUIP_STATE = "record_fashion_merge";

        /// <summary>
        /// 七夕鹊桥活动
        /// </summary>
        public static string OPACT_MAGPIE_BRIDGE_DAILY_PROGRESS = "_mgp_bdg_daily_prog";

        /// <summary>
        /// 荣耀积分
        /// </summary>
        public static string CHIJI_SCORE = "chi_ji_coin";

        /// <summary>
        /// 累计登录活动
        /// </summary>
        public static string ACT_DUNGEON = "act_dungeon";

        // 终极试炼代币1
        public static string ZJSL_WZHJJJ_COIN = "zjsl_wzhjjj_coin";

        // 终极试炼代币2
        public static string ZJSL_WZHGGG_COIN = "zjsl_wzhggg_coin";

        /// <summary>
        /// 当天深渊次数的挑战次数
        /// </summary>
        public static string DUNGEON_HELL_TIMES = "dungeon_hell_times";

        /// <summary>
        /// 当天远古次数的挑战次数
        /// </summary>
        public static string DUNGEON_YUANGU_TIMES = "dungeon_yuangu_times";

        /// <summary>
        /// 累计充值的金额
        /// </summary>
        public static string TOTAL_RECHARGENUM = "active_score_1583";

        /// <summary>
        /// 春季挑战积分
        /// </summary>
        public static string TOTAL_SPRING_SCORE = "_spring_total_score";

        /// <summary>
        /// 挑战俱乐部总积分
        /// </summary>
        public static string TOTAL_CHALLENGE_SCORE = "_challenge_total_score";

        /// <summary>
        /// 挑战俱乐部每日积分
        /// </summary>
        public static string TOTAL_CHALLENGE_DAY_SCORE = "_challenge_day_score";

        /// <summary>
        /// 每周签到累计次数
        /// </summary>
        public static string TOTAL_WEEKLY_NUM = "_week_sign_spring_num";

        /// <summary>
        /// 招募红点
        /// </summary>
        public static string COUNTER_HIRE_RED_POINT = "hire_red_point";

        /// <summary>
        /// 七日活跃度
        /// </summary>
        public static string COUNTER_SEVENDAYS_ACTIVITY_POINT = "seven_day_activity_point";
    }


    class CountDataManager : DataManager<CountDataManager>
    {
       

        List<CounterInfo> m_arrCountInfos = new List<CounterInfo>();

        public override EEnterGameOrder GetOrder()
        {
            return EEnterGameOrder.CountDataManager;
        }

        public override void Initialize()
        {
            
        }

        public override void Clear()
        {
            m_arrCountInfos.Clear();
        }

        public void SetCount(string a_strKey, uint a_value)
        {
            Logger.LogProcessFormat("update counter:key = {0} value = {1}", a_strKey, a_value);

            var item = m_arrCountInfos.Find(x => x.name == a_strKey);

            if (item == null)
            {
                item = new CounterInfo() { name = a_strKey, value = a_value };
                m_arrCountInfos.Add(item);
            }
            else
            {
                item.value = a_value; 
            }

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnCountValueChange, a_strKey);
        }


        //只用来缓存数据
        public void SetCountWithoutUiEvent(string strKey, uint strValue)
        {
            var counterInfo = m_arrCountInfos.Find(x => x.name == strKey);

            //没有找到，创建
            if (counterInfo == null)
            {
                counterInfo = new CounterInfo()
                {
                    name = strKey,
                    value = strValue,
                };
                m_arrCountInfos.Add(counterInfo);
            }
            else
            {
                //找到，更新Value
                counterInfo.value = strValue;
            }
        }

        public int GetCount(string name, int id)
        {
            string key = string.Format("{0}{1}", name, id);
            return GetCount(key);
        }

        public int GetCount(string name)
        {
            int Count = 0;

            for (int i = 0; i < m_arrCountInfos.Count; i++)
            {
                if (name != m_arrCountInfos[i].name)
                {
                    continue;
                }

                return (int)m_arrCountInfos[i].value;
            }

            return Count;
        }

        public List<CounterInfo> GetCountInfos()
        {
            return m_arrCountInfos;
        }

        public CounterInfo GetCountInfo(string a_strKey)
        {
            for (int i = 0; i < m_arrCountInfos.Count; i++)
            {
                if (a_strKey != m_arrCountInfos[i].name)
                {
                    continue;
                }

                return m_arrCountInfos[i];
            }
            return null;
        }
    }
}
