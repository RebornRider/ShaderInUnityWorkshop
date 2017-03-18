using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

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

    public override void ExtendedOnGUI(Rect position, MaterialProperty prop, string label, MaterialEditor editor,
        IEnumerable<ExtendedPropertyAttribute> attributes, IEnumerable<MaterialProperty> allProperties)
    {
        position = EditorGUILayout.GetControlRect(true, GetPropertyHeight(prop, label, editor),
            EditorStyles.layerMaskField);

        attributes = attributes.EmptyIfNull();
        allProperties = allProperties.EmptyIfNull();

        BackgroundColorAttribute backgroundColorAttribute = BackgroundColorAttributeHelper.GetBackgroundColorAttribute(attributes);
        backgroundColorAttribute.BeginBackgroundColor();
        using (
            new EditorGUI.DisabledScope(
                DependantPropertyHelper.IsDisabled(attributes, allProperties)))
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
        backgroundColorAttribute.EndBackgroundColor();

    }
}