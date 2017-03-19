using System.Collections.Generic;
using UnityEditor;

public partial class ExtendedMaterialEditor
{
    public class MaterialPropertyInfo
    {
        public ExtendedMaterialPropertyDrawer ExtendedMaterialDrawer { get; set; }
        public readonly List<ExtendedMaterialPropertyDecorator> ExtendedDecorators = new List<ExtendedMaterialPropertyDecorator>();
        public readonly List<ExtendedMaterialPropertyAttribute> ExtendedAttributes = new List<ExtendedMaterialPropertyAttribute>();
        public readonly List<ExtendedMaterialPropertyGizmo> ExtendedGizmos = new List<ExtendedMaterialPropertyGizmo>();
        public readonly MaterialProperty Prop;
        public MaterialEditor Editor { get; private set; }
        public IEnumerable<MaterialProperty> AllProperties { get; private set; }

        public bool HasExtendedAttributes
        {
            get { return ExtendedAttributes.Count > 0; }
        }

        public bool HasExtendedGizmos
        {
            get { return ExtendedGizmos.Count > 0; }
        }

        public bool IsHiddenInInspector
        {
            get
            {
                return (Prop.flags &
                        (MaterialProperty.PropFlags.HideInInspector | MaterialProperty.PropFlags.PerRendererData)) !=
                       MaterialProperty.PropFlags.None;
            }
        }

        public MaterialPropertyInfo(MaterialProperty prop)
        {
            Prop = prop;

        }

        public void ExtendedApply(IEnumerable<MaterialProperty> allProperties, MaterialEditor editor)
        {
            AllProperties = allProperties.EmptyIfNull();
            Editor = editor;

            ExtendedMaterialDrawer.ExtendedApply(this);
            foreach (var extendedDecorator in ExtendedDecorators)
            {
                extendedDecorator.ExtendedApply(this);
            }
            foreach (var extendedPropertyAttribute in ExtendedAttributes)
            {
                extendedPropertyAttribute.ExtendedApply(this);
            }
            foreach (var extendedPropertyGizmo in ExtendedGizmos)
            {
                extendedPropertyGizmo.ExtendedApply(this);
            }
        }

        public void OnSceneGUI()
        {
            foreach (var extendedGizmo in ExtendedGizmos)
            {
                extendedGizmo.OnSceneGUI();
            }
        }
    }
}