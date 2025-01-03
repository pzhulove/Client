using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameClient;
using ProtoTable;
using UnityEngine.UI;
public class InstituteFinishFrame : ClientFrame
{
    GameObject awardContainer;
    GameObject hassPass;
    Button nextBtn;
    Button returnBtn;
    private InstituteTable nextData;
    private InstituteBattleFrame batleFrame;
    private Text title;
    public override string GetPrefabPath()
    {
        return "UIFlatten/Prefabs/Battle/Finish/InstituteFinishFrame";
    }

    protected override void _bindExUI()
    {
        base._bindExUI();
        awardContainer = mBind.GetGameObject("awardContainer");
        nextBtn = mBind.GetCom<Button>("nextBtn");
        returnBtn = mBind.GetCom<Button>("returnBtn");
        title = mBind.GetCom<Text>("title");
        hassPass = mBind.GetGameObject("haveAwarded");
    }

    protected override void _OnOpenFrame()
    {
        base._OnOpenFrame();
        List<InstituteTable> list = TableManager.instance.GetJobInstituteData(BattleMain.instance.GetLocalPlayer().playerInfo.occupation);
        InstituteTable curData = list.Find(x => { return x.ID == InstituteFrame.id; });

        InstituteFrame.id++;
       
        nextData = list.Find(x => { return x.ID == InstituteFrame.id; });
        nextBtn.gameObject.CustomActive(nextData != null && nextData.LevelLimit <= PlayerBaseData.GetInstance().Level);
        ShowAwardInfo(curData);
        title.text = curData.Title;
        nextBtn.onClick.AddListener(ChallengeNext);
        returnBtn.onClick.AddListener(ReturnTown);

        batleFrame = ClientSystemManager.GetInstance().GetFrame(typeof(InstituteBattleFrame)) as InstituteBattleFrame;
        hassPass.CustomActive(batleFrame.hasPassed);
    }

    private void ShowAwardInfo(InstituteTable data)
    {
        MissionTable missionTableData = TableManager.GetInstance().GetTableItem<MissionTable>(data.MissionID);
        if (missionTableData != null)
        {
            string[] awards = missionTableData.Award.Split(new char[] { ',' });

            for (int i = 0; i < awards.Length; i++)
            {
                var award = awards[i].Split(new char[] { '_' });
                if (award.Length == 2)
                {
                    ItemData itemdata = ItemDataManager.CreateItemDataFromTable(int.Parse(award[0]));
                    if (itemdata == null)
                    {
                        continue;
                    }

                    itemdata.Count = int.Parse(award[1]);

                    ComItem ShowItem = CreateComItem(awardContainer);
                    ShowItem.Setup(itemdata, (var1, var2) =>
                    {
                        ItemTipManager.GetInstance().ShowTip(var2);
                    });
                    if (hassPass)
                    {
                        ShowItem.ItemData.IsSelected = true;
                        ShowItem.SetShowSelectState(true);
                    }

                }
            }

        }
    }


    void ReturnTown()
    {
        ClientSystemManager.instance.SwitchSystem<ClientSystemTown>();
    }

    void ChallengeNext()
    {
        if (nextData != null)
        {
            GameFrameWork.instance.StartCoroutine(InstituteFrame._commonStart(nextData.DungeonID,1));
        }
    }
}
