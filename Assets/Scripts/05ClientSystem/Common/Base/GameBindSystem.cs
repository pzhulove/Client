using System;
using System.Collections.Generic;
using System.Reflection;
using Network;
using UnityEngine;
using UnityEngine.UI;
using System.Text;


namespace  GameClient
{
    public class GameBindSystem
    {
        //protected DictionaryView<string, System.Object> contorlDics = new DictionaryView<string, object>();
        private GameObject mRoot;
        
        public class objectBindings
        {
            public string name;
            public FieldInfo field;
        }

        public class controlBindings
        {
            public string name;
            public FieldInfo field;
            public System.Type type;
            public bool bArray;
            public int baseNum;
        }

        public class eventBindings
        {
            public List<string> strList = new List<string>();
            public Type     controlType;
            public Type     eventType;
            public Delegate method;
        }

        public class msgBindings
        {
            public uint id;
            public Delegate method;
        }

        public class protoBindings
        {
            public uint id;
            public Action<MsgDATA> action;
        }

        public class soundBindings
        {
            public string name;
            public bool bNeed;
            public string sound;
        }

        protected List<objectBindings>      objectBindList  = new List<objectBindings>();
        protected List<controlBindings>     controlBindList = new List<controlBindings>();
        protected List<eventBindings>       eventBindList  = new List<eventBindings>();
        protected List<msgBindings>         msgBindList    = new List<msgBindings>();
        protected List<protoBindings>       protoBindList = new List<protoBindings>();
        protected List<soundBindings>       soundBindList = new List<soundBindings>();

        protected bool bBinding = false;
        
        protected void DeinitUIBinding()
        {
            objectBindList.RemoveAll(
                obj=>
                {
                    obj.field.SetValue(this, null);
                    return true;
                }
                );

            controlBindList.RemoveAll(
                obj=>
                {
                    if (obj.bArray)
                    {
                        Array arrCom = obj.field.GetValue(this) as Array;
                        for (int i = 0; i < arrCom.Length; ++i)
                        {
                            arrCom.SetValue(null, i);
                        }
                    }
                    else
                    {
                        obj.field.SetValue(this, null);
                    }
                    return true;
                }
                );

            eventBindList.Clear();
            msgBindList.Clear();
            protoBindList.Clear();
            soundBindList.Clear();
            //contorlDics.Clear();

            bBinding = false;
        }          
        
        protected void InitUIBinding()
        {
            if (bBinding)
            {
                return;
            }

            bBinding = true;

            

            System.Type type = this.GetType();
            FieldInfo[] fieldInfo = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
            if (fieldInfo == null)
            {
                return;
            }
            for (int i = 0; i < fieldInfo.Length; ++i)
            {
                var current = fieldInfo[i];
                if (current == null)
                {
                    continue;
                }
                object[] oats = current.GetCustomAttributes(typeof(UIObjectAttribute), false);
                if (oats == null)
                {
                    continue;
                }
                if (oats.Length > 0)
                {
                    objectBindings ob = new objectBindings();
                    ob.name = (oats[0] as UIObjectAttribute).objectName;
                    ob.field = current;
                    objectBindList.Add(ob);
                    continue;
                }

                oats = current.GetCustomAttributes(typeof(UIControlAttribute), false);
                if (oats == null)
                {
                    continue;
                }
                if (oats.Length > 0)
                {
                    var attr = oats[0] as UIControlAttribute;
                    controlBindings cb = new controlBindings();
                    cb.name = attr.controlName;
                    cb.field = current;
                    cb.type = attr.componentType;
                    cb.bArray = current.FieldType.IsArray;
                    cb.baseNum = attr.baseNum;
                    controlBindList.Add(cb);
                }
            }

            MethodInfo[] methods = type.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic);

