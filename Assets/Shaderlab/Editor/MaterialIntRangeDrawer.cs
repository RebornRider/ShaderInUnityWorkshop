using UnityEditor;
using UnityEngine;

internal class MaterialIntRangeDrawer : ExtendedMaterialPropertyDrawer
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
        int num = EditorGUI.IntSlider(position, LabelString, (int)Prop.floatValue, (int)Prop.rangeLimits.x, (int)Prop.rangeLimits.y);
        EditorGUI.showMixedValue = false;
        EditorGUIUtility.labelWidth = labelWidth;
        if (EditorGUI.EndChangeCheck())
        {
            Prop.floatValue = num;
        }
    }
}