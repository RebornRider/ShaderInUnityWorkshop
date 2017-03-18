using UnityEngine;

public class BackgroundColorAttribute : ExtendedPropertyAttribute
{
    private readonly Color color = Color.magenta;
    private Color originalBackgroundColor;
    private static readonly NullObjectImpl nullObject = new NullObjectImpl();
    public static BackgroundColorAttribute NullObject
    {
        get { return nullObject; }
    }

    private BackgroundColorAttribute()
    {
    }

    public BackgroundColorAttribute(float r, float g, float b, float a)
    {
        color = new Color(r, g, b, a);
    }

    public BackgroundColorAttribute(string colorHTML)
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

    private sealed class NullObjectImpl : BackgroundColorAttribute
    {
        public override void BeginBackgroundColor()
        {

        }

        public override void EndBackgroundColor()
        {

        }
    }
}