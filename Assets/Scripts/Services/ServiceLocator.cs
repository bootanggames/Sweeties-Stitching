using System;
using System.Collections.Generic;
using UnityEngine;

public static class ServiceLocator
{
    private static Dictionary<Type,IGameService> services = new ();

    public static void ClearAll()
    {
        services.Clear ();
    }
    public static void RegisterService<T>(T service) where T : IGameService
    {
        var type = typeof(T);
        services [type] = service;
    }

    public static void UnRegisterService<T>(T service) where T : IGameService
    {
        var type = typeof(T);
        services.Remove (type);
    }

    public static T GetService<T> () where T : IGameService
    {
        var type = typeof(T);
        try
        {
            return (T)services [type];
        }
        catch
        {
            return default;
        }
    }
}
