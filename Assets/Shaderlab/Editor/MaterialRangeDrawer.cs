using UnityEditor;
using UnityEngine;

internal class MaterialRangeDrawer : ExtendedMaterialPropertyDrawer
{
    private static readonly MaterialProperty.PropType[] validPropTypes = { MaterialProperty.PropType.Range };
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
}