using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public abstract class ExtendedMaterialPropertyAspect
{
    protected MaterialProperty Prop
    {
        get { return materialPropertyInfo.Prop; }
    }

    protected MaterialEditor Editor
    {
        get { return materialPropertyInfo.Editor; }
    }

    protected ExtendedMaterialPropertyDrawer ExtendedDrawer
    {
        get { return materialPropertyInfo.ExtendedMaterialDrawer; }
    }
    protected IEnumerable<ExtendedMaterialPropertyAttribute> ExtendedAttributes
    {
        get { return materialPropertyInfo.ExtendedAttributes; }
    }

    protected IEnumerable<MaterialProperty> AllProperties
    {
        get { return materialPropertyInfo.AllProperties; }
    }

    protected IEnumerable<ExtendedMaterialPropertyGizmo> ExtendedGizmos
    {
        get { return materialPropertyInfo.ExtendedGizmos; }
    }

    private ExtendedMaterialEditor.MaterialPropertyInfo materialPropertyInfo;

    protected string LabelString
    {
        get { return Prop.displayName; }
    }

    protected GUIContent LabelContent
    {
        get { return new GUIContent(Prop.displayName); }
    }

    public virtual void Setup(ExtendedMaterialEditor.MaterialPropertyInfo materialPropertyInfo)
    {
        this.materialPropertyInfo = materialPropertyInfo;
    }
}