using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public class ExtendedMaterialEditor : MaterialEditor
{

    private object lastTarget;

    private Dictionary<string, MaterialPropertyInfo> cachedMaterialPropertyInfos = new Dictionary<string, MaterialPropertyInfo>();

    class MaterialPropertyInfo
    {
        public MaterialPropertyDrawer drawer;
        public List<MaterialPropertyDrawer> decorators;
        public ExtendedPropertyDrawer extendedDrawer;
        public List<ExtendedPropertyDecorator> extendedDecorators;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        if (serializedObject.isEditingMultipleObjects || target == null)
        {
            return;
        }
        SerializedProperty theShader = serializedObject.FindProperty("m_Shader");
        if (isVisible && !theShader.hasMultipleDifferentValues && theShader.objectReferenceValue != null)
        {
            MaterialProperty[] props = GetMaterialProperties(targets);
            SetDefaultGUIWidths();



            EditorGUI.BeginChangeCheck();
            foreach (MaterialProperty prop in props)
            {
                if ((prop.flags &
                     (MaterialProperty.PropFlags.HideInInspector | MaterialProperty.PropFlags.PerRendererData)) !=
                    MaterialProperty.PropFlags.None)
                {
                    continue;
                }

                MaterialPropertyInfo materialPropertyInfo;
                if (target != lastTarget)
                {
                    cachedMaterialPropertyInfos[prop.name] = AcquirePropertyDrawers(prop);
                }
                materialPropertyInfo = cachedMaterialPropertyInfos[prop.name];


                float propertyHeight = GetTotalPropertyHeight(prop, materialPropertyInfo);

                Rect position = EditorGUILayout.GetControlRect(true, propertyHeight);

                float height = position.height;
                position.height = 0.0f;
                if (materialPropertyInfo.decorators.Count > 0)
                {
                    foreach (MaterialPropertyDrawer decoratorDrawer in materialPropertyInfo.decorators)
                    {
                        position.height = decoratorDrawer.GetPropertyHeight(prop, prop.displayName, this);
                        float labelWidth = EditorGUIUtility.labelWidth;
                        float fieldWidth = EditorGUIUtility.fieldWidth;
                        decoratorDrawer.OnGUI(position, prop, prop.displayName, this);
                        EditorGUIUtility.labelWidth = labelWidth;
                        EditorGUIUtility.fieldWidth = fieldWidth;
                        position.y += position.height;
                        height -= position.height;
                    }
                }
                position.height = height;
                if (materialPropertyInfo.drawer != null)
                {
                    float labelWidth1 = EditorGUIUtility.labelWidth;
                    float fieldWidth1 = EditorGUIUtility.fieldWidth;
                    materialPropertyInfo.drawer.OnGUI(position, prop, prop.displayName, this);
                    EditorGUIUtility.labelWidth = labelWidth1;
                    EditorGUIUtility.fieldWidth = fieldWidth1;
                }
                else
                {
                    switch (prop.type)
                    {
                        case MaterialProperty.PropType.Color:
                            ColorProperty(prop, prop.displayName);
                            break;
                        case MaterialProperty.PropType.Vector:
                            VectorProperty(prop, prop.displayName);
                            break;
                        case MaterialProperty.PropType.Float:
                            FloatProperty(prop, prop.displayName);
                            break;
                        case MaterialProperty.PropType.Range:
                            RangeProperty(prop, prop.displayName);
                            break;
                        case MaterialProperty.PropType.Texture:
                            TextureProperty(prop, prop.displayName);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }

            RenderQueueField();
            if (EditorGUI.EndChangeCheck())
            {
                PropertiesChanged();
            }

            lastTarget = target;
        }
    }


    private float GetTotalPropertyHeight(MaterialProperty prop, MaterialPropertyInfo materialPropertyInfo)
    {
        float propertyHeight = 0.0f;
        if (materialPropertyInfo.decorators != null)
        {
            foreach (MaterialPropertyDrawer decoratorDrawer in materialPropertyInfo.decorators)
                propertyHeight += decoratorDrawer.GetPropertyHeight(prop, prop.displayName, this);
        }
        if (materialPropertyInfo.drawer != null)
            propertyHeight += materialPropertyInfo.drawer.GetPropertyHeight(prop, prop.displayName, this);
        return propertyHeight;
    }

    private MaterialPropertyInfo AcquirePropertyDrawers(MaterialProperty prop)
    {
        var info = new MaterialPropertyInfo
        {
            drawer = null,
            decorators = new List<MaterialPropertyDrawer>(),
            extendedDrawer = null,
            extendedDecorators = new List<ExtendedPropertyDecorator>()
        };
        string[] attributes = typeof(ShaderUtil).FindMethod("GetShaderPropertyAttributes",
                BindingFlags.NonPublic | BindingFlags.Static, typeof(Shader), typeof(string))
            .InvokeMethod<string[]>(null, ((Material)target).shader, prop.name);
        attributes = attributes ?? new string[0];

        if (attributes.Length > 0)
        {
            foreach (string attribute in attributes)
            {
                var propertyDrawer = GetShaderPropertyDrawer(attribute);
                if (propertyDrawer != null)
                {
                    if (propertyDrawer.GetType().IsSubclassOf(typeof(ExtendedPropertyDrawer)))
                    {
                        info.extendedDrawer = (ExtendedPropertyDrawer)propertyDrawer;
                    }
                    else if (propertyDrawer.GetType().IsSubclassOf(typeof(ExtendedPropertyDecorator)))
                    {
                        info.extendedDecorators.Add((ExtendedPropertyDecorator)propertyDrawer);
                    }
                    else if (propertyDrawer.GetType().Name.EndsWith("Decorator"))
                    {
                        info.decorators.Add(propertyDrawer);
                    }
                    else
                    {
                        info.drawer = propertyDrawer;
                    }
                }
            }
        }
        return info;
    }

    private static MaterialPropertyDrawer GetShaderPropertyDrawer(string attrib)
    {
        string str = attrib;
        string argsText = string.Empty;
        Match match = Regex.Match(attrib, "(\\w+)\\s*\\((.*)\\)");
        if (match.Success)
        {
            str = match.Groups[1].Value;
            argsText = match.Groups[2].Value.Trim();
        }
        var materialPropertyDrawers =
            AppDomain.CurrentDomain.GetAssemblies()
                .Select(a => a.GetTypes().Where(t => t.IsSubclassOf(typeof(MaterialPropertyDrawer)))).SelectMany(c => c);
        foreach (Type type in materialPropertyDrawers)
        {
            if (type.Name == str || type.Name == str + "Drawer" ||
                (type.Name == "Material" + str + "Drawer" || type.Name == str + "Decorator") ||
                type.Name == "Material" + str + "Decorator")
            {
                try
                {
                    return CreatePropertyDrawer(type, argsText);
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

    private static MaterialPropertyDrawer CreatePropertyDrawer(Type type, string argsText)
    {
        if (string.IsNullOrEmpty(argsText))
            return Activator.CreateInstance(type) as MaterialPropertyDrawer;
        string[] strArray = argsText.Split(',');
        object[] objArray = new object[strArray.Length];
        for (int index = 0; index < strArray.Length; ++index)
        {
            string s = strArray[index].Trim();
            float result;
            objArray[index] =
                !float.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture.NumberFormat, out result)
                    ? s
                    : (object)result;
        }
        return Activator.CreateInstance(type, objArray) as MaterialPropertyDrawer;
    }
}