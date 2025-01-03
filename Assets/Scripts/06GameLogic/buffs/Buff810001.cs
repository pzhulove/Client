using UnityEngine;
using System.Collections;

public class Buff810001 : BeBuff
{
	
    private enum eState
    {
        None,
        Inited,
        Checked,
    }

    private int mEffectID = -1;
    private eState mState = eState.None;

    public Buff810001(int bi, int buffLevel, int buffDuration, int attack = 0) : base(bi, buffLevel, buffDuration, attack)
    {
    }
	/*
    public override void OnInit()
    {
        if (buffData.ValueA.Count >= 1)
        {
            mEffectID = TableManager.GetValueFromUnionCell(buffData.ValueA[0], 1);
        }

        mAnotherBuff = null;
        mAnotherBuffDuration = -1;

		RegisterEvent(BeEventType.onBuffCreateEffect, (object[] args)=>{
			var effect = args[0] as GeEffectEx;
			if (effect != null && owner != null)
				owner.m_pkGeActor.AddTag(effect);
		});

		RegisterEvent(BeEventType.onBuffRemoveEffect, (object[] args)=>{
			var effect = args[0] as GeEffectEx;
			if (effect != null && owner != null)
				owner.m_pkGeActor.RemoveTag(effect);
		});

		RegisterEvent(BeEventType.onBuffReplaceEffect, (object[] args)=>{
			var oldEffect = args[0] as GeEffectEx;
			var newEffect = args[1] as GeEffectEx;
			if (newEffect != null && owner != null)
				owner.m_pkGeActor.ReplaceTag(newEffect, oldEffect);
		});
    }

    public override void OnStart()
    {
        _init();
        mState = eState.Inited;
    }

    BeBuff mAnotherBuff = null;
    int mAnotherBuffDuration = -1;

    private void _init()
    {
        mAnotherBuff = null;
        mAnotherBuffDuration = -1;

        var buffList = owner.buffController.GetBuffList();

        for (int i = 0; i < buffList.Count; ++i)
        {
            if (buffList[i].buffID == buffID && buffList[i] != this)
            {
                mAnotherBuff = buffList[i];
                mAnotherBuffDuration = mAnotherBuff.duration;

                buffList[i].ResetDuration(int.MaxValue);
            }
        }
    }

    public override void OnUpdate(int delta)
    {
        if (mState == eState.Inited)
        {
            mState = eState.Checked;

            //_updateCnt(owner.buffController.GetBuffCountByID(buffID));

            // 这里该类型的buff刚叠加满
            if (buffData.OverlayLimit > 0 && buffData.OverlayLimit == owner.buffController.GetBuffCountByID(buffID))
            {
                // 清除所有该类型的标记buff
                owner.buffController.RemoveBuff(buffID);

                Cancel();

                _tryDoEffect();
                _updateCnt(0);
            }
        }
    }

    private void _updateCnt(int cnt)
    {
    }

    private void _tryDoEffect()
    {
        if (mEffectID > 0)
        {
            if (releaser != null)
            {
                Logger.LogProcessFormat("relase {0} attack owner {1} with effectID {2}", releaser.GetName(), owner.GetName(), mEffectID);
                releaser.DoAttackTo(owner, mEffectID);
            }
            else
            {
                Logger.LogError("release is nil");
            }
        }
    }

    public override void OnFinish()
    {
        if (null != mAnotherBuff)
        {
            mAnotherBuff.ResetDuration(mAnotherBuffDuration);
            mAnotherBuff = null;
        }

        _updateCnt(owner.buffController.GetBuffCountByID(buffID) - 1);


    }
    */
}
