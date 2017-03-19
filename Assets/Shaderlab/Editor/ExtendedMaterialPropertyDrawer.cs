using JetBrains.Annotations;
using UnityEditor;

[UsedImplicitly]
public abstract class ExtendedMaterialPropertyDrawer : ExtendedMaterialPropertyAspect
{
    private float labelWidth;
    private float fieldWidth;
    public abstract void ExtendedOnGUI();

    public virtual float GetPropertyHeight()
    {
        return 16;
    }

    protected void EndDefaultGUIWidth()
    {
        EditorGUIUtility.labelWidth = labelWidth;
        EditorGUIUtility.fieldWidth = fieldWidth;
    }

    protected void BeginDefaultGUIWidth()
    {
        labelWidth = EditorGUIUtility.labelWidth;
        fieldWidth = EditorGUIUtility.fieldWidth;
        Editor.SetDefaultGUIWidths();
    }
}