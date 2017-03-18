using System.Collections.Generic;
using System.Linq;

public static class BackgroundColorAttributeHelper
{
    public static BackgroundColorAttribute GetBackgroundColorAttribute(IEnumerable<ExtendedPropertyAttribute> attributes)
    {
        return attributes.OfType<BackgroundColorAttribute>().FirstOrDefault() ?? BackgroundColorAttribute.NullObject;
    }
}