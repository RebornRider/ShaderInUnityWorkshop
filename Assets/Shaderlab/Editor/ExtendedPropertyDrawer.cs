using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[UsedImplicitly]
public abstract class ExtendedPropertyDrawer : MaterialPropertyDrawer
{
    public abstract void ExtendedOnGUI(Rect position, MaterialProperty prop, string label, MaterialEditor editor,
        IEnumerable<ExtendedPropertyAttribute> attributes, IEnumerable<MaterialProperty> allProperties);

    public sealed override void OnGUI(Rect position, MaterialProperty prop, GUIContent label, MaterialEditor editor)
    {
        ExtendedOnGUI(position, prop, label.text, editor, null, null);
    }

    public sealed override void OnGUI(Rect position, MaterialProperty prop, string label, MaterialEditor editor)
    {
        ExtendedOnGUI(position, prop, label, editor, null, null);
    }
}