using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public partial class ExtendedMaterialEditor : MaterialEditor
{
    private object lastTarget;

    private readonly Dictionary<string, MaterialPropertyInfo> cachedMaterialPropertyInfos =
        new Dictionary<string, MaterialPropertyInfo>();

    private MaterialProperty[] materialProperties;

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        if (IsValidTarget == false)
        {
            ResetMaterial();
            return;
        }

        if (IsDifferentTarget())
        {
            ApplyMaterial();
        }

        EditorGUI.BeginChangeCheck();
        Draw();
        if (EditorGUI.EndChangeCheck())
        {
            PropertiesChanged();
        }

        lastTarget = target;
    }

    private void ResetMaterial()
    {
        cachedMaterialPropertyInfos.Clear();
        lastTarget = null;
        materialProperties = null;
    }

    private void ApplyMaterial()
    {
        cachedMaterialPropertyInfos.Clear();
        materialProperties = GetMaterialProperties(targets);
        foreach (MaterialProperty prop in materialProperties)
        {
            cachedMaterialPropertyInfos[prop.name] = AcquirePropertyDrawers(prop);
        }
        foreach (MaterialProperty prop in materialProperties)
        {
            var cachedMaterialPropertyInfo = cachedMaterialPropertyInfos[prop.name];
            cachedMaterialPropertyInfo.ExtendedApply(materialProperties, this);
        }
    }

    private void Draw()
    {
        foreach (
            MaterialPropertyInfo propertyInfo in
            materialProperties.Select(prop => cachedMaterialPropertyInfos[prop.name])
                .Where((info => info.IsHiddenInInspector == false)))
        {
            DrawDecorators(propertyInfo);
            DrawDrawer(propertyInfo);
        }
        RenderQueueField();
    }

    private static void DrawDrawer(MaterialPropertyInfo propertyInfo)
    {
        float labelWidth = EditorGUIUtility.labelWidth;
        float fieldWidth = EditorGUIUtility.fieldWidth;
        propertyInfo.ExtendedMaterialDrawer.ExtendedOnGUI();
        EditorGUIUtility.labelWidth = labelWidth;
        EditorGUIUtility.fieldWidth = fieldWidth;
    }

    private bool IsValidTarget
    {
        get
        {
            return !(target == null || isVisible == false || serializedObject.isEditingMultipleObjects && serializedObject.targetObjects.Any(t => t is Material == false || ((Material)t).shader != ((Material)serializedObject.targetObject).shader));
        }
    }

    void OnSceneGUI()
    {
        if (Selection.activeGameObject == null || Selection.gameObjects.Length != 1 ||
            IsValidTarget == false)
        {
            return;
        }

        foreach (var materialPropertyInfo in cachedMaterialPropertyInfos)
        {
            materialPropertyInfo.Value.OnSceneGUI();
        }
    }

    private void DrawDecorators(MaterialPropertyInfo propertyInfo)
    {
        foreach (var decorator in propertyInfo.ExtendedDecorators)
        {
            float labelWidth = EditorGUIUtility.labelWidth;
            float fieldWidth = EditorGUIUtility.fieldWidth;
            decorator.ExtendedOnGUI();
            EditorGUIUtility.labelWidth = labelWidth;
            EditorGUIUtility.fieldWidth = fieldWidth;
        }
    }

    private bool IsDifferentTarget()
    {
        return !ReferenceEquals(target, lastTarget);
    }


    private float GetTotalPropertyHeight(MaterialProperty prop, MaterialPropertyInfo materialPropertyInfo)
    {
        float propertyHeight =
            materialPropertyInfo.ExtendedDecorators.Sum(
                decoratorDrawer => decoratorDrawer.GetPropertyHeight());

        propertyHeight += materialPropertyInfo.ExtendedMaterialDrawer.GetPropertyHeight();
        return propertyHeight;
    }

    private MaterialPropertyInfo AcquirePropertyDrawers(MaterialProperty prop)
    {
        var info = new MaterialPropertyInfo(prop);
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
                    if (propertyDrawer.GetType().IsSubclassOf(typeof(ExtendedMaterialPropertyDrawer)))
                    {
                        info.ExtendedMaterialDrawer = (ExtendedMaterialPropertyDrawer)propertyDrawer;
                    }
                    else if (propertyDrawer.GetType().IsSubclassOf(typeof(ExtendedMaterialPropertyAttribute)))
                    {
                        info.ExtendedAttributes.Add((ExtendedMaterialPropertyAttribute)propertyDrawer);
                    }
                    else if (propertyDrawer.GetType().IsSubclassOf(typeof(ExtendedMaterialPropertyGizmo)))
                    {
                        info.ExtendedGizmos.Add((ExtendedMaterialPropertyGizmo)propertyDrawer);
                    }
                    else if (propertyDrawer.GetType().IsSubclassOf(typeof(ExtendedMaterialPropertyDecorator)))
                    {
                        info.ExtendedDecorators.Add((ExtendedMaterialPropertyDecorator)propertyDrawer);
                    }
                    else
                    {
                        Debug.LogError("Failed to create ");
                    }
                }
            }
        }

        if (info.ExtendedMaterialDrawer == null)
        {
            ExtendedMaterialPropertyDrawer drawer;
            switch (prop.type)
            {
                case MaterialProperty.PropType.Color:
                    drawer = (ExtendedMaterialPropertyDrawer)CreateInstance(typeof(MaterialColorDrawer), string.Empty);
                    break;
                case MaterialProperty.PropType.Vector:
                    drawer = (ExtendedMaterialPropertyDrawer)CreateInstance(typeof(MaterialVectorDrawer), string.Empty);
                    break;
                case MaterialProperty.PropType.Float:
                    drawer = (ExtendedMaterialPropertyDrawer)CreateInstance(typeof(MaterialFloatDrawer), string.Empty);
                    break;
                case MaterialProperty.PropType.Range:
                    drawer = (ExtendedMaterialPropertyDrawer)CreateInstance(typeof(MaterialRangeDrawer), string.Empty);
                    break;
                case MaterialProperty.PropType.Texture:
                    drawer = (ExtendedMaterialPropertyDrawer)CreateInstance(typeof(MaterialTextureDrawer), string.Empty);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            info.ExtendedMaterialDrawer = drawer;
        }


        return info;
    }

    private static object GetShaderPropertyDrawer(string attrib)
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
                .Select(a => a.GetTypes().Where(t => t.IsSubclassOf(typeof(ExtendedMaterialPropertyAspect)))).SelectMany(c => c);
        foreach (Type type in materialPropertyDrawers)
        {
            if (type.Name == str || type.Name == str + "Drawer" || type.Name == "Material" + str + "Drawer" ||
                type.Name == str + "Decorator" || type.Name == "Material" + str || type.Name == "Material" + str + "Decorator" ||
                type.Name == str + "Gizmo" || type.Name == "Material" + str || type.Name == "Material" + str + "Gizmo" ||
                type.Name == str + "Attribute" || type.Name == "Material" + str || type.Name == "Material" + str + "Attribute")
            {
                try
                {
                    return CreateInstance(type, argsText);
                }
                catch (Exception ex)
                {
                    Debug.LogErrorFormat("Failed to create type: {0} with arguments '{1}'. Expection Message: {2}", type, argsText, ex.Message);
                    return null;
                }
            }
        }

        return null;
    }

    private static object CreateInstance(Type type, string argsText)
    {
        if (string.IsNullOrEmpty(argsText))
            return Activator.CreateInstance(type);
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
        return Activator.CreateInstance(type, objArray);
    }
}