namespace spectra.lib.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public sealed class BusinessStepAttribute(string name) : Attribute
{
    public string Name { get; } =
        string.IsNullOrWhiteSpace(name)
            ? throw new ArgumentException("Business step name cannot be null or whitespace.", nameof(name))
            : name;
    public string? Domain { get; set; }
    public string? Description { get; set; }
    public string? Risk { get; set; }
}
