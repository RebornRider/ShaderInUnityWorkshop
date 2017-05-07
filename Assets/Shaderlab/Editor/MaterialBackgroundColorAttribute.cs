using System;
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

    protected virtual void BeginBackgroundColor()
    {
        originalBackgroundColor = GUI.backgroundColor;
        GUI.backgroundColor = color;
    }

    protected virtual void EndBackgroundColor()
    {
        GUI.backgroundColor = originalBackgroundColor;
    }

    public BackgroundColorScope MakeBackgroundColorScope()
    {
        return new BackgroundColorScope(this);
    }

    public struct BackgroundColorScope : IDisposable
    {
        private readonly MaterialBackgroundColorAttribute backgroundColorAttribute;

        public BackgroundColorScope(MaterialBackgroundColorAttribute backgroundColorAttribute)
        {
            this.backgroundColorAttribute = backgroundColorAttribute;
            backgroundColorAttribute.BeginBackgroundColor();
        }

        public void Dispose()
        {
            backgroundColorAttribute.EndBackgroundColor();
        }
    }

    private sealed class NullObjectImpl : MaterialBackgroundColorAttribute
    {
        protected override void BeginBackgroundColor()
        {
        }

        protected override void EndBackgroundColor()
        {
        }
    }
}