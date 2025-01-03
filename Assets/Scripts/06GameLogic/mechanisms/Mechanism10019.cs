

//积蓄条机制 受到指定怪物攻击后附加buff 并积蓄进度条 进度条随时间退却 积满之后造成最大生命值比例伤害

public class Mechanism10019 : BeMechanism
{
    private int[] _targetMonsterId;
    private int _addBuffIdOnHurt;
    private int _criticalCount;
    private int _fadeTime;
    private VFactor _damageRate;

    private int _curCount;
    private int _fadeTimer;
#if !LOGIC_SERVER
    private UnityEngine.UI.Text _counter;
#endif
    public Mechanism10019(int mid, int lv) : base(mid, lv) { }

    public override void OnInit()
    {
        _targetMonsterId = new int[data.ValueA.Length];
        for (int i = 0; i < data.ValueA.Length; i++)
        {
            _targetMonsterId[i] = TableManager.GetValueFromUnionCell(data.ValueA[i], level);
        }
        _addBuffIdOnHurt = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
        _criticalCount = TableManager.GetValueFromUnionCell(data.ValueC[0], level);
        _fadeTime = TableManager.GetValueFromUnionCell(data.ValueD[0], level);
        _damageRate = VFactor.NewVFactor(TableManager.GetValueFromUnionCell(data.ValueE[0], level), GlobalLogic.VALUE_1000);
    }

    public override void OnStart()
    {
        _fadeTimer = 0;
        _curCount = 0;
        owner.RegisterEventNew(BeEventType.onHurt, (param) =>
        {
            var attacker = param.m_Obj2 as BeActor;
            if (attacker == null)
                return;
            
            if (!_isTargetMonster(attacker))
                return;
            
            owner.buffController.TryAddBuff(_addBuffIdOnHurt);
            _fadeTimer = 0;
            _curCount++;
            if (_curCount >= _criticalCount)
            {
                _curCount = 0;
                int hurtValue = owner.attribute.GetMaxHP() * _damageRate;
                owner.DoHurt(hurtValue);
            }
            RefreshUI();
        });
    }

    private bool _isTargetMonster(BeActor attacker)
    {
        for (int i = 0; i < _targetMonsterId.Length; i++)
        {
            if (attacker.GetEntityData().MonsterIDEqual(_targetMonsterId[i]))
            {
                return true;
            }
        }

        return false;
    }

    private void RefreshUI()
    {
#if !LOGIC_SERVER
        if (_counter == null)
        {
            var go = AssetLoader.instance.LoadResAsGameObject("UIFlatten/Prefabs/Battle/Hell/HeadCounter");
            _counter = go.GetComponent<UnityEngine.UI.Text>();
            Utility.AttachTo(go, owner.m_pkGeActor.goInfoBar);
        }

        if (_curCount > 0)
        {
            _counter.CustomActive(true);//may use canvas group
            _counter.text = _curCount.ToString();
        }
        else
        {
            _counter.CustomActive(false);
        }
#endif
    }

    public override void OnUpdate(int deltaTime)
    {
        if (_curCount <= 0)
            return;
        
        _fadeTimer += deltaTime;
        if (_fadeTimer >= _fadeTime)
        {
            _fadeTimer = 0;
            _curCount--;
            RefreshUI();
        }
    }

    public override void OnFinish()
    {
#if !LOGIC_SERVER
        if (_counter != null)
        {
            UnityEngine.GameObject.Destroy(_counter);
        }
#endif
    }
}
