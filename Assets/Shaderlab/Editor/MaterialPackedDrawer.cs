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

    protected override float GetPropertyHeight()
    {
        return IsPropertyTypeValid() ? DefaultPropterHeight * 5 : TypeWarningPropertyHeight;
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
        float lineHeight = DefaultPropterHeight;
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
}