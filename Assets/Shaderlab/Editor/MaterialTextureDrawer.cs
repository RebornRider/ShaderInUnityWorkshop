using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

internal class MaterialTextureDrawer : ExtendedMaterialPropertyDrawer
{
    private TextureDimension desiredTexdim;

    protected override float GetPropertyHeight()
    {
        return IsPropertyTypeValid() ? GetTextureFieldHeight() + 6 : TypeWarningPropertyHeight;
    }

    protected static float GetTextureFieldHeight()
    {
        return 64;
    }

    private static readonly MaterialProperty.PropType[] validPropTypes = { MaterialProperty.PropType.Texture };
    protected override MaterialProperty.PropType[] ValidPropTypes
    {
        get
        {
            return validPropTypes;
        }
    }


    protected override void DrawProperty(Rect position)
    {
        BeginDefaultGUIWidth();

        bool scaleOffset = (Prop.flags & MaterialProperty.PropFlags.NoScaleOffset) ==
                           MaterialProperty.PropFlags.None;
        EditorGUI.PrefixLabel(position, LabelContent);
        position.height = GetTextureFieldHeight();
        Rect texutreBodyPosition = position;
        texutreBodyPosition.xMin = texutreBodyPosition.xMax - EditorGUIUtility.fieldWidth;
        TexturePropertyBody(texutreBodyPosition, Prop);
        if (scaleOffset)
        {
            Editor.TextureScaleOffsetProperty(Editor.GetTexturePropertyCustomArea(position), Prop);
        }
        GUILayout.Space(-6f);
        Editor.TextureCompatibilityWarning(Prop);
        GUILayout.Space(6f);

        EndDefaultGUIWidth();
    }

    protected void TexturePropertyBody(Rect position, MaterialProperty prop)
    {
        if (prop.type != MaterialProperty.PropType.Texture)
            throw new ArgumentException(
                string.Format("The MaterialProperty '{0}' should be of type 'Texture' (its type is '{1})'", prop.name,
                    prop.type));
        desiredTexdim = prop.textureDimension;
        Type typeFromDimension = GetTextureTypeFromDimension();
        EditorGUI.BeginChangeCheck();
        EditorGUI.showMixedValue = prop.hasMixedValue;
        int controlId = GUIUtility.GetControlID(12354, FocusType.Keyboard, position);
        var texture = EditorGUIUtil.DoObjectField<Texture>(position, position, controlId, prop.textureValue,
            typeFromDimension, null, textureValidatorDelegate
            , false);
        EditorGUI.showMixedValue = false;
        if (EditorGUI.EndChangeCheck())
        {
            prop.textureValue = texture;
        }
    }

    protected Type GetTextureTypeFromDimension()
    {
        switch (desiredTexdim)
        {
            case TextureDimension.Any:
                return typeof(Texture);
            case TextureDimension.Tex2D:
                return typeof(Texture);
            case TextureDimension.Tex3D:
                return typeof(Texture3D);
            case TextureDimension.Cube:
                return typeof(Cubemap);
            case TextureDimension.Tex2DArray:
                return typeof(Texture2DArray);
            case TextureDimension.CubeArray:
                return typeof(CubemapArray);
            default:
                return null;
        }
    }

    protected readonly object textureValidatorDelegate;

    public MaterialTextureDrawer()
    {
        textureValidatorDelegate = EditorGUIUtil.CreateFieldValidatorDelegate("TextureValidator", this);
    }

    protected UnityEngine.Object TextureValidator(UnityEngine.Object[] references, Type objType,
        SerializedProperty property)
    {
        return
            references.OfType<Texture>()
                .FirstOrDefault(
                    texture => texture && (texture.dimension == desiredTexdim || desiredTexdim == TextureDimension.Any));
    }
}