            for (int i = 0; i < methods.Length; ++i)
            {
                var current = methods[i];

                //object[] oats = current.GetCustomAttributes(typeof(UIFrameSound), false);
                //if (oats.Length > 0)
                //{
                //    var frameSound = oats[0] as UIFrameSound;
                //    if (frameSound != null)
                //    {
                //        soundBindings sb = new soundBindings();
                //        sb.name = frameSound.name;
                //        sb.sound = frameSound.sound;
                //        sb.bNeed = frameSound.bNeed;
                //        soundBindList.Add(sb);
                //    }
                //}

                if (current == null)
                {
                    continue;
                }

                object[] oats = current.GetCustomAttributes(/*typeof(UIEventHandleAttribute), */false);

                for (int j = 0; j < oats.Length; ++j)
                {
                    var attributeType = oats[j].GetType();
                    if (oats[j] is UIEventHandleAttribute)
                    {
                        var eventAttr = (UIEventHandleAttribute) oats[j];

                        eventBindings eb = new eventBindings
                        {
                            controlType = eventAttr.controlType,
                            eventType = eventAttr.eventType
                        };
                        if (eventAttr.start == 0 && eventAttr.end == 0)
                        {
                            eb.strList.Add(eventAttr.controlName);
                            eb.method = Delegate.CreateDelegate(eventAttr.eventType,
                                this, current, true);

                        }
                        else
                        {
                            StringBuilder stringFormat = StringBuilderCache.Acquire();
                            for (int k = eventAttr.start; k <= eventAttr.end; ++k)
                            {
                                stringFormat.Clear();
                                stringFormat.AppendFormat(eventAttr.controlName, k);
                                eb.strList.Add(stringFormat.ToString());
                            }
                            StringBuilderCache.Release(stringFormat);

                            eb.method = Delegate.CreateDelegate(eventAttr.eventType,
                                this, current, true);
                        }
                        this.eventBindList.Add(eb);
                    }
                    else if (oats[j] is MessageHandleAttribute)
                    {
                        msgBindings eb = new msgBindings();
                        eb.id = ((MessageHandleAttribute) oats[j]).id;

                        try
                        {
                            eb.method = Delegate.CreateDelegate(typeof(Action<MsgDATA>),
                                this, current, true);
                        }
                        catch (Exception e)
                        {
                            Logger.LogErrorFormat("Error!! Bind Message Method {0} to Contorl {1}: {2} \n", current.ToString(), eb.id, e.ToString());
                        }


                        this.msgBindList.Add(eb);
                    }
                    else if (oats[j] is ProtocolHandleAttribute)
                    {
                        var eventAttr = (ProtocolHandleAttribute) oats[j];
                        Protocol.IGetMsgID protocol = eventAttr.GetBinder() as Protocol.IGetMsgID;
                        if (protocol == null)
                        {
                            Logger.LogError(string.Format("Type{0} in class {1} do not implement Protocol.IGetMsgID", attributeType.Name, type.Name));
                            continue;
                        }
                        protoBindings eb = new protoBindings();
                        eb.id = protocol.GetMsgID();
                        eb.action = new Action<MsgDATA>((MsgDATA data) =>
                        {
                            object msg = eventAttr.GetBinder();
                            int pos = 0;
                            if (!(msg is Protocol.IProtocolStream))
                            {
                                Logger.LogError(string.Format("Type{0} in class {1} do not implement Protocol.IProtocolStream", attributeType.Name, type.Name));
                                return;
                            }
                            ((Protocol.IProtocolStream)msg).decode(data.bytes, ref pos);
                            object[] parametors = new object[] { msg };
                            current.Invoke(this, parametors);
                        });
                        this.protoBindList.Add(eb);
                    }
                }
            }

           // ShowBindInfo();
        }

