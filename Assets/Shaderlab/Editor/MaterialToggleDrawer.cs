using System;
using UnityEditor;
using UnityEngine;

internal class MaterialToggleDrawer : ExtendedMaterialPropertyDrawer
{
    public MaterialToggleDrawer()
    {
    }

    public MaterialToggleDrawer(string keyword)
    {
        this.keyword = keyword;
    }

    private static bool IsPropertyTypeSuitable(MaterialProperty prop)
    {
        return prop.type == MaterialProperty.PropType.Float || prop.type == MaterialProperty.PropType.Range;
    }

    protected virtual void SetKeyword(MaterialProperty prop, bool on)
    {
        SetKeywordInternal(prop, on, "_ON");
    }

    protected void SetKeywordInternal(MaterialProperty prop, bool on, string defaultKeywordSuffix)
    {
        string text = (!string.IsNullOrEmpty(keyword)) ? keyword : (prop.name.ToUpperInvariant() + defaultKeywordSuffix);
        UnityEngine.Object[] targets = prop.targets;
        for (int i = 0; i < targets.Length; i++)
        {
            Material material = (Material)targets[i];
            if (on)
            {
                material.EnableKeyword(text);
            }
            else
            {
                material.DisableKeyword(text);
            }
        }
    }

    public override float GetPropertyHeight()
    {
        return IsPropertyTypeSuitable(Prop) ? base.GetPropertyHeight() : 40f;
    }

    public override void ExtendedOnGUI()
    {
        Rect position = EditorGUILayout.GetControlRect(true, GetPropertyHeight(),
            EditorStyles.layerMaskField);
        if (!IsPropertyTypeSuitable(Prop))
        {
            EditorGUI.HelpBox(position, "Toggle used on a non-float / non-range property: " + Prop.name, MessageType.Warning);
            return;
        }
        MaterialBackgroundColorAttribute backgroundColorAttribute = MaterialBackgroundColorAttributeHelper.GetBackgroundColorAttribute(ExtendedAttributes);
        backgroundColorAttribute.BeginBackgroundColor();
        using (
            new EditorGUI.DisabledScope(
                MaterialDependantPropertyHelper.IsDisabled(ExtendedAttributes, AllProperties)))
        {
            EditorGUI.BeginChangeCheck();
            bool flag = Math.Abs(Prop.floatValue) > 0.001f;
            EditorGUI.showMixedValue = Prop.hasMixedValue;
            flag = EditorGUI.Toggle(position, LabelContent, flag);
            EditorGUI.showMixedValue = false;
            if (EditorGUI.EndChangeCheck())
            {
                Prop.floatValue = ((!flag) ? 0f : 1f);
                SetKeyword(Prop, flag);
            }
        }
        backgroundColorAttribute.EndBackgroundColor();
    }
    public override void ExtendedApply(ExtendedMaterialEditor.MaterialPropertyInfo materialPropertyInfo)
    {
        base.ExtendedApply(materialPropertyInfo);
        if (IsPropertyTypeSuitable(Prop))
        {
            if (!Prop.hasMixedValue)
            {
                SetKeyword(Prop, Math.Abs(Prop.floatValue) > 0.001f);
            }
        }
    }

    protected readonly string keyword;
}