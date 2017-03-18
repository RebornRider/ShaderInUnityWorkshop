using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public class DependentPropertyAttribute : ExtendedPropertyAttribute
{
    private readonly string propertyName;
    private readonly bool isTexture;
    private readonly bool isValid = true;
    private readonly CompareFunction compareFunction = CompareFunction.Disabled;
    private readonly float comperand;

    public DependentPropertyAttribute(string textureName)
    {
        propertyName = textureName;
        isTexture = true;
        compareFunction = CompareFunction.Equal;
    }

    public DependentPropertyAttribute(string propertyName, string compareFunction, float comperand)
    {
        this.propertyName = propertyName;
        isTexture = false;
        object parsedComparisionFunction = Enum.Parse(typeof(CompareFunction), compareFunction, true);
        if (Enum.IsDefined(typeof(CompareFunction), parsedComparisionFunction))
        {
            this.compareFunction = (CompareFunction)parsedComparisionFunction;
        }
        else
        {
            isValid = false;
            Debug.LogError("compareFunction is not valid: " + compareFunction);
        }
        this.comperand = comperand;
    }

    public bool IsDisabled(IEnumerable<MaterialProperty> allProperties)
    {
        var property = allProperties.SingleOrDefault(p => p.name == propertyName && (isTexture || p.type == MaterialProperty.PropType.Range || p.type == MaterialProperty.PropType.Float));
        if (isValid == false || property == null || compareFunction == CompareFunction.Disabled || compareFunction == CompareFunction.Never)
        {
            return false;
        }
        if (compareFunction == CompareFunction.Always)
        {
            return true;
        }
        if (isTexture)
        {
            return property.textureValue == null;
        }

        switch (compareFunction)
        {
            case CompareFunction.Less:
                return (property.floatValue < comperand) == false;
            case CompareFunction.Equal:
                return (property.floatValue == comperand) == false;
            case CompareFunction.LessEqual:
                return (property.floatValue <= comperand) == false;
            case CompareFunction.Greater:
                return (property.floatValue > comperand) == false;
            case CompareFunction.NotEqual:
                return (property.floatValue != comperand) == false;
            case CompareFunction.GreaterEqual:
                return (property.floatValue >= comperand) == false;
            default:
                return false;
        }
    }
}