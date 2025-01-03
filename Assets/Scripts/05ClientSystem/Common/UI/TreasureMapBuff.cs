using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System;
using DG.Tweening;
using GameClient;

public class TreasureMapBuff : MonoBehaviour
{
    public Toggle mKeyToggle;
    public Image mKeyImage;
    public Toggle mMapToggle;
    public Image mMapImage;
    public Toggle mHuiZhangToggle;
    public Image mHuiZhangImage;
    public Toggle mHufuToggle;
    public Image mHufuImage;

    public GameObject mTextRoot;
    public Button mTextBtn;

    public Text mFakeText;
    public Text mRealText;

    private Toggle[] mToggleArray;
    private Image[] mImageArray;
    
    private string mMapString;
    private string mKeyString;
    private string mDefendString;
    private string mDebuffString;

    private string mColorMapString;
    private string mColorKeyString;
    private string mColorDefendString;
    private string mColorDebuffString;

    private string mActiveMapString;
    private string mActiveKeyString;
    private string mActiveDefendString;
    private string mActiveDebuffString;

    private string mActiveColorMapString;
    private string mActiveColorKeyString;
    private string mActiveColorDefendString;
    private string mActiveColorDebuffString;

    private static string mNameStringFormat = "<color=#FFCA00>{0}</color>";
    private static string mUnActiveString = "   <color=#BD3C31>（未激活）</color>";

    private void InitTRValue()
    {
        var mMapName = TR.Value("treasure_map_dungeon_key_name");
        var mKeyName = TR.Value("treasure_map_dungeon_map_name");
        var mDefendName = TR.Value("treasure_map_dungeon_defend_name");
        var mDebuffName = TR.Value("treasure_map_dungeon_debuff_name");
        var mColorMapName = string.Format(mNameStringFormat, mMapName);
        var mColorKeyName = string.Format(mNameStringFormat, mKeyName);
        var mColorDefendName = string.Format(mNameStringFormat, mDefendName);
        var mColorDebuffName = string.Format(mNameStringFormat, mDebuffName);
        var mMapDetail = TR.Value("treasure_map_dungeon_key_detail");
        var mKeyDetail = TR.Value("treasure_map_dungeon_map_detail");
        var mDefendDetail = TR.Value("treasure_map_dungeon_defend_detail");
        var mDebuffDetail = TR.Value("treasure_map_dungeon_debuff_detail");

        mActiveMapString = string.Format("{0}{1}\n{2}", mMapName, mUnActiveString, mMapDetail);
        mActiveKeyString = string.Format("{0}{1}\n{2}", mKeyName, mUnActiveString, mKeyDetail);
        mActiveDefendString = string.Format("{0}{1}\n{2}", mDefendName, mUnActiveString, mDefendDetail);
        mActiveDebuffString = string.Format("{0}{1}\n{2}", mDebuffName, mUnActiveString, mDebuffDetail);

        mActiveColorMapString = string.Format("{0}{1}\n{2}", mColorMapName, mUnActiveString, mMapDetail);
        mActiveColorKeyString = string.Format("{0}{1}\n{2}", mColorKeyName, mUnActiveString, mKeyDetail);
        mActiveColorDefendString = string.Format("{0}{1}\n{2}", mColorDefendName, mUnActiveString, mDefendDetail);
        mActiveColorDebuffString = string.Format("{0}{1}\n{2}", mColorDebuffName, mUnActiveString, mDebuffDetail);

        mMapString = string.Format("{0}\n{1}", mMapName, mMapDetail);
        mKeyString = string.Format("{0}\n{1}", mKeyName, mKeyDetail);
        mDefendString = string.Format("{0}\n{1}", mDefendName, mDefendDetail);
        mDebuffString = string.Format("{0}\n{1}", mDebuffName, mDebuffDetail);

        mColorMapString = string.Format("{0}\n{1}", mColorMapName, mMapDetail);
        mColorKeyString = string.Format("{0}\n{1}", mColorKeyName, mKeyDetail);
        mColorDefendString = string.Format("{0}\n{1}", mColorDefendName, mDefendDetail);
        mColorDebuffString = string.Format("{0}\n{1}", mColorDebuffName, mDebuffDetail);
    }

