using JetBrains.Annotations;

[UsedImplicitly]
public abstract class ExtendedMaterialPropertyDecorator : ExtendedMaterialPropertyAspect
{
    public abstract void ExtendedOnGUI();

    public virtual float GetPropertyHeight()
    {
        return 0f;
    }
}