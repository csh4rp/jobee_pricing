using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Jobee.Pricing.Domain.ValueObjects;

public record ProductVersion
{
    public Guid Id { get; private init; }
    
    public long Number { get; private set; }

    public ProductVersion(Guid id, long number)
    {
        Id = id;
        Number = number;
    }
    
    public static bool TryParse(string value, [NotNullWhen(true)] out ProductVersion? productVersion)
    {
        productVersion = null;

        if (string.IsNullOrEmpty(value))
        {
            return false;
        }
        
        try
        {
            var bytes = Convert.FromBase64String(value);
            var parts = Encoding.UTF8.GetString(bytes).Split(':');
            
            if (parts.Length != 2)
            {
                return false;
            }
            
            var id = Guid.Parse(parts[0]);
            var version = int.Parse(parts[1]);
            
            productVersion = new ProductVersion(id, version);
            
            return true;
        }
        catch
        {
            return false;
        }
    }

    public override string ToString()
    {
        var bytes = Encoding.UTF8.GetBytes($"{Id:N}:{Number:0000000}");
        return Convert.ToBase64String(bytes);
    }

    public void Deconstruct(out Guid id, out long version)
    {
        id = Id;
        version = Number;
    }
}