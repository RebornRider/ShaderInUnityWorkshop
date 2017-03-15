using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
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
                materialEditor.ShaderProperty(EditorGUILayout.GetControlRect(true, materialEditor.GetPropertyHeight(diffProperty, diffProperty.displayName), EditorStyles.layerMaskField), diffProperty, diffProperty.displayName);
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

public class Vector3Drawer : MaterialPropertyDrawer
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
}

public class Vector2Drawer : MaterialPropertyDrawer
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
}

public static class MaterialPropertyDrawerHelper
{
    public static Rect GetPropertyRect(MaterialEditor editor, MaterialProperty prop, string label, bool ignoreDrawer)
    {
        float height = 0.0f;
        if (!ignoreDrawer)
        {
            Type materialPropertyHandlerType =
                AppDomain.CurrentDomain.GetAssemblies()
                    .Single(x => x.GetName().Name == "UnityEditor")
                    .GetTypes()
                    .Single(x => x.Name == "MaterialPropertyHandler");

            object handler = materialPropertyHandlerType.GetMethods(BindingFlags.NonPublic | BindingFlags.Static).Single(x => x.Name == "GetHandler" && x.GetParameters().Length == 2 && x.GetParameters()[0].ParameterType == typeof(Shader) && x.GetParameters()[1].ParameterType == typeof(string))
                .Invoke(null, new object[] { ((Material)editor.target).shader, prop.name });
            if (handler != null)
            {
                height = (float)materialPropertyHandlerType.GetMethods(BindingFlags.NonPublic | BindingFlags.Static)
                    .Single(x => x.Name == "GetPropertyHeight" && x.GetParameters().Length == 3 && x.GetParameters()[0].ParameterType == typeof(MaterialProperty) && x.GetParameters()[1].ParameterType == typeof(string) && x.GetParameters()[2].ParameterType == typeof(MaterialEditor))
                    .Invoke(handler, new object[] { prop, label ?? prop.displayName, editor });

                if (materialPropertyHandlerType.GetProperty("propertyDrawer", typeof(MaterialPropertyDrawer)).GetValue(handler, null) != null)
                    return EditorGUILayout.GetControlRect(true, height, EditorStyles.layerMaskField);
            }
        }
        return EditorGUILayout.GetControlRect(true, height + MaterialEditor.GetDefaultPropertyHeight(prop), EditorStyles.layerMaskField);
    }
}

public class DependentTextureDrawer : MaterialPropertyDrawer
{
    private readonly string propertyName;

    public DependentTextureDrawer(string propertyName)
    {
        this.propertyName = propertyName;
    }

    public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
    {
        return 0;
    }

    public override void OnGUI(Rect position, MaterialProperty prop, GUIContent label, MaterialEditor editor)
    {
        position = MaterialPropertyDrawerHelper.GetPropertyRect(editor, prop, label.text, true);
        if (((Material)editor.target).GetTexture(propertyName) == null)
        {
            EditorGUI.DrawRect(position, Color.gray);
        }
        switch (prop.type)
        {
            case MaterialProperty.PropType.Color:
                editor.ColorProperty(position, prop, label.text);
                break;
            case MaterialProperty.PropType.Vector:
                editor.VectorProperty(position, prop, label.text);
                break;
            case MaterialProperty.PropType.Float:
                double num1 = editor.FloatProperty(position, prop, label.text);
                break;
            case MaterialProperty.PropType.Range:
                double num2 = editor.RangeProperty(position, prop, label.text);
                break;
            case MaterialProperty.PropType.Texture:
                editor.TextureProperty(position, prop, label.text);
                break;
            default:
                GUI.Label(position, "Unknown property type: " + prop.name + ": " + prop.type);
                break;
        }
    }

    class ExtendedMaterialEditor : MaterialEditor
    {
        private readonly MaterialEditor targetEditor;

        public ExtendedMaterialEditor(MaterialEditor targetEditor)
        {
            this.targetEditor = targetEditor;

        }


