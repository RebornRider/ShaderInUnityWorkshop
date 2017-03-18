using System;
using System.Collections.Generic;
using System.Linq;
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
            EditorGUILayout.HelpBox("This material is using a custom RenderQeue of: " + targetMat.renderQueue, MessageType.Info);
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
    public abstract override void OnGUI(Rect position, MaterialProperty prop, string label, MaterialEditor editor);
}

public abstract class ExtendedPropertyDecorator : MaterialPropertyDrawer
{
    public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
    {
        return 0;
    }
}

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

    public override void OnGUI(Rect position, MaterialProperty prop, string label, MaterialEditor editor)
    {
        EditorGUI.BeginChangeCheck();
        EditorGUI.showMixedValue = prop.hasMixedValue;
        float labelWidth = EditorGUIUtility.labelWidth;
        EditorGUIUtility.labelWidth = 0.0f;
        Vector3 vector3 = EditorGUI.Vector3Field(position, label, prop.vectorValue);
        EditorGUIUtility.labelWidth = labelWidth;
        EditorGUI.showMixedValue = false;
        if (EditorGUI.EndChangeCheck())
            prop.vectorValue = new Vector4(vector3.x, vector3.y, vector3.z, w);
    }
}

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

    public override void OnGUI(Rect position, MaterialProperty prop, string label, MaterialEditor editor)
    {
        EditorGUI.BeginChangeCheck();
        EditorGUI.showMixedValue = prop.hasMixedValue;
        float labelWidth = EditorGUIUtility.labelWidth;
        EditorGUIUtility.labelWidth = 0.0f;
        Vector2 vector2 = EditorGUI.Vector2Field(position, label, prop.vectorValue);
        EditorGUIUtility.labelWidth = labelWidth;
        EditorGUI.showMixedValue = false;
        if (EditorGUI.EndChangeCheck())
            prop.vectorValue = new Vector4(vector2.x, vector2.y, z, w);
    }
}


public class DependentTextureDecorator : ExtendedPropertyDecorator
{
    private readonly string propertyName;

    public DependentTextureDecorator(string propertyName)
    {
        this.propertyName = propertyName;
    }
}

public class ColorDecorator : ExtendedPropertyDecorator
{
    public readonly Color Color = Color.magenta;

    public ColorDecorator(float r, float g, float b, float a)
    {
        this.Color = new Color(r, g, b, a);
    }

    public ColorDecorator(string colorHTML)
    {
        Color parsedColor;
        if (ColorUtility.TryParseHtmlString(colorHTML, out parsedColor))
        {
            Color = parsedColor;
        }
    }
}