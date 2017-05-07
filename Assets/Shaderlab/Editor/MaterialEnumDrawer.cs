using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

internal class MaterialEnumDrawer : ExtendedMaterialPropertyDrawer
{
    private readonly GUIContent[] names;
    private readonly int[] values;

    public MaterialEnumDrawer(string enumName)
    {
        Type[] source =
            AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .ToArray();
        try
        {
            Type enumType =
                source.FirstOrDefault(
                    x => x.IsSubclassOf(typeof(Enum)) && (x.Name == enumName || x.FullName == enumName));
            string[] array = Enum.GetNames(enumType);
            names = new GUIContent[array.Length];
            for (var i = 0; i < array.Length; i++)
                names[i] = new GUIContent(array[i]);
            Array array2 = Enum.GetValues(enumType);
            values = new int[array2.Length];
            for (var j = 0; j < array2.Length; j++)
                values[j] = (int)array2.GetValue(j);
        }
        catch (Exception)
        {
            Debug.LogWarningFormat("Failed to create MaterialEnum, enum {0} not found", enumName);
            throw;
        }
    }

    public MaterialEnumDrawer(string n1, float v1) : this(new[]
    {
        n1
    }, new[]
    {
        v1
    })
    {
    }

    public MaterialEnumDrawer(string n1, float v1, string n2, float v2) : this(new[]
    {
        n1,
        n2
    }, new[]
    {
        v1,
        v2
    })
    {
    }

    public MaterialEnumDrawer(string n1, float v1, string n2, float v2, string n3, float v3) : this(new[]
    {
        n1,
        n2,
        n3
    }, new[]
    {
        v1,
        v2,
        v3
    })
    {
    }

    public MaterialEnumDrawer(string n1, float v1, string n2, float v2, string n3, float v3, string n4, float v4)
        : this(new[]
        {
            n1,
            n2,
            n3,
            n4
        }, new[]
        {
            v1,
            v2,
            v3,
            v4
        })
    {
    }


    public MaterialEnumDrawer(string n1, float v1, string n2, float v2, string n3, float v3, string n4, float v4,
        string n5, float v5) : this(new[]
    {
        n1,
        n2,
        n3,
        n4,
        n5
    }, new[]
    {
        v1,
        v2,
        v3,
        v4,
        v5
    })
    {
    }


    public MaterialEnumDrawer(string n1, float v1, string n2, float v2, string n3, float v3, string n4, float v4,
        string n5, float v5, string n6, float v6) : this(new[]
    {
        n1,
        n2,
        n3,
        n4,
        n5,
        n6
    }, new[]
    {
        v1,
        v2,
        v3,
        v4,
        v5,
        v6
    })
    {
    }

    public MaterialEnumDrawer(string n1, float v1, string n2, float v2, string n3, float v3, string n4, float v4,
        string n5, float v5, string n6, float v6, string n7, float v7) : this(new[]
    {
        n1,
        n2,
        n3,
        n4,
        n5,
        n6,
        n7
    }, new[]
    {
        v1,
        v2,
        v3,
        v4,
        v5,
        v6,
        v7
    })
    {
    }

    public MaterialEnumDrawer(string[] enumNames, float[] vals)
    {
        names = new GUIContent[enumNames.Length];
        for (var i = 0; i < enumNames.Length; i++)
            names[i] = new GUIContent(enumNames[i]);
        values = new int[vals.Length];
        for (var j = 0; j < vals.Length; j++)
            values[j] = (int)vals[j];
    }

    private static readonly MaterialProperty.PropType[] validPropTypes = { MaterialProperty.PropType.Float, MaterialProperty.PropType.Range };
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
        var num = (int)Prop.floatValue;
        num = EditorGUI.IntPopup(position, LabelContent, num, names, values);
        EditorGUI.showMixedValue = false;
        if (EditorGUI.EndChangeCheck())
        {
            Prop.floatValue = num;
        }
    }
}