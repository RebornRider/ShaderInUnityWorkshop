using UnityEditor;
using UnityEngine;

internal class MaterialVectorDrawer : ExtendedMaterialPropertyDrawer
{
    protected override float GetPropertyHeight()
    {
        return IsPropertyTypeValid() ? 32f : TypeWarningPropertyHeight;
    }

    private static readonly MaterialProperty.PropType[] validPropTypes = { MaterialProperty.PropType.Vector };
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
        Vector4 vector4 = EditorGUI.Vector4Field(position, LabelContent, Prop.vectorValue);
        EditorGUIUtility.labelWidth = labelWidth;
        EditorGUI.showMixedValue = false;
        if (EditorGUI.EndChangeCheck())
        {
            Prop.vectorValue = vector4;
        }
    }
}