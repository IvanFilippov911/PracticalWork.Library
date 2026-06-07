namespace PracticalWork.Tests.Common;

public sealed class TestTimeProvider(DateTime utcNow) : TimeProvider
{
    private DateTimeOffset _utcNow = DateTime.SpecifyKind(utcNow, DateTimeKind.Utc);

    public override DateTimeOffset GetUtcNow() => _utcNow;

    public void SetUtcNow(DateTime utcNow)
    {
        _utcNow = DateTime.SpecifyKind(utcNow, DateTimeKind.Utc);
    }
}
