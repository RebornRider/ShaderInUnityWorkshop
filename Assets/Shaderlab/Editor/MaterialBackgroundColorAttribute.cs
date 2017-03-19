using UnityEngine;

public class MaterialBackgroundColorAttribute : ExtendedMaterialPropertyAttribute
{
    private readonly Color color = Color.magenta;
    private Color originalBackgroundColor;
    private static readonly NullObjectImpl nullObject = new NullObjectImpl();
    public static MaterialBackgroundColorAttribute NullObject
    {
        get { return nullObject; }
    }

    private MaterialBackgroundColorAttribute()
    {
    }

    public MaterialBackgroundColorAttribute(float r, float g, float b, float a)
    {
        color = new Color(r, g, b, a);
    }

    public MaterialBackgroundColorAttribute(string colorHTML)
    {
        Color parsedColor;
        if (ColorUtility.TryParseHtmlString(colorHTML, out parsedColor))
        {
            color = parsedColor;
        }
    }

    public virtual void BeginBackgroundColor()
    {
        originalBackgroundColor = GUI.backgroundColor;
        GUI.backgroundColor = color;
    }

    public virtual void EndBackgroundColor()
    {
        GUI.backgroundColor = originalBackgroundColor;
    }

    private sealed class NullObjectImpl : MaterialBackgroundColorAttribute
    {
        public override void BeginBackgroundColor()
        {

        }

        public override void EndBackgroundColor()
        {

        }
    }
}