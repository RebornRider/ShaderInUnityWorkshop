using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class ExtendedStandardShaderGUI : ShaderGUI
{
    private ShaderGUI standardShaderGui;
    private List<string> standardShaderProperties;

    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        if (standardShaderGui == null)
        {
            Type standardShaderGuiType =
                AppDomain.CurrentDomain.GetAssemblies()
                    .Single(x => x.GetName().Name == "UnityEditor")
                    .GetTypes()
                    .Single(x => x.Name == "StandardShaderGUI");
            standardShaderGui = Activator.CreateInstance(standardShaderGuiType) as ShaderGUI;

            var standardShader = Shader.Find("Standard");
            standardShaderProperties = GetShaderPropertyNames(standardShader);
        }

        if (standardShaderGui == null)
        {
            Debug.Log("standardShaderGui is null");
            return;
        }

        EditorGUI.BeginChangeCheck();
        standardShaderGui.OnGUI(materialEditor, properties);

        EditorGUILayout.Space();
        GUILayout.Label("Custom Properties", EditorStyles.boldLabel);
        materialEditor.SetDefaultGUIWidths();
        foreach (var diffProperty in properties.Where(x => standardShaderProperties.Contains(x.name) == false))
        {
            if (IsPropertyVisible(diffProperty))
            {
                float propertyHeight = materialEditor.GetPropertyHeight(diffProperty, diffProperty.displayName);
                Rect controlRect = EditorGUILayout.GetControlRect(true, propertyHeight, EditorStyles.layerMaskField);
                materialEditor.ShaderProperty(controlRect, diffProperty, diffProperty.displayName);
            }
        }
        var targetMat = materialEditor.target as Material;
        if (targetMat.renderQueue != 2000 && targetMat.renderQueue != 2450 && targetMat.renderQueue != 3000)
        {
            EditorGUILayout.HelpBox("This material is using a custom RenderQeue of: " + targetMat.renderQueue,
                MessageType.Info);
        }
        materialEditor.RenderQueueField();

        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(targetMat);
        }
    }

    private static bool IsPropertyVisible(MaterialProperty diffProperty)
    {
        return (diffProperty.flags &
                (MaterialProperty.PropFlags.HideInInspector | MaterialProperty.PropFlags.PerRendererData)) ==
               MaterialProperty.PropFlags.None;
    }

    private static List<string> GetShaderPropertyNames(Shader targetShader)
    {
        var targetShaderProperties = new List<string>(ShaderUtil.GetPropertyCount(targetShader));
        for (int i = 0; i < ShaderUtil.GetPropertyCount(targetShader); i++)
        {
            targetShaderProperties.Add(ShaderUtil.GetPropertyName(targetShader, i));
        }
        return targetShaderProperties;
    }
}


public abstract class ExtendedPropertyDrawer : MaterialPropertyDrawer
{
    public abstract void OnGUI(Rect position, MaterialProperty prop, string label, MaterialEditor editor,
        IEnumerable<ExtendedPropertyAttribute> attributes, IEnumerable<MaterialProperty> allProperties);

    [Obsolete(
        "Call OnGUI(Rect position, MaterialProperty prop, string label, MaterialEditor editor, IEnumerable<ExtendedPropertyAttribute> attributes) instead"
    )]
    public sealed override void OnGUI(Rect position, MaterialProperty prop, GUIContent label, MaterialEditor editor)
    {
        OnGUI(position, prop, label.text, editor, null, null);
    }

    [Obsolete(
        "Call OnGUI(Rect position, MaterialProperty prop, string label, MaterialEditor editor, IEnumerable<ExtendedPropertyAttribute> attributes) instead"
    )]
    public sealed override void OnGUI(Rect position, MaterialProperty prop, string label, MaterialEditor editor)
    {
        OnGUI(position, prop, label, editor, null, null);
    }
}

public abstract class ExtendedPropertyAttribute : MaterialPropertyDrawer
{
    public sealed override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
    {
        return 0;
    }

    public sealed override void OnGUI(Rect position, MaterialProperty prop, GUIContent label, MaterialEditor editor)
    {
    }

    public sealed override void OnGUI(Rect position, MaterialProperty prop, string label, MaterialEditor editor)
    {
    }
}

public static class MaterialPropertyDrawerHelper
{
    public static Rect GetPropertyRect(MaterialEditor editor, MaterialProperty prop, string label, bool ignoreDrawer)

