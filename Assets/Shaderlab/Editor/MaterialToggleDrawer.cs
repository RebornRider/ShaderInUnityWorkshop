using System;
using UnityEditor;
using UnityEngine;

internal class MaterialToggleDrawer : ExtendedMaterialPropertyDrawer
{
    protected readonly string keyword;

    public MaterialToggleDrawer()
    {
    }

    public MaterialToggleDrawer(string keyword)
    {
        this.keyword = keyword;
    }

    protected virtual void SetKeyword(MaterialProperty prop, bool on)
    {
        SetKeywordInternal(prop, on, "_ON");
    }

    protected void SetKeywordInternal(MaterialProperty prop, bool on, string defaultKeywordSuffix)
    {
        string text = !string.IsNullOrEmpty(keyword) ? keyword : prop.name.ToUpperInvariant() + defaultKeywordSuffix;
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

    private static readonly MaterialProperty.PropType[] validPropTypes = { MaterialProperty.PropType.Float, MaterialProperty.PropType.Range };
    protected override MaterialProperty.PropType[] ValidPropTypes
    {
        get
        {
            return validPropTypes;
        }
    }


    protected override void DrawProperty(Rect position)
    {
        EditorGUI.BeginChangeCheck();
        bool flag = Math.Abs(Prop.floatValue) > 0.001f;
        EditorGUI.showMixedValue = Prop.hasMixedValue;
        flag = EditorGUI.Toggle(position, LabelContent, flag);
        EditorGUI.showMixedValue = false;
        if (EditorGUI.EndChangeCheck())
        {
            Prop.floatValue = !flag ? 0f : 1f;
            SetKeyword(Prop, flag);
        }
    }

    public override void Setup(ExtendedMaterialEditor.MaterialPropertyInfo materialPropertyInfo)
    {
        base.Setup(materialPropertyInfo);
        if (IsPropertyTypeValid())
        {
            if (!Prop.hasMixedValue)
            {
                SetKeyword(Prop, Math.Abs(Prop.floatValue) > 0.001f);
            }
        }
    }
}