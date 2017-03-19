using UnityEditor;
using UnityEngine;

internal class MaterialColorDrawer : ExtendedMaterialPropertyDrawer
{
    public override float GetPropertyHeight()
    {
        return Prop.type != MaterialProperty.PropType.Color ? 40f : base.GetPropertyHeight();
    }

    public override void ExtendedOnGUI()
    {
        Rect position = EditorGUILayout.GetControlRect(true, GetPropertyHeight(),
            EditorStyles.layerMaskField);
        if (Prop.type != MaterialProperty.PropType.Color)
        {
            EditorGUI.HelpBox(position, "MaterialColorDrawer used on a non-color property: " + Prop.name, MessageType.Warning);
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
        backgroundColorAttribute.EndBackgroundColor();
    }

}