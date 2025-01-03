/// <summary>
/// 药剂掌控（被动）
/// A:用于配置公共CD时间
/// </summary>
public class Skill2404 : BeSkill
{
    public Skill2404(int sid, int skillLevel) : base(sid, skillLevel)
    {
    }

    private int mCommonCDTime = 2000;

    public int CommonCdTime => mCommonCDTime;

    public override void OnInit()
    {
        base.OnInit();
        //if (skillData.ValueALength > 0)
        {
            mCommonCDTime = TableManager.GetValueFromUnionCell(skillData.ValueA[0], level);
        }
    }
    
}