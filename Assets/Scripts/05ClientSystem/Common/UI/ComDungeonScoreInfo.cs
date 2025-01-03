using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using ProtoTable;

public class ComDungeonScoreInfo : MonoBehaviour 
{
    public enum eType
    {
        eTime,
        eValue,
    }

    public enum eScoreType
    {
        None,
        HitCount,
        FightTime,
        ReborCount,
        StandardDamage,
        DamageNumber,
    }

    public eScoreType mScoreType;

    public GameObject mSuccesFail;
    public Text mLimitText;
    public ComTime mComLimitTime;
    public GameObject[] mNormalInfo;
    public GameObject[] mGuildeInfo;

    public ComCommonBind mBind;

    public delegate int TimeLimitCallback();
    public delegate int ScoreInfoCallback();
    public delegate int ScoreInfoScoreCallback();

    public Sprite[] mScore;
    public int mCurScore = 0;
    public int mMaxScore = 2;
    public Image mScoreImage;
    public Text mWord;

    public eType mType = eType.eValue;
    public ComTime mComTime;

    public int scoreStandard = 0;
    public int curScore = 0;
    public int scoreLevel = 0;
    public Text mDamageDesc = null;

    ScoreInfoCallback      mCallback;
    ScoreInfoScoreCallback mCallScoreback;
    TimeLimitCallback      mTimeLimitCallback;

    private int mTime;
    public void Init()
    {
        if (mScoreType == eScoreType.StandardDamage || mScoreType == eScoreType.DamageNumber)
        {
            if (mNormalInfo != null)
            {
                for (int i = 0; i < mNormalInfo.Length; i++)
                {
                    if (mNormalInfo[i] != null)
                        mNormalInfo[i].CustomActive(false);
                }
            }
            if (mGuildeInfo != null)
            {
                for (int i = 0; i < mGuildeInfo.Length; i++)
                {
                    if (mGuildeInfo[i] != null)
                        mGuildeInfo[i].CustomActive(true);
                }
            }

            mDamageDesc = null;
            if (mScoreType == eScoreType.StandardDamage && mGuildeInfo.Length > 1 && mGuildeInfo[1] != null)
                mDamageDesc = mGuildeInfo[1].GetComponent<Text>();
            if (mComTime != null)
            {
                mComTime.enabled = false;
            }
        }
        else
        {
            if (mNormalInfo != null)
            {
                for (int i = 0; i < mNormalInfo.Length; i++)
                {
                    if (mNormalInfo[i] != null)
                        mNormalInfo[i].CustomActive(true);
                }
            }
            if (mGuildeInfo != null)
            {
                for (int i = 0; i < mGuildeInfo.Length; i++)
                {
                    if (mGuildeInfo[i] != null)
                        mGuildeInfo[i].CustomActive(false);
                }
            }
            if (mComTime != null)
            {
                mComTime.enabled = true;
            }
        }
        SetScore(mMaxScore, mMaxScore);
        if (null != mWord) { mWord.text = ""; }
    }

    public void SetTimeLimiteCallback(TimeLimitCallback cb)
    {
        mTimeLimitCallback = cb;
    }

    public void SetCallback(ScoreInfoCallback cb)
    {
        mCallback = cb;
    }

    public void SetScoreCallback(ScoreInfoScoreCallback cb)
    {
        mCallScoreback = cb;
    }

    private void SetScore(int score, int maxScore)
    {
        mCurScore = score;
        mMaxScore = maxScore;
        mMaxScore = _getMaxScore();

        var len = mScore.Length;

        mCurScore %= (mMaxScore + 1);

        if (mScoreImage != null)
        {
            if (mCurScore == mMaxScore)
            {
                mScoreImage.sprite = mScore[len - 1];
            }
            else if (mCurScore == 0)
            {
                mScoreImage.sprite = mScore[0];
            }
            else if (mCurScore < mMaxScore)
            {
                mScoreImage.sprite = mScore[mCurScore];
            }
            mScoreImage.SetNativeSize();
        }
    }

