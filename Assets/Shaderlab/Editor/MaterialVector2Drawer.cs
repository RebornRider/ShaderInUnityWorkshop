using UnityEditor;
using UnityEngine;

public class MaterialVector2Drawer : ExtendedMaterialPropertyDrawer
{
    private readonly float z;
    private readonly float w;

    public MaterialVector2Drawer(float z, float w)
    {
        this.z = z;
        this.w = w;
    }

    public override float GetPropertyHeight()
    {
        return Prop.type != MaterialProperty.PropType.Vector ? 40f : 32f;
    }

    public override void ExtendedOnGUI()
    {
        Rect position = EditorGUILayout.GetControlRect(true, GetPropertyHeight(),
            EditorStyles.layerMaskField);
        if (Prop.type != MaterialProperty.PropType.Vector)
        {
            EditorGUI.HelpBox(position, "IntRange used on a non-vector property: " + Prop.name, MessageType.Warning);
            return;
        }
        MaterialBackgroundColorAttribute backgroundColorAttribute = MaterialBackgroundColorAttributeHelper.GetBackgroundColorAttribute(ExtendedAttributes);
        backgroundColorAttribute.BeginBackgroundColor();
        using (
            new EditorGUI.DisabledScope(
                MaterialDependantPropertyHelper.IsDisabled(ExtendedAttributes, AllProperties)))
        {
            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = Prop.hasMixedValue;
            float labelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 0.0f;
            Vector2 vector2 = EditorGUI.Vector2Field(position, LabelString, Prop.vectorValue);
            EditorGUIUtility.labelWidth = labelWidth;
            EditorGUI.showMixedValue = false;
            if (EditorGUI.EndChangeCheck())
            {
                Prop.vectorValue = new Vector4(vector2.x, vector2.y, z, w);
            }
        }
        backgroundColorAttribute.EndBackgroundColor();
    }
}