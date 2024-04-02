namespace ArgsConfigReader; 

[AttributeUsage(AttributeTargets.Property)]
public class ConfigPropertyAttribute : Attribute{
    public bool solo { get; init; }
    public string? shortName { get; init; }
    public string? longName { get; init; }
    public string? defaultValue { get; init; }
    public string? description { get; init; }
    public string? overwriteFlag { get; init; }
    public string? overwriteValue { get; init; }
}