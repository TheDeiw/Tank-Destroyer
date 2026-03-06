using System;

public enum BulletEffectType
{
    None,
    DamageModification,
    SpeedModification,
}

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
public sealed class BulletEffectAttribute : Attribute
{
    public BulletEffectType EffectType { get; }
    public string Description { get; }
    public int Priority { get; set; }

    public BulletEffectAttribute(BulletEffectType effectType, string description)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            throw new ArgumentNullException(nameof(description), "Description cannot be null or whitespace.");
        }
        EffectType = effectType;
        Description = description;
        Priority = 0; // Default priority
    }
}
