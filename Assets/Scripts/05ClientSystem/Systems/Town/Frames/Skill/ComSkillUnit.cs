using UnityEngine.UI;
using UnityEngine;
using GameClient;
using ProtoTable;
using UnityEditor;
using Protocol;
using UnityEngine.EventSystems;
using System;

public enum SkillUnitTextShowType
{
    ShowLevel = 0,
    ShowName,
}

class ComSkillUnit : MonoBehaviour
{
    public Drag_Me dragMe;
    public Toggle SkillToggle;
    public Image Icon;
    public UIGray uiGray;
    public Text SkilllevelText;
    public Text AddLevelText;
    public ComSetTextColor LevelColor;
    public GameObject LvUpRoot;
    public GameObject ForbidRoot;
    public GameObject allocate;
    public GameObject LearnRoot;
    public GameObject AwakeRoot;
    public GameObject TalentRoot;
    private Image mImgTalent;
    public SkillUnitTextShowType TextShowType = SkillUnitTextShowType.ShowLevel;
    public float DelayInitTime = 0.6f;
    [SerializeField] private string mLearnImgPath = "UI/Image/NewPacked/Skill.png:Skill_Img_Kexuexi";
    [SerializeField] private string mPvpForbidImgPath = "UI/Image/NewPacked/Skill.png:Skill_Img_Jinyong";
    [SerializeField] private string mSkillAwakeImgPath = "UI/Image/NewPacked/Skill.png:Skill_Img_Juexingjiesuo";
    [SerializeField] private string mSkillSetImgPath = "UI/Image/NewPacked/Skill.png:Skill_Img_Zhuang";
    [SerializeField] private string mTalentBgDefaultImgPath = "UI/Image/NewPacked/Skill.png:Img_Skill_Tianfu_Bg01";
    [SerializeField] private string mTalentIconPrefabPath = "UIFlatten/Prefabs/Skill/SkillUnitTalentIcon";
    [SerializeField] private string mTalentIconDefaultImgPath = "UI/Image/NewPacked/Skill.png:Img_Skill_Tianfu_Icon";
    [SerializeField] private Vector2 mTalentDefaultIconSize;
    [SerializeField] private Vector2 mTalentIconSize;

    private SkillTable SkillTableData = null;
    private int LearnedLevel = 0;
    private int AddLevel = 0;
    private bool bDelayInit = false;
    private float mTimer = 0.0f;
    private bool loadLvUpAnimDirty = false;
    private bool loadAllocateImgDirty = false;
    private bool loadForbidDirty = false;
    private bool loadLearnTextDirty = false;
    private bool loadAwakeTextDirty = false;

    void Start()
    {
        if (dragMe != null && dragMe.id <= 0)
        {
            dragMe.gameObject.CustomActive(false);
            return;
        }

        _BindUIEvent();
        _Init();
    }

    void OnDestroy()
    {
        _UnBindUIEvent();

        if (dragMe != null)
        {
            dragMe.ResponseDrag = null;
        }

        if (SkillToggle != null)
        {
            SkillToggle.onValueChanged.RemoveAllListeners();
        }

        loadLvUpAnimDirty = false;
        loadAllocateImgDirty = false;
        loadLearnTextDirty = false;
    }

