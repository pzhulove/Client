using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using GameClient;

public class PlayerTittleComponent : MonoBehaviour
{
    static Vector3 ms_tittleScale = new Vector3(150.0f, 150.0f, 150.0f);
    #region Method
    public static void OnNotifyTittleChanged(UInt32 iPlayerID,Int32 iTittleID)
    {
        OnChangedTittle(iPlayerID,iTittleID);
    }

//     public static void OnNotifyPkPointsChanged(UInt32 iPlayerID, Int32 iPkPoints)
//     {
//         OnPkPointsChanged(iPlayerID, iPkPoints);
//     }

    public static void OnNotifyTownPlayerLvChanged(UInt32 iPlayerID,Int32 iLevel)
    {
        OnChangedLv(iPlayerID, iLevel);
    }
    #endregion

//     static void OnPkPointsChanged(UInt32 iPlayerID,Int32 iPkPoints)
//     {
//         if (ClientSystem.IsTargetSystem<ClientSystemTown>())
//         {
//             var current = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;
//             if (current == null)
//             {
//                 current = ClientSystemManager.GetInstance().TargetSystem as ClientSystemTown;
//             }
// 
//             if (current != null)
//             {
//                 Logger.LogProcessFormat("iPlayerID = {0} 的称号系统刷新了!",iPlayerID);
//                 current.OnNotifyTownPlayerPkPointsChanged(iPlayerID, iPkPoints);
//             }
//         }
//     }

    static void OnChangedLv(UInt32 iPlayerID, Int32 iLevel)
    {
        if (ClientSystem.IsTargetSystem<ClientSystemTown>())
        {
            var current = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;
            if (current == null)
            {
                current = ClientSystemManager.GetInstance().TargetSystem as ClientSystemTown;
            }

            if (current != null)
            {
                Logger.LogProcessFormat("iPlayerID = {0} 的称号系统刷新了!", iPlayerID);
                current.OnNotifyTownPlayerLvChanged(iPlayerID, iLevel);
            }
            return;
        }
    }

    static void OnChangedTittle(UInt32 iPlayerID, Int32 iTittle)
    {
        if (ClientSystem.IsTargetSystem<ClientSystemTown>())
        {
            var current = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;
            if (current == null)
            {
                current = ClientSystemManager.GetInstance().TargetSystem as ClientSystemTown;
            }

            if (current != null)
            {
                Logger.LogProcessFormat("iPlayerID = {0} 的称号系统刷新了!", iPlayerID);
                current.OnNotifyTownPlayerTittleChanged(iPlayerID,iTittle);
            }
        }
    }

    enum TittlePlayType:int
    {
        TPT_NOGANG_NOTITTLE = 0,
        TPT_NOGANG_TITTLE,
        TPT_GANG_NOTITTLE,
        TPT_GANG_TITTLE,
        TPT_VIP_NOGANG_NOTITTLE,
        TPT_VIP_NOGANG_TITTLE,
        TPT_VIP_GANG_NOTITTLE,
        TPT_VIP_GANG_TITTLE,
        TPT_COUNT,
        TPT_TITTLE_STATUS = 5,
    }

    enum PlayStatusType:int
    {
        PST_1 = 0,
        PST_2,
        PST_3,
        PST_4,
        PST_5,
        PST_6,
        PST_7,
        PST_COUNT,
    }

    GameObject[] goArray = new GameObject[(int)PlayStatusType.PST_COUNT];
    PlayStatusType ePlayStatusType = PlayStatusType.PST_1;
    Int32 iCurrentIndex = 0;
    float fNextCallTime = 0.0f;
    CPlayerInfo info;
    TittleStatus[] akTittleStatus = new TittleStatus[] 
    {
        new TittleStatusOne(),
        new TittleStatusTwo(),
        new TittleStatusThree(),
        new TittleStatusFour(),
        new TittleStatusFive(),
        new TittleStatusSix(),
        new TittleStatusSeven()
    };
    TittlePlayType eTittlePlayType = TittlePlayType.TPT_COUNT;
    List<Int32>[] akTurnList = new List<Int32>[(int)TittlePlayType.TPT_COUNT] 
    {
        new List<Int32>() { 1, 2, 5 },
        new List<Int32>() { 1, 2, 3 },
        new List<Int32>() { 2, 4, 5 },
        new List<Int32>() { 2, 3, 4 },
        new List<Int32>() { 1, 2, 7 },
        new List<Int32>() { 1, 2, 6 },
        new List<Int32>() { 2, 7, 4 },
        new List<Int32>() { 2, 6, 4 },
    };

