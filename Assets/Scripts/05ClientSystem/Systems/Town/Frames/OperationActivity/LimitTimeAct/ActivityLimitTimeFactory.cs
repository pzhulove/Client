using System;
using GameClient;
using Protocol;
using ProtoTable;

public static class ActivityLimitTimeFactory
{
    public enum EActivityCounterType
    {
        QAT_SUMMER_DAILY_CHARGE=1001,
        OAT_SUMMER_WEEKLY_VACATION= 2001,

        OP_ACTIVITY_BEHAVIOR_DAILY_SUBMIT_TASK = 1002, // 单个活动任务每日领奖次数
        OP_ACTIVITY_BEHAVIOR_TOTAL_SUBMIT_TASK = 4002,  // 单个活动任务总的领奖次数
        OP_ITEM_ACTIVITY_BEHAVIOR_TOTAL_SUBMIT_TASK = 4003,//单个节日兑换活动任务总的领奖次数
        OP_ACTIVITY_BEHAVIOR_DAILY_DUNGEON_COUNT = 1003,// 每日地下城次数
        OP_ACTIVITY_BEHAVIOR_WEEKLY_SUBMIT_TASK = 2002,      //单个活动任务每周领奖次数
    }



    public enum EActivityType
    {
        NONE = 0,
        DAY_SINGLE_CHARGE = 1, // 每日单笔充值
        DAY_TATOL_CHARGE = 2, // 每日累计充值
        TATOL_CHARGE = 3, // 累计充值
        SINGLE_CHARGE = 4, // 单笔充值	
        COMBO_CHARGE = 5, // 连续充值
        DAY_COST_ITEM = 6, // 每日累计消耗道具
        COST_ITEM = 7, // 累计消耗道具
        DAY_BUY_GIFTPACK = 8, // 每日购买指定商城礼包
        BUY_GIFTPACK = 9, // 购买指定商城礼包
        DAY_LOGIN = 10, // 每日登陆
        LOGIN_DAYNUM = 11, // 累计登陆天数
        DAY_ONLINE_TIME = 12, // 每日累计在线
        ONLINE_TIME = 13, // 总累计在线
        DAY_COMPLETE_DUNG = 14, // 每日累计完成关卡
        COMPLETE_DUNG = 15, // 累计完成关卡
        BIND_PHONE = 16, // 手机绑定
        BUY_FASHION = 17, // 时装
        MALL_DISCOUNT_FOR_NEW_SERVER = 1000, // 新服商城时装打折
        LEVEL_FIGHTING_FOR_NEW_SERVER = 1001, // 新服冲级赛竞争阶段
        LEVEL_SHOW_FOR_NEW_SERVER = 1002, // 新服冲级赛公示阶段
        DUNGEON_DROP_ACTIVITY = 1100, //  地下城掉落活动
        DUNGEON_EXP_ADDITION = 1200, //  地下城结算经验加成
        PVP_PK_COIN = 1300, //  决斗币奖励
        APPOINTMENT_OCCU = 1400, //  预约职业活动
        HELL_TICKET_FOR_DRAW_PRIZE = 1500, //  深渊票消耗得抽奖
        FATIGUE_FOR_BUFF = 1600, //  疲劳消耗得BUFF
        FATIGUE_FOR_TOKEN_COIN = 1700, //  疲劳消耗得代币
        FATIGUE_BURNING = 1800, //  疲劳燃烧
        GAMBING = 1900, //  夺宝活动
        MALL_GIFT_PACK = 5000,//商城礼包
        DAILY_REWARD = 2000,//  每日奖励
        BOSS_KILL = 20000,//boss击杀
	    BOSS_EXCHANGE = 20001,//boss兑换
		BOSS_QUEST = 24000,//boss任务活动，走的boss活动的协议 所以叫这名字
        MAGPIE_BRIDGE = 2100,//鹊桥相会
        MONTH_CARD = 2200,//月卡活动
        MALL_ACTIVITY_GIFT_PACK = 30001,//商城活动礼包
        OAT_DUNGEON_DROP_RATE_ADDITION = 2400,//远古掉落活动
        CHANGE_FASHION = 2500,//百变换装活动
        OAT_CHANGE_FASHION_EXCHANGE = 2600,    // 绝版兑换
        OAT_CHANGE_FASHION_WELFARE = 2700,      // 换装福利 
        DUNGEON_RANDOM_BUFF = 2800,//地下城随机BUFF活动
        DUNGEON_CLEAR_GET_REWARD = 2900, //虚空通关活动
        OAT_BUY_FASHION_TICKET = 3000,//时装票购买活动
        BUFF_PRAY_ACTIVITY = 3100,//BUFF祈福活动
        OAT_GOBLIN_SHOP_ACT = 3400,//地精商店活动
        OAT_ARTIFACT_JAR = 3500,//神器罐子活动
        OAT_JAR_DRAW_LOTTERY = 3600,//罐子派奖活动
        OAT_LIMIT_TIME_HELL = 3700,//地下城限时活动
        OAT_BIND_GOLD_SHOP = 3900,  //绑金商店的活动
        OAT_RETURN_GIFT = 4100,// 回归赠礼活动
        OAT_ACCOUNT_SHOP = 4200,//账号兑换活动
        OAT_RETURN_REBATE = 4300,//回归返利
        OAT_CHALLENGE_CHAPTER = 4400,//挑战深渊
        OAT_RETURN_PRIVILEGE = 4500, //回归特权
        OAT_WEEK_DEEP = 4600,//周长深渊
        OAT_BUY_PRRSENT = 4700, //购买赠送
        OAT_LOGIN_REWARD = 4800, // 累计登录领奖
        OAT_PASS_DUNGEON_REWARD = 4900, //累计通关地下城领奖
        OAT_ARTIFACT_JAR_SHOW = 5200,//神器罐展示活动
        OAT_PROMOTE_EQUIP_STRENGTHEN_RATE = 5300,//提升强化率活动
        OAT_WEEK_SIGN_NEW_PLAYER = 5400, //新人周签到活动
        OAT_WEEK_SIGN_ACTIVITY = 5500,  //活动周签到活动
        OAT_SUMMER_WEEKLY_VACATION=5600,//暑期每周挑战活动
        OAT_SUMMER_GRANDdTOTAL_VACATION = 5700,//暑期累计挑战活动
        OAT_SECOND_ANNIVERSARY_PRAY = 5800,    // 两周年祈福
        OAT_FLYING_GIFTPACK_ACTIVITY = 5900,   //飞升礼包活动 
        OAT_TUANBEN_ACTIVITY = 6000,         //团本活动
        OAT_ANNIVERACCUMULATE = 6100,         //周年庆累计在线活动
        OAT_ANNIBERPARTY= 6200,//周年派对
        OAT_HALLOWEENONLINEACTIVITY = 6300, //万圣节在线活动
        OAT_NEWYEARONDAYACTIVITY=6400, //元旦活动
        OAT_CONSUMEREBATEACTIVITY=6500,//消费返利活动
        OAT_INTEGRATIONCHALLENGEACTIVITU= 6600,//积分挑战活动
        OAT_LIMITTIMEFIRSTDISCOUNTACTIIVITY=6800,//折扣活动id（这个活动不在限时活动界面显示，显示的内容在限时抢购5000 活动里面）
        OAT_MOUSEYEARREDPACKAGEACTIVITY = 6900,//鼠年红包活动
        OAT_SPRINGFESTIVALACTIVITY = 7000,//春节地下城
        OAT_SPRINGFESTIVALREDENVELOPERAINACTIVITY = 7100,//新春红包雨
        OAT_CHALLENGEPOINTSREWARDACTIVITY = 7200,//挑战俱乐部积分奖励
        OAT_SPINGCHALLENGEACTIVITY = 7300,//春季挑战活动
        OAT_SPRINGENGEPOINTSREWARDACTIVITY = 7400,//春季挑战积分奖励
        OAT_INAWEEKACTIVITY = 7500,//每周签到活动
        OAT_ONLINEGIFTGIVINGACTIVITY = 7600,//在线好礼活动
        OAT_PLANT_TREE = 7700,  //植树节活动
        OAT_FEEDBACKGIFTACTIVITY = 7900,//回馈大礼活动
        OAT_LIMITTIMEGROUPBUYACTIVITY = 50001, // 全民砍价活动
    }

