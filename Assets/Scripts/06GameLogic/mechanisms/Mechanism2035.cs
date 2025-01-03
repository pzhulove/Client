using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mechanism2035 : BeMechanism {
    private string prefabPath = "UIFlatten/Prefabs/BattleUI/DungeonBar/SpecialHpBar";
#if !LOGIC_SERVER
    private GameObject barGo;
    private GameClient.ClientSystemBattle system;
#endif
    public Mechanism2035(int mid, int lv) : base(mid, lv) { }

    public override void OnInit()
    {
        base.OnInit();
        prefabPath = data.StringValueA[0];
    }

    public override void OnReset()
    {
        prefabPath = "UIFlatten/Prefabs/BattleUI/DungeonBar/SpecialHpBar";

#if !LOGIC_SERVER
        barGo = null;
        system = null;
#endif
    }

public override void OnStart()
    {
        base.OnStart();
#if !LOGIC_SERVER
        InitSystem();
        GameObject barRoot = _getHpBarRoot();

        if (null == barRoot)
        {
            return;
        }
        barGo = AssetLoader.instance.LoadResAsGameObject(prefabPath);
        if (null == barGo)
        {
            return;
        }
        Utility.AttachTo(barGo, barRoot);
        int maxHp = owner.GetEntityData().GetMaxHP();
        int hp = owner.GetEntityData().GetHP();
        IHPBar barCom = barGo.GetComponent<CBossHpBar>();
        if (null == barCom)
        {
            return;
        }
        barCom.Init(maxHp, 0,-1);

        barCom.SetName(owner.GetEntityData().name, owner.GetEntityData().level);
#endif
        SetPlayerHpBar(false);

        handleA = OwnerRegisterEventNew(BeEventType.onHit, args =>
        //handleA = owner.RegisterEvent(BeEventType.onHit, (args) =>
        {
#if !LOGIC_SERVER
            int changeHp = args.m_Int;
            if (null == barCom)
            {
                return;
            }
        //    barCom.Damage(maxHp - (hp + changeHp), false);

            barCom.Damage(changeHp, true);
#endif
        });

        handleB = owner.CurrentBeScene.RegisterEventNew(BeEventSceneType.OnEndAirBattle, (args) =>
        {
#if !LOGIC_SERVER
            SetPlayerHpBar(true);
            Unload();
#endif
        }); 

    }

    private void SetPlayerHpBar(bool value)
    {
#if !LOGIC_SERVER
        if (null == system)
        {
            return ;
        }
        system.PlayerSelfInfoRoot.SetActive(value);
#endif
    }

#if !LOGIC_SERVER
    private GameObject _getHpBarRoot()
    {       
        if (null == system)
        {
            return null;
        }      
        return system.MonsterBossRoot;
    }
#endif

    private void InitSystem()
    {
#if !LOGIC_SERVER
        system = GameClient.ClientSystemManager.GetInstance().TargetSystem as GameClient.ClientSystemBattle;

        if (null == system)
        {
            system = GameClient.ClientSystemManager.GetInstance().CurrentSystem as GameClient.ClientSystemBattle;
        }
#endif
    }

    public override void OnDead()
    {
        base.OnDead();
        Unload();
    }

    private void Unload()
    {
#if !LOGIC_SERVER
        if (barGo != null)
        {
            GameObject.Destroy(barGo);
            barGo = null;
        }
#endif
    }

    public override void OnFinish()
    {
        base.OnFinish();
        Unload();
    }

}
