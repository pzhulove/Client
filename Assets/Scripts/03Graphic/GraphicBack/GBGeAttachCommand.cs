using UnityEngine;
//Created Time : 2020-7-27
//Created By Shensi
//Description:
//挂件相关命令集以及对应的处理器
public enum GeAttachBackCmdType
{
    Create = 0,
    Play_Action,
    Stop_Action,
    Visible_Enable,
    Layer_Set,
    Change_Phase,
    Pause_Animation,
    Resume_Animation,
    Destroy,
    MAX_COUNT
};
public partial class GeAttach
{
#if !LOGIC_SERVER
    //创建挂件
    public sealed class AttachCreateGBCommand : IGBCommand
    {
        public string attachRes;
        public GameObject parent;
        public string attachName;
        public bool copyInPool;
        public bool asyncLoad;
        public bool highPriority;
        public GeAttach attach;
        public override bool Resume()
        {
            attach.Create(attachRes, parent, attachName, copyInPool, asyncLoad, highPriority);
            attach.createdInBackMode = false;
            return true;
        }
        public override void OnRecycle()
        {
            attachRes = string.Empty;
            parent = null;
            attachName = string.Empty;
            copyInPool = false;
            asyncLoad = false;
            highPriority = false;
            attach = null;
        }
        public override byte CmdType()
        {
            return CommandType;
        }
        public override byte Catalog()
        {
            return CatalogType;
        }
        public static byte CommandType { get { return (byte)GeAttachBackCmdType.Create; } }
        public static byte CatalogType { get { return (byte)GB_CATALOG.ATTACH; } }
        public static AttachCreateGBCommand Acquire() { return GBCommandPoolSystem.GetInstance().Allocate(CatalogType, CommandType) as AttachCreateGBCommand; }
    }
    //播放挂件动画
    public sealed class AttachPlayActionGBCommand : IGBCommand
    {
        public string name;
        public float speed;
        public bool loop;
        public float offset;
        public bool isStop; //内部维护
        public bool isPause; //内部维护
        public long timeStamp;
        public GeAttach attach;
        public override bool Resume()
        {
            float curOfffset = (FrameSync.GetTicksNow() - timeStamp) / 1000.0f;
            attach.PlayAction(name, speed, loop, offset + curOfffset);
            return true;
        }
        public override void OnRecycle()
        {
            name = string.Empty;
            speed = 0.0f;
            loop = false;
            offset = 0.0f;
            timeStamp = 0;
            isStop = false;
            isPause = false;
            attach = null;
        }
        public override byte CmdType()
        {
            return CommandType;
        }
        public override byte Catalog()
        {
            return CatalogType;
        }
        public static byte CommandType { get { return (byte)GeAttachBackCmdType.Play_Action; } }
        public static byte CatalogType { get { return (byte)GB_CATALOG.ATTACH; } }
        public static AttachPlayActionGBCommand Acquire()
        {
            return GBCommandPoolSystem.GetInstance().Allocate(CatalogType, CommandType) as AttachPlayActionGBCommand;
        }
    }
    //停止挂件动画
    public sealed class AttachStopActionGBCommand : IGBCommand
    {
        public GeAttach attach;
        public long timeStamp;
        public override bool Resume()
        {
            attach.StopAction();
            return true;
        }
        public override void OnRecycle()
        {
            attach = null;
            timeStamp = 0;
        }
        public override byte CmdType()
        {
            return CommandType;
        }
        public override byte Catalog()
        {
            return CatalogType;
        }
        public static byte CommandType { get { return (byte)GeAttachBackCmdType.Stop_Action; } }
        public static byte CatalogType { get { return (byte)GB_CATALOG.ATTACH; } }
        public static AttachStopActionGBCommand Acquire()
        {
            return GBCommandPoolSystem.GetInstance().Allocate(CatalogType, CommandType) as AttachStopActionGBCommand;
        }
    }
    //显隐挂件
    public sealed class AttachVisibleGBCommand : IGBCommand
    {
        public bool isVisible;
        public GeAttach attach;
        public override bool Resume()
        {
            attach.SetVisible(isVisible);
            return true;
        }
        public override void OnRecycle()
        {
            isVisible = false;
            attach = null;
        }
        public override byte CmdType()
        {
            return CommandType;
        }
        public override byte Catalog()
        {
            return CatalogType;
        }
        public static byte CommandType { get { return (byte)GeAttachBackCmdType.Visible_Enable; } }
        public static byte CatalogType { get { return (byte)GB_CATALOG.ATTACH; } }
        public static AttachVisibleGBCommand Acquire()
        {
            return GBCommandPoolSystem.GetInstance().Allocate(CatalogType, CommandType) as AttachVisibleGBCommand;
        }
    }
    public sealed class AttachLayerGBCommand : IGBCommand
    {
        public int layer;
        public GeAttach attach;
        public override bool Resume()
        {
            attach.SetLayer(layer);
            return true;
        }
        public override void OnRecycle()
        {
            layer = 0;
            attach = null;
        }
        public override byte CmdType()
        {
            return CommandType;
        }
        public override byte Catalog()
        {
            return CatalogType;
        }
        public static byte CommandType { get { return (byte)GeAttachBackCmdType.Layer_Set; } }
        public static byte CatalogType { get { return (byte)GB_CATALOG.ATTACH; } }
        public static AttachLayerGBCommand Acquire()
        {
            return GBCommandPoolSystem.GetInstance().Allocate(CatalogType, CommandType) as AttachLayerGBCommand;
        }
    }
    //挂件换材质
    public sealed class AttachChangePhaseGBCommand : IGBCommand
    {
        public string phaseEffect;
        public int phaseIdx;
        public bool forceAddtive;
        public GeAttach attach;
        public override bool Resume()
        {
            attach.ChangePhase(phaseEffect, phaseIdx, forceAddtive);
            return true;
        }
        public override void OnRecycle()
        {
            phaseEffect = string.Empty;
            phaseIdx = 0;
            forceAddtive = false;
            attach = null;
        }
        public override byte CmdType()
        {
            return CommandType;
        }
        public override byte Catalog()
        {
            return CatalogType;
        }
        public static byte CommandType { get { return (byte)GeAttachBackCmdType.Change_Phase; } }
        public static byte CatalogType { get { return (byte)GB_CATALOG.ATTACH; } }
        public static AttachChangePhaseGBCommand Acquire()
        {
            return GBCommandPoolSystem.GetInstance().Allocate(CatalogType, CommandType) as AttachChangePhaseGBCommand;
        }
    }
    //恢复播放挂件动画
    public sealed class AttachResumeAniGBCommand : IGBCommand
    {
        public GeAttach attach;
        public long timeStamp;
        public override bool Resume()
        {
            attach.ResumeAnimation();
            return true;
        }
        public override void OnRecycle()
        {
            attach = null;
            timeStamp = 0;
        }
        public override byte CmdType()
        {
            return CommandType;
        }
        public override byte Catalog()
        {
            return CatalogType;
        }
        public static byte CommandType { get { return (byte)GeAttachBackCmdType.Resume_Animation; } }
        public static byte CatalogType { get { return (byte)GB_CATALOG.ATTACH; } }
        public static AttachResumeAniGBCommand Acquire()
        {
            return GBCommandPoolSystem.GetInstance().Allocate(CatalogType, CommandType) as AttachResumeAniGBCommand;
        }
    }
    //暂停播放挂件动画 
    public sealed class AttachPauseAniGBCommand : IGBCommand
    {
        public GeAttach attach;
        public long timeStamp;
        public override bool Resume()
        {
            attach.PauseAnimation();
            return true;
        }
        public override void OnRecycle()
        {
            attach = null;
            timeStamp = 0;
        }
        public override byte CmdType()
        {
            return CommandType;
        }
        public override byte Catalog()
        {
            return CatalogType;
        }
        public static byte CommandType { get { return (byte)GeAttachBackCmdType.Pause_Animation; } }
        public static byte CatalogType { get { return (byte)GB_CATALOG.ATTACH; } }
        public static AttachPauseAniGBCommand Acquire()
        {
            return GBCommandPoolSystem.GetInstance().Allocate(CatalogType, CommandType) as AttachPauseAniGBCommand;
        }
    }
}
//挂件命令集处理器
public sealed class AttachGraphicBack : GraphicBackController
{
    public UnityEngine.GameObject parentNode = null;   //挂件所需的父节点，在恢复到正常模式下需要
    public override GB_CATALOG CataLog() { return GB_CATALOG.ATTACH; }
    public static AttachGraphicBack Acquire() { return GBControllerAllocator.GetInstance().Allocate((byte)GB_CATALOG.ATTACH) as AttachGraphicBack; }
    public AttachGraphicBack()
    {
        consecutiveCodes = new int[] { (int)(GeAttachBackCmdType.Create), (int)(GeAttachBackCmdType.Change_Phase),
                                        (int)GeAttachBackCmdType.Visible_Enable ,(int)GeAttachBackCmdType.Layer_Set,(int)GeAttachBackCmdType.Play_Action,
                                        (int)GeAttachBackCmdType.Resume_Animation ,(int)GeAttachBackCmdType.Pause_Animation,(int)GeAttachBackCmdType.Stop_Action};

    }
    public override void Clear()
    {
        base.Clear();
        parentNode = null;

    }
    public override void OnRecycle()
    {
        parentNode = null;
    }
    public override void FlipToFront()
    {
        //以当前的设计思路是已经创建的 附件，不会走resume 这个流程，所以如果没有有create 那么 resume没有任何意义
        var cmd = Get((int)(GeAttachBackCmdType.Create)) as GeAttach.AttachCreateGBCommand;
        if (cmd != null)
        {
            cmd.parent = parentNode;
            base.FlipToFront();
        }
        else
        {
            Recycle();
        }

    }
    public override void RecordCmd(int type, IGBCommand cmd)
    {
        switch ((GeAttachBackCmdType)type)
        {
            //挂件通用命令集处理流程，和entity不一样的地方，恢复动画不要要计算动画偏移，因为挂件需要访问的主体的动画偏移
            case GeAttachBackCmdType.Create:
            case GeAttachBackCmdType.Visible_Enable:
            case GeAttachBackCmdType.Layer_Set:
            case GeAttachBackCmdType.Change_Phase:
                {
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
            //停止播放动作的时候，需要查询在命令集中是否存在播放动作的命令，这个时候需要计算播放动作命令已经经过的时长
            case GeAttachBackCmdType.Stop_Action:
                {
                    var stopCmd = cmd as GeAttach.AttachStopActionGBCommand;
                    if (stopCmd == null)
                    {
                        Logger.LogError("Input Attach Stop_Action is not right cmd");
                        return;
                    }
                    if (!mCmdList.ContainsKey((int)GeAttachBackCmdType.Play_Action))
                    {
                        return;
                    }
                    var changeActionCmd = mCmdList[(int)GeAttachBackCmdType.Play_Action] as GeAttach.AttachPlayActionGBCommand;
                    if (changeActionCmd == null)
                    {
                        Logger.LogError("Input Attach Stop_Action changeActionCmd is not right cmd");
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
            case GeAttachBackCmdType.Pause_Animation:
                {
                    var pauseCmd = cmd as GeAttach.AttachPauseAniGBCommand;
                    if (pauseCmd == null)
                    {
                        Logger.LogError("Input Attach Pause_Animation is not right cmd");
                        return;
                    }
                    if (!mCmdList.ContainsKey((int)GeAttachBackCmdType.Play_Action))
                    {
                        return;
                    }
                    var changeActionCmd = mCmdList[(int)GeAttachBackCmdType.Play_Action] as GeAttach.AttachPlayActionGBCommand;
                    if (changeActionCmd == null)
                    {
                        Logger.LogError("Input Attach Pause_Animation changeActionCmd is not right cmd");
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
            case GeAttachBackCmdType.Resume_Animation:
                {
                    var resumeCmd = cmd as GeAttach.AttachResumeAniGBCommand;
                    if (resumeCmd == null)
                    {
                        Logger.LogError("Input Resume_Animation is not right cmd");
                        return;
                    }
                    if (!mCmdList.ContainsKey((int)GeAttachBackCmdType.Play_Action))
                    {
                        return;
                    }
                    var changeActionCmd = mCmdList[(int)GeAttachBackCmdType.Play_Action] as GeAttach.AttachPlayActionGBCommand;
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
                        if (mCmdList.ContainsKey((int)GeAttachBackCmdType.Pause_Animation))
                        {
                            mCmdList[(int)GeAttachBackCmdType.Pause_Animation].Recycle();
                            mCmdList.Remove((int)GeAttachBackCmdType.Pause_Animation);
                        }
                        else
                        {
                            Logger.LogErrorFormat("GeAttachBackCmdType Resume_Animation has not right pause animation {0}", changeActionCmd.attach != null ? changeActionCmd.attach.Name : "ERROR:" + changeActionCmd.name);
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
            case GeAttachBackCmdType.Play_Action:
                {
                    if (mCmdList.ContainsKey((int)GeAttachBackCmdType.Stop_Action))
                    {
                        mCmdList[(int)GeAttachBackCmdType.Stop_Action].Recycle();
                        mCmdList.Remove((int)GeAttachBackCmdType.Stop_Action);
                    }
                    if (mCmdList.ContainsKey((int)GeAttachBackCmdType.Pause_Animation))
                    {
                        mCmdList[(int)GeAttachBackCmdType.Pause_Animation].Recycle();
                        mCmdList.Remove((int)GeAttachBackCmdType.Pause_Animation);
                    }
                    if (mCmdList.ContainsKey((int)GeAttachBackCmdType.Stop_Action))
                    {
                        mCmdList[(int)GeAttachBackCmdType.Stop_Action].Recycle();
                        mCmdList.Remove((int)GeAttachBackCmdType.Stop_Action);
                    }
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
            case GeAttachBackCmdType.Destroy:
                {
                    Recycle();
                }
                break;
        }
    }
#endif
}
