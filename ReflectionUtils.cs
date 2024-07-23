using System;
using System.Reflection;

namespace RMServerAssist;

public static class ReflectionUtils
{
    public static T Invoke<T>(this object obj, string methodName, params object[] args)
    {
        var method = obj.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (method == null)
        {
            throw new MissingMethodException(obj.GetType().Name, methodName);
        }
        
        return (T) method.Invoke(obj, args);
    }
    
    public static void Invoke(this object obj, string methodName, params object[] args)
    {
        var method = obj.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (method == null)
        {
            throw new MissingMethodException(obj.GetType().Name, methodName);
        }
        
        method.Invoke(obj, args);
    }
    
    public static T GetFieldValue<T>(this object obj, string fieldName)
    {
        var field = obj.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (field == null)
        {
            throw new MissingFieldException(obj.GetType().Name, fieldName);
        }
        
        return (T) field.GetValue(obj);
    }
}