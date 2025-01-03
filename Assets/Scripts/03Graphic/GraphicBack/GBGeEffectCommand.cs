using UnityEngine;
//Created Time : 2020-7-27
//Created By Shensi
//Description:
//特效相关命令集已经对应的处理器
public enum GeEffectBackCmdType
{
    Create = 0,
    Destroy,
    Pause_Animation,
    Resume_Animation,
    Play_Animation,
    Layer,
    Position,
    Local_Position,
    Speed,
    Time_Len,
    Reset_Elapse_Time,
    Scale,
    Scale_Effect,
    Touch_Ground,
    Visible,
    Rotation,
    MAX_COUNT
}


public partial class GeEffectEx
{
#if !LOGIC_SERVER
    //特效创建命令
    public sealed class CreateEffectGBCommand : IGBCommand
    {
        public string strResName;
        public EffectsFrames info;
        public float time;
        public Vector3 initPos;
        public bool bFaceLeft;
        public GameObject parentObj;
        public GeEntity owner = null;
        public string attachNode = string.Empty;
        public bool useCube;
        public GeEffectEx _this;
        public bool isPause = false;
        public float elapseTime = 0.0f;
        public uint frame = 0;
        public long timeStamp;
        public override void OnRecycle()
        {
              strResName = string.Empty;
              info = null;
              time = 0.0f;
              initPos = Vector3.zero;
              bFaceLeft = false;
              parentObj = null;
              owner = null;
              attachNode = string.Empty;
              useCube = false;
              _this = null;
              isPause = false;
              elapseTime = 0.0f;
              frame = 0;
              timeStamp = 0;
        }
        public override bool Resume()
        {
            float deltaTime = FrameSync.GetTicksNow() - timeStamp;
            if (!_this.IsDead() /*&& deltaTime < time*/)
            {
                GameObject.Destroy(_this.m_EffectSpace.m_RootNode);
                GameObject.Destroy(_this.m_EffectSpace.m_EffectNode);
                _this.m_EffectSpace.m_RootNode = null;
                _this.m_EffectSpace.m_EffectNode = null;
                bool bResult = false;
                if (owner != null)
                {
                    if (!owner.CanRemove())
                    {
                        var parentNode = owner.GetAttachNode(attachNode);
                        if (isPause)
                        {
                            bResult = _this.Init(strResName, info, time, initPos, bFaceLeft, parentNode, useCube);
                            _this.m_ElapsedTimeMS = (uint)(deltaTime + elapseTime);

                        }
                        else
                        {
                            bResult = _this.Init(strResName, info, time, initPos, bFaceLeft, parentNode, useCube);
                            _this.m_ElapsedTimeMS = (uint)(deltaTime);
                        }
                        _this.createdInBackMode = false;
                        if (!bResult)
                        {
                            _this.Deinit();
                            _this.Remove();
                            return false;
                        }
                        return true;
                    }
                }
                else
                {
                    if (isPause)
                    {
                        bResult = _this.Init(strResName, info, time - deltaTime - elapseTime, initPos, bFaceLeft, parentObj, useCube);
                        _this.m_ElapsedTimeMS = (uint)(deltaTime + elapseTime);
                    }
                    else
                    {
                        bResult = _this.Init(strResName, info, time - deltaTime, initPos, bFaceLeft, parentObj, useCube);
                        _this.m_ElapsedTimeMS = (uint)(deltaTime);
                    }
                    _this.createdInBackMode = false;
                    if (!bResult)
                    {
                        _this.Deinit();
                        _this.Remove();
                        return false;
                    }
                    return true;
                }
            }
            return false;
        }
        public override byte CmdType()
        {
            return CommandType;
        }
        public override byte Catalog()
        {
            return CatalogType;
        }
        public static byte CommandType { get { return (byte)GeEffectBackCmdType.Create; } }
        public static byte CatalogType { get { return (byte)GB_CATALOG.EFFECT; } }
        public static CreateEffectGBCommand Acquire() { return GBCommandPoolSystem.GetInstance().Allocate(CatalogType,CommandType) as CreateEffectGBCommand; }
    }

