namespace Jobee.Pricing.Domain.Settings;

public class Setting
{
    public string Name { get; private set; } = null!;

    public string Value { get; private set; } = null!;

    private Setting()
    {
    }
    
    public Setting(string name, string value)
    {
        Name = name;
        Value = value;
    }
}