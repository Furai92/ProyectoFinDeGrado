
public abstract class TechBase
{
    public int Level { get; protected set; }

    public abstract void OnTechAdded();
    public abstract void OnTechUpgraded();
    public abstract void OnTechRemoved();
}