	public static IActivity Create(uint activityId)
    {
        var activityType = GetActivityType(activityId);
        IActivity activity = null;
        switch (activityType)
        {
            case EActivityType.NONE:
                break;
            case EActivityType.BUY_GIFTPACK:
            case EActivityType.DAY_BUY_GIFTPACK:
            case EActivityType.COST_ITEM:
            case EActivityType.DAY_COST_ITEM:
            case EActivityType.COMBO_CHARGE:
            case EActivityType.SINGLE_CHARGE:
            case EActivityType.DAY_SINGLE_CHARGE:
            case EActivityType.TATOL_CHARGE:
            case EActivityType.DAY_TATOL_CHARGE:
            case EActivityType.LOGIN_DAYNUM:
            case EActivityType.DAY_COMPLETE_DUNG:
            case EActivityType.DAY_ONLINE_TIME:
            case EActivityType.ONLINE_TIME:
            case EActivityType.COMPLETE_DUNG:
            case EActivityType.DAY_LOGIN:
                activity = new LimitTimeCommonActivity();
                break;
            case EActivityType.MALL_DISCOUNT_FOR_NEW_SERVER:
                activity = new FashionDiscountActivity();
                break;
            case EActivityType.BUY_FASHION:
                activity = new FashionActivity();
                break;
            case EActivityType.LEVEL_FIGHTING_FOR_NEW_SERVER:
                activity = new LevelFightActivity();
                break;
            case EActivityType.LEVEL_SHOW_FOR_NEW_SERVER:
                activity = new LevelFightShowActivity();
                break;
            case EActivityType.DUNGEON_DROP_ACTIVITY:
            case EActivityType.DUNGEON_EXP_ADDITION:
            case EActivityType.PVP_PK_COIN:
            case EActivityType.OAT_DUNGEON_DROP_RATE_ADDITION:
                activity = new DungeonDropActivity();
                break;
            case EActivityType.APPOINTMENT_OCCU:
                activity = new ReservationUpgradeActivity();
                break;
            case EActivityType.HELL_TICKET_FOR_DRAW_PRIZE:
                activity = new HellTicketActivity();
                break;
            case EActivityType.FATIGUE_FOR_BUFF:
                activity = new FatigueForBuffActivity();
                break;
            case EActivityType.FATIGUE_FOR_TOKEN_COIN:
                activity = new CoinExchangeActivity();
                break;
            case EActivityType.FATIGUE_BURNING:
                activity = new FatigueBurnActivity();
                break;
            case EActivityType.DAILY_REWARD:
                activity = new DailyRewardActivity();
                break;
            case EActivityType.BOSS_KILL:
                activity = new BossKillActivity();
                break;
            case EActivityType.BOSS_EXCHANGE:
                activity = new BossExchangeActivity();
                break;
            case EActivityType.MALL_GIFT_PACK:
                activity = new LimitTimeMallGiftPackActivity();
                break;
            case EActivityType.MALL_ACTIVITY_GIFT_PACK:
                activity = new MallActivityGiftPackActivity();
                break;
            case EActivityType.MAGPIE_BRIDGE:
                activity = new QiXiQueQiaoActivity();
                break;
            case EActivityType.MONTH_CARD:
                activity = new MonthCardActivity();
                break;
			case EActivityType.BOSS_QUEST:
				activity = new BossQuestActivity();
				break;
            case EActivityType.CHANGE_FASHION:
                activity = new ChangeFashionActivity();
                break;
			case EActivityType.DUNGEON_RANDOM_BUFF:
                activity = new VanityBonusActivity();
                break;
            case EActivityType.DUNGEON_CLEAR_GET_REWARD:
                activity = new VanityCustomClearanceActivity();
                break;
            case EActivityType.OAT_BUY_FASHION_TICKET:
                activity = new FashionTicketBuyActivity();
                break;
            case EActivityType.OAT_CHANGE_FASHION_EXCHANGE:
                activity = new ChangeFashionSpecialExchangeActivity();
                break;
            case EActivityType.OAT_CHANGE_FASHION_WELFARE:
                activity = new ChangeFashionExchangeActivity();
                break;
			case EActivityType.BUFF_PRAY_ACTIVITY:
                //activity = new BuffPrayActivity();
                 activity = new BuffPrayActivityNew();
                break;
            case EActivityType.OAT_GOBLIN_SHOP_ACT:
                activity = new GoblinShopActivity();
                break;
            case EActivityType.OAT_LIMIT_TIME_HELL:
                activity = new LimitTimeHellActivity();
                break;
            case EActivityType.OAT_RETURN_GIFT:
                activity = new ReturnGiftActivity();
                break;
            case EActivityType.OAT_ACCOUNT_SHOP:
                activity = new ReturnExchangeActivity();
                break;
            case EActivityType.OAT_RETURN_REBATE:
                activity = new ConsumePointActivity();
                break;
            case EActivityType.OAT_CHALLENGE_CHAPTER:
                activity = new ConsumeDeepTicketActivity();
                break;
            case EActivityType.OAT_RETURN_PRIVILEGE:
                activity = new ReturnTeamBuffActivity();
                break;
            case EActivityType.OAT_PASS_DUNGEON_REWARD:
                activity = new AccumulateClearanceActivity();
                break;
            case EActivityType.OAT_BUY_PRRSENT:
                activity = new BuyPresentationActivity();
                break;
            case EActivityType.OAT_LOGIN_REWARD:
                activity = new AccumulateLoginActivity();
                break;
            case EActivityType.OAT_PROMOTE_EQUIP_STRENGTHEN_RATE:
                activity = new EquipmentUpgradeActivity();
                break;
            case EActivityType.OAT_SUMMER_WEEKLY_VACATION:
                activity = new SummerVacationWeeklyActivity();
                break;
            case EActivityType.OAT_SUMMER_GRANDdTOTAL_VACATION:
                activity = new SummerVacationGrandTotalActivity();
                break;
            case EActivityType.OAT_SECOND_ANNIVERSARY_PRAY:
                activity = new AnniversaryBuffPrayActivity();
                break;
            case EActivityType.OAT_FLYING_GIFTPACK_ACTIVITY:
                activity = new FlyingGiftPackActivity();
                break;
            case EActivityType.OAT_ANNIVERACCUMULATE:
                activity = new AnniversaryAccumulateClearanceActivity();
                break;
            case EActivityType.OAT_TUANBEN_ACTIVITY:
                activity = new TuanBenActivity();
                break;
            case EActivityType.OAT_ANNIBERPARTY:
                activity = new AnniversaryPartyActivity();
                break;
            case EActivityType.OAT_HALLOWEENONLINEACTIVITY:
                activity = new HalloweenOnlineActivity();
                break;
            case EActivityType.OAT_NEWYEARONDAYACTIVITY:
                activity = new NewYearOnDayActivity();
                break;
            case EActivityType.OAT_CONSUMEREBATEACTIVITY:
                activity = new ConsumeRebateActivity();
                break;
            case EActivityType.OAT_INTEGRATIONCHALLENGEACTIVITU:
                activity = new IntegrationChallengeActivity();
                break;
            case EActivityType.OAT_SPRINGFESTIVALACTIVITY:
                activity = new SpringFestivalDungeonActivity();
                break;
            case EActivityType.OAT_SPRINGFESTIVALREDENVELOPERAINACTIVITY:
                activity = new SpringFestivalRedEnvelopeRainActivity();
                break;
            case EActivityType.OAT_MOUSEYEARREDPACKAGEACTIVITY:
                activity = new MouseYearRedPackageActivity();
                break;
            case EActivityType.OAT_ONLINEGIFTGIVINGACTIVITY:
                activity = new OnLineGiftGivingActivity();
                break;
            case EActivityType.OAT_CHALLENGEPOINTSREWARDACTIVITY:
            case EActivityType.OAT_SPRINGENGEPOINTSREWARDACTIVITY:
                activity = new RewardPointsActivity();
                break;
            case EActivityType.OAT_SPINGCHALLENGEACTIVITY:
                activity = new SpringChallengeActivity();
                break;
            case EActivityType.OAT_INAWEEKACTIVITY:
                activity = new WeeklyCheckInActivity();
                break;
            case EActivityType.OAT_PLANT_TREE:
                activity = new ArborDayActivity();
                break;
            case EActivityType.OAT_FEEDBACKGIFTACTIVITY:
                activity = new FeedbackGiftActivity();
                break;
            case EActivityType.OAT_LIMITTIMEGROUPBUYACTIVITY:
                activity = new LimitTimeGroupBuyActivity();
                break;
        }

        if (activity != null)
            activity.Init(activityId);

        return activity;
    }

