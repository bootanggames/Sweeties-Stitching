using System;
using UnityEngine;

public class GameEvent
{
    public event Action Event;

    public void RegisterEvent(Action eventAction)
    {
        if (eventAction != null)
        {
            Event += eventAction;
        }
    }

    public void UnregisterEvent(Action eventAction)
    {
        if (eventAction != null)
        {
            Event -= eventAction;
        }
    }

    public void RaiseEvent()
    {
        Event?.Invoke();
    }

    public void UnRegisterAll()
    {
        Event = null;
    }
}

public class GameEvent<T>
{
    public event Action<T> Event;
    public void RegisterEvent(Action<T> eventAction)
    {
        if (eventAction != null)
        {
            Event += eventAction;
        }
    }

    public void UnregisterEvent(Action<T> eventAction)
    {
        if (eventAction != null)
        {
            Event -= eventAction;
        }
    }

    public void RaiseEvent(T param)
    {
        Event?.Invoke(param);
    }

    public void UnRegisterAll()
    {
        Event = null;
    }
}

public class GameEvent<T1,T2>
{
    public event Action<T1,T2> Event;
    public void RegisterEvent(Action<T1,T2> eventAction)
    {
        if (eventAction != null)
        {
            Event += eventAction;
        }
    }

    public void UnregisterEvent(Action<T1,T2> eventAction)
    {
        if (eventAction != null)
        {
            Event -= eventAction;
        }
    }

    public void RaiseEvent(T1 param1, T2 param2)
    {
        Event?.Invoke(param1, param2);
    }

    public void UnRegisterAll()
    {
        Event = null;
    }
}

public class GameEvent<T1, T2,T3>
{
    public event Action<T1, T2,T3> Event;
    public void RegisterEvent(Action<T1, T2,T3> eventAction)
    {
        if (eventAction != null)
        {
            Event += eventAction;
        }
    }

    public void UnregisterEvent(Action<T1, T2,T3> eventAction)
    {
        if (eventAction != null)
        {
            Event -= eventAction;
        }
    }

    public void RaiseEvent(T1 param1, T2 param2, T3 param3)
    {
        Event?.Invoke(param1, param2, param3);
    }

    public void UnRegisterAll()
    {
        Event = null;
    }
}
public class GameEvent<T1, T2, T3, T4>
{
    public event Action<T1, T2, T3, T4> Event;
    public void RegisterEvent(Action<T1, T2, T3, T4> eventAction)
    {
        if (eventAction != null)
        {
            Event += eventAction;
        }
    }

    public void UnregisterEvent(Action<T1, T2, T3, T4> eventAction)
    {
        if (eventAction != null)
        {
            Event -= eventAction;
        }
    }

    public void RaiseEvent(T1 param1, T2 param2, T3 param3, T4 param4)
    {
        Event?.Invoke(param1, param2, param3, param4);
    }

    public void UnRegisterAll()
    {
        Event = null;
    }
}
public class GameEvent<T1, T2, T3, T4, T5>
{
    public event Action<T1, T2, T3, T4, T5> Event;
    public void RegisterEvent(Action<T1, T2, T3, T4, T5> eventAction)
    {
        if (eventAction != null)
        {
            Event += eventAction;
        }
    }

    public void UnregisterEvent(Action<T1, T2, T3, T4, T5> eventAction)
    {
        if (eventAction != null)
        {
            Event -= eventAction;
        }
    }

    public void RaiseEvent(T1 param1, T2 param2, T3 param3, T4 param4, T5 param5)
    {
        Event?.Invoke(param1, param2, param3, param4, param5);
    }

    public void UnRegisterAll()
    {
        Event = null;
    }
}
public class GameEvent<T1, T2, T3, T4, T5, T6>
{
    public event Action<T1, T2, T3, T4, T5, T6> Event;
    public void RegisterEvent(Action<T1, T2, T3, T4, T5, T6> eventAction)
    {
        if (eventAction != null)
        {
            Event += eventAction;
        }
    }

