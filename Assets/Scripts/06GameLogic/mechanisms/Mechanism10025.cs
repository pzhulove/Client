using System.Collections;
using System.Collections.Generic;

//监听双形态切换的时候，添加buff信息
public class Mechanism10025 : BeMechanism
{
    int[] openBuffInfoArray;
    int[] closeBuffInfoArray;

    public Mechanism10025(int mid, int lv) : base(mid, lv) { }

    public override void OnInit()
    {
        openBuffInfoArray = new int[data.ValueALength];
        for (int i = 0; i < data.ValueALength; i++)
        {
            openBuffInfoArray[i] = TableManager.GetValueFromUnionCell(data.ValueA[i], level);
        }
        closeBuffInfoArray = new int[data.ValueBLength];
        for (int i = 0; i < data.ValueBLength; i++)
        {
            closeBuffInfoArray[i] = TableManager.GetValueFromUnionCell(data.ValueB[i], level);
        }
    }

    public override void OnStart()
    {
        handleA = owner.RegisterEventNew(BeEventType.onOpen2ndState, args =>
        {
            for (int i = 0; i < openBuffInfoArray.Length; i++)
            {
                owner.buffController.TryAddBuff(openBuffInfoArray[i]);
            }
        });
        handleB = owner.RegisterEventNew(BeEventType.onClose2ndState, args =>
        {
            for (int i = 0; i < closeBuffInfoArray.Length; i++)
            {
                owner.buffController.TryAddBuff(closeBuffInfoArray[i]);
            }
        });
    }
}
