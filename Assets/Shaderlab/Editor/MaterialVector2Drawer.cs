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
        Vector2 vector2 = EditorGUI.Vector2Field(position, LabelString, Prop.vectorValue);
        EditorGUIUtility.labelWidth = labelWidth;
        EditorGUI.showMixedValue = false;
        if (EditorGUI.EndChangeCheck())
        {
            Prop.vectorValue = new Vector4(vector2.x, vector2.y, z, w);
        }
    }
}