using GameClient;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DungeonInfoFrame : ClientFrame
{
    private Button btnClick;
    private BaseBattle battle;

    private Transform imageContainer;

    private GameObject curShowImage;
    private GameObject singleImage;

    private Button closeBtn;
    private Button knownBtn;

    private Button nextPageBtn;
    public override string GetPrefabPath()
    {
        return "UIFlatten/Prefabs/BattleUI/DungeonInfoFrame";
    }

    protected override void _bindExUI()
    {
        base._bindExUI();
        btnClick = mBind.GetCom<Button>("btnClick");
        closeBtn = mBind.GetCom<Button>("closeBtn");
        imageContainer = mBind.GetCom<Transform>("imageContainer");
        singleImage = mBind.GetGameObject("singleImage");
        knownBtn = mBind.GetCom<Button>("knowBtn");
        nextPageBtn = mBind.GetCom<Button>("nextPageBtn");
    }
    protected override void _OnOpenFrame()
    {

        base._OnOpenFrame();
        closeBtn.onClick.RemoveAllListeners();
        closeBtn.onClick.AddListener(() =>
        {
            Close();
        });

        knownBtn.onClick.RemoveAllListeners();
        knownBtn.onClick.AddListener(() =>
        {
            Close();
        });

        if (userData != null)
        {
            PauseFight();
            int roomID = (int)userData;
            if (roomID == 23101)
            {
                nextPageBtn.gameObject.CustomActive(false);
                btnClick.gameObject.CustomActive(false);
                singleImage.CustomActive(true);
                knownBtn.gameObject.CustomActive(true);
                imageContainer.gameObject.CustomActive(false);
            }
            else if (roomID == 23131)
            {
                nextPageBtn.gameObject.CustomActive(true);
                closeBtn.gameObject.CustomActive(false);
                knownBtn.gameObject.CustomActive(false);
                btnClick.gameObject.CustomActive(true);
                singleImage.CustomActive(false);
                ShowImage();
            }
        }
    }

    private void ShowImage()
    {
        curShowImage = imageContainer.GetChild(0).gameObject;
        curShowImage.CustomActive(true);
        btnClick.onClick.RemoveAllListeners();
        btnClick.onClick.AddListener(() =>
        {
            ShowNextPage();
        });
        nextPageBtn.onClick.RemoveAllListeners();
        nextPageBtn.onClick.AddListener(() =>
        {
            ShowNextPage();
        });
    }

    private void ShowNextPage()
    {
        int index = 0;
        curShowImage.CustomActive(false);
        index++;
        if (index >= imageContainer.childCount)
        {
            Close();
        }
        else
        {
            if (index == imageContainer.childCount - 1)
            {
                closeBtn.gameObject.CustomActive(true);
                btnClick.gameObject.CustomActive(false);
                nextPageBtn.gameObject.CustomActive(false);
                knownBtn.gameObject.CustomActive(true);
            }
            curShowImage = imageContainer.GetChild(index).gameObject;
            curShowImage.CustomActive(true);
        }
    }

    private void PauseFight()
    {
        battle = BattleMain.instance.GetBattle() as BaseBattle;
        if (battle != null && battle.dungeonManager!=null)
        {
            battle.dungeonManager.PauseFight();

        }
    }

    protected override void _OnCloseFrame()
    {
        base._OnCloseFrame();
        if (battle != null && battle.dungeonManager != null)
        {
            battle.dungeonManager.ResumeFight();
        }
    }

    protected override void _unbindExUI()
    {
        base._unbindExUI();
    }

}
