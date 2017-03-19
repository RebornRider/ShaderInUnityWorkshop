using UnityEditor;
using UnityEngine;

internal class MaterialKeywordEnumDrawer : ExtendedMaterialPropertyDrawer
{
    public MaterialKeywordEnumDrawer(string kw1) : this(new[]
    {
        kw1
    })
    {
    }

    public MaterialKeywordEnumDrawer(string kw1, string kw2) : this(new[]
    {
        kw1,
        kw2
    })
    {
    }

    public MaterialKeywordEnumDrawer(string kw1, string kw2, string kw3) : this(new[]
    {
        kw1,
        kw2,
        kw3
    })
    {
    }

    public MaterialKeywordEnumDrawer(string kw1, string kw2, string kw3, string kw4) : this(new[]
    {
        kw1,
        kw2,
        kw3,
        kw4
    })
    {
    }

    public MaterialKeywordEnumDrawer(string kw1, string kw2, string kw3, string kw4, string kw5) : this(new[]
    {
        kw1,
        kw2,
        kw3,
        kw4,
        kw5
    })
    {
    }

    public MaterialKeywordEnumDrawer(string kw1, string kw2, string kw3, string kw4, string kw5, string kw6) : this(new[]
    {
        kw1,
        kw2,
        kw3,
        kw4,
        kw5,
        kw6
    })
    {
    }

    public MaterialKeywordEnumDrawer(string kw1, string kw2, string kw3, string kw4, string kw5, string kw6, string kw7) : this(new[]
    {
        kw1,
        kw2,
        kw3,
        kw4,
        kw5,
        kw6,
        kw7
    })
    {
    }

    public MaterialKeywordEnumDrawer(string kw1, string kw2, string kw3, string kw4, string kw5, string kw6, string kw7, string kw8) : this(new[]
    {
        kw1,
        kw2,
        kw3,
        kw4,
        kw5,
        kw6,
        kw7,
        kw8
    })
    {
    }

    public MaterialKeywordEnumDrawer(string kw1, string kw2, string kw3, string kw4, string kw5, string kw6, string kw7, string kw8, string kw9) : this(new[]
    {
        kw1,
        kw2,
        kw3,
        kw4,
        kw5,
        kw6,
        kw7,
        kw8,
        kw9
    })
    {
    }

    public MaterialKeywordEnumDrawer(params string[] keywords)
    {
        this.keywords = new GUIContent[keywords.Length];
        for (int i = 0; i < keywords.Length; i++)
        {
            this.keywords[i] = new GUIContent(keywords[i]);
        }
    }

    private static bool IsPropertyTypeSuitable(MaterialProperty prop)
    {
        return prop.type == MaterialProperty.PropType.Float || prop.type == MaterialProperty.PropType.Range;
    }

    private void SetKeyword(MaterialProperty prop, int index)
    {
        for (int i = 0; i < keywords.Length; i++)
        {
            string keywordName = GetKeywordName(prop.name, keywords[i].text);
            Object[] targets = prop.targets;
            for (int j = 0; j < targets.Length; j++)
            {
                Material material = (Material)targets[j];
                if (index == i)
                {
                    material.EnableKeyword(keywordName);
                }
                else
                {
                    material.DisableKeyword(keywordName);
                }
            }
        }
    }

    public override float GetPropertyHeight()
    {
        return IsPropertyTypeSuitable(Prop) ? base.GetPropertyHeight() : 40f;
    }

    public override void ExtendedOnGUI()
    {
        Rect position = EditorGUILayout.GetControlRect(true, GetPropertyHeight(),
            EditorStyles.layerMaskField);
        if (!IsPropertyTypeSuitable(Prop))
        {
            EditorGUI.HelpBox(position, "MaterialKeywordEnumDrawer used on a non-float / non-range property: " + Prop.name, MessageType.Warning);
        }
        else
        {
            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = Prop.hasMixedValue;
            int num = (int)Prop.floatValue;
            num = EditorGUI.Popup(position, LabelContent, num, keywords);
            EditorGUI.showMixedValue = false;
            if (EditorGUI.EndChangeCheck())
            {
                Prop.floatValue = (float)num;
                SetKeyword(Prop, num);
            }
        }
    }

    public override void ExtendedApply(ExtendedMaterialEditor.MaterialPropertyInfo materialPropertyInfo)
    {
        base.ExtendedApply(materialPropertyInfo);
        if (IsPropertyTypeSuitable(Prop))
        {
            if (!Prop.hasMixedValue)
            {
                SetKeyword(Prop, (int)Prop.floatValue);
            }
        }
    }

    private static string GetKeywordName(string propName, string name)
    {
        string text = propName + "_" + name;
        return text.Replace(' ', '_').ToUpperInvariant();
    }

    private readonly GUIContent[] keywords;
}