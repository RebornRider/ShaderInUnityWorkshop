using UnityEditor;
using UnityEngine;

internal class MaterialIntRangeDrawer : ExtendedMaterialPropertyDrawer
{
    public override float GetPropertyHeight()
    {
        return Prop.type != MaterialProperty.PropType.Range ? 40f : base.GetPropertyHeight();
    }

    public override void ExtendedOnGUI()
    {
        Rect position = EditorGUILayout.GetControlRect(true, GetPropertyHeight(),
            EditorStyles.layerMaskField);
        if (Prop.type != MaterialProperty.PropType.Range)
        {
            EditorGUI.HelpBox(position, "IntRange used on a non-range property: " + Prop.name, MessageType.Warning);
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
            int num = EditorGUI.IntSlider(position, LabelString, (int)Prop.floatValue, (int)Prop.rangeLimits.x, (int)Prop.rangeLimits.y);
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