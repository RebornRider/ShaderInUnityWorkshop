using System.Reflection;
using UnityEditor;
using UnityEngine;

internal class MaterialPowerSliderDrawer : ExtendedMaterialPropertyDrawer
{
    private readonly float power;

    public MaterialPowerSliderDrawer(float power)
    {
        this.power = power;
    }

    private static readonly MaterialProperty.PropType[] validPropTypes = { MaterialProperty.PropType.Range };
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
}