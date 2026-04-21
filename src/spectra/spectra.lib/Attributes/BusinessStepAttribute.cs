namespace spectra.lib.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public sealed class BusinessStepAttribute(string name) : Attribute
{
    public string Name { get; } = name;
    public string? Domain { get; set; }
    public string? Description { get; set; }
    public string? Risk { get; set; }
}
