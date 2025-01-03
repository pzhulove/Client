#if !LOGIC_SERVER
using System.Collections.Generic;
using GameClient;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSelectSkillJoystick :SkillJoystick
{
    private int lastSelectIndex;
    private List<int> seatList = new List<int>();   //玩家座位列表
    public override string PrefabPath => "UIFlatten/Prefabs/ETCInput/HGJoystickSelectSeat";
    public override void OnStart()
    {
        base.OnStart();
        InitSelectJoystickUI();

        lastSelectIndex = 0;
        seatList.Clear();
        if (BattleMain.instance == null || BattleMain.instance.GetPlayerManager() == null)
            return;
        List<BattlePlayer> playerList = BattleMain.instance.GetPlayerManager().GetAllPlayers();
        if (playerList == null)
            return;
        playerList = PlayerListSort(playerList, m_Owner);
        for (int i = 0; i < playerList.Count; i++)
        {
            if (playerList[i].playerActor == null)
                continue;
            int seat = playerList[i].GetPlayerSeat();
            
            var jobData = TableManager.GetInstance().GetTableItem<ProtoTable.JobTable>(playerList[i].playerInfo.occupation);
            if (jobData == null)
                continue;
            var resData = TableManager.GetInstance().GetTableItem<ProtoTable.ResTable>(jobData.Mode);
            if (resData == null)
                return;
            if (!seatList.Contains(seat))
                seatList.Add(seat);
            InitPlayerData(i, resData.IconPath);
        }
    }

    public override void OnMove(int vx, int vy, int degree)
    {
        base.OnMove(vx, vy, degree);
        int index = GetSelectIndexByDegree(degree);
        if (index == lastSelectIndex)
            return;
        
        ChangeSeatSelect(index);
    }

    public override void OnRelease(int degree, bool hasDrag, float duration)
    {
        base.OnRelease(degree, hasDrag, duration);
        
        int seatIndex = GetSelectIndexByDegree(degree);
        if (m_Skill != null)
            InputManager.instance.CreateSkillFrameCommand(m_Skill.skillID, new SkillFrameCommand.SkillFrameData
            {
                type = SkillFrameCommand.SkillFrameDataType.Joystick_PlayerIndex, 
                data1 = (uint) seatList[seatIndex]
            });
            //InputManager.instance.CreateSkillFrameCommand(m_Skill.skillID, seatList[seatIndex] + 5);
    }

    public override void OnClear()
    {
        base.OnClear();
        seatList.Clear();
        lastSelectIndex = 0;
        selectSeatCommonBind = null;
        for(int i = 0; i < iconImageArr.Length; i++)
        {
            iconImageArr[i] = null;
        }
        
        for(int i = 0; i < selectTransArr.Length; i++)
        {
            selectTransArr[i] = null;
        }
        
        for(int i = 0; i < headArr.Length; i++)
        {
            headArr[i] = null;
        }
        
        for(int i = 0; i < grayArr.Length; i++)
        {
            grayArr[i] = null;
        }
        
        for(int i = 0; i < selectHightColor.Length; i++)
        {
            selectHightColor[i] = null;
        }
    }

    private void ChangeSeatSelect(int curIndex)
    {
        if (curIndex > 3)
            return;
        SetSingleData(lastSelectIndex, false);
        SetSingleData(curIndex, true);
        lastSelectIndex = curIndex;
    }

    
    private int GetSelectIndexByDegree(int degree)
    {
        int index = 0;
        int tmp = degree;
        if ((tmp >= 0 && tmp < 70) || (tmp >= 300 && tmp < 360))
            index = 2;
        else if (tmp >= 70 && tmp < 210)
            index = 1;
        else if (tmp >= 210 && tmp < 280)
            index = 0;
        index = CheckJoystickSelect(index);
        return index;
    }
    
    private int CheckJoystickSelect(int index)
    {
        if (seatList == null)
            return 0;
        if (index >= seatList.Count)
            return 0;
        BeActor actor = GetPlayerBySeat(seatList[index]);
        if (actor == null)
            return 0;
        if (actor != null && actor.IsDead())
            return 0;
        return index;
    }

    //根据座位号获取玩家信息
    private BeActor GetPlayerBySeat(int seat)
    {
        if (BattleMain.instance == null || BattleMain.instance.GetPlayerManager() == null)
            return null;
        BattlePlayer player = BattleMain.instance.GetPlayerManager().GetPlayerBySeat((byte)seat);
        if (player == null)
            return null;
        return player.playerActor;
    }
    
    private List<BattlePlayer> PlayerListSort(List<BattlePlayer> list,BeActor owner)
    {
        int index = list.FindIndex(x => { return x.playerActor == owner; });

        List<BattlePlayer> newList = new List<BattlePlayer>();
        newList.Add(list[index]);

        for (int i=0;i< list.Count; i++)
        {
            if (i != index)
                newList.Add(list[i]);
        }

        return newList;
    }
    
    //设置各个位置玩家的相关信息
    private void InitPlayerData(int seat, string iconPath)
    {
        if (seat > 3)
            return;
        if (iconImageArr[seat] != null)
        {
            ETCImageLoader.LoadSprite(ref iconImageArr[seat], iconPath);
        }
        headArr[seat].gameObject.CustomActive(true);
    }
    
    private ComCommonBind selectSeatCommonBind = null;
    private Image[] iconImageArr = new Image[3];    //头像
    private RectTransform[] selectTransArr = new RectTransform[3];  //选择
    private RectTransform[] headArr = new RectTransform[3];  //整个头部组件
    private UIGray[] grayArr = new UIGray[3];   //头像设置为灰色的脚本
    private Image[] selectHightColor = new Image[6];    //选择高亮框的颜色
    //d初始化摇杆UI数据
    private void InitSelectJoystickUI()
    {
        selectSeatCommonBind = m_JoystickGo.GetComponent<ComCommonBind>();

        iconImageArr[0] = selectSeatCommonBind.GetCom<Image>("PlayerOneIcon");
        iconImageArr[1] = selectSeatCommonBind.GetCom<Image>("PlayerTwoIcon");
        iconImageArr[2] = selectSeatCommonBind.GetCom<Image>("PlayerThreeIcon");

        selectTransArr[0] = selectSeatCommonBind.GetCom<RectTransform>("PlayerOneSelect");
        selectTransArr[1] = selectSeatCommonBind.GetCom<RectTransform>("PlayerTwoSelect");
        selectTransArr[2] = selectSeatCommonBind.GetCom<RectTransform>("PlayerThreeSelect");

        headArr[0] = selectSeatCommonBind.GetCom<RectTransform>("PlayerOneHead");
        headArr[1] = selectSeatCommonBind.GetCom<RectTransform>("PlayerTwoHead");
        headArr[2] = selectSeatCommonBind.GetCom<RectTransform>("PlayerThreeHead");

        grayArr[0] = selectSeatCommonBind.GetCom<UIGray>("IconGrayOne");
        grayArr[1] = selectSeatCommonBind.GetCom<UIGray>("IconGrayTwo");
        grayArr[2] = selectSeatCommonBind.GetCom<UIGray>("IconGrayThree");

        selectHightColor[0] = selectSeatCommonBind.GetCom<Image>("SelectHightOne1");
        selectHightColor[1] = selectSeatCommonBind.GetCom<Image>("SelectHightOne2");
        selectHightColor[2] = selectSeatCommonBind.GetCom<Image>("SelectHightTwo1");
        selectHightColor[3] = selectSeatCommonBind.GetCom<Image>("SelectHightTwo2");
        selectHightColor[4] = selectSeatCommonBind.GetCom<Image>("SelectHightThree1");
        selectHightColor[5] = selectSeatCommonBind.GetCom<Image>("SelectHightThree2");
    }
    
    
    private void SetSingleData(int index, bool isSelect)
    {
        if (index < 0 || index > 2)
            return;
        if (index >= seatList.Count)
            return;
        headArr[index].localScale = isSelect ? new Vector3(1.2f, 1.2f, 1.2f) : Vector3.one;
        selectTransArr[index].gameObject.CustomActive(isSelect);
        BeActor actor = GetPlayerBySeat(seatList[index]);
        grayArr[index].enabled = actor != null && actor.IsDead();

        int hightImageOne = 0;
        int hightImageTwo = 0;
        
        if (index == 0)
        {
            hightImageOne = 0;
            hightImageTwo = 1;
        }
        else if(index == 1)
        {
            hightImageOne = 2;
            hightImageTwo = 3;
        }
        else if (index == 2)
        {
            hightImageOne = 4;
            hightImageTwo = 5;
        }

        selectHightColor[hightImageOne].color = isSelect ? new Color(1, 1, 1, 0.5f) : new Color(0, 0, 0, 0.5f);
        selectHightColor[hightImageTwo].color = isSelect ? new Color(1, 1, 1, 0.5f) : new Color(0, 0, 0, 0.5f);
    }
}
#endif