    //通过活动id获取活动的类型
    public static EActivityType GetActivityType(uint activityId)
    {
        if (ActivityDataManager.GetInstance() == null)
        {
            return EActivityType.NONE;
        }

        //商城礼包
        if (ActivityDataManager.GetInstance().IsContainGiftActivity(MallTypeTable.eMallType.SN_ACTIVITY_GIFT, activityId))
        {
            return EActivityType.MALL_ACTIVITY_GIFT_PACK;
        }

        //商城其他礼包活动
        if (ActivityDataManager.GetInstance().GetLimitTimeActivityData(activityId) != null)
        {
            var activity = ActivityDataManager.GetInstance().GetLimitTimeActivityData(activityId);
            if (activity.tmpType == (uint) OpActivityTmpType.OAT_LIMIT_TIME_GIFT_PACK)
            {
	            return EActivityType.MALL_GIFT_PACK;
			}
            //否则就是对应数值的活动类型
			return GetActivityTypeByLimitTimeType((OpActivityTmpType)activity.tmpType);

        }
        //boss活动 通过活动id在活动表中查找类型
        else if (ActivityDataManager.GetInstance().GetBossActivityData(activityId) != null)
        {
            var activeMainTableData = TableManager.GetInstance().GetTableItem<ActiveMainTable>((int)activityId);

            if (activeMainTableData != null)
            {
                if (activeMainTableData.ActivityType == ActiveMainTable.eActivityType.KillBossActivity)
                {
                    return EActivityType.BOSS_KILL;
                }
                else if (activeMainTableData.ActivityType == ActiveMainTable.eActivityType.ExchangeActivity)
                {
                    return EActivityType.BOSS_EXCHANGE;
                }
				else if (activeMainTableData.ActivityType == ActiveMainTable.eActivityType.QuestActivity)
                {
	                return EActivityType.BOSS_QUEST;
                }
            }
        }


        return EActivityType.NONE;
    }

    public static EActivityType GetActivityTypeByLimitTimeType(OpActivityTmpType Orgtype)
    {
        return (EActivityType) Orgtype;
    }

}