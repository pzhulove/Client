using System.Collections.Generic;
//Created Time : 2020-7-27
//Created By Shensi
//Description:
//命令帧缓存池
public interface GBCommandPool
{
    //从池子中取出命令帧给外部使用
    IGBCommand Allocate(byte cmdType);
    //将已使用完毕的命令帧放入池子中
    void Free(IGBCommand cmd);
    //清空池子
    void Clear();
}
#if !LOGIC_SERVER
//实体，人物，怪物命令帧缓存池
public sealed class GBEntityCommandPool: GBCommandPool
{
    List<IGBCommand>[] mCmdList = new List<IGBCommand>[(int)GeEntityBackCmdType.MAX_COUNT];
    public IGBCommand Allocate(byte cmdType)
    {
        var poolList = mCmdList[cmdType];
        if (poolList == null)
        {
            mCmdList[cmdType] = new List<IGBCommand>();
            poolList = mCmdList[cmdType];
        }
        IGBCommand newCmd = null;
        if (poolList.Count > 0)
        {
            int tailIndex = poolList.Count - 1;
            newCmd = poolList[tailIndex];
            poolList.RemoveAt(tailIndex);
        }
        else
        {
            switch ((GeEntityBackCmdType)cmdType)
            {
                case GeEntityBackCmdType.Actor_LowLevel:
                    newCmd = new ActorLowLevelGBCommand();
                    break;
                case GeEntityBackCmdType.Actor_Shadow_Visible:
                    newCmd = new ActorShadowVisibleGBCommand();
                    break;
                case GeEntityBackCmdType.Actor_Visible:
                    newCmd = new ActorVisibleGBCommand();
                    break;
                case GeEntityBackCmdType.Add_Shadow:
                    newCmd = new GeEntity.EntityAddShadowGBCommand();
                    break;
                case GeEntityBackCmdType.Change_Action:
                    newCmd = new GeEntity.EntityChangeActionGBCommand();
                    break;
                case GeEntityBackCmdType.Change_Avatar:
                    newCmd = new GeEntity.EntityChangeAvatarGBCommand();
                    break;
                case GeEntityBackCmdType.Change_Model:
                    newCmd = new ActorChangeModelGBCommand();
                    break;
                case GeEntityBackCmdType.Create_Actor:
                    newCmd = new ActorCreateGBCommand();
                    break;
                case GeEntityBackCmdType.Create_SnapShot:
                    newCmd = new GeEntity.EntityCreateSnapShotGBCommand();
                    break;
                case GeEntityBackCmdType.Destroy:
                    break;
                case GeEntityBackCmdType.Pause_Animation:
                    newCmd = new GeEntity.EntityPauseAniGBCommand();
                    break;
                case GeEntityBackCmdType.Play_Attachment_Ani:
                    newCmd = new GeEntity.EntityAttachmentPlayAniGBCommond();
                    break;
                case GeEntityBackCmdType.Preload_Animation:
                    newCmd = new GeEntity.EntityPreloadAniGBCommand();
                    break;
                case GeEntityBackCmdType.Resume_Animation:
                    newCmd = new GeEntity.EntityResumeAniGBCommand();
                    break;
                case GeEntityBackCmdType.Stop_Action:
                    newCmd = new GeEntity.EntityStopAniGBCommand();
                    break;
                case GeEntityBackCmdType.Suit_Avatar:
                    newCmd = new GeEntity.EntitySuitAvatarGBCommand();
                    break;
            }
        }
        return newCmd;
    }
    public void Free(IGBCommand cmd)
    {
        if (cmd == null) return;
        var poolList = mCmdList[(int)cmd.CmdType()];
        if(poolList == null)
        {
            Logger.LogErrorFormat("Entity PoolCmd Does not exist {0}", cmd.CmdType());
            return;
        }
        cmd.OnRecycle();
        poolList.Add(cmd);
    }
    public void Clear()
    {
        for(int i = 0; i < mCmdList.Length;i++)
        {
            if (mCmdList[i] == null) continue;
            mCmdList[i].Clear();
        }
    }
}
//特效命令帧缓存池
public sealed class GBEffectCommandPool : GBCommandPool
{
    List<IGBCommand>[] mCmdList = new List<IGBCommand>[(int)GeEffectBackCmdType.MAX_COUNT];
    public IGBCommand Allocate(byte cmdType)
    {
        var poolList = mCmdList[cmdType];
        if (poolList == null)
        {
            mCmdList[cmdType] = new List<IGBCommand>();
            poolList = mCmdList[cmdType];
        }
        IGBCommand newCmd = null;
        if (poolList.Count > 0)
        {
            int tailIndex = poolList.Count - 1;
            newCmd = poolList[tailIndex];
            poolList.RemoveAt(tailIndex);
        }
        else
        {
            switch ((GeEffectBackCmdType)cmdType)
            {
                case GeEffectBackCmdType.Create:
                    newCmd = new GeEffectEx.CreateEffectGBCommand();
                    break;
                case GeEffectBackCmdType.Destroy:
                    break;
                case GeEffectBackCmdType.Pause_Animation:
                    newCmd = new GeEffectEx.PauseEffectAnimationGBCommand();
                    break;
                case GeEffectBackCmdType.Resume_Animation:
                    newCmd = new GeEffectEx.ResumeEffectAnimationGBCommand();
                    break;
                case GeEffectBackCmdType.Play_Animation:
                    newCmd = new GeEffectEx.PlayAnimationEffectGBCommand();
                    break;
                case GeEffectBackCmdType.Layer:
                    newCmd = new GeEffectEx.LayerEffectGBCommand();
                    break;
                case GeEffectBackCmdType.Speed:
                    newCmd = new GeEffectEx.SpeedEffectGBCommand();
                    break;
                case GeEffectBackCmdType.Time_Len:
                    newCmd = new GeEffectEx.TimeLenEffectGBComand();
                    break;
                case GeEffectBackCmdType.Reset_Elapse_Time:
                    newCmd = new GeEffectEx.ResetElapseTimeEffectGBCommand();
                    break;
                case GeEffectBackCmdType.Visible:
                    newCmd = new GeEffectEx.VisibleEffectGBCommand();
                    break;
                case GeEffectBackCmdType.Local_Position:
                    newCmd = new GeEffectEx.LocalPositionEffectGBCommand();
                    break;
                case GeEffectBackCmdType.Position:
                    newCmd = new GeEffectEx.PositionEffectGBCommand();
                    break;
                case GeEffectBackCmdType.Scale:
                    newCmd = new GeEffectEx.ScaleEffectGBCommand();
                    break;
                case GeEffectBackCmdType.Scale_Effect:
                    newCmd = new GeEffectEx.LocalScaleEffectGBCommand();
                    break;
                case GeEffectBackCmdType.Rotation:
                    newCmd = new GeEffectEx.RotationEffectGBCommand();
                    break;
            }
        }
        return newCmd;
    }
    public void Free(IGBCommand cmd)
    {
        if (cmd == null) return;
        var poolList = mCmdList[(int)cmd.CmdType()];
        if (poolList == null)
        {
            Logger.LogErrorFormat("Effect PoolCmd Does not exist {0}", cmd.CmdType());
            return;
        }
        cmd.OnRecycle();
        poolList.Add(cmd);
    }
    public void Clear()
    {
        for (int i = 0; i < mCmdList.Length; i++)
        {
            if (mCmdList[i] == null) continue;
            mCmdList[i].Clear();
        }
    }
}
//挂件命令帧缓存池
public sealed class GBAttachCommandPool : GBCommandPool
{
    List<IGBCommand>[] mCmdList = new List<IGBCommand>[(int)GeAttachBackCmdType.MAX_COUNT];
    public IGBCommand Allocate(byte cmdType)
    {
        var poolList = mCmdList[cmdType];
        if (poolList == null)
        {
            mCmdList[cmdType] = new List<IGBCommand>();
            poolList = mCmdList[cmdType];
        }
        IGBCommand newCmd = null;
        if (poolList.Count > 0)
        {
            int tailIndex = poolList.Count - 1;
            newCmd = poolList[tailIndex];
            poolList.RemoveAt(tailIndex);
        }
        else
        {
            switch ((GeAttachBackCmdType)cmdType)
            {
                case GeAttachBackCmdType.Create:
                    newCmd = new GeAttach.AttachCreateGBCommand();
                    break;
                case GeAttachBackCmdType.Destroy:
                    break;
                case GeAttachBackCmdType.Pause_Animation:
                    newCmd = new GeAttach.AttachPauseAniGBCommand();
                    break;
                case GeAttachBackCmdType.Resume_Animation:
                    newCmd = new GeAttach.AttachResumeAniGBCommand();
                    break;
                case GeAttachBackCmdType.Change_Phase:
                    newCmd = new GeAttach.AttachChangePhaseGBCommand();
                    break;
                case GeAttachBackCmdType.Layer_Set:
                    newCmd = new GeAttach.AttachLayerGBCommand();
                    break;
                case GeAttachBackCmdType.Visible_Enable:
                    newCmd = new GeAttach.AttachVisibleGBCommand();
                    break;
                case GeAttachBackCmdType.Stop_Action:
                    newCmd = new GeAttach.AttachStopActionGBCommand();
                    break;
                case GeAttachBackCmdType.Play_Action:
                    newCmd = new GeAttach.AttachPlayActionGBCommand();
                    break;
            }
        }
        return newCmd;
    }
    public void Free(IGBCommand cmd)
    {
        if (cmd == null) return;
        var poolList = mCmdList[(int)cmd.CmdType()];
        if (poolList == null)
        {
            Logger.LogErrorFormat("Effect PoolCmd Does not exist {0}", cmd.CmdType());
            return;
        }
        cmd.OnRecycle();
        poolList.Add(cmd);
    }
    public void Clear()
    {
        for (int i = 0; i < mCmdList.Length; i++)
        {
            if (mCmdList[i] == null) continue;
            mCmdList[i].Clear();
        }
    }
}
//材质命令帧缓存池
public sealed class GBMaterialCommandPool : GBCommandPool
{
    List<IGBCommand>[] mCmdList = new List<IGBCommand>[(int)AnimatBackCmdType.MAX_COUNT];
    public IGBCommand Allocate(byte cmdType)
    {
        var poolList = mCmdList[cmdType];
        if (poolList == null)
        {
            mCmdList[cmdType] = new List<IGBCommand>();
            poolList = mCmdList[cmdType];
        }
        IGBCommand newCmd = null;
        if (poolList.Count > 0)
        {
            int tailIndex = poolList.Count - 1;
            newCmd = poolList[tailIndex];
            poolList.RemoveAt(tailIndex);
        }
        else
        {
            switch ((AnimatBackCmdType)cmdType)
            {
                case AnimatBackCmdType.Push:
                    newCmd = new GeAnimatManagerEx.PushAnimatGBCommand();
                    break;
                case AnimatBackCmdType.Remove:
                    newCmd = new GeAnimatManagerEx.RemoveAnimatGBCommand();
                    break;
            }
        }
        return newCmd;
    }
    public void Free(IGBCommand cmd)
    {
        if (cmd == null) return;
        var poolList = mCmdList[(int)cmd.CmdType()];
        if (poolList == null)
        {
            Logger.LogErrorFormat("Material PoolCmd Does not exist {0}", cmd.CmdType());
            return;
        }
        cmd.OnRecycle();
        poolList.Add(cmd);
    }
    public void Clear()
    {
        for (int i = 0; i < mCmdList.Length; i++)
        {
            if (mCmdList[i] == null) continue;
            mCmdList[i].Clear();
        }
    }
}
//所有命令帧总管，对外部使用者可见，方便所有命令帧统一管理
public class GBCommandPoolSystem:Singleton<GBCommandPoolSystem>
{
    private GBCommandPool[] m_Pools = new GBCommandPool[(int)GB_CATALOG.MAX_COUNT];
    public override void Init()
    {
        m_Pools[(int)GB_CATALOG.ENTITY] = new GBEntityCommandPool();
        m_Pools[(int)GB_CATALOG.ATTACH] = new GBAttachCommandPool();
        m_Pools[(int)GB_CATALOG.EFFECT] = new GBEffectCommandPool();
        m_Pools[(int)GB_CATALOG.MATERIAL] = new GBMaterialCommandPool();       
    }
    public IGBCommand Allocate(byte calog,byte cmdType)
    {
        if (calog >= (byte)GB_CATALOG.MAX_COUNT) return null;
        var curPool = m_Pools[(int)calog];
        IGBCommand newCmd = curPool.Allocate(cmdType);
        if(newCmd != null)
        {
            newCmd.OnCreate();
        }
        return newCmd;
    }
    public  void Free(IGBCommand cmd)
    {
        if (cmd == null || cmd.Catalog() >= (byte)GB_CATALOG.MAX_COUNT) return;
        m_Pools[cmd.Catalog()].Free(cmd);
    }
    public void Clear()
    {
        for(int i = 0; i < m_Pools.Length;i++)
        {
            m_Pools[i].Clear();
        }
    }
}
#endif