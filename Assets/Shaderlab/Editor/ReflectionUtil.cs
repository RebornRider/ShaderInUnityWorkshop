using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using UnityEngine;

public static class ReflectionUtil
{
    public static MethodInfo GetMethod(this Type type, string methodName, params Type[] parameterTypes)
    {
        return parameterTypes != null ? type.GetMethod(methodName, parameterTypes) : type.GetMethod(methodName, new Type[] { });
    }

    public static MethodInfo FindMethod(this Type type, string methodName, BindingFlags bindingFlags, params Type[] parameterTypes)
    {
        parameterTypes = parameterTypes ?? new Type[0];
        MethodInfo[] methods = type.GetMethods(bindingFlags);
        foreach (MethodInfo methodInfo in methods)
        {
            if ((methodInfo.Name == methodName && methodInfo.GetParameters().Length == parameterTypes.Length) == false) { continue; }
            bool sucess = true;
            for (var i = 0; i < methodInfo.GetParameters().Length; i++)
            {
                if (methodInfo.GetParameters()[i].ParameterType != parameterTypes[i])
                {
                    sucess = false;
                    break;
                }
            }
            if (sucess)
            {
                return methodInfo;
            }

        }
        return null;
    }

    public static PropertyInfo FindProperty(this Type type, string propertieName, BindingFlags bindingFlags)
    {
        PropertyInfo[] properties = type.GetProperties(bindingFlags);
        foreach (PropertyInfo propertyInfo in properties)
        {
            if (propertyInfo.Name == propertieName) { return propertyInfo; }

        }
        return null;
    }

    public static Type GetUnityEditorType(string className)
    {
        return AppDomain.CurrentDomain.GetAssemblies().Single(a => a.FullName == "UnityEditor").GetType("UnityEditor." + className);
    }

    public static void InvokeMethod(this MethodInfo method, object obj, params object[] parameters)
    {
        method.Invoke(obj, BindingFlags.InvokeMethod, null, parameters, CultureInfo.InvariantCulture);
    }

    public static T1 InvokeMethod<T1>(this MethodInfo method, object obj, params object[] parameters)
    {
        T1 result;
        try
        {
            result = (T1)method.Invoke(obj, BindingFlags.InvokeMethod, null, parameters, CultureInfo.InvariantCulture);
        }
        catch (TargetParameterCountException e)
        {
            Debug.Log(e);
            throw;
        }

        return result;
    }
}