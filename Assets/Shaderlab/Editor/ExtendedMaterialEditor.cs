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

    private readonly Dictionary<string, MaterialPropertyInfo> cachedMaterialPropertyInfos = new Dictionary<string, MaterialPropertyInfo>();

    private class MaterialPropertyInfo
    {
        public MaterialPropertyDrawer Drawer;
        public List<MaterialPropertyDrawer> Decorators;
        public ExtendedPropertyDrawer ExtendedDrawer;
        public List<ExtendedPropertyDecorator> ExtendedDecorators;
        public bool HasDrawer { get { return Drawer != null; } }
        public bool HasDecorators { get { return Decorators.Count > 0; } }
        public bool HasExtendedDrawer { get { return ExtendedDrawer != null; } }
        public bool HasExtendedDecorators { get { return ExtendedDecorators.Count > 0; } }
        public bool IsCustom { get { return HasDrawer || HasDecorators; } }
        public bool IsExtended { get { return HasExtendedDrawer || HasExtendedDecorators; } }
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        if (serializedObject.isEditingMultipleObjects || target == null || isVisible == false)
        {
            return;
        }
        if (IsDifferentTarget())
        {
            cachedMaterialPropertyInfos.Clear();
        }

        SerializedProperty theShader = serializedObject.FindProperty("m_Shader");
        if (!theShader.hasMultipleDifferentValues && theShader.objectReferenceValue != null)
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

                if (IsDifferentTarget())
                {
                    cachedMaterialPropertyInfos[prop.name] = AcquirePropertyDrawers(prop);
                }
                MaterialPropertyInfo propertyInfo = cachedMaterialPropertyInfos[prop.name];


                float propertyHeight = GetTotalPropertyHeight(prop, propertyInfo);

                Rect position = EditorGUILayout.GetControlRect(true, propertyHeight);

                float height = position.height;
                position.height = 0.0f;

                if (propertyInfo.HasDecorators)
                {
                    foreach (MaterialPropertyDrawer decoratorDrawer in propertyInfo.Decorators)
                    {
                        position.height = decoratorDrawer.GetPropertyHeight(prop, prop.displayName, this);
                        float labelWidth = EditorGUIUtility.labelWidth;
                        float fieldWidth = EditorGUIUtility.fieldWidth;
                        decoratorDrawer.OnGUI(position, prop, new GUIContent(prop.displayName), this);
                        EditorGUIUtility.labelWidth = labelWidth;
                        EditorGUIUtility.fieldWidth = fieldWidth;
                        position.y += position.height;
                        height -= position.height;
                    }
                }

                position.height = height;
                if (propertyInfo.HasDrawer)
                {
                    float labelWidth1 = EditorGUIUtility.labelWidth;
                    float fieldWidth1 = EditorGUIUtility.fieldWidth;
                    propertyInfo.Drawer.OnGUI(position, prop, new GUIContent(prop.displayName), this);
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

    private bool IsDifferentTarget()
    {
        return !ReferenceEquals(target, lastTarget);
    }


    private float GetTotalPropertyHeight(MaterialProperty prop, MaterialPropertyInfo materialPropertyInfo)
    {
        float propertyHeight = 0.0f;
        if (materialPropertyInfo.Decorators != null)
        {
            foreach (MaterialPropertyDrawer decoratorDrawer in materialPropertyInfo.Decorators)
                propertyHeight += decoratorDrawer.GetPropertyHeight(prop, prop.displayName, this);
        }
        if (materialPropertyInfo.Drawer != null)
            propertyHeight += materialPropertyInfo.Drawer.GetPropertyHeight(prop, prop.displayName, this);
        return propertyHeight;
    }

    private MaterialPropertyInfo AcquirePropertyDrawers(MaterialProperty prop)
    {
        var info = new MaterialPropertyInfo
        {
            Drawer = null,
            Decorators = new List<MaterialPropertyDrawer>(),
            ExtendedDrawer = null,
            ExtendedDecorators = new List<ExtendedPropertyDecorator>()
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
                        info.ExtendedDrawer = (ExtendedPropertyDrawer)propertyDrawer;
                    }
                    else if (propertyDrawer.GetType().IsSubclassOf(typeof(ExtendedPropertyDecorator)))
                    {
                        info.ExtendedDecorators.Add((ExtendedPropertyDecorator)propertyDrawer);
                    }
                    else if (propertyDrawer.GetType().Name.EndsWith("Decorator"))
                    {
                        info.Decorators.Add(propertyDrawer);
                    }
                    else
                    {
                        info.Drawer = propertyDrawer;
                    }
                    propertyDrawer.Apply(prop);
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