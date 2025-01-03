using System;
using System.Collections.Generic;
///////删除linq
using System.Text;

namespace GameClient
{
    /*
    界面管理，新加功能不要改变已有的功能
    已实现功能：
    1.界面对象预先创建，调用初始化（初始化里面可以注册打开界面的消息，这样就可以通过消息来打开界面，解耦）
    2.ItemTip这样的界面，会同时存在两个及以上的frame实例（GameObject），这种情况，需要重载加载预制体的代码，创建多份实例，进行管理
    */
    class FrameManager : Singleton<FrameManager>
    {
        List<ClientFrame> m_arrFrames = new List<ClientFrame>();

        // 不用反射查询所有类的方式，太费了，直接硬编码
        public override void Init()
        {
            //_RegisterFrame(new GuildBattleFrame());
        }

        public override void UnInit()
        {
            for (int i = 0; i < m_arrFrames.Count; ++i)
            {
                m_arrFrames[i].Clear();
            }
            m_arrFrames.Clear();
        }

        void _RegisterFrame(ClientFrame a_frame)
        {
            if (a_frame != null)
            {
                a_frame.Init();
                m_arrFrames.Add(a_frame);
            }
        }
    }

    
}
