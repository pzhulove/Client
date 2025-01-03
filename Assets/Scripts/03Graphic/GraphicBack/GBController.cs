using System.Collections.Generic;
//Created Time : 2020-7-27
//Created By Shensi
//Description:
//用于处理在追帧时候压入函数命令帧的时候的通用处理
public enum GB_CATALOG   //处理类别
{
    ENTITY = 0,          //实体或者人物，怪物
    ATTACH,              //挂件
    EFFECT,              //特效
    MATERIAL,            //材质
    MAX_COUNT
};
#if !LOGIC_SERVER
//通用命令帧处理器
public abstract class GraphicBackController
{
    protected Dictionary<int, IGBCommand> mCmdList = new Dictionary<int, IGBCommand>();  //压入的命令帧
    protected int[] consecutiveCodes = null;       //不同种类的命令帧，执行顺序,如果不需要则不用赋值，按照默认规则执行
    private bool m_isValid = false;                //是否正在使用中 用于标记是否处于池子的状态     
    public bool isValid() { return m_isValid; }
    public GraphicBackController()
    {

    }
    bool IsEmpty() { return mCmdList.Count <= 0; }
    //处理器类别
    public abstract GB_CATALOG CataLog();
    //追帧时将命令帧放入命令集保存
    public abstract void RecordCmd(int cmdType,IGBCommand cmd);
    //从池子里面取出来的回调
    public virtual void OnCreate()                             
    {
        if (m_isValid)
        {
            Logger.LogErrorFormat("Already used Controller {0}", CataLog());
            return;
        }
        m_isValid = true;
    }
    //放入池子中       
    public virtual void Release()                             
    {
        if (isValid())
        {
            m_isValid = false;
            GBControllerAllocator.GetInstance().Free(this);
        }
        else
        {
            Logger.LogErrorFormat("Repeat recycle controller {0}", CataLog());
        }
    }
    //执行命令帧
    protected void _executeCmd()
    {
        if (consecutiveCodes == null)
        {
            var iter = mCmdList.GetEnumerator();
            while (iter.MoveNext())
            {
                bool result = iter.Current.Value.Resume();
                if (!result)
                {
                    break;
                }
            }
        }
        else
        {
            for (int i = 0; i < consecutiveCodes.Length; i++)
            {
                int curCode = consecutiveCodes[i];
                if (mCmdList.ContainsKey(curCode))
                {
                    bool result = mCmdList[curCode].Resume();
                    if (!result)
                    {
                        break;
                    }
                }
            }
        }
    }
    //放入池子中的回调
    public abstract void OnRecycle();
    //从追帧模式变成普通模式后的处理，将命令帧集合取出来执行
    public virtual void FlipToFront()
    {
        _executeCmd();
        Recycle();
    }
    //获得某种类型的命令帧
    public IGBCommand Get(int key)
    {
        if (mCmdList.ContainsKey(key))
        {
            return mCmdList[key];
        }
        return null;
    }
    //清空 （不放入池子）
    public virtual void Clear()
    {
        mCmdList.Clear();
    }
    //回收命令帧到池子中去
    public virtual void Recycle()
    {
        var iter = mCmdList.GetEnumerator();
        while(iter.MoveNext())
        {
            var curCmd = iter.Current.Value;
            if(curCmd != null)
                curCmd.Recycle();
        }
        mCmdList.Clear();
    }
}
#endif