using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using System;
using GameClient;

public interface IHPBar
{
    /// <summary>
    /// 初始化血条
    /// </summary>
    /// <param name="hp"></param>
    /// <param name="mp"></param>
    /// <param name="maxHp">单条血量最大值，-1表示单条</param>
    void Init(int hp, int mp, int maxHp = -1, int resistVale = 0);

    void SetName(string name, int level);

    void SetName(string name);

    void SetLevel(int level);

    void SetIcon(Sprite headIcon, Material material);

	void Damage(int value, bool withAnimate);


    // feat need to add
	//void SetHPRate(float percent);

	void SetMPRate(float percent);

    void SetActive(bool active);

    void Unload();

    void SetHidden(bool hidden);

    bool GetHidden();

    eHpBarType GetBarType();

	void SetHP(int curHP, int maxHP);
	void SetMP(int curMP, int maxMP);
    void InitResistMagic(int resistValue,BeActor player);
    void SetBuffName(string text);
}

public enum eHpBarType
{
    Boss,
    Monster,
    Elite,

    Player,
	player_Monster
}

public class HpBarNode
{
    public int          id            { get; private set; }
    public int          changedHp     { get; private set; }

    public int          hp            { get; private set; }
    public int          maxHp         { get; private set; }
    public int          singleBarHp   { get; private set; }
    public eHpBarType   barType       { get; private set; }

    public string       name          { get; private set; }
    public int          level         { get; private set; }
    public Sprite       headIcon      { get; private set; }
    public Material     headIconMaterial { get; private set; }

    private VFactor mSingleBarHpFactor = VFactor.one;
	private int 	    originSingleBarHp { get; set; }
    private int         originMaxHp;


    public void InitHpData(int id, eHpBarType barType, int maxHp, int singleBarHp)
    {
		this.id 		 = id;
        this.barType     = barType;
        this.hp          = maxHp;
        this.maxHp       = maxHp;

        this.singleBarHp = singleBarHp;
		this.originSingleBarHp = this.singleBarHp;
        this.changedHp   = 0;
        this.originMaxHp = maxHp;

        Logger.LogProcessFormat("[HpBarNode] 设置数据 ID:{0}, 类型:{1}, 最大值:{2}, 单条值:{3}", this.id, this.barType, this.maxHp, this.singleBarHp);
    }

    public void InitInfo(string name, int level, Sprite headIcon, Material headIconmaterial)
    {
        this.name = name;
        this.level = level;
        this.headIcon = headIcon;
        this.headIconMaterial = headIconmaterial;

        Logger.LogProcessFormat("[HpBarNode] 设置信息 名字:{0}, 等级:{1}", this.name, this.level);
    }

	public void SyncHPBar(int hp, int maxHp)
    {
		//this.changedHp += (this.hp - hp);
        //
        if (maxHp <= 0)
        {
            return ;
        }

        mSingleBarHpFactor = new VFactor(maxHp, this.originMaxHp);
        this.singleBarHp = (mSingleBarHpFactor * ((long)this.originSingleBarHp)).roundInt;

        /*
		if (VFactor.one == mSingleBarHpFactor)
		{
			mSingleBarHpFactor.nom = maxHp;
			mSingleBarHpFactor.den = this.maxHp;

			this.singleBarHp       = (mSingleBarHpFactor * ((long)this.originSingleBarHp)).roundInt;
		}
		else
		{
            if (mSingleBarHpFactor.nom % this.maxHp == 0)
            {
                mSingleBarHpFactor.nom = 1;
            }
            else
            {
                mSingleBarHpFactor = mSingleBarHpFactor / (long)this.maxHp;
            }

            if (mSingleBarHpFactor.den % maxHp == 0)
            {
                mSingleBarHpFactor.den = 1;
            }
            else
            {
			    mSingleBarHpFactor = mSingleBarHpFactor * (long)maxHp;
            }

			this.singleBarHp   = (mSingleBarHpFactor * ((long)this.originSingleBarHp)).roundInt;
		}

        if (this.singleBarHp <= 0)
        {
			this.singleBarHp = this.originSingleBarHp;
        }*/

        this.maxHp = maxHp;
        this.hp    = hp;

     //   Logger.LogErrorFormat("[HpBarNode] 同步血量 名字:{0}, {1}/{2} num:{3} single:{4}", this.name, this.hp, this.maxHp, this.hp/(float)singleBarHp, this.singleBarHp);
    }