        private class MaterialPropertyHandler
        {
            private static Dictionary<string, MaterialPropertyHandler> s_PropertyHandlers = new Dictionary<string, MaterialPropertyHandler>();
            private MaterialPropertyDrawer m_PropertyDrawer;
            private List<MaterialPropertyDrawer> m_DecoratorDrawers;

            public MaterialPropertyDrawer propertyDrawer
            {
                get
                {
                    return m_PropertyDrawer;
                }
            }

            public bool IsEmpty()
            {
                return m_PropertyDrawer == null && (m_DecoratorDrawers == null || m_DecoratorDrawers.Count == 0);
            }

            public void OnGUI(ref Rect position, MaterialProperty prop, GUIContent label, MaterialEditor editor)
            {
                float height = position.height;
                position.height = 0.0f;
                if (m_DecoratorDrawers != null)
                {
                    foreach (MaterialPropertyDrawer decoratorDrawer in m_DecoratorDrawers)
                    {
                        position.height = decoratorDrawer.GetPropertyHeight(prop, label.text, editor);
                        float labelWidth = EditorGUIUtility.labelWidth;
                        float fieldWidth = EditorGUIUtility.fieldWidth;
                        decoratorDrawer.OnGUI(position, prop, label, editor);
                        EditorGUIUtility.labelWidth = labelWidth;
                        EditorGUIUtility.fieldWidth = fieldWidth;
                        position.y += position.height;
                        height -= position.height;
                    }
                }
                position.height = height;
                if (m_PropertyDrawer == null)
                    return;
                float labelWidth1 = EditorGUIUtility.labelWidth;
                float fieldWidth1 = EditorGUIUtility.fieldWidth;
                m_PropertyDrawer.OnGUI(position, prop, label, editor);
                EditorGUIUtility.labelWidth = labelWidth1;
                EditorGUIUtility.fieldWidth = fieldWidth1;
            }

            public float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
            {
                float num = 0.0f;
                if (m_DecoratorDrawers != null)
                {
                    foreach (MaterialPropertyDrawer decoratorDrawer in m_DecoratorDrawers)
                        num += decoratorDrawer.GetPropertyHeight(prop, label, editor);
                }
                if (m_PropertyDrawer != null)
                    num += m_PropertyDrawer.GetPropertyHeight(prop, label, editor);
                return num;
            }

            private static string GetPropertyString(Shader shader, string name)
            {
                if (shader == null)
                    return string.Empty;
                return shader.GetInstanceID() + "_" + name;
            }

            internal static void InvalidatePropertyCache(Shader shader)
            {
                if (shader == null)
                    return;
                string str = shader.GetInstanceID() + "_";
                List<string> stringList = new List<string>();
                foreach (string key in s_PropertyHandlers.Keys)
                {
                    if (key.StartsWith(str))
                        stringList.Add(key);
                }
                foreach (string key in stringList)
                    s_PropertyHandlers.Remove(key);
            }

            private static MaterialPropertyDrawer CreatePropertyDrawer(Type klass, string argsText)
            {
                if (string.IsNullOrEmpty(argsText))
                    return Activator.CreateInstance(klass) as MaterialPropertyDrawer;
                string[] strArray = argsText.Split(',');
                object[] objArray = new object[strArray.Length];
                for (int index = 0; index < strArray.Length; ++index)
                {
                    string s = strArray[index].Trim();
                    float result;
                    objArray[index] = !float.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture.NumberFormat, out result) ? s : (object)result;
                }
                return Activator.CreateInstance(klass, objArray) as MaterialPropertyDrawer;
            }

