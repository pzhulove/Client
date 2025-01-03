using GameClient;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//触碰到阻挡反弹
public class Mechanism141 : BeMechanism
{
    public Mechanism141(int mid, int lv) : base(mid, lv) { }

    protected int removeBuffId = 360802;                //触碰地面的时候移除的BuffId
    protected VInt xSpeed = 0;                           //反弹的时候X轴速度
    protected VInt zSpeed = 0;                           //反弹的时候Z轴速度
    protected int rotateSpeed = 0;                      //反弹的时候的旋转速度
    protected int mBlockHurtID = 0;                     // 撞墙时的触发效果id（选填）
    private bool mCanRotateOnControled = false;            // 在受控状态下是否执行（选填，默认不执行）
    private bool mRemoveOnTouchGround = true;

    protected bool reboundFlag = false;
    protected int curRotateAngle = 0;
    protected Transform root = null;
    protected GameObject shadowObj = null;
    protected Vector3 shadowStartPos = Vector3.zero;

    public override void OnReset()
    {
        xSpeed = 0; 
        zSpeed = 0;
        rotateSpeed = 0;
        mBlockHurtID = 0;
        mCanRotateOnControled = false;
        mRemoveOnTouchGround = true;
        reboundFlag = false;
        curRotateAngle = 0;
        root = null;
        shadowObj = null;
        shadowStartPos = Vector3.zero;
    }
    public override void OnInit()
    {
        base.OnInit();
        removeBuffId = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        if(data.ValueB.Count > 0)
        {
            xSpeed = VInt.NewVInt(TableManager.GetValueFromUnionCell(data.ValueB[0], level),GlobalLogic.VALUE_1000);
            zSpeed = VInt.NewVInt(TableManager.GetValueFromUnionCell(data.ValueB[1], level), GlobalLogic.VALUE_1000);
        }

        if (data.ValueC.Count > 0)
        {
            rotateSpeed = TableManager.GetValueFromUnionCell(data.ValueC[0], level);
        }
        
        if (data.ValueD.Count > 0)
        {
            mBlockHurtID = TableManager.GetValueFromUnionCell(data.ValueD[0], level);
        }

        if (data.ValueE.Count > 0)
        {
            mCanRotateOnControled = TableManager.GetValueFromUnionCell(data.ValueE[0], level) != 0;
        }

        mRemoveOnTouchGround = data.ValueF.Count == 0;
    }

    public override void OnStart()
    {
        if (!CanRotate())
            return;
        reboundFlag = false;
        RecordModelInfo();
        handleA = owner.RegisterEventNew(BeEventType.onXInBlock, (args) =>
        {
            Rebound();
        });

        if (mRemoveOnTouchGround)
        {
            handleB = owner.RegisterEventNew(BeEventType.onTouchGround, eventParam =>
            {
                TouchGround();
            });
        }

        handleC = OwnerRegisterEventNew(BeEventType.onHit, args =>
        //handleC = owner.RegisterEvent(BeEventType.onHit, (object[] args) => 
        {
            owner.buffController.RemoveBuff(removeBuffId);
        });
    }

    public override void OnUpdate(int deltaTime)
    {
        if (!CanRotate())
            return;
        base.OnUpdate(deltaTime);
        Rotating(deltaTime);
    }

    public override void OnFinish()
    {
        base.OnFinish();
        RestoreRotate();
    }
#if !LOGIC_SERVER
    GameObject rotate;
#endif
    //记录下模型信息
    protected void RecordModelInfo()
    {
#if !LOGIC_SERVER
        if (owner.m_pkGeActor == null)
            return;
        if (rotate == null)
        {
            rotate = new GameObject();
            root = owner.m_pkGeActor.GetEntityNode(GeEntity.GeEntityNodeType.Charactor).transform;
            if(root != null)
                rotate.transform.SetParent(root);
            rotate.transform.localPosition = Vector3.up;
            rotate.transform.localRotation = Quaternion.identity;
            rotate.transform.localScale = Vector3.one;
        }
        shadowObj = owner.m_pkGeActor.GetShadowObj();
        if (shadowObj != null)
            shadowStartPos = shadowObj.transform.localPosition;
#endif
    }

    //旋转
    protected void Rotating(int deltaTime)
    {
#if !LOGIC_SERVER
        if (root != null && rotateSpeed != 0)
        {
            curRotateAngle = (curRotateAngle + rotateSpeed) % 360;
            root.RotateAround(rotate.transform.position, Vector3.forward, curRotateAngle);         
        }
#endif
    }

    //恢复原来的角度
    protected void RestoreRotate()
    {
#if !LOGIC_SERVER
        if (root == null)
            return;
        if(rotate!=null)
        {
            GameObject.Destroy(rotate);
            rotate = null;
        }
        root.localPosition = Vector3.zero;
        root.localEulerAngles = Vector3.zero;

        if (shadowObj != null)
            shadowObj.transform.localPosition = shadowStartPos;
        if (shadowObj != null)
            shadowObj.CustomActive(true);
#endif
    }

    //碰到阻挡反弹
    protected void Rebound()
    {
        if (reboundFlag)
            return;
        if (xSpeed == 0 && zSpeed == 0)
            return;
        
        if (mBlockHurtID > 0)
        {
            var thisAttachBuff = GetAttachBuff();
            if (thisAttachBuff != null && thisAttachBuff.releaser != null)
            {
                var hitPos = owner.GetPosition();
                hitPos.z += VInt.one.i;
                thisAttachBuff.releaser._onHurtEntity(owner, hitPos, mBlockHurtID);
            }
        }
        
        reboundFlag = true;
        VInt x = owner.moveXSpeed > 0 ? -xSpeed : xSpeed;
        VInt z = owner.moveYSpeed > 0 ? -zSpeed : zSpeed;
        owner.SetMoveSpeedX(x);
        owner.SetMoveSpeedZ(z);
#if !LOGIC_SERVER
        if (shadowObj != null)
            shadowObj.CustomActive(false);
#endif
    }

    //触碰到地面
    protected void TouchGround()
    {
        owner.buffController.RemoveBuff(removeBuffId);
    }

    //判断是否可以旋转
    protected bool CanRotate()
    {
        if (mCanRotateOnControled)
        {
            if (owner.GetStateGraph().CurrentStateHasTag((int)AStateTag.AST_LOCKZ))
                return false;
        }
        else
        {
            if (owner.GetStateGraph().CurrentStateHasTag((int)AStateTag.AST_CONTROLED | (int)AStateTag.AST_LOCKZ))
                return false;
        }
        
        if (!owner.stateController.CanBeFloat())
            return false;
        return true;
    }
}
