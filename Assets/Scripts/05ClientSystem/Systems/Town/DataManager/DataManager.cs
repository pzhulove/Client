using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using System.Collections.Generic;
using System.Reflection;
using System;
using System.Runtime.InteropServices;
using Protocol;
using Network;

namespace GameClient
{
    public abstract class DataManager<T> : GameBindSystem, IDataManager where T : DataManager<T>, new()
    {
        private static T ms_instance;
        private static bool ms_bCreated = false;

        protected List<EnterGameBinding> m_arrEnterGameBindings = new List<EnterGameBinding>();
        protected bool m_bBindInited = false;

        protected DataManager()
        {
            if (ms_bCreated)
            {
                Logger.LogErrorFormat("{0} can not create twice!!", typeof(T).Name);
            }
            ms_bCreated = true;
        }

        public virtual EEnterGameOrder GetOrder()
        {
            return EEnterGameOrder.Default;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public abstract void Initialize();

        /// <summary>
        /// 清理
        /// </summary>
        public abstract void Clear();

        public virtual void Update(float a_fTime)
        {

        }

 
        public static T GetInstance()
        {
            if (DataManager<T>.ms_instance == null)
            {
                DataManager<T>.ms_instance = Activator.CreateInstance<T>();
 #if !SERVER_LOGIC 

               //DataManager<T>.ms_instance.InitUIBinding();

 #endif

                Logger.LogProcessFormat("Create {0}", typeof(T).Name);
            }
            return DataManager<T>.ms_instance;
        }

        public void InitiallizeSystem()
        {
            ClearAll();
            Initialize();
            //InitBindSystem(null);
        }

        public void ProcessInitNetMessage(WaitNetMessageManager.NetMessages a_msgEvent)
        {
            _SetupEnterGameData(a_msgEvent);
        }
        
        /* 
        public void InitializeAll(WaitNetMessageManager.NetMessages a_msgEvent)
        {
            ClearAll();

            Initialize();
            InitBindSystem(null);
            
            _SetupEnterGameData(a_msgEvent);
        }
        */

        public void ClearAll()
        {
            Clear();
            ExistBindSystem();
        }

        public void BindEnterGameMsg(List<uint> a_msgEvent)
        {
            _InitEnterGameBind();

            for (int i = 0; i < m_arrEnterGameBindings.Count; ++i)
            {
                a_msgEvent.Add(m_arrEnterGameBindings[i].id);
            }
        }

        /// <summary>
        /// EnterSystem
        /// </summary>
        public virtual void OnEnterSystem()
        {

        }
        /// <summary>
        /// ExitSystem
        /// </summary>
        public virtual void OnExitSystem()
        {

        }
        /// <summary>
        /// OnApplicationQuit 当应用程序退出的时侯做一些保存工作
        /// </summary>
        public virtual void OnApplicationQuit()
        {

        }
        /// <summary>
        /// OnApplicationQuit 当应用程序启动的时侯加载配置文件
        /// </summary>
        public virtual void OnApplicationStart()
        {

        }

        void _SetupEnterGameData(WaitNetMessageManager.NetMessages a_msgEvent)
        {
            for (int i = 0; i < m_arrEnterGameBindings.Count; ++i)
            {
                EnterGameBinding eb = m_arrEnterGameBindings[i];
                List<MsgDATA> arrData = a_msgEvent.GetMessageDatas(eb.id);
                if (arrData != null)
                {
                    Logger.LogProcessFormat("{2}根据消息{0}(ID:{1})的网络数据初始化...",
                        ProtocolHelper.instance.GetName(eb.id), eb.id, typeof(T).Name);
                    Action<MsgDATA> action = eb.method as Action<MsgDATA>;
                    if (action != null)
                    {
                        for (int j = 0; j < arrData.Count; ++j)
                        {
                            action(arrData[j]);
                        }
                    }
                }
                else
                {
                    Logger.LogErrorFormat("{2}根据网络数据初始化，找不到消息{0}(ID:{1})的网络数据！！", 
                        ProtocolHelper.instance.GetName(eb.id), eb.id, typeof(T).Name);
                }
            }
        }

        public virtual void OnBindEnterGameMsg()
        {

        }

        void _InitEnterGameBind()
        {
            if (m_bBindInited)
            {
                return;
            }

            OnBindEnterGameMsg();

            /*
            MethodInfo[] methods = GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            for (int i = 0; i < methods.Length; ++i)
            {
                var current = methods[i];
                object[] oats = current.GetCustomAttributes(typeof(EnterGameMessageHandleAttribute), false);
                if (oats.Length > 0)
                {
                    EnterGameBinding eb = new EnterGameBinding();
                    eb.id = (oats[0] as EnterGameMessageHandleAttribute).id;
                    eb.order = (oats[0] as EnterGameMessageHandleAttribute).order;

                    try
                    {
                        eb.method = Delegate.CreateDelegate(typeof(Action<MsgDATA>),
                               this, current, true);
                    }
                    catch (Exception e)
                    {
                        Logger.LogErrorFormat("错误!! 绑定消息{0}(ID:{1})到方法{2}", ProtocolHelper.instance.GetName(eb.id), eb.id, current.ToString());
                    }

                    bool bAdded = false;
                    for (int j = 0; j < m_arrEnterGameBindings.Count; ++j)
                    {
                        if (eb.order < m_arrEnterGameBindings[j].order)
                        {
                            m_arrEnterGameBindings.Insert(j, eb);
                            bAdded = true;
                            break;
                        }
                    }
                    if (bAdded == false)
                    {
                        m_arrEnterGameBindings.Add(eb);
                    }
                }
            }*/



            m_bBindInited = true;
        }
    }
}