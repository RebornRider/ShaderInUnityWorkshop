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
    private ExtendedMaterialEditor extendedMaterialEditor;

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
        if (extendedMaterialEditor == null || extendedMaterialEditor.WrapedEditor == null)
        {
            extendedMaterialEditor = materialEditor.Extend();
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
        extendedMaterialEditor.SetDefaultGUIWidths();
        foreach (var diffProperty in properties.Where(x => standardShaderProperties.Contains(x.name) == false))
        {
            if (IsPropertyVisible(diffProperty))
            {
                float propertyHeight = extendedMaterialEditor.GetPropertyHeight(diffProperty, diffProperty.displayName);
                Rect controlRect = EditorGUILayout.GetControlRect(true, propertyHeight, EditorStyles.layerMaskField);
                extendedMaterialEditor.ShaderProperty(controlRect, diffProperty, diffProperty.displayName);
            }
        }
        var targetMat = extendedMaterialEditor.target as Material;
        if (targetMat.renderQueue != 2000 && targetMat.renderQueue != 2450 && targetMat.renderQueue != 3000)
        {
            EditorGUILayout.HelpBox("This material is using a custom RenderQeue of: " + targetMat.renderQueue, MessageType.Info);
        }
        extendedMaterialEditor.RenderQueueField();

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

public interface IExtendedPropertyDrawer
{
    void OnGUI(Rect position, MaterialProperty prop, GUIContent label, MaterialEditor editor,
        List<MaterialPropertyDrawer> decoratorDrawers);
}

public class Vector3Drawer : MaterialPropertyDrawer, IExtendedPropertyDrawer
{
    private readonly int w;

    public Vector3Drawer(float w)
    {
        this.w = 1;
    }

    public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
    {
        if (prop.type != MaterialProperty.PropType.Vector)
            return 40f;
        return 0;
    }

    public override void OnGUI(Rect position, MaterialProperty prop, GUIContent label, MaterialEditor editor)
    {
        if (prop.type != MaterialProperty.PropType.Vector)
        {
            GUIContent label1 = (GUIContent)typeof(EditorGUIUtility).GetMethods(BindingFlags.NonPublic | BindingFlags.Static).Single(x => x.Name == "TempContent" && x.GetParameters().Length == 2 && x.GetParameters()[0].ParameterType == typeof(string) && x.GetParameters()[1].ParameterType == typeof(Texture))
                  .Invoke(null, new object[] { "Vector3Drawer used on a non-vector property: " + prop.name,  (Texture)typeof(EditorGUIUtility).GetMethod("GetHelpIcon", BindingFlags.NonPublic | BindingFlags.Static)
              .Invoke(null, new object[] { MessageType.Warning }) });
            EditorGUI.LabelField(position, label1, EditorStyles.helpBox);
        }
        else
        {
            position = MaterialPropertyDrawerHelper.GetPropertyRect(editor, prop, label.text, true);
            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = prop.hasMixedValue;
            float labelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 0.0f;
            //EditorGUI.DrawRect(position, Color.green);
            Vector3 vectorXYZ = EditorGUI.Vector3Field(position, label.text, new Vector3(prop.vectorValue.x, prop.vectorValue.y, prop.vectorValue.z));
            EditorGUIUtility.labelWidth = labelWidth;
            EditorGUI.showMixedValue = false;
            if (EditorGUI.EndChangeCheck())
                prop.vectorValue = new Vector4(vectorXYZ.x, vectorXYZ.y, vectorXYZ.z, w);
        }
    }

    public void OnGUI(Rect position, MaterialProperty prop, GUIContent label, MaterialEditor editor, List<MaterialPropertyDrawer> decoratorDrawers)
    {
        if (prop.type != MaterialProperty.PropType.Vector)
        {
            GUIContent label1 = (GUIContent)typeof(EditorGUIUtility).GetMethods(BindingFlags.NonPublic | BindingFlags.Static).Single(x => x.Name == "TempContent" && x.GetParameters().Length == 2 && x.GetParameters()[0].ParameterType == typeof(string) && x.GetParameters()[1].ParameterType == typeof(Texture))
                  .Invoke(null, new object[] { "Vector3Drawer used on a non-vector property: " + prop.name,  (Texture)typeof(EditorGUIUtility).GetMethod("GetHelpIcon", BindingFlags.NonPublic | BindingFlags.Static)
              .Invoke(null, new object[] { MessageType.Warning }) });
            EditorGUI.LabelField(position, label1, EditorStyles.helpBox);
        }
        else
        {
            position = MaterialPropertyDrawerHelper.GetPropertyRect(editor, prop, label.text, true);
            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = prop.hasMixedValue;
            float labelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 0.0f;
            //EditorGUI.DrawRect(position, Color.green);
            Vector3 vectorXYZ = EditorGUI.Vector3Field(position, label.text, new Vector3(prop.vectorValue.x, prop.vectorValue.y, prop.vectorValue.z));
            EditorGUIUtility.labelWidth = labelWidth;
            EditorGUI.showMixedValue = false;
            if (EditorGUI.EndChangeCheck())
                prop.vectorValue = new Vector4(vectorXYZ.x, vectorXYZ.y, vectorXYZ.z, w);
        }
    }
}

public class Vector2Drawer : MaterialPropertyDrawer, IExtendedPropertyDrawer
{
    private readonly float z;
    private readonly int w;

    public Vector2Drawer(float z, float w)
    {
        this.z = z;
        this.w = 1;
    }

    public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
    {
        if (prop.type != MaterialProperty.PropType.Vector)
            return 40f;
        return 0;
    }

    public override void OnGUI(Rect position, MaterialProperty prop, GUIContent label, MaterialEditor editor)
    {
        if (prop.type != MaterialProperty.PropType.Vector)
        {
            GUIContent label1 = (GUIContent)typeof(EditorGUIUtility).GetMethods(BindingFlags.NonPublic | BindingFlags.Static).Single(x => x.Name == "TempContent" && x.GetParameters().Length == 2 && x.GetParameters()[0].ParameterType == typeof(string) && x.GetParameters()[1].ParameterType == typeof(Texture))
                  .Invoke(null, new object[] { "Vector2Drawer used on a non-vector property: " + prop.name,  (Texture)typeof(EditorGUIUtility).GetMethod("GetHelpIcon", BindingFlags.NonPublic | BindingFlags.Static)
              .Invoke(null, new object[] { MessageType.Warning }) });
            EditorGUI.LabelField(position, label1, EditorStyles.helpBox);
        }
        else
        {
            position = MaterialPropertyDrawerHelper.GetPropertyRect(editor, prop, label.text, true);
            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = prop.hasMixedValue;
            float labelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 0.0f;
            //EditorGUI.DrawRect(position, Color.red);
            Vector2 vectorXY = EditorGUI.Vector2Field(position, label.text, new Vector2(prop.vectorValue.x, prop.vectorValue.y));
            EditorGUIUtility.labelWidth = labelWidth;
            EditorGUI.showMixedValue = false;
            if (EditorGUI.EndChangeCheck())
                prop.vectorValue = new Vector4(vectorXY.x, vectorXY.y, z, w);
        }
    }

    public void OnGUI(Rect position, MaterialProperty prop, GUIContent label, MaterialEditor editor, List<MaterialPropertyDrawer> decoratorDrawers)
    {
        if (prop.type != MaterialProperty.PropType.Vector)
        {
            GUIContent label1 = (GUIContent)typeof(EditorGUIUtility).GetMethods(BindingFlags.NonPublic | BindingFlags.Static).Single(x => x.Name == "TempContent" && x.GetParameters().Length == 2 && x.GetParameters()[0].ParameterType == typeof(string) && x.GetParameters()[1].ParameterType == typeof(Texture))
                  .Invoke(null, new object[] { "Vector2Drawer used on a non-vector property: " + prop.name,  (Texture)typeof(EditorGUIUtility).GetMethod("GetHelpIcon", BindingFlags.NonPublic | BindingFlags.Static)
              .Invoke(null, new object[] { MessageType.Warning }) });
            EditorGUI.LabelField(position, label1, EditorStyles.helpBox);
        }
        else
        {
            position = MaterialPropertyDrawerHelper.GetPropertyRect(editor, prop, label.text, true);
            if (decoratorDrawers != null && decoratorDrawers.Any(x => x.GetType() == typeof(ColorDecorator)))
            {
                EditorGUI.DrawRect(position, decoratorDrawers.OfType<ColorDecorator>().First().Color);
            }
            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = prop.hasMixedValue;
            float labelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 0.0f;
            Vector2 vectorXY = EditorGUI.Vector2Field(position, label.text, new Vector2(prop.vectorValue.x, prop.vectorValue.y));
            EditorGUIUtility.labelWidth = labelWidth;
            EditorGUI.showMixedValue = false;
            if (EditorGUI.EndChangeCheck())
                prop.vectorValue = new Vector4(vectorXY.x, vectorXY.y, z, w);
        }
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
                height = handler.GetType().FindMethod("GetPropertyHeight", BindingFlags.Default, typeof(MaterialEditor), typeof(string), typeof(MaterialEditor)).InvokeMethod<float>(handler, prop, label ?? prop.displayName, editor);

                if (handler.GetType().FindProperty("propertyDrawer", BindingFlags.Default).GetGetMethod().InvokeMethod<MaterialPropertyDrawer>(handler) != null)
                {
                    return EditorGUILayout.GetControlRect(true, height, EditorStyles.layerMaskField);
                }
            }
        }
        return EditorGUILayout.GetControlRect(true, height + MaterialEditor.GetDefaultPropertyHeight(prop), EditorStyles.layerMaskField);
    }
}

public class DependentTextureDecorator : MaterialPropertyDrawer
{
    private readonly string propertyName;

    public DependentTextureDecorator(string propertyName)
    {
        this.propertyName = propertyName;
    }

    public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
    {
        return 0;
    }

    public override void OnGUI(Rect position, MaterialProperty prop, GUIContent label, MaterialEditor editor)
    {

    }
}

public class ColorDecorator : MaterialPropertyDrawer
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

    public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
    {
        return 0;
    }

    public override void OnGUI(Rect position, MaterialProperty prop, GUIContent label, MaterialEditor editor)
    {

    }
}