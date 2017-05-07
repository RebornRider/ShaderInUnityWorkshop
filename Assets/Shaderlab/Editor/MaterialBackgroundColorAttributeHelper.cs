using System.Collections.Generic;
using System.Linq;

public static class MaterialBackgroundColorAttributeHelper
{
    public static MaterialBackgroundColorAttribute GetBackgroundColorAttribute(this IEnumerable<ExtendedMaterialPropertyAttribute> attributes)
    {
        return attributes.OfType<MaterialBackgroundColorAttribute>().FirstOrDefault() ?? MaterialBackgroundColorAttribute.NullObject;
    }
}