    public void UnregisterEvent(Action<T1, T2, T3, T4, T5, T6> eventAction)
    {
        if (eventAction != null)
        {
            Event -= eventAction;
        }
    }

    public void RaiseEvent(T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6)
    {
        Event?.Invoke(param1, param2, param3, param4, param5, param6);
    }

    public void UnRegisterAll()
    {
        Event = null;
    }
}
public class GameFunc<T, result>
{
    private event Func<T, result> Event;
    public void RegisterEvent(Func<T, result> method)
    {
        if (method == null)
            return;

        Event += method;
    }

    public void UnregisterEvent(Func<T, result> method)
    {
        Event -= method;
    }

    public void UnRegisterAll()
    {
        Event = null;
    }

    public result Raise(T param)
    {
        if (Event != null)
        {
            return Event.Invoke(param);
        }
        return default;
    }
}
public class GameFunc<T1,T2, result>
{
    private event Func<T1,T2, result> Event;
    public void RegisterEvent(Func<T1,T2, result> method)
    {
        if (method == null)
            return;

        Event += method;
    }

    public void UnregisterEvent(Func<T1, T2, result> method)
    {
        Event -= method;
    }

    public void UnRegisterAll()
    {
        Event = null;
    }

    public result Raise(T1 param1, T2 Param2)
    {
        if (Event != null)
        {
            return Event.Invoke(param1, Param2);
        }
        return default;
    }
}
public class GameFunc<T1, T2,T3, result>
{
    private event Func<T1, T2, T3, result> Event;
    public void RegisterEvent(Func<T1, T2, T3, result> method)
    {
        if (method == null)
            return;

        Event += method;
    }

    public void UnregisterEvent(Func<T1, T2, T3, result> method)
    {
        Event -= method;
    }

    public void UnRegisterAll()
    {
        Event = null;
    }

    public result Raise(T1 param1, T2 Param2, T3 Param3)
    {
        if (Event != null)
        {
            return Event.Invoke(param1, Param2, Param3);
        }
        return default;
    }
}
public class GameFunc<T1, T2,T3,T4, result>
{
    private event Func<T1, T2,T3,T4, result> Event;
    public void RegisterEvent(Func<T1, T2,T3,T4, result> method)
    {
        if (method == null)
            return;

        Event += method;
    }

    public void UnregisterEvent(Func<T1, T2,T3,T4, result> method)
    {
        Event -= method;
    }

    public void UnRegisterAll()
    {
        Event = null;
    }

    public result Raise(T1 param1, T2 Param2,T3 Param3, T4 Param4)
    {
        if (Event != null)
        {
            return Event.Invoke(param1, Param2, Param3, Param4);
        }
        return default;
    }
}
public class GameFunc<T1, T2, T3, T4,T5, result>
{
    private event Func<T1, T2, T3, T4, T5, result> Event;
    public void RegisterEvent(Func<T1, T2, T3, T4, T5, result> method)
    {
        if (method == null)
            return;

        Event += method;
    }

    public void UnregisterEvent(Func<T1, T2, T3, T4, T5, result> method)
    {
        Event -= method;
    }

    public void UnRegisterAll()
    {
        Event = null;
    }

    public result Raise(T1 param1, T2 Param2, T3 Param3, T4 Param4, T5 Param5)
    {
        if (Event != null)
        {
            return Event.Invoke(param1, Param2, Param3, Param4, Param5);
        }
        return default;
    }
}

public class GameFunc<result>
{
    private event Func<result> Event;
    public void RegisterEvent(Func<result> method)
    {
        if (method == null)
            return;

        Event += method;
    }

    public void UnregisterEvent(Func<result> method)
    {
        Event -= method;
    }

    public void UnRegisterAll()
    {
        Event = null;
    }

    public result RaiseEvent()
    {
        if (Event != null)
        {
            // NOTE: Only last subscriber’s return is given
            return Event.Invoke();
        }
        return default;
    }
}


