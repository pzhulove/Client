namespace GameClient
{
    public enum BaseJobID
    {
        Guijianshi = 10,
        Shenqiangshou = 20,
        Mofashi = 30,
        GeDouJia_girl=40,
        Shenqiangshou_girl = 50,
    }

    public enum ChangeJobState
    {
        JobState_Null = 0, // 不能转职
        CanChangeJob,  // 可以转职
        DoChangeJobTask, // 转职任务期间,
        CanAwake,         // 可以觉醒
        DoAwakeJobTask,     // 觉醒任务期间

        TYPE_NUM
    }

    public enum AuctionItemNumPerPage
    {
        FORTY_PER_PAGE = 40,
        EIGHT_PER_PAGE= 8,
    }

    public enum AuctionSearchLimitType
    {
        ItemTypeLimit = 0,
        QualityTypeLimit,
        ArmorTypeLimit,
        ItemLevelLimit,
        JobTypeLimit,
        StrengthLevelLimit,
    
        TYPE_NUM
    }

    // 等级类型
    public enum LevelType
    {
        OneToFive = 0,
        SixToTen,
        ElevenToFifteen,
        SixteenToTwenty,
        TwentyOneToTwetyFive,
        TewntySixToThirty,
        ThirtyOneToThirtyFive,
        ThirtySixToFouty,
        FortyOneToFortyFive,
        FortySixToFifty,
        FiftyOneToFiftyFive,
        FiftySixToSixty,

        TYPE_NUM
    }

    // 强化等级类型
    public enum StrengthenType
    {
        OneToThree = 0,
        FourToSix,
        SevenToTen,
        ElevenToThirteen,
        FourteenToFifteen,
        SixteenAbove,

        TYPE_NUM
    }
}