    bool bHasTittle = false;
    bool bHasGang = false;
    bool bHasVip = false;
    Int32 iMark = 0;
    ProtoTable.ItemTable tittleItem = null;

    #region var
    Int32 iTittleID = 0;
    string name = "";
    string bangName = "[秋天童话&帮会]";
    Int32 iRoleLv = 0;
    Int32 iPkPoints = 0;
	PlayerInfoColor color; 
    #endregion

    class TittleStatus
    {
        public PlayerTittleComponent THIS;
        public GameObject parent;
        public GameObject self;
        TittleHelpComponent tittleHelpComponent;
        Animator animator;

        public virtual void SetUIData()
        {

        }

        public virtual float GetLastTime()
        {
            if (tittleHelpComponent != null)
            {
                return tittleHelpComponent.fPlayerTime;
            }

            return 5.0f;
        }

        public void SetAcive(bool bActive)
        {
            if (self != null && self.activeSelf != bActive)
            {
                self.gameObject.SetActive(bActive);
            }

            if (bActive)
            {
                if (animator != null)
                {
                    if (tittleHelpComponent != null)
                    {
                        int iLayerIndex = animator.GetLayerIndex(tittleHelpComponent.LayerName);
                        if (iLayerIndex != -1)
                        {
                            animator.Play(tittleHelpComponent.ClipName, iLayerIndex);
                        }
                    }
                }
            }
        }

        protected void setName(Text text)
        {
            if(text != null)
            {
                text.text = THIS.name;
                if (THIS.info != null) 
                {
					text.color = THIS.info.GetColor(THIS.color);

/*                    if(ClientSystem.IsTargetSystem<ClientSystemTown>())
                    {
                        text.color = THIS.info.GetColor(PlayerInfoColor.TOWN_PLAYER);
                    }
                    else if(ClientSystem.IsTargetSystem<ClientSystemBattle>())
                    {
                        text.color = THIS.info.GetColor(PlayerInfoColor.LEVEL_PLAYER);
                    }*/
                }
            }
        }
        protected void setGangName(Text text)
        {
            if (text != null)
            {
                text.text = THIS.bangName;
            }
        }

        protected void setLevel(Text text)
        {
            if (text != null)
            {
                text.text = string.Format("Lv.{0}", THIS.iRoleLv);
            }
        }

        protected void setAnimation(GameObject goAni,
        TittleAniSizeConvert sizeConvert,
        ref GameObject goTittle)
        {
            goTittle = null;
            if (goAni != null && sizeConvert != null)
            {
                THIS.tittleItem = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>(THIS.iTittleID);
                if (THIS.tittleItem != null)
                {
                    goTittle = AssetLoader.instance.LoadResAsGameObject(THIS.tittleItem.ModelPath, false);
                    if (goTittle != null)
                    {
                        for (int i = 0; i < goAni.transform.childCount; ++i)
                        {
                            var child = goAni.transform.GetChild(i);
                            if (child != null)
                            {
                                GameObject.Destroy(child.gameObject);
                            }
                        }

                        goTittle.name = "tittle";
                        Utility.AttachTo(goTittle, goAni);
                        tittleHelpComponent = goTittle.GetComponent<TittleHelpComponent>();
                        animator = goTittle.GetComponent<Animator>();

                        sizeConvert.ResetTarget(goTittle.transform,1.0f, 1.0f);
                    }
                }
            }
        }

        protected void setPkPoints(Image Grade, Int32 pkPoints)
        {
            if (Grade != null)
            {
                if (pkPoints >= 0)
                {
                    int RemainPoints = 0;
                    int TotalPoints = 0;
                    int pkIndex = 0;
                    bool bMaxLv = false;

                    string PkPath = Utility.GetPathByPkPoints((uint)pkPoints, ref RemainPoints, ref TotalPoints, ref pkIndex, ref bMaxLv);

                    if (PkPath != "" && PkPath != "-" && PkPath != "0")
                    {
                        // Grade.sprite = AssetLoader.instance.LoadRes(PkPath, typeof(Sprite)).obj as Sprite;
                        ETCImageLoader.LoadSprite(ref Grade, PkPath);
                        Grade.SetNativeSize();
                    }
                }
            }
        }
    }