    public void Damage(int changedHp)
    {
        this.changedHp += changedHp;
        this.hp        -= changedHp;

        Logger.LogProcessFormat("[HpBarNode] 伤害 ID:{0}, 名字:{1}, 类型:{2}, 等级:{3}, HP:{4}, HPChange:{5}",
                this.id, this.name, this.barType, this.level, this.hp, this.changedHp);
    }

    public bool IsHpChanged()
    {
        return changedHp != 0;
    }

    public void ClearChangedHp()
    {
        changedHp = 0;
    }

    public void ResetHp()
    {
        hp = maxHp;
        changedHp = 0;
    }

    public void Reset()
    {
        id = 0;
        changedHp = 0;
        hp = 0;
        maxHp = 0;
        singleBarHp = 0;
		originSingleBarHp = 0;
        barType = eHpBarType.Monster;
        name = string.Empty;
        level = 0;
        headIcon = null;
        headIconMaterial = null;

        Logger.LogProcessFormat("[HpBarNode] 重置 ID:{0}, 名字:{1}, 类型:{2}, 等级:{3}, HP:{4}, HPChange:{5}",
                this.id, this.name, this.barType, this.level, this.hp, this.changedHp);
    }
}

public class HPBarManager
{
    private int mStartHpBarIdOrigin = 1;

    public const int kInvalidHpBarId    = -1;

    private List<HpBarNode> mHpBarNodes = new List<HpBarNode>();
    private List<int> mRemovedHPBarIds = new List<int>();

    private int mLastHpBarId    = kInvalidHpBarId;
    private int mCurrentHpBarId = kInvalidHpBarId;

    protected int currentHpBarId 
    {
        get 
        {
            return mCurrentHpBarId;
        }

        set 
        {
            mCurrentHpBarId = value;
        }
    }

    protected int lastHpBarId 
    {
        get 
        {
            return mLastHpBarId;
        }

        set 
        {
            mLastHpBarId = value;
        }
    }
    private eHpBarType mLastBarType = eHpBarType.Monster;

    public void AddHpBar(IHPBar bar, bool bActive = false)
    {
        if (null != bar)
        {
            bar.SetActive(bActive);
        }
    }

	public void SyncHPBar(int barId, int hp, int maxHp)
    {
        HpBarNode barNode = _getHpBarNodeByID(barId);

        if (null == barNode)
        {
            return;
        }

		barNode.SyncHPBar(hp, maxHp);

        if (hp <= 0)
        {
            mRemovedHPBarIds.Add(barId);
        }
    }

    public void ResetHpBar(int barId)
    {
        HpBarNode barNode = _getHpBarNodeByID(barId);

        if (null == barNode)
        {
            return;
        }

        barNode.ResetHp();
    }

    public int AddHpBar(BeEntity entity, eHpBarType type, string name, int singleBarHp, Sprite headIcon, Material headIconMaterial)
    {
        if (null == entity)
        {
            return kInvalidHpBarId;
        }

        BeEntityData entityData = entity.GetEntityData();

        if (null == entityData)
        {
            return kInvalidHpBarId;
        }

        int barId = _getNewBarId();

        HpBarNode node = _createHpBarNodeFromPool();

        node.InitHpData(barId, type, entityData.GetMaxHP(), singleBarHp);
        node.InitInfo(name, entityData.GetLevel(), headIcon, headIconMaterial);

        Logger.LogProcessFormat("[HpBarManager] 创建血条数据 ID:{0} 类型:{1}, 名字:{2}, 等级:{3}, 血量:{4}, 单条最大:{5}", 
                node.id, node.barType, node.name, node.level, node.maxHp, node.singleBarHp);

        mHpBarNodes.Add(node);

        _addHpBarGameObject(type);

        return barId;
    }

    private int _getNewBarId()
    {
        return ++mStartHpBarIdOrigin;
    }

    public void RemoveHPBar(int barId)
    {
        mRemovedHPBarIds.Add(barId);
    }
    private void _realRemoveHpBar(int barId)
    {
        HpBarNode barNode = _getHpBarNodeByID(barId);

        if (null == barNode)
        {
            return ;
        }
      
        mHpBarNodes.Remove(barNode);

        IHPBar bar = _getHpBarGameObject(barNode.barType);
        if (bar != null)
        {
            bar.Unload();
        }

        _destroyHpBarNodeToPool(barNode);
    }


