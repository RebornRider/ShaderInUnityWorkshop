using UnityEditor;
using UnityEngine;

internal class MaterialColorDrawer : ExtendedMaterialPropertyDrawer
{
    private static readonly MaterialProperty.PropType[] validPropTypes = { MaterialProperty.PropType.Color };
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
        bool hdr = (Prop.flags & MaterialProperty.PropFlags.HDR) != MaterialProperty.PropFlags.None;
        Color color = EditorGUI.ColorField(position, LabelContent, Prop.colorValue, true, true, hdr, null);
        EditorGUI.showMixedValue = false;
        if (EditorGUI.EndChangeCheck())
        {
            Prop.colorValue = color;
        }
        EndDefaultGUIWidth();
    }
}