    private void _BindUIEvent()
    {
        UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.SkillLearnedLevelChanged, _OnSkillLearnedLevelChanged);
        UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnSelectSkillPage, _OnSwitchSkillTree);
        UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.SpChanged, _OnSpChanged);
        UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.SkillBarChanged, _OnSkillSlotChanged);
    }

    private void _UnBindUIEvent()
    {
        UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.SkillLearnedLevelChanged, _OnSkillLearnedLevelChanged);
        UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnSelectSkillPage, _OnSwitchSkillTree);
        UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.SpChanged, _OnSpChanged);
        UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.SkillBarChanged, _OnSkillSlotChanged);
    }

    // //天赋修改
    // private void _OnSkillTalentChanged(UIEvent uiEvent)
    // {
    //     Skill skill = uiEvent.Param1 as Skill;

    //     if (skill == null || dragMe == null)
    //     {
    //         return;
    //     }
       
    //     if(dragMe.id != 0)
    //     {    
    //         if (skill.id == dragMe.id)
    //         {
    //             _setTalent();
    //         }
    //     }
    // }

    //等级和天赋写在一起了
    private void _OnSkillLearnedLevelChanged(UIEvent uiEvent)
    {
        Skill skill = uiEvent.Param1 as Skill;

        if (skill == null || dragMe == null)
        {
            return;
        }
       
        if(dragMe.id != 0)
        {    
            if (skill.id == dragMe.id)
            {             
                LearnedLevel = skill.level;
                _ShowText();
                _setTalent();
                _ShowUiGray();
            }
        }
    }

    private void _OnSwitchSkillTree(UIEvent uiEvent)
    {
        if(SkillFrame.frameParam == null)
        {
            return;
        }

        if (SkillFrame.frameParam.frameType != SkillFrameType.Normal)
        {
            return;
        }

        LearnedLevel = SkillDataManager.GetInstance().GetLearnedSkillLv(SkillTableData);
        AddLevel = _GetAddSkillLv();

        _ShowText();
        _ShowAddLevel();
        _ShowUiGray();
        _ShowLvUpAnim();
        _ShowAllocate();
        _setPvpForbid();
        _setTalent();
        
        if (SkillToggle != null && SkillToggle.isOn)
        {
            SkillToggle.isOn = false;
            SkillToggle.isOn = true;
        }
    }

    private void _OnSpChanged(UIEvent uiEvent)
    {
        if (dragMe == null)
        {
            return;
        }

        if (dragMe.id != 0)
        {
            _ShowLvUpAnim();
        }
    }

    private void _OnSkillSlotChanged(UIEvent uiEvent)
    {
        _ShowAllocate();
    }

    private void _Init()
    {
        bDelayInit = false;
        mTimer = 0.0f;

        if (dragMe == null)
        {
            Logger.LogErrorFormat("dragMe is null in [_Init], Ԥ����ְҵ:{0},����Ԥ����.", PlayerBaseData.GetInstance().JobTableID);
            return;
        }

        if (dragMe.id != 0 && SkillTableData == null)
        {
            SkillTableData = TableManager.GetInstance().GetTableItem<SkillTable>(dragMe.id);
        }

        dragMe.ResponseDrag = _DealDrag;

        LearnedLevel = SkillDataManager.GetInstance().GetLearnedSkillLv(SkillTableData);
        AddLevel = _GetAddSkillLv();

        _JudgeDelayInit();

        if(!bDelayInit)
        {
            _InitSkillInfo();
        }

        if (SkillToggle != null)
        {
            int skillid = dragMe.id;

            SkillToggle.onValueChanged.RemoveAllListeners();
            SkillToggle.onValueChanged.AddListener((value) => { _OnChooseSkill(skillid, value); });

            if (dragMe.GroupIndex == 1)
            {
                SkillToggle.isOn = true;
            }
        }
    }

    private void _JudgeDelayInit()
    {
        if(SkillTableData == null)
        {
            return;
        }

        if (PlayerBaseData.GetInstance().Level >= 30)
        {
            if (SkillTableData.LevelLimit < 30)
            {
                bDelayInit = true;
            }
        }
        else if (PlayerBaseData.GetInstance().Level > 10 && PlayerBaseData.GetInstance().Level < 30)
        {
            if (SkillTableData.LevelLimit <= 10 || SkillTableData.LevelLimit >= 30)
            {
                bDelayInit = true;
            }
        }
        else
        {
            if (SkillTableData.LevelLimit > 10)
            {
                bDelayInit = true;
            }
        }
    }

    private void _InitSkillInfo()
    {
        _LoadIcon();
        _ShowText();
        _ShowUiGray();
        _ShowAddLevel();
        _ShowLvUpAnim();
        _ShowAllocate();
        _SetAwakeTip();
        _setPvpForbid();
        _setTalent();
    }

    //天赋标记
    private void _setTalent()
    {
        if (SkillDataManager.GetInstance().IsSkillHaveTalent(SkillTableData.ID))
        {
            //有俩张图 底图相同 天赋图标区分为默认与天赋icon，分为俩个size
            //底图
            if (null == TalentRoot.GetComponent<Image>())
            {
                var img = TalentRoot.AddComponent<Image>();
                if (null != img)
                {
                    img.SafeSetImage(mTalentBgDefaultImgPath);
                }
            }
            //icon图
            if (null == mImgTalent)
            {
                GameObject obj = AssetLoader.instance.LoadResAsGameObject(mTalentIconPrefabPath);
                if (obj == null)
                {
                    Logger.LogError("Load [TalentIconPrefab] failed in ComSkillUnit");
                    return;
                }
                obj.transform.SetParent(TalentRoot.transform, false);
                mImgTalent = obj.GetComponent<Image>();
                if (null == mImgTalent)
                {
                    Logger.LogError("TalentIconPrefab没有挂Image");
                    return;
                }
            }
            if (null != mImgTalent)
            {
                var skilldata = SkillDataManager.GetInstance().GetCurSkillInfoById(SkillTableData.ID);
                if (null != skilldata)
                {
                    var table = TableManager.GetInstance().GetTableItem<TalentTable>((int)skilldata.talentId);
                    if (null != table)
                    {
                        mImgTalent.SafeSetImage(table.Icon);
                        mImgTalent.rectTransform.sizeDelta = mTalentDefaultIconSize;
                    }
                    else
                    {
                        mImgTalent.SafeSetImage(mTalentIconDefaultImgPath);
                        mImgTalent.rectTransform.sizeDelta = mTalentIconSize;
                    }
                }
                else
                {
                    mImgTalent.SafeSetImage(mTalentIconDefaultImgPath);
                    mImgTalent.rectTransform.sizeDelta = mTalentIconSize;
                }
            }
        }
        else
        {
            TalentRoot.CustomActive(false);
        }
    }

    //图标
    private void _LoadIcon()
    {
        if (Icon != null)
        {
            if(Icon.sprite == null)
            {
                if(SkillTableData != null)
                {
                    ETCImageLoader.LoadSprite(ref Icon, SkillTableData.Icon);
                    Icon.CustomActive(true);
                }
                else
                {
                    Icon.CustomActive(false);
                }
            }
            else
            {
                Icon.CustomActive(true);
            }
        }
    }

    //等级
    private void _ShowText()
    {
        if(SkilllevelText != null && SkillTableData != null)
        {
            if (TextShowType == SkillUnitTextShowType.ShowLevel)
            {
                SkilllevelText.text = string.Format("Lv.{0}/{1}", LearnedLevel, SkillTableData.TopLevelLimit);
            }
            else
            {
                SkilllevelText.text = SkillTableData.Name;
            }
        }
    }

    //附加等级
    private void _ShowAddLevel()
    {
        if(AddLevelText != null)
        {
            if(TextShowType == SkillUnitTextShowType.ShowLevel)
            {
                if (LearnedLevel > 0 && AddLevel > 0)
                {
                    AddLevelText.text = string.Format("(+{0})", AddLevel);
                }
                else
                {
                    AddLevelText.text = "";
                }
            }
            else
            {
                AddLevelText.text = "";
            }
        }
    }

    //可升级标记
    private void _ShowLvUpAnim()
    {
        bool show = SkillDataManager.GetInstance().CheckLvUp(LearnedLevel, SkillTableData, false);

        if(LearnedLevel > 0)
        {
            // ��ʾ��������
            if (loadLvUpAnimDirty)
            {
                if (LvUpRoot != null)
                {
                    LvUpRoot.CustomActive(show);
                }
            }
            else
            {
                if (show)
                {
                    _LoadLvUpAnim();
                }
            }

            // ��ѧϰ��������������ʾ����
            if(loadLearnTextDirty)
            {
                if (LearnRoot != null)
                {
                    LearnRoot.CustomActive(false);
                }
            }
        }
        else
        {
            if(loadLearnTextDirty)
            {
                if(LearnRoot != null)
                {
                    LearnRoot.CustomActive(show);
                }
            }
            else
            {
                if(show)
                {
                    _LoadLearnText();
                }
            }

            if (loadLvUpAnimDirty)
            {
                if (LvUpRoot != null)
                {
                    LvUpRoot.CustomActive(false);
                }
            }
        }
    }
    //加载动画
    private void _LoadLvUpAnim()
    {
        if(LvUpRoot == null)
        {
            return;
        }

        string path = "UIFlatten/Prefabs/Skill/SkillLvUpAnim";

        GameObject obj = AssetLoader.instance.LoadResAsGameObject(path);
        if (obj == null)
        {
            Logger.LogError("Load [SkillLvUpAnim] failed in ComSkillUnit");
            return;
        }

        obj.transform.SetParent(LvUpRoot.transform, false);

        loadLvUpAnimDirty = true;
    }
    //加载可学习标记
    private void _LoadLearnText()
    {
        if (LearnRoot == null)
        {
            return;
        }
        var img = LearnRoot.AddComponent<Image>();
        img.SafeSetImage(mLearnImgPath, true);
        // GameObject obj = AssetLoader.instance.LoadResAsGameObject(mPathLearnTip);
        // if (obj == null)
        // {
        //     Logger.LogError("Load [_LoadLearnText] failed in ComSkillUnit");
        //     return;
        // }
        // obj.transform.SetParent(LearnRoot.transform, false);

        loadLearnTextDirty = true;
    }

    //灰化
    private void _ShowUiGray()
    {
        // 添加判断该技能 是否等级为0/是否在pvp中被禁用
        if (LearnedLevel > 0)
        {
            // icon�û�
            if (uiGray != null)
            {
                uiGray.SetEnable(false);
            }

            // ������icon�ûұ���һ��
            if (LevelColor != null)
            {
                LevelColor.SetColor(0);
            }
        }
        else
        {
            if (uiGray != null)
            {
                uiGray.SetEnable(true);
            }

            if (LevelColor != null)
            {
                LevelColor.SetColor(1);
            }
        }
    }

    //
    private void _ShowAllocate()
    {
        if(SkillTableData == null)
        {
            return;
        }

        SkillBarGrid slotinfo = SkillDataManager.GetInstance().GetCurSlotInfoBySkillId((UInt16)SkillTableData.ID);

        bool show = (slotinfo != null);

        if (loadAllocateImgDirty)
        {
            if (allocate != null)
            {
                allocate.CustomActive(show);
            }
        }
        else
        {
            if (show)
            {
                _LoadAllocateImg();
            }
        }
    }

    private void _LoadAllocateImg()
    {
        if (allocate == null)
        {
            return;
        }

        Image img = allocate.AddComponent<Image>();

        ETCImageLoader.LoadSprite(ref img, mSkillSetImgPath);

        loadAllocateImgDirty = true;
    }

    private void _SetAwakeTip()
    {
        if (4 == SkillTableData.SkillCategory)
        {
            if (PlayerBaseData.GetInstance().AwakeState <= 0)
                _ShowAwakeTip();
            else
                AwakeRoot.CustomActive(false);
        }
    }

    private void _ShowAwakeTip()
    {
        AwakeRoot.CustomActive(true);
        if (!loadAwakeTextDirty)
        {
            var img = AwakeRoot.AddComponent<Image>();
            img.SafeSetImage(mSkillAwakeImgPath, true);
            loadAwakeTextDirty = true;
        }
    }

    //是否禁用
    private void _setPvpForbid()
    {
        // 添加判断该技能是否在pvp中被禁用
        if ((SkillFrame.frameParam.tabTypeIndex == SkillFrameTabType.PVP || SkillFrame.frameParam.frameType == SkillFrameType.FairDuel) && SkillTableData.CanUseInPVP == 3)
        {
            ForbidRoot.CustomActive(true);
            if (!loadForbidDirty)
            {
                loadForbidDirty = true;
                _LoadForbid();
            }
        }
        else
            ForbidRoot.CustomActive(false);
    }
    //加载禁用标记
    private void _LoadForbid()
    {
        var img = ForbidRoot.AddComponent<Image>();
        img.SafeSetImage(mPvpForbidImgPath, true);
    }

    //选中技能
    private void _OnChooseSkill(int skillid, bool value)
    {
        if (skillid <= 0 || !value)
        {
            return;
        }

        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnChooseSkill, skillid, LearnedLevel, AddLevel);
    }

    //处理拖拽
    private bool _DealDrag(PointerEventData DragData)
    {
        GameObject DragObj = DragData.pointerPress;

        if (DragObj == null)
        {
            return false;
        }

        Drag_Me dragme = DragObj.GetComponent<Drag_Me>();
        if (dragme == null)
        {
            Logger.LogError("[SkillUnit] DragMe is null");
            return false;
        }

        if(dragme.DragGroup != EDragGroup.SkillTreeGroup)
        {
            Logger.LogErrorFormat("[SkillUnit]�������ô���,DragGroup = {0},����Ԥ�������޸�", dragme.DragGroup);
            return false;
        }

        if (!ClientSystemManager.GetInstance().IsFrameOpen<SkillConfigurationFrame>())
        {
            return false;
        }

        return SkillDataManager.GetInstance().CheckDrag(dragme.id);
    }

    private int _GetAddSkillLv()
    {
        if (SkillTableData != null)
        {
            if (SkillFrame.frameParam.frameType == SkillFrameType.Normal && SkillFrame.frameParam.tabTypeIndex == SkillFrameTabType.PVE)
            {
                return SkillDataManager.GetInstance().GetAddedSkillLevel(SkillTableData.ID, true);
            }
            else
            {
                return SkillDataManager.GetInstance().GetAddedSkillLevel(SkillTableData.ID, false);
            }
        }

        return 0;
    }

    void Update()
    {
        mTimer += Time.deltaTime;

        if (mTimer > DelayInitTime && bDelayInit)
        {
            // 延迟初始化
            _InitSkillInfo();
            bDelayInit = false;
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(ComSkillUnit))]
public class DSkillUnitInspector : Editor
{
    public override void OnInspectorGUI()
    {      
        base.OnInspectorGUI();

        if (GUILayout.Button("生成icon", GUILayout.Width(200)))
        {
            _LoadOne(target as ComSkillUnit);
        }
    }

    private void _LoadOne(ComSkillUnit unit)
    {
        if (null == unit)
        {
            return;
        }

        var dragMe = unit.GetComponent<Drag_Me>();
        if (null == dragMe)
        {
            return;
        }

        var Icon = unit.Icon;

        var SkillTableData = TableManager.GetInstance().GetTableItem<SkillTable>(dragMe.id);
        if (Icon != null)
        {
            if (SkillTableData != null)
            {
                ETCImageLoader.LoadSprite(ref Icon, SkillTableData.Icon);
                Icon.CustomActive(true);
            }
            else
            {
                Icon.CustomActive(false);
            }
        }
    }
}
#endif