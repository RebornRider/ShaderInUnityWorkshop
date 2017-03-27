using UnityEditor;
using UnityEngine;

public class MaterialPackedDrawer : ExtendedMaterialPropertyDrawer
{
    private readonly GUIContent xLabel;
    private readonly GUIContent yLabel;
    private readonly GUIContent zLabel;
    private readonly GUIContent wLabel;

    public MaterialPackedDrawer()
    {
    }

    public MaterialPackedDrawer(string x, string y, string z, string w)
    {
        xLabel = new GUIContent(x);
        yLabel = new GUIContent(y);
        zLabel = new GUIContent(z);
        wLabel = new GUIContent(w);
    }

    public override float GetPropertyHeight()
    {
        return Prop.type != MaterialProperty.PropType.Vector ? 40f : base.GetPropertyHeight() * 5;
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
            float lineHeight = base.GetPropertyHeight();
            position.height = lineHeight;
            EditorGUI.LabelField(position, LabelString);
            position.y += lineHeight;
            EditorGUI.indentLevel++;
            BeginDefaultGUIWidth();
            float x = EditorGUI.FloatField(position, xLabel, Prop.vectorValue.x);
            position.y += lineHeight;
            float y = EditorGUI.FloatField(position, yLabel, Prop.vectorValue.y);
            position.y += lineHeight;
            float z = EditorGUI.FloatField(position, zLabel, Prop.vectorValue.z);
            position.y += lineHeight;
            float w = EditorGUI.FloatField(position, wLabel, Prop.vectorValue.w);
            EndDefaultGUIWidth();
            EditorGUI.indentLevel--;
            EditorGUI.showMixedValue = false;
            if (EditorGUI.EndChangeCheck())
            {
                Prop.vectorValue = new Vector4(x, y, z, w);
            }
        }
        backgroundColorAttribute.EndBackgroundColor();

    }
}