    public void RemoveHPBar(IHPBar bar)
    {
    }

    public void ShowHPBar(int barId, int value, HitTextType type = HitTextType.NORMAL)
    {
        HpBarNode barNode = _getHpBarNodeByID(barId);

        if (null == barNode)
        {
            return ;
        }

        barNode.Damage(value);

        if (_isShowNextHpBar(type))
        {
            if (null != _getHpBarNodeByID(barId))
            {
                currentHpBarId = barId;
            }
        }
    }

    public void ShowHPBar(IHPBar bar, int value, HitTextType type = HitTextType.NORMAL)
    {
        if (null != bar)
        {
            bar.Damage(value, true);
        }
    }

    private bool _isShowNextHpBar(HitTextType type)
    {
        return type == HitTextType.NORMAL 
            || type == HitTextType.CRITICAL;
    }

    private bool _needChangeHpBarGameObjectType()
    {
        HpBarNode lastNode = _getHpBarNodeByID(lastHpBarId);
        HpBarNode curNode = null;
        if (null == lastNode)
        {
            curNode = _getHpBarNodeByID(currentHpBarId);
            if(curNode == null)
            {
                return false;
            }
            return curNode.barType != mLastBarType;
        }

        curNode = _getHpBarNodeByID(currentHpBarId);

        if (null == curNode)
        {
            return false;
        }

        return curNode.barType != lastNode.barType;
    }
    private HpBarNode _getHpBarNodeFromPool(int id)
    {
        for(int i = 0; i < mHpBarNodesPool.Count;i++)
        {
            if(mHpBarNodesPool[i].id == id)
            {
                return mHpBarNodesPool[i];
            }
        }
        return null;
    }

    private void _hiddenLastHpBarGameObject()
    {
        HpBarNode lastNode = _getHpBarNodeByID(lastHpBarId);
        if (null != lastNode)
        {
            IHPBar bar = _getHpBarGameObject(lastNode.barType);
            if(bar != null)
                bar.SetActive(false);
        }
        else
        {
            IHPBar bar = _getHpBarGameObject(mLastBarType);
            if(bar != null)
            {
                bar.SetActive(false);
            }
        }
    }

    private void _showCurrentHpBarGameObject()
    {
        HpBarNode node = _getHpBarNodeByID(currentHpBarId);

        if (null != node && node.IsHpChanged())
        {
            IHPBar bar = _getHpBarGameObject(node.barType);
            if (bar != null)
            {
                bar.SetActive(true);

                bar.Init(node.maxHp, 0, node.singleBarHp);

                bar.SetName(node.name, node.level);
                bar.SetIcon(node.headIcon, node.headIconMaterial);

                bar.Damage(node.maxHp - (node.hp + node.changedHp), false);

                bar.Damage(node.changedHp, true);
            }
            mLastBarType = node.barType;
            lastHpBarId = currentHpBarId;

            Logger.LogProcessFormat("[HpBarManager] 显示血条 ID:{0}", node.id);
        }
    }


    public void Update(int delta)
    {
        bool hasHpChanged = false;

        for (int i = 0; i < mHpBarNodes.Count; ++i)
        {
            if (mHpBarNodes[i].IsHpChanged())
            {
                hasHpChanged = true;
                break;
            }
        }

        if (hasHpChanged)
        {
            if (kInvalidHpBarId != currentHpBarId)
            {
                if (_needChangeHpBarGameObjectType())
                {
                    _hiddenLastHpBarGameObject();
                }
                
                _showCurrentHpBarGameObject();
            }
        }

        for (int i = 0; i < mHpBarNodes.Count; ++i)
        {
            mHpBarNodes[i].ClearChangedHp();
        }

        for (int i = 0; i < mRemovedHPBarIds.Count; ++i)
        {
            _realRemoveHpBar(mRemovedHPBarIds[i]);
        }
        mRemovedHPBarIds.Clear();

    }

    public void ShowMPBar(IHPBar bar, float percent)
	{
        if (null == bar)
        {
            return ;
        }

		bar.SetMPRate(percent);
	}

    public void Unload()
    {
        _unloadAllHpBarGameObject();
    }

#region HpBarGameObect Cache
    private List<IHPBar> mCacheHPBar = new List<IHPBar>();

