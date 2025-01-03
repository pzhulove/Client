//Created Time : 2020-7-27
//Created By Shensi
//Description:
//人物，怪物相关命令集，以及对应的处理器
using UnityEngine;
using System.Collections.Generic;
#if !LOGIC_SERVER
//人物阴影显隐命令
public sealed class ActorShadowVisibleGBCommand : IGBCommand
{
    public GeSceneEx scene;
    public bool isVisible;
    public GeActorEx actor;
    public override bool Resume()
    {
        actor.SetShadowVisible(scene, isVisible);
        return true;
    }
    public override void OnRecycle()
    {
        actor = null;
        scene = null;
        isVisible = false;
    }
    public override byte CmdType()
    {
        return CommandType;
    }
    public override byte Catalog()
    {
        return CatalogType;
    }
    public static byte CommandType { get { return (byte)GeEntityBackCmdType.Actor_Shadow_Visible; } }
    public static byte CatalogType { get { return (byte)GB_CATALOG.ENTITY; } }
    public static ActorShadowVisibleGBCommand Acquire() { return GBCommandPoolSystem.GetInstance().Allocate(CatalogType, CommandType) as ActorShadowVisibleGBCommand; }
}
//人物换模型命令
public sealed  class ActorChangeModelGBCommand : IGBCommand
{
    public bool isPreChange;
    public int resId;
    public GeActorEx actor;
    public override bool Resume()
    {
        if (isPreChange)
        {
            actor.PreChangeModel(resId);
        }
        else
        {
            actor.TryChangeModel(resId);
        }
        return true;
    }
    public override void OnRecycle()
    {
        actor = null;
        isPreChange = false;
        resId = 0;
    }

    public override byte CmdType()
    {
        return CommandType;
    }
    public override byte Catalog()
    {
        return CatalogType;
    }
    public static byte CommandType { get { return (byte)GeEntityBackCmdType.Change_Model; } }
    public static byte CatalogType { get { return (byte)GB_CATALOG.ENTITY; } }
    public static ActorChangeModelGBCommand Acquire() { return GBCommandPoolSystem.GetInstance().Allocate(CatalogType, CommandType) as ActorChangeModelGBCommand; }
}
//人物显隐命令
public sealed  class ActorVisibleGBCommand : IGBCommand
{
    public bool isVisible;
    public GeActorEx actor;
    public override bool Resume()
    {
        actor.SetActorVisible(isVisible);
        return true;
    }
    public override void OnRecycle()
    {
        actor = null;
        isVisible = false;
    }
    public override byte CmdType()
    {
        return CommandType;
    }
    public override byte Catalog()
    {
        return CatalogType;
    }
    public static byte CommandType { get { return (byte)GeEntityBackCmdType.Actor_Visible; } }
    public static byte CatalogType { get { return (byte)GB_CATALOG.ENTITY; } }
    public static ActorVisibleGBCommand Acquire() { return GBCommandPoolSystem.GetInstance().Allocate(CatalogType, CommandType) as ActorVisibleGBCommand; }
}
//人物显示等级命令
public sealed  class ActorLowLevelGBCommand : IGBCommand
{
    public GeActorEx actor;
    public override bool Resume()
    {
        actor.SetActorForLowLevel();
        return true;
    }
    public override void OnRecycle()
    {
        actor = null;
    }
    public override byte CmdType()
    {
        return CommandType;
    }
    public override byte Catalog()
    {
        return CatalogType;
    }
    public static byte CommandType { get { return (byte)GeEntityBackCmdType.Actor_LowLevel; } }
    public static byte CatalogType { get { return (byte)GB_CATALOG.ENTITY; } }
    public static ActorLowLevelGBCommand Acquire() { return GBCommandPoolSystem.GetInstance().Allocate(CatalogType, CommandType) as ActorLowLevelGBCommand; }
}
//人物创建命令
public sealed  class ActorCreateGBCommand : IGBCommand
{
    public int resID;
    public GameObject entityRoot;
    public GeSceneEx scene;
    public int iUnitId;
    public bool isBattleActor;
    public bool usePool;
    public bool useCube;
    public GeActorEx actor;
    public override bool Resume()
    {
        if (!actor.Create(resID, entityRoot, scene, iUnitId, isBattleActor, usePool, useCube))
        {
            actor.Remove();
        }
        return true;
    }
    public override void OnRecycle()
    {
         resID = 0;
         entityRoot = null;
         scene = null;
         iUnitId = 0;
         isBattleActor = false;
         usePool = false;
         useCube = false;
         actor = null;
    }
    public override byte CmdType()
    {
        return CommandType;
    }
    public override byte Catalog()
    {
        return CatalogType;
    }
    public static byte CommandType { get { return (byte)GeEntityBackCmdType.Create_Actor; } }
    public static byte CatalogType { get { return (byte)GB_CATALOG.ENTITY; } }
    public static ActorCreateGBCommand Acquire() { return GBCommandPoolSystem.GetInstance().Allocate(CatalogType, CommandType) as ActorCreateGBCommand; }
}

