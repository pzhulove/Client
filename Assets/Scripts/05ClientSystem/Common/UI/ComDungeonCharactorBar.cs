using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public enum eDungeonCharactorBar
{
    /// <summary>
    /// 吟唱
    /// </summary>
    Sing,
    /// <summary>
    /// 蓄力
    /// </summary>
    Power,
    /// <summary>
    /// buff技能
    /// </summary>
    Buff,
    /// <summary>
    /// 光环机制
    /// </summary>
    Loop,
	//僵尸buff
	Continue,
	DunFu,
    //蹲伏CD
    DunFuCD,
    //天雷倒计时
    RayDrop,
    //炎机制条
    Fire,
    //周长副本正负极
    Progress,
    //怪物受伤蓄能条
    MonsterEnergyBar

}

public interface IDungeonCharactorBar
{
    void SetRate(float percent);

    eDungeonCharactorBar GetBarType();
	void SetBarType(eDungeonCharactorBar type);

    GameObject GetGameObject();

    void Show(bool isShow);

	void SetText(string content);
    void SetCdText(float cdTime);
}

public class ComDungeonCharactorBar : ComBaseCharactorBar, IDungeonCharactorBar
{
    public Image mIconBar;
    public Image mProcessBar;
    public Text contentText;

    public eDungeonCharactorBar mType;

    public void SetRate(float percent)
    {
        if (null != mProcessBar)
        {
            mProcessBar.fillAmount = Mathf.Clamp01(percent);
        }
    }

	public void SetBarType(eDungeonCharactorBar type)
	{
		mType = type;
	}

    public eDungeonCharactorBar GetBarType()
    {
        return mType;
    }

    public void Show(bool isShow)
    {
        _SetVisible(isShow);
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }

	public void SetText(string content)
	{
        if (contentText != null)
        {
            contentText.text = content;
        }
	}

    public void SetCdText(float cdTime)
    {

    }
}
