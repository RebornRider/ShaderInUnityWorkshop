using System.Reflection;
using UnityEditor;
using UnityEngine;

public static class MaterialPropertyDrawerHelper
{
    public static Rect GetPropertyRect(MaterialEditor editor, MaterialProperty prop, string label, bool ignoreDrawer)
    {
        float height = 0.0f;

        if (ignoreDrawer == false)

        {
            object handler = ReflectionUtil.GetUnityEditorType("MaterialPropertyHandler").FindMethod("GetHandler",
                    BindingFlags.NonPublic | BindingFlags.Static, typeof(Shader), typeof(string))
                .InvokeMethod<object>(null, ((Material)editor.target).shader, prop.name);

            if (handler != null)
            {
                height =
                    handler.GetType()
                        .FindMethod("GetPropertyHeight", BindingFlags.Default, typeof(MaterialEditor), typeof(string),
                            typeof(MaterialEditor))
                        .InvokeMethod<float>(handler, prop, label ?? prop.displayName, editor);


                if (handler.GetType()
                        .FindProperty("propertyDrawer", BindingFlags.Default)
                        .GetGetMethod()
                        .InvokeMethod<MaterialPropertyDrawer>(handler) != null)
                {
                    return EditorGUILayout.GetControlRect(true, height, EditorStyles.layerMaskField);
                }
            }
        }

        return EditorGUILayout.GetControlRect(true, height + MaterialEditor.GetDefaultPropertyHeight(prop),
            EditorStyles.layerMaskField);
    }
}