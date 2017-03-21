using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public static class EditorGUIUtil
{
    private static MethodInfo doObjectFieldMethod;
    private static Type objectFieldValidatorDelegate;

    private static MethodInfo DoObjectFieldMethod
    {
        get { return doObjectFieldMethod ?? InitializeDoObjectFieldMethod(); }
    }

    private static Type ObjectFieldValidatorDelegate
    {
        get { return objectFieldValidatorDelegate ?? InitializeObjectFieldValidatorDelegate(); }
    }

    private static Type InitializeObjectFieldValidatorDelegate()
    {
        var pbjectFieldValidatorDelegate = ReflectionUtil.GetUnityEditorType("EditorGUI").GetMethods(BindingFlags.Static | BindingFlags.NonPublic).Where(m => m.Name.Contains("DoObjectField")).ToArray();//.FirstOrDefault(m => m.Name.Contains("DoObjectField")).GetParameters()[6].ParameterType;
        objectFieldValidatorDelegate = pbjectFieldValidatorDelegate.First().GetParameters()[6].ParameterType;
        return objectFieldValidatorDelegate;
    }

    private static MethodInfo InitializeDoObjectFieldMethod()
    {
        doObjectFieldMethod = typeof(EditorGUI).FindMethod("DoObjectField", BindingFlags.Static | BindingFlags.NonPublic,
            typeof(Rect), typeof(Rect), typeof(int), typeof(UnityEngine.Object), typeof(Type),
            typeof(SerializedProperty), ObjectFieldValidatorDelegate, typeof(bool));
        return doObjectFieldMethod;
    }

    public static object CreateFieldValidatorDelegate(string methodName, object target)
    {
        return Delegate.CreateDelegate(ObjectFieldValidatorDelegate, target, methodName, false, true);
    }

    public static T DoObjectField<T>(Rect position, Rect dropRect, int id, UnityEngine.Object obj, Type objType,
        SerializedProperty property, object validator, bool allowSceneObjects)
    {
        var result = DoObjectFieldMethod.InvokeMethod<T>(null, position, dropRect, id, obj, objType, property,
            validator, allowSceneObjects);
        return result;
    }
}