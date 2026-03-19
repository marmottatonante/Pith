namespace Keystone.Observables;

public interface IObservable
{
    event Action? Changing;
    event Action? Changed;
}

public interface IReadOnlyProperty<T> : IObservable
{
    T Value { get; }
}

public sealed class Property<T>(T initial) : IReadOnlyProperty<T>
{
    private T _value = initial;
    private Action? _unbind;

    public event Action? Changing;
    public event Action? Changed;

    public T Value
    {
        get => _value;
        set
        {
            if (_unbind is not null)
                throw new InvalidOperationException("Property is bound.");
            Set(value);
        }
    }

    private void Set(T value)
    {
        if (EqualityComparer<T>.Default.Equals(_value, value)) return;
        Changing?.Invoke();
        _value = value;
        Changed?.Invoke();
    }

    public void Bind(Func<T> compute, params IObservable[] sources)
    {
        Unbind();
        void handler() => Set(compute());
        foreach (var source in sources) source.Changed += handler;
        _unbind = () => { foreach (var source in sources) source.Changed -= handler; };
        handler();
    }

    public void Unbind()
    {
        _unbind?.Invoke();
        _unbind = null;
    }

    public IReadOnlyProperty<T> AsReadOnly() => this;
}