        protected void ShowBindInfo()
        {
            ExceptionManager.GetInstance().RecordLog("file name:" + this.GetType().Name + "\n");

            if (objectBindList.Count > 0)
            {
                ExceptionManager.GetInstance().RecordLog(string.Format("objectBindings:{0}\n", objectBindList.Count));
                for (int i = 0; i < objectBindList.Count; ++i)
                {
                    Logger.LogErrorFormat("name:{0}\n", objectBindList[i].name);
                }
            }

            if (controlBindList.Count > 0)
            {
                ExceptionManager.GetInstance().RecordLog(string.Format("controlBindList:{0}\n", controlBindList.Count));
                for (int i = 0; i < controlBindList.Count; ++i)
                {
                    ExceptionManager.GetInstance().RecordLog(string.Format("name:{0}\n", controlBindList[i].name));
                }
            }

            if (eventBindList.Count > 0)
            {
                ExceptionManager.GetInstance().RecordLog(string.Format("eventBindList:{0}\n", eventBindList.Count));
                for (int i = 0; i < eventBindList.Count; ++i)
                {
                    StringBuilder sb = new StringBuilder();
                    for (int j = 0; j < eventBindList[i].strList.Count; ++j)
                        sb.AppendFormat(",{0}", i, eventBindList[i].strList[j]);

                    ExceptionManager.GetInstance().RecordLog(string.Format("name:{0}", sb.ToString()));
                }
            }


            if (msgBindList.Count > 0)
            {
                ExceptionManager.GetInstance().RecordLog(string.Format("msgBindList:{0}\n", msgBindList.Count));
                for (int i = 0; i < msgBindList.Count; ++i)
                {
                    ExceptionManager.GetInstance().RecordLog(string.Format("id:{0}\n", msgBindList[i].id));
                }
            }

            if (protoBindList.Count > 0)
            {
                ExceptionManager.GetInstance().RecordLog(string.Format("protoBindList:{0}\n", protoBindList.Count));
                for (int i = 0; i < protoBindList.Count; ++i)
                {
                    ExceptionManager.GetInstance().RecordLog(string.Format("id:{0}\n", protoBindList[i].id));
                }
            }

            if (soundBindList.Count > 0)
            {
                ExceptionManager.GetInstance().RecordLog(string.Format("soundBindings:{0}\n", soundBindList.Count));
                for (int i = 0; i < soundBindList.Count; ++i)
                {
                    ExceptionManager.GetInstance().RecordLog(string.Format("name:{0}\n", soundBindList[i].name));
                }
            }


            ExceptionManager.GetInstance().PrintLogToFile(true);
        }

        protected void InitUISystem(GameObject frame)
        {
            if(frame == null)
            {
                return;
            }

            mRoot = frame;


            for (int i = 0; i < objectBindList.Count; ++i)
            {
                var current = objectBindList[i];
                GameObject obj = Utility.FindGameObject(frame, current.name);
                current.field.SetValue(this, obj);
            }

            //contorlDics.Clear();

            StringBuilder stringFormat = StringBuilderCache.Acquire();
            for(int i = 0; i < controlBindList.Count; ++i)
            {
                var current = controlBindList[i];
                System.Type type = current.type;
                if(type == null)
                {
                    type = current.field.FieldType;
                }

                if (current.bArray)
                {
                    Array arrCom = current.field.GetValue(this) as Array;

                    for(int j = 0; j < arrCom.Length; ++j)
                    {
                        stringFormat.Clear();
                        stringFormat.AppendFormat(current.name, j + current.baseNum);
                        string name = stringFormat.ToString();
                        Component com = Utility.FindComponent(frame, type, name);
                        arrCom.SetValue(com, j);
                        //contorlDics.Add(name, com);
                    }
                }
                else
                {
                    Component com = Utility.FindComponent(frame, type, current.name);
                    current.field.SetValue(this, com);
                    //contorlDics.Add(current.name, com);
                }
            }
            StringBuilderCache.Release(stringFormat);

            for(int i = 0; i < eventBindList.Count; ++i)
            {
                var current = eventBindList[i];

                for(int k = 0; k < current.strList.Count; ++k)
                {
                    var name = current.strList[k];

                    //                     object control = null;
                    //                     contorlDics.TryGetValue(name, out control);
                    //                     if (control == null)
                    //                     {
                    //                         control = Utility.FindComponent(frame, current.controlType, name);
                    //                         if (!contorlDics.ContainsKey(name))
                    //                         {
                    //                             contorlDics.Add(name, control);
                    //                         }
                    //                     }

                    object control = Utility.FindComponent(frame, current.controlType, name);
                    if (current.controlType == typeof(Button))
                    {
                        Button btn = control as Button;

                        if (btn)
                        {
                            try
                            {
                                if (current.strList.Count <= 1)
                                {
                                    var callback = current.method as UnityEngine.Events.UnityAction;

                                    soundBindings sb = null;
                                    for(int j = 0; j < soundBindList.Count; ++j)
                                    {
                                        if(name == soundBindList[j].name)
                                        {
                                            sb = soundBindList[j];
                                            break;
                                        }
                                    }

                                    UIFrameSound frameSound = null;
                                    if (sb == null)
                                    {
                                        frameSound = new UIFrameSound(name);
                                    }
                                    else
                                    {
                                        frameSound = new UIFrameSound(name);
                                        frameSound.bNeed = sb.bNeed;
                                        frameSound.sound = sb.sound;
                                    }

                                    if(frameSound != null)
                                    {
                                        btn.onClick.AddListener(frameSound.OnPlaySound);
                                    }
                                    
                                    btn.onClick.AddListener(callback);
                                }
                                else
                                {
                                    var callback = current.method as UnityEngine.Events.UnityAction<int>;
                                    var index = k;
                                    btn.onClick.AddListener( () => { callback.DynamicInvoke(index); });
                                }
                            }
                            catch (Exception e)
                            {
                                Logger.LogErrorFormat("Error!! Bind Method {0} to Contorl {1}: {2} \n", current.method.ToString(), name, e.ToString());
                            }
                        }
                        else
                        {
                            Logger.LogErrorFormat("Current Can Only Bind Button Method {0} to Contorl {1} \n", current.method.ToString(), name);
                        }
                    }
                    else if(current.controlType == typeof(Toggle))
                    {

                        if (current.strList.Count <= 1)
                        {
                            Toggle toggle = control as Toggle;
                            toggle.onValueChanged.AddListener(current.method as UnityEngine.Events.UnityAction<bool>);
                        }
                        else
                        {
                            Toggle toggle = control as Toggle;
                            int iIndex = k;
                            Delegate callback = current.method;
                            toggle.onValueChanged.AddListener(value => { callback.DynamicInvoke(iIndex, value); });
                        }                    
                    }
                }
            }
        }

