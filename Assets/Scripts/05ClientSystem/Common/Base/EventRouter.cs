using System;

public class EventRouter<T>
{
    public DictionaryView<T, Delegate> m_eventTable = new DictionaryView<T, Delegate>();

    public void AddEventHandler(T eventType, Action handler)
    {
        if (this.OnHandlerAdding(eventType, handler))
        {
            this.m_eventTable[eventType] = (Action)Delegate.Combine((Action)this.m_eventTable[eventType], handler);
        }
    }

    public void AddEventHandler<T1>(T eventType, Action<T1> handler)
    {
        if (this.OnHandlerAdding(eventType, handler))
        {
            this.m_eventTable[eventType] = (Action<T1>)Delegate.Combine((Action<T1>)this.m_eventTable[eventType], handler);
        }
    }

    public void AddEventHandler<T1, T2>(T eventType, Action<T1, T2> handler)
    {
        if (this.OnHandlerAdding(eventType, handler))
        {
            this.m_eventTable[eventType] = (Action<T1, T2>)Delegate.Combine((Action<T1, T2>)this.m_eventTable[eventType], handler);
        }
    }

    public void AddEventHandler<T1, T2, T3>(T eventType, Action<T1, T2, T3> handler)
    {
        if (this.OnHandlerAdding(eventType, handler))
        {
            this.m_eventTable[eventType] = (Action<T1, T2, T3>)Delegate.Combine((Action<T1, T2, T3>)this.m_eventTable[eventType], handler);
        }
    }

    public void AddEventHandler<T1, T2, T3, T4>(T eventType, Action<T1, T2, T3, T4> handler)
    {
        if (this.OnHandlerAdding(eventType, handler))
        {
            this.m_eventTable[eventType] = (Action<T1, T2, T3, T4>)Delegate.Combine((Action<T1, T2, T3, T4>)this.m_eventTable[eventType], handler);
        }
    }

    public void BroadCastEvent(T eventType)
    {
        if (this.OnBroadCasting(eventType))
        {
            Action action = this.m_eventTable[eventType] as Action;
            if (action != null)
            {
                action.Invoke();
            }
        }
    }

    public void BroadCastEvent<T1>(T eventType, T1 arg1)
    {
        if (this.OnBroadCasting(eventType))
        {
            Action<T1> action = this.m_eventTable[eventType] as Action<T1>;
            if (action != null)
            {
                action(arg1);
            }
        }
    }

    public void BroadCastEvent<T1, T2>(T eventType, T1 arg1, T2 arg2)
    {
        if (this.OnBroadCasting(eventType))
        {
            Action<T1, T2> action = this.m_eventTable[eventType] as Action<T1, T2>;
            if (action != null)
            {
                action.Invoke(arg1, arg2);
            }
        }
    }

    public void BroadCastEvent<T1, T2, T3>(T eventType, T1 arg1, T2 arg2, T3 arg3)
    {
        if (this.OnBroadCasting (eventType))
        {
            Action<T1, T2, T3> action = this.m_eventTable[eventType] as Action<T1, T2, T3>;
            if (action != null)
            {
                action.Invoke(arg1, arg2, arg3);
            }
        }
    }

    public void BroadCastEvent<T1, T2, T3, T4>(T eventType, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
    {
        if (this.OnBroadCasting(eventType))
        {
            Action<T1, T2, T3, T4> action = this.m_eventTable[eventType] as Action<T1, T2, T3, T4>;
            if (action != null)
            {
                action.Invoke(arg1, arg2, arg3, arg4);
            }
        }
    }

    public void ClearAllEvents()
    {
        this.m_eventTable.Clear();
    }

    private bool OnBroadCasting(T eventType)
    {
        return this.m_eventTable.ContainsKey(eventType);
    }

    private bool OnHandlerAdding(T eventType, Delegate handler)
    {
        bool flag = true;
        if (!this.m_eventTable.ContainsKey(eventType))
        {
            this.m_eventTable.Add(eventType, null);
        }
        Delegate delegate2 = this.m_eventTable[eventType];
        if ((delegate2 != null) && (delegate2.GetType() != handler.GetType()))
        {
            flag = false;
        }
        return flag;
    }

    private bool OnHandlerRemoving(T eventType, Delegate handler)
    {
        bool flag = true;
        if (this.m_eventTable.ContainsKey(eventType))
        {
            Delegate delegate2 = this.m_eventTable[eventType];
            return ((delegate2 != null) && ((delegate2.GetType() == handler.GetType()) && flag));
        }
        return false;
    }

    public void RemoveEventHandler(T eventType, Action handler)
    {
        if (this.OnHandlerRemoving(eventType, handler))
        {
            this.m_eventTable[eventType] = (Action)Delegate.Remove((Action)this.m_eventTable[eventType], handler);
        }
    }

    public void RemoveEventHandler<T1>(T eventType, Action<T1> handler)
    {
        if (this.OnHandlerRemoving(eventType, handler))
        {
            this.m_eventTable[eventType] = (Action<T1>)Delegate.Remove((Action<T1>)this.m_eventTable[eventType], handler);
        }
    }

    public void RemoveEventHandler<T1, T2>(T eventType, Action<T1, T2> handler)
    {
        if (this.OnHandlerRemoving(eventType, handler))
        {
            this.m_eventTable[eventType] = (Action<T1, T2>)Delegate.Remove((Action<T1, T2>)this.m_eventTable[eventType], handler);
        }
    }

    public void RemoveEventHandler<T1, T2, T3>(T eventType, Action<T1, T2, T3> handler)
    {
        if (this.OnHandlerRemoving(eventType, handler))
        {
            this.m_eventTable[eventType] = (Action<T1, T2, T3>)Delegate.Remove((Action<T1, T2, T3>)this.m_eventTable[eventType], handler);
        }
    }

    public void RemoveEventHandler<T1, T2, T3, T4>(T eventType, Action<T1, T2, T3, T4> handler)
    {
        if (this.OnHandlerRemoving(eventType, handler))
        {
            this.m_eventTable[eventType] = (Action<T1, T2, T3, T4>)Delegate.Remove((Action<T1, T2, T3, T4>)this.m_eventTable[eventType], handler);
        }
    }
}