//人物怪物实体处理器
public sealed class GeEntityGraphicBack : GraphicBackController
{
    public GeEntityGraphicBack()
    {
        consecutiveCodes = new int[] { (int)(GeEntityBackCmdType.Create_Actor),(int)GeEntityBackCmdType.Suit_Avatar,(int)GeEntityBackCmdType.Change_Avatar,
                                        (int)(GeEntityBackCmdType.Preload_Animation), (int)(GeEntityBackCmdType.Add_Shadow),
                                        (int)(GeEntityBackCmdType.Actor_Shadow_Visible),
                                        (int)(GeEntityBackCmdType.Change_Model),(int)(GeEntityBackCmdType.Actor_Visible),(int)(GeEntityBackCmdType.Actor_LowLevel) ,
                                        (int)GeEntityBackCmdType.Create_SnapShot,(int)GeEntityBackCmdType.Change_Action,
                                        (int)GeEntityBackCmdType.Resume_Animation,(int)GeEntityBackCmdType.Pause_Animation,(int)GeEntityBackCmdType.Stop_Action,
                                        (int)GeEntityBackCmdType.Play_Attachment_Ani};
    }
    private Dictionary<string, IGBCommand> mAttachCommand = new Dictionary<string, IGBCommand>();
    public override GB_CATALOG CataLog() { return GB_CATALOG.ENTITY; }
    public static GeEntityGraphicBack Acquire() { return GBControllerAllocator.GetInstance().Allocate((byte)GB_CATALOG.ENTITY) as GeEntityGraphicBack; }
    //人物未创建时有关挂件的命令帧放入这里,由人物来管理
    public IGBCommand GetAttachmentCmd(string attachName)
    {
        if (mAttachCommand.ContainsKey(attachName))
        {
            return mAttachCommand[attachName];
        }
        return null;
    }
    public void RecordAttachmentCmd(string attachName, IGBCommand cmd)
    {
        if (mAttachCommand.ContainsKey(attachName))
        {
            mAttachCommand[attachName] = cmd;
        }
        else
        {
            mAttachCommand.Add(attachName, cmd);
        }
    }
    public override void Recycle()
    {
        base.Recycle();
        _recycleAttachmentCmd();
    }
    private void _recycleAttachmentCmd()
    {
        var iter = mAttachCommand.GetEnumerator();
        while (iter.MoveNext())
        {
            var curCmd = iter.Current.Value;
            if (curCmd != null)
                curCmd.Recycle();
        }
        mAttachCommand.Clear();
    }
    public override void FlipToFront()
    {
        _executeCmd();
        base.Recycle();
    }
    public void FlipAttachToFront()
    {
        var iter = mAttachCommand.GetEnumerator();
        while (iter.MoveNext())
        {
            iter.Current.Value.Resume();
        }
        _recycleAttachmentCmd();
    }
    public override void OnRecycle()
    {

    }
    
