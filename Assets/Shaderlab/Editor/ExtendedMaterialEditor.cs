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

    private readonly Dictionary<string, MaterialPropertyInfo> cachedMaterialPropertyInfos =
        new Dictionary<string, MaterialPropertyInfo>();

    private class MaterialPropertyInfo
    {
        public MaterialPropertyDrawer Drawer;
        public List<MaterialPropertyDrawer> Decorators;
        public ExtendedPropertyDrawer ExtendedDrawer;
        public List<ExtendedPropertyAttribute> ExtendedAttributes;

        public bool HasDrawer
        {
            get { return Drawer != null; }
        }

        public bool HasDecorators
        {
            get { return Decorators.Count > 0; }
        }

        public bool HasExtendedDrawer
        {
            get { return ExtendedDrawer != null; }
        }

        public bool HasExtendedAttributes
        {
            get { return ExtendedAttributes.Count > 0; }
        }

        public bool IsCustom
        {
            get { return HasDrawer || HasDecorators; }
        }

        public bool IsExtended
        {
            get { return HasExtendedDrawer || HasExtendedAttributes; }
        }
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

                DrawDecorators(position, propertyInfo, prop);
                if (propertyInfo.HasExtendedDrawer)
                {
                    float labelWidth = EditorGUIUtility.labelWidth;
                    float fieldWidth = EditorGUIUtility.fieldWidth;
                    propertyInfo.ExtendedDrawer.ExtendedOnGUI(position, prop, prop.displayName, this,
                        propertyInfo.ExtendedAttributes, props);
                    EditorGUIUtility.labelWidth = labelWidth;
                    EditorGUIUtility.fieldWidth = fieldWidth;
                }
                else if (propertyInfo.HasDrawer)
                {
                    DrawDrawer(propertyInfo, position, prop);
                }
                else
                {
                    DefaultDraw(prop, propertyInfo.ExtendedAttributes, props);
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

    private void DefaultDraw(MaterialProperty prop, IEnumerable<ExtendedPropertyAttribute> attributes, IEnumerable<MaterialProperty> allProperties)
    {
        var label = new GUIContent(prop.displayName);
        switch (prop.type)
        {
            case MaterialProperty.PropType.Color:
                {
                    DrawDefaultColor(prop, attributes, allProperties, label);
                    break;
                }
            case MaterialProperty.PropType.Vector:
                {
                    Rect position = MaterialPropertyDrawerHelper.GetPropertyRect(this, prop, label.text, true);
                    BackgroundColorAttribute backgroundColorAttribute = BackgroundColorAttributeHelper.GetBackgroundColorAttribute(attributes);
                    backgroundColorAttribute.BeginBackgroundColor();
                    using (new EditorGUI.DisabledScope(DependantPropertyHelper.IsDisabled(attributes, allProperties)))
                    {
                        EditorGUI.BeginChangeCheck();
                        EditorGUI.showMixedValue = prop.hasMixedValue;
                        float labelWidth = EditorGUIUtility.labelWidth;
                        EditorGUIUtility.labelWidth = 0.0f;
                        Vector4 vector4 = EditorGUI.Vector4Field(position, label, prop.vectorValue);
                        EditorGUIUtility.labelWidth = labelWidth;
                        EditorGUI.showMixedValue = false;
                        if (EditorGUI.EndChangeCheck())
                        {
                            prop.vectorValue = vector4;
                        }
                    }
                    backgroundColorAttribute.EndBackgroundColor();
                    break;
                }

            case MaterialProperty.PropType.Float:
                {
                    Rect position = MaterialPropertyDrawerHelper.GetPropertyRect(this, prop, label.text, true);
                    BackgroundColorAttribute backgroundColorAttribute = BackgroundColorAttributeHelper.GetBackgroundColorAttribute(attributes);

                    backgroundColorAttribute.BeginBackgroundColor();
                    using (new EditorGUI.DisabledScope(DependantPropertyHelper.IsDisabled(attributes, allProperties)))
                    {
                        EditorGUI.BeginChangeCheck();
                        EditorGUI.showMixedValue = prop.hasMixedValue;
                        float num = EditorGUI.FloatField(position, label, prop.floatValue);
                        EditorGUI.showMixedValue = false;
                        if (EditorGUI.EndChangeCheck())
                        {
                            prop.floatValue = num;
                        }
                    }

                    backgroundColorAttribute.EndBackgroundColor();

                    break;
                }
            case MaterialProperty.PropType.Range:
                {
                    Rect position = MaterialPropertyDrawerHelper.GetPropertyRect(this, prop, label.text, true);
                    BackgroundColorAttribute backgroundColorAttribute = BackgroundColorAttributeHelper.GetBackgroundColorAttribute(attributes);
                    backgroundColorAttribute.BeginBackgroundColor();
                    using (new EditorGUI.DisabledScope(DependantPropertyHelper.IsDisabled(attributes, allProperties)))
                    {
                        EditorGUI.BeginChangeCheck();
                        EditorGUI.showMixedValue = prop.hasMixedValue;
                        float labelWidth = EditorGUIUtility.labelWidth;
                        EditorGUIUtility.labelWidth = 0.0f;
                        float num = EditorGUI.Slider(position, label, prop.floatValue, prop.rangeLimits.x, prop.rangeLimits.y);
                        EditorGUI.showMixedValue = false;
                        EditorGUIUtility.labelWidth = labelWidth;
                        if (EditorGUI.EndChangeCheck())
                        {
                            prop.floatValue = num;
                        }
                    }
                    backgroundColorAttribute.EndBackgroundColor();
                    break;
                }
            case MaterialProperty.PropType.Texture:
                {
                    BackgroundColorAttribute backgroundColorAttribute = BackgroundColorAttributeHelper.GetBackgroundColorAttribute(attributes);
                    backgroundColorAttribute.BeginBackgroundColor();
                    using (new EditorGUI.DisabledScope(DependantPropertyHelper.IsDisabled(attributes, allProperties)))
                    {
                        TextureProperty(prop, prop.displayName);
                    }
                    backgroundColorAttribute.EndBackgroundColor();
                    break;
                }
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void DrawDefaultColor(MaterialProperty prop, IEnumerable<ExtendedPropertyAttribute> attributes, IEnumerable<MaterialProperty> allProperties, GUIContent label)
    {
        Rect position = MaterialPropertyDrawerHelper.GetPropertyRect(this, prop, label.text, true);

        attributes = attributes.EmptyIfNull();
        allProperties = allProperties.EmptyIfNull();

        BackgroundColorAttribute backgroundColorAttribute = BackgroundColorAttributeHelper.GetBackgroundColorAttribute(attributes);
        if (backgroundColorAttribute != null)
        {
            backgroundColorAttribute.BeginBackgroundColor();
        }
        using (new EditorGUI.DisabledScope(DependantPropertyHelper.IsDisabled(attributes, allProperties)))
        {
            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = prop.hasMixedValue;
            bool hdr = (prop.flags & MaterialProperty.PropFlags.HDR) != MaterialProperty.PropFlags.None;
            Color color = EditorGUI.ColorField(position, label, prop.colorValue, true, true, hdr, null);
            EditorGUI.showMixedValue = false;
            if (EditorGUI.EndChangeCheck())
            {
                prop.colorValue = color;
            }
        }
        if (backgroundColorAttribute != null)
        {
            backgroundColorAttribute.EndBackgroundColor();
        }
    }

    private void DrawDrawer(MaterialPropertyInfo propertyInfo, Rect position, MaterialProperty prop)
    {
        float labelWidth = EditorGUIUtility.labelWidth;
        float fieldWidth = EditorGUIUtility.fieldWidth;
        propertyInfo.Drawer.OnGUI(position, prop, new GUIContent(prop.displayName), this);
        EditorGUIUtility.labelWidth = labelWidth;
        EditorGUIUtility.fieldWidth = fieldWidth;
    }

    private void DrawDecorators(Rect position, MaterialPropertyInfo propertyInfo, MaterialProperty prop)
    {
        float height = position.height;
        position.height = 0.0f;
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
        position.height = height;
    }

    private bool IsDifferentTarget()
    {
        return !ReferenceEquals(target, lastTarget);
    }


    private float GetTotalPropertyHeight(MaterialProperty prop, MaterialPropertyInfo materialPropertyInfo)
    {
        float propertyHeight =
            materialPropertyInfo.Decorators.Sum(
                decoratorDrawer => decoratorDrawer.GetPropertyHeight(prop, prop.displayName, this));

        if (materialPropertyInfo.HasDrawer)
        {
            propertyHeight += materialPropertyInfo.Drawer.GetPropertyHeight(prop, prop.displayName, this);
        }
        return propertyHeight;
    }

    private MaterialPropertyInfo AcquirePropertyDrawers(MaterialProperty prop)
    {
        var info = new MaterialPropertyInfo
        {
            Drawer = null,
            Decorators = new List<MaterialPropertyDrawer>(),
            ExtendedDrawer = null,
            ExtendedAttributes = new List<ExtendedPropertyAttribute>()
        };
        string[] attributes = typeof(ShaderUtil).FindMethod("GetShaderPropertyAttributes",
                BindingFlags.NonPublic | BindingFlags.Static, typeof(Shader), typeof(string))
            .InvokeMethod<string[]>(null, ((Material)target).shader, prop.name);
        attributes = attributes ?? new string[0];


        foreach (string attribute in attributes)
        {
            var propertyDrawer = GetShaderPropertyDrawer(attribute);
            if (propertyDrawer != null)
            {
                if (propertyDrawer.GetType().IsSubclassOf(typeof(ExtendedPropertyDrawer)))
                {
                    info.ExtendedDrawer = (ExtendedPropertyDrawer)propertyDrawer;
                }
                else if (propertyDrawer.GetType().IsSubclassOf(typeof(ExtendedPropertyAttribute)))
                {
                    info.ExtendedAttributes.Add((ExtendedPropertyAttribute)propertyDrawer);
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
            if (type.Name == str || type.Name == str + "Drawer" || type.Name == "Material" + str + "Drawer" ||
                type.Name == str + "Decorator" || type.Name == "Material" + str + "Decorator" ||
                type.Name == str + "Attribute" || type.Name == "Material" + str + "Attribute")
            {
                try
                {
                    return CreatePropertyDrawer(type, argsText);
                }
                catch (Exception)
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