using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

[UsedImplicitly]
public abstract class ExtendedPropertyAttribute : MaterialPropertyDrawer
{
    public sealed override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
    {
        return 0;
    }

    public sealed override void OnGUI(Rect position, MaterialProperty prop, GUIContent label, MaterialEditor editor)
    {
    }

    public sealed override void OnGUI(Rect position, MaterialProperty prop, string label, MaterialEditor editor)
    {
    }
}