using System.Reflection;
using UnityEditor;
using UnityEngine;

internal class MaterialPowerSliderDrawer : ExtendedMaterialPropertyDrawer
{
    public MaterialPowerSliderDrawer(float power)
    {
        this.power = power;
    }

    public override float GetPropertyHeight()
    {
        return Prop.type != MaterialProperty.PropType.Range ? 40f : base.GetPropertyHeight();
    }

    public override void ExtendedOnGUI()
    {
        Rect position = EditorGUILayout.GetControlRect(true, GetPropertyHeight(),
            EditorStyles.layerMaskField);
        if (Prop.type != MaterialProperty.PropType.Range)
        {
            EditorGUI.HelpBox(position, "PowerSlider used on a non-range property: " + Prop.name, MessageType.Warning);
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
            float labelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 0.0f;

            float num = typeof(EditorGUI).FindMethod("PowerSlider", BindingFlags.Static | BindingFlags.NonPublic,
                    position.GetType(), LabelContent.GetType(), Prop.floatValue.GetType(), Prop.rangeLimits.x.GetType(),
                    Prop.rangeLimits.y.GetType(), power.GetType())
                .InvokeMethod<float>(null, position, LabelContent, Prop.floatValue, Prop.rangeLimits.x, Prop.rangeLimits.y, power);
            //EditorGUI.PowerSlider(position, LabelContent, Prop.floatValue, Prop.rangeLimits.x, Prop.rangeLimits.y, power);
            EditorGUI.showMixedValue = false;
            EditorGUIUtility.labelWidth = labelWidth;
            if (EditorGUI.EndChangeCheck())
            {
                Prop.floatValue = num;
            }
        }
        backgroundColorAttribute.EndBackgroundColor();
    }

    private readonly float power;
}