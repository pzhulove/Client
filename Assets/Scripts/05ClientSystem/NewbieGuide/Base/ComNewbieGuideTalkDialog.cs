using GameClient;
using ProtoTable;

public class ComNewbieGuideTalkDialog : ComNewbieGuideBase
{
    public int id;

    #region vitrual
    public override void StartInit(params object[] args)
    {
        // 参数列表
        // 0.对话id
        // 1.保存点
        // 2.暂停单局

        base.StartInit(args);

        id = -1;
        mTryPauseBattle = true;

        if (args != null)
        {
            if (args.Length >= 1)
            {
                id = (int)args[0];
            }

            if (args.Length >= 2)
            {
                if ((eNewbieGuideAgrsName)args[1] == eNewbieGuideAgrsName.SaveBoot)
                {
                    mSendSaveBoot = true;
                }
            }

            if (args.Length >= 3)
            {
                if ((eNewbieGuideAgrsName)args[2] == eNewbieGuideAgrsName.PauseBattle)
                {
                    mTryPauseBattle = true;
                }
            }
        }
    }

    protected override GuideState _init()
    {
        if (id <= 0)
        {
            Logger.LogWarningFormat("新手引导报错---对话ID不能为0,引导步骤:{0}", mGuideControl.GuideTaskID);
            return GuideState.Exception;
        }

        TalkTable talkdata = TableManager.GetInstance().GetTableItem<TalkTable>(id);

        if(talkdata == null)
        {
            Logger.LogWarningFormat("新手引导报错---对话表找不到ID为{0}的对话,引导步骤:{1}", id, mGuideControl.GuideTaskID);
        }

        if (!AddDialog(id, null))
        {
            Logger.LogWarningFormat("新手引导报错---创建[TalkDialog]类型对话失败");
            return GuideState.Exception;
        }
        
        return GuideState.Normal;
    }
    #endregion
}
