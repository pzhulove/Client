//Created Time : 2020-7-27
//Created By Shensi
//Description:
//材质相关命令集
public enum AnimatBackCmdType
{
    Push = 0,
    Remove,
    Clear,
    MAX_COUNT
}

public partial class GeAnimatManagerEx
{
#if !LOGIC_SERVER
    //创建材质指令
    public sealed class PushAnimatGBCommand : IGBCommand
    {
        public string name;
        public float timeLen;
        public bool enableAnim;
        public bool needRecover;
        public GeAnimatManagerEx _this;
        public long timeStamp;
        public uint handle;
        public override bool Resume()
        {
            float deltaTime = (FrameSync.GetTicksNow() - timeStamp) / 1000.0f;
            if (timeLen <= 0 || deltaTime < timeLen)
            {
                _this.m_AnimatStack.Add(new GeAnimatCacheEx(name, handle, timeLen <= 0 ? timeLen : timeLen - deltaTime, enableAnim, needRecover));
                _this._ApplyAnimat(name, timeLen, enableAnim);
                return true;
            }

            return true;
        }
        public override void OnRecycle()
        {
            name = string.Empty;
            timeLen = 0;
            enableAnim = false;
            needRecover = false;
            timeStamp = 0;
            _this = null;
            handle = uint.MaxValue;
        }
        public override byte CmdType()
        {
            return CommandType;
        }
        public override byte Catalog()
        {
            return CatalogType;
        }
        public static byte CommandType { get { return (byte)AnimatBackCmdType.Push; } }
        public static byte CatalogType { get { return (byte)GB_CATALOG.MATERIAL; } }
        public static PushAnimatGBCommand Acquire() { return GBCommandPoolSystem.GetInstance().Allocate(CatalogType, CommandType) as PushAnimatGBCommand; }
    }
    //删除材质指令
    public sealed class RemoveAnimatGBCommand : IGBCommand //该命令似乎没有作用，因为只需要寻找是否push了指令然后删除就可以了
    {
        public GeAnimatManagerEx _this;
        public uint handle;
        public override bool Resume()
        {
            _this.RemoveAnimat(handle);
            return true;
        }
        public override void OnRecycle()
        {
            _this = null;
            handle = uint.MaxValue;
        }
        public override byte CmdType()
        {
            return CommandType;
        }
        public override byte Catalog()
        {
            return CatalogType;
        }
        public static byte CommandType { get { return (byte)AnimatBackCmdType.Remove; } }
        public static byte CatalogType { get { return (byte)GB_CATALOG.MATERIAL; } }
        public static RemoveAnimatGBCommand Acquire() { return GBCommandPoolSystem.GetInstance().Allocate(CatalogType,CommandType) as RemoveAnimatGBCommand; }
    }
}
//材质指令集处理器
public sealed class AnimatGraphicBack : GraphicBackController
{
    public AnimatGraphicBack()
    {
      
    }
    public static AnimatGraphicBack Acquire() { return GBControllerAllocator.GetInstance().Allocate((byte)GB_CATALOG.MATERIAL) as AnimatGraphicBack; }
    public override void OnRecycle()
    {
        
    }
    public override GB_CATALOG CataLog() { return GB_CATALOG.MATERIAL; }
    public override void RecordCmd(int type, IGBCommand cmd)
    {
        switch ((AnimatBackCmdType)type)
        {
            case AnimatBackCmdType.Push:
                {
                    var curCmd = cmd as GeAnimatManagerEx.PushAnimatGBCommand;
                    if (curCmd != null)
                    {
                        mCmdList.Add((int)curCmd.handle, curCmd);
                    }
                }
                break;
            case AnimatBackCmdType.Remove:
                {
                    var curCmd = cmd as GeAnimatManagerEx.RemoveAnimatGBCommand;
                    if (curCmd != null )
                    {
                        if (mCmdList.ContainsKey((int)curCmd.handle))
                        {
                            mCmdList[(int)curCmd.handle].Recycle();
                            mCmdList.Remove((int)curCmd.handle);
                        }
                        curCmd.Recycle();
                    }
                }
                break;
            case AnimatBackCmdType.Clear:
                {
                    Recycle();
                }
                break;
        }
    }
#endif
}


