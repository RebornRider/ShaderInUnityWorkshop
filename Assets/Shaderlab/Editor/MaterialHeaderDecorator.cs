using System.Globalization;
using UnityEditor;
using UnityEngine;

internal class MaterialHeaderDecorator : ExtendedMaterialPropertyDecorator
{
    public MaterialHeaderDecorator(string header)
    {
        this.header = header;
    }

    public MaterialHeaderDecorator(float headerAsNumber)
    {
        header = headerAsNumber.ToString(CultureInfo.InvariantCulture);
    }

    public override float GetPropertyHeight()
    {
        return 24f;
    }

    public override void ExtendedOnGUI()
    {
        Rect position = EditorGUILayout.GetControlRect(true, GetPropertyHeight(),
            EditorStyles.layerMaskField);
        position.y += 8f;
        position = EditorGUI.IndentedRect(position);
        GUI.Label(position, header, EditorStyles.boldLabel);
    }

    private readonly string header;
}