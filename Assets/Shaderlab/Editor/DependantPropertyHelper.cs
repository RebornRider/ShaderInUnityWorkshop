using System.Collections.Generic;
using System.Linq;
using UnityEditor;

public static class DependantPropertyHelper
{
    public static bool IsDisabled(IEnumerable<ExtendedPropertyAttribute> attributes, IEnumerable<MaterialProperty> allProperties)
    {
        bool isDisabled = attributes.OfType<DependentPropertyAttribute>().Any(x => x.IsDisabled(allProperties));
        return isDisabled;
    }
}