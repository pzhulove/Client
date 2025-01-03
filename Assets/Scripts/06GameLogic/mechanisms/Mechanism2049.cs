using System;
using System.Collections.Generic;
using GameClient;

public class Mechanism2049 : BeMechanism
{
    private IBeEventHandle mBulletEvent; //监听子弹
    private IBeEventHandle mBulletHitEvent; //监听子弹碰撞事件

    private BeEntity mFirstTarget;
    private VInt3 mTargetPos;

    int CREAT_NEXT_INTERVAL = 200;
    int BOOM_TIMES = 6;
    int mBulletId = 63688;
    int mBoomId = 63689;
    int mHurtId = 0;

    int mBoomTimes = 0;
    bool mOriginFace = false;
    public Mechanism2049(int sid, int skillLevel) : base(sid, skillLevel)
    {
    }

    public override void OnInit()
    {
        CREAT_NEXT_INTERVAL = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        BOOM_TIMES = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
        mBulletId = TableManager.GetValueFromUnionCell(data.ValueC[0], level);
        mBoomId = TableManager.GetValueFromUnionCell(data.ValueD[0], level);
        mHurtId = TableManager.GetValueFromUnionCell(data.ValueE[0], level);
    }

    public override void OnReset()
    {
        Clear();
    }

    public override void OnStart()
    {
        mBoomTimes = BOOM_TIMES;
        Clear();

        if(owner != null)
        {
            mOriginFace = owner.GetFace();
            if(mBulletEvent == null)
            {
                mBulletEvent = owner.RegisterEventNew(BeEventType.onAfterGenBullet, (args1) =>
                {
                    BeProjectile projectile = args1.m_Obj as BeProjectile;
                    if (projectile != null && projectile.m_iResID == mBulletId)
                    {
                        if(mBulletHitEvent == null)
                        {
                            mBulletHitEvent = projectile.RegisterEventNew(BeEventType.onHitOther, args2 =>
                            //mBulletHitEvent = projectile.RegisterEvent(BeEventType.onHitOther, (object[] args2) =>
                            {
                                mTargetPos = args2.m_Vint3;
                                if (!creatFlag && mFirstTarget == null && args2.m_Int == mHurtId) //确保只会召唤一次爆炸 判断触发效果id
                                {
                                    mFirstTarget = args2.m_Obj as BeEntity;
                                    creatFlag = true;
                                }
                            });
                        }
                    }
                });
            }
        }
    }

    private int timer = 0;
    private bool creatFlag = false;
    public override void OnUpdate(int deltaTime)
    {
        if (creatFlag)
        {
            var thisAttachBuff = GetAttachBuff();
            if(mBoomTimes <= 0 && owner != null && thisAttachBuff != null)
            {
                //owner.buffController.RemoveBuff(this.attachBuff);
                RemoveAttachBuff();
            }
            timer += deltaTime;
            if (timer >= CREAT_NEXT_INTERVAL && mBoomTimes > 0) 
            {
                timer = 0;
                CreatBoom();
            }
        }
    }

    void CreatBoom()
    {
        if (mFirstTarget != null && !mFirstTarget.IsDeadOrRemoved())
        {
            VInt3 newPos = mTargetPos;
            newPos.x = mFirstTarget.GetPosition().x;
            newPos.y = mFirstTarget.GetPosition().y;

            mTargetPos = newPos;
        }
        BeEntity boom = owner.AddEntity(mBoomId, mTargetPos);
        if(boom != null)
        {
            boom.SetFace(mOriginFace, true, true);
        }
        mBoomTimes--;
    }

    public override void OnFinish()
    {
        Clear();
    }

    void Clear()
    {
        mFirstTarget = null;
        mTargetPos = VInt3.zero;
        RemoveHandle();
        // mBoomTimes = 0;
        mOriginFace = false;
        timer = 0;
        creatFlag = false;
    }

    void RemoveHandle()
    {
        if(mBulletEvent != null)
        {
            mBulletEvent.Remove();
            mBulletEvent = null;
        }
        if(mBulletHitEvent != null)
        {
            mBulletHitEvent.Remove();
            mBulletHitEvent = null;
        }
    }
}