    {
        float height = 0.0f;

        if (!ignoreDrawer)

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


                if (
                    handler.GetType()
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

[UsedImplicitly]
public class Vector3Drawer : ExtendedPropertyDrawer
{
    private readonly float w;

    public Vector3Drawer(float w)
    {
        this.w = w;
    }

    public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
    {
        return base.GetPropertyHeight(prop, label, editor) * 1;
    }

    public override void OnGUI(Rect position, MaterialProperty prop, string label, MaterialEditor editor,
        IEnumerable<ExtendedPropertyAttribute> attributes, IEnumerable<MaterialProperty> allProperties)
    {
        position = MaterialPropertyDrawerHelper.GetPropertyRect(editor, prop, label, true);

        BackgroundColorAttribute backgroundColorAttribute = attributes.OfType<BackgroundColorAttribute>().FirstOrDefault();
        if (backgroundColorAttribute != null)
        {
            EditorGUI.DrawRect(position, backgroundColorAttribute.Color);
        }

        var isDisabledBecauseDependantTextureIsUnassigned =
            attributes.OfType<DependentTextureAttribute>()
                .Any(x => x.IsDisabled(allProperties));
        if (isDisabledBecauseDependantTextureIsUnassigned)
        {
            EditorGUI.BeginDisabledGroup(true);
        }
        EditorGUI.BeginChangeCheck();
        EditorGUI.showMixedValue = prop.hasMixedValue;
        float labelWidth = EditorGUIUtility.labelWidth;
        EditorGUIUtility.labelWidth = 0.0f;
        Vector3 vector3 = EditorGUI.Vector3Field(position, label, prop.vectorValue);
        EditorGUIUtility.labelWidth = labelWidth;
        EditorGUI.showMixedValue = false;
        if (EditorGUI.EndChangeCheck())
            prop.vectorValue = new Vector4(vector3.x, vector3.y, vector3.z, w);

        if (isDisabledBecauseDependantTextureIsUnassigned)
        {
            EditorGUI.EndDisabledGroup();
        }
    }
}

[UsedImplicitly]
public class Vector2Drawer : ExtendedPropertyDrawer
{
    private readonly float z;
    private readonly float w;

    public Vector2Drawer(float z, float w)
    {
        this.z = z;
        this.w = w;
    }

    public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
    {
        return base.GetPropertyHeight(prop, label, editor) * 1;
    }

    public override void OnGUI(Rect position, MaterialProperty prop, string label, MaterialEditor editor,
        IEnumerable<ExtendedPropertyAttribute> attributes, IEnumerable<MaterialProperty> allProperties)
    {
        position = MaterialPropertyDrawerHelper.GetPropertyRect(editor, prop, label, true);


        BackgroundColorAttribute backgroundColorAttribute = attributes.OfType<BackgroundColorAttribute>().FirstOrDefault();
        if (backgroundColorAttribute != null)
        {
            EditorGUI.DrawRect(position, backgroundColorAttribute.Color);
        }
        var isDisabledBecauseDependantTextureIsUnassigned =
            attributes.OfType<DependentTextureAttribute>()
                .Any(x => x.IsDisabled(allProperties));
        if (isDisabledBecauseDependantTextureIsUnassigned)
        {
            EditorGUI.BeginDisabledGroup(true);
        }

        EditorGUI.BeginChangeCheck();
        EditorGUI.showMixedValue = prop.hasMixedValue;
        float labelWidth = EditorGUIUtility.labelWidth;
        EditorGUIUtility.labelWidth = 0.0f;
        Vector2 vector2 = EditorGUI.Vector2Field(position, label, prop.vectorValue);
        EditorGUIUtility.labelWidth = labelWidth;
        EditorGUI.showMixedValue = false;
        if (EditorGUI.EndChangeCheck())
        {
            prop.vectorValue = new Vector4(vector2.x, vector2.y, z, w);
        }

        if (isDisabledBecauseDependantTextureIsUnassigned)
        {
            EditorGUI.EndDisabledGroup();
        }
    }
}


[UsedImplicitly]
public class DependentTextureAttribute : ExtendedPropertyAttribute
{
    private readonly string propertyName;

    public DependentTextureAttribute(string propertyName)
    {
        this.propertyName = propertyName;
    }

    public bool IsDisabled(IEnumerable<MaterialProperty> allProperties)
    {
        var property = allProperties.SingleOrDefault(p => p.name == propertyName);
        return property != null && property.textureValue == null;
    }
}

[UsedImplicitly]
public class BackgroundColorAttribute : ExtendedPropertyAttribute
{
    public readonly Color Color = Color.magenta;

    public BackgroundColorAttribute(float r, float g, float b, float a)
    {
        this.Color = new Color(r, g, b, a);
    }

    public BackgroundColorAttribute(string colorHTML)
    {
        Color parsedColor;
        if (ColorUtility.TryParseHtmlString(colorHTML, out parsedColor))
        {
            Color = parsedColor;
        }
    }
}