    class TittleStatusOne : TittleStatus
    {
        public Text name;
        public override void SetUIData()
        {
            setName(name);
        }
    }

    class TittleStatusTwo : TittleStatus
    {
        public Image pkLv;
        public Text name;
        public override void SetUIData()
        {
            setName(name);
            setPkPoints(pkLv, THIS.iPkPoints);
        }
    }

    class TittleStatusThree : TittleStatus
    {
        //TITTLE
        public GameObject goAni;
        public TittleAniSizeConvert sizeConvert;
        GameObject goTittle;

        public Image icon;
        public Text lv;
        public Text name;

        public override void SetUIData()
        {
            setAnimation(goAni, sizeConvert,ref goTittle);
            setLevel(lv);
            setName(name);
            if(icon.gameObject.activeSelf)
            {
                icon.gameObject.SetActive(false);
            }
        }
    }

    class TittleStatusFour : TittleStatus
    {
        public Image icon;
        public Text lv;
        public Text name;
        public override void SetUIData()
        {
            setGangName(lv);
            setName(name);
        }
    }

    class TittleStatusFive : TittleStatus
    {
        public Text lv;
        public Text name;
        public override void SetUIData()
        {
            setLevel(lv);
            setName(name);
        }
    }

    class TittleStatusSix : TittleStatus
    {
        //TITTLE
        public GameObject goAni;
        public TittleAniSizeConvert sizeConvert;
        GameObject goTittle;

        public Text lv;
        public Text name;
        //VIP
        public override void SetUIData()
        {
            setAnimation(goAni, sizeConvert,ref goTittle);
            setLevel(lv);
            setName(name);
        }
    }

    class TittleStatusSeven : TittleStatus
    {
        public Text lv;
        public Text name;
        //VIP
        public override void SetUIData()
        {
            setLevel(lv);
            setName(name);
        }
    }

    void _Initialize()
    {
        System.Text.StringBuilder stringFormat = StringBuilderCache.Acquire();
        for (int i = 0; i < goArray.Length; ++i)
        {
            stringFormat.Clear();
            stringFormat.AppendFormat("Status{0}", i + 1);
            goArray[i] = Utility.FindChild(transform.gameObject, stringFormat.ToString());

            if (akTittleStatus[i] != null)
            {
                akTittleStatus[i].THIS = this;
                akTittleStatus[i].parent = transform.gameObject;
                akTittleStatus[i].self = Utility.FindChild(transform.gameObject, stringFormat.ToString());
            }
        }
        StringBuilderCache.Release(stringFormat);

        TittleStatusOne one = akTittleStatus[0] as TittleStatusOne;
        one.name = one.self.GetComponent<Text>();
        TittleStatusTwo two = akTittleStatus[1] as TittleStatusTwo;
        two.pkLv = Utility.FindComponent<Image>(two.self, "pkLv");
        two.name = Utility.FindComponent<Text>(two.self, "Name");
        TittleStatusThree three = akTittleStatus[2] as TittleStatusThree;
        three.goAni = Utility.FindChild(three.self, "Ani");
        three.sizeConvert = Utility.FindComponent<TittleAniSizeConvert>(three.self, "Ani");
        three.icon = Utility.FindComponent<Image>(three.self, "Icon");
        three.lv = Utility.FindComponent<Text>(three.self, "Status/Lv");
        three.name = Utility.FindComponent<Text>(three.self, "Status/Name");
        TittleStatusFour four = akTittleStatus[3] as TittleStatusFour;
        four.icon = Utility.FindComponent<Image>(four.self, "Icon");
        four.lv = Utility.FindComponent<Text>(four.self, "Status/GangName");
        four.name = Utility.FindComponent<Text>(four.self, "Status/Name");
        TittleStatusFive five = akTittleStatus[4] as TittleStatusFive;
        five.lv = Utility.FindComponent<Text>(five.self, "Lv");
        five.name = Utility.FindComponent<Text>(five.self, "Name");
        TittleStatusSix six = akTittleStatus[5] as TittleStatusSix;
        six.goAni = Utility.FindChild(six.self, "Ani");
        six.sizeConvert = Utility.FindComponent<TittleAniSizeConvert>(six.self, "Ani");
        six.lv = Utility.FindComponent<Text>(six.self, "Status/Lv");
        six.name = Utility.FindComponent<Text>(six.self, "Status/Name");
        TittleStatusSeven seven = akTittleStatus[6] as TittleStatusSeven;
        seven.lv = Utility.FindComponent<Text>(seven.self, "Lv");
        seven.name = Utility.FindComponent<Text>(seven.self, "Name");

        if (transform.parent != null)
        {
            info = transform.parent.GetComponent<CPlayerInfo>();
            //名字
            Text textComp = Utility.FindChild(transform.parent.gameObject, "Bottom/Name").GetComponent<Text>();
            if (textComp != null)
            {
                textComp.gameObject.SetActive(false);
            }

            // 等级
            Text Lv = Utility.FindChild(transform.parent.gameObject, "Bottom/Lv").GetComponent<Text>();
            if (Lv != null)
            {
                Lv.gameObject.SetActive(false);
            }
        }
    }

