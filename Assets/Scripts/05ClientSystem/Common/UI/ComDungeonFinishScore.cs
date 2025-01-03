using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Protocol;
using ProtoTable;
using System;
using GameClient;

public class ComDungeonFinishScore : MonoBehaviour
{
    public ComDungeonScoreInfo[] infos;
    public Image mDungeonScore;
    public Sprite[] mScoreList;
    //public Text mExp;
    public GameObject mSuccessRoot;
    public GameObject mSuccessScore;
    public GameObject mSuccessEffect;
    public GameObject mFailRoot;

    public TextEx mTextPassTime;

    public Text mScoreDesText;
    public Text mDiffDesText;

    public TextEx mTextLevel;


    public ComExpBar mExpBar;

    public ComCommonBind mScoreBind;

    public void Init(int sumExp, DungeonScore finalScore)
    {
        _setOneSplit("sumexp", sumExp);

        //if (null != mExp)
        //{
        //    mExp.text = sumExp.ToString();
        //}


        UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ExpChanged, _onExpUpdate);

        if (null != mExpBar)
        {
            //var sum = (UInt64)(TableManager.instance.GetTableItem<ProtoTable.ExpTable>(60 - 1).TotalExp);

            mExpBar.TextFormat = exp =>
            {
                var ky = TableManager.instance.GetCurRoleExp(exp);
                if (ky.Value == 0)
                {
                    return "100%";
                }
                else
                {
                    double dRate = ((double)ky.Key) / ((double)ky.Value);
                    return string.Format("{0:P}", dRate, ky.Key, ky.Value);
                }
            };

            mExpBar.SetExp(GameClient.BattleDataManager.GetInstance().originExp, true, (exp)=>
            {
                return TableManager.instance.GetCurRoleExp(exp);
                //return new KeyValuePair<UInt64, UInt64>(exp, sum);
            });

            _onExpUpdate(null);
        }

        int time = BattleMain.instance.GetDungeonStatistics().AllFightTime(true);
        int passTime = time / 1000 + (int)TimeManager.GetInstance().GetServerTime();
        string strPassTime = Function.SetShowTime(passTime);
        mTextPassTime.SafeSetText(strPassTime);

        _setCallback( 0, 
                ()=> {
                    return BattleMain.instance.GetDungeonStatistics().AllHitCount();
                },
                ()=> {
                    return BattleMain.instance.GetDungeonStatistics().HitCountScore();
                });

        _setCallback( 1, 
                ()=>{ return BattleMain.instance.GetDungeonStatistics().AllFightTime(true); },
                ()=>{ return BattleMain.instance.GetDungeonStatistics().AllFightTimeScore(true); });

        _setCallback( 2, 
                ()=> {
                    return BattleMain.instance.GetDungeonStatistics().AllRebornCount();
                },
                ()=> {
                    return BattleMain.instance.GetDungeonStatistics().RebornCountScore();
                });

        if (infos != null)
        {
            for (int i = 0; i < this.infos.Length; i++)
            {
                if (this.infos[i] != null)
                {
                    this.infos[i].curScore = (int)finalScore;

                }
            }
        }
        _updateInfo();

