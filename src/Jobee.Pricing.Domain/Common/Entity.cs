namespace Jobee.Pricing.Domain.Common;

public abstract class Entity<TId>
{
    public TId Id { get; protected set; } = default!;
    
    private readonly Queue<object> _events = [];
    
    protected void EnqueueEvent(object @event) => _events.Enqueue(@event);
    
    public IEnumerable<object> DequeueEvents()
    {
        while (_events.Count > 0)
        {
            yield return _events.Dequeue();
        }
    }
}