    public void Initialize(Int32 iTittleID,
    string name,
    string bangName,
    Int32 iRoleLv,
    Int32 iPkPoints = 0,
		PlayerInfoColor color=PlayerInfoColor.TOWN_PLAYER)
    {
        this.iTittleID = iTittleID;
        this.name = name;
        this.bangName = bangName;
        this.iRoleLv = iRoleLv;
        this.iPkPoints = iPkPoints;
		this.color = color;
        _Initialize();
        OnTittleTypeChanged();
    }

    public void SetLevel(Int32 iRoleLv)
    {
        this.iRoleLv = iRoleLv;
        OnTittleTypeChanged();
    }

    public void SetTittle(Int32 iTittleID)
    {
        this.iTittleID = iTittleID;
        OnTittleTypeChanged();
    }

    void OnSetData()
    {
        for (int i = 0; i < akTittleStatus.Length; ++i)
        {
            akTittleStatus[i].SetUIData();
        }
    }

    void OnSetTurn(PlayStatusType eCurrent)
    {
        if(eCurrent >= PlayStatusType.PST_1 && eCurrent < PlayStatusType.PST_COUNT)
        {
            for (int i = 0; i < akTittleStatus.Length; ++i)
            {
                if ((int)eCurrent == i)
                {
                    akTittleStatus[i].SetAcive(true);
                    float fInvokeTime = akTittleStatus[i].GetLastTime();
                    Invoke("UpdateTurn", fInvokeTime);
                }
                else
                {
                    akTittleStatus[i].SetAcive(false);
                }
            }
        }
    }

    TittlePlayType _GetTargetTittlePlayType()
    {
        bHasTittle = iTittleID != 0;
        bHasGang = false;
        bHasVip = false;
        iMark = 0;
        Int32 k = 0;

        iMark |= ((bHasTittle ? 1 : 0) << k); ++k;

        iMark |= ((bHasGang ? 1 : 0) << k); ++k;

        iMark |= ((bHasVip ? 1 : 0) << k); ++k;

        return (TittlePlayType)iMark;
    }

    public void OnTittleTypeChanged()
    {
        eTittlePlayType = _GetTargetTittlePlayType();
        OnSetData();
        OnResetTurn();
    }

    public void OnLevelChanged(Int32 iRoleLv)
    {
        this.iRoleLv = iRoleLv;
        OnTittleTypeChanged();
    }

    public void OnTittleChanged(Int32 iTittle)
    {
        this.iTittleID = iTittle;
        OnTittleTypeChanged();
    }

    public void OnPkPointsChanged(Int32 iPkPoints)
    {
        this.iPkPoints = iPkPoints;
        OnTittleTypeChanged();
    }

    void OnResetTurn()
    {
        iCurrentIndex = -1;
        UpdateTurn();
        CancelInvoke("UpdateTurn");
        Invoke("UpdateTurn",0.0f);
    }

    public void OnResetData()
    {
        OnSetData();
        OnResetTurn();
    }

    void UpdateTurn()
    {
        if(eTittlePlayType >= 0 && eTittlePlayType < TittlePlayType.TPT_COUNT)
        {
            iCurrentIndex = (iCurrentIndex + 1) % akTurnList[(int)eTittlePlayType].Count;
            ePlayStatusType = (PlayStatusType)(akTurnList[(int)eTittlePlayType][iCurrentIndex] - 1);
            OnSetTurn(ePlayStatusType);
        }
    }
}