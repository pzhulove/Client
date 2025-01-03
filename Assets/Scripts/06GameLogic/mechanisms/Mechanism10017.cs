using System.Collections;
using System.Collections.Generic;

//互斥光环机制，添加某Buff的时候，从目标身上移除一些指定Buff
public class Mechanism10017 : BeMechanism
{
    class TargetInfo
    {
        public BeActor target;
        public bool inRange;

        public TargetInfo(BeActor target)
        {
            this.target = target;
            inRange = false;
        }
    }

    int radius;                 //检测半径
    int interval;               //检测时间间隔
    int addBuffId;              //添加的BuffID
    int[] checkBuffIdArray;     //删除的BuffID

    int timer;

    List<TargetInfo> targetInfoList = new List<TargetInfo>();

    public Mechanism10017(int mid, int lv) : base(mid, lv) { }

    public override void OnInit()
    {
        radius = VInt.NewVInt(TableManager.GetValueFromUnionCell(data.ValueA[0], level), GlobalLogic.VALUE_1000).i;
        interval = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
        addBuffId = TableManager.GetValueFromUnionCell(data.ValueC[0], level);
        checkBuffIdArray = new int[data.ValueDLength];
        for (int i = 0; i < data.ValueDLength; i++)
        {
            checkBuffIdArray[i] = TableManager.GetValueFromUnionCell(data.ValueD[i], level);
        }
    }

    public override void OnStart()
    {
        targetInfoList.Clear();
        var players = owner.CurrentBeBattle.dungeonPlayerManager.GetAllPlayers();
        for (int i = 0; i < players.Count; i++)
        {
            var target = players[i].playerActor;
            var info = new TargetInfo(target);
            targetInfoList.Add(info);
        }

        timer = 0;
    }

    public override void OnUpdate(int deltaTime)
    {
        timer += deltaTime;
        if (timer >= interval)
        {
            timer = 0;

            for (int i = 0; i < targetInfoList.Count; i++)
            {
                if (targetInfoList[i].target == null)
                {
                    continue;
                }

                bool inRange = false;
                if (!targetInfoList[i].target.IsDead())
                {
                    inRange = (targetInfoList[i].target.GetPosition() - owner.GetPosition()).magnitude <= radius;
                }

                if (!targetInfoList[i].inRange && inRange)
                {
                    targetInfoList[i].target.buffController.TryAddBuff(addBuffId, -1);
                    for (int j = 0; j < checkBuffIdArray.Length; j++)
                    {
                        var buff = targetInfoList[i].target.buffController.HasBuffByID(checkBuffIdArray[j]);
                        targetInfoList[i].target.buffController.RemoveBuff(buff);
                    }
                }
                else if (targetInfoList[i].inRange && !inRange)
                {
                    var buff = targetInfoList[i].target.buffController.HasBuffByID(addBuffId);
                    targetInfoList[i].target.buffController.RemoveBuff(buff);

                    var m = targetInfoList[i].target.GetMechanismByIndex(GetMechanismIndex());
                    if (m != null)
                    {
                        if (m.attachBuffPID > 0)
                        {
                            targetInfoList[i].target.buffController.RemoveBuffByPID(m.attachBuffPID);
                        }
                        else
                        {
                            targetInfoList[i].target.RemoveMechanism(m);
                        }
                    }
                }

                targetInfoList[i].inRange = inRange;
            }
        }
    }

    public override void OnFinish()
    {
        for (int i = 0; i < targetInfoList.Count; i++)
        {
            if (targetInfoList[i].target != null && !targetInfoList[i].target.IsDead())
            {
                var buff = targetInfoList[i].target.buffController.HasBuffByID(addBuffId);
                targetInfoList[i].target.buffController.RemoveBuff(buff);
            }
        }
        targetInfoList.Clear();
    }
}
