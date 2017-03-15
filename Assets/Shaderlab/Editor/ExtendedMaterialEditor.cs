using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public class ExtendedMaterialEditor
{
    public MaterialEditor WrapedEditor { get; private set; }

    public ExtendedMaterialEditor(MaterialEditor materialEditor)
    {
        WrapedEditor = materialEditor;
    }

    public int GetInstanceID()
    {
        return WrapedEditor.GetInstanceID();
    }

    public string name
    {
        get { return WrapedEditor.name; }
        set { WrapedEditor.name = value; }
    }

    public HideFlags hideFlags
    {
        get { return WrapedEditor.hideFlags; }
        set { WrapedEditor.hideFlags = value; }
    }

    public void SetDirty()
    {
        WrapedEditor.SetDirty();
    }

    public bool DrawDefaultInspector()
    {
        return WrapedEditor.DrawDefaultInspector();
    }

    public void Repaint()
    {
        WrapedEditor.Repaint();
    }

    public GUIContent GetPreviewTitle()
    {
        return WrapedEditor.GetPreviewTitle();
    }

    public string GetInfoString()
    {
        return WrapedEditor.GetInfoString();
    }

    public void ReloadPreviewInstances()
    {
        WrapedEditor.ReloadPreviewInstances();
    }

    public void DrawHeader()
    {
        WrapedEditor.DrawHeader();
    }

    public void DrawPreview(Rect previewArea)
    {
        WrapedEditor.DrawPreview(previewArea);
    }

    public bool UseDefaultMargins()
    {
        return WrapedEditor.UseDefaultMargins();
    }

    public void Initialize(Object[] targets)
    {
        WrapedEditor.Initialize(targets);
    }

    public bool MoveNextTarget()
    {
        return WrapedEditor.MoveNextTarget();
    }

    public void ResetTarget()
    {
        WrapedEditor.ResetTarget();
    }

    public Object target
    {
        get { return WrapedEditor.target; }
        set { WrapedEditor.target = value; }
    }

    public Object[] targets
    {
        get { return WrapedEditor.targets; }
    }

    public SerializedObject serializedObject
    {
        get { return WrapedEditor.serializedObject; }
    }

    public void SetShader(Shader shader)
    {
        WrapedEditor.SetShader(shader);
    }

    public void SetShader(Shader newShader, bool registerUndo)
    {
        WrapedEditor.SetShader(newShader, registerUndo);
    }

    public void Awake()
    {
        WrapedEditor.Awake();
    }

    public void OnInspectorGUI()
    {
        WrapedEditor.OnInspectorGUI();
    }

    public void PropertiesChanged()
    {
        WrapedEditor.PropertiesChanged();
    }

    public float RangeProperty(MaterialProperty prop, string label)
    {
        return WrapedEditor.RangeProperty(prop, label);
    }

    public float RangeProperty(Rect position, MaterialProperty prop, string label)
    {
        return WrapedEditor.RangeProperty(position, prop, label);
    }

    public float FloatProperty(MaterialProperty prop, string label)
    {
        return WrapedEditor.FloatProperty(prop, label);
    }

    public float FloatProperty(Rect position, MaterialProperty prop, string label)
    {
        return WrapedEditor.FloatProperty(position, prop, label);
    }

    public Color ColorProperty(MaterialProperty prop, string label)
    {
        return WrapedEditor.ColorProperty(prop, label);
    }

    public Color ColorProperty(Rect position, MaterialProperty prop, string label)
    {
        return WrapedEditor.ColorProperty(position, prop, label);
    }

    public Vector4 VectorProperty(MaterialProperty prop, string label)
    {
        return WrapedEditor.VectorProperty(prop, label);
    }

    public Vector4 VectorProperty(Rect position, MaterialProperty prop, string label)
    {
        return WrapedEditor.VectorProperty(position, prop, label);
    }

    public void TextureScaleOffsetProperty(MaterialProperty property)
    {
        WrapedEditor.TextureScaleOffsetProperty(property);
    }

    public float TextureScaleOffsetProperty(Rect position, MaterialProperty property)
    {
        return WrapedEditor.TextureScaleOffsetProperty(position, property);
    }

    public float TextureScaleOffsetProperty(Rect position, MaterialProperty property, bool partOfTexturePropertyControl)
    {
        return WrapedEditor.TextureScaleOffsetProperty(position, property, partOfTexturePropertyControl);
    }

    public Texture TextureProperty(MaterialProperty prop, string label)
    {
        return WrapedEditor.TextureProperty(prop, label);
    }

    public Texture TextureProperty(MaterialProperty prop, string label, bool scaleOffset)
    {
        return WrapedEditor.TextureProperty(prop, label, scaleOffset);
    }

    public bool HelpBoxWithButton(GUIContent messageContent, GUIContent buttonContent)
    {
        return WrapedEditor.HelpBoxWithButton(messageContent, buttonContent);
    }

    public void TextureCompatibilityWarning(MaterialProperty prop)
    {
        WrapedEditor.TextureCompatibilityWarning(prop);
    }

    public Texture TexturePropertyMiniThumbnail(Rect position, MaterialProperty prop, string label, string tooltip)
    {
        return WrapedEditor.TexturePropertyMiniThumbnail(position, prop, label, tooltip);
    }

    public Rect GetTexturePropertyCustomArea(Rect position)
    {
        return WrapedEditor.GetTexturePropertyCustomArea(position);
    }

    public Texture TextureProperty(Rect position, MaterialProperty prop, string label)
    {
        return WrapedEditor.TextureProperty(position, prop, label);
    }

    public Texture TextureProperty(Rect position, MaterialProperty prop, string label, bool scaleOffset)
    {
        return WrapedEditor.TextureProperty(position, prop, label, scaleOffset);
    }

    public Texture TextureProperty(Rect position, MaterialProperty prop, string label, string tooltip, bool scaleOffset)
    {
        return WrapedEditor.TextureProperty(position, prop, label, tooltip, scaleOffset);
    }

    public float GetPropertyHeight(MaterialProperty prop)
    {
        return WrapedEditor.GetPropertyHeight(prop);
    }

    public float GetPropertyHeight(MaterialProperty prop, string label)
    {
        return WrapedEditor.GetPropertyHeight(prop, label);
    }

    public void BeginAnimatedCheck(MaterialProperty prop)
    {
        WrapedEditor.BeginAnimatedCheck(prop);
    }

    public void EndAnimatedCheck()
    {
        WrapedEditor.EndAnimatedCheck();
    }

    public void ShaderProperty(MaterialProperty prop, string label)
    {
        WrapedEditor.ShaderProperty(prop, label);
    }

    public void ShaderProperty(MaterialProperty prop, GUIContent label)
    {
        WrapedEditor.ShaderProperty(prop, label);
    }

    public void ShaderProperty(MaterialProperty prop, string label, int labelIndent)
    {
        WrapedEditor.ShaderProperty(prop, label, labelIndent);
    }

    public void ShaderProperty(MaterialProperty prop, GUIContent label, int labelIndent)
    {
        WrapedEditor.ShaderProperty(prop, label, labelIndent);
    }

    public void ShaderProperty(Rect position, MaterialProperty prop, string label)
    {
        WrapedEditor.ShaderProperty(position, prop, label);
    }

    public void ShaderProperty(Rect position, MaterialProperty prop, GUIContent label)
    {
        WrapedEditor.ShaderProperty(position, prop, label);
    }

    public void ShaderProperty(Rect position, MaterialProperty prop, string label, int labelIndent)
    {
        WrapedEditor.ShaderProperty(position, prop, label, labelIndent);
    }

    public void ShaderProperty(Rect position, MaterialProperty prop, GUIContent label, int labelIndent)
    {
        WrapedEditor.ShaderProperty(position, prop, label, labelIndent);
    }

    public void LightmapEmissionProperty()
    {
        WrapedEditor.LightmapEmissionProperty();
    }

    public void LightmapEmissionProperty(int labelIndent)
    {
        WrapedEditor.LightmapEmissionProperty(labelIndent);
    }

    public void LightmapEmissionProperty(Rect position, int labelIndent)
    {
        WrapedEditor.LightmapEmissionProperty(position, labelIndent);
    }

    public void DefaultShaderProperty(MaterialProperty prop, string label)
    {
        WrapedEditor.DefaultShaderProperty(prop, label);
    }

    public void DefaultShaderProperty(Rect position, MaterialProperty prop, string label)
    {
        WrapedEditor.DefaultShaderProperty(position, prop, label);
    }

    public float RangeProperty(string propertyName, string label, float v2, float v3)
    {
        return WrapedEditor.RangeProperty(propertyName, label, v2, v3);
    }

    public float FloatProperty(string propertyName, string label)
    {
        return WrapedEditor.FloatProperty(propertyName, label);
    }

    public Color ColorProperty(string propertyName, string label)
    {
        return WrapedEditor.ColorProperty(propertyName, label);
    }

    public Vector4 VectorProperty(string propertyName, string label)
    {
        return WrapedEditor.VectorProperty(propertyName, label);
    }

    public Texture TextureProperty(string propertyName, string label, ShaderUtil.ShaderPropertyTexDim texDim)
    {
        return WrapedEditor.TextureProperty(propertyName, label, texDim);
    }

    public Texture TextureProperty(string propertyName, string label, ShaderUtil.ShaderPropertyTexDim texDim, bool scaleOffset)
    {
        return WrapedEditor.TextureProperty(propertyName, label, texDim, scaleOffset);
    }

    public void ShaderProperty(Shader shader, int propertyIndex)
    {
        WrapedEditor.ShaderProperty(shader, propertyIndex);
    }

    public void SetDefaultGUIWidths()
    {
        WrapedEditor.SetDefaultGUIWidths();
    }

    public bool PropertiesGUI()
    {
        return WrapedEditor.PropertiesGUI();
    }

    public void PropertiesDefaultGUI(MaterialProperty[] props)
    {
        WrapedEditor.PropertiesDefaultGUI(props);
    }

    public void RegisterPropertyChangeUndo(string label)
    {
        WrapedEditor.RegisterPropertyChangeUndo(label);
    }

    public void OnPreviewSettings()
    {
        WrapedEditor.OnPreviewSettings();
    }

    public void DefaultPreviewSettingsGUI()
    {
        WrapedEditor.DefaultPreviewSettingsGUI();
    }

    public Texture2D RenderStaticPreview(string assetPath, Object[] subAssets, int width, int height)
    {
        return WrapedEditor.RenderStaticPreview(assetPath, subAssets, width, height);
    }

    public bool HasPreviewGUI()
    {
        return WrapedEditor.HasPreviewGUI();
    }

    public bool RequiresConstantRepaint()
    {
        return WrapedEditor.RequiresConstantRepaint();
    }

    public void OnInteractivePreviewGUI(Rect r, GUIStyle background)
    {
        WrapedEditor.OnInteractivePreviewGUI(r, background);
    }

    public void OnPreviewGUI(Rect r, GUIStyle background)
    {
        WrapedEditor.OnPreviewGUI(r, background);
    }

    public void DefaultPreviewGUI(Rect r, GUIStyle background)
    {
        WrapedEditor.DefaultPreviewGUI(r, background);
    }

    public void OnEnable()
    {
        WrapedEditor.OnEnable();
    }

    public void UndoRedoPerformed()
    {
        WrapedEditor.UndoRedoPerformed();
    }

    public void OnDisable()
    {
        WrapedEditor.OnDisable();
    }

    public void RenderQueueField()
    {
        WrapedEditor.RenderQueueField();
    }

    public void RenderQueueField(Rect r)
    {
        WrapedEditor.RenderQueueField(r);
    }

    public Rect TexturePropertySingleLine(GUIContent label, MaterialProperty textureProp)
    {
        return WrapedEditor.TexturePropertySingleLine(label, textureProp);
    }

    public Rect TexturePropertySingleLine(GUIContent label, MaterialProperty textureProp, MaterialProperty extraProperty1)
    {
        return WrapedEditor.TexturePropertySingleLine(label, textureProp, extraProperty1);
    }

    public Rect TexturePropertySingleLine(GUIContent label, MaterialProperty textureProp, MaterialProperty extraProperty1,
        MaterialProperty extraProperty2)
    {
        return WrapedEditor.TexturePropertySingleLine(label, textureProp, extraProperty1, extraProperty2);
    }

    public Rect TexturePropertyWithHDRColor(GUIContent label, MaterialProperty textureProp, MaterialProperty colorProperty,
        ColorPickerHDRConfig hdrConfig, bool showAlpha)
    {
        return WrapedEditor.TexturePropertyWithHDRColor(label, textureProp, colorProperty, hdrConfig, showAlpha);
    }

    public Rect TexturePropertyTwoLines(GUIContent label, MaterialProperty textureProp, MaterialProperty extraProperty1,
        GUIContent label2, MaterialProperty extraProperty2)
    {
        return WrapedEditor.TexturePropertyTwoLines(label, textureProp, extraProperty1, label2, extraProperty2);
    }

    public bool isVisible
    {
        get { return WrapedEditor.isVisible; }
    }
}

public static class ExtendedMaterialEditorHelper
{
    public static ExtendedMaterialEditor Extend(this MaterialEditor materialEditor)
    {
        return new ExtendedMaterialEditor(materialEditor);
    }
}