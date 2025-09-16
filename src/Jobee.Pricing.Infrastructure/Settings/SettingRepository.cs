using Jobee.Pricing.Domain.Settings;
using Marten;

namespace Jobee.Pricing.Infrastructure.Settings;

public class SettingRepository : ISettingRepository
{
    private readonly IDocumentStore _documentStore;

    public SettingRepository(IDocumentStore documentStore)
    {
        _documentStore = documentStore;
    }

    public async Task<Setting?> FindSettingAsync(string name, CancellationToken cancellationToken)
    {
        await using var session = _documentStore.LightweightSession();
        return await session.Query<Setting>()
            .FirstOrDefaultAsync(s => s.Name == name, cancellationToken);
    }
}