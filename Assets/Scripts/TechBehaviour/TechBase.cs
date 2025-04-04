
public abstract class TechBase
{
    public PlayerEntity.TechGroup Group { get; set; }
    public PlayerEntity PlayerRef { get; set; }

    public abstract void OnTechAdded();
    public abstract void OnTechUpgraded();
    public abstract void OnTechRemoved();
}
