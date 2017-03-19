using UnityEditor;
using UnityEngine;

internal class MaterialVectorDrawer : ExtendedMaterialPropertyDrawer
{
    public override float GetPropertyHeight()
    {
        return Prop.type != MaterialProperty.PropType.Vector ? 40f : 32;
    }

    public override void ExtendedOnGUI()
    {
        Rect position = EditorGUILayout.GetControlRect(true, GetPropertyHeight(),
            EditorStyles.layerMaskField);
        if (Prop.type != MaterialProperty.PropType.Vector)
        {
            EditorGUI.HelpBox(position, "MaterialVectorDrawer used on a non-vector property: " + Prop.name, MessageType.Warning);
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
            Vector4 vector4 = EditorGUI.Vector4Field(position, LabelContent, Prop.vectorValue);
            EditorGUIUtility.labelWidth = labelWidth;
            EditorGUI.showMixedValue = false;
            if (EditorGUI.EndChangeCheck())
            {
                Prop.vectorValue = vector4;
            }
        }
        backgroundColorAttribute.EndBackgroundColor();
    }

}