    private int _getMaxScore()
    {
        int dungeonID = GameClient.BattleDataManager.GetInstance().BattleInfo.dungeonId;
        DungeonTable tab = TableManager.instance.GetTableItem<DungeonTable>(dungeonID);
        if (null != tab)
        {
            switch (mScoreType)
            {
                case eScoreType.FightTime:
                    return tab.TimeSplitArg.eValues.everyValues.Count - 1;
                case eScoreType.HitCount:
                    return tab.HitSplitArg.eValues.everyValues.Count - 1;
                case eScoreType.ReborCount:
                    return tab.RebornSplitArg.eValues.everyValues.Count - 1;
            }
        }

        return 2;
    }

    private int _getTopScore()
    {
        int dungeonID = GameClient.BattleDataManager.GetInstance().BattleInfo.dungeonId;
        DungeonTable tab = TableManager.instance.GetTableItem<DungeonTable>(dungeonID);
        if (null != tab)
        {
            switch (mScoreType)
            {
                case eScoreType.FightTime:
                    return TableManager.GetValueFromUnionCell(tab.TimeSplitArg, tab.TimeSplitArg.eValues.everyValues.Count - 1);
                case eScoreType.HitCount:
                    return TableManager.GetValueFromUnionCell(tab.HitSplitArg, tab.HitSplitArg.eValues.everyValues.Count - 1);
                case eScoreType.ReborCount:
                    return TableManager.GetValueFromUnionCell(tab.RebornSplitArg, tab.RebornSplitArg.eValues.everyValues.Count - 1);
            }
        }

        return -1;
    }

    private void _updateInfos()
    {
        int scoreSplit = _getTopScore();
        bool flag = false;
        int value =  0;

        if (null != mCallback)
        {
            value = mCallback.Invoke();
        }

        switch (mScoreType)
        {
            case eScoreType.FightTime:
                {
                    mComLimitTime.SetTime(scoreSplit * 1000);
                    flag = (scoreSplit * 1000) > value;
                }
                break;
            case eScoreType.HitCount:
                {
                    mLimitText.text = scoreSplit.ToString();
                    flag = scoreSplit > value;
                }
                break;
            case eScoreType.ReborCount:
                {
                    mLimitText.text = scoreSplit.ToString();
                    flag = scoreSplit > value;
                }
                break;
            case eScoreType.StandardDamage:
                {
                    if (mDamageDesc != null)
                        mDamageDesc.text = scoreStandard.ToString();
                    if (curScore >= scoreLevel)
                    {
                        flag = true;
                    }
                    else
                    {
                        flag = false;
                    }
                    SetScore(curScore, scoreLevel);
                }
                break;
        }


        if (null != mBind)
        {
            GameObject successRoot = mBind.GetGameObject("successRoot");
            if (null != successRoot)
            {
                successRoot.SetActive(flag);
            }

            GameObject failRoot    = mBind.GetGameObject("failRoot");
            if (null != failRoot)
            {
                failRoot.SetActive(!flag);
            }

            if (flag)
            {
                GameObject effect = mBind.GetPrefabInstance("effect_dacheng");
                Utility.AttachTo(effect, mSuccesFail);
            }
        }
    }

    public void UpdateInfo()
    {
        _updateInfos();
        if (mScoreType == eScoreType.StandardDamage || mScoreType == eScoreType.DamageNumber) return;
        if (null != mCallback)
        {
            try
            {
                var value = mCallback.Invoke();

                if (mType == eType.eValue)
                {
                    mWord.text = value.ToString();

                }
                else if (mType == eType.eTime)
                {
                    if (null != mTimeLimitCallback)
                    {
                        try
                        {
                            mComTime.mTimeInLimit = mTimeLimitCallback.Invoke() * 1000;
                        }
                        catch
                        {
                            mComTime.mTimeInLimit = int.MaxValue;
                        }
                    }
                    mComTime.SetTime(value);
                }
            }
            catch { }
        }

        if (null != mCallScoreback)
        {
            try
            {
                var score = mCallScoreback.Invoke();
                SetScore(score, mMaxScore);
            }
            catch
            { }
        }
    }
}
