using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Vector2Drawer : ExtendedPropertyDrawer
{
    private readonly float z;
    private readonly float w;

    public Vector2Drawer(float z, float w)
    {
        this.z = z;
        this.w = w;
    }

    public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
    {
        return base.GetPropertyHeight(prop, label, editor) * 1;
    }

    public override void ExtendedOnGUI(Rect position, MaterialProperty prop, string label, MaterialEditor editor,
        IEnumerable<ExtendedPropertyAttribute> attributes, IEnumerable<MaterialProperty> allProperties)
    {
        position = MaterialPropertyDrawerHelper.GetPropertyRect(editor, prop, label, true);

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
            Vector2 vector2 = EditorGUI.Vector2Field(position, label, prop.vectorValue);
            EditorGUIUtility.labelWidth = labelWidth;
            EditorGUI.showMixedValue = false;
            if (EditorGUI.EndChangeCheck())
            {
                prop.vectorValue = new Vector4(vector2.x, vector2.y, z, w);
            }
        }
        backgroundColorAttribute.EndBackgroundColor();
    }
}