    //设置特效位置指令
    public sealed class PositionEffectGBCommand : IGBCommand
    {
        public GeEffectEx _this;
        public Vector3 pos;
        public override bool Resume()
        {
            _this.SetPosition(pos);
            return true;
        }
        public override byte CmdType()
        {
            return CommandType;
        }
        public override byte Catalog()
        {
            return CatalogType;
        }
        public override void OnRecycle()
        {
            pos = Vector3.zero;
            _this = null;
        }
        public static byte CommandType { get { return (byte)GeEffectBackCmdType.Position; } }
        public static byte CatalogType { get { return (byte)GB_CATALOG.EFFECT; } }
        public static PositionEffectGBCommand Acquire() { return GBCommandPoolSystem.GetInstance().Allocate(CatalogType, CommandType) as PositionEffectGBCommand; }
    }
    //纯设置特效缩放指令，不改变特效大小
    public sealed class ScaleEffectGBCommand : IGBCommand
    {
        public Vector3 scale;
        public GeEffectEx _this;
        public override bool Resume()
        {
            _this.SetScale(scale.x, scale.y, scale.z);
            return true;
        }
        public override byte CmdType()
        {
            return CommandType;
        }
        public override byte Catalog()
        {
            return CatalogType;
        }
        public override void OnRecycle()
        {
            scale = Vector3.one;
            _this = null;
        }
        public static byte CommandType { get { return (byte)GeEffectBackCmdType.Scale; } }
        public static byte CatalogType { get { return (byte)GB_CATALOG.EFFECT; } }
        public static ScaleEffectGBCommand Acquire() { return GBCommandPoolSystem.GetInstance().Allocate(CatalogType, CommandType) as ScaleEffectGBCommand; }
    }
    //改变特效大小，改指令在恢复执行的之前，要把ScaleEffectGBCommand合成为这一个指令
    public sealed class LocalScaleEffectGBCommand : IGBCommand
    {
        public float scale;
        public Vector3 vecScale;
        public GeEffectEx _this;
        public override bool Resume()
        {
            _this.SetScale(scale);
            _this.SetScale(vecScale.x, vecScale.y, vecScale.z);
            return true;
        }
        public override byte CmdType()
        {
            return CommandType;
        }
        public override byte Catalog()
        {
            return CatalogType;
        }
        public override void OnRecycle()
        {
            vecScale = Vector3.one;
            scale = 1.0f;
            _this = null;
        }
        public static byte CommandType { get { return (byte)GeEffectBackCmdType.Scale_Effect; } }
        public static byte CatalogType { get { return (byte)GB_CATALOG.EFFECT; } }
        public static LocalScaleEffectGBCommand Acquire() { return GBCommandPoolSystem.GetInstance().Allocate(CatalogType, CommandType) as LocalScaleEffectGBCommand; }
    }
    //设置本地坐标
    public sealed class LocalPositionEffectGBCommand : IGBCommand
    {
        public GeEffectEx _this;
        public Vector3 pos;
        public override bool Resume()
        {
            _this.SetLocalPosition(pos);
            return true;
        }
        public override byte CmdType()
        {
            return CommandType;
        }
        public override byte Catalog()
        {
            return CatalogType;
        }
        public override void OnRecycle()
        {
            pos = Vector3.zero;
            _this = null;
        }
        public static byte CommandType { get { return (byte)GeEffectBackCmdType.Local_Position; } }
        public static byte CatalogType { get { return (byte)GB_CATALOG.EFFECT; } }
        public static LocalPositionEffectGBCommand Acquire() { return GBCommandPoolSystem.GetInstance().Allocate(CatalogType, CommandType) as LocalPositionEffectGBCommand; }
    }
    //设置特效旋转指令
    public sealed class RotationEffectGBCommand : IGBCommand
    {
        public GeEffectEx _this;
        public Quaternion rot;
        public override bool Resume()
        {
            _this.SetLocalRotation(rot);
            return true;
        }
        public override byte CmdType()
        {
            return CommandType;
        }
        public override byte Catalog()
        {
            return CatalogType;
        }
        public override void OnRecycle()
        {
            rot = Quaternion.identity;
            _this = null;
        }
        public static byte CommandType { get { return (byte)GeEffectBackCmdType.Rotation; } }
        public static byte CatalogType { get { return (byte)GB_CATALOG.EFFECT; } }
        public static RotationEffectGBCommand Acquire() { return GBCommandPoolSystem.GetInstance().Allocate(CatalogType, CommandType) as RotationEffectGBCommand; }
    }
    //暂停特效命令
    public sealed class PauseEffectAnimationGBCommand : IGBCommand
    {
        public GeEffectEx _this;
        public long timeStamp;
        public override bool Resume()
        {
            _this.Pause();
            return true;
        }
        public override void OnRecycle()
        {
            _this = null;
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
        public static byte CommandType { get { return (byte)GeEffectBackCmdType.Pause_Animation; } }
        public static byte CatalogType { get { return (byte)GB_CATALOG.EFFECT; } }
        public static PauseEffectAnimationGBCommand Acquire() {return GBCommandPoolSystem.GetInstance().Allocate(CatalogType,CommandType) as PauseEffectAnimationGBCommand; }
    }
    //恢复播放特效命令
    public sealed class ResumeEffectAnimationGBCommand : IGBCommand
    {
        public GeEffectEx _this;
        public long timeStamp;
        public override void OnRecycle()
        {
            _this = null;
            timeStamp = 0;
        }
        public override bool Resume()
        {
            _this.Resume();
            return true;
        }
        public override byte CmdType()
        {
            return CommandType;
        }
        public override byte Catalog()
        {
            return CatalogType;
        }
        public static byte CommandType { get { return (byte)GeEffectBackCmdType.Resume_Animation; } }
        public static byte CatalogType { get { return (byte)GB_CATALOG.EFFECT; } }
        public static ResumeEffectAnimationGBCommand Acquire() { return GBCommandPoolSystem.GetInstance().Allocate(CatalogType,CommandType) as ResumeEffectAnimationGBCommand; }
    }
     //设置特效层级命令
    public sealed class LayerEffectGBCommand : IGBCommand
    {
        public GeEffectEx _this;
        public int layer;
        public override bool Resume()
        {
            _this.SetLayer(layer);
            return true;
        }
        public override void OnRecycle()
        {
            _this = null;
            layer = 0;
        }
        public override byte CmdType()
        {
            return CommandType;
        }
        public override byte Catalog()
        {
            return CatalogType;
        }
        public static byte CommandType { get { return (byte)GeEffectBackCmdType.Layer; } }
        public static byte CatalogType { get { return (byte)GB_CATALOG.EFFECT; } }
        public static LayerEffectGBCommand Acquire() { return GBCommandPoolSystem.GetInstance().Allocate(CatalogType, CommandType) as LayerEffectGBCommand; }
    }
    //设置特效播放速度指令
    public sealed class SpeedEffectGBCommand : IGBCommand
    {
        public GeEffectEx _this;
        public float fSpeed;
        public override bool Resume()
        {
            _this.SetSpeed(fSpeed);
            return true;
        }
        public override void OnRecycle()
        {
            _this = null;
            fSpeed = 0.0f;
        }
        public override byte CmdType()
        {
            return CommandType;
        }
        public override byte Catalog()
        {
            return CatalogType;
        }
        public static byte CommandType { get { return (byte)GeEffectBackCmdType.Speed; } }
        public static byte CatalogType { get { return (byte)GB_CATALOG.EFFECT; } }
        public static SpeedEffectGBCommand Acquire() { return GBCommandPoolSystem.GetInstance().Allocate(CatalogType, CommandType) as SpeedEffectGBCommand; }
    }
    //设置特效时间长度指令
    public sealed class TimeLenEffectGBComand : IGBCommand
    {
        public int timeLen;
        public GeEffectEx _this;
        public override bool Resume()
        {
            _this.SetTimeLen(timeLen);
            return true;
        }
        public override void OnRecycle()
        {
            timeLen = 0;
            _this = null;
        }
        public override byte CmdType()
        {
            return CommandType;
        }
        public override byte Catalog()
        {
            return CatalogType;
        }
        public static byte CommandType { get { return (byte)GeEffectBackCmdType.Time_Len; } }
        public static byte CatalogType { get { return (byte)GB_CATALOG.EFFECT; } }
        public static TimeLenEffectGBComand Acquire() { return GBCommandPoolSystem.GetInstance().Allocate(CatalogType, CommandType) as TimeLenEffectGBComand; }
    }
    //重置特效时间指令
    public sealed class ResetElapseTimeEffectGBCommand : IGBCommand
    {
        public GeEffectEx _this;
        public long timeStamp;
        public override bool Resume()
        {
            _this.ResetElapsedTime();
            return true;
        }
        public override void OnRecycle()
        {
            _this = null;
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
        public static byte CommandType { get { return (byte)GeEffectBackCmdType.Reset_Elapse_Time; } }
        public static byte CatalogType { get { return (byte)GB_CATALOG.EFFECT; } }
        public static ResetElapseTimeEffectGBCommand Acquire() { return GBCommandPoolSystem.GetInstance().Allocate(CatalogType, CommandType) as ResetElapseTimeEffectGBCommand; }
    }
    //播放特效动画
    public sealed class PlayAnimationEffectGBCommand : IGBCommand
    {
        public GeEffectEx _this;
        public bool isLoop;
        public long timeStamp;
        public override void OnRecycle()
        {
            _this = null;
            isLoop = false;
            timeStamp = 0;
        }
        public override bool Resume()
        {
            _this.Play(isLoop);
            return true;
        }
        public override byte CmdType()
        {
            return CommandType;
        }
        public override byte Catalog()
        {
            return CatalogType;
        }
        public static byte CommandType { get { return (byte)GeEffectBackCmdType.Play_Animation; } }
        public static byte CatalogType { get { return (byte)GB_CATALOG.EFFECT; } }
        public static PlayAnimationEffectGBCommand Acquire() { return GBCommandPoolSystem.GetInstance().Allocate(CatalogType, CommandType) as PlayAnimationEffectGBCommand; }
    }
    //设置动画显隐指令
    public sealed class VisibleEffectGBCommand : IGBCommand
    {
        public GeEffectEx _this;
        public bool isVisible;
        public override bool Resume()
        {
            _this.SetVisible(isVisible);
            return true;
        }
        public override void OnRecycle()
        {
            _this = null;
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
        public static byte CommandType { get { return (byte)GeEffectBackCmdType.Visible; } }
        public static byte CatalogType { get { return (byte)GB_CATALOG.EFFECT; } }
        public static VisibleEffectGBCommand Acquire() { return GBCommandPoolSystem.GetInstance().Allocate(CatalogType, CommandType) as VisibleEffectGBCommand; }
    }
#endif
}
#if !LOGIC_SERVER
//特效命令集处理器
public sealed class EffectGraphicBack : GraphicBackController
{
    public EffectGraphicBack()
    {
        consecutiveCodes = new int[] { (int)(GeEffectBackCmdType.Create), (int)(GeEffectBackCmdType.Layer),
                                        (int)GeEffectBackCmdType.Position ,(int)GeEffectBackCmdType.Local_Position,(int)GeEffectBackCmdType.Rotation,(int)GeEffectBackCmdType.Speed,
                                       (int)GeEffectBackCmdType.Scale,(int)GeEffectBackCmdType.Scale_Effect,(int)GeEffectBackCmdType.Touch_Ground,(int)GeEffectBackCmdType.Visible,
                                        (int)GeEffectBackCmdType.Time_Len,(int)GeEffectBackCmdType.Reset_Elapse_Time,
                                        (int)GeEffectBackCmdType.Play_Animation, (int)GeEffectBackCmdType.Resume_Animation,(int)GeEffectBackCmdType.Pause_Animation,};
    }
    public static EffectGraphicBack Acquire() { return GBControllerAllocator.GetInstance().Allocate((byte)GB_CATALOG.EFFECT) as EffectGraphicBack; }
    public override void OnRecycle()
    {

    }
    public override GB_CATALOG CataLog() { return GB_CATALOG.EFFECT; }
    public override void RecordCmd(int type, IGBCommand cmd)
    {
        switch ((GeEffectBackCmdType)type)
        {
            //一般情况下，这几类命令类型，都是替换关系，没有特别额外的处理
            case GeEffectBackCmdType.Create:
            case GeEffectBackCmdType.Layer:
            case GeEffectBackCmdType.Position:
            case GeEffectBackCmdType.Local_Position:
            case GeEffectBackCmdType.Rotation:
            case GeEffectBackCmdType.Speed:
            case GeEffectBackCmdType.Visible:
            case GeEffectBackCmdType.Touch_Ground:
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
            case GeEffectBackCmdType.Scale:
                {
                    //只是改变缩放，先查找原来是否存在要改变特效实际缩放的指令，如果有，那么用这个指令替换改变缩放指令不放入指令集中
                    var scaleCmd = cmd as GeEffectEx.ScaleEffectGBCommand;
                    var localScaleCmd = Get((int)GeEffectBackCmdType.Scale_Effect) as GeEffectEx.LocalScaleEffectGBCommand;
                    if (scaleCmd != null && localScaleCmd != null)
                    {
                        localScaleCmd.vecScale = scaleCmd.scale;
                        scaleCmd.Recycle();
                    }
                    else
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
                }
                break;
            case GeEffectBackCmdType.Scale_Effect:
                {
                    //改变特效缩放，先查找换来是否存在只改变缩放的指令，如果那么用新指令替换旧指令，把旧指令删除不放入指令集中
                    var localScaleCmd = cmd as GeEffectEx.LocalScaleEffectGBCommand;
                    var scaleCmd = Get((int)GeEffectBackCmdType.Scale) as GeEffectEx.ScaleEffectGBCommand;
                    if (scaleCmd != null && localScaleCmd != null)
                    {
                        localScaleCmd.vecScale = scaleCmd.scale;
                        scaleCmd.Recycle();
                        mCmdList.Remove((int)GeEffectBackCmdType.Scale);
                    }
                    else
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
                }
                break;
            //暂停播放动作的时候，需要查询在命令集中是否存在创建特效的命令，这个时候需要计算特效生命周期已经经过的时长
            case GeEffectBackCmdType.Pause_Animation:
                {
                    var pauseCmd = cmd as GeEffectEx.PauseEffectAnimationGBCommand;
                    var createCmd = Get((int)GeEffectBackCmdType.Create) as GeEffectEx.CreateEffectGBCommand;
                    if (createCmd != null && !createCmd.isPause && pauseCmd != null)
                    {
                        createCmd.isPause = true;
                        createCmd.elapseTime += (pauseCmd.timeStamp - createCmd.timeStamp);
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
                }
                break;
            //恢复了动作则相当于将创建特效的时间戳重新计时，这样能够在恢复特效的时候能将动画偏移计算正确
            case GeEffectBackCmdType.Resume_Animation:
                {
                    var resumeCmd = cmd as GeEffectEx.ResumeEffectAnimationGBCommand;
                    var createCmd = Get((int)GeEffectBackCmdType.Create) as GeEffectEx.CreateEffectGBCommand;
                    if (createCmd != null && createCmd.isPause && resumeCmd != null)
                    {
                        createCmd.timeStamp = resumeCmd.timeStamp;
                        createCmd.isPause = false;
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
                }
                break;
            //重新设置创建特效的生命周期
            case GeEffectBackCmdType.Time_Len:
                {
                    var timeLenCmd = cmd as GeEffectEx.TimeLenEffectGBComand;
                    if (timeLenCmd != null)
                    {
                        if (mCmdList.ContainsKey((int)GeEffectBackCmdType.Create))
                        {
                            var createCmd = mCmdList[(int)GeEffectBackCmdType.Create] as GeEffectEx.CreateEffectGBCommand;
                            createCmd.time = timeLenCmd.timeLen;
                        }

                    }
                    cmd.Recycle();
                }
                break;
            //重置特效持续时间
            case GeEffectBackCmdType.Reset_Elapse_Time:
                {
                    var resetElapseCmd = cmd as GeEffectEx.ResetElapseTimeEffectGBCommand;
                    if (resetElapseCmd != null)
                    {
                        if (mCmdList.ContainsKey((int)GeEffectBackCmdType.Create))
                        {
                            var createCmd = mCmdList[(int)GeEffectBackCmdType.Create] as GeEffectEx.CreateEffectGBCommand;
                            createCmd.timeStamp = resetElapseCmd.timeStamp;
                            createCmd.elapseTime = 0.0f;
                        }
                    }
                    resetElapseCmd.Recycle();
                }
                break;
            case GeEffectBackCmdType.Destroy:
                {
                    Recycle();
                }
                break;
            case GeEffectBackCmdType.Play_Animation:
                {
                    var playCmd = cmd as GeEffectEx.PlayAnimationEffectGBCommand;
                    var createCmd = Get((int)GeEffectBackCmdType.Create) as GeEffectEx.CreateEffectGBCommand;
                    if (createCmd != null && createCmd.isPause && playCmd != null)
                    {
                        createCmd.timeStamp = playCmd.timeStamp;
                        createCmd.isPause = false;
                    }
                    if (mCmdList.ContainsKey((int)GeEffectBackCmdType.Pause_Animation))
                    {
                        mCmdList[(int)GeEffectBackCmdType.Pause_Animation].Recycle();
                        mCmdList.Remove((int)GeEffectBackCmdType.Pause_Animation);
                    }
                    if (mCmdList.ContainsKey((int)GeEffectBackCmdType.Resume_Animation))
                    {
                        mCmdList[(int)GeEffectBackCmdType.Resume_Animation].Recycle();
                        mCmdList.Remove((int)GeEffectBackCmdType.Resume_Animation);
                    }
                    if (mCmdList.ContainsKey((int)GeEffectBackCmdType.Play_Animation))
                    {
                        mCmdList[(int)GeEffectBackCmdType.Play_Animation].Recycle();
                        mCmdList[(int)GeEffectBackCmdType.Play_Animation] = cmd;
                    }
                    else
                    {
                        mCmdList.Add((int)GeEffectBackCmdType.Play_Animation, cmd);
                    }
                }
                break;

        }
    }

}
#endif