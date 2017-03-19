using UnityEditor;
using UnityEngine;

public class MaterialVector3Drawer : ExtendedMaterialPropertyDrawer
{
    public readonly float W;

    public MaterialVector3Drawer(float w)
    {
        W = w;
    }

    public override float GetPropertyHeight()
    {
        return Prop.type != MaterialProperty.PropType.Vector ? 40f : 32;
    }

    public override void ExtendedOnGUI()
    {

        Rect position = EditorGUILayout.GetControlRect(true, GetPropertyHeight(),
            EditorStyles.layerMaskField);
        if (Prop.type != MaterialProperty.PropType.Vector)
        {
            EditorGUI.HelpBox(position, "MaterialVector3Drawer used on a non-vector property: " + Prop.name, MessageType.Warning);
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
            Vector3 vector3 = EditorGUI.Vector3Field(position, LabelString, Prop.vectorValue);
            EditorGUIUtility.labelWidth = labelWidth;
            EditorGUI.showMixedValue = false;
            if (EditorGUI.EndChangeCheck())
            {
                Prop.vectorValue = new Vector4(vector3.x, vector3.y, vector3.z, W);
            }
        }
        backgroundColorAttribute.EndBackgroundColor();

    }
}