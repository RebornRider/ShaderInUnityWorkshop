using UnityEditor;

internal class MaterialTextureDrawer : ExtendedMaterialPropertyDrawer
{
    public override void ExtendedOnGUI()
    {
        MaterialBackgroundColorAttribute backgroundColorAttribute = MaterialBackgroundColorAttributeHelper.GetBackgroundColorAttribute(ExtendedAttributes);
        backgroundColorAttribute.BeginBackgroundColor();
        using (
            new EditorGUI.DisabledScope(
                MaterialDependantPropertyHelper.IsDisabled(ExtendedAttributes, AllProperties)))
        {
            BeginDefaultGUIWidth();
            Editor.TextureProperty(Prop, LabelString);
            EndDefaultGUIWidth();
        }
        backgroundColorAttribute.EndBackgroundColor();
    }
}