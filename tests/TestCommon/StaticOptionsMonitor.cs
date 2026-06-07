using Microsoft.Extensions.Options;

namespace PracticalWork.Tests.Common;

public sealed class StaticOptionsMonitor<T>(T currentValue) : IOptionsMonitor<T>
{
    public T CurrentValue { get; } = currentValue;

    public T Get(string? name) => CurrentValue;

    public IDisposable OnChange(Action<T, string?> listener) => NoopDisposable.Instance;

    private sealed class NoopDisposable : IDisposable
    {
        public static readonly NoopDisposable Instance = new();

        public void Dispose()
        {
        }
    }
}
