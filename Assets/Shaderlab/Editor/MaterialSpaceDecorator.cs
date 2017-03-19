using UnityEditor;

internal class MaterialSpaceDecorator : ExtendedMaterialPropertyDecorator
{
    public MaterialSpaceDecorator()
    {
        height = 6f;
    }

    public MaterialSpaceDecorator(float height)
    {
        this.height = height;
    }

    public override float GetPropertyHeight()
    {
        return height;
    }


    public override void ExtendedOnGUI()
    {
        EditorGUILayout.GetControlRect(true, height, EditorStyles.layerMaskField);
    }

    private readonly float height;
}