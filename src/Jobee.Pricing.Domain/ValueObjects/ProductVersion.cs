using System.Text;

namespace Jobee.Pricing.Domain.ValueObjects;

public record ProductVersion
{
    public Guid Id { get; private set; }
    public int Number { get; private set; }
    
    public ProductVersion(Guid id, int number)
    {
        Id = id;
        Number = number;
    }

    public ProductVersion(string value)
    {
        var bytes = Convert.FromBase64String(value);
        var stringValue = Encoding.UTF8.GetString(bytes);
        var parts = stringValue.Split('-');
        if (parts.Length != 2 || !Guid.TryParse(parts[0], out var id) || !int.TryParse(parts[1], out var version))
        {
            throw new ArgumentException("Invalid ProductVersion format", nameof(value));
        }
        
        Id = id;
        Number = version;
    }

    public override string ToString()
    {
        var stringValue = $"{Id:N}-{Number:D6}";
        var bytes = Encoding.UTF8.GetBytes(stringValue);
        return Convert.ToBase64String(bytes);
    }
}