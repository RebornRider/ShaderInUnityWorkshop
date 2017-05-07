using JetBrains.Annotations;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

[UsedImplicitly]
public abstract class ExtendedMaterialPropertyDrawer : ExtendedMaterialPropertyAspect
{
    private float labelWidth;
    private float fieldWidth;

    public void ExtendedOnGUI()
    {
        Rect position = EditorGUILayout.GetControlRect(true, GetPropertyHeight(), EditorStyles.layerMaskField);
        LastRect = position;
        if (IsPropertyTypeValid() == false)
        {
            EditorGUI.HelpBox(position, InvalidTypeMessage(), MessageType.Error);
            return;
        }

        using (MakeDisabledScope())
        using (ExtendedAttributes.GetBackgroundColorAttribute().MakeBackgroundColorScope())
        {
            DrawProperty(position);
        }
    }

    private EditorGUI.DisabledScope MakeDisabledScope()
    {
        return MaterialDependantPropertyHelper.IsDisabled(ExtendedAttributes, AllProperties) ? new EditorGUI.DisabledScope(true) : new EditorGUI.DisabledScope(!GUI.enabled);
    }

    protected abstract void DrawProperty(Rect position);

    protected virtual float GetPropertyHeight()
    {
        return IsPropertyTypeValid() ? DefaultPropterHeight : TypeWarningPropertyHeight;
    }
    protected const float DefaultPropterHeight = 16f;
    protected const float TypeWarningPropertyHeight = 64f;
    protected abstract MaterialProperty.PropType[] ValidPropTypes { get; }
    protected bool IsPropertyTypeValid()
    {
        return ValidPropTypes.Contains(Prop.type);
    }

    protected string InvalidTypeMessage()
    {
        var sb = new StringBuilder();
        sb.AppendFormat("{0} is an invalid type for {1} on property {2} !\n Valid Types: ", Prop.type, GetType().Name,
            Prop.displayName);
        sb.Append(string.Join(", ", ValidPropTypes.Select(x => x.ToString()).ToArray()));
        return sb.ToString();

    }

    public Rect LastRect { get; private set; }

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