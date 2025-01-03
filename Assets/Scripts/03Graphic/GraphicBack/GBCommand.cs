//Created Time : 2020-7-27
//Created By Shensi
//Description:
//追帧时依据游戏逻辑产生的命令帧
public abstract class IGBCommand
{
    private bool m_isValid = false;             //是否不在池子中
    public bool isValid() { return m_isValid; }
    //从池子中取出来使用的回调
    public virtual void OnCreate()
    {
        if(m_isValid)
        {
            Logger.LogErrorFormat("Already used cmd {0} {1}",CmdType(),Catalog());
            return;
        }
        m_isValid  = true;
    }
    //从追帧模式恢复到正常模式执行缓存的命令帧
    public abstract bool Resume();
    //命令帧类型
    public abstract byte CmdType();
    //命令帧所涉及的处理器类别
    public abstract byte Catalog();
    //命令帧放入池子中的回调
    public abstract void OnRecycle();
    //命令帧放入池子
    public void Recycle()
    {
#if !LOGIC_SERVER
        if (isValid())
        {
            m_isValid = false;
            GBCommandPoolSystem.GetInstance().Free(this);
        }
        else
        {
            Logger.LogErrorFormat("Repeat recycle {0} {1}",CmdType(),Catalog());
        }
#endif
    }
};

