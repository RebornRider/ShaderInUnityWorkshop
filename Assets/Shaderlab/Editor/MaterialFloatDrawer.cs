using UnityEditor;
using UnityEngine;

internal class MaterialFloatDrawer : ExtendedMaterialPropertyDrawer
{
    private static readonly MaterialProperty.PropType[] validPropTypes = { MaterialProperty.PropType.Float };
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
        BeginDefaultGUIWidth();
        float num = EditorGUI.FloatField(position, LabelContent, Prop.floatValue);
        EndDefaultGUIWidth();
        EditorGUI.showMixedValue = false;
        if (EditorGUI.EndChangeCheck())
        {
            Prop.floatValue = num;
        }
    }
}