    public override void RecordCmd(int type, IGBCommand cmd)
    {
        switch ((GeEntityBackCmdType)type)
        {
            //一般情况下，这几类命令类型，都是替换关系，没有特别额外的处理
            case GeEntityBackCmdType.Add_Shadow:
            case GeEntityBackCmdType.Create_Actor:
            case GeEntityBackCmdType.Create_SnapShot:
            case GeEntityBackCmdType.Actor_Shadow_Visible:
            case GeEntityBackCmdType.Change_Model:
            case GeEntityBackCmdType.Actor_Visible:
            case GeEntityBackCmdType.Actor_LowLevel:
            case GeEntityBackCmdType.Suit_Avatar:
                {
                    if (mCmdList.ContainsKey(type))
                    {
                        if(mCmdList[type] != null)
                        {
                            mCmdList[type].Recycle();
                        }
                        mCmdList[type] = cmd;
                    }
                    else
                    {
                        mCmdList.Add(type, cmd);
                    }
                }
                break;
                //换装的情况下，需要对不同部位的装备，进行替换
            case GeEntityBackCmdType.Change_Avatar:
                {
                    var curChangeAvtarCmd = cmd as GeEntity.EntityChangeAvatarGBCommand;
                    if (curChangeAvtarCmd == null)
                    {
                        Logger.LogError("Input Change_Avatar curCmd is not right cmd");
                        return;
                    }
                    if (mCmdList.ContainsKey(type))
                    {
                        var originChangeAvatarCmd = mCmdList[type] as GeEntity.EntityChangeAvatarGBCommand;
                        if (originChangeAvatarCmd != null)
                        {
                            var iter = curChangeAvtarCmd.changeAvatarGBInfo.GetEnumerator();
                            while (iter.MoveNext())
                            {
                                if (originChangeAvatarCmd.changeAvatarGBInfo.ContainsKey(iter.Current.Key))
                                {
                                    GeEntity.ChangeAvatarInfoPool.Release(originChangeAvatarCmd.changeAvatarGBInfo[iter.Current.Key]);
                                    originChangeAvatarCmd.changeAvatarGBInfo[iter.Current.Key] = iter.Current.Value;
                                }
                                else
                                {
                                    originChangeAvatarCmd.changeAvatarGBInfo.Add(iter.Current.Key, iter.Current.Value);
                                }
                            }
                        }
                        else
                        {
                            Logger.LogError("Input Change_Avatar origin cmd is not right cmd");
                        }
                        curChangeAvtarCmd.Recycle();
                    }
                    else
                    {
                        mCmdList.Add(type, cmd);
                    }
                }
                break;
                //停止播放动作的时候，需要查询在命令集中是否存在播放动作的命令，这个时候需要计算播放动作命令已经经过的时长
            case GeEntityBackCmdType.Stop_Action:
                {
                    var stopCmd = cmd as GeEntity.EntityStopAniGBCommand;
                    if (stopCmd == null)
                    {
                        Logger.LogError("Input Stop_Action is not right cmd");
                        return;
                    }
                    if (!mCmdList.ContainsKey((int)GeEntityBackCmdType.Change_Action))
                    {
                        return;
                    }
                    var changeActionCmd = mCmdList[(int)GeEntityBackCmdType.Change_Action] as GeEntity.EntityChangeActionGBCommand;
                    if (changeActionCmd == null)
                    {
                        Logger.LogError("Input Stop_Action changeActionCmd is not right cmd");
                        return;
                    }
                    //如果上一次已经停止了动作那么就不需要计算动画帧的偏移
                    if (changeActionCmd.isStop) return;
                    changeActionCmd.isStop = true;
                    float deltaTime = (stopCmd.timeStamp - changeActionCmd.timeStamp) / 1000.0f;
                    changeActionCmd.offset += deltaTime;
                    if (mCmdList.ContainsKey(type))
                    {
                        mCmdList[type].Recycle();
                        mCmdList[type] = cmd;
                    }
                    else
                    {
                        mCmdList.Add(type, cmd);
                    }
                }
                break;
            //暂停播放动作的时候，需要查询在命令集中是否存在播放动作的命令，这个时候需要计算播放动作命令已经经过的时长
            case GeEntityBackCmdType.Pause_Animation:
                {
                    var pauseCmd = cmd as GeEntity.EntityPauseAniGBCommand;
                    if (pauseCmd == null)
                    {
                        Logger.LogError("Input Pause_Animation is not right cmd");
                        return;
                    }
                    if (!mCmdList.ContainsKey((int)GeEntityBackCmdType.Change_Action))
                    {
                        return;
                    }
                    var changeActionCmd = mCmdList[(int)GeEntityBackCmdType.Change_Action] as GeEntity.EntityChangeActionGBCommand;
                    if (changeActionCmd == null)
                    {
                        Logger.LogError("Input Pause_Animation changeActionCmd is not right cmd");
                        return;
                    }
                    //如果上一次已经停止了动作，不要要重复计算                    
                    if (changeActionCmd.isStop) return;
                    //如果上一次已经暂停了动作，也不需要重复计算
                    if (changeActionCmd.isPause) return;
                    float deltaTime = (pauseCmd.timeStamp - changeActionCmd.timeStamp) / 1000.0f;
                    changeActionCmd.isPause = true;
                    changeActionCmd.offset += deltaTime;
                    if (mCmdList.ContainsKey(type))
                    {
                        mCmdList[type].Recycle();
                        mCmdList[type] = cmd;
                    }
                    else
                    {
                        mCmdList.Add(type, cmd);
                    }
                }
                break;
                //恢复了动作则相当于将播放动作的时间戳重新计时，这样能够在下一次有关相关动作命令的时候，将动画偏移计算正确
            case GeEntityBackCmdType.Resume_Animation:
                {
                    var resumeCmd = cmd as GeEntity.EntityResumeAniGBCommand;
                    if (resumeCmd == null)
                    {
                        Logger.LogError("Input Resume_Animation is not right cmd");
                        return;
                    }
                    if (!mCmdList.ContainsKey((int)GeEntityBackCmdType.Change_Action))
                    {
                        return;
                    }
                    var changeActionCmd = mCmdList[(int)GeEntityBackCmdType.Change_Action] as GeEntity.EntityChangeActionGBCommand;
                    if (changeActionCmd == null)
                    {
                        Logger.LogError("Input Resume_Animation changeActionCmd is not right cmd");
                        return;
                    }
                    //如果上一次收到了停止播放动画命令，则不要处理
                    if (changeActionCmd.isStop) return;
                    //上一次收到了暂停播放动画命令，则删除暂停播放动画命令帧
                    if (changeActionCmd.isPause)
                    {
                        changeActionCmd.timeStamp = resumeCmd.timeStamp;
                        if (mCmdList.ContainsKey((int)GeEntityBackCmdType.Pause_Animation))
                        {
                            mCmdList[(int)GeEntityBackCmdType.Pause_Animation].Recycle();
                            mCmdList.Remove((int)GeEntityBackCmdType.Pause_Animation);
                        }
                        else
                        {
                            Logger.LogErrorFormat("GeEntityBackCmdType Resume_Animation has not right pause animation {0}", changeActionCmd._this.EntityName);
                        }
                    }
                    else
                    {
                        return;
                    }
                    changeActionCmd.isPause = false;
                    if (mCmdList.ContainsKey(type))
                    {
                        mCmdList[type].Recycle();
                        mCmdList[type] = cmd;
                    }
                    else
                    {
                        mCmdList.Add(type, cmd);
                    }
                }
                break;
             //预加载动画命令帧
            case GeEntityBackCmdType.Preload_Animation:
                {
                    var newCmd = cmd as GeEntity.EntityPreloadAniGBCommand;
                    if (newCmd == null)
                    {
                        Logger.LogError("Input Preload_Animation is not right cmd");
                        return;
                    }
                    if (mCmdList.ContainsKey((int)GeEntityBackCmdType.Preload_Animation))
                    {
                        var originCmd = mCmdList[(int)GeEntityBackCmdType.Preload_Animation] as GeEntity.EntityPreloadAniGBCommand;
                        if (originCmd != null)
                        {
                            originCmd.aniNames.Add(newCmd.aniName);
                        }
                        else
                        {
                            Logger.LogError("Input orgin Preload_Animation is not right cmd");
                        }
                        newCmd.Recycle();
                    }
                    else
                    {
                        newCmd.aniNames = new List<string>();
                        newCmd.aniNames.Add(newCmd.aniName);
                        mCmdList.Add((int)GeEntityBackCmdType.Preload_Animation, newCmd);
                    }
                }
                break;
                //当播放新的动画命令时，去掉上一次的暂停，停止，恢复动画帧命令，并且把上一次播放挂件的动画命令帧去除
            case GeEntityBackCmdType.Change_Action:
                {
                    if (mCmdList.ContainsKey((int)GeEntityBackCmdType.Pause_Animation))
                    {
                        mCmdList[(int)GeEntityBackCmdType.Pause_Animation].Recycle();
                        mCmdList.Remove((int)GeEntityBackCmdType.Pause_Animation);
                    }
                    if (mCmdList.ContainsKey((int)GeEntityBackCmdType.Resume_Animation))
                    {
                        mCmdList[(int)GeEntityBackCmdType.Resume_Animation].Recycle();
                        mCmdList.Remove((int)GeEntityBackCmdType.Resume_Animation);
                    }
                    if (mCmdList.ContainsKey((int)GeEntityBackCmdType.Stop_Action))
                    {
                        mCmdList[(int)GeEntityBackCmdType.Stop_Action].Recycle();
                        mCmdList.Remove((int)GeEntityBackCmdType.Stop_Action);
                    }
                    if (mCmdList.ContainsKey((int)GeEntityBackCmdType.Change_Action))
                    {
                        mCmdList[(int)GeEntityBackCmdType.Change_Action].Recycle();
                        mCmdList[(int)GeEntityBackCmdType.Change_Action] = cmd;
                    }
                    else
                    {
                        mCmdList.Add((int)GeEntityBackCmdType.Change_Action, cmd);
                    }
                    _recycleAttachmentCmd();
                }
                break;
            case GeEntityBackCmdType.Destroy:
                {
                    Recycle();
                }
                break;
        }
    }
}
#endif