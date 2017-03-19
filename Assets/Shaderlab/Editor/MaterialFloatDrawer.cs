using UnityEditor;
using UnityEngine;

internal class MaterialFloatDrawer : ExtendedMaterialPropertyDrawer
{
    public override float GetPropertyHeight()
    {
        return Prop.type != MaterialProperty.PropType.Float ? 40f : base.GetPropertyHeight();
    }

    public override void ExtendedOnGUI()
    {
        Rect position = EditorGUILayout.GetControlRect(true, GetPropertyHeight(),
            EditorStyles.layerMaskField);
        if (Prop.type != MaterialProperty.PropType.Float)
        {
            EditorGUI.HelpBox(position, "MaterialFloatDrawer used on a non-float property: " + Prop.name, MessageType.Warning);
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
            BeginDefaultGUIWidth();
            float num = EditorGUI.FloatField(position, LabelContent, Prop.floatValue);
            EndDefaultGUIWidth();
            EditorGUI.showMixedValue = false;
            if (EditorGUI.EndChangeCheck())
            {
                Prop.floatValue = num;
            }
        }
        backgroundColorAttribute.EndBackgroundColor();
    }

}