    void Start()
    {
        InitTRValue();

        mToggleArray = new Toggle[] { mKeyToggle, mMapToggle, mHuiZhangToggle, mHufuToggle };
        mImageArray = new Image[] { mKeyImage, mMapImage, mHuiZhangImage, mHufuImage };
        //UI
        if (mTextRoot)
        {
            mTextRoot.SetActive(false);
        }
        InitBuffList();
        if (mTextBtn)
        {
            mTextBtn.onClick.AddListener(() => 
            {
                for(int i = 0; i < mToggleArray.Length; ++i)
                {
                    mToggleArray[i].isOn = false;
                }
                mTextRoot.SetActive(false);
            });
        }

    }
    
    private void InitBuffList()
    {
        mKeyToggle.onValueChanged.AddListener(OnKeyToggleClick);
        mMapToggle.onValueChanged.AddListener(OnMapToggleClick);
        mHuiZhangToggle.onValueChanged.AddListener(OnHuiZhangToggleClick);
        mHufuToggle.onValueChanged.AddListener(OnHufuToggleClick);
    }
    

    void OnKeyToggleClick(bool flag)
    {
        if (flag)
        {
            mTextRoot.transform.position = mKeyToggle.transform.position;
            mTextRoot.SetActive(true);
            if (mKeyImage.enabled)
            {
                mFakeText.text = mActiveKeyString; 
                mRealText.text = mActiveColorKeyString;
            }
            else
            {
                mFakeText.text = mKeyString;
                mRealText.text = mColorKeyString;
            }
        }
        else
        {
            mTextRoot.SetActive(false);
        }
    }
    void OnMapToggleClick(bool flag)
    {
        if (flag)
        {
            mTextRoot.transform.position = mMapToggle.transform.position;
            mTextRoot.SetActive(true);
            if (mMapImage.enabled)
            {
                mFakeText.text = mActiveMapString;
                mRealText.text = mActiveColorMapString;
            }
            else
            {
                mFakeText.text = mMapString;
                mRealText.text = mColorMapString;
            }
        }
        else
        {
            mTextRoot.SetActive(false);
        }
    }
    void OnHuiZhangToggleClick(bool flag)
    {
        if (flag)
        {
            mTextRoot.transform.position = mHuiZhangToggle.transform.position;
            mTextRoot.SetActive(true);
            if (mHuiZhangImage.enabled)
            {
                mFakeText.text = mActiveDefendString;
                mRealText.text = mActiveColorDefendString;
            }
            else
            {
                mFakeText.text = mDefendString;
                mRealText.text = mColorDefendString;
            }
        }
        else
        {
            mTextRoot.SetActive(false);
        }
    }
    void OnHufuToggleClick(bool flag)
    {
        if (flag)
        {
            mTextRoot.transform.position = mHufuToggle.transform.position;
            mTextRoot.SetActive(true);
            if (mHufuImage.enabled)
            {
                mFakeText.text = mActiveDebuffString;
                mRealText.text = mActiveColorDebuffString;
            }
            else
            {
                mFakeText.text = mDebuffString;
                mRealText.text = mColorDebuffString;
            }
        }
        else
        {
            mTextRoot.SetActive(false);
        }
    }

    public void HideLock(int buffId)
    {
        if(buffId == 570205)
        {
            mKeyImage.enabled = false;
        }
        if(buffId == 570206)
        {
            mMapImage.enabled = false;
        }
        if(buffId == 570172)//魔王护符  吃到魔王护符后，大魔王九宫格范围内的骚扰技能会失效
        {
            mHufuImage.enabled = false;
        }
        if(buffId == 570173)//神圣徽章  大幅度降低大魔王的攻击能力、防御能力
        {
            mHuiZhangImage.enabled = false;
        }
    }
}
