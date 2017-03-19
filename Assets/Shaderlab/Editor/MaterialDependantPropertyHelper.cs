using System.Collections.Generic;
using System.Linq;
using UnityEditor;

public static class MaterialDependantPropertyHelper
{
    public static bool IsDisabled(IEnumerable<ExtendedMaterialPropertyAttribute> attributes, IEnumerable<MaterialProperty> allProperties)
    {
        return attributes.OfType<MaterialDependentPropertyAttribute>().Any(x => x.IsDisabled(allProperties));
    }
}