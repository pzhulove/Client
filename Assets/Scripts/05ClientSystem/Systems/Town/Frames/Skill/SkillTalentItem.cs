using System.Collections;
using System.Collections.Generic;
using GameClient;
using ProtoTable;
using UnityEngine;
using UnityEngine.UI;

public class SkillTalentItem : MonoBehaviour
{
    public delegate void OnTalentItemSelectEvent(TalentTable table);
    [SerializeField] private Image mImgIcon;
    [SerializeField] private Text mTextName;
    [SerializeField] private GameObject mObjSelect;
    [SerializeField] private GameObject mObjPvpForbid;
    private TalentTable mCurTable;
    private OnTalentItemSelectEvent mSlectEvent;
    public void OnInit(TalentTable curTable, bool isSelect, OnTalentItemSelectEvent eventSelect)
    {
        mCurTable = curTable;
        mSlectEvent = eventSelect;
        mTextName.SafeSetText(mCurTable.name);
        mImgIcon.SafeSetImage(mCurTable.Icon);
        mObjSelect.CustomActive(isSelect);
        mObjPvpForbid.CustomActive(false);
        //pvp禁用
        if (mCurTable.CanPvp)
        {
            if (SkillDataManager.GetInstance().GetCurType() != Protocol.SkillConfigType.SKILL_CONFIG_PVE)
            {
                mObjPvpForbid.CustomActive(true);
            }
        }
    }

    public void OnSelect()
    {
        if (null != mSlectEvent)
            mSlectEvent(mCurTable);
    }
}
