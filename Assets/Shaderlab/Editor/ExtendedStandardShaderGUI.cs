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