        protected void RegisterMsgHandler()
        {
            for(int i = 0; i < msgBindList.Count; ++i)
            {
                NetProcess.AddMsgHandler(msgBindList[i].id, msgBindList[i].method as Action<MsgDATA>);
            }

            for (int i = 0; i < protoBindList.Count; ++i)
            {
                NetProcess.AddMsgHandler(protoBindList[i].id, protoBindList[i].action);
            }
        }

        protected void RemoveMsgHandler()
        {
            for (int i = 0; i < msgBindList.Count; ++i)
            {
                NetProcess.RemoveMsgHandler(msgBindList[i].id, msgBindList[i].method as Action<MsgDATA>);
            }

            for (int i = 0; i < protoBindList.Count; ++i)
            {
                NetProcess.RemoveMsgHandler(protoBindList[i].id, protoBindList[i].action);
            }
        }
        
        public void InitBindSystem(GameObject frameroot)
        {
            try
            {
                InitUIBinding();
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message);
            }
            InitUISystem(frameroot);
            RegisterMsgHandler();
        }

        public T GetComponentInChilderByName<T>(string name) where T : Component
        {
            if (mRoot == null)
            {
                Logger.LogError("mRoot is nil");
                return null;
            }

            if ("." == name)
            {
                return mRoot.GetComponentInChildren<T>();
            }
            else
            {
                var fCom = Utility.GetComponetInChild<T>(mRoot, name);
                if (fCom != null)
                {
                    return fCom;
                }
            }

            return null;
        }

        public T GetComponentByName<T>(string name) where T : Component
        {
            if (mRoot == null)
            {
                Logger.LogError("mRoot is nil");
                return null;
            }

            if ("." == name)
            {
                return mRoot.GetComponent<T>();
            }
            else
            {
                var fCom = Utility.FindComponent<T>(mRoot, name, true);
                if (fCom != null)
                {
                    return fCom;
                }
            }

            return null;
        }
        
        public void ExistBindSystem()
        {
            mRoot = null;

            RemoveMsgHandler();
            DeinitUIBinding();
        }
    }
}