            private static MaterialPropertyDrawer GetShaderPropertyDrawer(string attrib, out bool isDecorator)
            {
                isDecorator = false;
                string str = attrib;
                string argsText = string.Empty;
                Match match = Regex.Match(attrib, "(\\w+)\\s*\\((.*)\\)");
                if (match.Success)
                {
                    str = match.Groups[1].Value;
                    argsText = match.Groups[2].Value.Trim();
                }

                Type editorAssembliesType = AppDomain.CurrentDomain.GetAssemblies()
                    .Single(x => x.GetName().Name == "UnityEditor")
                    .GetTypes()
                    .Single(x => x.Name == "EditorAssemblies");
                foreach (Type klass in (IEnumerable<Type>)editorAssembliesType.GetMethods(BindingFlags.NonPublic | BindingFlags.Static)
                    .Single(x => x.Name == "SubclassesOf" && x.GetParameters().Length == 1 && x.GetParameters()[0].ParameterType == typeof(Type))
                    .Invoke(editorAssembliesType, new object[] { typeof(MaterialPropertyDrawer) }))
                //EditorAssemblies.SubclassesOf(typeof(MaterialPropertyDrawer)))
                {
                    if (klass.Name == str || klass.Name == str + "Drawer" || (klass.Name == "Material" + str + "Drawer" || klass.Name == str + "Decorator") || klass.Name == "Material" + str + "Decorator")
                    {
                        try
                        {
                            isDecorator = klass.Name.EndsWith("Decorator");
                            return CreatePropertyDrawer(klass, argsText);
                        }
                        catch (Exception ex)
                        {
                            Debug.LogWarningFormat("Failed to create material drawer {0} with arguments '{1}'", str, argsText);
                            return null;
                        }
                    }
                }
                return null;
            }

            private static MaterialPropertyHandler GetShaderPropertyHandler(Shader shader, string name)
            {
                string[] propertyAttributes = (string[])typeof(ShaderUtil).GetMethods(BindingFlags.NonPublic | BindingFlags.Static)
                    .Single(x => x.Name == "GetShaderPropertyAttributes" && x.GetParameters().Length == 2 && x.GetParameters()[0].ParameterType == typeof(Shader) && x.GetParameters()[0].ParameterType == typeof(string))
                    .Invoke(null, new object[] { shader, name });
                //ShaderUtil.GetShaderPropertyAttributes(shader, name);
                if (propertyAttributes == null || propertyAttributes.Length == 0)
                    return null;
                var materialPropertyHandler = new MaterialPropertyHandler();
                foreach (string attrib in propertyAttributes)
                {
                    bool isDecorator;
                    MaterialPropertyDrawer shaderPropertyDrawer = GetShaderPropertyDrawer(attrib, out isDecorator);
                    if (shaderPropertyDrawer != null)
                    {
                        if (isDecorator)
                        {
                            if (materialPropertyHandler.m_DecoratorDrawers == null)
                                materialPropertyHandler.m_DecoratorDrawers = new List<MaterialPropertyDrawer>();
                            materialPropertyHandler.m_DecoratorDrawers.Add(shaderPropertyDrawer);
                        }
                        else
                        {
                            if (materialPropertyHandler.m_PropertyDrawer != null)
                                Debug.LogWarning(string.Format("Shader property {0} already has a property drawer", name), shader);
                            materialPropertyHandler.m_PropertyDrawer = shaderPropertyDrawer;
                        }
                    }
                }
                return materialPropertyHandler;
            }

            internal static MaterialPropertyHandler GetHandler(Shader shader, string name)
            {
                if (shader == null)
                    return null;
                string propertyString = GetPropertyString(shader, name);
                MaterialPropertyHandler materialPropertyHandler;
                if (s_PropertyHandlers.TryGetValue(propertyString, out materialPropertyHandler))
                    return materialPropertyHandler;
                materialPropertyHandler = GetShaderPropertyHandler(shader, name);
                if (materialPropertyHandler != null && materialPropertyHandler.IsEmpty())
                    materialPropertyHandler = null;
                s_PropertyHandlers[propertyString] = materialPropertyHandler;
                return materialPropertyHandler;
            }
        }
    }

}