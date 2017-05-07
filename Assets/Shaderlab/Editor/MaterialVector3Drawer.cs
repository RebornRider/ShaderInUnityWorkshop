using UnityEditor;
using UnityEngine;

public class MaterialVector3Drawer : ExtendedMaterialPropertyDrawer
{
    public readonly float W;

    public MaterialVector3Drawer(float w)
    {
        W = w;
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
        Vector3 vector3 = EditorGUI.Vector3Field(position, LabelString, Prop.vectorValue);
        EditorGUIUtility.labelWidth = labelWidth;
        EditorGUI.showMixedValue = false;
        if (EditorGUI.EndChangeCheck())
        {
            Prop.vectorValue = new Vector4(vector3.x, vector3.y, vector3.z, W);
        }
    }
}