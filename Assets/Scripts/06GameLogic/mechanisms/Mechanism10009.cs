using GameClient;
using System.Collections;
using System.Collections.Generic;

//宝箱怪状态改变机制
public class Mechanism10009 : BeMechanism
{
    public Mechanism10009(int mid, int lv) : base(mid, lv) { }
    
    private VFactor[] _changeStateHp;
    private int[] _changeActionsType;
    private Dictionary<int, string[]> _changeActionsName;
    private string[] _transformActions;
    private int _curState;
    private bool _stateTransforming;
    private bool _waitToDead;

    private readonly int _batiBuff = 521668;

    public override void OnInit()
    {
        
    }

    public override void OnStart()
    {
        _curState = 0;
        _waitToDead = false;
        _stateTransforming = false;
        
        _changeStateHp = new VFactor[data.ValueALength];
        _transformActions = new string[data.ValueALength];
        _changeActionsType = new int[data.ValueBLength];

        for (int i = 0; i < data.ValueALength; i++)
        {
            _changeStateHp[i] = new VFactor(TableManager.GetValueFromUnionCell(data.ValueA[i], level), GlobalLogic.VALUE_1000);
            _transformActions[i] = string.Format(data.StringValueA[0], i);
        }

        _changeActionsName = new Dictionary<int, string[]>(data.ValueBLength);
        for (int i = 0; i < data.ValueBLength; i++)
        {
            int changeActionType = TableManager.GetValueFromUnionCell(data.ValueB[i], level);
            _changeActionsType[i] = changeActionType;
            string orignalName = owner.GetActionNameByType((ActionType) changeActionType);
            string[] changeNames = new string[data.ValueALength];
            for (int j = 0; j < data.ValueALength; j++)
            {
                changeNames[j] = string.Format("{0}_{1}", orignalName, j + 1);
            }

            _changeActionsName.Add(changeActionType, changeNames);
        }
        
        handleA = owner.RegisterEventNew(BeEventType.onDead, param =>
        {
            if (_curState < _changeStateHp.Length)
            {
                param.m_Bool = false;
                _waitToDead = true;
            }
        });
    }

    public override void OnUpdate(int deltaTime)
    {
        if (_curState >= _changeStateHp.Length || _stateTransforming)
            return;
        int nextStateHp = owner.attribute.GetMaxHP() * _changeStateHp[_curState];
        if (owner.attribute.GetHP() < nextStateHp)
        {
            //enter state
            foreach (var pair in _changeActionsName)
            {
                owner.ReplaceActionName((ActionType) pair.Key, pair.Value[_curState]);
            }
            
            string transformAction = _transformActions[_curState];
            owner.PlayAction(transformAction);
            _stateTransforming = true;
            int actionDuration = owner.GetCurrentActionDuration();
            owner.buffController.TryAddBuff(_batiBuff, actionDuration);
            owner.delayCaller.DelayCall(actionDuration, () =>
            {
                //finish state
                _stateTransforming = false;
                _curState++;
                if (_curState >= _changeStateHp.Length && _waitToDead)
                {
                    _waitToDead = false;
                    owner.DoDead();
                }
            });
        }
    }
    

    public override void OnFinish()
    {
        
    }
}