		_setScore(finalScore);
    }

    private void _onExpUpdate(UIEvent ui)
    {
        if (null != mExpBar)
        {
            mExpBar.SetExp(PlayerBaseData.GetInstance().Exp, false, (exp)=>
            {
                return TableManager.instance.GetCurRoleExp(exp);
            });
        }
    }

    private void _setOneSplit(string key, int va)
    {
        if (null != mScoreBind)
        {
            ComFlyNumber kv = mScoreBind.GetCom<ComFlyNumber>(key);

            if (null != kv)
            {
                kv.SetNumber(va);
            }
        }
    }

    private float _getCurrentVipExpRate()
    {
        return Utility.GetCurVipLevelPrivilegeData(ProtoTable.VipPrivilegeTable.eType.DUNGEON_EXP);
    }

    private bool _hasVipExpRate()
    {
        return _getCurrentVipExpRate() > 0.0f;
    }

    private float _getCurrentVipGoldRate()
    {
        return Utility.GetCurVipLevelPrivilegeData(ProtoTable.VipPrivilegeTable.eType.DUNGEON_DROP_GOLD);
    }

    private bool _hasVipGoldRate()
    {
        return _getCurrentVipGoldRate() > 0.0f;
    }


    private bool _hasMonthCard()
    {
        BattlePlayer player = BattleMain.instance.GetPlayerManager().GetMainPlayer();
        if (null == player)
        {
            return false;
        }

        return player.IsPlayerMonthCard();
    }

    public void SetExpSplit(int baseExp, int drugExp, int scoreExp, int diffExp, int vipexp = 0, int vipgold = 0, int monthCardGold = 0,int tapAdd = 0,int relationExp = 0)
    {
        _setOneSplit("base", baseExp);
        _setOneSplit("drug", drugExp);
        _setOneSplit("score", scoreExp);
        _setOneSplit("diff", diffExp);

        Text viplevelexp = mScoreBind.GetCom<Text>("viplevelexp");
        Text viplevelgold = mScoreBind.GetCom<Text>("viplevelgold");
        ComFlyNumber vipcountgold = mScoreBind.GetCom<ComFlyNumber>("vipcountgold");
        ComFlyNumber vipcountexp = mScoreBind.GetCom<ComFlyNumber>("vipcountexp");
        Text normallevelexp = mScoreBind.GetCom<Text>("normallevelexp");
        Text normalcountexp = mScoreBind.GetCom<Text>("normalcountexp");
        GameObject normalExpRoot = mScoreBind.GetGameObject("normalExpRoot");
        GameObject vipExpRoot = mScoreBind.GetGameObject("vipExpRoot");
        GameObject vipGoldRoot = mScoreBind.GetGameObject("vipGoldRoot");
        GameObject monthCardRoot = mScoreBind.GetGameObject("monthCardRoot");
        Text monthGoldAdd = mScoreBind.GetCom<Text>("monthGoldAdd");
        GameObject tapRoot = mScoreBind.GetGameObject("tapRoot");
        Text tapExp = mScoreBind.GetCom<Text>("tapExp");
        GameObject relationRoot = mScoreBind.GetGameObject("relationroot");
        Text relationExpText = mScoreBind.GetCom<Text>("relationExp");
        Text otherExpText = mScoreBind.GetCom<Text>("otherExp");

        bool relationTapIsFlag = tapAdd > relationExp;
        vipExpRoot.SetActive(false);
        normalExpRoot.SetActive(false);
        monthCardRoot.SetActive(false);
        vipGoldRoot.SetActive(false);
        tapRoot.SetActive(relationTapIsFlag && tapAdd > 0);
        relationRoot.SetActive(!relationTapIsFlag && relationExp > 0);
        
        if(tapAdd > 0)
        {
            if(null != tapExp)
            {
                tapExp.text = tapAdd.ToString();
            }
        }

        if (relationExp > 0)
        {
            if (relationExpText != null)
            {
                relationExpText.text = relationExp.ToString();
            }
        }

        if (_hasVipExpRate())
        {
            vipExpRoot.SetActive(true);
            viplevelexp.text = PlayerBaseData.GetInstance().VipLevel.ToString();
            vipcountexp.SetNumber(vipexp);
        }
        else 
        {
            normalExpRoot.SetActive(true);

            KeyValuePair<int, float> kv = Utility.GetFirstValidVipLevelPrivilegeData(ProtoTable.VipPrivilegeTable.eType.DUNGEON_EXP);

            if (kv.Key > 0)
            {
                normallevelexp.text = kv.Key.ToString();
                normalcountexp.text = ((int)(kv.Value * 100)).ToString();
            }
        }

        if (_hasVipGoldRate())
        {
            vipGoldRoot.SetActive(true);
            viplevelgold.text = PlayerBaseData.GetInstance().VipLevel.ToString();
            vipcountgold.SetNumber(vipgold);
        }

        if (_hasMonthCard() || monthCardGold > 0)
        {
            monthCardRoot.SetActive(true);
            monthGoldAdd.text = monthCardGold.ToString();
        }

        int relationExpAdd = relationTapIsFlag ? tapAdd : relationExp;
        otherExpText.SafeSetText((baseExp + drugExp + scoreExp + diffExp + relationExpAdd).ToString());
    }

    private void _setCallback(int idx, ComDungeonScoreInfo.ScoreInfoCallback cb, ComDungeonScoreInfo.ScoreInfoScoreCallback scb)
    {
        if (idx < infos.Length && idx >= 0 && infos[idx] != null)
        {
            infos[idx].SetCallback(cb);
            infos[idx].SetScoreCallback(scb);
        }
    }

    private void _updateInfoByIdx(int idx)
    {
        if (idx < infos.Length && idx >= 0 && infos[idx] != null)
        {
            infos[idx].UpdateInfo();
        }
    }

    public void Uninit()
    {
        for (int i = 0; i < infos.Length; i++)
        {
            _setCallback(i, null, null);
        }
        
        UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ExpChanged, _onExpUpdate);
    }

    private void _set1Image(Image image, bool isSucces, string spritename)
    {
        if (null != mScoreBind)
        {
            // image.sprite = mScoreBind.GetSprite(spritename);
            mScoreBind.GetSprite(spritename, ref image);

            if (isSucces)
            {
                var effect = mScoreBind.GetPrefabInstance("effect");
                Utility.AttachTo(effect, image.gameObject);
            }
        }
    }

	private void _setScore(DungeonScore finalScore)
    {
        var player = BattleMain.instance.GetPlayerManager().GetMainPlayer();
        //var finalScore = BattleMain.instance.GetDungeonStatistics().FinalDungeonScore(player.playerInfo.seat);

        if (null != mScoreDesText)
        {
            mScoreDesText.text = finalScore.ToString();
        }

        mTextLevel.SafeSetText("LV." + PlayerBaseData.GetInstance().Level.ToString());

        if (null != mDiffDesText)
        {
            DungeonID id = new DungeonID(BattleDataManager.GetInstance().BattleInfo.dungeonId);
            DungeonTable node = TableManager.instance.GetTableItem<DungeonTable>(BattleDataManager.GetInstance().BattleInfo.dungeonId);
            string hardStr = GameClient.ChapterUtility.GetHardString(id.diffID);
            string hardCol = GameClient.ChapterUtility.GetHardColorString(id.diffID);
            if (node != null)
            {
                if (node.SubType == DungeonTable.eSubType.S_LIMIT_TIME_HELL ||
                    node.SubType == DungeonTable.eSubType.S_LIMIT_TIME__FREE_HELL||
                    node.SubType == DungeonTable.eSubType.S_WEEK_HELL ||
                    node.SubType == DungeonTable.eSubType.S_WEEK_HELL_PER ||
                    node.SubType == DungeonTable.eSubType.S_ANNIVERSARY_HARD ||
                    node.SubType == DungeonTable.eSubType.S_ANNIVERSARY_NORMAL)
                {
                    hardStr = GameClient.ChapterUtility.GetHardString(3);
                    hardCol = GameClient.ChapterUtility.GetHardColorString(3);
                }
            }
            mDiffDesText.text = string.Format("<color={0}>{1}</color>", hardCol, hardStr);
        }

        if (null != mScoreBind)
        {
            Image s0 = mScoreBind.GetCom<Image>("s0");
            Image s1 = mScoreBind.GetCom<Image>("s1");
            Image s2 = mScoreBind.GetCom<Image>("s2");
            GameObject fail = mScoreBind.GetGameObject("fail");
            GameObject successRoot = mScoreBind.GetGameObject("successroot");
            GameObject failRoot = mScoreBind.GetGameObject("failroot");

            s0.enabled = (false);
            s1.enabled = (false);
            s2.enabled = (false);

            successRoot.CustomActive(true);

            fail.CustomActive(false);
            failRoot.CustomActive(false);

            switch (finalScore)
            {
                case DungeonScore.C:
                    //s1.enabled = (true);
                    fail.CustomActive(true);
                    failRoot.CustomActive(true);
                    successRoot.CustomActive(false);
                    //_set1Image(s1, false, "fail");
                    GameStatisticManager.GetInstance().DoStartCheckPointsSettlement(BattleMain.instance.GetDungeonManager().GetDungeonDataManager().id.dungeonID, "C");
                    break;
                case DungeonScore.A:
                case DungeonScore.B:
                    s1.enabled = (true);
                    _set1Image(s1, true, "a");
                    GameStatisticManager.GetInstance().DoStartCheckPointsSettlement(BattleMain.instance.GetDungeonManager().GetDungeonDataManager().id.dungeonID, "A");
                    break;
                case DungeonScore.S:
                    s1.enabled = (true);
                    _set1Image(s1, true, "s");
                    GameStatisticManager.GetInstance().DoStartCheckPointsSettlement(BattleMain.instance.GetDungeonManager().GetDungeonDataManager().id.dungeonID, "S");
                    break;
                case DungeonScore.SS:
                    //s1.gameObject.SetActive(true);
                    //s2.gameObject.SetActive(true);
                    s1.enabled = (true);
                    s2.enabled = (true);

                    _set1Image(s1, true, "s");
                    _set1Image(s2, true, "s");
                    GameStatisticManager.GetInstance().DoStartCheckPointsSettlement(BattleMain.instance.GetDungeonManager().GetDungeonDataManager().id.dungeonID, "SS");
                    break;
                case DungeonScore.SSS:
                    s0.enabled = (true);
                    s1.enabled = (true);
                    s2.enabled = (true);

                    _set1Image(s0, true, "s");
                    _set1Image(s1, true, "s");
                    _set1Image(s2, true, "s");
                    GameStatisticManager.GetInstance().DoStartCheckPointsSettlement(BattleMain.instance.GetDungeonManager().GetDungeonDataManager().id.dungeonID, "SSS");
                    break;
                default:
                    break;
            }
        }
    }

    private void _updateInfo()
    {
        for (int i = 0; i < infos.Length; ++i)
        {
            _updateInfoByIdx(i);
        }
    }
}