    protected void _unloadAllHpBarGameObject()
    {
        for (int i = 0; i < mCacheHPBar.Count; ++i)
        {
            mCacheHPBar[i] = null;
        }
        mCacheHPBar.Clear();
    }

    private void _addHpBarGameObject(eHpBarType type)
    {
        if (_hasCreateHpBarGameobject(type))
        {
            return ;
        }

        IHPBar bar = _createHpBarAndAttach(type);

        if (null == bar)
        {
            return;
        }

        mCacheHPBar.Add(bar);
    }

    private bool _hasCreateHpBarGameobject(eHpBarType type)
    {
        return null != _getHpBarGameObject(type);
    }

    private IHPBar _getHpBarGameObject(eHpBarType type)
    {
        for (int i = 0; i < mCacheHPBar.Count; ++i)
        {
            if (null != mCacheHPBar[i] && type == mCacheHPBar[i].GetBarType()) 
            {
                return mCacheHPBar[i];
            }
        }

        return null;
    }

    private bool _isHpBarNodeIsHpChanged(int barId)
    {
        HpBarNode node = _getHpBarNodeByID(barId);

        if (null == node)
        {
            return false;
        }

        return node.IsHpChanged();
    }
    public void ShowBuffName(int id,string name)
    {
        HpBarNode node = _getHpBarNodeByID(id);
        if (null == node)
        {
            return ;
        }
        IHPBar bar = _getHpBarGameObject(node.barType);
        if (bar != null)
            bar.SetBuffName(name);
    }


    private HpBarNode _getHpBarNodeByID(int barId)
    {
        for (int i = 0; i < mHpBarNodes.Count; ++i)
        {
            if (barId == mHpBarNodes[i].id)
            {
                return mHpBarNodes[i];
            }
        }

        return null;
    }

    private IHPBar _createHpBarAndAttach(eHpBarType type)
    {
        GameObject barRoot = _getHpBarRoot();

        if (null == barRoot)
        {
            return null;
        }

        string     barPath = _getHpBarPathByType(type);
        GameObject barGo   = AssetLoader.instance.LoadResAsGameObject(barPath);

        Utility.AttachTo(barGo, barRoot);

        IHPBar barCom  = barGo.GetComponent<CBossHpBar>();

        barCom.SetActive(false);

        return barCom;
    }

    private GameObject _getHpBarRoot()
    {
        GameClient.ClientSystemBattle system = GameClient.ClientSystemManager.GetInstance().TargetSystem as GameClient.ClientSystemBattle;

        if (null == system)
        {
            system = GameClient.ClientSystemManager.GetInstance().CurrentSystem as GameClient.ClientSystemBattle;
        }

        if (null == system)
        {
            return null;
        }

        return system.MonsterBossRoot;
    }

    private string _getHpBarPathByType(eHpBarType type)
    {
        switch (type)
        {
            case eHpBarType.Monster:
                return "UIFlatten/Prefabs/BattleUI/DungeonBar/HPBar_Monster";
            case eHpBarType.Elite:
                return "UIFlatten/Prefabs/BattleUI/DungeonBar/HPBar_Elite";
            case eHpBarType.Boss:
                return "UIFlatten/Prefabs/BattleUI/DungeonBar/HPBar_BOSS_NEW";
        }

        return "UIFlatten/Prefabs/BattleUI/DungeonBar/HPBar_Monster";
    }
#endregion

#region NodesPool
    protected List<HpBarNode> mHpBarNodesPool = new List<HpBarNode>();

    protected const int kDefaultBarNodeCount = 8;

    protected void _destroyHpBarNodeToPool(HpBarNode node)
    {
        if (null == node)
        {
            return ;
        }

        node.Reset();
        mHpBarNodesPool.Add(node);
    }
    
    protected HpBarNode _createHpBarNodeFromPool()
    {
        if (mHpBarNodesPool.Count <= 0)
        {
            _addDefaultCountBarNodes2Pool();
        }

        HpBarNode node = mHpBarNodesPool[0];

        node.Reset();

        mHpBarNodesPool.Remove(node);

        return node;
    }

    protected void _addDefaultCountBarNodes2Pool()
    {
        for (int i = 0; i < kDefaultBarNodeCount; ++i)
        {
            _addOneBarNodeToPool();
        }
    }

    protected void _addOneBarNodeToPool()
    {
        HpBarNode node = new HpBarNode();
        node.Reset();
        mHpBarNodesPool.Add(node);
    }
#endregion
}
