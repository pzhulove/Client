using UnityEngine;
using System.Collections.Generic;

/*
 * 技能摇杆控制投射物落点
*/
public class Mechanism53 : BeMechanism
{
    protected int m_SkillId = 0;                                //对应的技能ID
    protected List<int> resIdList = new List<int>();            //影响的实体ID
    protected bool m_CannotInBlock;
    protected VInt m_TrailSpeed;

    protected IBeEventHandle m_AfterGenBulletHandler = null;    //监听投射物落下

    public Mechanism53(int mid, int lv) : base(mid, lv){}

    public override void OnReset()
    {
        resIdList.Clear();
        m_AfterGenBulletHandler = null;
        m_CannotInBlock = false;
        m_TrailSpeed = 0;
    }

    public override void OnInit()
    {
        m_SkillId = TableManager.GetValueFromUnionCell(data.ValueA[0],level);
        if (data.ValueB.Count > 0)
        {
            for(int i = 0; i < data.ValueB.Count; i++)
            {
                resIdList.Add(TableManager.GetValueFromUnionCell(data.ValueB[i], level));
            }
        }

        m_CannotInBlock = data.ValueC.Length > 0;
        if(data.ValueD.Length > 0)
            m_TrailSpeed = new VInt((float)TableManager.GetValueFromUnionCell(data.ValueD[0], level));;
    }

    public override void OnStart()
    {
        if (owner != null)
        {
            RemoveHandle();

            m_AfterGenBulletHandler = owner.RegisterEventNew(BeEventType.onAfterGenBullet, (args) =>
            {
                BeSkill skill = owner.GetSkill(m_SkillId);
                if (skill != null)
                {
                    BeProjectile projectile = args.m_Obj as BeProjectile;
                    if (projectile != null && resIdList.Contains(projectile.m_iResID))
                    {
                        skill.SetInnerState(BeSkill.InnerState.LAUNCH);

                        VInt3 projectPos = projectile.GetPosition();
                        VInt3 effectPos = skill.GetEffectPos();

                        if (owner.GetFace())
                            projectPos.x = effectPos.x;
                        else
                            projectPos.x = effectPos.x;
                        projectPos.y = effectPos.y;
                        
                        if (m_CannotInBlock && owner.CurrentBeScene.IsInBlockPlayer(projectPos))
                        {
                            var pos  = BeAIManager.FindStandPositionNew(projectPos, owner.CurrentBeScene, false, false, 50);
                            projectPos.x = pos.x;
                            projectPos.y = pos.y;
                        }
                        
                        if (m_TrailSpeed > 0)
                        {
                            projectPos.z = 0;
                            var dir = (projectPos - projectile.GetPosition()).NormalizeTo(m_TrailSpeed.i);
                            projectile.SetMoveSpeedX(dir.x);
                            projectile.SetMoveSpeedY(dir.y);
                            projectile.SetMoveSpeedZ(dir.z);
                            projectile.InitLocalRotation();
                            projectile.isAngleWithEffect = true;
                        }
                        else
                        {
                            projectile.SetPosition(projectPos);
                        }
                        projectile.SetFace(owner.GetFace(), true);
                    }
                }
            });
        }
    }

    public override void OnFinish()
    {
        RemoveHandle();
    }

    protected void RemoveHandle()
    {
        if (m_AfterGenBulletHandler != null)
        {
            m_AfterGenBulletHandler.Remove();
            m_AfterGenBulletHandler = null;
        }
    }
}
