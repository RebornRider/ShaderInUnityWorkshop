using UnityEditor;
using UnityEngine;

internal class MaterialRangeDrawer : ExtendedMaterialPropertyDrawer
{
    public override float GetPropertyHeight()
    {
        return Prop.type != MaterialProperty.PropType.Range ? 40f : base.GetPropertyHeight();
    }

    public override void ExtendedOnGUI()
    {
        Rect position = EditorGUILayout.GetControlRect(true, GetPropertyHeight());
        if (Prop.type != MaterialProperty.PropType.Range)
        {
            EditorGUI.HelpBox(position, "MaterialRangeDrawer used on a non-range property: " + Prop.name, MessageType.Warning);
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
            float num = EditorGUI.Slider(position, LabelContent, Prop.floatValue, Prop.rangeLimits.x, Prop.rangeLimits.y);
            EditorGUI.showMixedValue = false;
            EditorGUIUtility.labelWidth = labelWidth;
            if (EditorGUI.EndChangeCheck())
            {
                Prop.floatValue = num;
            }
        }
        backgroundColorAttribute.EndBackgroundColor();
    }

}